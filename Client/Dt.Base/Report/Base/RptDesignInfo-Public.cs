#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-25 创建
**************************************************************************/
#endregion

#region 命名空间
using System.Threading.Tasks;
using Windows.Storage;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 报表设计的描述信息
    /// </summary>
    public partial class RptDesignInfo : RptInfoBase
    {
        /// <summary>
        /// 是否显示新文件菜单项，默认false
        /// </summary>
        public bool ShowNewFile { get; set; }
        
        /// <summary>
        /// 是否显示打开文件菜单项，默认false
        /// </summary>
        public bool ShowOpenFile { get; set; }

        /// <summary>
        /// 是否显示保存菜单项，默认false
        /// </summary>
        public bool ShowSave { get; set; }
        
        /// <summary>
        /// 保存模板内容
        /// </summary>
        /// <param name="p_xml"></param>
        /// <returns></returns>
        public virtual async Task<bool> SaveTemplate(string p_xml)
        {
            if (_curFile == null)
            {
                var filePicker = Kit.GetFileSavePicker();
                filePicker.FileTypeChoices.Add("报表模板", new List<string>(new string[] { ".rpt" }));
                filePicker.SuggestedFileName = "新模板";
                var sf = await filePicker.PickSaveFileAsync();
                if (sf != null)
                {
                    using (var stream = await sf.OpenStreamForWriteAsync())
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.SetLength(0);
                        using (var sw = new StreamWriter(stream))
                        {
                            await sw.WriteAsync(Rpt.SerializeTemplate(Root));
                            Kit.Msg("保存成功！");
                        }
                    }
                    _curFile = sf;
                }
            }
            else
            {
                using (var stream = await _curFile.OpenStreamForWriteAsync())
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.SetLength(0);
                    using (var sw = new StreamWriter(stream))
                    {
                        await sw.WriteAsync(Rpt.SerializeTemplate(Root));
                        await sw.FlushAsync();
                        Kit.Msg("保存成功！");
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 创建新模板
        /// </summary>
        /// <returns></returns>
        public async Task CreateNew()
        {
            if (IsDirty)
            {
                if (await Kit.Confirm("当前模板已修改，保存吗？"))
                {
                    await SaveTemplate();
                }
                else if (!await Kit.Confirm("创建新模板会丢失修改内容，继续创建吗？"))
                {
                    return;
                }
            }

            _curFile = null;
            await ImportTemplate(null);
        }

        /// <summary>
        /// 打开模板
        /// </summary>
        /// <returns></returns>
        public async Task OpenFile()
        {
            if (IsDirty && !await Kit.Confirm("当前模板已修改，打开新模板会丢失修改内容，继续打开吗？"))
                return;

            _curFile = await OpenLoadTemplate();
            if (_curFile != null)
                Name = _curFile.Path;
        }

        /// <summary>
        /// 导入新模板
        /// </summary>
        /// <returns></returns>
        public async Task ImportFile()
        {
            if (IsDirty && !await Kit.Confirm("当前模板已修改，导入新模板会丢失修改内容，继续导入吗？"))
                return;

            await OpenLoadTemplate();
        }

        async Task<StorageFile> OpenLoadTemplate()
        {
            var picker = Kit.GetFileOpenPicker();
            picker.FileTypeFilter.Add(".rpt");
            picker.FileTypeFilter.Add(".xml");
            var sf = await picker.PickSingleFileAsync();
            if (sf != null)
            {
                using (var stream = await sf.OpenStreamForReadAsync())
                using (var reader = new StreamReader(stream))
                {
                    await ImportTemplate(reader.ReadToEnd());
                }
            }
            return sf;
        }
    }
}