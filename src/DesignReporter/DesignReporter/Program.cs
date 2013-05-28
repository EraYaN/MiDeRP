using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DesignReporter
{
    class Program
    {
        static DirectoryInfo srcPath = new DirectoryInfo("src/");
        static DirectoryInfo hwPath = new DirectoryInfo("hardware/");
        static DirectoryInfo docsPath = new DirectoryInfo("docs/");
        static FileInfo outputPath = new FileInfo("sourceBijlage.tex");
        static StringBuilder sb = new StringBuilder(
@"\documentclass{report}
\input{library/preamble}
\input{library/style}
\addbibresource{library/bibliography.bib}

\title{EPO-2: Test Code}
\author{Erwin de Haan \and Others}


\begin{document});"); 
        static void Main(string[] args)
        {
            Console.WriteLine("DesignReporter started.");

            FileInfo[] files = srcPath.GetFiles();
            foreach (FileInfo fi in files)
            {
                string ext = fi.Extension;
                string caption = fi.Directory.Name + @"/" + fi.Name;
                string pathescaped = fi.FullName.Replace('\\','/');
                if (ext == ".cs")
                {
                    //caption = fi.Directory.Name+@"\"+fi.Name;
                    sb.AppendLine(String.Format(@"\includecode[csharp]{{1}}{{0}}\newpage", pathescaped, caption));
                }
                else if (ext == ".c" || ext == ".h")                
                {
                    //caption = Path.GetDirectoryName(f) + Path.GetFileName(f);
                    sb.AppendLine(String.Format(@"\includecode[c]{{1}}{{0}}\newpage", pathescaped, caption));
                }
                else if (ext == ".vhdl" || ext == ".vhd")
                {
                    //caption = Path.GetDirectoryName(f) + Path.GetFileName(f);
                    sb.AppendLine(String.Format(@"\includecode[vhdl]{{1}}{{0}}\newpage", pathescaped, caption));
                    
                }
                else if (ext == ".xaml")
                {
                    //caption = Path.GetDirectoryName(f) + Path.GetFileName(f);
                    sb.AppendLine(String.Format(@"\includecode[xaml]{{1}}{{0}}\newpage", pathescaped, caption));

                }
            }
            sb.Append(@"\r\n\end{document}");
            outputPath.CreateText().WriteLine(sb.ToString());            
        }
    }
}
