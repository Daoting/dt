﻿using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

                    //replacementsDictionary["$UseWebAssembly$"] = _useWebAssembly.ToString();
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

            try
            {
                RemoveAndSetStartup();
                CopyStubFile();
                ShowWelcomePage();
            }
            catch { }
        }

        void RemoveAndSetStartup()
        {
            Project folder = _dte.Solution.Projects.OfType<Project>().FirstOrDefault((Project p) => p.Name == "App");
            if (folder == null)
                return;

            if (!_useWebAssembly)
                GetSubPrj(folder, ".Wasm")?.Delete();

            if (!_useiOS)
                GetSubPrj(folder, ".iOS")?.Delete();

            if (!_useAndroid)
                GetSubPrj(folder, ".Droid")?.Delete();

            if (_useSvcType == SvcType.None)
                GetSubPrj(folder, ".Svc")?.Delete();

            // 设置启动项目
            var prj = GetSubPrj(folder, ".Win");
            var sln = _dte.Solution.SolutionBuild as SolutionBuild2;
            if (sln != null && prj != null)
            {
                sln.StartupProjects = prj.UniqueName;
                var cfg = sln.SolutionConfigurations.Cast<SolutionConfiguration2>().FirstOrDefault((SolutionConfiguration2 c) => c.Name == "Debug" && c.PlatformName == "x86");
                if (cfg != null)
                {
                    cfg.Activate();
                }
            }
        }

        void CopyStubFile()
        {
            try
            {
                var stubPath = Path.Combine(_targetPath, _projectName + ".Client", "Stub");
                var optionsPath = Path.Combine(stubPath, "Options");
                var stubFile = Path.Combine(stubPath, "AppStub.cs");

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

        Project GetSubPrj(Project p_folder, string p_ends)
        {
            for (int i = 1; i <= p_folder.ProjectItems.Count; i++)
            {
                Project subPrj = p_folder.ProjectItems.Item(i).SubProject;
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