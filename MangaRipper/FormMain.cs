﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace MangaRipper
{
    public partial class FormMain : Form
    {
        List<IChapter> queue = new List<IChapter>();

        public FormMain()
        {
            InitializeComponent();
        }

        private void btnGetChapter_Click(object sender, EventArgs e)
        {
            var titleUrl = new Uri(txtTitleUrl.Text);
            ITitle title = TitleFactory.CreateTitle(titleUrl);
            title.RefreshChapterCompleted += new RunWorkerCompletedEventHandler(title_RefreshChapterCompleted);
            title.RefreshChapterProgressChanged += new ProgressChangedEventHandler(title_RefreshChapterProgressChanged);

            btnGetChapter.Enabled = false;
            title.RefreshChapterAsync();
            txtPercent.Text = "0%";
        }

        void title_RefreshChapterProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => txtPercent.Text = e.ProgressPercentage + "%"));
            }
            else
            {
                txtPercent.Text = e.ProgressPercentage + "%";
            }
        }

        void title_RefreshChapterCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnGetChapter.Enabled = true;
            ITitle title = (ITitle)sender;
            dgvChapter.DataSource = title.Chapters;

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var items = new List<IChapter>();
            foreach (DataGridViewRow row in dgvChapter.Rows)
            {
                if (row.Selected == true)
                {
                    items.Add((IChapter)row.DataBoundItem);
                }
            }

            items.Reverse();
            foreach (IChapter item in items)
            {
                if (queue.IndexOf(item) < 0)
                {
                    queue.Add(item);
                }
            }

            ReBindQueueList();
        }

        private void ReBindQueueList()
        {
            dgvQueueChapter.DataSource = null;
            dgvQueueChapter.DataSource = queue;
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            var items = new List<IChapter>();
            foreach (DataGridViewRow row in dgvChapter.Rows)
            {
                items.Add((IChapter)row.DataBoundItem);
            }
            items.Reverse();
            foreach (IChapter item in items)
            {
                if (queue.IndexOf(item) < 0)
                {
                    queue.Add(item);
                }
            }

            ReBindQueueList();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in dgvQueueChapter.SelectedRows)
            {
                IChapter chapter = (IChapter)item.DataBoundItem;
                if (chapter.IsBusy == false)
                {
                    queue.Remove(chapter);
                }
            }
            ReBindQueueList();
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            queue.RemoveAll(r => r.IsBusy == false);
            ReBindQueueList();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            DownloadChapter();
        }

        private void DownloadChapter()
        {
            if (dgvQueueChapter.Rows.Count > 0)
            {
                IChapter chapter = (IChapter)dgvQueueChapter.Rows[0].DataBoundItem;

                chapter.RefreshImageUrlProgressChanged += new ProgressChangedEventHandler(chapter_RefreshPageProgressChanged);
                chapter.RefreshImageUrlCompleted += new RunWorkerCompletedEventHandler(chapter_RefreshPageCompleted);

                btnDownload.Enabled = false;
                dgvQueueChapter.Rows[0].Cells["ColChapterStatus"].Value = "0%";
                chapter.RefreshImageUrlAsync(txtSaveTo.Text);
            }
            else
            {
                btnDownload.Enabled = true;
            }
        }

        void chapter_RefreshPageCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IChapter chapter = (IChapter)sender;
            if (e.Cancelled == false && e.Error == null)
            {
                queue.Remove(chapter);
            }
            ReBindQueueList();
            DownloadChapter();
        }

        void chapter_RefreshPageProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            IChapter chapter = (IChapter)sender;
            foreach (DataGridViewRow item in dgvQueueChapter.Rows)
            {
                if (chapter == item.DataBoundItem)
                {
                    item.Cells["ColChapterStatus"].Value = e.ProgressPercentage.ToString() + "%";
                    break;
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (dgvQueueChapter.Rows.Count > 0)
            {
                IChapter chapter = (IChapter)dgvQueueChapter.Rows[0].DataBoundItem;
                chapter.CancelRefreshImageUrl();
            }
        }

        private void btnChangeSaveTo_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = txtSaveTo.Text;
            DialogResult dr = folderBrowserDialog1.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                txtSaveTo.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            Process.Start(txtSaveTo.Text);
        }

        private void dgvSupportedSites_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                Process.Start(dgvSupportedSites.Rows[e.RowIndex].Cells[1].Value.ToString());
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            dgvQueueChapter.AutoGenerateColumns = false;
            dgvChapter.AutoGenerateColumns = false;
            this.Text = String.Format("{0} {1}", Application.ProductName, Application.ProductVersion.Remove(Application.ProductVersion.LastIndexOf(".")));

            dgvSupportedSites.Rows.Add("MangaFox", "http://www.mangafox.com/");
            dgvSupportedSites.Rows.Add("MangaShare", "http://read.mangashare.com/");
        }
    }
}
