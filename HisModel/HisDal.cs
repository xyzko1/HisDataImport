using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace HisModel
{
    /// <summary>
    /// 数字报
    /// </summary>
    public class HisDal
    {
        public void ReadData()
        {
            //string path = @"D:\Epaper_zgyyb\images\pic_bak\zgyyb";
            string path = @"\\172.17.100.61\zgyyb";
            DateTime dt = DateTime.Now.Date;
            using (DLEntities ent = new DLEntities())
            {
                path = path + "\\" + dt.Year+"\\" + (dt.Month>=10?dt.Month.ToString() :("0"+ dt.Month))+ "\\" + (dt.Day >= 10 ? dt.Day.ToString() : ("0" + dt.Day));
                DirectoryInfo dir = new DirectoryInfo(path);
                //if (dir.Exists)
                //{
                //    var childDirs = dir.GetDirectories();
                //    for (int i = 0; i < childDirs.Length; i++)
                //    {
                //        var pName = childDirs[i].Name;
                //        var dirPathForMonth = path + "\\" + childDirs[i].Name;
                //        var monthPath = new DirectoryInfo(dirPathForMonth);
                //        var monthDirs = monthPath.GetDirectories();
                //        for (int j = 0; j < monthDirs.Length; j++)
                //        {
                //            var dirPathForDay = dirPathForMonth + "\\" + monthDirs[j].Name;
                //            var dayPath = new DirectoryInfo(dirPathForDay);
                //            var dayDirs = dayPath.GetDirectories();
                //            for (int k = 0; k < dayDirs.Length; k++)
                //            {
                //                var dirPathForItem = dirPathForDay + "\\" + dayDirs[k].Name;
                //                var xmlItemDirs = new DirectoryInfo(dirPathForItem);
                //                var xmlFiles = xmlItemDirs.GetFiles("*.XML");
                //                DealXML(ent, dirPathForItem, xmlFiles);

                //            }
                //        }
                //    }
                //}
                if (dir.Exists)
                {
                            var dayDirs = dir.GetDirectories();
                    List<cb_digital_page>  curPages = ent.cb_digital_page.Where(u => u.Date == dt).ToList();
                   
                    //if (curPages.Count< dayDirs.Length) {
                        for (int k = 0; k < dayDirs.Length; k++)
                        {
                            //校验若存在新xml数据,则插入新增的数据
                            bool flag = true;
                            foreach (var item in curPages)
                            {
                                if (item.PageSNO == dayDirs[k].Name) {
                                    flag = false;
                                }
                            }
                            var dirPathForItem = path + "\\" + dayDirs[k].Name;
                            var xmlItemDirs = new DirectoryInfo(dirPathForItem);
                            var xmlFiles = xmlItemDirs.GetFiles("*.XML");
                            if (flag) {
                                DealXML(ent, dirPathForItem, xmlFiles);
                            }
                            string toFilePath = @"C:\taiji\zgyyb" + "\\" + dt.Year + "\\" + (dt.Month >= 10 ? dt.Month.ToString() : ("0" + dt.Month)) + "\\" + (dt.Day >= 10 ? dt.Day.ToString() : ("0" + dt.Day)) + "\\" + dayDirs[k].Name;
                        DirectoryInfo dirToFilePath = new DirectoryInfo(toFilePath + "\\" + xmlFiles[0].Name);
                        if (!dirToFilePath.Exists) {
                            CopyFileStream(dirPathForItem + "\\" + xmlFiles[0].Name, toFilePath + "\\" + xmlFiles[0].Name);
                        }
                         
                            var pdfFiles = xmlItemDirs.GetFiles("*.pdf");

                            if (pdfFiles.Count() > 0)
                            {
                            DirectoryInfo dirToPdfFiles = new DirectoryInfo(toFilePath + "\\" + pdfFiles[0].Name);
                            if (!dirToPdfFiles.Exists)
                            {
                                CopyFileStream(dirPathForItem + "\\" + pdfFiles[0].Name, toFilePath + "\\" + pdfFiles[0].Name);
                            }
                            }
                            var jpgFiles = xmlItemDirs.GetFiles("*.jpg");
                            for (int x = 0; x < jpgFiles.Count(); x++)
                            {
                            DirectoryInfo dirToJpgFiles = new DirectoryInfo(toFilePath + "\\" + jpgFiles[x].Name);
                            if (!dirToJpgFiles.Exists)
                            {
                                CopyFileStream(dirPathForItem + "\\" + jpgFiles[x].Name, toFilePath + "\\" + jpgFiles[x].Name);
                            }
                            }
                        }
                    //}
                           
                }
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"C:\log\HisData\log.txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 数据导入成功");
                }
            }
        }
        public static bool CopyFileStream(string fromFile, string toFile)
        {
            if (File.Exists(fromFile))
            {
                string toDir = Path.GetDirectoryName(toFile);

                if (!Directory.Exists(toDir)) Directory.CreateDirectory(toDir);

                if (File.Exists(toFile)) File.Delete(toFile);

                using (FileStream fsReader = new FileStream(fromFile, FileMode.Open, FileAccess.Read))
                {
                    using (FileStream fsWriter = new FileStream(toFile, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        //每次读取1M
                        byte[] buffer = new byte[1024 * 1024 * 1];

                        while (true)
                        {
                            int r = fsReader.Read(buffer, 0, buffer.Length);

                            if (r == 0)
                            {
                                break;
                            }
                            fsWriter.Write(buffer, 0, r);
                        }
                    }
                }
                return true;
            }
            else
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"C:\log\HisData\log.txt", true))
                {
                    sw.WriteLine(string.Format("文件\"{0}\"不存在，无法拷贝！", fromFile));
                }
                return false;
            }
        }
        public void DealXML(DLEntities ent, string path, FileInfo[] xmls)
        {
            string filePath = @"";

            Regex imageReg = new Regex(@"\<IMG\=(.)*?\.jpg\>", RegexOptions.IgnoreCase);
            Regex ctsmReg = new Regex(@"\<CTSM\>(.|\n)*?\<\/CTSM\>", RegexOptions.Multiline);
            Regex ctzzReg = new Regex(@"\<CTZZ\>(.|\n)*?\<\/CTZZ\>", RegexOptions.Multiline);

            for (int i = 0; i < xmls.Length; i++)
            {
                var xml = xmls[i];
                try
                {
                    XDocument doc = XDocument.Load(xml.FullName);
                    var page = doc.Root.Element("name");

                    //var setDate = DateTime.Parse(DateTime.ParseExact(doc.Root.Element("date").Value, "yyyyMMdd", CultureInfo.CurrentCulture).ToString("yyyy-MM-dd"));
                    var setDate = DateTime.Now.Date;
                    var PageNO = doc.Root.Element("版次").Value;
                    var digital_page = new cb_digital_page
                    {
                        PageName = doc.Root.Element("版名").Value,
                        PaperID = 1,
                        Date = setDate,
                        //PaperIssueID = int.Parse(doc.Root.Element("当期期号").Value),
                        //Number = doc.Root.Element("number").Value,
                        //PageStyle = doc.Root.Element("版式").Value,
                        PageSNO = PageNO,
                        //PageCount = doc.Root.Element("pagecount").Value,
                        PageSort = doc.Root.Element("版面序号").Value,
                        fromWhere = "XML导入",
                        Editor = doc.Root.Element("本期编辑").Value
                    };
                    ent.cb_digital_page.Add(digital_page);
                    ent.Configuration.ValidateOnSaveEnabled = false;
                    ent.SaveChanges();

                    var pagId = ent.cb_digital_page.Where(u => u.Date == setDate && u.PageSNO == PageNO).Select(u => u.id).FirstOrDefault();
                     filePath = filePath + path.Substring(path.IndexOf("\\zgyyb"));
                    var news = doc.Root.Elements("Rec").ToList();
                    var list = news.Select(t => new cb_digital_news
                    {
                        PaperID = 1,
                        PageID = pagId,
                        ArticleName = t.Element("报纸名称").Value,
                        ArticleNO = t.Element("报纸编号").Value,
                        PublishDate = DateTime.Parse(DateTime.ParseExact(t.Element("出版日期").Value, "yyyyMMdd", CultureInfo.CurrentCulture).ToString("yyyy-MM-dd")),
                        IssueID = t.Element("期号").Value,
                        //IssueID = t.Element("当期期号").Value,
                        //Issue = t.Element("总第期号").Value,
                        PageCount = t.Element("总版面数").Value,
                        //PageStyle = t.Element("版式").Value,
                        PageName = t.Element("版名").Value,
                        PageSNO = t.Element("版次").Value,
                        PageSort = t.Element("版面序号").Value,
                        ArticleSort = t.Element("文章序号").Value,
                        Editor = t.Element("本期编辑").Value,
                        PageIndex = t.Element("所在页面").Value,
                        //ArticleType = t.Element("稿件类型").Value,
                        PageColumn = t.Element("栏目").Value,
                        PrimersTitle = t.Element("引题").Value,
                        Title = t.Element("标题").Value,
                        SubTitle = t.Element("副标题").Value,
                        Author = t.Element("作者").Value,
                        Coordinate = t.Element("坐标").Value,
                        ImageName = t.Element("图片").Value,
                        ImageWaterMark = t.Element("图片水印").Value,
                        ImageMark = t.Element("图片说明").Value,
                        ReprintTag = t.Element("转版标记").Value,
                        //Abstract = t.Element("编者按").Value,
                        Content = t.Element("正文").Value,
                        fromWhere = "XML导入",
                        ImageFilePath = filePath.Replace('\\', '/') + "/L" + t.Element("版次").Value + ".jpg"
                    }).ToList();
                    for (int j = 0; j < list.Count; j++)
                    {
                        var item = list[j];
                        ent.cb_digital_news.Add(item);
                        ent.Configuration.ValidateOnSaveEnabled = false;
                        ent.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"C:\log\HisData\log.txt", true))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + ex.Message);
                    }
                    continue;
                }
            }

        }
    }
}
