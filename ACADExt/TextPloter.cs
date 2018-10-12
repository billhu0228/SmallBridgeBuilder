using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data;
using MOExcel = Microsoft.Office.Interop.Excel;

namespace ACADExt
{
    class TextPloter
    {





        public static void PrintNumTitle(Database db, Point3d PaperOrigenPoint,Bridge theBridge)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.PaperSpace], OpenMode.ForWrite) as BlockTableRecord;

                // 右上角标号
                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i <= 1; i++)
                    {
                        DBText num = new DBText();
                        
                        num.Height = 3.5;
                        if (i == 0)
                        {
                            num.TextString =(j+1).ToString();
                            num.Position = PaperOrigenPoint.Convert3D(387.5+j*420, 283);
                        }
                        else
                        {
                            num.TextString = "2";
                            num.Position = PaperOrigenPoint.Convert3D(402.5 + j * 420, 283);
                        }
                        num.HorizontalMode = TextHorizontalMode.TextCenter;
                        num.VerticalMode = TextVerticalMode.TextVerticalMid;
                        num.AlignmentPoint = num.Position;
                        num.Layer = "标注";
                        num.TextStyleId = st["仿宋"];
                        recorder.AppendEntity(num);
                        tr.AddNewlyCreatedDBObject(num, true);
                    }

                }


                // 标题
                for (int j = 0; j < 2; j++)
                {
                    DBText title = new DBText();
                    if (j==0)
                    {
                        title.TextString = "桥梁总体布置图（一）";
                        title.Position = PaperOrigenPoint.Convert3D(167.5, 15);
                    }
                    else
                    {
                        title.TextString = "桥梁总体布置图（二）";
                        title.Position = PaperOrigenPoint.Convert3D(167.5+420, 15);
                    }                    
                    title.Height = 4.5;                    
                    title.HorizontalMode = TextHorizontalMode.TextCenter;
                    title.VerticalMode = TextVerticalMode.TextVerticalMid;
                    title.AlignmentPoint = title.Position;
                    title.Layer = "标注";
                    title.TextStyleId = st["仿宋"];
                    title.WidthFactor = 0.8;
                    recorder.AppendEntity(title);
                    tr.AddNewlyCreatedDBObject(title, true);
                }


                tr.Commit();
            }
            return;
        }


        public static void PrintNote(Database db, Point3d InsertPoint)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;                
                BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.PaperSpace], OpenMode.ForWrite) as BlockTableRecord;

                MText theNote=new MText();
                theNote.Contents = "注\\P" +
                    "{   1.图中尺寸均以毫米为单位。}\\P" +
                    "{   2.图中斜撑与联板为示出。}\\P" ;
                theNote.Location = InsertPoint;
                theNote.TextStyleId = st["仿宋"];
                theNote.TextHeight = 2.5;
                recorder.AppendEntity(theNote);
                tr.AddNewlyCreatedDBObject(theNote, true);
                tr.Commit();
            }
            return;
        }


        public static void PrintNote2(Database db, Point3d InsertPoint)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.PaperSpace], OpenMode.ForWrite) as BlockTableRecord;

                MText theNote = new MText();
                theNote.Contents = "注\\P" +
                    "{   1.图中尺寸均以毫米为单位。}\\P" +
                    "{   2.桥梁首尾两节不设加强弦杆。}\\P"+
                    "{   3.对于加强的桥梁，销子（连同插销）的数量，按上表算出后，单排时应减4个，双排时应减8个，三排时应减12个。}\\P" +
                    "{   4.端柱、桥座及座板包括在首节内。}\\P" +
                    "{   5.表列撑架螺栓数量中含斜撑螺栓和支撑架螺栓，若减去斜撑所用螺栓（首节减4个，中节减4个,尾节减8个）余者即为}\\P" +
                    "{     相应之首节、中节、尾节支撑架、联板所用之螺栓数量。}\\P" +
                    "{   6.中央桥头搭板与中央钢桥面板相同。}\\P"                    ;
                theNote.Location = InsertPoint;
                theNote.TextStyleId = st["仿宋"];
                theNote.TextHeight = 2.5;
                recorder.AppendEntity(theNote);
                tr.AddNewlyCreatedDBObject(theNote, true);
                tr.Commit();
            }
            return;
        }

        public static void PrintFName(Database db, Point3d InsertPoint,Bridge bridge)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.PaperSpace], OpenMode.ForWrite) as BlockTableRecord;

                DBText title = new DBText();
                title.TextString = bridge.Name;
                title.Position = InsertPoint.Convert3D(108.3735, 17.5);
                title.Height = 3;
                title.HorizontalMode = TextHorizontalMode.TextCenter;
                title.VerticalMode = TextVerticalMode.TextVerticalMid;
                title.AlignmentPoint = title.Position;
                title.Layer = "标注";
                title.TextStyleId = st["仿宋"];
                title.WidthFactor = 0.8;
                recorder.AppendEntity(title);
                tr.AddNewlyCreatedDBObject(title, true);

                title = new DBText();
                title.TextString = string.Format("{0}-{1}",bridge.BS,bridge.ES);
                title.Position = InsertPoint.Convert3D(108.3735, 17.5-5);
                title.Height = 3;
                title.HorizontalMode = TextHorizontalMode.TextCenter;
                title.VerticalMode = TextVerticalMode.TextVerticalMid;
                title.AlignmentPoint = title.Position;
                title.Layer = "标注";
                title.TextStyleId = st["仿宋"];
                title.WidthFactor = 0.8;
                recorder.AppendEntity(title);
                tr.AddNewlyCreatedDBObject(title, true);


                title = new DBText();
                title.TextString = bridge.Name;
                title.Position = InsertPoint.Convert3D(108.3735+420, 17.5);
                title.Height = 3;
                title.HorizontalMode = TextHorizontalMode.TextCenter;
                title.VerticalMode = TextVerticalMode.TextVerticalMid;
                title.AlignmentPoint = title.Position;
                title.Layer = "标注";
                title.TextStyleId = st["仿宋"];
                title.WidthFactor = 0.8;
                recorder.AppendEntity(title);
                tr.AddNewlyCreatedDBObject(title, true);

                title.TextString = string.Format("{0}-{1}", bridge.BS, bridge.ES);
                title.Position = InsertPoint.Convert3D(108.3735+420, 17.5 - 5);
                title.Height = 3;
                title.HorizontalMode = TextHorizontalMode.TextCenter;
                title.VerticalMode = TextVerticalMode.TextVerticalMid;
                title.AlignmentPoint = title.Position;
                title.Layer = "标注";
                title.TextStyleId = st["仿宋"];
                title.WidthFactor = 0.8;
                recorder.AppendEntity(title);
                tr.AddNewlyCreatedDBObject(title, true);

                tr.Commit();
            }
            return;
        }


        public static void PrintLineText(Database db,Point2d StartPoint, Point2d EndPoint,string[] textList,bool isLeft=true,double scale = 100)
        {
            int number = textList.Count();
            double wdh = 2.5;
            double WdWidth=1.25;
            double LineWidth=0;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;
                Line L1 = new Line();
                Line side = new Line();
                DBText txt = new DBText();

                foreach (string cnt in textList)
                {
                    double w = WdWidth * cnt.Length * scale;
                    if (w > LineWidth) { LineWidth = w; }
                }

                
                double xdir = isLeft ? -1 : 1;
                double ydir;
                if (EndPoint.Y > StartPoint.Y)
                {
                    L1 = new Line(StartPoint.Convert3D(), EndPoint.Convert3D(0, +(wdh + 0.5) * scale * (number - 1)));
                    ydir = 1;
                }
                else
                {
                    L1 = new Line(StartPoint.Convert3D(), EndPoint.Convert3D(0, -(wdh + 0.5) * scale * (number - 1)));
                    ydir = -1;
                }
                L1.Layer = "标注";
                modelSpace.AppendEntity(L1);
                tr.AddNewlyCreatedDBObject(L1, true);

                int i = 0;
                foreach (string cnt in textList)
                {

                    side = new Line(EndPoint.Convert3D(0, i *( wdh+0.5) * scale * ydir), EndPoint.Convert3D(xdir * LineWidth, i * (wdh+0.5) * scale * ydir));
                    side.Layer = "标注";
                    modelSpace.AppendEntity(side);
                    tr.AddNewlyCreatedDBObject(side, true);

                    if (cnt.StartsWith("Joint"))
                    {
                        break;
                    }
                    txt = new DBText();
                    txt.TextString = cnt;
                    txt.Height = wdh * scale;    
                    txt.Position = side.StartPoint;
                    txt.HorizontalMode = isLeft ? TextHorizontalMode.TextRight : TextHorizontalMode.TextLeft;
                    txt.VerticalMode = TextVerticalMode.TextBottom;
                    //txt.Justify = AttachmentPoint.BaseLeft;
                    txt.AlignmentPoint =txt.Position;
                    txt.Layer = "标注";
                    txt.TextStyleId = st["fsdb"];
                    txt.WidthFactor = 0.75;
                    modelSpace.AppendEntity(txt);
                    tr.AddNewlyCreatedDBObject(txt, true);

                    i++;
                }


                if (textList[0].StartsWith("Joint"))
                {
                    MatchCollection matches = Regex.Matches(textList[0], @"(\d+\.?\d*)");


                    txt = new DBText();
                    txt.TextString = "Joint";
                    txt.Height = wdh * scale;
                    txt.Position = side.StartPoint;
                    txt.HorizontalMode = isLeft ? TextHorizontalMode.TextRight : TextHorizontalMode.TextLeft;
                    txt.VerticalMode = TextVerticalMode.TextBottom;
                    //txt.Justify = AttachmentPoint.BaseLeft;
                    txt.AlignmentPoint = txt.Position;
                    txt.Layer = "标注";
                    txt.TextStyleId = st["fsdb"];
                    txt.WidthFactor = 0.75;
                    modelSpace.AppendEntity(txt);
                    tr.AddNewlyCreatedDBObject(txt, true);


                    txt = new DBText();
                    txt.TextString =string.Format ("e={0}cm", matches[0]);
                    txt.Height = wdh * scale;
                    txt.Position = side.StartPoint;
                    txt.HorizontalMode = isLeft ? TextHorizontalMode.TextRight : TextHorizontalMode.TextLeft;
                    txt.VerticalMode = TextVerticalMode.TextTop;
                    //txt.Justify = AttachmentPoint.BaseLeft;
                    txt.AlignmentPoint = txt.Position;
                    txt.Layer = "标注";
                    txt.TextStyleId = st["fsdb"];
                    txt.WidthFactor = 0.75;
                    modelSpace.AppendEntity(txt);
                    tr.AddNewlyCreatedDBObject(txt, true);
                    side.EndPoint = side.StartPoint.Convert3D(xdir*0.7 * side.Length);

                }


                tr.Commit();
            }
        }




        public static void PrintCirText(Database db, int textstring,  Point2d PositionPoint, double scale = 100)
        {
            DBText txt = new DBText();
            Circle C1 = new Circle();            
            Circle C2 = new Circle();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;


                txt.TextString = textstring.ToString();
                txt.Height = 2.5 * scale;
                txt.Position = PositionPoint.Convert3D();
                txt.HorizontalMode = TextHorizontalMode.TextCenter;
                txt.VerticalMode = TextVerticalMode.TextVerticalMid;
                txt.AlignmentPoint = PositionPoint.Convert3D();
                txt.TextStyleId = st["fsdb"];
                txt.Layer = "标注";
                txt.WidthFactor = 0.75;

                C1 = new Circle(PositionPoint.Convert3D(), Vector3d.ZAxis, 1.5 * scale);
                C2 = new Circle(PositionPoint.Convert3D(), Vector3d.ZAxis, 1.8 * scale);
                C1.Layer = "标注";
                C2.Layer = "标注";
                modelSpace.AppendEntity(txt);
                tr.AddNewlyCreatedDBObject(txt, true);
                modelSpace.AppendEntity(C1);
                tr.AddNewlyCreatedDBObject(C1, true);
                modelSpace.AppendEntity(C2);
                tr.AddNewlyCreatedDBObject(C2, true);
                tr.Commit();
            }
            return;
        }

        public static void PrintCirText(Database db, int textstring, Point2d PositionPoint,Point2d startP,Point2d endP, double scale = 100)
        {
            DBText txt = new DBText();
            Circle C1 = new Circle();
            Circle C2 = new Circle();
            Line L1, L2;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;


                txt.TextString = textstring.ToString();
                txt.Height = 2.5 * scale;
                txt.Position = PositionPoint.Convert3D();
                txt.HorizontalMode = TextHorizontalMode.TextCenter;
                txt.VerticalMode = TextVerticalMode.TextVerticalMid;
                txt.AlignmentPoint = PositionPoint.Convert3D();
                txt.TextStyleId = st["fsdb"];
                txt.Layer = "标注";
                txt.WidthFactor = 0.75;

                C1 = new Circle(PositionPoint.Convert3D(), Vector3d.ZAxis, 1.5 * scale);
                C2 = new Circle(PositionPoint.Convert3D(), Vector3d.ZAxis, 1.8 * scale);
                C1.Layer = "标注";
                C2.Layer = "标注";

                L1 = new Line(PositionPoint.Convert3D(0, 1.8 * scale), endP.Convert3D());
                L2 = new Line(PositionPoint.Convert3D(0, -1.8 * scale), startP.Convert3D());
                L1.Layer = "标注";
                L2.Layer = "标注";

                modelSpace.AppendEntity(txt);
                tr.AddNewlyCreatedDBObject(txt, true);
                modelSpace.AppendEntity(C1);
                tr.AddNewlyCreatedDBObject(C1, true);
                modelSpace.AppendEntity(C2);
                tr.AddNewlyCreatedDBObject(C2, true);
                modelSpace.AppendEntity(L1);
                tr.AddNewlyCreatedDBObject(L1, true);
                modelSpace.AppendEntity(L2);
                tr.AddNewlyCreatedDBObject(L2, true);
                tr.Commit();
            }
            return;
        }



        public static DBText PrintText(Database db, string textstring,double H, Point2d PositionPoint, double scale = 100,bool isCover=false)
        {
            DBText txt=new DBText();

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;
                txt.TextString = textstring;
                txt.Height = H * scale;
                txt.Position = PositionPoint.Convert3D();
                txt.HorizontalMode = TextHorizontalMode.TextCenter;
                txt.VerticalMode = TextVerticalMode.TextBottom;
                txt.AlignmentPoint = PositionPoint.Convert3D(0, 0.5 * scale);
                txt.TextStyleId = st["fsdb"];
                txt.Layer = "标注";
                txt.WidthFactor = 0.75;
                if (isCover)
                {
                    // 遮罩
                }

                modelSpace.AppendEntity(txt);
                tr.AddNewlyCreatedDBObject(txt, true);
                tr.Commit();
            }
            return txt;
        }

        public static void PrintTitle(Database db,string textstring,Point2d PositionPoint ,double scale= 100,string btrname="")
        {

            DBText title= new DBText(),scalestring= new DBText();
            Line BLine,SLine;
            Regex regChina = new Regex("^[^\x00-\xFF]");
            Regex regEnglish = new Regex("^[a-zA-Z]");



            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                double WdWidth;
                if (regEnglish.IsMatch(textstring[textstring.Length-1].ToString()))
                {
                    WdWidth = 2.5;
                    scalestring.TextString = string.Format("Ech:1/{0:G0}", scale);

                }
                else
                {
                    WdWidth = 5 * 1.2;
                    scalestring.TextString = string.Format("1:{0:G0}", scale);
                }
                double WdLeng = textstring.Length * WdWidth * scale;


                BlockTableRecord modelSpace;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                if (btrname == "")
                {
                    modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                }
                else
                {
                    modelSpace = tr.GetObject(blockTbl[btrname], OpenMode.ForWrite) as BlockTableRecord;
                }
     
                BLine = new Line(PositionPoint.Convert3D(-0.5 * WdLeng, 0), PositionPoint.Convert3D(0.5 * WdLeng, 0));
                SLine = new Line(PositionPoint.Convert3D(-0.5 * WdLeng, -1*scale), PositionPoint.Convert3D(0.5 * WdLeng, -1 * scale));
                BLine.Layer = "标注";
                BLine.LineWeight = LineWeight.LineWeight050;
                SLine.Layer = "标注";
                BLine.ColorIndex = 1;


                title = new DBText();
                title.TextString = textstring;
                title.Height = 5 * scale;
                title.Position = PositionPoint.Convert3D();
                title.HorizontalMode = TextHorizontalMode.TextCenter;
                title.VerticalMode = TextVerticalMode.TextBottom;
                title.AlignmentPoint = PositionPoint.Convert3D(0,0.5*scale);       

                scalestring.Position = PositionPoint.Convert3D();
                scalestring.Height = 3.5 * scale;
                scalestring.HorizontalMode = TextHorizontalMode.TextCenter;
                scalestring.VerticalMode = TextVerticalMode.TextTop;
                scalestring.AlignmentPoint = PositionPoint.Convert3D(0,-2*scale);
                title.TextStyleId = st["仿宋"];
                scalestring.TextStyleId = st["仿宋"];
                title.Layer = "标注";
                scalestring.Layer = "标注";
                title.WidthFactor = 0.75;
                scalestring.WidthFactor = 0.75;                

                modelSpace.AppendEntity(BLine);
                tr.AddNewlyCreatedDBObject(BLine, true);
                modelSpace.AppendEntity(SLine);
                tr.AddNewlyCreatedDBObject(SLine, true);
                modelSpace.AppendEntity(title);
                tr.AddNewlyCreatedDBObject(title, true);
                modelSpace.AppendEntity(scalestring);
                tr.AddNewlyCreatedDBObject(scalestring, true);
                tr.Commit();
            }
            return ;

        }



        /// <summary>
        /// 剖面标注
        /// </summary>
        /// <param name="db"></param>
        /// <param name="theName"></param>
        /// <param name="StartPoint"></param>
        /// <param name="EndPoint"></param>
        /// <param name="scale"></param>
        public static void DimSection(Database db, char theName, Point2d StartPoint,Point2d EndPoint, double scale = 100)
        {

            
            Line[] Lset = new Line[2];
            Polyline[] PLset = new Polyline[2];
            double ang = new Line2d(StartPoint, EndPoint).Direction.Angle;               
                
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                for (int i = 0; i < 2; i++)
                {
                    DBText t = new DBText()
                    {
                        TextString = theName.ToString(),
                        Layer = "标注",
                        Height = 4 * scale,
                        TextStyleId = st["fsdb"],
                        HorizontalMode = TextHorizontalMode.TextCenter,
                        VerticalMode = TextVerticalMode.TextBottom,
                        WidthFactor = 0.75,
                        Rotation = ang,
                    };
                    if (i == 0)
                    {
                        t.AlignmentPoint = StartPoint.Convert3D(0, 0.5 * scale);
                    }
                    else
                    {
                        t.AlignmentPoint = EndPoint.Convert3D(0, 0.5 * scale);
                    }
                    modelSpace.AppendEntity(t);
                    tr.AddNewlyCreatedDBObject(t, true);


                    Line L;
                    Polyline PL=new Polyline();
                    Point2d p0=new Point2d(), p1=new Point2d();

                    L = i == 0 ? new Line(StartPoint.Convert3D(-1.5 * scale, 0), StartPoint.Convert3D(1.5 * scale, 0)) :
                        new Line(EndPoint.Convert3D(-1.5 * scale, 0), EndPoint.Convert3D(1.5 * scale, 0));
                    p0 = i == 0 ? StartPoint : EndPoint;
                    p1 = i == 0 ? StartPoint.Convert2D(0, -2 * scale) : EndPoint.Convert2D(0, -2 * scale);

                    L.Layer = "标注";
                    L.ColorIndex = 1;                    
                    L.TransformBy(Matrix3d.Rotation(ang,Vector3d.ZAxis,p0.Convert3D()));
                    modelSpace.AppendEntity(L);
                    tr.AddNewlyCreatedDBObject(L, true);

                    PL.AddVertexAt(0,p0, 0, 0, 2*scale);
                    PL.AddVertexAt(1, p1, 0, 0, 0);
                    PL.TransformBy(Matrix3d.Rotation(ang, Vector3d.ZAxis, p0.Convert3D()));
                    modelSpace.AppendEntity(PL);
                    tr.AddNewlyCreatedDBObject(PL, true);
                }
                tr.Commit();
            }

        }

        public static void DimRoadDirect(Database db, Point2d AnchorPoint, double scale,bool isUp)
        {
            Polyline Arrow=new Polyline();
            DBText txt=new DBText();
            int diry = isUp ? 1 : -1;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                txt.TextString =isUp?"DABOLA":"COYAH";
                txt.Layer = "标注";
                txt.Height = 3 * scale;
                txt.TextStyleId = st["fsdb"];
                txt.Position = AnchorPoint.Convert3D();
                txt.HorizontalMode = TextHorizontalMode.TextMid;
                txt.VerticalMode = isUp?TextVerticalMode.TextBottom:TextVerticalMode.TextTop;
                txt.AlignmentPoint = AnchorPoint.Convert3D(0,1.2*scale*diry);
                txt.WidthFactor = 0.75;
                modelSpace.AppendEntity(txt);
                tr.AddNewlyCreatedDBObject(txt, true);      
                Point2d p0 = AnchorPoint;
                Point2d p1 = p0.Convert2D(0, -diry * 7.5 * scale);
                Point2d p2 = p1.Convert2D(-1 * scale);
                Point2d p3 = p2.Convert2D(0, diry * 3.5 * scale);
                Point2d p4 = p3.Convert2D(-1.5 * scale);
                Arrow.AddVertexAt(0, p0, 0, 0, 0);
                Arrow.AddVertexAt(1, p1, 0, 0, 0);
                Arrow.AddVertexAt(2, p2, 0, 0, 0);
                Arrow.AddVertexAt(3, p3, 0, 0, 0);
                Arrow.AddVertexAt(4, p4, 0, 0, 0);
                Arrow.Layer = "标注";
                Arrow.Closed = true;
                modelSpace.AppendEntity(Arrow);
                tr.AddNewlyCreatedDBObject(Arrow, true);
                tr.Commit();
            }

        }





    }
}
