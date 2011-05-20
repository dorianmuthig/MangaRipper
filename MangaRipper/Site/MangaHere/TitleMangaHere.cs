﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MangaRipper
{
    public class TitleMangaHere : TitleBase
    {
        public TitleMangaHere(Uri address)
        {
            Address = address;
        }
        protected override List<IChapter> ParseChapterObjects(string html)
        {
            var list = new List<IChapter>();
            Regex reg = new Regex(@"<a href=""(?<Value>/manga/[^""]+)"" >\r\n\s+(?<Text>[^\t]+)",
                RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);

            foreach (Match match in matches)
            {
                var value = new Uri(Address, match.Groups["Value"].Value);
                string name = match.Groups["Text"].Value;
                IChapter chapter = new ChapterMangaHere(name, value);
                list.Add(chapter);
            }

            return list;
        }

        protected override List<Uri> ParseChapterAddresses(string html)
        {
            return null;
        }
    }
}
