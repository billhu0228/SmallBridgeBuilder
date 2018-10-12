using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACADExt
{
    class LatexGen
    {
        public Bridge curBridge;        

        public LatexGen(Bridge bri)
        {
            curBridge = bri;
        }

        public string GetReady(string rootDir)
        {
            string wdir = Path.Combine(rootDir, curBridge.Name);

            if (Directory.Exists(wdir))
            {
                AnsysCal.DelectDir(wdir);
            }
            Directory.CreateDirectory(wdir);

            CopyDirectory(Path.Combine(rootDir,"template"), rootDir,curBridge.Name);

            return Path.Combine(rootDir, curBridge.Name);
        }



        public void WriteTex(string filepath)
        {
            using (FileStream fs = new FileStream(Path.Combine(filepath, "CalSheet.tex"),
                FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("\\documentclass{HPDI}");
                    sw.WriteLine("\\usepackage{multirow}");
                    sw.WriteLine("\\pagestyle{myfancy}");
                    sw.WriteLine("%\\geometry{showframe}");
                    sw.WriteLine("\\begin{document}");
                    sw.WriteLine("\\ctitle{Kogolnige Bridge}");
                    sw.WriteLine(@"\setcounter{page}{1}");
                    sw.WriteLine("\\section{基本信息}");
                    sw.WriteLine(@"{\color{red} Kogolnige Bridge}原桥址起点桩号{\color{red}10+581.91}，");
                    sw.WriteLine(@"终点桩号{\color{red}10+594.35}，为{\color{red}SS}型标准“321贝雷梁”装配式公路钢桥。");
                    sw.WriteLine("新桥拟采用{\\color{red}" +
                        curBridge.TypeString() +
                        "}型标准贝雷梁，采用{\\color{red}" +
                        curBridge.Pieces+
                        "}节贝雷梁，桥长{\\color{red}15.0}m。");
                    sw.WriteLine(@"桥梁计算模型如下图\ref{fig:1}所示（图中未示出桥面板）。");
                    sw.WriteLine("\\begin{figure}[!ht]");
                    sw.WriteLine("\\centering");
                    sw.WriteLine(@"\includegraphics[width=0.8\textwidth]{pic/1.png}");
                    sw.WriteLine("\\caption{桥梁模型示意图\\label{fig:1}}");
                    sw.WriteLine("\\end{figure}");
                    sw.WriteLine("\\section{设计荷载组合}");
                    sw.WriteLine("根据Australian Standard(AS 5100.2)，荷载组合考虑以下三组，其中ULS-1和ULS-2为极限状态荷载组合，SLS-1为正常使用状态荷载组合:");
                    sw.WriteLine("\\begin{itemize}");
                    sw.WriteLine("\\item[ULS-1:]1.1恒载+1.8活载+1.0温度效应");
                    sw.WriteLine("\\item[ULS-2:]1.1恒载+1.25温度效应+1.0活载");
                    sw.WriteLine("\\item[SLS-1:]1.0恒载+0.7活载+0.5温度效应");
                    sw.WriteLine("\\end{itemize}");
                    sw.WriteLine(@"\clearpage");
                    sw.WriteLine(@"\section{分析结果}");
                    sw.WriteLine(@"\subsection{极限承载能力验算结果}");
                    sw.WriteLine("考虑ULS-1和ULS-2两种荷载组合，结构最不利应力云图如下图\\ref{fig:2}所示，其最大应力值139.92MPa，最小值-296.81MPa，均满足规范限值。");
                    sw.WriteLine(@"\begin{figure}[!ht]");
                    sw.WriteLine(@"\centering");
                    sw.WriteLine(@"\includegraphics[width=0.6\textwidth]{pic/2.png}");
                    sw.WriteLine(@"\caption{杆件等效应力云图\label{fig:2}}");
                    sw.WriteLine(@"\end{figure}");
                    sw.WriteLine(@"\subsection{活载变形验算结果}");
                    sw.WriteLine(@"仅在活载作用下，桥梁竖向变形如下图所示，竖向变形约为26mm，小于澳洲标准1/600L=xxx的限值，满足规范要求。");
                    sw.WriteLine(@"\begin{figure}[!ht]");
                    sw.WriteLine(@"\centering");
                    sw.WriteLine(@"\includegraphics[width=0.6\textwidth]{pic/3.png}");
                    sw.WriteLine(@"\caption{桥梁竖向变形\label{fig:3}}");
                    sw.WriteLine(@"\end{figure}");
                    sw.WriteLine(@"\end{document}");
                }
            }
        }








        public void RunXeLatex(string outdir,string fname)
        {
            string cmdA = string.Format("xelatex -interaction=batchmode -output-directory=\"{0}\" \"{1}\"",outdir,fname);
            Execute(cmdA, 0, outdir);
            Execute(cmdA, 0, outdir);
        }

        /// <summary>  
        /// 执行DOS命令，返回DOS命令的输出  
        /// </summary>  
        /// <param name="dosCommand">dos命令</param>  
        /// <param name="milliseconds">等待命令执行的时间（单位：毫秒），  
        /// 如果设定为0，则无限等待</param>  
        /// <returns>返回DOS命令的输出</returns>  
        public static string Execute(string command, int seconds,string wdir)
        {
            string output = ""; //输出字符串  
            if (command != null && !command.Equals(""))
            {
                Process process = new Process();//创建进程对象  
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";//设定需要执行的命令  
                startInfo.Arguments ="/C"+ command;//“/C”表示执行完命令后马上退出  
                startInfo.UseShellExecute = false;//不使用系统外壳程序启动  
                //startInfo.RedirectStandardInput = true;//不重定向输入  
                startInfo.RedirectStandardOutput = true; //重定向输出  
                //startInfo.CreateNoWindow = true;//不创建窗口  
                startInfo.WorkingDirectory = wdir;
                process.StartInfo = startInfo;
                try
                {
                    if (process.Start())//开始进程  
                    {   
                        if (seconds == 0)
                        {
                            process.WaitForExit();//这里无限等待进程结束  
                        }
                        else
                        {
                            process.WaitForExit(seconds); //等待进程结束，等待时间为指定的毫秒  
                        }
                        output = process.StandardOutput.ReadToEnd();//读取进程的输出  
                    }
                }
                catch
                {
                }
                finally
                {
                    if (process != null)
                        process.Close();
                }
            }
            return output;
        }










        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="srcdir"></param>
        /// <param name="desdir"></param>
        private void CopyDirectory(string srcdir, string desdir,string newname=null)
        {
            string folderName = srcdir.Substring(srcdir.LastIndexOf("\\") + 1);

            string desfolderdir = "";
            if (newname ==null)
            {
                desfolderdir = desdir + "\\" + folderName;
            }
            else
            {
                desfolderdir = desdir + "\\" + newname;
            }
            

            if (desdir.LastIndexOf("\\") == (desdir.Length - 1))
            {
                desfolderdir = desdir + folderName;
            }
            string[] filenames = Directory.GetFileSystemEntries(srcdir);

            foreach (string file in filenames)// 遍历所有的文件和目录
            {
                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {

                    string currentdir = desfolderdir + "\\" + file.Substring(file.LastIndexOf("\\") + 1);
                    if (!Directory.Exists(currentdir))
                    {
                        Directory.CreateDirectory(currentdir);
                    }

                    CopyDirectory(file, desfolderdir);
                }

                else // 否则直接copy文件
                {
                    string srcfileName = file.Substring(file.LastIndexOf("\\") + 1);

                    srcfileName = desfolderdir + "\\" + srcfileName;


                    if (!Directory.Exists(desfolderdir))
                    {
                        Directory.CreateDirectory(desfolderdir);
                    }


                    File.Copy(file, srcfileName);
                }
            }//foreach 
        }//function end






    }
}
