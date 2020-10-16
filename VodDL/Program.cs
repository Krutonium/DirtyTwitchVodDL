using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
namespace VodDL
{
    class Program
    {
        private static int CurrentDownloads;
        static List<string> Files = new List<string>();
        static bool DownloadDone = false;
        static void Main(string[] args)
        {
            if (!Directory.Exists("./vod/"))
            {
                Directory.CreateDirectory("./vod/");
            }
            try
            {
                int x = 0;
                while (!DownloadDone)
                {
                    var Client = new WebClient();
                    Client.DownloadFileCompleted += ClientOnDownloadFileCompleted;
                    string link = args[0] + x + ".ts";
                    Client.DownloadFileTaskAsync(new Uri(link), "./vod/" + x + ".ts");
                    CurrentDownloads += 1;
                    Files.Add("./vod/" + x + ".ts");
                    Console.WriteLine("Downloading " + x);
                    x++;
                    while (CurrentDownloads > 10)
                    {
                        System.Threading.Thread.Sleep(500);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Done"); 
            }
            Console.WriteLine("Combining Files...");
            using (Stream destStream = File.OpenWrite(args[1]))
            {
                foreach (string srcFileName in Files)
                {
                    using (Stream srcStream = File.OpenRead(srcFileName))
                    {
                        srcStream.CopyTo(destStream);
                    }
                }
            }
            Console.WriteLine("Done!");
            Directory.Delete("./vod/", true);
        }
        
        private static void ClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            CurrentDownloads -= 1;
            long length = new FileInfo(Files[Files.Count]).Length;
            if (length < 500) //less than 500 bytes
            {
                DownloadDone = true;
            }
        }
    }
}
