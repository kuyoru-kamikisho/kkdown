using downcore;
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
            Statics.Log($"The file will be output to: {outputDir}", ConsoleColor.Gray);
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
    var f = Httpk.getSize(d.url);
    f.Wait();
    d.size = f.Result.size;
    d.fileName = f.Result.filename;
}

