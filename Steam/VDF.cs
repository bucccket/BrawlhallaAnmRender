using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace BrawlhallaANMReader.Steam;

/// <summary>
/// VDF/ACF parser, reads .vdf and .acf files into tree-like datastructure
/// </summary>
public class VDF
{
    /// <value> Root of VDF tree </value>
    public VdfContainer Root { get; } = new();

    /// <summary> VDF regex pattern with 3 capture types, open scope, entry, close scope </summary>
    private static readonly string _pattern = "(?:\"(.*)\".*\\n\\t*{)|(?:\\1\"(.*)\"\\t*\"(.*)\"\\t*?)|(})";
    private static readonly Regex _regex = new(_pattern, RegexOptions.ECMAScript);

    /// <summary>
    /// Initializes a new instance of the <see cref="VDF"/> class.
    /// </summary>
    public VDF() { }

    /// <summary>
    /// Parses this instance.
    /// </summary>
    /// <exception cref="FileNotFoundException">cannot resolve steam registry path</exception>
    public void Parse()
    {
        string?[] InstallPath = new string?[] {
            SanitizePath(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", null) as string),
            SanitizePath(Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "SteamPath", null) as string)
        };

        Parse(SanitizePath(InstallPath.First(x => x is not null) + @"\steamapps\libraryfolders.vdf") ?? throw new FileNotFoundException("cannot resolve steam registry path"));
    }

    /// <summary>
    /// Sanitizes a path to not get cluttered with \ and /s as well as other edge cases
    /// </summary>
    /// <param name="path">The raw path as string</param>
    /// <returns>string or null, if null is provided</returns>
    public static string? SanitizePath(string? path)
    {
        if (path is null)
            return null;
        return Path.GetFullPath(new Uri(path).LocalPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    /// <summary>
    /// Parses the VDF file at specified path
    /// </summary>
    /// <param name="path">The path specified</param>
    /// <exception cref="FileNotFoundException">Path does not exist</exception>
    public void Parse(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Path \"{path}\" does not exist!");
        }
        string data = File.ReadAllText(path);
        Match m = _regex.Match(data);
        VdfContainer scope = Root;
        while (m.Success)
        {
            VdfEntry entry = new();
            m.Groups.Values.Where(g => g.Success).ToList()
            .ForEach(g =>
            {
                if (g.Name.Equals("1"))
                {
                    VdfContainer child = new(g.Value, scope);
                    scope.AddChild(child);
                    scope = child;
                }
                if (g.Name.Equals("2")) { entry.Key = g.Value; }
                if (g.Name.Equals("3")) { entry.Value = g.Value; }
                if (g.Name.Equals("4")) { scope = scope.Parent ?? throw new VDFException("Closing scope mismatch."); }
            });
            if (entry.Key is not null) { scope.AddEntry(entry); }
            m = m.NextMatch();
        }
    }
}

/// <summary>
/// Tree class, contains one, none or multipe <see cref="VdfEntry"> VdfEntries </see> or <see cref="VdfContainer"> VdfContainers </see>
/// </summary>
public class VdfContainer
{
    /// <value> Name of container </value>
    public string? Name { get; set; } = default;
    /// <value> all entries inside container </value>
    public List<VdfEntry> Entries { get; set; } = new();
    /// <value> child containers </value>
    public List<VdfContainer> Children { get; set; } = new();
    /// <value> Parent containers. Null if container is root </value>
    public VdfContainer? Parent { get; set; } = null;

    /// <summary> Initializes a new instance of the <see cref="VdfContainer"/> root. </summary>
    public VdfContainer() { }
    /// <summary> Initializes a new instance of the <see cref="VdfContainer"/> child. </summary>
    /// <param name="name">The container's name</param>
    /// <param name="parent">The container's parent.</param>
    public VdfContainer(string name, VdfContainer parent) { Name = name; Parent = parent; }

    /// <summary> Adds the entry. </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public void AddEntry(string key, string? value) => Entries.Add(new VdfEntry(key, value));
    /// <summary> Adds the entry instance </summary>
    /// <param name="entry"> <see cref="VdfEntry"/> instance</param>
    public void AddEntry(VdfEntry entry) => Entries.Add(entry);
    /// <summary> Adds the child instance </summary>
    /// <param name="child">The child.</param>
    public void AddChild(VdfContainer child) => Children.Add(child);
    /// <summary> Finds container by the key specified  </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public VdfContainer? Find(string key) => Children.ToList().FirstOrDefault(x => x?.Name?.Equals(key) ?? false, null);
    /// <summary> Finds entry by the key specified  </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public VdfEntry? FindEntry(string key) => Entries.ToList().FirstOrDefault(x => x?.Key.Equals(key) ?? false, null);
}

/// <summary>
/// Single VDF entry with key and value.
/// </summary>
public class VdfEntry
{
    /// <value> The key </value>
    public string Key { get; set; } = default!;
    /// <value> The value </value>
    public string? Value { get; set; } = default;

    /// <summary> Initializes a new empty instance of the <see cref="VdfEntry"/> class. </summary>
    public VdfEntry() { }
    /// <summary> Initializes a new instance of the <see cref="VdfEntry"/> class. </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public VdfEntry(string key, string? value) { Key = key; Value = value; }
}
