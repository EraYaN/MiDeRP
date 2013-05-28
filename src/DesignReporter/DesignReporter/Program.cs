using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DesignReporter
{
    class Program
    {
        static DirectoryInfo srcPath = new DirectoryInfo("src\\controller");
        static DirectoryInfo hwPath = new DirectoryInfo("hardware");
        static DirectoryInfo docsPath = new DirectoryInfo("docs");
        static FileInfo outputPath = new FileInfo("sourceBijlage.tex");
		static String start = @"\documentclass{report}
\input{docs/library/preamble}
\input{docs/library/style}
\addbibresource{docs/library/bibliography.bib}

\title{EPO-2: Source code}
\author{Erwin de Haan \and Others}


\begin{document}";
        static void Main(string[] args)
        {
            Console.WriteLine("DesignReporter started.");
			Console.WriteLine(Directory.GetCurrentDirectory());
            List<FileInfo> files = new List<FileInfo>();
			files.AddRange(srcPath.GetFiles("*",SearchOption.AllDirectories));
			files.AddRange(hwPath.GetFiles("*", SearchOption.AllDirectories));
			files.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.FullName, y.FullName));
			IEnumerable<FileInfo> fileQuery =
			from fileQ in files
			where !fileQ.FullName.Contains("obj") && !fileQ.FullName.Contains("Visual Micro") && !fileQ.FullName.Contains("robotsim") && !fileQ.FullName.Contains("Properties") && !fileQ.FullName.Contains("App")
			select fileQ;
			
			StreamWriter file = outputPath.CreateText();
			file.WriteLine(start);
			file.WriteLine("\r\n\r\n");
			foreach (FileInfo fi in fileQuery)
            {
                string ext = fi.Extension;
                string caption = fi.Directory.Name + @"/" + fi.Name;
				caption = caption.Replace("_","\\_");
				string filename = fi.Name;
				filename = filename.Replace("_", "\\_");
				string pathescaped = fi.FullName.Replace(outputPath.Directory.FullName+"\\", "").Replace('\\', '/');
                if (ext == ".cs")
                {
                    //caption = fi.Directory.Name+@"\"+fi.Name;
					file.WriteLine(String.Format(@"\subsection{{{2}}}\includecode[csharp]{{{1}}}{{{0}}}", pathescaped, caption, filename));
                }
                else if (ext == ".c" || ext == ".h")                
                {
                    //caption = Path.GetDirectoryName(f) + Path.GetFileName(f);
					file.WriteLine(String.Format(@"\subsection{{{2}}}\includecode[c]{{{1}}}{{{0}}}", pathescaped, caption, filename));
                }
                else if (ext == ".vhdl" || ext == ".vhd")
                {
                    //caption = Path.GetDirectoryName(f) + Path.GetFileName(f);
					file.WriteLine(String.Format(@"\subsection{{{2}}}\includecode[vhdl]{{{1}}}{{{0}}}", pathescaped, caption, filename));
                    
                }
                else if (ext == ".xaml")
                {
                    //caption = Path.GetDirectoryName(f) + Path.GetFileName(f);
					file.WriteLine(String.Format(@"\subsection{{{2}}}\includecode[xaml]{{{1}}}{{{0}}}", pathescaped, caption, filename));

                }
            }
            file.WriteLine(@"\end{document}");
            file.Close();
			Console.WriteLine("Done. Press any key to exit.");
			Console.ReadKey();
        }

    }
}
