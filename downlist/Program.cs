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

            Console.WriteLine($"Create task: {name}");

            var targetClone = fileRef!.Clone();
            var task = Task.Run(() =>
            {
                var client = new WebClient();
                var fileStream = new FileStream(
                    targetClone!.outPath,
                    FileMode.Create,
                    FileAccess.Write
                    );
                using Stream downloadStream = client.OpenRead(targetClone!.url);
                downloadStream.CopyTo(fileStream);
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
Console.WriteLine("All tasks have been finished.");
