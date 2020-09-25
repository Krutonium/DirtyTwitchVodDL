﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
namespace VodDL
{
    class Program
    {
        private static int CurrentDownloads;
        static void Main(string[] args)
        {
            List<string> Files = new List<string>();
            if (!Directory.Exists("./vod/"))
            {
                Directory.CreateDirectory("./vod/");
            }
            try
            {
                int x = 0;
                while (x < 50)
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
        }
    }
}