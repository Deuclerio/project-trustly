using System;
using System.Collections.Generic;
using System.Text;

namespace GithubScraper.Domain.Entity
{
    public class ScrapeResult
    {
        public string FileExtension { get; set; }

        public long NumberOfLines { get; set; }

        public long Size { get; set; }
    }
}
