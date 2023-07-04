using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace BrawlhallaANMReader.Steam
{
    public class VDF
    {
        public VdfContainer Root { get; } = new();

        private static readonly string _pattern = "(?:\"(.*)\".*\\n\\t*{)|(?:\\1\"(.*)\"\\t*\"(.*)\"\\t*?)|(})";
        private static readonly Regex _regex = new(_pattern, RegexOptions.ECMAScript);

        public VDF() { }
        public void Parse()
        {
            string?[] InstallPath = new string?[] {
                SanitizePath(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", null) as string),
                SanitizePath(Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "SteamPath", null) as string)
            };

            Parse(SanitizePath(InstallPath.First(x => x is not null) + @"\steamapps\libraryfolders.vdf") ?? throw new Exception("cannot resolve steam registry path"));
        }

        public static string? SanitizePath(string? path)
        {
            if (path is null)
                return null;
            return Path.GetFullPath(new Uri(path).LocalPath)
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        public void Parse(string path)
        {
            if (!File.Exists(path))
            {
                throw new VDFException($"Path {path} does not exist!");
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

    public class VdfContainer
    {
        public string? Name { get; set; } = default;
        public List<VdfEntry> Entries { get; set; } = new();
        public List<VdfContainer> Children { get; set; } = new();
        public VdfContainer? Parent { get; set; } = null;

        public VdfContainer() { }
        public VdfContainer(string name, VdfContainer parent) { Name = name; Parent = parent; }

        public void AddEntry(string key, string? value) => Entries.Add(new VdfEntry(key, value));
        public void AddEntry(VdfEntry entry) => Entries.Add(entry);
        public void AddChild(VdfContainer child) => Children.Add(child);
        public VdfContainer? Find(string key) => Children.ToList().FirstOrDefault(x => x?.Name?.Equals(key) ?? false, null);
        public VdfEntry? FindEntry(string key) => Entries.ToList().FirstOrDefault(x => x?.Key.Equals(key) ?? false, null);
    }

    public class VdfEntry
    {
        public string Key { get; set; } = default!;
        public string? Value { get; set; } = default;

        public VdfEntry() { }
        public VdfEntry(string key, string? value) { Key = key; Value = value; }
    }
}
