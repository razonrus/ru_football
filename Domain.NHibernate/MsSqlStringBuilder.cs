using System.Collections.Generic;
using System.Linq;
using System.Text;
using IndyCode.Infrastructure.Common.Extensions;

namespace ru_football.Domain.NHibernate
{
    public static class MsSqlStringBuilder
    {
        public static string AddContainsPattern(string value)
        {
            var stringBuilder = new StringBuilder();

            List<string> words = value
                .Split(' ')
                .Select(Prepare)
                .Where(x => string.IsNullOrEmpty(x) == false)
                .ToList();

            if (words.IsEmpty())
                return string.Empty;

            stringBuilder.AppendFormat("\"{0}*\"", words[0]);

            for (int i = 1; i < words.Count; i++)
                stringBuilder.AppendFormat("AND\"{0}*\"", words[i]);

            return stringBuilder.ToString();
        }

        private static string Prepare(string word)
        {
            return word
                .Replace("	", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty);
        }
    }
}