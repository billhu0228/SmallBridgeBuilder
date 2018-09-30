using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACADExt
{
    class Node
    {
        public int ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public Node(int id, double x0, double y0, double z0)
        {
            ID = id;
            X = x0;
            Y = y0;
            Z = z0;
        }
        public Node()
        {
            ID = 0;
            X = 0.0;
            Y = 0.0;
            Z = 0.0;
        }

    }
    class Beam
    {
        public int ID { get; set; }
        public Node Ni { get; set; }
        public Node Nj { get; set; }
        public Node Nk { get; set; }
        public int SecN { get; set; }
        public int MatN { get; set; }
        public string Gr { get; set; }

        public Beam(int id, Node ni, Node nj, Node nk, int secnum, int matnum, string group)
        {
            ID = id;
            Ni = ni;
            Nj = nj;
            Nk = nk;
            SecN = secnum;
            MatN = matnum;
            Gr = group;
        }


        public double LocX
        {
            get
            {
                return (Ni.X + Nj.X) * 0.5;
            }
        }
        public double LocY
        {
            get
            {
                return (Ni.Y + Nj.Y) * 0.5;
            }
        }
        public string Xdata { get; set; }
    }
    class Section
    {
        public int ID { set; get; }
        public string Name { get; set; }
        public string SType {get;set;}
        public double W1 { set; get; }
        public double W2 { set; get; }
        public double W3 { set; get; }
        public double T1 { set; get; }
        public double T2 { set; get; }
        public double T3 { set; get; }
        public Section(int i,string name,string tp,double a,double b,double c,double d,double e,double f)
        {
            ID = i;
            Name = name;
            SType = tp;
            W1 = a;
            W2 = b;
            W3 = c;
            T1 = d;
            T2 = e;
            T3 = f;
        }
    }

    class AnsysCal
    {
        public Bridge curBridge;
        readonly List<Node> NodeList;
        List<Beam> BeamList;
        List<Section> SectList;
        int Num_Node;
        int Num_Elem;
        int Num_Sect;


        public AnsysCal(Bridge bri)
        {
            curBridge = bri;
            Num_Elem = 1;
            Num_Node = 1;
            Num_Sect = 1;
            NodeList = new List<Node>();
            BeamList = new List<Beam>();
            SectList = new List<Section>();
        }






        public void GenerateSection()
        {
            Section st;
            st = new Section(Num_Sect, "I8", "I", 50,50, 80, 6.5, 6.5, 4.5);
            Num_Sect++;
            SectList.Add(st);
            st = new Section(Num_Sect, "C10N", "CHAN", 48,48,100, 5.3,5.3,5.3);
            Num_Sect++;
            SectList.Add(st);
            st = new Section(Num_Sect, "C10P", "CHAN", 48, 48, 100, 5.3, 5.3, 5.3);
            Num_Sect++;
            SectList.Add(st);
            st = new Section(Num_Sect, "HBeam", "I", 122,122,270,15,15,8);
            Num_Sect++;
            SectList.Add(st);
            st = new Section(Num_Sect, "L90", "L", 90, 56, 0, 7, 7, 0);
            Num_Sect++;
            SectList.Add(st);
            st = new Section(Num_Sect, "L63", "L", 63, 40, 0, 5, 5, 0);
            Num_Sect++;
            SectList.Add(st);
            st = new Section(Num_Sect, "L36", "L", 36, 36, 0, 5, 5, 0);
            Num_Sect++;
            SectList.Add(st);
            st = new Section(Num_Sect, "L36CC", "L", 36, 36, 0, 5, 5, 0);
            Num_Sect++;
            SectList.Add(st);
            st = new Section(Num_Sect, "Rig", "CSOLID", 10, 0, 0, 0, 0, 0);
            Num_Sect++;
            SectList.Add(st);
            st = new Section(Num_Sect, "D30", "CSOLID", 15, 0, 0, 0, 0, 0);
            Num_Sect++;
            SectList.Add(st);
        }
        /// <summary>
        /// 配置模型
        /// </summary>
        public void GenerateModel()
        {
            Node item;
            Node Nu1 = new Node();
            Node Nu2 = new Node();
            Node Nu3 = new Node();
            Node Nu4 = new Node();
            Node Nu5 = new Node();
            Node Nu6 = new Node();
            Node Nu7 = new Node();
            Node Nm1 = new Node();
            Node Nm2 = new Node();
            Node Nm3 = new Node();
            Node Nb1 = new Node();
            Node Nb2 = new Node();
            Node Nb3 = new Node();
            Node Nb4 = new Node();
            Node Nb5 = new Node();
            Node Nb6 = new Node();
            Node Nb7 = new Node();
            Node nkup = new Node(9999,0, 1000000,0);
            NodeList.Add(nkup);
            Node nkdown = new Node(8888, 0,-1000000, 0);
            NodeList.Add(nkdown);
            Node nZero = new Node(0, 0, 0, 0);

            List<Node> beforeNode = new List<Node>() ;
            List<Node> curNode=new List<Node>();

            for (int ii = 0; ii < curBridge.Pieces; ii++)
            {
                for (int kk = 0; kk < 2; kk++)
                {
                    MakeDeck(ii, kk, ref nkup);
                }
                //double x0
            }
            MakeDeck(curBridge.Pieces, 0, ref nkup);





            double dz=0;
            for (int kk = 0; kk < curBridge.Layers; kk++)
            {
                for (int ii = 0; ii < curBridge.Pieces; ii++)
                {
                    for (int dir = -1; dir < 2; dir += 2)
                    {
                        for (int jj = 0; jj < curBridge.Columns; jj++)
                        {
                            switch (jj)
                            {
                                case 0:
                                    dz = 2100 * dir;
                                    break;
                                case 1:
                                    dz = (2100 + 450) * dir;
                                    break;
                                case 2:
                                    dz = (2100 + 450 + 250) * dir;
                                    break;
                            }
                            // 一片节点
                            curNode = MakeOnePieceNode(ii, kk, dz,
                                ref Nu1, ref Nu2, ref Nu3, ref Nu4, ref Nu5, ref Nu6, ref Nu7,
                                ref Nm1, ref Nm2, ref Nm3,
                                ref Nb1, ref Nb2, ref Nb3, ref Nb4, ref Nb5, ref Nb6, ref Nb7);
                            // 一片连杆
                            MakeOnePieceBeam(ref curNode, ref nkup, ref nkdown,(curBridge.Layers>1&&kk==0));
                            // 支撑架
                            if (jj == 1&&kk==(curBridge.Layers-1))
                            {
                                MakeZCG(ref beforeNode, ref curNode,ref nkup);
                            }
                            if (jj == 2 )
                            {
                                Beam elem;
                                elem = new Beam(Num_Elem, curNode[1], beforeNode[1], null, 5, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, curNode[3], beforeNode[3], null, 5, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, curNode[5], beforeNode[5], null, 5, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;

                            }
                            // 斜撑
                            if (jj == 1 && kk == 0)
                            {
                                Beam elem;
                                elem = new Beam(Num_Elem, curNode[11], beforeNode[1], nkup, 1, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, curNode[13], beforeNode[3], nkup, 1, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, curNode[15], beforeNode[5], nkup, 1, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                            }
                            else if (jj == 0 && curBridge.Columns == 1 && kk == 0)
                            {
                                var N1 = from nd in NodeList where nd.X == curNode[0].X && nd.Y == 0 && nd.Z == curNode[0].Z + dir * 450 select nd;
                                var N2 = from nd in NodeList where nd.X == curNode[3].X && nd.Y == 0 && nd.Z == curNode[3].Z + dir * 450 select nd;
                                var N3 = from nd in NodeList where nd.X == curNode[6].X && nd.Y == 0 && nd.Z == curNode[6].Z + dir * 450 select nd;
                                Beam elem;
                                elem = new Beam(Num_Elem, N1.First(), curNode[0], nkup, 1, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, N2.First(), curNode[3], nkup, 1, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, N3.First(), curNode[6], nkup, 1, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                            }
                            else if(jj==1&&kk==1)
                            {
                                Beam elem;
                                elem = new Beam(Num_Elem, curNode[1], beforeNode[7], null, 8, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, curNode[7], beforeNode[1], null, 8, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, curNode[3], beforeNode[8], null, 8, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, curNode[8], beforeNode[3], null, 8, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, curNode[5], beforeNode[9], null, 8, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, curNode[9], beforeNode[5], null, 8, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, curNode[11], beforeNode[7], null, 8, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, curNode[7], beforeNode[11], null, 8, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, curNode[13], beforeNode[8], null, 8, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, curNode[8], beforeNode[13], null, 8, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, curNode[15], beforeNode[9], null, 8, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, curNode[9], beforeNode[15], null, 8, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;

                                elem = new Beam(Num_Elem, curNode[1], beforeNode[1], null, 5, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, curNode[11], beforeNode[11], null, 5, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;

                                elem = new Beam(Num_Elem, curNode[3], beforeNode[3], null, 5, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, curNode[13], beforeNode[13], null, 5, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;

                                elem = new Beam(Num_Elem, curNode[5], beforeNode[5], null, 5, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;
                                elem = new Beam(Num_Elem, curNode[15], beforeNode[15], null, 5, 1, "");
                                BeamList.Add(elem);
                                Num_Elem++;

                            }
                            beforeNode = curNode;
                        }
                    }
                }
            }



        }






        /// <summary>
        /// 形成桥面系
        /// </summary>
        /// <param name="ii"></param>
        /// <param name="kk"></param>
        /// <param name="nkup"></param>
        private void MakeDeck(int ii,int kk,ref Node nkup)
        {
            Node N1 = new Node(Num_Node, ii * 3000 + kk * 1500, 0, -2925);
            NodeList.Add(N1);
            Num_Node++;
            Node N2 = new Node(Num_Node, ii * 3000 + kk * 1500, 0, -2800);
            NodeList.Add(N2);
            Num_Node++;
            Node N3 = new Node(Num_Node, ii * 3000 + kk * 1500, 0, -2550);
            NodeList.Add(N3);
            Num_Node++;
            Node N4 = new Node(Num_Node, ii * 3000 + kk * 1500, 0, -2100);
            NodeList.Add(N4);
            Num_Node++;
            Node N5 = new Node(Num_Node, ii * 3000 + kk * 1500, 0, -1050);
            NodeList.Add(N5);
            Num_Node++;
            Node N6 = new Node(Num_Node, ii * 3000 + kk * 1500, 0, 0);
            NodeList.Add(N6);
            Num_Node++;
            Node N7 = new Node(Num_Node, ii * 3000 + kk * 1500, 0, 1050);
            NodeList.Add(N7);
            Num_Node++;
            Node N8 = new Node(Num_Node, ii * 3000 + kk * 1500, 0, 2100);
            NodeList.Add(N8);
            Num_Node++;
            Node N9 = new Node(Num_Node, ii * 3000 + kk * 1500, 0, 2550);
            NodeList.Add(N9);
            Num_Node++;
            Node N10 = new Node(Num_Node, ii * 3000 + kk * 1500, 0, 2800);
            NodeList.Add(N10);
            Num_Node++;
            Node N11 = new Node(Num_Node, ii * 3000 + kk * 1500, 0, 2925);
            NodeList.Add(N11);
            Num_Node++;

            Beam elem;
            elem = new Beam(Num_Elem, N1, N2, nkup, 4, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, N2, N3, nkup, 4, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, N3, N4, nkup, 4, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, N4, N5, nkup, 4, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, N5, N6, nkup, 4, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, N6, N7, nkup, 4, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, N7, N8, nkup, 4, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, N8, N9, nkup, 4, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, N9, N10, nkup, 4, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, N10, N11, nkup, 4, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
        }



        private void MakeZCG(ref List<Node> beforeNode, ref List<Node> curNode, ref Node nkup)
        {
            Beam elem;
            elem = new Beam(Num_Elem, curNode[2], beforeNode[2], nkup, 5, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, beforeNode[4], curNode[4], nkup, 5, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, beforeNode[4], beforeNode[3], nkup, 6, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, beforeNode[3], beforeNode[2], nkup, 6, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, curNode[2], curNode[3], nkup, 6, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, curNode[3], curNode[4], nkup, 6, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, beforeNode[3], curNode[2], nkup, 7, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, beforeNode[3], curNode[4], nkup, 7, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, curNode[3], beforeNode[2], nkup, 7, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, curNode[3], beforeNode[4], nkup, 7, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
        }








        private void MakeOnePieceBeam(ref List<Node> opnl,ref Node nkup,ref Node nkdown,bool isFist=false)
        {
            Node Nu1 = opnl[0];
            Node Nu2 = opnl[1];
            Node Nu3 = opnl[2];
            Node Nu4 = opnl[3];
            Node Nu5 = opnl[4];
            Node Nu6 = opnl[5];
            Node Nu7 = opnl[6];
            Node Nm1 = opnl[7];
            Node Nm2 = opnl[8];
            Node Nm3 = opnl[9];
            Node Nb1 = opnl[10];
            Node Nb2 = opnl[11];
            Node Nb3 = opnl[12];
            Node Nb4 = opnl[13];
            Node Nb5 = opnl[14];
            Node Nb6 = opnl[15];
            Node Nb7 = opnl[16];            

            Beam elem;
            elem = new Beam(Num_Elem, Nb1, Nb2, nkup, 2, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nb2, Nb3, nkup, 2, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nb3, Nb4, nkup, 2, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nb4, Nb5, nkup, 2, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nb5, Nb6, nkup, 2, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nb6, Nb7, nkup, 2, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nu1, Nu2, nkup, 2, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nu2, Nu3, nkup, 2, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nu3, Nu4, nkup, 2, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nu4, Nu5, nkup, 2, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nu5, Nu6, nkup, 2, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nu6, Nu7, nkup, 2, 1, "");
            BeamList.Add(elem);
            Num_Elem++;

            elem = new Beam(Num_Elem, Nb1, Nb2, nkdown, 3, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nb2, Nb3, nkdown, 3, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nb3, Nb4, nkdown, 3, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nb4, Nb5, nkdown, 3, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nb5, Nb6, nkdown, 3, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nb6, Nb7, nkdown, 3, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nu1, Nu2, nkdown, 3, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nu2, Nu3, nkdown, 3, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nu3, Nu4, nkdown, 3, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nu4, Nu5, nkdown, 3, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nu5, Nu6, nkdown, 3, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nu6, Nu7, nkdown, 3, 1, "");
            BeamList.Add(elem);
            Num_Elem++;



            // 腹杆
            elem = new Beam(Num_Elem, Nb2, Nm1, null, 1, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nm1, Nu2, null, 1, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nb4, Nm2, null, 1, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nm2, Nu4, null, 1, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nb6, Nm3, null, 1, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nm3, Nu6, null, 1, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nm1, Nu3, null, 1, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nu3, Nm2, null, 1, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nm2, Nb3, null, 1, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nb3, Nm1, null, 1, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nm2, Nu5, null, 1, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nu5, Nm3, null, 1, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nm3, Nb5, null, 1, 1, "");
            BeamList.Add(elem);
            Num_Elem++;
            elem = new Beam(Num_Elem, Nb5, Nm2, null, 1, 1, "");
            BeamList.Add(elem);
            Num_Elem++;

            if (isFist)
            {
                Node N1 = new Node(Num_Node, Nu2.X, Nu2.Y + 100, Nu2.Z);
                NodeList.Add(N1);
                Num_Node++;
                Node N3 = new Node(Num_Node, Nu4.X, Nu4.Y + 100, Nu4.Z);
                NodeList.Add(N3);
                Num_Node++;
                Node N5 = new Node(Num_Node, Nu6.X, Nu6.Y + 100, Nu6.Z);
                NodeList.Add(N5);
                Num_Node++;

                elem = new Beam(Num_Elem, Nu2, N1, null, 9, 2, "");
                BeamList.Add(elem);
                Num_Elem++;
                elem = new Beam(Num_Elem, Nu4, N3, null, 9, 2, "");
                BeamList.Add(elem);
                Num_Elem++;
                elem = new Beam(Num_Elem, Nu6, N5, null, 9, 2, "");
                BeamList.Add(elem);
                Num_Elem++;
            }
        }

        private List<Node> MakeOnePieceNode(int ii,int kk,double dz,
            ref Node Nu1, ref Node Nu2, ref Node Nu3, ref Node Nu4, ref Node Nu5, ref Node Nu6, ref Node Nu7,
            ref Node Nm1, ref Node Nm2, ref Node Nm3,
            ref Node Nb1, ref Node Nb2, ref Node Nb3, ref Node Nb4, ref Node Nb5, ref Node Nb6, ref Node Nb7)
        {
            List<Node> res=new List<Node>();

            Nb1 = new Node(Num_Node, ii * 3000, kk * 1500, dz);
            NodeList.Add(Nb1);            
            Num_Node++;
            Nb2 = new Node(Num_Node, ii * 3000 + 90, kk * 1500, dz);
            NodeList.Add(Nb2);
            Num_Node++;
            Nb3 = new Node(Num_Node, ii * 3000 + 90 + 705, kk * 1500, dz);
            NodeList.Add(Nb3);
            Num_Node++;
            Nb4 = new Node(Num_Node, ii * 3000 + 1500, kk * 1500, dz);
            NodeList.Add(Nb4);
            Num_Node++;
            Nb5 = new Node(Num_Node, ii * 3000 + 1500 + 705, kk * 1500, dz);
            NodeList.Add(Nb5);
            Num_Node++;
            Nb6 = new Node(Num_Node, ii * 3000 + 1500 + 1410, kk * 1500, dz);
            NodeList.Add(Nb6);
            Num_Node++;
            Nb7 = new Node(Num_Node, ii * 3000 + 3000, kk * 1500, dz);
            NodeList.Add(Nb7);
            Num_Node++;
            Nu1 = new Node(Num_Node, ii * 3000, kk * 1500 + 1400, dz);
            NodeList.Add(Nu1);
            Num_Node++;
            Nu2 = new Node(Num_Node, ii * 3000 + 90, kk * 1500 + 1400, dz);
            NodeList.Add(Nu2);
            Num_Node++;
            Nu3 = new Node(Num_Node, ii * 3000 + 90 + 705, kk * 1500 + 1400, dz);
            NodeList.Add(Nu3);
            Num_Node++;
            Nu4 = new Node(Num_Node, ii * 3000 + 1500, kk * 1500 + 1400, dz);
            NodeList.Add(Nu4);
            Num_Node++;
            Nu5 = new Node(Num_Node, ii * 3000 + 1500 + 705, kk * 1500 + 1400, dz);
            NodeList.Add(Nu5);
            Num_Node++;
            Nu6 = new Node(Num_Node, ii * 3000 + 1500 + 1410, kk * 1500 + 1400, dz);
            NodeList.Add(Nu6);
            Num_Node++;
            Nu7 = new Node(Num_Node, ii * 3000 + 3000, kk * 1500 + 1400, dz);
            NodeList.Add(Nu7);
            Num_Node++;
            Nm1 = new Node(Num_Node, ii * 3000 + 90, kk * 1500 + 700, dz);
            NodeList.Add(Nm1);
            Num_Node++;
            Nm2 = new Node(Num_Node, ii * 3000 + 1500, kk * 1500 + 700, dz);
            NodeList.Add(Nm2);
            Num_Node++;
            Nm3 = new Node(Num_Node, ii * 3000 + 2910, kk * 1500 + 700, dz);
            NodeList.Add(Nm3);
            Num_Node++;

            res.Add(Nu1);
            res.Add(Nu2);
            res.Add(Nu3);
            res.Add(Nu4);
            res.Add(Nu5);
            res.Add(Nu6);
            res.Add(Nu7);
            res.Add(Nm1);
            res.Add(Nm2);
            res.Add(Nm3);
            res.Add(Nb1);
            res.Add(Nb2);
            res.Add(Nb3);
            res.Add(Nb4);
            res.Add(Nb5);
            res.Add(Nb6);
            res.Add(Nb7);
            return res;
        }



        public string AnsysCalculator(string rootDir)
        {
            string wdir = Path.Combine(rootDir, curBridge.Name);
            if (Directory.Exists(wdir))
            {
                DelectDir(wdir);
            }
            Directory.CreateDirectory(wdir);
            return wdir;
        }



        public void WriteMain(string filepath)
        {
            using (FileStream fs = new FileStream(Path.Combine(filepath, "Main.inp"),
                FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("!=======================================================");
                    sw.WriteLine("! Modeled by Bill/2019-9-25");
                    sw.WriteLine("!=======================================================");
                    sw.WriteLine("finish");
                    sw.WriteLine("/CLEAR,START");
                    sw.WriteLine("/CWD,'{0}'",filepath);
                    sw.WriteLine("/prep7");
                    sw.WriteLine("MP,EX,  1,2.0E5");
                    sw.WriteLine("MP,DENS,1,7.85e-9");
                    sw.WriteLine("MP,ALPX,1,1.2E-5");
                    sw.WriteLine("MP,NUXY,1,0.3");
                    sw.WriteLine("MP,EX,  2,2.0E15");
                    sw.WriteLine("MP,DENS,2,7.85e-19");
                    sw.WriteLine("MP,ALPX,2,1.2E-5");
                    sw.WriteLine("MP,NUXY,2,0.3");
                    sw.WriteLine("et,1,188");
                    sw.WriteLine("!=======================================================");
                    sw.WriteLine("/input,Sect,inp");
                    sw.WriteLine("/input,Node,inp");
                    sw.WriteLine("/input,Elem,inp");
                    sw.WriteLine("allsel");
                    sw.WriteLine("eplot");
                    sw.WriteLine("nummrg,node");



                }
            }
        }




        public void WriteSect(string filepath)
        {
            using (FileStream fs = new FileStream(Path.Combine(filepath, "Sect.inp"),
                FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine("/prep7");
                    foreach (var item in SectList)
                    {
                        sw.WriteLine("SECTYPE,{0},BEAM,{1},{2},0", item.ID,item.SType,item.Name);
                        if (item.SType=="I"|| item.SType == "CHAN")
                        {
                            if (item.Name == "HBeam")
                            {
                                sw.WriteLine("SECOFFSET, USER, 0, -50");
                            }
                            else if (item.Name == "C10N")
                            {
                                sw.WriteLine("SECOFFSET, USER, -40, 50");
                            }
                            else if (item.Name == "C10P")
                            {
                                sw.WriteLine("SECOFFSET, USER,-40, 50");
                            }
                            sw.WriteLine("SECDATA,{0},{1},{2},{3},{4},{5},0,0,0,0,0,0",
                                item.W1,item.W2,item.W3,item.T1,item.T2,item.T3);
                        }
                        else if (item.SType == "L"&& item.Name.Last()!='C')
                        {
                            sw.WriteLine("SECOFFSET, USER, {0},{1}",item.W1*0.5,-50);
                            sw.WriteLine("SECDATA,{0},{1},{2},{3},0,0,0,0,0,0,0,0", item.W1,item.W2,item.T1,item.T2);
                        }
                        else if (item.SType == "L" && item.Name.Last() == 'C')
                        {                            
                            sw.WriteLine("SECDATA,{0},{1},{2},{3},0,0,0,0,0,0,0,0", item.W1, item.W2, item.T1, item.T2);
                        }
                        else if (item.SType == "CSOLID")
                        {
                            sw.WriteLine("SECDATA,{0},0,0,0,0,0,0,0,0,0,0,0",item.W1);
                        }



                    }
                }
            }
        }


        public void WriteNode(string filepath)
        {
            using (FileStream fs = new FileStream(Path.Combine(filepath, "Node.inp"), 
                FileMode.Create,FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine("/prep7");
                    foreach (var item in NodeList)
                    {
                        sw.WriteLine("n,{0},{1},{2},{3}",item.ID,item.X,item.Y,item.Z);
                    }
                }
            }
        }

        public void WriteElem(string filepath)
        {
            using (FileStream fs = new FileStream(Path.Combine(filepath, "Elem.inp"),
                FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine("/prep7");
                    foreach (var item in BeamList)
                    {
                        if (item.Nk == null)
                        {
                            sw.WriteLine("secn,{0}", item.SecN);
                            sw.WriteLine("e,{0},{1}", item.Ni.ID, item.Nj.ID);
                        }
                        else
                        {
                            sw.WriteLine("secn,{0}", item.SecN);
                            sw.WriteLine("e,{0},{1},{2}", item.Ni.ID, item.Nj.ID, item.Nk.ID);
                        }
                    }
                }
            }
        }





        public static List<double> Arange(double st, double ed, double step)
        {
            List<double> res = new List<double>();

            res.Add(st);
            while (res.Last() < ed)
            {
                res.Add(res.Last() + step);
            }
            return res;
        }

        public static void DelectDir(string srcPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                    {
                        File.Delete(i.FullName);      //删除指定文件
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

    }
}
