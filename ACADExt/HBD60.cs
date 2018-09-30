using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;

[assembly: CommandClass(typeof(ACADExt.HBD60))]
namespace ACADExt
{
    public class HBD60
    {
        [CommandMethod("HBD60BlocksDef")]
        public static void BlocksDef()
        {
            //Document doc =curdoc;
            Document doc =Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                //-------------------------------------------------------------------------------------------
                // 自定义块
                //-------------------------------------------------------------------------------------------
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = new BlockTableRecord();
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForWrite) as TextStyleTable;
                
                if (!bt.Has("HDB60-1"))
                {
                    btr.Name = "HDB60-1";
                    bt.UpgradeOpen();
                    bt.Add(btr);
                    tr.AddNewlyCreatedDBObject(btr, true);
                    

                    // 轴线
                    Polyline AxisTri = new Polyline()
                    {
                        Layer = "中心线",
                        Closed = true,
                    };
                    AxisTri.AddVertexAt(0, new Point2d( -1524,0), 0, 0, 0);
                    AxisTri.AddVertexAt(1, new Point2d(0, 2250), 0, 0, 0);
                    AxisTri.AddVertexAt(2, new Point2d( 1524,0), 0, 0, 0);
                    btr.AppendEntity(AxisTri);
                    tr.AddNewlyCreatedDBObject(AxisTri, true);

                    // 工字钢
                    List<Line> H0, H1, H2, H3;
                    H0 = HShapePlot(AxisTri.GetLine(0), 150, 8, btr, db);
                    H1 = HShapePlot(AxisTri.GetLine(1), 150, 8, btr, db);
                    H2 = HShapePlot(AxisTri.GetLine(2), 200, 8, btr, db);
                    Line AxY = new Line(new Point3d(0, -150, 0), new Point3d(0,2250+150, 0));
                    H3 = HShapePlot(AxY, 150, 8, btr, db);

                    // 修剪
                    Polyline cuter = new Polyline();
                    cuter.AddVertexAt(0, new Point2d(-1524, 5000), 0, 0, 0);
                    cuter.AddVertexAt(1, new Point2d(-1524, 100), 0, 0, 0);
                    cuter.AddVertexAt(2, new Point2d(5000, 100), 0, 0, 0);
                    //LinesCut(ref H0, ref cuter, true);
                    //cuter.TransformBy(Matrix3d.Mirroring(AxY.Convert3D()));
                    //LinesCut(ref H0, ref cuter, true);

                }


                tr.Commit();
            }

           



            ed.WriteMessage("\n HBD60 Block Defination Success..");
        }

        

        private static void LinesCut(ref List<Line> main, ref Polyline cuter,bool direct)
        {
            Point3dCollection pts;
            foreach(Line ll in main)
            {
                pts=null;
                ll.IntersectWith(cuter, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);
                if (pts.Count != 0)
                {
                    if (direct)
                    {
                        ll.StartPoint = pts[0];
                    }
                    else
                    {
                        ll.EndPoint = pts[0];
                    }                    
                }
                else
                {
                    continue;
                }
            }

        }




        private static List<Line> HShapePlot(Line A2,double w1,double t1, BlockTableRecord btr,Database db)
        {
            List<Line> res = new List<Line>();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {   
                Line L1;
                L1 = (Line)A2.GetOffsetCurves(0.5*w1)[0];
                L1.Layer = "粗线";
                btr.AppendEntity(L1);
                tr.AddNewlyCreatedDBObject(L1, true);
                res.Add(L1);
                L1 = (Line)A2.GetOffsetCurves(-0.5*w1)[0];
                L1.Layer = "粗线";
                btr.AppendEntity(L1);
                tr.AddNewlyCreatedDBObject(L1, true);
                res.Add(L1);
                L1 = (Line)A2.GetOffsetCurves(-0.5 * t1)[0];
                L1.Layer = "虚线";
                btr.AppendEntity(L1);
                tr.AddNewlyCreatedDBObject(L1, true);
                res.Add(L1);
                L1 = (Line)A2.GetOffsetCurves(0.5 * t1)[0];
                L1.Layer = "虚线";
                btr.AppendEntity(L1);
                tr.AddNewlyCreatedDBObject(L1, true);
                res.Add(L1);
                tr.Commit();
            }
            return res;
        }
    }
}
      