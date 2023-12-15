using downcore;
using System.Timers;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

var regexStr = new Regex(@"^\d+$");
var speedStr = new Regex(@"^\d+[km]b/s$");
var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

int fragmentLength = 4;
string outputDir = currentDirectory;
bool withPopupWindow = false;
bool quietMode = false;
List<DownTarget> linkTargtes = new List<DownTarget>();
bool openWhenFinish = false;
int speed = int.MaxValue;

bool v = args.Any(s => s == "-v");
bool n = args.Any(s => s == "-n");
bool o = args.Any(s => s == "-o");
bool a = args.Any(s => s == "-a");
bool s = args.Any(s => s == "-s");
bool l = args.Any(s => s == "-l");
bool c = args.Any(s => s == "-c");
bool h = args.Any(s => s == "-h");
bool m = args.Any(s => s == "-m");

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

if (c)
{
    openWhenFinish = true;
}

if (m)
{
    var mp = Statics.FindNextString(args, "-l");
    if (mp != null)
    {
        var im = speedStr.IsMatch(mp);
        if (im)
        {
            speed = Statics.ParseDataRate(mp);
        }
        else
        {
            Statics.Log($"Invalid speed param: {mp}.", ConsoleColor.Red);
        }
        Statics.Log($"Speed limit: {speed}kb/s.", ConsoleColor.Red);
    }
    else
    {
        Statics.Log(@"You have entered the parameter ""-m"" but did not specify a speed.", ConsoleColor.Red);
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
            d.starts[i] = dpsize * i + 1;
            if (dpsize <= remain)
            {
                d.ends[i] = dpsize * (i + 1);
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

foreach (var d in linkTargtes)
{
    if (d.fileName != null)
    {
        for (int i = 0; i < fragmentLength; i++)
        {
            int ci = i;
            var r1 = d.starts[ci];
            var r2 = d.ends[ci];

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
                }
            });
            downloadTasks.Add(task);
        }
    }
    else
    {
        Statics.Log($"[{d.fileName}...]\t{d.errorInfo}", ConsoleColor.DarkRed);
    }
}

await Task.WhenAll(downloadTasks);

Statics.Log("All tasks have been finished.", ConsoleColor.Green);
