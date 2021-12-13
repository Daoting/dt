#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using Dt.Xls.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
#endregion

namespace Dt.Xls.OOXml
{
    internal class PackageXml
    {
        private static string ConvertRelationshipType2ContentType(string relationType, ExcelFileType workbookType)
        {
            if (!string.IsNullOrEmpty(relationType))
            {
                switch (relationType)
                {
                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings":
                        return "application/vnd.openxmlformats-officedocument.spreadsheetml.sharedStrings+xml";

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles":
                        return "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml";

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme":
                        return "application/vnd.openxmlformats-officedocument.theme+xml";

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument":
                        if (workbookType != ExcelFileType.XLSM)
                        {
                            return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml";
                        }
                        return "application/vnd.ms-excel.sheet.macroEnabled.main+xml";

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet":
                        return "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml";

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/externalLink":
                        return "application/vnd.openxmlformats-officedocument.spreadsheetml.externalLink+xml";

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/table":
                        return "application/vnd.openxmlformats-officedocument.spreadsheetml.table+xml";

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing":
                        return "application/vnd.openxmlformats-officedocument.drawing+xml";

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/chart":
                        return "application/vnd.openxmlformats-officedocument.drawingml.chart+xml";

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/diagramColors":
                        return "application/vnd.openxmlformats-officedocument.drawingml.diagramColors+xml";

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/diagramData":
                        return "application/vnd.openxmlformats-officedocument.drawingml.diagramData+xml";

                    case "http://schemas.microsoft.com/office/2007/relationships/diagramDrawing":
                        return "application/vnd.ms-office.drawingml.diagramDrawing+xml";

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/diagramLayout":
                        return "application/vnd.openxmlformats-officedocument.drawingml.diagramLayout+xml";

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/diagramQuickStyle":
                        return "application/vnd.openxmlformats-officedocument.drawingml.diagramStyle+xml";

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/comments":
                        return "application/vnd.openxmlformats-officedocument.spreadsheetml.comments+xml";

                    case "http://schemas.microsoft.com/office/2006/relationships/vbaProject":
                        return "application/vnd.ms-office.vbaProject";

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/vmlDrawing":
                        return "application/vnd.openxmlformats-officedocument.vmlDrawing";

                    case "http://schemas.openxmlformats.org/officeDocument/2006/relationships/ctrlProp":
                        return "application/vnd.ms-excel.controlproperties+xml";
                }
            }
            return string.Empty;
        }

        public static Stream CreateStreamFromObject(object obj, Type type)
        {
            if (obj == null)
            {
                return null;
            }
            Stream stream = null;
            if (obj is CT_Relationships)
            {
                CT_Relationships relationships = obj as CT_Relationships;
                stream = (Stream) new MemoryStream();
                new XmlWriterSettings().Encoding = Encoding.UTF8;
                XmlWriter @this = XmlWriter.Create(stream, new XmlWriterSettings());
                using (@this.WriteDocument(true))
                {
                    using (@this.WriteElement("Relationships", "http://schemas.openxmlformats.org/package/2006/relationships"))
                    {
                        foreach (Relationship relationship in relationships.Relationship)
                        {
                            using (@this.WriteElement("Relationship"))
                            {
                                @this.WriteAttributeString("Id", relationship.Id);
                                @this.WriteAttributeString("Type", relationship.Type);
                                @this.WriteAttributeString("Target", relationship.Target.Trim(new char[] { '\n', '\b', ' ' }));
                                if (!string.IsNullOrWhiteSpace(relationship.TargetMode))
                                {
                                    @this.WriteAttributeString("TargetMode", relationship.TargetMode);
                                }
                            }
                        }
                    }
                }
                @this.Flush();
                stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                return stream;
            }
            if (obj is CT_Types)
            {
                CT_Types types = obj as CT_Types;
                stream = (Stream) new MemoryStream();
                new XmlWriterSettings().Encoding = Encoding.UTF8;
                XmlWriter writer2 = XmlWriter.Create(stream, new XmlWriterSettings());
                using (writer2.WriteDocument(true))
                {
                    using (writer2.WriteElement("Types", "http://schemas.openxmlformats.org/package/2006/content-types"))
                    {
                        foreach (object obj2 in types.Items)
                        {
                            if (obj2 is CT_Default)
                            {
                                CT_Default default2 = obj2 as CT_Default;
                                using (writer2.WriteElement("Default"))
                                {
                                    writer2.WriteAttributeString("ContentType", default2.ContentType);
                                    writer2.WriteAttributeString("Extension", default2.Extension);
                                    continue;
                                }
                            }
                            if (obj2 is CT_Override)
                            {
                                CT_Override @override = obj2 as CT_Override;
                                using (writer2.WriteElement("Override"))
                                {
                                    writer2.WriteAttributeString("ContentType", @override.ContentType);
                                    writer2.WriteAttributeString("PartName", @override.PartName);
                                }
                            }
                        }
                    }
                }
                writer2.Flush();
                stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
            }
            return stream;
        }

        public static CT_Types GetContentTypes(XFile rootFile, MemoryFolder mFolder, ExcelFileType workbookType)
        {
            if (((rootFile == null) || (rootFile.RelationFiles == null)) || ((rootFile.RelationFiles.Count == 0) || (mFolder == null)))
            {
                return null;
            }
            List<object> defaultType = GetDefaultType();
            if (defaultType == null)
            {
                return null;
            }
            if (!UpdateFileList(defaultType, rootFile, mFolder, workbookType))
            {
                return null;
            }
            return new CT_Types { Items = defaultType.ToArray() };
        }

        private static List<object> GetDefaultType()
        {
            List<object> list = new List<object>();
            CT_Default default2 = new CT_Default {
                Extension = "bin",
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.printerSettings"
            };
            list.Add(default2);
            default2 = new CT_Default {
                Extension = "rels",
                ContentType = "application/vnd.openxmlformats-package.relationships+xml"
            };
            list.Add(default2);
            default2 = new CT_Default {
                Extension = "xml",
                ContentType = "application/xml"
            };
            list.Add(default2);
            default2 = new CT_Default {
                Extension = "jpg",
                ContentType = "image/jpg"
            };
            list.Add(default2);
            default2 = new CT_Default {
                Extension = "png",
                ContentType = "image/png"
            };
            list.Add(default2);
            default2 = new CT_Default {
                Extension = "bmp",
                ContentType = "image/bmp"
            };
            list.Add(default2);
            default2 = new CT_Default {
                Extension = "gif",
                ContentType = "image/gif"
            };
            list.Add(default2);
            default2 = new CT_Default {
                Extension = "emf",
                ContentType = "image/x-emf"
            };
            list.Add(default2);
            return list;
        }

        public static void SaveContentTypes(XFile file, MemoryFolder mFolder, ExcelFileType workbookType)
        {
            CT_Types types = GetContentTypes(file, mFolder, workbookType);
            if (types != null)
            {
                mFolder.CreateMemoryFile("[Content_Types].xml", CreateStreamFromObject(types, typeof(CT_Types)));
            }
        }

        private static bool UpdateFileList(List<object> types, XFile xFile, MemoryFolder mFolder, ExcelFileType workbookType)
        {
            if (((types == null) || (xFile == null)) || (mFolder == null))
            {
                return false;
            }
            bool flag = true;
            if (!string.IsNullOrEmpty(xFile.FileName))
            {
                string str = ConvertRelationshipType2ContentType(xFile.FileType, workbookType);
                if (!string.IsNullOrEmpty(str))
                {
                    CT_Override @override = new CT_Override {
                        PartName = xFile.FileName.Replace('\\', '/'),
                        ContentType = str
                    };
                    if (new List<string> { "application/vnd.openxmlformats-officedocument.spreadsheetml.comments+xml", "application/vnd.openxmlformats-officedocument.drawingml.diagramColors+xml", "application/vnd.openxmlformats-officedocument.drawingml.diagramData+xml", "application/vnd.ms-office.drawingml.diagramDrawing+xml", "application/vnd.openxmlformats-officedocument.drawingml.diagramLayout+xml", "application/vnd.openxmlformats-officedocument.drawingml.diagramStyle+xml", "application/vnd.ms-excel.controlproperties+xml", "application/vnd.openxmlformats-officedocument.vmlDrawing" }.Contains(@override.ContentType))
                    {
                        @override.PartName = xFile.Target.Replace("..", "/xl");
                    }
                    bool flag2 = false;
                    foreach (object obj2 in types)
                    {
                        if (obj2 is CT_Override)
                        {
                            CT_Override override2 = obj2 as CT_Override;
                            if ((override2.ContentType == @override.ContentType) && (override2.PartName == @override.PartName))
                            {
                                flag2 = true;
                                break;
                            }
                        }
                    }
                    if (!flag2)
                    {
                        types.Add(@override);
                    }
                }
            }
            if ((xFile.RelationFiles != null) && (xFile.RelationFiles.Count > 0))
            {
                foreach (XFile file in xFile.RelationFiles.Values)
                {
                    if (file != null)
                    {
                        flag = flag && UpdateFileList(types, file, mFolder, workbookType);
                    }
                }
            }
            return flag;
        }

        public static void WriteWorkbookFile(XFile workbookFile, List<SheetInfo> sheets, MemoryFolder mFolder)
        {
            MemoryStream stream = new MemoryStream();
            new XmlWriterSettings().Encoding = Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create((Stream) stream, new XmlWriterSettings());
            writer.WriteStartDocument();
            writer.WriteStartElement("workbook", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
            writer.WriteAttributeString("xmlns", "r", null, "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            writer.WriteStartElement("sheets");
            foreach (SheetInfo info in sheets)
            {
                writer.WriteStartElement("sheet");
                writer.WriteAttributeString("name", info.name);
                writer.WriteAttributeString("sheetId", ((uint) info.sheetID).ToString());
                writer.WriteAttributeString("r", "id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships", info.rID);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
            mFolder.CreateMemoryFile(workbookFile.FileName, (Stream) stream);
        }
    }
}

