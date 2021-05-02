using GithubScraper.Domain;
using GithubScraper.Domain.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace GithubScraper.Test
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestsTheTestRepo()
        {
            var scraper = new Scraper();

            var data = scraper.Execute("Deuclerio", "scrapertestdata");

            Assert.AreEqual(5, data.Count);


            AssertData(data, "", 0, 0);
            AssertData(data, "jpg", 15360, 0);
            AssertData(data, "js", 221, 9);
            AssertData(data, "pdf", 36700160, 0);
            AssertData(data, "ts", 553, 20);

        }

        public void AssertData(List<ScrapeResult> data, string extension, int size, long numberOfLines)
        {
            var ext = data.SingleOrDefault(x => x.FileExtension == extension);

            Assert.AreEqual(size, ext.Size);
            Assert.AreEqual(numberOfLines, ext.NumberOfLines);
        }
    }
}
