﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;

namespace MangaRipper
{
    static class Common
    {
        public static void PopulateSiteGrid(DataGridView grid)
        {
            grid.Rows.Clear();
            grid.Rows.Add("BleachExile", "http://manga.bleachexile.com/", "English");
            grid.Rows.Add("MangaFox", "http://www.mangafox.com/", "English");
            grid.Rows.Add("MangaHere", "http://www.mangahere.com/", "English");
            grid.Rows.Add("MangaShare", "http://read.mangashare.com/", "English");
            grid.Rows.Add("MangaToshokan", "http://www.mangatoshokan.com/", "English");
        }

        public static void SaveIChapterCollection(BindingList<IChapter> chapters, string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, chapters);
            }
        }

        public static BindingList<IChapter> LoadIChapterCollection(string fileName)
        {
            var result = new BindingList<IChapter>();

            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    if (fs.Length != 0)
                    {
                        IFormatter formatter = new BinaryFormatter();
                        result = (BindingList<IChapter>)formatter.Deserialize(fs);
                    }
                }
            }
            catch { }

            return result;
        }
    }
}