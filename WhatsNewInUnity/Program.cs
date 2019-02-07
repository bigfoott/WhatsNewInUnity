using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;

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
                string[] lines = File.ReadAllLines("auth.txt");
                Auth.SetUserCredentials(lines[0], lines[1], lines[2], lines[3]);

                while (true)
                {
                    var web = new HtmlWeb();
                    string url = "https://unity3d.com/unity/whats-new";
                    var doc = web.Load(url);
                    string ver = doc.DocumentNode.SelectSingleNode("//h1").InnerText;
                    if (!File.Exists("version.txt")) File.WriteAllText("version.txt", "");
                    if (File.ReadAllText("version.txt") != ver)
                    {
                        Tweet.PublishTweet(ver + " is out!\n\nView the changelog here: " + url + "/" + ver.Split(' ')[1]);
                        Console.WriteLine("[" + DateTime.Now + "] Tweeted update " + ver);
                        File.WriteAllText("version.txt", ver);
                    }

                    Thread.Sleep(600000); // 10 minutes
                }
            }
        }
    }
}
