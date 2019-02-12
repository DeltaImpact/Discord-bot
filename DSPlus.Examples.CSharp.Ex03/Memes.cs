using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using DSharpPlus.Entities;

namespace DSPlus.Examples
{
    public static class Memes
    {
        public static string MemeWfWikiPath = "files/wfWiki";
        public static Dictionary<string, List<Meme>> MemeWfWikiDictionary = new Dictionary<string, List<Meme>>();

        public static int CountOfCategories = 0;
        public static int CountOfMemes = 0;

        static string url = "http://37.230.115.216/images/meme/wfWiki/";


        static Memes()
        {
            LoadMemesInfo();

            //Console.WriteLine("Glory for the king!");
            //Console.WriteLine(MemesString());
        }

        public static void LoadMemesInfo()
        {
            LoadLinksToFilesForWFWiki(url);
            UpdateMemesCounters();
        }

        public static string GenerateStringWithoutMeme(Meme meme, string message)
        {
            char prefix = '-';
            if (meme.Prefix == "\\\\") prefix = '\\';
            if (meme.Prefix == "\\/") prefix = '/';

            string command = prefix + meme.Name.ToLower();

            int indexOfCommand = message.ToLower()
                .IndexOf(command, StringComparison.Ordinal);

            int lenghtOfCommand = command.Length;

            return message.Remove(indexOfCommand, lenghtOfCommand);

        }

        public static string FindMemeString(string message)
        {
            foreach (var memeWfWikiCategory in MemeWfWikiDictionary.Keys)
            {
                foreach (var memeWfWiki in MemeWfWikiDictionary[memeWfWikiCategory])
                {
                    // "(\/ex$)|(\/ex) "
                    List<string> prefixes = new List<string>();
                    prefixes.Add("\\\\");
                    prefixes.Add("\\/");

                    string memeName = memeWfWiki.Name;
                    string text = message;
                    string memeNameLower = memeWfWiki.Name.ToLower();
                    string textLower = message.ToLower();

                    for (int i = 0; i < prefixes.Count; i++)
                    {
                        Regex regex = new Regex("(" + prefixes[i] + memeNameLower + "$)|("
                                                + prefixes[i] + memeNameLower + ") ");
                        //Regex regex = new Regex("(/" + memeName + "$)|(/"+ memeName + ") ");


                        string newText = "";
                        MatchCollection matches = regex.Matches(textLower);
                        if (matches.Count > 0)
                        {


                            newText = matches[0].Value;
                            return newText;
                        }

                    }
                }
            }

            return null;
        }

        public static Meme FindMeme(string message)
        {
            foreach (var memeWfWikiCategory in MemeWfWikiDictionary.Keys)
            {
                foreach (var memeWfWiki in MemeWfWikiDictionary[memeWfWikiCategory])
                {
                    // "(\/ex$)|(\/ex) "
                    List<string> prefixes = new List<string>();
                    prefixes.Add("\\\\");
                    prefixes.Add("\\/");

                    string memeName = memeWfWiki.Name;
                    string text = message;
                    string memeNameLower = memeWfWiki.Name.ToLower();
                    string textLower = message.ToLower();

                    for (int i = 0; i < prefixes.Count; i++)
                    {
                        Regex regex = new Regex("(" + prefixes[i] + memeNameLower + "$)|("
                                                + prefixes[i] + memeNameLower + ") ");
                        //Regex regex = new Regex("(/" + memeName + "$)|(/"+ memeName + ") ");
                        
                        MatchCollection matches = regex.Matches(textLower);
                        if (matches.Count > 0)
                        {
                            
                            return new Meme(memeWfWiki.Name, memeWfWiki.Path, prefixes[i]);

                            //newText = matches[0].Value;
                            //return newText;
                        }

                    }
                }
            }

            return null;
        }










        



        static void LoadLinksToFilesForWFWiki(string url)
        {
            MemeWfWikiDictionary.Clear();

            String currentPath = Directory.GetCurrentDirectory();
            String pathWithName = currentPath + "\\text.txt";


            WebResponse response = null;
            StreamReader reader = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                request.Method = "GET";

                response = request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                var result = reader.ReadToEnd();
                //Console.WriteLine(result);
                //Console.WriteLine(PathWithName);
                //Writes loaded page to file.
                //StreamWriter w = new StreamWriter(PathWithName, false);
                //w.Write(result);
                //w.Close();


                List<Meme> memesInFolderList = new List<Meme>();


                string s = result;
                //Regex regex = new Regex(@"<tr><td valign=""top""><img src=""\/icons\/.+\"" alt=""\[(.+)\]""><\/td><td><a href=""(.+)"">(.+)<\/a><\/td><td align=""right"">(.+)  <\/td><td align=""right"">(.+)<\/td><td>&nbsp;<\/td><\/tr>");
                Regex regex = new Regex(
                    @"<tr><td valign=""top""><img src=""\/icons\/.+\"" alt=""\[(.+)\]""><\/td><td><a href=""(.+)(\/|\.gif|\.bmp|\.png|\.jpeg|\.jpg)"">(.+)<\/a><\/td><td align=""right"">(.+)  <\/td><td align=""right"">(.+)<\/td><td>&nbsp;<\/td><\/tr>");

                MatchCollection matches = regex.Matches(s);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        //Console.WriteLine(match.Value);
                        if (match.Groups[1].ToString() == "IMG")
                        {
                            //Console.WriteLine("(" + match.Groups[1] + ") " + match.Groups[2] + "    " + url +
                            //                  match.Groups[4]);
                            memesInFolderList.Add(new Meme(match.Groups[2].ToString(), url + match.Groups[4]));
                        }

                        if (match.Groups[1].ToString() == "DIR")
                        {
                            //Console.WriteLine("(" + match.Groups[1] + ") " + match.Groups[2] + "    " + url +
                            //                  match.Groups[2] + "/");

                            ;

                            MemeWfWikiDictionary.Add(
                                match.Groups[2].ToString(),
                                LoadLinksToFilesFromFolder(url + match.Groups[2] + "/"));
                        }
                    }

                    string nameOfnoncategoryItems = "non category";
                    MemeWfWikiDictionary.Add(
                        nameOfnoncategoryItems,
                        memesInFolderList);
                }
                else
                {
                    Console.WriteLine("Совпадений не найдено");
                }
            }
            catch (Exception ex)
            {
                // handle error
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (response != null)
                    response.Close();
            }
        }


        static List<Meme> LoadLinksToFilesFromFolder(string url)
        {
            string result;
            WebResponse response = null;
            StreamReader reader = null;
            List<Meme> memesInFolderList = new List<Meme>();

            try
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                request.Method = "GET";
                response = request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = reader.ReadToEnd();


                string s = result;
                Regex regex = new Regex(
                    @"<tr><td valign=""top""><img src=""\/icons\/.+\"" alt=""\[(.+)\]""><\/td><td><a href=""(.+)(\/|\.gif|\.bmp|\.png|\.jpeg|\.jpg)"">(.+)<\/a><\/td><td align=""right"">(.+)  <\/td><td align=""right"">(.+)<\/td><td>&nbsp;<\/td><\/tr>");
                //Console.WriteLine(s);
                //Console.WriteLine("");
                //Console.WriteLine("");
                //Console.WriteLine("");
                MatchCollection matches = regex.Matches(s);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        //Console.WriteLine(match.Value);
                        if (match.Groups[1].ToString() == "IMG")
                        {
                            //Console.WriteLine("(" + match.Groups[1] + ") " + match.Groups[2] + "    " + url +
                            //                                    match.Groups[4]);
                            memesInFolderList.Add(new Meme(match.Groups[2].ToString(), url + match.Groups[4]));
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Совпадений в " + url + " \"не найдено");
                }
            }
            catch (Exception ex)
            {
                // handle error
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (response != null)
                    response.Close();
            }

            return memesInFolderList;
        }

        public static string MemesString(bool pathsInfo = false)
        {
            string result = "";
            foreach (var memeWFWikiCategory in MemeWfWikiDictionary.Keys)
            {
                result += "\n/" + memeWFWikiCategory + "\n";
                foreach (var memeWFWiki in MemeWfWikiDictionary[memeWFWikiCategory])
                {
                    if (!pathsInfo)
                    {
                        result += " " + memeWFWiki.Name;
                    }
                    else
                    {
                        result += memeWFWiki.Name + ": " + memeWFWiki.Path + "\n";
                    }
                    //var wiki = memeWFWiki;
                    //commands.CreateCommand(memeWFWiki.Name)
                    //    .Description("WF Wiki meme")
                    //    .Do(async (e) =>
                    //    {

                    //        Message[] messagesToDelete;
                    //        messagesToDelete = new[] { e.Message };

                    //        var task1 = e.Channel.SendFile(wiki.Path);
                    //        var task2 = e.Channel.DeleteMessages(messagesToDelete);
                    //        await Task.WhenAll(task1, task2);
                    //        await e.Channel.SendMessage(e.User.NicknameMention);
                    //    });
                    //}
                }

                result += "\n";
                //for (int i = 0; i < memeWFWikiListNames.Count; i++)
                //{
                //    var i1 = i;
                //    commands.CreateCommand(memeWFWikiListNames[i])
                //        .Do(async (e) =>
                //        {
                //            int num = i1;
                //            Message[] messagesToDelete;
                //            messagesToDelete = new[] { e.Message };

                //            var task1 = e.Channel.SendFile(memeWFWikiList[num]);
                //            var task2 = e.Channel.DeleteMessages(messagesToDelete);
                //            await Task.WhenAll(task1, task2);
                //            await e.Channel.SendMessage(e.User.NicknameMention);
                //        });
                //}
            }

            return result;
        }

        public static DiscordEmbed MemesEmbed(bool pathsInfo = false)
        {
            var result = new DiscordEmbedBuilder();


            foreach (var memeWFWikiCategory in MemeWfWikiDictionary.Keys.Reverse())
            {
                string list = "";
                foreach (var memeWFWiki in MemeWfWikiDictionary[memeWFWikiCategory])
                {
                    if (!pathsInfo)
                    {
                        list += " " + memeWFWiki.Name;
                    }
                    else
                    {
                        list += memeWFWiki.Name + ": " + memeWFWiki.Path + "\n";
                    }
                }

                result.AddField(memeWFWikiCategory, list);
            }

            return result;
        }

        public static void UpdateMemesCounters(bool pathsInfo = false)
        {
            CountOfCategories = MemeWfWikiDictionary.Keys.Count;
            CountOfMemes = 0;

            foreach (var memeWFWikiCategory in MemeWfWikiDictionary.Keys)
            {
                CountOfMemes += MemeWfWikiDictionary[memeWFWikiCategory].Count;
            }
        }
    }
}