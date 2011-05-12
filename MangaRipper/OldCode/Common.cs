﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Net;
using System.IO;
using System.Net.Sockets;

namespace MangaRipper
{
    static class Common
    {
        private delegate void MessageBoxThreadSafeDelegate(Form owner, string text, string caption, MessageBoxButtons button, MessageBoxIcon icon);

        private delegate void SetControlPropertyThreadSafeDelegate(Control control, string propertyName, object propertyValue);

        public static void MessageBoxThreadSafe(Form owner, string text, string caption, MessageBoxButtons button, MessageBoxIcon icon)
        {
            if (owner.InvokeRequired)
            {
                owner.Invoke(new MessageBoxThreadSafeDelegate(MessageBoxThreadSafe), owner, text, caption, button, icon);
            }
            else
            {
                MessageBox.Show(owner, text, caption, button, icon);
            }
        }

        public static void SetControlPropertyThreadSafe(Control control, string propertyName, object propertyValue)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new SetControlPropertyThreadSafeDelegate(SetControlPropertyThreadSafe), new object[] { control, propertyName, propertyValue });
            }
            else
            {
                control.GetType().InvokeMember(propertyName, BindingFlags.SetProperty, null, control, new object[] { propertyValue });
            }
        }

        public static string DownloadWebsite(string url, int retryMax, ref bool isCancel)
        {
            int retryCount = 0;
            StringBuilder result = new StringBuilder();
            while (retryCount < retryMax && !isCancel)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Proxy = null;
                    request.Timeout = 30000;
                    request.Credentials = CredentialCache.DefaultCredentials;
                    if (isCancel) { break; }
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (isCancel) { break; }
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            byte[] downBuffer = new byte[1024];
                            int bytesSize = 0;
                            while ((bytesSize = responseStream.Read(downBuffer, 0, downBuffer.Length)) > 0)
                            {
                                if (isCancel) { break; }
                                result.Append(System.Text.Encoding.GetEncoding("UTF-8").GetString(downBuffer, 0, bytesSize));
                            }
                        }
                    }
                    break;
                }
                catch (Exception)
                {
                    retryCount++;
                    if (retryCount == retryMax)
                    {
                        throw;
                    }
                }
            }
            if (!isCancel)
            {
                return result.ToString();
            }
            else
            {
                return "";
            }
        }

        public static void DownloadImage(string imageURL, string saveToFolder, int retryMax, ref bool isCancel)
        {
            int retryCount = 0;
            while (retryCount < retryMax && !isCancel)
            {
                try
                {
                    string filename = imageURL.Remove(0, imageURL.LastIndexOf("/") + 1);
                    if (!File.Exists(saveToFolder + "\\" + filename))
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(imageURL);
                        request.Proxy = null;
                        request.Credentials = CredentialCache.DefaultCredentials;
                        if (isCancel) { break; }
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (isCancel) { break; }
                            using (Stream responseStream = response.GetResponseStream())
                            {
                                using (Stream strLocal = new FileStream(saveToFolder + "\\" + filename + ".mr", FileMode.Create, FileAccess.Write, FileShare.None))
                                {
                                    byte[] downBuffer = new byte[1024];
                                    int bytesSize = 0;
                                    while ((bytesSize = responseStream.Read(downBuffer, 0, downBuffer.Length)) > 0)
                                    {
                                        if (isCancel) { break; }
                                        strLocal.Write(downBuffer, 0, bytesSize);
                                    }
                                }
                                if (!isCancel)
                                {
                                    File.Move(saveToFolder + "\\" + filename + ".mr", saveToFolder + "\\" + filename);
                                }
                            }
                        }
                    }
                    break;
                }
                catch (Exception)
                {
                    retryCount++;
                    if (retryCount == retryMax)
                    {
                        throw;
                    }
                }
            }
        }
    }
}