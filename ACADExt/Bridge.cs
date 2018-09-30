using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Reflection;
using System.IO;

namespace ACADExt
{
    public enum BridgeType
    {
        T321,HBD60
    }
    public class Bridge
    {
        public int ID { get; set; } // 序号
        public string Name { get; set; }  // 桥名
        public BridgeType Type { get; set; } // 桥梁类型
        public int Columns { get; set; }  // 排数
        public int Layers { get; set; }  // 层数
        public int Pieces { get; set; }  // 节数
        public bool IsEnhence { get; set; }  // 是否增强
        public bool IsTriple { get; set; }   // 是否有第三弦杆
        public double Slop { set; get; }
        public bool IsValid
        {
            get
            {
                if (Columns != 0 && Pieces != 0 && Layers != 0)
                {
                    return true;
                }
                return false;
            }
        }

        public static System.Data.DataTable MatTab;
        public static System.Data.DataTable NumTab;






        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">序号</param>
        /// <param name="nm">桥名</param>
        /// <param name="brt">结构类型</param>
        /// <param name="col">排</param>
        /// <param name="lay">层</param>
        /// <param name="pc">块</param>
        /// <param name="isE">是否加强</param>
        /// <param name="isT">是否第三弦杆</param>
        /// <param name="ss">纵坡</param>
        public Bridge(int id, string nm, BridgeType brt, int col,int lay,int pc,bool isE, bool isT,double ss)
        {
            ID = id;
            Name = nm;
            Type = brt;
            Columns = col;
            Layers = lay;
            Pieces = pc;
            IsEnhence = isE;
            IsTriple = isT;
            Slop = ss;
            MatTab = GetMatTab();
            NumTab = GetTab("NumberTable.csv");



        }
        public Bridge(System.Data.DataTable theDT, int DalotId)
        {
            var results = from myRow in theDT.AsEnumerable()
                          where myRow.Field<int>("序号") == DalotId
                          select myRow;
            var theData = results.First();
            ID = (int)theData["序号"];

            Name = (string)theData["桥名"];
            Type = (string)theData["类型"]=="T321"?BridgeType.T321:BridgeType.HBD60;
            Columns = (int)theData["排数"];
            Layers = (int)theData["层数"];
            Pieces = (int)theData["节数"]; 
            IsEnhence = (bool)theData["是否增强"] ;
            IsTriple = (bool)theData["是否有第三弦杆"];
            Slop = (double)theData["纵坡"] ;
            MatTab = GetMatTab();
            NumTab = GetTab("NumberTable.csv");
        }
        public Bridge()
        {
            ID = 0;
            Name = "";
            Type = 0;
            Columns = 1;
            Layers = 1;
            Pieces = 1;
            IsEnhence = false;
            IsTriple = false;
            Slop = 0.0;
            MatTab = GetMatTab();
            NumTab = GetTab("NumberTable.csv");
        }



        /// <summary>
        /// 绘制平立面
        /// </summary>
        /// <param name="db"></param>
        /// <param name="AnchorPoint"></param>
        public double PlotA(Database db, Point2d AnchorPoint,int ScaleA=0)
        {
            Transaction tr = db.TransactionManager.StartTransaction();
            BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord ms = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                OpenMode.ForWrite) as BlockTableRecord;
            DimStyleTable dst = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);

            if (ScaleA==0)
            {
                ScaleA = GetScaleA();
            }
            double dy = ScaleA * -50;

            // 立面图
            ObjectId blkRecId = blockTbl["T321-1"];
            ObjectId HLJJ = blockTbl["T321-HLJJ"];
            ObjectId QMB = blockTbl["T321-QMB"];
            ObjectId HL = blockTbl["T321-HL-C"+Columns.ToString()];
            ObjectId T321PM = blockTbl["T321-2"];
            ObjectId XZ = blockTbl["T321-XZ"];
            ObjectId QZC1 = blockTbl["T321-QZ-C1"];
            ObjectId QZC1R = blockTbl["T321-QZ-C1R"];
            ObjectId QZC2 = blockTbl["T321-QZ-C2"];
            ObjectId QZC2R = blockTbl["T321-QZ-C2R"];
            ObjectId ZBC1 = blockTbl["T321-ZB-C1"];
            ObjectId ZBC2 = blockTbl["T321-ZB-C2"];
            ObjectId ZBC3 = blockTbl["T321-ZB-C3"];
            ObjectId ZBC3L = blockTbl["T321-ZB-C3L"];
            ObjectId ZCJ = blockTbl["T321-ZCJ-2"];
            ObjectId KFLG = blockTbl["T321-KFLG"];
            ObjectId QMB2 = blockTbl["T321-QMB-2"];
            ObjectId JQXG = blockTbl["T321-JQXG"];

            // 立面桁架
            for (int i = 0; i < Pieces; i++)
            {
                for (int j = 0; j < Layers; j++)
                {
                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(0+i*3000,0+j*1500), blkRecId))
                    {
                        ms.AppendEntity(acBlkRef);
                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                    }
                    if (IsEnhence&&i!=0&&i!=(Pieces-1))
                    {
                        if (j == 0)
                        {
                            using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(0 + i * 3000, 0 + j * 1500-100), JQXG))
                            {
                                ms.AppendEntity(acBlkRef);
                                tr.AddNewlyCreatedDBObject(acBlkRef, true);
                            }
                        }
                        else if (j == Layers - 1)
                        {
                            using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(0 + i * 3000, 0 + j * 1500+1500), JQXG))
                            {
                                ms.AppendEntity(acBlkRef);
                                tr.AddNewlyCreatedDBObject(acBlkRef, true);
                            }
                        }

                    }







                    if (j == 0)
                    {
                        using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(1413 + i * 3000, 328), HLJJ))
                        {
                            ms.AppendEntity(acBlkRef);
                            tr.AddNewlyCreatedDBObject(acBlkRef, true);
                        }
                        using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(1413 + i * 3000, dy), HL))
                        {
                            ms.AppendEntity(acBlkRef);
                            tr.AddNewlyCreatedDBObject(acBlkRef, true);
                        }
                        using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(1413+1410 + i * 3000, 328), HLJJ))
                        {
                            ms.AppendEntity(acBlkRef);
                            tr.AddNewlyCreatedDBObject(acBlkRef, true);
                        }
                        using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(1413 + 1410 + i * 3000, dy), HL))
                        {
                            ms.AppendEntity(acBlkRef);
                            tr.AddNewlyCreatedDBObject(acBlkRef, true);
                        }
                        using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(1323 + i * 3000,553), QMB))
                        {
                            ms.AppendEntity(acBlkRef);
                            tr.AddNewlyCreatedDBObject(acBlkRef, true);
                        }
                    }
                }
            }            
            blkRecId = blockTbl["T321-ZCJ-1"];
            double offsetZCJ = 0.0;

            if (Columns != 1)
            {
                for (int i = 0; i < Pieces; i++)
                {
                    if (IsEnhence)
                    {
                        if (i == 0 || i == (Pieces - 1))
                        {
                            offsetZCJ = 0;
                        }
                        else
                        {
                            offsetZCJ = 100;
                        }
                    } 
                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(910 + i * 3000, (Layers) * 1500-70+ offsetZCJ), blkRecId))
                    {
                        ms.AppendEntity(acBlkRef);
                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                    }
                }
            }

            blkRecId = blockTbl["T321-YTDZ"];
            using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(), blkRecId))
            {
                ms.AppendEntity(acBlkRef);
                tr.AddNewlyCreatedDBObject(acBlkRef, true);
            }
            blkRecId = blockTbl["T321-HL-1"];
            using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(-157,58), blkRecId))
            {
                ms.AppendEntity(acBlkRef);
                tr.AddNewlyCreatedDBObject(acBlkRef, true);
            }
            using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(-157, dy), HL))
            {
                ms.AppendEntity(acBlkRef);
                tr.AddNewlyCreatedDBObject(acBlkRef, true);
            }




            blkRecId = blockTbl["T321-ITDZ"];
            using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(Pieces * 3000.0), blkRecId))
            {
                ms.AppendEntity(acBlkRef);
                tr.AddNewlyCreatedDBObject(acBlkRef, true);
            }
            blkRecId = blockTbl["T321-QZ"];
            using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(Pieces * 3000.0+37,-145), blkRecId))
            {
                ms.AppendEntity(acBlkRef);
                tr.AddNewlyCreatedDBObject(acBlkRef, true);
            }
            using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D( - 37, -145), blkRecId))
            {
                ms.AppendEntity(acBlkRef);
                tr.AddNewlyCreatedDBObject(acBlkRef, true);
            }
            blkRecId = blockTbl["T321-ZB"];
            using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(Pieces * 3000.0 +37, -297), blkRecId))
            {
                ms.AppendEntity(acBlkRef);
                tr.AddNewlyCreatedDBObject(acBlkRef, true);
            }
            using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(-37, -297), blkRecId))
            {
                ms.AppendEntity(acBlkRef);
                tr.AddNewlyCreatedDBObject(acBlkRef, true);
            }


            // 平面桁架
            for (int k = 0; k < 2; k++)
            {
                double sidey = (-4200 ) * k;
                int dir = k == 0 ? 1 : -1;
                for (int i = 0; i < Columns; i++)
                {
                    double cy;
                    if (i == 0)
                    {
                        cy = 0;
                    }
                    else if(i==1)
                    {
                        cy =dir* 450;
                    }
                    else
                    {
                        cy = (450 + 250)*dir;
                    }
                    
                    for (int j = 0; j < Pieces; j++)
                    {
                        using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(j*3000,dy-825+sidey+cy), T321PM))
                        {
                            ms.AppendEntity(acBlkRef);
                            tr.AddNewlyCreatedDBObject(acBlkRef, true);
                        }
                        if (j != 0)
                        {
                            using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(j * 3000, dy - 825 + sidey + cy-75.3), XZ))
                            {
                                ms.AppendEntity(acBlkRef);
                                tr.AddNewlyCreatedDBObject(acBlkRef, true);
                            }

                        }
                        if (i == 0)
                        {
                            if (Columns != 1)
                            {
                                using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(j * 3000 + 910, dy - 825 + sidey + dir * 450), ZCJ))
                                {
                                    if (dir == -1)
                                    {
                                        acBlkRef.TransformBy(Matrix3d.Mirroring(new Line3d(
                                        AnchorPoint.Convert3D(j * 3000 + 910, dy - 825 + sidey + dir * 450),
                                        AnchorPoint.Convert3D(j * 3000 + 910 + 100, dy - 825 + sidey + dir * 450)
                                        )));
                                    }
                                    ms.AppendEntity(acBlkRef);
                                    tr.AddNewlyCreatedDBObject(acBlkRef, true);
                                }
                            }
                            if(k == 0)
                            {
                                if (j < 0.5 * Pieces)
                                {
                                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(j * 3000 + 329, dy - 825 + sidey), KFLG))
                                    {
                                        ms.AppendEntity(acBlkRef);
                                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                                    }
                                }
                                else
                                {
                                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(j * 3000 + 2821, dy - 825 + sidey - 180), QMB2))
                                    {
                                        ms.AppendEntity(acBlkRef);
                                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                                    }
                                }
                            }


                        }
                    }

                }

                if (Columns == 1)
                {
                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(-37, dy - 825 + sidey), QZC1))
                    {
                        ms.AppendEntity(acBlkRef);
                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                    }
                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(Pieces*3000+37, dy - 825 + sidey), QZC1R))
                    {
                        ms.AppendEntity(acBlkRef);
                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                    }
                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(-37, dy - 825 + sidey), ZBC1))
                    {
                        acBlkRef.TransformBy(Matrix3d.Mirroring(new Line3d(
                            AnchorPoint.Convert3D(-37, dy - 825 + sidey), 
                            AnchorPoint.Convert3D(-37, dy - 825 + sidey + 100)
                            )));
                        ms.AppendEntity(acBlkRef);
                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                    }
                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(37+Pieces*3000, dy - 825 + sidey), ZBC1))
                    {
                        ms.AppendEntity(acBlkRef);
                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                    }
                }

                else if (Columns == 2)
                {
                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(-37, dy - 825 + sidey), QZC1))
                    {
                        ms.AppendEntity(acBlkRef);
                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                    }
                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(-37, dy - 825 + sidey+dir*450), QZC1))
                    {
                        ms.AppendEntity(acBlkRef);
                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                    }
                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(Pieces * 3000 + 37, dy - 825 + sidey), QZC1R))
                    {
                        ms.AppendEntity(acBlkRef);
                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                    }
                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(Pieces * 3000 + 37, dy - 825 + sidey+dir*450), QZC1R))
                    {
                        ms.AppendEntity(acBlkRef);
                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                    }
                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(Pieces * 3000+37, dy - 825 + sidey+dir*225), ZBC2))
                    {
                        ms.AppendEntity(acBlkRef);
                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                    }
                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(-37, dy - 825 + sidey + dir * 225), ZBC2))
                    {
                        acBlkRef.TransformBy(Matrix3d.Mirroring(new Line3d(AnchorPoint.Convert3D(-37, dy - 825 + sidey + dir * 225), AnchorPoint.Convert3D(-37, dy - 825 + sidey + dir * 225+100))));
                        ms.AppendEntity(acBlkRef);
                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                    }
                }
                else
                {
                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(-37, dy - 825 + sidey), QZC1))
                    {
                        ms.AppendEntity(acBlkRef);
                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                    }
                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(-37, dy - 825 + sidey + dir * (450+125)), QZC2))
                    {
                        ms.AppendEntity(acBlkRef);
                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                    }
                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(Pieces * 3000 + 37, dy - 825 + sidey), QZC1R))
                    {
                        ms.AppendEntity(acBlkRef);
                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                    }
                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(Pieces * 3000 + 37, dy - 825 + sidey + dir * (450 + 125)), QZC2R))
                    {
                        ms.AppendEntity(acBlkRef);
                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                    }

                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(Pieces * 3000 + 37, dy - 825 + sidey + dir * 350), ZBC3))
                    {
                        ms.AppendEntity(acBlkRef);
                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                    }

                    using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(-37, dy - 825 + sidey + dir * 350), ZBC3))
                    {
                        acBlkRef.TransformBy(Matrix3d.Mirroring(new Line3d(
                            AnchorPoint.Convert3D(-37, dy - 825 + sidey + dir * 350),
                            AnchorPoint.Convert3D(-37, dy - 825 + sidey + dir * 350+100)
                            )));
                        if (dir == -1)
                        {
                            acBlkRef.TransformBy(Matrix3d.Mirroring(new Line3d(
                            AnchorPoint.Convert3D(-37, dy - 825 + sidey + dir * 350),
                            AnchorPoint.Convert3D(-37+100, dy - 825 + sidey + dir * 350 )
                            )));
                        }
                        ms.AppendEntity(acBlkRef);
                        tr.AddNewlyCreatedDBObject(acBlkRef, true);
                    }

                }

            }
            tr.Commit();
            tr.Dispose();

            // 标注


            var DimStyleID = dst["1-" + ScaleA.ToString()];
            Point3d SW = AnchorPoint.Convert3D();
            Point3d SE = AnchorPoint.Convert3D(Pieces * 3000.0);
            Point3d NW = SW.Convert3D(0, Layers * 1400 + (Layers - 1) * 100);
            Point3d NE = SE.Convert3D(0, Layers * 1400 + (Layers - 1) * 100);
            Point3d CT = NW.Convert3D(0.5 * Pieces * 3000.0);

            Point3d pNW = SW.Convert3D(0, dy-825);
            Point3d pNE = SE.Convert3D(0, dy-825);
            Point3d pSW = pNW.Convert3D(0,-4200);
            Point3d pSE = pNE.Convert3D(0,- 4200);
            Point3d pCT = pNW.Convert3D(0.5 * Pieces * 3000.0);

            string stmp = string.Format("{0}×3000=<>", Pieces);
            DimPloter.Dim0(db, NW, NE, NE.Convert3D(0, 10 * ScaleA), DimStyleID,0,stmp);            
            DimPloter.Dim0(db, SW, NW, NW.Convert3D(-10*ScaleA), DimStyleID, 0.5 * Math.PI);
            DimPloter.Dim0(db, SE, SE.Convert3D(0,1400), SE.Convert3D(8 * ScaleA), DimStyleID, 0.5 * Math.PI);
            for (int i = 0; i < Layers-1; i++)
            {
                DimPloter.Dim0(db, SE.Convert3D(0, 1500 * (i + 1) - 100), SE.Convert3D(0, 1500 * (i + 1)), SE.Convert3D(8 * ScaleA), DimStyleID, 0.5 * Math.PI);
                DimPloter.Dim0(db, SE.Convert3D(0, 1500 * (i + 1)), SE.Convert3D(0, 1500 * (i + 1)+1400), SE.Convert3D(8 * ScaleA), DimStyleID, 0.5 * Math.PI);
            }
            DimPloter.Dim0(db, pSW, pSE, pSW.Convert3D(0, -15 * ScaleA), DimStyleID);
            DimPloter.Dim0(db, pSW, pNW, pNW.Convert3D( -10 * ScaleA), DimStyleID,0.5*Math.PI);

            double ymin = pSW.Convert3D(0, -15 * ScaleA).Y;

            // 图名

            TextPloter.PrintTitle(db, "立 面", CT.Convert2D(0, 25 * ScaleA), ScaleA);
            TextPloter.PrintTitle(db, "平 面", pCT.Convert2D(0, 20 * ScaleA), ScaleA);
            double ymax = CT.Convert2D(0, 25 * ScaleA).Y;
            return ymax-AnchorPoint.Y;
        }
        /// <summary>
        /// 绘制断面、量表
        /// </summary>
        /// <param name="db"></param>
        /// <param name="AnchorPoint"></param>
        public void PlotB(Database db, Point2d AnchorPoint)
        {
            int ScaleB = 50;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord ms = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;
                DimStyleTable dst = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                string blkname = string.Format("T321-C{0}L{1}", Columns, Layers);
                ObjectId blkRecId = blockTbl[blkname];
                using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(), blkRecId))
                {
                    ms.AppendEntity(acBlkRef);
                    tr.AddNewlyCreatedDBObject(acBlkRef, true);
                }
                tr.Commit();
            }

            // 图名
            TextPloter.PrintTitle(db, "标准横断面", AnchorPoint.Convert2D(0, 20 * ScaleB), ScaleB);

        }



        public void PlotNumTB(Database db, Point2d AnchorPoint)
        {
            string enh;
            if (IsEnhence)
            {
                enh = "R";
            }
            else
            {
                enh = "N";
            }
            string colname = string.Format("C{0}L{1}-{2}-", Columns, Layers, enh);

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord ps = tr.GetObject(blockTbl[BlockTableRecord.PaperSpace],
                    OpenMode.ForWrite) as BlockTableRecord;
                DimStyleTable dst = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                ObjectId blkRecId = blockTbl["T321-NumberTable"];
                using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(), blkRecId))
                {
                    ps.AppendEntity(acBlkRef);
                    tr.AddNewlyCreatedDBObject(acBlkRef, true);
                }

                DBText title = new DBText();
                title.TextString = "部件数量表";
                title.Height = 5;
                title.Position = AnchorPoint.Convert3D(0, 7);
                title.HorizontalMode = TextHorizontalMode.TextCenter;
                title.VerticalMode = TextVerticalMode.TextBottom;
                title.AlignmentPoint = title.Position;
                title.TextStyleId = st["仿宋"];
                title.Layer = "标注";
                title.WidthFactor = 0.8;
                ps.AppendEntity(title);
                tr.AddNewlyCreatedDBObject(title, true);


                DBText data;


                for (int i = 0; i < NumTab.Rows.Count; i++)
                {
                    int NofFirst = Convert2Int(NumTab.Rows[i][colname + "F"]);
                    int NofMid = Convert2Int(NumTab.Rows[i][colname + "M"]) * (Pieces - 2);
                    int NofEnd = Convert2Int(NumTab.Rows[i][colname + "E"]);
                    int sum = NofFirst + NofMid + NofEnd;

                    int k = 0;

                    foreach (var item in new List<int>() { NofFirst, NofMid, NofEnd, sum })
                    {
                        data = new DBText();
                        data.Height = 2.5;
                        data.TextString = item.ToString();
                        double dx = 8*k+5.5;
                        double dy = -9-6*i;
                        data.Position = AnchorPoint.Convert3D(dx, dy);
                        data.HorizontalMode = TextHorizontalMode.TextCenter;
                        data.VerticalMode = TextVerticalMode.TextVerticalMid;
                        data.AlignmentPoint = data.Position;
                        data.TextStyleId = st["仿宋"];
                        data.Layer = "标注";
                        data.WidthFactor = 0.8;
                        ps.AppendEntity(data);
                        tr.AddNewlyCreatedDBObject(data, true);

                        k++;
                    }                    
                }
                tr.Commit();
            }
        }






        /// <summary>
        /// 绘制图表
        /// </summary>
        /// <param name="db"></param>
        /// <param name="AnchorPoint"></param>
        public void PlotMatTB(Database db,Point2d AnchorPoint)
        {
            string enh;
            if (IsEnhence)
            {
                enh = "R";
            }
            else
            {
                enh = "N";
            }
            string colname=string.Format("C{0}L{1}-{2}-",Columns,Layers,enh);
            List<double> res=new List<double>();

            for (int i = 0; i < MatTab.Rows.Count; i++)
            {
                var xx = Convert2Double(MatTab.Rows[i][colname + "F"])
                    + Convert2Double(MatTab.Rows[i][colname + "M"])*(Pieces-2)
                    + Convert2Double(MatTab.Rows[i][colname + "E"]);
                res.Add(xx);
            }


                
            

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord ps = tr.GetObject(blockTbl[BlockTableRecord.PaperSpace],
                    OpenMode.ForWrite) as BlockTableRecord;
                DimStyleTable dst = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                ObjectId blkRecId = blockTbl["T321-Table"];
                using (BlockReference acBlkRef = new BlockReference(AnchorPoint.Convert3D(), blkRecId))
                {
                    ps.AppendEntity(acBlkRef);
                    tr.AddNewlyCreatedDBObject(acBlkRef, true);
                }

                DBText title = new DBText();                
                title.TextString = "材料汇总表";
                title.Height = 5 ;
                title.Position = AnchorPoint.Convert3D(0,7);
                title.HorizontalMode = TextHorizontalMode.TextCenter;
                title.VerticalMode = TextVerticalMode.TextBottom;
                title.AlignmentPoint = title.Position;
                title.TextStyleId = st["仿宋"];                
                title.Layer = "标注";                
                title.WidthFactor = 0.8;
                ps.AppendEntity(title);
                tr.AddNewlyCreatedDBObject(title, true);


                DBText data;
                for (int i = 0; i < res.Count; i++)
                {
                    data = new DBText();
                    data.Height = 2.5;
                    data.TextString =string.Format("{0:F2}", res[i]);
                    double dx = 0;
                    double dy = 0;
                    if (i < 16)
                    {
                        dy = -20;
                        dx = i * 18 - 133;
                    }
                    else
                    {
                        dy = -44;
                        dx = (i - 16) * 18 - 133;
                    }
                    data.Position = AnchorPoint.Convert3D(dx, dy);
                    data.HorizontalMode = TextHorizontalMode.TextCenter;
                    data.VerticalMode = TextVerticalMode.TextVerticalMid;
                    data.AlignmentPoint = data.Position;
                    data.TextStyleId = st["仿宋"];
                    data.Layer = "标注";
                    data.WidthFactor = 0.8;
                    ps.AppendEntity(data);
                    tr.AddNewlyCreatedDBObject(data, true);
                }

                MText sum = new MText();   
                sum.Contents=string.Format("钢材合计:\n{0} kg",res.Sum());
                sum.TextHeight = 2.5;
                sum.TextStyleId= st["仿宋"];
                sum.Location= AnchorPoint.Convert3D(155-7.5, -24+4.2);
                sum.Layer = "标注";                
                ps.AppendEntity(sum);
                tr.AddNewlyCreatedDBObject(sum, true);








                tr.Commit();

            }
        }











        public int GetScaleA()
        {
            int ScaleA=0;

            if (Pieces <= 0)
            {
                ScaleA = 50;
            }
            else if (Pieces <= 7)
            {
                ScaleA = 75;
            }
            else
            {
                ScaleA = 100;
            }
            return ScaleA;
        }


        public static int Convert2Int(object ss)
        {
            string inp = ss.ToString();
            return inp == "-" ? 0 : Convert.ToInt32(ss);
        }

        public static double Convert2Double(object ss)
        {
            string inp = ss.ToString();
            return inp == "-" ? 0 : Convert.ToDouble(ss);
        }

        public static System.Data.DataTable OpenCSV(Stream fs,  bool haveTitleRow = true)
        {
            System.Data.DataTable dt = new System.Data.DataTable();          


            //FileStream fs = new FileStream(fullFileName, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine;
            //标示列数
            int columnCount = 0;
            //是否已建立了表的字段
            bool bCreateTableColumns = false;

            string[] separators = { "," };
            //逐行读取CSV中的数据
            
            while ((strLine = sr.ReadLine()) != null)
            {
                strLine = strLine.Trim();
                aryLine = strLine.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                if (bCreateTableColumns == false)
                {
                    bCreateTableColumns = true;
                    columnCount = aryLine.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        System.Data.DataColumn dc
                            = new System.Data.DataColumn(haveTitleRow == true ? aryLine[i] : "COL" + i.ToString());
                        dt.Columns.Add(dc);
                    }
                    bCreateTableColumns = true;

                    if (haveTitleRow == true)
                    {
                        continue;
                    }
                }

                DataRow dr = dt.NewRow();
                for (int j = 0; j < columnCount; j++)
                {
                    dr[j] = aryLine[j];
                }
                dt.Rows.Add(dr);

            }
            return dt;
        }


        private static System.Data.DataTable GetTab(string ss)
        {
            //获得正在运行类所在的名称空间
            Type type = MethodBase.GetCurrentMethod().DeclaringType;
            string _namespace = type.Namespace;

            //获得当前运行的Assembly
            Assembly _assembly = Assembly.GetExecutingAssembly();

            //根据名称空间和文件名生成资源名称
            string resourceName = _namespace + string.Format(".Resource.{0}",ss);

            Stream stream = _assembly.GetManifestResourceStream(resourceName);
            MemoryStream ms = CopyToMemory(stream);

            return OpenCSV(ms);
        }


        private static System.Data.DataTable GetMatTab()
        {
            //获得正在运行类所在的名称空间
            Type type = MethodBase.GetCurrentMethod().DeclaringType;
            string _namespace = type.Namespace;

            //获得当前运行的Assembly
            Assembly _assembly = Assembly.GetExecutingAssembly();

            //根据名称空间和文件名生成资源名称
            string resourceName = _namespace + ".Resource.MaterialTable.csv";

            Stream stream = _assembly.GetManifestResourceStream(resourceName);
            MemoryStream ms = CopyToMemory(stream);

            return OpenCSV(ms);
        }


        public static MemoryStream CopyToMemory(Stream input)
        {
            // It won't matter if we throw an exception during this method;
            // we don't *really* need to dispose of the MemoryStream, and the
            // caller should dispose of the input stream
            MemoryStream ret = new MemoryStream();

            byte[] buffer = new byte[8192];
            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ret.Write(buffer, 0, bytesRead);
            }
            // Rewind ready for reading (typical scenario)
            ret.Position = 0;
            return ret;
        }

    }
}
