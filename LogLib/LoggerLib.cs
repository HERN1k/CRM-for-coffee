using System.Text;

using LogLib.Types;

namespace LogLib
{
  public class LoggerLib : ILoggerLib
  {
    private static string _currentDirectory { get; } = AppDomain.CurrentDomain.BaseDirectory;
    private static string _directoryLogPath { get; } = $"{_currentDirectory}Logs";
    private static string _warningLogPath { get; } = $"{_directoryLogPath}\\WarningLogs.txt";
    private static string _errorLogPath { get; } = $"{_directoryLogPath}\\ErrorLogs.txt";
    private string _fatalErrorLogPath { get; } = $"{_directoryLogPath}\\FatalErrorLogs.txt";
    private static uint _count { get; set; } = 0;

    public void Write()
    {
      WriteLine(string.Empty);
    }

    public void Write(object input)
    {
      if (input == null)
      {
        WriteLine(string.Empty);
        return;
      }
      WriteLine(input?.ToString());
    }

    private void WriteLine(string? message)
    {
      string time = DateTime.Now.ToString("HH:mm:ss");
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine(new string('-', 64));
      Console.ResetColor();
      Console.ForegroundColor = ConsoleColor.DarkYellow;
      Console.Write($"[ Message ]\t");
      Console.ResetColor();
      Console.ForegroundColor = ConsoleColor.Gray;
      Console.Write($"{time}");
      Console.ResetColor();
      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine($"\t{message}");
      Console.ResetColor();
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine(new string('-', 64));
      Console.ResetColor();
    }

    public void WriteLog(string method)
    {
      if (_count == 0)
        Console.Clear();
      _count++;
      string time = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
      Console.ForegroundColor = ConsoleColor.Green;
      if (method == "GET")
        Console.Write($"[ {method} ]\t\t");
      else
        Console.Write($"[ {method} ]\t");
      Console.ResetColor();
      Console.ForegroundColor = ConsoleColor.DarkYellow;
      Console.Write($"Count: {_count}\t");
      Console.ResetColor();
      Console.ForegroundColor = ConsoleColor.Gray;
      Console.Write($"{time}\n");
      Console.ResetColor();
    }

    public async Task WriteWarningLog(string message)
    {
      string time = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine(new string('-', 64));
      Console.ResetColor();
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.Write("[ Warning ]\t");
      Console.ResetColor();
      Console.ForegroundColor = ConsoleColor.DarkYellow;
      Console.Write($"{time}\n");
      Console.ResetColor();
      Console.ForegroundColor = ConsoleColor.White;
      Console.Write("\tMessage: ");
      Console.ResetColor();
      Console.Write($"{message}\n");
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine(new string('-', 64));
      Console.ResetColor();
      bool isFileExist = await IsExists(_warningLogPath);
      if (!isFileExist)
        return;
      using StreamWriter stream = new(_warningLogPath, true, Encoding.UTF8);
      await stream.WriteLineAsync(new string('-', 64));
      await stream.WriteAsync("[ Warning ]\t");
      await stream.WriteAsync($"{time}\n");
      await stream.WriteLineAsync($"\tMessage: {message}");
      await stream.WriteLineAsync(new string('-', 64));
      stream.Close();
    }

    public async Task WriteErrorLog(Exception exception)
    {
      string time = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine(new string('-', 64));
      Console.ResetColor();
      Console.ForegroundColor = ConsoleColor.Red;
      Console.Write("[ Error ]\t");
      Console.ResetColor();
      Console.ForegroundColor = ConsoleColor.DarkYellow;
      Console.Write($"{time}\n");
      Console.ResetColor();
      Console.ForegroundColor = ConsoleColor.White;
      Console.Write("\tMessage: ");
      Console.ResetColor();
      Console.Write($"{exception.Message}\n");
      Console.ForegroundColor = ConsoleColor.White;
      Console.Write("\tMethod: ");
      Console.ResetColor();
      Console.Write($"{exception.TargetSite}\n");
      Console.ForegroundColor = ConsoleColor.White;
      Console.Write("\tStack trace: ");
      Console.ResetColor();
      Console.Write($"{exception.StackTrace}\n");
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine(new string('-', 64));
      Console.ResetColor();
      bool isFileExist = await IsExists(_errorLogPath);
      if (!isFileExist)
        return;
      using StreamWriter stream = new(_errorLogPath, true, Encoding.UTF8);
      await stream.WriteLineAsync(new string('-', 64));
      await stream.WriteAsync("[ Error ]\t");
      await stream.WriteAsync($"{time}\n");
      await stream.WriteLineAsync($"\tMessage: {exception.Message}");
      await stream.WriteLineAsync($"\tMethod: {exception.TargetSite}");
      await stream.WriteLineAsync($"\tStack trace: {exception.StackTrace}");
      await stream.WriteLineAsync(new string('-', 64));
      stream.Close();
    }

    private async Task<bool> IsExists(string logPath)
    {
      try
      {
        bool IsDirectoryExists = Directory.Exists(_directoryLogPath);
        if (!IsDirectoryExists)
        {
          Directory.CreateDirectory(_directoryLogPath);
        }
        bool IsFileExists = File.Exists(logPath);
        if (!IsFileExists)
        {
          using FileStream fs = File.Create(logPath);
        }
        return true;
      }
      catch (Exception exception)
      {
        string time = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('-', 64));
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[ Fatal Error ]\t");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write($"{time}\n");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("\tMessage: ");
        Console.ResetColor();
        Console.Write($"{exception.Message}\n");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("\tMethod: ");
        Console.ResetColor();
        Console.Write($"{exception.TargetSite}\n");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("\tStack trace: ");
        Console.ResetColor();
        Console.Write($"{exception.StackTrace}\n");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('-', 64));
        Console.ResetColor();
        Environment.Exit(-1);
        using StreamWriter stream = new(_fatalErrorLogPath, true, Encoding.UTF8);
        await stream.WriteLineAsync(new string('-', 64));
        await stream.WriteAsync("[ Fatal Error ]\t");
        await stream.WriteAsync($"{time}\n");
        await stream.WriteLineAsync($"\tMessage: {exception.Message}");
        await stream.WriteLineAsync($"\tMethod: {exception.TargetSite}");
        await stream.WriteLineAsync($"\tStack trace: {exception.StackTrace}");
        await stream.WriteLineAsync(new string('-', 64));
        stream.Close();
        return false;
      }
    }
  }
}
