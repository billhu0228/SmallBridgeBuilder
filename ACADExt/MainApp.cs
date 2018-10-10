using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using MOExcel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;

[assembly: CommandClass(typeof(ACADExt.MainApp))]
namespace ACADExt
{
    /// <summary>
    /// 主要逻辑入口
    /// </summary>
    class MainApp
    {
        [CommandMethod("main")]
        public void Main()
        {


            //--------------------------------------------------------------------------------------
            // 基本句柄
            //--------------------------------------------------------------------------------------
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            ObjectId paperSpace = db.CreatPaperSpace();
            MOExcel.Application app = new MOExcel.Application();
            MOExcel.Workbook wbook = app.Workbooks.Add(Type.Missing);
            MOExcel.Worksheet wsheet = (MOExcel.Worksheet)wbook.Worksheets[1];
            
            //--------------------------------------------------------------------------------------
            // 初始化桥梁列表
            //--------------------------------------------------------------------------------------
            Bridge TheBridge;
            System.Data.DataTable Parameters = new System.Data.DataTable();

            string aa = BPublicFunctions.GetXPath("选择设计表", "参数表|*.xls");
            if (aa == "")
            {
                return;
            }
            else
            {
                Parameters = BPublicFunctions.GetDataFromExcelByCom(true, aa);
                ed.WriteMessage("\n桥梁数据读取成功");
            }
            var IDtoPolot = Parameters.AsEnumerable().Select(t => t.Field<int>("序号")).ToList();
            
            //--------------------------------------------------------------------------------------
            // 加载贝雷梁模型                    
            //--------------------------------------------------------------------------------------
            MyCommands.CADini();

            string T321ModelFilePath = BPublicFunctions.GetCADPath("选择设计表", "模型|*.model");
            Database sourceDb = new Database(false, true);
            try
            {
                sourceDb.ReadDwgFile(T321ModelFilePath, System.IO.FileShare.Read, true, "");
                ObjectIdCollection blockIds = new ObjectIdCollection();
                Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = sourceDb.TransactionManager;

                using (Transaction myT = tm.StartTransaction())
                {
                    // Open the block table
                    BlockTable bt = (BlockTable)tm.GetObject(sourceDb.BlockTableId, OpenMode.ForRead, false);

                    // Check each block in the block table
                    foreach (ObjectId btrId in bt)
                    {
                        BlockTableRecord btr = (BlockTableRecord)tm.GetObject(btrId, OpenMode.ForRead, false);
                        // Only add named & non-layout blocks to the copy list
                        if (!btr.IsAnonymous && !btr.IsLayout)
                        {
                            blockIds.Add(btrId);
                        }                            
                        btr.Dispose();
                    }
                }
                IdMapping mapping = new IdMapping();
                sourceDb.WblockCloneObjects(blockIds, db.BlockTableId, mapping, DuplicateRecordCloning.Replace, false);


            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                ed.WriteMessage("\nError during copy: " + ex.Message);
            }
            sourceDb.Dispose();


            //--------------------------------------------------------------------------------------
            // 初始化绘图空间                     
            //-------------------------------------------------------------------------------------- 
            ed.WriteMessage("\n 环境初始化成功.\n");
            //--------------------------------------------------------------------------------------
            // 绘图
            //--------------------------------------------------------------------------------------
            string fsd = Path.Combine(Path.GetDirectoryName(T321ModelFilePath), "TK.dwg");

            int conter = 0;
            foreach (int ii in IDtoPolot)
            {
                TheBridge = new Bridge(Parameters, ii);

                double dymax;
                Point2d AP = Point2d.Origin.Convert2D(-0.5 * TheBridge.Pieces * 3000, conter * -30000);
                Point2d CC = Point2d.Origin.Convert2D(0, conter * -30000);
                Point2d PlotB_AP = CC.Convert2D(30000);
                dymax =TheBridge.PlotA(db, AP);
                TheBridge.PlotB(db, PlotB_AP);
                conter++;
                
                // 套图框
                db.XrefAttachAndInsert(fsd, paperSpace, Point3d.Origin.Convert3D(0, (ii-1) *(-297), 0));
                db.XrefAttachAndInsert(fsd, paperSpace, Point3d.Origin.Convert3D(420, (ii - 1) * (-297), 0));
                // 图名图号注释
                TextPloter.PrintNumTitle(db, Point3d.Origin.Convert3D(0, (ii - 1) * (-297), 0), TheBridge);
                TextPloter.PrintNote(db, Point3d.Origin.Convert3D(360, (ii - 1) * (-297)+40, 0));
                TextPloter.PrintNote2(db, Point3d.Origin.Convert3D(250+420, (ii - 1) * (-297) + 62, 0));
                // 视口
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Layout lay = (Layout)tr.GetObject(paperSpace, OpenMode.ForWrite);
                    Viewport vpA, vpB;
                    var btr = (BlockTableRecord)tr.GetObject(lay.BlockTableRecordId, OpenMode.ForWrite);
                    vpA = new Viewport();
                    btr.AppendEntity(vpA);
                    tr.AddNewlyCreatedDBObject(vpA, true);
                    vpA.On = true;
                    vpA.GridOn = false;
                    vpA.DrawMyViewport(1, Point3d.Origin.Convert3D(0, (ii - 1) * (-297)),
                        CC.Convert2D(0,dymax+(8- 0.5* 247)* TheBridge.GetScaleA()), TheBridge.GetScaleA());
                    vpB = new Viewport();
                    btr.AppendEntity(vpB);
                    tr.AddNewlyCreatedDBObject(vpB, true);
                    vpB.On = true;
                    vpB.GridOn = false;
                    vpB.DrawMyViewport(1, Point3d.Origin.Convert3D(420, (ii - 1) * (-297)),
                        PlotB_AP.Convert2D(90*50,-35*50), 50);
                    tr.Commit();
                }

                // 表格
                TheBridge.PlotNumTB(db, Point2d.Origin.Convert2D(420+310, (ii - 1) * (-297)+240));

                TheBridge.PlotMatTB(db, Point2d.Origin.Convert2D(420 + 620, (ii - 1) * (-297) + 240));


            }













            Marshal.ReleaseComObject(wbook);
            wbook = null;
            app.Workbooks.Close();
            app.KillExcelApp();

          
        }

        [CommandMethod("Cal")]
        public void Calculate()
        {

            //--------------------------------------------------------------------------------------
            // 初始化桥梁列表
            //--------------------------------------------------------------------------------------
            Bridge TheBridge;
            System.Data.DataTable Parameters = new System.Data.DataTable();

            string rootDir = BPublicFunctions.GetPath("选择Ansys路径");
            string latexDir = BPublicFunctions.GetPath("选择Latex路径");

            string aa = BPublicFunctions.GetXPath("选择参数表", "参数表|*.xls");
            if (aa == "")
            {
                return;
            }
            else
            {
                Parameters = BPublicFunctions.GetDataFromExcelByCom(true, aa);                
            }
            var IDtoPolot = Parameters.AsEnumerable().Select(t => t.Field<int>("序号")).ToList();
            
            foreach (int ii in IDtoPolot)
            {
                TheBridge = new Bridge(Parameters, ii);

                AnsysCal bill = new AnsysCal(TheBridge);
                string wdir=bill.AnsysCalculator(rootDir);
                bill.GenerateModel();
                bill.GenerateSection();
                bill.WriteMain(wdir);
                bill.WritePrep(wdir);
                bill.WriteNode(wdir);
                bill.WriteSect(wdir);
                bill.WriteElem(wdir);
                bill.WriteDebug(wdir);
                bill.WriteSolu(wdir);
                bill.WriteSM1600(wdir);
                bill.WritePost(wdir);
              


                LatexGen kitty = new LatexGen(TheBridge);
                string filewdir=kitty.GetReady(latexDir);
                kitty.WriteTex(filewdir);
                //kitty.RunXeLatex(filewdir, Path.Combine(filewdir,"CalSheet.tex"));


                break;


            }

        }
    }
}
