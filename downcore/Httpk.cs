using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace downcore
{
    public class LinkFileInfo
    {
        public string filename;
        public long size;
    }
    public static class Httpk
    {
        /// <summary>
        /// Unit is bit.
        /// </summary>
        public static async Task<LinkFileInfo> getSize(string url)
        {
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, url);
            HttpResponseMessage response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            long size = response.Content.Headers.ContentLength ?? -1;
            string[] segs = response.RequestMessage.RequestUri.Segments;
            string fileName = response.Content.Headers.ContentDisposition?.FileName
                ?? segs[segs.Length - 1]
                ?? "resource" + new Random().Next();

            return new LinkFileInfo
            {
                filename = fileName,
                size = size,
            };
        }
    }
}
