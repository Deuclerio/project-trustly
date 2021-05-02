using System;
using System.Collections.Generic;
using System.Text;

namespace GithubScraper.Domain.Entity
{
    public class FolderContent
    {
        public ObjectType Type { get; set; }

        public string Url { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public decimal Size { get; set; }

        public string SizeUnit { get; set; }
        public long NumberOfLines { get; set; }


        public long ConvertToBytes(FolderContent f)
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
    }
}
