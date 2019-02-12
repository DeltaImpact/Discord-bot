using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using HtmlAgilityPack;

namespace DSPlus.Examples
{
    public class SearchResult
    {
        public string header;
        public string link;
    }

    public static class WikiParsing
    {
        const string SearchPage =
            //"http://ru.warframe.wikia.com/wiki/%D0%A1%D0%BB%D1%83%D0%B6%D0%B5%D0%B1%D0%BD%D0%B0%D1%8F:Search?";
            //"http://httpbin.org/get?";
            //"http://httpbin.org/get?query=dasd";
            "http://ru.warframe.wikia.com/wiki/%D0%A1%D0%BB%D1%83%D0%B6%D0%B5%D0%B1%D0%BD%D0%B0%D1%8F:Search?query=";

        public static async Task<List<SearchResult>> GetInfoAboutItem(string nameOfThing)
        {
            //var doc = GrabHtmlDataByWebClient(nameOfThing);
            var doc = await DownloadPage(nameOfThing);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(doc);
            var items = new List<SearchResult>();
            try
            {
                var searchResults = htmlDoc.DocumentNode.SelectNodes("//li[@class=\"result\"]").ToList().GetRange(0, 4);
                for (int i = 0; i < searchResults.Count; i++)
                {
                    var currentNode = searchResults[i];
                    var currentNodeRaw = searchResults[i].InnerHtml;
                    HtmlDocument currentNodeRebuilt = new HtmlDocument();
                    currentNodeRebuilt.LoadHtml(currentNodeRaw);
                    var link = "";
                    var header = "";
                    var headerNode = currentNodeRebuilt.DocumentNode.SelectSingleNode("//a[@class=\"result-link\"]");
                    foreach (var attr in headerNode.Attributes)
                        if (attr.Name == "href")
                            link = attr.Value;
                    header = headerNode.InnerText;
                    items.Add(new SearchResult {header = header, link = link});
                }
            }
            catch (Exception e)
            {
                throw new KeyNotFoundException();
            }

            return items;
        }

        public static object ParseItemPage(HtmlDocument page)
        {
            var itemImg = "";
            foreach (var link in page.DocumentNode.SelectNodes("//img[@class=\"lzyPlcHld lzyTrns lzyLoaded\"]"))
            {
                foreach (var attr in link.Attributes)
                    if (attr.Name == "href")
                        itemImg = attr.Value;
            }

            var pageHeader = "";
            foreach (var curPageHeader in page.DocumentNode.SelectNodes("//h1[@class=\"page-header__title\"]"))
            {
                pageHeader = curPageHeader.InnerText;
                break;
            }

            //foreach (var div in page.DocumentNode.SelectNodes("//div[@class=\"pi-data-value pi-font\"]"))
            //{
            //}

            return new {img = itemImg, header = pageHeader};
        }

        static async Task<string> DownloadPage(string nameOfThing)
        {
            using (var client = new HttpClient())
            {
                using (var r = await client.GetAsync(new Uri(SearchPage + nameOfThing)))
                {
                    string result = await r.Content.ReadAsStringAsync();
                    return result;
                }
            }
        }

        private static string GrabHtmlDataByWebClient(string nameOfThing)
        {
            string moodysWebstring = SearchPage + nameOfThing;
            Uri moodysWebAddress = new Uri(moodysWebstring);

            var httpClient = new HttpClient();
            return httpClient.GetAsync(moodysWebAddress).Result.Content.ReadAsStringAsync().Result;
        }
    }
}