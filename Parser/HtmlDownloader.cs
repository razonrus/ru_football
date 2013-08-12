using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Parser
{
    public static class HtmlDownloader
    {
        private static readonly bool enableCache = false;
        private static readonly Dictionary<string, PageCache> pageCache = new Dictionary<string, PageCache>();

        public static string GetHtml(string url)
        {
            if (enableCache && pageCache.ContainsKey(url))
            {
                PageCache cache = pageCache[url];
                if (cache.Time - DateTime.Now <= TimeSpan.FromHours(1))
                    return cache.Html;

                pageCache.Remove(url);
            }

            var request = (HttpWebRequest) WebRequest.Create(url);

            Console.WriteLine("Get response from {0}", url);
            try
            {
                var response = (HttpWebResponse) request.GetResponse();

                Stream receiveStream = response.GetResponseStream();

                var readStream = new StreamReader(receiveStream, Encoding.UTF8);

                string html = readStream.ReadToEnd();

                if (enableCache)
                    pageCache.Add(url, new PageCache(html));

                response.Close();
                readStream.Close();

                return html;
            }
            catch(Exception)
            {
                return string.Empty;
            }
        }

        #region Nested type: PageCache

        private class PageCache
        {
            public PageCache(string html)
            {
                Html = html;
                Time = DateTime.Now;
            }

            public string Html { get; private set; }

            public DateTime Time { get; private set; }
        }

        #endregion
    }
}