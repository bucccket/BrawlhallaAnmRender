namespace BrawlhallaANMReader.Steam;
public class Steam
{
    public List<LibraryFolder> LibraryFolders { get; set; } = new();

    private readonly VDF _vdfFile = new();

    public Steam()
    {
        _vdfFile.Parse();
        VdfContainer root = _vdfFile.Root;
        List<VdfContainer> vdfLibraryFolders = root.Find("libraryfolders")?.Children ?? throw new Exception("cannot find libraryfolders");
        vdfLibraryFolders.ForEach(v =>
        {
            LibraryFolders.Add(new LibraryFolder(v));
        });
    }

    public string? FindGameFolder(string gameID)
    {
        return LibraryFolders.Select(l => l.FindGameById(gameID)?.InstallDir).Where(l => l is not null).First();
    }
}

public class SteamApp
{
    public string AppId { get; set; } = default!;
    public string AppName { get; set; } = default!;
    public string InstallDir { get; set; } = default!;

    private readonly VDF _vdfFile = new();

    public SteamApp(string steamappDir, string steamappID)
    {
        _vdfFile.Parse(SanitizePath($"{steamappDir}\\steamapps\\appmanifest_{steamappID}.acf")!);
        VdfContainer appState = _vdfFile.Root.Find("AppState") ?? throw new Exception("Cannot read AppState");
        AppId = appState.FindEntry("appid")?.Value ?? throw new Exception("Cannot read AppId");
        AppName = appState.FindEntry("name")?.Value ?? throw new Exception("Cannot read AppName");
        string subdir = appState.FindEntry("name")?.Value ?? throw new Exception("Cannot read AppName");
        InstallDir = SanitizePath($"{steamappDir}\\steamapps\\common\\{subdir}")!;
    }

    public static string? SanitizePath(string? path)
    {
        if (path is null)
            return null;
        return Path.GetFullPath(new Uri(path).LocalPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
}

public class LibraryFolder
{
    public string LibraryFolderId { get; set; } = default!;
    public List<string> GameIds { get; set; } = new();
    public List<SteamApp> SteamApps { get; set; } = new();
    public string LibraryPath { get; set; }

    public LibraryFolder(VdfContainer libraryFolder)
    {
        LibraryFolderId = libraryFolder.Name ?? throw new Exception("Library folder parsing failed. No ID given");
        LibraryPath = libraryFolder.FindEntry("path")?.Value ?? throw new Exception("Library folder parsing failed. No path given");
        VdfContainer? apps = libraryFolder.Find("apps");
        if (apps is null) return;
        apps.Entries.ForEach(child => GameIds.Add(child.Key));
        GameIds.ForEach(ID => SteamApps.Add(new SteamApp(LibraryPath, ID)));
    }

    public SteamApp? FindGameById(string id) => SteamApps.FirstOrDefault(app => app?.AppId.Equals(id) ?? false, null);
}