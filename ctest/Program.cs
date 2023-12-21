using System.Data.Common;

Console.WriteLine("0123456789");
Console.WriteLine("0123456789");
Console.WriteLine("0123456789");
Console.WriteLine("0123456789");

int currentRow = Console.CursorTop;
int currentColumn = Console.CursorLeft;


Console.SetCursorPosition(4, 2);
Console.Write("Hello");
Console.SetCursorPosition(currentColumn, currentRow);