using BrawlhallaANMReader.Anm;
using BrawlhallaANMReader.CSV;
using BrawlhallaANMReader.Lang;
using BrawlhallaANMReader.utils;
using Microsoft.Win32;


String BrawlhallaFolder = @"E:\SteamLibrary\steamapps\common\Brawlhalla";
String SwzPath = $"{BrawlhallaFolder}\\SWZ\\837857090";

string?[] InstallPath = new string?[] {
    Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", null) as string,
    Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "SteamPath", null) as string
};

AnmFile thing = new();
thing.Parse($"{BrawlhallaFolder}\\anims\\Animation_Aang.anm");
thing.ToXml(@".\out.xml");

string xml = File.ReadAllText($"{SwzPath}\\Engine\\LanguageTypes.xml");
LanguageType.Parse(xml);

// foreach (LanguageType lang in LanguageType.LanguageTypes) Logger.Debug(lang.ToString());

StringTable.LoadLanguageBins($"{BrawlhallaFolder}\\languages");
Logger.Log(StringTable.GetString("CostumeType_MuninBeach_DisplayName", LanguageType.GetLanguage(1))[..4]);


CsvSerializer<HurtboxTypes> ser = new()
{
    HasHeader = true
};
IList<HurtboxTypes> csvFileData = ser.Deserialize(File.OpenRead($"{SwzPath}\\Game\\hurtboxTypes.csv"));
ser.Serialize(File.OpenWrite(@".\test.csv"), csvFileData);
