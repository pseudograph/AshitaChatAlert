using System.Text;
using TextCopy;

namespace AshitaChatAlert;

class AshitaChatAlert
{
    private static string? LatestFile;
    private static List<string>? RegionWhiteList;
    private static List<string>? ContentWhiteList;
    
    [STAThread]
    public static void Main(string[] args)
    {
        ContentWhiteList = new List<string>{"holla", "dem", "mea"};
        Console.WriteLine("Enter whitelist:");
        RegionWhiteList?.Add(new string(Console.ReadLine()).ToLower());
        using var logWatcher =
            (args.Length == 0) ? new FileSystemWatcher(".\\chatlogs") : new FileSystemWatcher(args[0]);
        logWatcher.NotifyFilter =
            NotifyFilters.CreationTime |
            NotifyFilters.FileName |
            NotifyFilters.LastWrite |
            NotifyFilters.Size;

        LoadLatestLog();
        
        logWatcher.Changed += OnChanged;
        logWatcher.Created += OnCreated;
        logWatcher.Filter = "*.log";
        logWatcher.EnableRaisingEvents = true;

        Console.WriteLine("Monitoring log. Press 'q' to exit.");
        Console.ReadLine();
    }

    private static void LoadLatestLog()
    {
        var directory = new DirectoryInfo(".\\chatlogs");
        LatestFile = directory.GetFiles()
            .OrderByDescending(f => f.LastWriteTime)
            .First().FullName;
        Console.WriteLine($"Log loaded: {LatestFile}");
    }

    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine("New log file found.");
        LoadLatestLog();
    }

    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        }

        try
        {
            ClipboardService.SetText($"/pcmd add {GetLastPlayerName()}");
        }
        catch (InvalidDataException exception)
        {
            Console.WriteLine(exception.Message);
        }
    }

    private static string GetLastPlayerName()
    {
        if (LatestFile != null)
        {
            string lastLine = File.ReadLines(LatestFile).Last().ToLower();
            string[] lastLineParts = lastLine.Split(':');
            if (lastLineParts.Length < 4) throw new InvalidDataException("Probably an emote.");
            string playerRegion = lastLineParts[2].Remove(0, 4);
            string content = lastLineParts[3];
            if (!CheckContentAgainstWhiteList(content)) throw new InvalidDataException("Not in whitelist.");
            return playerRegion.Contains('[') ? CleanYell(playerRegion) : CleanShout(playerRegion);
        }
    }

    private static bool CheckContentAgainstWhiteList(string content)
    {
        bool relevantShout = false;
        if (ContentWhiteList == null) return relevantShout;
        foreach (string whitelistEntry in ContentWhiteList.Where(content.Contains))
        {
            relevantShout = true;
        }

        return relevantShout;
    }

    private static string CleanYell(string noTimestampString)
    {
        int regionLeftIndex = noTimestampString.IndexOf('[');
        int regionLength = noTimestampString.IndexOf(']') - regionLeftIndex - 1;
        string region = noTimestampString.Substring(regionLeftIndex + 1, regionLength);
        
        if (!region.Contains(RegionWhiteList[0])) throw new InvalidDataException("Not in ID whitelist");
        var cleanedString = new StringBuilder("");
        foreach (char c in noTimestampString.TakeWhile(c => c != '[' && c != ':'))
        {
            cleanedString.Append(c);
        }
        return cleanedString.ToString();
    }

    private static string CleanShout(string noTimestampString)
    {
        var cleanedString = new StringBuilder("");
        foreach (char c in noTimestampString.TakeWhile(c => c != '[' && c != ':'))
        {
            cleanedString.Append(c);
        }
        return cleanedString.ToString();
    }
}