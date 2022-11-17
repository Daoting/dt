using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Dt.PrjWizard
{
    public class DtWizard : IWizard
    {
        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            _targetPath = replacementsDictionary["$destinationdirectory$"];
            _projectName = replacementsDictionary["$projectname$"];
            ValidateProjectName();

            if (runKind == WizardRunKind.AsMultiProject)
                _dte = (DTE2)automationObject;

            using (OptionsForm form = new OptionsForm())
            {
                DialogResult result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    _useWebAssembly = form.UseWebAssembly;
                    _useiOS = form.UseiOS;
                    _useAndroid = form.UseAndroid;
                    _useSvcType = form.UseSvcType;

                    //replacementsDictionary["$issingletonsvc$"] = (_useSvcType == SvcType.CustomSvc) ? "false" : "true";
                }
                else
                {
                    if (result != DialogResult.Abort)
                    {
                        throw new WizardBackoutException();
                    }
                    MessageBox.Show("Aborted");
                    throw new WizardCancelledException();
                }
            }
        }

        public void RunFinished()
        {
            if (_dte == null)
                return;

            _appFolder = _dte.Solution.Projects.OfType<Project>().FirstOrDefault((Project p) => p.Name == "App");

            ShowWelcomePage();
            AdjustClientPrj();
            AdjustSvcFiles();
            RemoveUnselectPrj();
            SetStartupPrj();
        }

        void AdjustClientPrj()
        {
            try
            {
                // Client\Stub下的文件
                var stubPath = Path.Combine(_targetPath, _projectName + ".Client", "Stub");
                var optionsPath = Path.Combine(stubPath, "Options");
                var stubFile = Path.Combine(stubPath, "AppStub.cs");

                // Common.props复制到根目录
                File.Copy(Path.Combine(optionsPath, "Common.props"), Path.Combine(_targetPath, "Common.props"));
                if (_useSvcType == SvcType.DtSvc)
                {
                    File.Copy(Path.Combine(optionsPath, "AppStub-Lob.cs"), stubFile);
                    File.Delete(Path.Combine(stubPath, "RpcConfig.cs"));
                }
                else if (_useSvcType == SvcType.CustomSvc)
                {
                    File.Copy(Path.Combine(optionsPath, "AppStub-Custom.cs"), stubFile);
                    File.Delete(Path.Combine(stubPath, "PushApi.cs"));
                }
                else
                {
                    File.Copy(Path.Combine(optionsPath, "AppStub-Single.cs"), stubFile);
                    File.Delete(Path.Combine(_targetPath, _projectName + ".Client", "Agent", "AtSvc.cs"));
                    File.Delete(Path.Combine(stubPath, "PushApi.cs"));
                    File.Delete(Path.Combine(stubPath, "RpcConfig.cs"));
                }
                Directory.Delete(optionsPath, true);
            }
            catch { }

            /* 动态替换TargetFrameworks占位符造成初次无法编译！很多方法都无法解决！！
            try
            {
                // Client项目支持的平台
                string target = "";
                if (_useAndroid)
                    target = ";net6.0-android";
                if (_useiOS)
                    target += ";net6.0-ios";
                if (_useWebAssembly)
                    target += ";net6.0";

                // 替换占位符
                using (var fs = File.Open(Path.Combine(_targetPath, "Targets.props"), FileMode.Open, FileAccess.ReadWrite))
                using (var sr = new StreamReader(fs))
                {
                    var str = sr.ReadToEnd().Replace("$targetframeworks$", target);
                    var data = Encoding.UTF8.GetBytes(str);
                    fs.SetLength(0);
                    fs.Write(data, 0, data.Length);
                }

                // 动态替换TargetFrameworks占位符造成初次无法编译！删除Test项目使依赖它的项目重新加载！
                var prj = _dte.Solution.Projects.OfType<Project>().FirstOrDefault((Project p) => p.Name.EndsWith(".Test"));
                prj.Delete();
                Directory.Delete(Path.Combine(_targetPath, _projectName + ".Test"), true);

                var cli = _dte.Solution.Projects.OfType<Project>().FirstOrDefault((Project p) => p.Name.EndsWith(".Client"));
                cli.Save();
            }
            catch { }
            */
        }

        void AdjustSvcFiles()
        {
            try
            {
                using (var fs = File.Open(Path.Combine(_targetPath, _projectName + ".Svc", "Program.cs"), FileMode.Open, FileAccess.ReadWrite))
                using (var sr = new StreamReader(fs))
                {
                    var str = sr.ReadToEnd().Replace("$issingletonsvc$", (_useSvcType == SvcType.CustomSvc) ? "false" : "true");
                    var data = Encoding.UTF8.GetBytes(str);
                    fs.SetLength(0);
                    fs.Write(data, 0, data.Length);
                }
            }
            catch { }
        }

        void RemoveUnselectPrj()
        {
            try
            {
                if (!_useAndroid)
                {
                    GetAppSubPrj(".Droid")?.Delete();
                    Directory.Delete(Path.Combine(_targetPath, _projectName + ".Droid"), true);
                }
                
                if (!_useiOS)
                {
                    GetAppSubPrj(".iOS")?.Delete();
                    Directory.Delete(Path.Combine(_targetPath, _projectName + ".iOS"), true);
                }
                
                if (!_useWebAssembly)
                {
                    GetAppSubPrj(".Wasm")?.Delete();
                    Directory.Delete(Path.Combine(_targetPath, _projectName + ".Wasm"), true);
                }

                // 删除单机版的服务
                if (_useSvcType == SvcType.None)
                {
                    var svc = _dte.Solution.Projects.OfType<Project>().FirstOrDefault((Project p) => p.Name.EndsWith(".Svc"));
                    if (svc != null)
                    {
                        svc.Delete();
                        Directory.Delete(Path.Combine(_targetPath, _projectName + ".Svc"), true);
                    }
                }
            }
            catch { }
        }

        void SetStartupPrj()
        {
            try
            {
                var sln = _dte.Solution.SolutionBuild as SolutionBuild2;
                if (sln == null)
                    return;

                // 设置活动配置
                var cfg = sln.SolutionConfigurations.Cast<SolutionConfiguration2>().FirstOrDefault((SolutionConfiguration2 c) => c.Name == "Debug" && c.PlatformName == "x86");
                if (cfg != null)
                {
                    cfg.Activate();
                }

                // 设置启动项目
                var prj = _dte.Solution.Projects.OfType<Project>().FirstOrDefault((Project p) => p.Name.EndsWith(".Svc"));
                if (prj == null)
                    prj = GetAppSubPrj(".Win");
                if (prj != null)
                    sln.StartupProjects = prj.UniqueName;
            }
            catch { }
        }

        void ShowWelcomePage()
        {
            // 欢迎页
            var path = Path.Combine(_targetPath, _projectName + ".Client", "Stub", "Readme.txt");
            if (File.Exists(path))
                _dte.ItemOperations.OpenFile(path);

            // 异常，托管调试助手 "InvalidVariant":
            //_dte.ItemOperations.Navigate("https://github.com/daoting/dt");
        }

        Project _appFolder;

        Project GetAppSubPrj(string p_ends)
        {
            for (int i = 1; i <= _appFolder.ProjectItems.Count; i++)
            {
                Project subPrj = _appFolder.ProjectItems.Item(i).SubProject;
                if (subPrj != null && subPrj.Name.EndsWith(p_ends))
                {
                    return subPrj;
                }
            }
            return null;
        }

        public void ProjectFinishedGenerating(Project project)
        {
        }

        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }

        void ValidateProjectName()
        {
            if (_projectName != null)
            {
                if (_projectName.Contains(" "))
                {
                    MessageBox.Show("项目名称不能包含空格", "创建项目出错");
                    throw new WizardBackoutException();
                }
                if (_projectName.Contains("-"))
                {
                    MessageBox.Show("项目名称不能包含'-'", "创建项目出错");
                    throw new WizardBackoutException();
                }
                if (char.IsDigit(_projectName.First<char>()))
                {
                    MessageBox.Show("项目名称不能以数字开头", "创建项目出错");
                    throw new WizardBackoutException();
                }
            }
        }

        string _targetPath;
        string _projectName;
        DTE2 _dte;
        bool _useWebAssembly;
        bool _useiOS;
        bool _useAndroid;
        SvcType _useSvcType;
    }
}