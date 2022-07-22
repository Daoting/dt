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
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.OOXml
{
    internal class XFile
    {
        private string _fileName;
        private string _fileType;
        private Dictionary<string, XFile> _relationFiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XFile" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="fileType">Type of the file</param>
        public XFile(string fileName, string fileType)
        {
            this._fileName = fileName;
            this._fileType = fileType;
        }

        /// <summary>
        /// Adds the relation file.
        /// </summary>
        /// <param name="file">file</param>
        /// <returns>relation id.</returns>
        public string AddRelationFile(XFile file)
        {
            if (file == null)
            {
                return string.Empty;
            }
            int num = this.RelationFiles.Count + 1;
            string str = "rId" + ((int) num).ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
            if (this.RelationFiles.ContainsKey(str))
            {
                this.RelationFiles[str] = file;
                return str;
            }
            this.RelationFiles.Add(str, file);
            return str;
        }

        /// <summary>
        /// Fixes the name of the file.
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <returns>file name.</returns>
        public static string FixFileName(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                if (fileName.IndexOf("..") < 0)
                {
                    return fileName;
                }
                string[] strArray = fileName.Split(@"\/".ToCharArray());
                List<object> list = new List<object>();
                for (int i = 0; i < strArray.Length; i++)
                {
                    if (strArray[i].IndexOf("..") >= 0)
                    {
                        if (list.Count > 0)
                        {
                            list.RemoveAt(list.Count - 1);
                        }
                    }
                    else
                    {
                        list.Add(strArray[i]);
                    }
                }
                fileName = "";
                foreach (string str in list)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        fileName = fileName + @"\" + str;
                    }
                }
            }
            return fileName;
        }

        /// <summary>
        /// Gets the relation file by its name.
        /// </summary>
        /// <param name="name">The file name.</param>
        /// <returns>Returns the file.</returns>
        public XFile GetFileByName(string name)
        {
            if (((this._relationFiles != null) && (this._relationFiles.Count != 0)) && !string.IsNullOrEmpty(name))
            {
                foreach (XFile file in this._relationFiles.Values)
                {
                    if (file.FileName.Replace('/', '\\').Equals(name))
                    {
                        return file;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the file by relation ID.
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Instance of XFile</returns>
        public XFile GetFileByRelationID(string id)
        {
            if (((this._relationFiles != null) && (this._relationFiles.Count != 0)) && !string.IsNullOrWhiteSpace(id))
            {
                return this._relationFiles[id];
            }
            return null;
        }

        /// <summary>
        /// Gets the type of the file by.
        /// </summary>
        /// <param name="type">type</param>
        /// <returns></returns>
        public XFile GetFileByType(string type)
        {
            if (((this._relationFiles != null) && (this._relationFiles.Count != 0)) && !string.IsNullOrEmpty(type))
            {
                foreach (XFile file in this._relationFiles.Values)
                {
                    if (file.FileType.Equals(type))
                    {
                        return file;
                    }
                }
            }
            return null;
        }

        public static List<Dt.Xls.OOXml.Relationship> GetRelationshipsByBaseName(string baseName, MemoryFolder mFolder)
        {
            if ((baseName == null) || (mFolder == null))
            {
                return null;
            }
            string relationshipsNameByBaseName = GetRelationshipsNameByBaseName(baseName);
            if (relationshipsNameByBaseName == null)
            {
                return null;
            }
            return ReadRelationships(mFolder.GetFile(relationshipsNameByBaseName));
        }

        public static string GetRelationshipsNameByBaseName(string baseName)
        {
            try
            {
                if (baseName == null)
                {
                    return string.Empty;
                }
                if (baseName.Length == 0)
                {
                    return @"_rels\.rels";
                }
                return (Path.GetDirectoryName(baseName) + @"\_rels\" + Path.GetFileName(baseName) + ".rels");
            }
            catch
            {
                return null;
            }
        }

        private static bool IsAbsolutePath(string fileName)
        {
            return (fileName.IndexOf("file:///", (StringComparison) StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public void LoadPackageRelationFiles(MemoryFolder mFolder)
        {
            if (mFolder != null)
            {
                List<Dt.Xls.OOXml.Relationship> relationshipsByBaseName = GetRelationshipsByBaseName(this.FileName, mFolder);
                if (relationshipsByBaseName != null)
                {
                    foreach (Dt.Xls.OOXml.Relationship relationship in relationshipsByBaseName)
                    {
                        string target = relationship.Target;
                        if ((target.IndexOf('/') != 0) && !IsAbsolutePath(target))
                        {
                            target = (((this.FileName == null) || (this.FileName.Length == 0)) ? "" : (Path.GetDirectoryName(this.FileName) + @"\")) + target;
                        }
                        target = FixFileName(target);
                        XFile file = new XFile(target, relationship.Type) {
                            Relationship = relationship
                        };
                        if (!IsAbsolutePath(target))
                        {
                            file.LoadPackageRelationFiles(mFolder);
                        }
                        if (this.RelationFiles.ContainsKey(relationship.Id))
                        {
                            this.RelationFiles[relationship.Id] = file;
                        }
                        else
                        {
                            this.RelationFiles.Add(relationship.Id, file);
                        }
                    }
                }
            }
        }

        private static List<Dt.Xls.OOXml.Relationship> ReadRelationships(Stream stream)
        {
            List<Dt.Xls.OOXml.Relationship> list = new List<Dt.Xls.OOXml.Relationship>();
            try
            {
                if (stream == null)
                {
                    return list;
                }
                stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                foreach (XNode node in XElement.Load(stream).Nodes())
                {
                    Dt.Xls.OOXml.Relationship relationship = new Dt.Xls.OOXml.Relationship();
                    XElement element2 = node as XElement;
                    if (element2 != null)
                    {
                        if (element2.Attribute("Id") != null)
                        {
                            relationship.Id = element2.Attribute("Id").Value;
                        }
                        if (element2.Attribute("Target") != null)
                        {
                            relationship.Target = element2.Attribute("Target").Value;
                        }
                        if (element2.Attribute("TargetMode") != null)
                        {
                            relationship.TargetMode = element2.Attribute("TargetMode").Value;
                        }
                        if (element2.Attribute("Type") != null)
                        {
                            relationship.Type = element2.Attribute("Type").Value;
                        }
                        if (element2.Attribute("Value") != null)
                        {
                            relationship.Value = element2.Attribute("Value").Value;
                        }
                        list.Add(relationship);
                    }
                }
            }
            catch
            {
            }
            return list;
        }

        public void SavePackageRelationFiles(MemoryFolder mFolder)
        {
            SavePackageRelationFiles(this, mFolder);
        }

        private static void SavePackageRelationFiles(XFile xFile, MemoryFolder mFolder)
        {
            if (((xFile != null) && (xFile.RelationFiles != null)) && ((xFile.RelationFiles.Count != 0) && (mFolder != null)))
            {
                string str = string.IsNullOrEmpty(xFile.FileName) ? "" : Path.GetDirectoryName(xFile.FileName);
                string str2 = string.IsNullOrEmpty(xFile.FileName) ? "" : Path.GetFileName(xFile.FileName);
                Dictionary<string, string[]> htRelationShip = new Dictionary<string, string[]>();
                foreach (string str3 in xFile.RelationFiles.Keys)
                {
                    XFile file = xFile.RelationFiles[str3];
                    if (file != null)
                    {
                        string[] strArray = new string[2];
                        if (!string.IsNullOrWhiteSpace(file.Target))
                        {
                            strArray[0] = file.Target;
                        }
                        else
                        {
                            strArray[0] = file.FileName.Replace('\\', '/');
                        }
                        strArray[1] = file.FileType;
                        if (htRelationShip.ContainsKey(str3))
                        {
                            htRelationShip[str3] = strArray;
                        }
                        else
                        {
                            htRelationShip.Add(str3, strArray);
                        }
                    }
                    SavePackageRelationFiles(file, mFolder);
                }
                CT_Relationships relationships = ToRelationships(htRelationShip);
                if (relationships != null)
                {
                    mFolder.CreateMemoryFile(str + @"\_rels\" + str2 + ".rels", PackageXml.CreateStreamFromObject(relationships, typeof(CT_Relationships)));
                }
            }
        }

        private static CT_Relationships ToRelationships(Dictionary<string, string[]> htRelationShip)
        {
            if (htRelationShip == null)
            {
                return null;
            }
            CT_Relationships relationships = new CT_Relationships {
                Relationship = new Dt.Xls.OOXml.Relationship[htRelationShip.Count]
            };
            int index = 0;
            foreach (string str in htRelationShip.Keys)
            {
                string[] strArray = htRelationShip[str];
                if ((strArray != null) && (strArray.Length >= 2))
                {
                    Dt.Xls.OOXml.Relationship relationship = new Dt.Xls.OOXml.Relationship {
                        Id = str,
                        Target = strArray[0],
                        Type = strArray[1]
                    };
                    if (((!relationship.Target.EndsWith(".xml") && !relationship.Target.EndsWith("png")) && (!relationship.Target.EndsWith("jpg") && !relationship.Target.EndsWith("bmp"))) && ((!relationship.Target.EndsWith("gif") && !relationship.Target.EndsWith("vml")) && (!relationship.Target.EndsWith("emf") && (relationship.Type != "http://schemas.microsoft.com/office/2006/relationships/vbaProject"))))
                    {
                        relationship.TargetMode = "External";
                    }
                    relationships.Relationship[index] = relationship;
                }
                index++;
            }
            return relationships;
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName
        {
            get { return  this._fileName; }
        }

        /// <summary>
        /// Gets the type of the file.
        /// </summary>
        /// <value>The type of the file.</value>
        public string FileType
        {
            get { return  this._fileType; }
        }

        /// <summary>
        /// Gets the relation files.
        /// </summary>
        /// <value>The relation files.</value>
        public Dictionary<string, XFile> RelationFiles
        {
            get
            {
                if (this._relationFiles == null)
                {
                    this._relationFiles = new Dictionary<string, XFile>();
                }
                return this._relationFiles;
            }
        }

        public Dt.Xls.OOXml.Relationship Relationship { get; set; }

        internal string Target { get; set; }

        internal string TargetMode { get; set; }
    }
}

