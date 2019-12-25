#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Devices.Enumeration.Pnp;
using Windows.System;
#endregion

namespace Dt.Charts
{
    internal class SystemInfoEstimate
    {
        const string DeviceClassKey = "{A45C254E-DF1C-4EFD-8020-67D146A850E0},10";
        const string DeviceDriverVersionKey = "{A8B865DD-2E3D-4094-AD97-E593A70C75D6},3";
        const string HalDeviceClass = "4d36e966-e325-11ce-bfc1-08002be10318";
        const string ItemNameKey = "System.ItemNameDisplay";
        const string ManufacturerKey = "System.Devices.Manufacturer";
        const string ModelNameKey = "System.Devices.ModelName";
        const string PrimaryCategoryKey = "{78C34FC8-104A-4ACA-9EA4-524D52996E57},97";
        const string RootContainer = "{00000000-0000-0000-FFFF-FFFFFFFFFFFF}";
        const string RootQuery = "System.Devices.ContainerId:=\"{00000000-0000-0000-FFFF-FFFFFFFFFFFF}\"";

        public static Task<string> GetDeviceCategoryAsync()
        {
            return GetRootDeviceInfoAsync("{78C34FC8-104A-4ACA-9EA4-524D52996E57},97");
        }

        public static Task<string> GetDeviceManufacturerAsync()
        {
            return GetRootDeviceInfoAsync("System.Devices.Manufacturer");
        }

        public static Task<string> GetDeviceModelAsync()
        {
            return GetRootDeviceInfoAsync("System.Devices.ModelName");
        }

        /// <summary>
        /// hdt
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        static Task<PnpObject> GetHalDevice(params string[] properties)
        {
            PnpObject result = null;
            var actualProperties = Enumerable.Concat<string>(properties, new string[] { "{A45C254E-DF1C-4EFD-8020-67D146A850E0},10" });
            PnpObjectCollection objects = WindowsRuntimeSystemExtensions.GetAwaiter<PnpObjectCollection>(PnpObject.FindAllAsync(PnpObjectType.Device, actualProperties, "System.Devices.ContainerId:=\"{00000000-0000-0000-FFFF-FFFFFFFFFFFF}\"")).GetResult();
            foreach (PnpObject obj in objects)
            {
                KeyValuePair<string, object> pair = Enumerable.Last<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>) obj.Properties);
                if ((pair.Value != null) && pair.Value.ToString().Equals("4d36e966-e325-11ce-bfc1-08002be10318"))
                {
                    result = obj;
                    break;
                }
            }
            return Task.FromResult<PnpObject>(result);
        }

        /// <summary>
        /// hdt
        /// </summary>
        /// <returns></returns>
        public static Task<ProcessorArchitecture> GetProcessorArchitectureAsync()
        {
            TaskAwaiter<PnpObject> awaiter = SystemInfoEstimate.GetHalDevice(new string[] { "System.ItemNameDisplay" }).GetAwaiter();
            PnpObject halDevice = awaiter.GetResult();
            ProcessorArchitecture arm;
            if(halDevice != null && halDevice.Properties["System.ItemNameDisplay"] != null)
            {
                string str = halDevice.Properties["System.ItemNameDisplay"].ToString();
                if (str.Contains("x64"))
                {
                    arm = ProcessorArchitecture.X64;
                }
                else if (str.Contains("ARM"))
                {
                    arm = ProcessorArchitecture.Arm;
                }
                else
                {
                    arm = ProcessorArchitecture.X86;
                }
            }
            else
            {
                arm = ProcessorArchitecture.Unknown;
            }
            return Task.FromResult<ProcessorArchitecture>(arm);
        }

        /// <summary>
        /// hdt
        /// </summary>
        /// <param name="propertyKey"></param>
        /// <returns></returns>
        static Task<string> GetRootDeviceInfoAsync(string propertyKey)
        {
            TaskAwaiter<PnpObject> awaiter;
            awaiter = WindowsRuntimeSystemExtensions.GetAwaiter<PnpObject>(PnpObject.CreateFromIdAsync(PnpObjectType.DeviceContainer, "{00000000-0000-0000-FFFF-FFFFFFFFFFFF}", new string[] { propertyKey }));
            PnpObject pnp = awaiter.GetResult();
            string str = pnp.Properties[propertyKey].ToString();
            return Task.FromResult<string>(str);
        }

        /// <summary>
        /// hdt
        /// </summary>
        /// <returns></returns>
        public static Task<string> GetWindowsVersionAsync()
        {
            TaskAwaiter<PnpObject> awaiter;
            awaiter = SystemInfoEstimate.GetHalDevice(new string[] { "{A8B865DD-2E3D-4094-AD97-E593A70C75D6},3" }).GetAwaiter();
            PnpObject hal = awaiter.GetResult();
            string str;
            string[] versionParts;
            if(hal == null || hal.Properties.ContainsKey("{A8B865DD-2E3D-4094-AD97-E593A70C75D6},3"))
            {
                str = null;
            }
            else
            {
                versionParts = hal.Properties["{A8B865DD-2E3D-4094-AD97-E593A70C75D6},3"].ToString().Split(new char[] { '.' });
                str = string.Join(".", Enumerable.ToArray<string>(Enumerable.Take<string>(versionParts, 2)));
            }
            return Task.FromResult<string>(str);
        }   
    }
}

