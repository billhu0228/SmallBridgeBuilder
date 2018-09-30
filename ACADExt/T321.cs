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



[assembly: CommandClass(typeof(ACADExt.T321))]
namespace ACADExt
{
    public class T321
    {    

        [CommandMethod("T321BlocksDef")]
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
                List<Line> axlist=new List<Line>();
                for (int i = 0; i < 8; i++)
                {
                    axlist.Add(new Line());
                }

                if (!bt.Has("T321-1"))
                {
                    btr.Name = "T321-1";
                    bt.UpgradeOpen();
                    bt.Add(btr);
                    tr.AddNewlyCreatedDBObject(btr, true);

                    // 轴线
                    Line AxisX0 = new Line(Point2d.Origin.Convert3D(-1500 - 150),Point2d.Origin.Convert3D(1500+150));
                    btr.AppendEntity(AxisX0);
                    tr.AddNewlyCreatedDBObject(AxisX0, true);
                    AxisX0.Layer = "中心线";
                    

                    Line AxisX1 = (Line)AxisX0.GetOffsetCurves(1400)[0];
                    btr.AppendEntity(AxisX1);
                    tr.AddNewlyCreatedDBObject(AxisX1, true);
                    AxisX1.Layer = "中心线";
                    
                    
                    Line AxisY1 = new Line(new Point3d(0, -150, 0), new Point3d(0, 1400 + 150, 0));
                    btr.AppendEntity(AxisY1);
                    tr.AddNewlyCreatedDBObject(AxisY1, true);
                    AxisY1.Layer = "中心线";
                    

                    Line AxisY0 = (Line)AxisY1.GetOffsetCurves(705*2.0)[0];
                    btr.AppendEntity(AxisY0);
                    tr.AddNewlyCreatedDBObject(AxisY0, true);
                    AxisY0.Layer = "中心线";

                    Line AxisY2 = (Line)AxisY1.GetOffsetCurves(-705 * 2.0)[0];
                    btr.AppendEntity(AxisY2);
                    tr.AddNewlyCreatedDBObject(AxisY2, true);
                    AxisY2.Layer = "中心线";

                    Point3d pt0 = new Point3d(-1410, 700, 0);
                    Point3d pt1 = new Point3d(-705, 1400, 0);
                    Point3d pt2 = new Point3d(0,700, 0);
                    Point3d pt3 = new Point3d(-705, 0, 0);
                    Point3d pt4 = new Point3d(-1410, 700, 0);

                    Point3d pt5 = new Point3d(1410, 700, 0);
                    Point3d pt6 = new Point3d(705,1400, 0);
                    Point3d pt7 = new Point3d(705, 0, 0);
                    Point3d pt8 = new Point3d(1410,700, 0);


                    axlist[0] = new Line(pt0,pt1);
                    axlist[1] = new Line(pt1,pt2);
                    axlist[2] = new Line(pt2, pt3);
                    axlist[3] = new Line(pt3, pt4);
                    axlist[4] = new Line(pt8, pt6);
                    axlist[5] = new Line(pt6, pt2);
                    axlist[6] = new Line(pt2, pt7);
                    axlist[7] = new Line(pt7, pt5);

                    foreach (Line ll in axlist)
                    {
                        ll.Layer = "中心线";
                        btr.AppendEntity(ll);
                        tr.AddNewlyCreatedDBObject(ll, true);
                    }


                    //Polyline AxisTri = new Polyline()
                    //{
                    //    Layer = "中心线",
                    //    Closed = true,
                    //};
                    //AxisTri.AddVertexAt(0, new Point2d( -1524,0), 0, 0, 0);
                    //AxisTri.AddVertexAt(1, new Point2d(0, 2250), 0, 0, 0);
                    //AxisTri.AddVertexAt(2, new Point2d( 1524,0), 0, 0, 0);
                    //btr.AppendEntity(AxisTri);
                    //tr.AddNewlyCreatedDBObject(AxisTri, true);

                    // 工字钢
                    //List<Line> H0, H1, H2, H3;
                    //H0 = HShapePlot(AxisTri.GetLine(0), 150, 8, btr, db);
                    //H1 = HShapePlot(AxisTri.GetLine(1), 150, 8, btr, db);
                    //H2 = HShapePlot(AxisTri.GetLine(2), 200, 8, btr, db);


                    // 修剪
                    //Polyline cuter = new Polyline();
                    //cuter.AddVertexAt(0, new Point2d(-1524, 5000), 0, 0, 0);
                    //cuter.AddVertexAt(1, new Point2d(-1524, 100), 0, 0, 0);
                    //cuter.AddVertexAt(2, new Point2d(5000, 100), 0, 0, 0);
                    //LinesCut(ref H0, ref cuter, true);
                    //cuter.TransformBy(Matrix3d.Mirroring(AxY.Convert3D()));
                    //LinesCut(ref H0, ref cuter, true);

                }


                tr.Commit();
            }

           



            ed.WriteMessage("\n T321 Block Defination Success..");
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
      