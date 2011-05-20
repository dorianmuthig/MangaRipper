﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MangaRipper
{
    public class TitleMangaToshokan : TitleBase
    {
        public TitleMangaToshokan(Uri address)
        {
            Address = address;
        }
        protected override List<IChapter> ParseChapterObjects(string html)
        {
            var list = new List<IChapter>();
            Regex reg = new Regex("<td width='40%' align='left' class='ccell'><a href='(?<Value>[^']+)' title=\"[^\"]+\">(?<Text>[^<]+)</a><span>",
                RegexOptions.IgnoreCase);
            MatchCollection m = reg.Matches(html);

            foreach (Match item in m)
            {
                var value = new Uri(Address, item.Groups["Value"].Value);
                string name = item.Groups["Text"].Value;

                IChapter chapter = new ChapterMangaToshokan(name, value);
                list.Add(chapter);
            }

            return list;
        }

        protected override List<Uri> ParseChapterAddresses(string html)
        {
            var list = new List<Uri>();
            Regex reg = new Regex(@"<a href='(?<Value>http://www.mangatoshokan.com/series/[^/]+/\d+)'>\d+</a>",
                RegexOptions.IgnoreCase);
            MatchCollection m = reg.Matches(html);

            foreach (Match item in m)
            {
                if (list.Where(r=>r.AbsoluteUri == item.Groups["Value"].Value).Count() == 0)
                {
                    var value = new Uri(Address, item.Groups["Value"].Value);
                    list.Add(value);
                }
            }

            return list;
        }
    }
}
