using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GithubScraper.Domain.Entity;

namespace GithubScraper.Domain
{
    /// <summary>
    /// Scrapes a Github repo returning the number of lines, number of bytes
    /// of all files grouped by file extension.
    /// </summary>
    public class Scraper
    {

        public static string GitHubUrl = "https://github.com/";


        public List<ScrapeResult> Execute(string user, string repo)
        {
            var folderContents = new List<FolderContent>();


            GetFolderContent($"https://github.com/{user}/{repo}", folderContents);


            Parallel.ForEach(folderContents.Where(x => x.Type == ObjectType.FILE), (item) =>
             {
                 GetFileInfo(item);
             });



            var result = folderContents.Where(x => x.Type == ObjectType.FILE)
                 .GroupBy(x => x.Extension)
                 .Select(x => new ScrapeResult
                 {
                     FileExtension = x.Key,
                     Size = x.Sum(y => y.ConvertToBytes(y)),
                     NumberOfLines = x.Sum(y => y.NumberOfLines)
                 });

            return result.OrderBy(x => x.FileExtension).ToList();
        }

        public void GetFolderContent(string pageUrl, List<FolderContent> folderContents)
        {
            var client = new WebClient();
            byte[] data = client.DownloadData(pageUrl);
            var dataStr = client.Encoding.GetString(data);


            var doc = new HtmlDocument();
            doc.LoadHtml(dataStr);

            var trs = doc.DocumentNode.SelectNodes("//table[contains(@class,'files')]/tbody/tr[contains(@class,'js-navigation-item')]");


            Parallel.ForEach(trs, (tr) =>
            {
                var type = tr.SelectSingleNode("td[1]/svg").Attributes["aria-label"].Value;// directory or file
                var url = tr.SelectSingleNode("td[2]/span/a").Attributes["href"].Value;
                var name = tr.SelectSingleNode("td[2]/span/a").InnerText;


                var dirObjectType = type == "file" ? ObjectType.FILE : ObjectType.DIRECTORY;


                var newItem = new FolderContent { Type = dirObjectType, Url = url, Name = name, Extension = GetExtention(name) };

                folderContents.Add(newItem);

                if (newItem.Type == ObjectType.DIRECTORY)
                {
                    GetFolderContent(GitHubUrl + url, folderContents);
                }
            });


        }

        public string GetExtention(string file)
        {
            var count = file.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            return count.Length == 1 ? string.Empty : count[1];
        }

        public void GetFileInfo(FolderContent file)
        {
            var client = new WebClient();
            byte[] data = client.DownloadData(GitHubUrl + file.Url);
            var dataStr = client.Encoding.GetString(data);


            var doc = new HtmlDocument();
            doc.LoadHtml(dataStr);

            var div = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'Box-header')]/div");

            var divider = div.SelectSingleNode("span");

            if (divider == null)// only size
            {
                var fileSize = div.InnerText.Trim().Split(' ');
                file.Size = Convert.ToDecimal(fileSize[0]);
                file.SizeUnit = fileSize[1].ToLower();
            }
            else
            {

                var match = regex.Match(div.InnerText.Trim().Replace('\n', ' '));

                file.NumberOfLines = Convert.ToInt32(match.Groups[1].Value.Replace("lines", ""));

                var fileSize = match.Groups[2].Value.Split(' ');

                file.Size = Convert.ToDecimal(fileSize[0]);
                file.SizeUnit = fileSize[1].Trim().ToLower();


            }

        }


        Regex regex = new Regex(@"(\d* lines).*\(.*\).* ([0-9]*[\.\d*].*[a-zA-Z]*)");


    }
}
