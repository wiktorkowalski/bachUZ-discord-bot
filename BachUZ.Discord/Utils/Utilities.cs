using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace BachUZ.Discord.Utils
{
    public static class Utilities
    {
        private static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize)).ToList();
        }
        public static List<string> SplitMessage(string message)
        {
            if (message.Length < 2000)
            {
                return new List<string> { message };
            }

            var messageChunks = new List<string>();
            var lines = message.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                if (line.Length > 2000)
                {
                    if (sb.Length > 0)
                    {
                        messageChunks.Add(sb.ToString());
                        sb.Clear();
                    }
                    var chunks = Split(message, 2000);
                    messageChunks.AddRange(chunks);
                }
                else
                {
                    if (sb.Length + line.Length > 2000)
                    {
                        messageChunks.Add(sb.ToString());
                        sb.Clear();
                    }
                    sb.AppendLine(line);
                }
            }
            messageChunks.Add(sb.ToString());
            return messageChunks;
        }
    }
}
