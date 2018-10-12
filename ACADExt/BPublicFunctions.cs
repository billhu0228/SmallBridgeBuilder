using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using WFM = System.Windows.Forms;
using MOExcel = Microsoft.Office.Interop.Excel;
using Autodesk.AutoCAD.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;
using SD= System.Data;

[assembly: CommandClass(typeof(ACADExt.BPublicFunctions))]

namespace ACADExt
{
    public class BPublicFunctions
    {



        public static string GetPath(string PromptOpts)
        {
            string xpath = "";
            WFM.FolderBrowserDialog path = new WFM.FolderBrowserDialog();

            if (path.ShowDialog() == WFM.DialogResult.OK)
            {
                xpath = path.SelectedPath;
            }
            else
            {
                xpath = "";
            }
            return xpath;
        }


        /// <summary>
        /// 提示对话框
        /// </summary>
        /// <param name="PromptOpts">对话框标题</param>
        /// <returns></returns>
        public static string GetXPath(string PromptOpts,string FilterStr="Excel文件|*.xls;*.xlsx")
        {
            string xpath = "";
            WFM.OpenFileDialog dialog = new WFM.OpenFileDialog();
            dialog.Title = PromptOpts;

            dialog.InitialDirectory = "G:/BailayBeam/03 测试";
            dialog.Filter = FilterStr;
            //dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == WFM.DialogResult.OK)
            {
                xpath = dialog.FileName;
            }
            else
            {
                xpath = "";
            }
            
            return xpath;
        }
        public static string GetCADPath(string PromptOpts, string FilterStr = "模型文件|*.Model")
        {
            string xpath = "";
            WFM.OpenFileDialog dialog = new WFM.OpenFileDialog();
            dialog.Title = PromptOpts;

            dialog.InitialDirectory = "G:/BailayBeam/03 测试";
            dialog.Filter = FilterStr;
            //dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == WFM.DialogResult.OK)
            {
                xpath = dialog.FileName;
            }
            else
            {
                xpath = "";
            }

            return xpath;
        }


        public static SD.DataTable GetDataFromExcelByCom(bool hasTitle, string fileName)
        {
            MOExcel.Application app = new MOExcel.Application();
            MOExcel.Sheets sheets;
            object oMissiong = System.Reflection.Missing.Value;
            MOExcel.Workbook workbook = null;
            System.Data.DataTable dt = new System.Data.DataTable();

            try
            {
                if (app == null) return null;
                workbook = app.Workbooks.Open(fileName, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong,
                    oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong);
                sheets = workbook.Worksheets;

                //将数据读入到DataTable中
                MOExcel.Worksheet worksheet = (MOExcel.Worksheet)sheets.get_Item(1);//读取第一张表 
                if (worksheet == null) return null;

                int iRowCount = worksheet.UsedRange.Rows.Count;
                int iColCount = worksheet.UsedRange.Columns.Count;
                //生成列头
                for (int i = 0; i < iColCount; i++)
                {
                    var name = "column" + i;
                    if (hasTitle)
                    {
                        var txt = ((MOExcel.Range)worksheet.Cells[1, i + 1]).Text.ToString();
                        if (!string.IsNullOrEmpty(txt)) name = txt;
                    }
                    while (dt.Columns.Contains(name)) name = name + "_1";//重复行名称会报错。
                    if (name == "桥名"|| name == "类型")
                    {
                        dt.Columns.Add(new System.Data.DataColumn(name, typeof(string)));
                    }
                    else if (name == "序号"|| name == "排数"|| name == "层数"|| name == "节数")
                    {
                        dt.Columns.Add(new System.Data.DataColumn(name, typeof(int)));
                    }
                    else if(name == "纵坡" || name == "BeginStation" || name == "EndStation")
                    {
                        dt.Columns.Add(new System.Data.DataColumn(name, typeof(double)));
                    }
                    else
                    {
                        dt.Columns.Add(new System.Data.DataColumn(name, typeof(bool)));
                    }


                }
                //生成行数据
                MOExcel.Range range;
                int rowIdx = hasTitle ? 2 : 1;
                for (int iRow = rowIdx; iRow <= iRowCount; iRow++)
                {
                    SD.DataRow dr = dt.NewRow();
                    for (int iCol = 1; iCol <= iColCount; iCol++)
                    {
                        range = (MOExcel.Range)worksheet.Cells[iRow, iCol];
                        dr[iCol - 1] = (range.Value2 == null) ? "" : range.Text.ToString();
                    }
                    dt.Rows.Add(dr);
                }



                return dt;
            }
            catch { return dt; }
            finally
            {
                workbook.Close(false, oMissiong, oMissiong);
                Marshal.ReleaseComObject(workbook);
                workbook = null;
                app.Workbooks.Close();
                app.KillExcelApp();
            }
        }


        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static SD.DataTable OpenCSV(string filePath)
        {
            Encoding encoding = Encoding.UTF8;//
            SD.DataTable dt = new SD.DataTable();
            FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            //StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            StreamReader sr = new StreamReader(fs, encoding);
            //string fileContent = sr.ReadToEnd();
            //encoding = sr.CurrentEncoding;
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] TableHead = null;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                //strLine = Common.ConvertStringUTF8(strLine, encoding);
                //strLine = Common.ConvertStringUTF8(strLine);

                if (IsFirst == true)
                {
                    TableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = TableHead.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        SD.DataColumn dc = new SD.DataColumn(TableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    SD.DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (aryLine != null && aryLine.Length > 0)
            {
                dt.DefaultView.Sort = TableHead[0] + " " + "asc";
            }

            sr.Close();
            fs.Close();
            return dt;
        }

















    }























}