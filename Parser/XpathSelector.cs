using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace Parser
{
    public static class XpathSelector
    {
        public static IEnumerable<HtmlNode> Get(string html, string xpath)
        {
            HtmlNodeCollection nodes = GetDocumentNode(html).SelectNodes(xpath);

            if (nodes != null)
                return nodes.ToList();

            return new HtmlNode[0];
        }

        private static HtmlNode GetDocumentNode(string source)
        {
            var doc = new HtmlDocument { OptionOutputAsXml = true };
            doc.LoadHtml(source);
            return doc.DocumentNode;
        }
    }
}