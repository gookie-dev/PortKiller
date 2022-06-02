using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PortKiller
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "PortKiller by gookie";
            Console.Write("Please enter the port: ");
            String port = Console.ReadLine();
            Console.WriteLine();
            List<int> pids = new List<int>();
            using (Process process = new Process())
            {
                ProcessStartInfo StartInfo = new ProcessStartInfo();
                StartInfo.FileName = "netstat.exe";
                StartInfo.Arguments = "-a -n -o";
                StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                StartInfo.UseShellExecute = false;
                StartInfo.RedirectStandardOutput = true;
                process.StartInfo = StartInfo;
                process.Start();

                string[] NetStatRows = Regex.Split(process.StandardOutput.ReadToEnd(), "\r\n").Skip(4).ToArray();

                foreach (string NetStatRow in NetStatRows.Take(NetStatRows.Count() - 1).ToArray())
                {
                    string[] NetStatColumns = Regex.Split(NetStatRow, "\\s+").Skip(1).ToArray();

                    if (NetStatRow.Contains(":" + port + " "))
                    {
                        int pid;
                        if(NetStatColumns.Length > 4)
                        {
                            pid = Int16.Parse(NetStatColumns[4]);
                        }
                        else
                        {
                            pid = Int16.Parse(NetStatColumns[3]);
                        }

                        Console.WriteLine(Process.GetProcessById(pid).ProcessName + ":" + pid.ToString());
                        pids.Add(pid);
                    }                  
                }
            }

            if (pids.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("Do you want to kill the above mentioned processes? (y/n) ");
                if (Console.ReadLine().Equals("y"))
                {
                    foreach(int pid in pids)
                    {
                        Process.GetProcessById(pid).Kill();
                    }
                    Console.WriteLine("All processes have been killed, you can now shoot the window!");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("No process was found that uses port " + port + "!");
                Console.ReadLine();
            }
        }
    }
}
