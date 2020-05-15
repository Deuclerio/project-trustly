using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GithubScraper.Domain
{
    /// <summary>
    /// Scrapes a Github repo returning the number of lines, number of bytes
    /// of all files grouped by file extension.
    /// </summary>
    public class Scraper
    {
        public List<ScrapeResult> Execute(string user, string repo)
        {
            var folderContents = new List<FolderContent>();


            GetFolderContent($"https://github.com/{user}/{repo}", folderContents);


            Parallel.ForEach(folderContents.Where(x => x.Type == "file"), (item) =>
             {
                 GetFileInfo(item);
             });



            var result = folderContents.Where(x => x.Type == "file")
                 .GroupBy(x => x.Extension)
                 .Select(x => new ScrapeResult
                 {
                     FileExtension = x.Key,
                     Size = x.Sum(y => ConvertToBytes(y)),
                     NumberOfLines = x.Sum(y => y.NumberOfLines)
                 });

            return result.OrderBy(x => x.FileExtension).ToList();
        }

        long ConvertToBytes(FolderContent f)
        {
            if (f.SizeUnit == "byte" || f.SizeUnit == "bytes")
            {
                return (long)f.Size;
            }

            if (f.SizeUnit == "kb")
            {
                return (long)f.Size * 1024;
            }

            if (f.SizeUnit == "mb")
            {
                return (long)f.Size * 1024 * 1024;
            }

            throw new Exception("Invalid value:" + f.SizeUnit);

        }

        void GetFolderContent(string pageUrl, List<FolderContent> folderContents)
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

                folderContents.Add(new FolderContent { Type = type, Url = url, Name = name, Extension = GetExtention(name) });

                if (type == "directory")
                {
                    GetFolderContent("https://github.com" + url, folderContents);
                }
            });


        }

        string GetExtention(string file)
        {
            var count = file.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            return count.Length == 1 ? string.Empty : count[1];
        }


        void GetFileInfo(FolderContent file)
        {
            var client = new WebClient();
            byte[] data = client.DownloadData("https://github.com" + file.Url);
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

    public class FolderContent
    {
        public string Type { get; set; }

        public string Url { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public decimal Size { get; set; }

        public string SizeUnit { get; set; }
        public long NumberOfLines { get; set; }
    }

    public class ScrapeResult
    {
        public string FileExtension { get; set; }

        public long NumberOfLines { get; set; }

        public long Size { get; set; }
    }
}
