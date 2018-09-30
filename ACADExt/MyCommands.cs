﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

[assembly: CommandClass(typeof(ACADExt.MyCommands))]

namespace ACADExt
{
    public class MyCommands
    {
        public static void CADRead(string filename)
        {
            Document doc = Application.DocumentManager.Open(filename, false);
            Database db = doc.Database;
            var CurEditor = doc.Editor;
            double f = 1.0;
            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = "\nSelect annotative objects";
            var psr = CurEditor.GetSelection(pso);

            if (psr.Status != PromptStatus.OK)
            {
                return;
            }
            var map = new Dictionary<ObjectId, string>();

            foreach (dynamic id in psr.Value.GetObjectIds())
            {
                map.Add(id, id.Layer);
            }

            var sorted = map.OrderBy(kv => kv.Value);

            // Print them in order to the command-line


            foreach (var item in sorted)
            {
                CurEditor.WriteMessage("\nObject {0} on layer {1}", item.Key, item.Value);
            }


            doc.CloseAndDiscard();
        }
        //-------------------------------------------------------------------------------------------
        [CommandMethod("ini")]
        public static void CADini()
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor ed = acDoc.Editor;
            // Start a transaction
            using (Transaction tr = acCurDb.TransactionManager.StartTransaction())
            {
                Dictionary<string, short> ldic = new Dictionary<string, short>()
                {
                    ["粗线"] = 4,
                    ["细线"] = 2,
                    ["标注"] = 7,
                    ["中心线"] = 1,
                    ["虚线"] = 3,
                    ["填充"] = 8,
                    ["图框"] = 8,
                    ["地质"] = 8,
                };
                List<string> Lname = new List<string>() { "CENTER", "DASHED" };
                LayerTable acLyrTbl;
                acLyrTbl = tr.GetObject(acCurDb.LayerTableId, OpenMode.ForRead) as LayerTable;
                LinetypeTable acLinTbl;
                acLinTbl = tr.GetObject(acCurDb.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;
                foreach (string ltname in Lname)
                {
                    if (!acLinTbl.Has(ltname))
                    {
                        acCurDb.LoadLineTypeFile(ltname, "acad.lin");
                    }
                }
                LayerTableRecord acLyrTblRec = new LayerTableRecord();
                foreach (string key in ldic.Keys)
                {
                    short cid = ldic[key];
                    acLyrTblRec = new LayerTableRecord();
                    if (!acLyrTbl.Has(key))
                    {
                        acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, cid);
                        if (cid != 4) { acLyrTblRec.LineWeight = LineWeight.LineWeight013; }
                        else { acLyrTblRec.LineWeight = LineWeight.LineWeight030; }
                        if (cid == 1) { acLyrTblRec.LinetypeObjectId = acLinTbl["CENTER"]; }
                        if (cid == 3) { acLyrTblRec.LinetypeObjectId = acLinTbl["DASHED"]; }
                        if (key == "图框") { acLyrTblRec.IsPlottable = false; }
                        if (key == "地质") { acLyrTblRec.IsPlottable = false; }
                        acLyrTblRec.Name = key;
                        if (acLyrTbl.IsWriteEnabled == false) acLyrTbl.UpgradeOpen();
                        acLyrTbl.Add(acLyrTblRec);
                        tr.AddNewlyCreatedDBObject(acLyrTblRec, true);
                    }
                    else
                    {
                        acLyrTblRec = tr.GetObject(acLyrTbl[key], OpenMode.ForWrite) as LayerTableRecord;
                        acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, cid);
                        if (cid != 4) { acLyrTblRec.LineWeight = LineWeight.LineWeight013; }
                        else { acLyrTblRec.LineWeight = LineWeight.LineWeight030; }
                        if (cid == 1) { acLyrTblRec.LinetypeObjectId = acLinTbl["CENTER"]; }
                        if (cid == 3) { acLyrTblRec.LinetypeObjectId = acLinTbl["DASHED"]; }
                        if (key == "图框") { acLyrTblRec.IsPlottable = false; }
                        if (key == "地质") { acLyrTblRec.IsPlottable = false; }
                    }
                }
                if (!acLyrTbl.Has("sjx"))
                {
                    acLyrTblRec = new LayerTableRecord();
                    acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, 1);
                    acLyrTblRec.Name = "sjx";
                    acLyrTblRec.LineWeight = LineWeight.LineWeight015;
                    if (acLyrTbl.IsWriteEnabled == false) acLyrTbl.UpgradeOpen();
                    acLyrTbl.Add(acLyrTblRec);
                    tr.AddNewlyCreatedDBObject(acLyrTblRec, true);
                }
                if (!acLyrTbl.Has("dmx"))
                {
                    acLyrTblRec = new LayerTableRecord();
                    acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, 8);
                    acLyrTblRec.Name = "dmx";
                    acLyrTblRec.LineWeight = LineWeight.LineWeight015;
                    if (acLyrTbl.IsWriteEnabled == false) acLyrTbl.UpgradeOpen();
                    acLyrTbl.Add(acLyrTblRec);
                    tr.AddNewlyCreatedDBObject(acLyrTblRec, true);
                }


                //-------------------------------------------------------------------------------------------
                TextStyleTable st = tr.GetObject(acCurDb.TextStyleTableId, OpenMode.ForWrite) as TextStyleTable;
                if (!st.Has("EN"))
                {
                    TextStyleTableRecord str = new TextStyleTableRecord()
                    {
                        Name = "En",
                        FileName = "times.ttf",
                        XScale = 0.85,
                    };
                    st.Add(str);
                    tr.AddNewlyCreatedDBObject(str, true);
                }
                else
                {
                    TextStyleTableRecord str = tr.GetObject(st["En"], OpenMode.ForWrite) as TextStyleTableRecord;
                    str.FileName = "times.ttf";
                    str.XScale = 0.85;
                }
                if (!st.Has("fsdb"))
                {
                    TextStyleTableRecord str2 = new TextStyleTableRecord()
                    {
                        Name = "fsdb",
                        FileName = "fsdb_e.shx",
                        BigFontFileName = "fsdb.shx",
                        XScale = 0.75,
                    };
                    ObjectId textstyleid = st.Add(str2);
                    tr.AddNewlyCreatedDBObject(str2, true);
                }
                else
                {
                    TextStyleTableRecord str = tr.GetObject(st["fsdb"], OpenMode.ForWrite) as TextStyleTableRecord;
                    str.FileName = "fsdb_e.shx";
                    str.BigFontFileName = "fsdb.shx";
                    str.XScale = 0.75;
                }
                if (!st.Has("仿宋"))
                {
                    TextStyleTableRecord str2 = new TextStyleTableRecord()
                    {
                        Name = "仿宋",
                        FileName = "仿宋_GB2312.ttf",                        
                        XScale = 0.8,
                    };
                    ObjectId textstyleid = st.Add(str2);
                    tr.AddNewlyCreatedDBObject(str2, true);
                }
                else
                {
                    TextStyleTableRecord str = tr.GetObject(st["仿宋"], OpenMode.ForWrite) as TextStyleTableRecord;
                    str.FileName = "仿宋_GB2312.ttf";                    
                    str.XScale = 0.8;
                }
                if (!st.Has("钢筋"))
                {
                    TextStyleTableRecord str2 = new TextStyleTableRecord()
                    {
                        Name = "钢筋",
                        FileName = "FS-GB2312-Rebar.ttf",
                        XScale = 0.8,
                    };
                    ObjectId textstyleid = st.Add(str2);
                    tr.AddNewlyCreatedDBObject(str2, true);
                }
                else
                {
                    TextStyleTableRecord str = tr.GetObject(st["钢筋"], OpenMode.ForWrite) as TextStyleTableRecord;
                    str.FileName = "FS-GB2312-Rebar.ttf";
                    str.XScale = 0.8;
                }







                //-------------------------------------------------------------------------------------------
                DimStyleTable dst = (DimStyleTable)tr.GetObject(acCurDb.DimStyleTableId, OpenMode.ForWrite);
                foreach (int thescale in new int[] {50, 75, 100, 125, 150, 200 })
                {
                    string scname = "1-" + thescale.ToString();
                    DimStyleTableRecord dstr = new DimStyleTableRecord();
                    if (!dst.Has(scname))
                    {
                        dstr.Name = "1-" + thescale.ToString();
                        dstr.Dimscale = thescale;
                        dstr.Dimtxsty = st["仿宋"];
                        dstr.Dimclrd = Color.FromColorIndex(ColorMethod.ByAci, 6);
                        dstr.Dimclre = Color.FromColorIndex(ColorMethod.ByAci, 6);
                        dstr.Dimdli = 5.0;
                        dstr.Dimexe = 1.0;
                        dstr.Dimexo = 1.0;
                        dstr.DimfxlenOn = true;
                        dstr.Dimfxlen = 4;
                        dstr.Dimtxt = 2.5;
                        dstr.Dimasz = 1.5;
                        dstr.Dimtix = true;
                        dstr.Dimtmove = 1;
                        dstr.Dimtad = 1;
                        dstr.Dimgap = 0.8;
                        dstr.Dimdec = 0;
                        dstr.Dimtih = false;
                        dstr.Dimtoh = false;
                        dstr.Dimdsep = '.';
                        //dstr.Dimlfac = 0.1;
                        dst.Add(dstr);
                        tr.AddNewlyCreatedDBObject(dstr, true);
                    }
                    else
                    {
                        dstr = tr.GetObject(dst[scname], OpenMode.ForWrite) as DimStyleTableRecord;
                        dstr.Name = "1-" + thescale.ToString();
                        dstr.Dimscale = thescale;
                        dstr.Dimtxsty = st["fsdb"];
                        dstr.Dimclrd = Color.FromColorIndex(ColorMethod.ByAci, 6);
                        dstr.Dimclre = Color.FromColorIndex(ColorMethod.ByAci, 6);
                        dstr.Dimdli = 5.0;
                        dstr.Dimexe = 1.0;
                        dstr.Dimexo = 1.0;
                        dstr.DimfxlenOn = true;
                        dstr.Dimfxlen = 4;
                        dstr.Dimtxt = 2.5;
                        dstr.Dimasz = 1.5;
                        dstr.Dimtix = true;
                        dstr.Dimtmove = 1;
                        dstr.Dimtad = 1;
                        dstr.Dimgap = 0.8;
                        dstr.Dimdec = 0;
                        dstr.Dimtih = false;
                        dstr.Dimtoh = false;
                        dstr.Dimdsep = '.';
                        dstr.Dimlfac = 0.1;
                    }

                }
                //-------------------------------------------------------------------------------------------
                // 自定义块
                //-------------------------------------------------------------------------------------------
                BlockTable bt = (BlockTable)tr.GetObject(acCurDb.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = new BlockTableRecord();

                //-------------------------------------------------------------------------------------------
                if (!bt.Has("BG"))
                {
                    btr.Name = "BG";
                    bt.UpgradeOpen();
                    bt.Add(btr);
                    tr.AddNewlyCreatedDBObject(btr, true);
                    Polyline Paa = new Polyline()
                    {
                        //Color = Color.FromColorIndex(ColorMethod.ByAci, 9),
                        //Layer = "标注",
                    };
                    Paa.AddVertexAt(0, new Point2d(0, 0), 0, 0, 200);
                    Paa.AddVertexAt(1, new Point2d(0, 200), 0, 0, 0);
                    btr.AppendEntity(Paa);
                    tr.AddNewlyCreatedDBObject(Paa, true);
                    AttributeDefinition curbg = new AttributeDefinition();
                    curbg.Position = new Point3d(120, 200, 0);
                    curbg.Height = 250;
                    curbg.WidthFactor = 0.75;
                    curbg.Tag = "标高";
                    //curbg.Layer = "标注";
                    curbg.TextStyleId = st["fsdb"];
                    btr.AppendEntity(curbg);
                    tr.AddNewlyCreatedDBObject(curbg, true);
                }
                //-------------------------------------------------------------------------------------------
                if (!bt.Has("ZP"))
                {
                    BlockTableRecord btr2 = new BlockTableRecord();
                    btr2.Name = "ZP";
                    bt.UpgradeOpen();
                    bt.Add(btr2);
                    tr.AddNewlyCreatedDBObject(btr2, true);
                    Polyline Paa2 = new Polyline()
                    {
                        Color = Color.FromColorIndex(ColorMethod.ByAci, 9),
                    };
                    Paa2.AddVertexAt(0, new Point2d(0 - 350, 0), 0, 0, 80);
                    Paa2.AddVertexAt(1, new Point2d(200 - 350, 0), 0, 0, 0);
                    Paa2.AddVertexAt(2, new Point2d(900 - 350, 0), 0, 0, 0);
                    btr2.AppendEntity(Paa2);
                    tr.AddNewlyCreatedDBObject(Paa2, true);
                    AttributeDefinition curzp = new AttributeDefinition();
                    curzp.Position = new Point3d(220 - 350, 0, 0);
                    curzp.Height = 250;
                    curzp.WidthFactor = 0.75;
                    curzp.Tag = "左坡";
                    curzp.TextStyleId = st["fsdb"];
                    btr2.AppendEntity(curzp);
                    tr.AddNewlyCreatedDBObject(curzp, true);
                }

                //-------------------------------------------------------------------------------------------
                if (!bt.Has("YP"))
                {
                    BlockTableRecord btr3 = new BlockTableRecord();
                    btr3.Name = "YP";
                    bt.UpgradeOpen();
                    bt.Add(btr3);
                    tr.AddNewlyCreatedDBObject(btr3, true);
                    Polyline Paa3 = new Polyline()
                    {
                        Color = Color.FromColorIndex(ColorMethod.ByAci, 9),
                    };
                    Paa3.AddVertexAt(0, new Point2d(0 + 350, 0), 0, 0, 80);
                    Paa3.AddVertexAt(1, new Point2d(-200 + 350, 0), 0, 0, 0);
                    Paa3.AddVertexAt(2, new Point2d(-900 + 350, 0), 0, 0, 0);
                    btr3.AppendEntity(Paa3);
                    tr.AddNewlyCreatedDBObject(Paa3, true);
                    AttributeDefinition curyp = new AttributeDefinition();
                    curyp.Position = new Point3d(-220 + 350, 0, 0);
                    curyp.HorizontalMode = TextHorizontalMode.TextRight;
                    curyp.AlignmentPoint = curyp.Position;
                    curyp.Height = 250;
                    curyp.WidthFactor = 0.75;
                    curyp.Tag = "右坡";
                    curyp.TextStyleId = st["fsdb"];
                    btr3.AppendEntity(curyp);
                    tr.AddNewlyCreatedDBObject(curyp, true);
                }
                //-------------------------------------------------------------------------------------------

                //-------------------------------------------------------------------------------------------
                tr.Commit();
            }
        }
        //-------------------------------------------------------------------------------------------
        // Something From Keanw : http://www.keanw.com/
        //-------------------------------------------------------------------------------------------
        [CommandMethod("sect", CommandFlags.UsePickSet)]
        public static void ObjectsByLayer()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Transaction tr = db.TransactionManager.StartTransaction();


            string output = Path.ChangeExtension(doc.Name, "dat");

            if (doc == null)
                return;
            var ed = doc.Editor;
            // Select the objects to sort
            // Setups the keyword options
            PromptSelectionOptions Opts = new PromptSelectionOptions();
            Opts.MessageForAdding = "\n请选择断面图";
            var psr = ed.GetSelection(Opts);
            if (psr.Status != PromptStatus.OK)
                return;
            // We'll sort them based on a string value (the layer name)
            var map = new Dictionary<ObjectId, string>();
            foreach (dynamic id in psr.Value.GetObjectIds())
            {
                if (id.ObjectClass.Name == "AcDbText")
                {
                    if (id.TextString.Contains("K"))
                    {
                        map.Add(id, id.TextString);
                        WriteMessage(output, id.TextString);
                    }
                    else if (id.TextString.Contains("Wy"))
                    {
                        WriteMessage(output, "#------------------------------------------------------#");
                        map.Add(id, id.TextString);
                        WriteMessage(output, id.TextString);
                    }
                    if (id.Layer == "右标高1")
                    {
                        map.Add(id, id.TextString);
                        WriteMessage(output, "H0=" + id.TextString);
                    }
                    else if (id.Layer == "左标高")
                    {
                        map.Add(id, id.TextString);
                        WriteMessage(output, "H1=" + id.TextString);
                    }
                    else if (id.Layer == "左标高1")
                    {
                        map.Add(id, id.TextString);
                        WriteMessage(output, "H2=" + id.TextString);
                    }
                }
                else if (id.ObjectClass.Name == "AcDb2dPolyline")
                {

                    Polyline2d tmp = (Polyline2d)tr.GetObject(id, OpenMode.ForRead);
                    // tmp.
                    if (tmp.Layer == "dmx")
                    {
                        WriteMessage(output, "dmx");
                        IEnumerator vertices = tmp.GetEnumerator();
                        while (vertices.MoveNext())
                        {
                            ObjectId vetxid = (ObjectId)vertices.Current;
                            Vertex2d vtx = (Vertex2d)vetxid.GetObject(OpenMode.ForRead);
                            string loc = string.Format("{0},{1},0", vtx.Position.X, vtx.Position.Y);
                            WriteMessage(output, loc);
                        }
                    }
                    else if (tmp.Layer == "sjx")
                    {
                        WriteMessage(output, "sjx");
                        IEnumerator vertices = tmp.GetEnumerator();
                        while (vertices.MoveNext())
                        {
                            ObjectId vetxid = (ObjectId)vertices.Current;
                            Vertex2d vtx = (Vertex2d)vetxid.GetObject(OpenMode.ForRead);
                            string loc = string.Format("{0},{1},0", vtx.Position.X, vtx.Position.Y);
                            WriteMessage(output, loc);
                        }
                    }

                }
                else if (id.ObjectClass.Name == "AcDbLine")
                {
                    Line zx = (Line)tr.GetObject(id, OpenMode.ForRead);
                    if (zx.Layer == "zhix")
                    {
                        WriteMessage(output, "X=" + zx.StartPoint.X.ToString());
                    }
                }
            }
            var sorted = map.OrderBy(kv => kv.Value);
            // Print them in order to the command-line
            foreach (var item in sorted)
            {
                ed.WriteMessage("\nObject {0} on layer {1}", item.Key, item.Value);
            }
            tr.Commit();
            tr.Dispose();
        }











        [CommandMethod("textext", CommandFlags.UsePickSet)]
        public static void TextExe()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            string output = Path.ChangeExtension(doc.Name, "dat");

            if (doc == null)
                return;
            var ed = doc.Editor;
            PromptSelectionOptions Opts = new PromptSelectionOptions();
            Opts.MessageForAdding = "\n请选择断面图";
            var psr = ed.GetSelection(Opts);
            if (psr.Status != PromptStatus.OK)
                return;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                foreach (dynamic id in psr.Value.GetObjectIds())
                {
                    if (id.ObjectClass.Name == "AcDbText")
                    {
                        DBText item = tr.GetObject(id, OpenMode.ForRead) as DBText;
                        WriteMessage(output,  item.TextString);
                        
                    }
                    else if (id.ObjectClass.Name == "AcDbMtext")
                    {
                        MText item = tr.GetObject(id, OpenMode.ForRead) as MText;
                        WriteMessage(output, item.Contents);
                    }

                }
            }


        }



        //-------------------------------------------------------------------------------------------


        [CommandMethod("bReplace", CommandFlags.UsePickSet)]
        public static void TextReplace()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Transaction tr = db.TransactionManager.StartTransaction();


            if (doc == null)
                return;
            var ed = doc.Editor;

            PromptSelectionOptions Opts = new PromptSelectionOptions();
            Opts.MessageForAdding = "\n请选择文字范围";
            var psr = ed.GetSelection(Opts);
            if (psr.Status != PromptStatus.OK)
                return;

            PromptResult oldText = ed.GetString("\n请输入需替换文字");
            PromptResult newText = ed.GetString("\n替换为");

            int counter = 0;

            foreach (dynamic id in psr.Value.GetObjectIds())
            {
                if (id.ObjectClass.Name == "AcDbText")
                {
                    if (id.TextString.Contains(oldText.StringResult))
                    {
                        DBText oldObj = (DBText)tr.GetObject(id, OpenMode.ForWrite);
                        oldObj.TextString = newText.StringResult;
                        counter++;
                    }
                }
            }
            {
                ed.WriteMessage("\n{0}处文字被成功替换", counter);
            }

            tr.Commit();
            tr.Dispose();
        }







        [CommandMethod("bSumL", CommandFlags.UsePickSet)]
        public static void LineSum()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                if (doc == null)
                    return;
                var ed = doc.Editor;

                PromptSelectionOptions Opts = new PromptSelectionOptions();
                Opts.MessageForAdding = "\n请选择直线";
                var psr = ed.GetSelection(Opts);
                if (psr.Status != PromptStatus.OK)
                    return;


                int counter = 0;
                double sumup = 0;

                foreach (dynamic id in psr.Value.GetObjectIds())
                {
                    if (id.ObjectClass.Name == "AcDbLine")
                    {
                        Line oldObj = (Line)tr.GetObject(id, OpenMode.ForRead);
                        sumup += oldObj.Length;
                        counter++;
                    }
                }
                {
                    ed.WriteMessage("\n共选择{0}个直线，合计长度{1:0.0}", counter, sumup);
                }
                tr.Commit();
            }
        }



        //-------------------------------------------------------------------------------------------
        /// <summary>
        /// 输出指定信息到文本文件
        /// </summary>
        /// <param name="path">文本文件路径</param>
        /// <param name="msg">输出信息</param>
        public static void WriteMessage(string path, string msg)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine("{0}", msg, DateTime.Now);
                    sw.Flush();
                }
            }
        }

    }
}