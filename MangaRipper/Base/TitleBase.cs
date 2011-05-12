﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Net;

namespace MangaRipper
{
    public abstract class TitleBase : ITitle
    {
        public event RunWorkerCompletedEventHandler RefreshChapterCompleted;

        public event ProgressChangedEventHandler RefreshChapterProgressChanged;

        protected BackgroundWorker _bw;

        abstract protected List<Uri> ParseChapterUrlFromHtml(string html);

        abstract protected List<IChapter> ParseChapterFromHtml(string html);

        public List<IChapter> Chapters
        {
            get;
            protected set;
        }

        public Uri Url
        {
            get;
            protected set;
        }

        public bool IsBusy
        {
            get
            {
                bool busy = false;
                if (_bw != null)
                {
                    busy = _bw.IsBusy;
                }
                return busy;
            }
        }

        public void CancelRefreshChapter()
        {
            if (_bw != null && _bw.IsBusy == true)
            {
                _bw.CancelAsync();
            }
        }

        public void RefreshChapterAsync()
        {
            if (_bw == null || _bw.IsBusy == false)
            {
                _bw = new BackgroundWorker();
                _bw.WorkerReportsProgress = true;
                _bw.WorkerSupportsCancellation = true;

                _bw.DoWork += new DoWorkEventHandler(DoWork);
                _bw.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
                _bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);

                _bw.RunWorkerAsync();
            }
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (RefreshChapterCompleted != null)
            {
                RefreshChapterCompleted(this, e);
            }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (RefreshChapterProgressChanged != null)
            {
                RefreshChapterProgressChanged(this, e);
            }
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            if (_bw.CancellationPending == true)
            {
                e.Cancel = true;
                return;
            }

            var client = new WebClient();
            client.Proxy = null;
            string html = client.DownloadString(Url);

            List<Uri> uris = ParseChapterUrlFromHtml(html);
            var sb = new StringBuilder();

            if (uris != null)
            {
                int count = 0;
                foreach (Uri item in uris)
                {
                    string content = client.DownloadString(item);
                    sb.AppendLine(content);
                    count++;
                    RefreshChapterProgressChanged(this, new ProgressChangedEventArgs(count * 100 / uris.Count, null));
                }
            }

            RefreshChapterProgressChanged(this, new ProgressChangedEventArgs(100, null));

            if (sb.Length == 0)
            {
                sb.AppendLine(html);
            }

            Chapters = ParseChapterFromHtml(sb.ToString());
        }
    }
}