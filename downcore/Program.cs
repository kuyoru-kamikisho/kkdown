using downcore;
using System.Timers;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

var regexStr = new Regex(@"^\d+$");
var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

int fragmentLength = 4;
string outputDir = currentDirectory;
List<DownTarget> linkTargtes = new List<DownTarget>();

bool v = args.Any(s => s == "-v");
bool n = args.Any(s => s == "-n");
bool o = args.Any(s => s == "-o");
bool l = args.Any(s => s == "-l");
bool h = args.Any(s => s == "-h");

if (v)
{
    Statics.Log(Statics.appName, ConsoleColor.DarkRed);
    Statics.Log(Statics.version, ConsoleColor.DarkRed);
    return;
}

if (h)
{
    Statics.Log($"You can find detailed usage help in the official repository:", ConsoleColor.DarkCyan);
    Statics.Log(Statics.repositry, ConsoleColor.Cyan);
    return;
}

if (n)
{
    var n1 = Statics.FindNextString(args, "-n");
    if (n1 != null && regexStr.IsMatch(n1))
    {
        fragmentLength = int.Parse(n1);
    }
    else
    {
        Statics.Log(@"You have entered the parameter ""-n"" but did not specify the correct number of rows (integer).", ConsoleColor.Red);
    }
}
if (o)
{
    var o1 = Statics.FindNextString(args, "-o");
    if (o1 != null)
    {
        outputDir = Path.Combine(currentDirectory, o1);
        var o2 = Directory.Exists(outputDir);
        if (o2)
        {
            Statics.Log($"The file will be output to: {outputDir}", ConsoleColor.DarkYellow);
        }
        else
        {
            Statics.Log($"Path {o1} not exists, will auto create it.", ConsoleColor.Gray);
            Directory.CreateDirectory(outputDir);
        }
    }
    else
    {
        Statics.Log(@"You have entered the parameter ""-o"" but did not specify the correct path of out (string).", ConsoleColor.Red);
    }
}
if (l)
{
    var lh = Statics.FindNextString(args, "-l");
    if (lh != null)
    {
        var x = Array.IndexOf(args, "-l");
        for (int i = x; i < args.Length; i++)
        {
            if (args[i].StartsWith("http"))
            {
                var d = new DownTarget();
                d.url = args[i];
                linkTargtes.Add(d);
            }
        }
    }
    else
    {
        Statics.Log(@"You have entered the parameter ""-l"" but did not specify a url.", ConsoleColor.Red);
        return;
    }
}

foreach (var d in linkTargtes)
{
    try
    {
        var f = Httpk.getSize(d.url);
        f.Wait();
        d.size = f.Result.size;
        d.fileName = f.Result.filename;

        long remain = d.size;
        long dpsize = d.size / fragmentLength;
        d.pieceGetedSize = new long[fragmentLength];
        d.starts = new long[fragmentLength];
        d.ends = new long[fragmentLength];

        for (int i = 0; i < fragmentLength; i++)
        {
            remain -= dpsize;
            d.starts[i] = dpsize * i;
            if (dpsize <= remain)
            {
                d.ends[i] = dpsize * (i + 1) - 1;
            }
            else
            {
                d.ends[i] = d.size;
            }
        }
    }
    catch (AggregateException error)
    {
        d.errorInfo = error.Message;
    }
}

Statics.Log("Get resource info success.", ConsoleColor.Cyan);
Statics.Log("Your download has been started, please wait a moment.", ConsoleColor.Cyan);

List<Task> downloadTasks = new List<Task>();

int ctop = Console.CursorTop;
int cleft = Console.CursorLeft;
bool iswriting = false;

Console.Clear();
Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.WriteLine();
Console.WriteLine("  ┌─────────────────┐");
Console.WriteLine("  │  kkdown v0.0.1  │");

for (int j = 0; j < linkTargtes.Count; j++)
{
    int cj = j;
    var d = linkTargtes[j];

    if (d.fileName != null)
    {
        if (j == 0)
        {
            Console.WriteLine("  ├─────────────────┴──────────────────────────────────────────────┐");
        }
        else
        {
            Console.WriteLine("  ├────────────────────────────────────────────────────────────────┤");
        }
        Console.WriteLine("  │                                                                │");

        Console.WriteLine($"  │  file     {d.fileName}");
        Console.WriteLine("  │  total    [                    ] 0% ");

        for (int i = 0; i < fragmentLength; i++)
        {
            int ci = i;
            var r1 = d.starts[ci];
            var r2 = d.ends[ci];

            Console.WriteLine($"  │  p{i + 1}       [                    ] 0% ");

            Task task = Task.Run(async () =>
            {
                var outPiece = Path.Combine(outputDir, $"./{d.fileName}_{ci}");
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(d.url, HttpCompletionOption.ResponseHeadersRead);

                response.EnsureSuccessStatusCode();
                client.DefaultRequestHeaders.Range = new RangeHeaderValue(r1, r2);

                var stream = await client.GetStreamAsync(d.url);
                var outputFile = new FileStream(outPiece, FileMode.Create, FileAccess.Write);

                byte[] buffer = new byte[8192];
                long downloadedSize = 0;
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await outputFile.WriteAsync(buffer, 0, bytesRead);
                    downloadedSize += bytesRead;
                    d.pieceGetedSize[ci] = downloadedSize;

                    if (!iswriting)
                    {
                        iswriting = true;

                        double tp = Math.Floor(d.getTotalProgress() * 10000) / 100;
                        double pp = Math.Floor(d.getPieceProgress(ci) * 10000) / 100;
                        int tn = (int)Math.Floor(tp / 5);
                        int pn = (int)Math.Floor(pp / 5);

                        Console.SetCursorPosition(0, 5 + cj * 8);
                        Console.WriteLine($"  │  file     {d.fileName}");
                        Console.SetCursorPosition(0, 6 + cj * 8);
                        Console.WriteLine($"  │  total    [{new string('■', tn)}{new string(' ', 20 - tn)}] {tp}%");
                        Console.SetCursorPosition(0, 7 + ci + cj * 8);
                        Console.WriteLine($"  │  p{ci + 1}       [{new string('■', pn)}{new string(' ', 20 - pn)}] {pp}%");
                        iswriting = false;
                    }
                }
                outputFile.Close();
            });
            downloadTasks.Add(task);
        }

        Console.WriteLine("  │                                               ");

        if (j == linkTargtes.Count - 1)
        {
            Console.WriteLine("  └────────────────────────────────────────────────────────────────┘");
        }
    }
}

ctop = Console.CursorTop;
cleft = Console.CursorLeft;

await Task.WhenAll(downloadTasks);

Console.ResetColor();
Console.SetCursorPosition(cleft, ctop);

Statics.Log("\n  All download tasks have been finished, start merge...", ConsoleColor.Green);

for (int i = 0; i < linkTargtes.Count; i++)
{
    var dti = linkTargtes[i];

    if (dti.fileName != null)
    {
        var fpath = Path.Combine(outputDir + "/", dti.fileName);
        var writer = new FileStream(fpath, FileMode.Create, FileAccess.Write);
        for (int j = 0; j < fragmentLength; j++)
        {
            var outPiece = Path.Combine(outputDir, $"./{dti.fileName}_{j}");
            FileStream segmentStream = File.OpenRead(outPiece);
            await segmentStream.CopyToAsync(writer);
            segmentStream.Close();
            File.Delete(outPiece);
        }
        writer.Close();
        Statics.Log("  output to:  \n    " + fpath, ConsoleColor.Green);
    }
    else
    {
        Statics.Log($"  download link error in {i}: {dti.errorInfo}\n  link: {dti.url}\n\n", ConsoleColor.DarkRed);
    }
}

