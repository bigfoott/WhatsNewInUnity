using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using System.Xml;
using System.Net;

namespace WhatsNewInUnity
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists("auth.txt"))
            {
                Console.WriteLine("Auth file not found.");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("[" + DateTime.Now + "] Starting...");
                string[] lines = File.ReadAllLines("auth.txt");
                Auth.SetUserCredentials(lines[0], lines[1], lines[2], lines[3]);

                HtmlWeb web;
                string url = "https://unity3d.com/unity/whats-new";

                Console.WriteLine("[" + DateTime.Now + "] Beginning loop.");
                while (true)
                {
                    Thread.Sleep(600000); // 10 minutes

                    HtmlDocument doc = null;
                    try
                    {
                        web = new HtmlWeb();
                        doc = web.Load(url);
                    }
                    catch { }

                    string[] rss = new string[0];
                    try
                    {
                        using (var wc = new WebClient())
                        {
                            rss = wc.DownloadString("https://unity3d.com/unity/beta/latest.xml").Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        }
                    } 
                    catch { }

                    if (doc != null)
                    {
                        string ver = doc.DocumentNode.SelectSingleNode("//h1").InnerText;
                        if (!File.Exists("version.txt")) File.WriteAllText("version.txt", ver);
                        if (File.ReadAllText("version.txt") != ver)
                        {
                            Tweet.PublishTweet(ver + " is out!\n\nView the changelog here: " + url + "/" + ver.Split(' ')[1]);
                            Console.WriteLine("[" + DateTime.Now + "] Tweeted update " + ver);
                            File.WriteAllText("version.txt", ver);
                        }
                    }

                    if (rss.Length != 0)
                    {
                        string ver = rss.First(s => s.Contains("<title>"));
                        ver = ver.Split(new string[] { "<title>" }, StringSplitOptions.None)[2].Split(new string[] { "</title>" }, StringSplitOptions.None)[0].Split(' ')[1];

                        if (!File.Exists("version-alphabeta.txt")) File.WriteAllText("version-alphabeta.txt", ver);
                        if (File.ReadAllText("version-alphabeta.txt") != ver)
                        {
                            Tweet.PublishTweet(ver + " is out!\n\nView the changelog here: " + url + "/" + ver.Split(' ')[1]);
                            Console.WriteLine("[" + DateTime.Now + "] Tweeted update " + ver);
                            File.WriteAllText("version-alphabeta.txt", ver);
                        }
                    }
                }
            }
        }
    }
}
