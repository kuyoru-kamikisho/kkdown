using downlist;
using System;
using System.Net;
using System.Text.RegularExpressions;

var outRegex = @"\[(.*?)\]";
var listFile = @"./downlist.krc";
var targets = new List<FileRef>();

string outpath = "./";
string? line = null;
var reader = new StreamReader(listFile);

Console.WriteLine("Reading the downlist.");

FileRef? fileRef = null;
var tasks = new List<Task>();

while ((line = reader.ReadLine()) != null)
{
    if (string.IsNullOrWhiteSpace(line))
    {
        fileRef = null;
    }
    else
    {
        if (line.StartsWith("#"))
        {
            continue;
        }
        if (line.StartsWith("["))
        {
            var match = Regex.Match(line, outRegex);
            outpath = match.Groups[1].Value;
            if (!Directory.Exists(outpath))
            {
                Directory.CreateDirectory(outpath);
            }
        }
        else if (line.StartsWith("http"))
        {
            var urio = new Uri(line);
            var name = urio.Segments[urio.Segments.Length - 1];
            fileRef!.url = line;
            fileRef!.filename = name;
            fileRef!.outPath = $"{outpath}{name}";

            var targetClone = fileRef!.Clone();
            var task = Task.Run(async () =>
            {
                var httpClient = new HttpClient();
                var response1 = await httpClient.GetAsync(targetClone.url);
                try
                {
                    response1.EnsureSuccessStatusCode();
                    targetClone.size = response1.Content.Headers.ContentLength;
                    Console.WriteLine($"\n{targetClone.name} {targetClone.size} bytes, downloading...");
                    var fileStream = new FileStream(
                        targetClone!.outPath,
                        FileMode.Create,
                        FileAccess.Write
                        );
                    using (Stream responseStream = await response1.Content.ReadAsStreamAsync())
                    {
                        Console.WriteLine($"{targetClone.name} received data...");
                        await responseStream.CopyToAsync(fileStream);
                    }
                    Console.WriteLine($"{targetClone.name} downloaded.");
                    fileStream.Close();
                }
                catch (HttpRequestException error)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nCannot download {targetClone.name}:\n    {error.Message}");
                    Console.ResetColor();
                }
            });
            tasks.Add(task);
        }
        else
        {
            fileRef = new FileRef();
            fileRef.name = line;
        }
    }
}

await Task.WhenAll(tasks);
Console.WriteLine("\nAll tasks have been finished.");
