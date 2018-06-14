﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Crawler
{
    public class PhantomCrawler
    {
        private static string _js;

        static PhantomCrawler()
        {
            File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "proxy.js"));
        }

        public PhantomCrawler()
        {

        }

        public Response Request(Request request)
        {
            var extension = Path.GetExtension(request.Uri.ToString()).ToLower();
            var file = "ph_download/" + ShortGUID() + extension;

            var args = "proxy.js " + Uri.EscapeUriString(request.Uri.ToString()) + " " + file;
            if (request.Proxy != null)
                args += " " + request.Proxy.Host + " " + request.Proxy.Port + " " + request.Proxy.Username + " " + request.Proxy.Password;

            var p = new Process();
            p.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "phantomjs.exe");
            p.StartInfo.Arguments = args;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = false;
            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.RedirectStandardError = false;
            p.StartInfo.CreateNoWindow = false;
            p.Start();

            p.WaitForExit(15000);
            p.Kill();
            p.Dispose();
            p = null;

            file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);

            var resposne = new Response();
            if (File.Exists(file))
            {
                resposne.Data = File.ReadAllText(file);
                File.Delete(file);
            }

            return resposne;
        }

        private string ShortGUID()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                i *= ((int)b + 1);
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
    }
}