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
using System.IO.IsolatedStorage;

namespace MangaRipper
{
    static class Common
    {
        /// <summary>
        /// Save BindingList of IChapter to IsolateStorage
        /// </summary>
        /// <param name="chapters"></param>
        /// <param name="fileName"></param>
        public static void SaveIChapterCollection(BindingList<IChapter> chapters, string fileName)
        {
            IsolatedStorageFile scope = IsolatedStorageFile.GetUserStoreForApplication();
            using (var fs = new IsolatedStorageFileStream(fileName, FileMode.Create, scope))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, chapters);
            }
        }

        /// <summary>
        /// Load BindingList of IChapter from IsolateStorage
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static BindingList<IChapter> LoadIChapterCollection(string fileName)
        {
            IsolatedStorageFile scope = IsolatedStorageFile.GetUserStoreForApplication();
            BindingList<IChapter> result = null;
            try
            {
                using (var fs = new IsolatedStorageFileStream(fileName, FileMode.Open, scope))
                {
                    if (fs.Length != 0)
                    {
                        IFormatter formatter = new BinaryFormatter();
                        result = (BindingList<IChapter>)formatter.Deserialize(fs);
                    }
                }
            }
            finally
            {
                if (result == null)
                {
                    result = new BindingList<IChapter>(); 
                }
            }

            return result;
        }
    }
}