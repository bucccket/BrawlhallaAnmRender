using BrawlhallaANMReader.Anm.Anm;
using BrawlhallaANMReader.Anm.Utils;
using BrawlhallaANMReader.Steam;
using BrawlhallaANMReader.Swz;
using BrawlhallaANMReader.Swz.CSV;

Logger.Log("Launched.");

PRNG prng = new(123);
prng.Random();

Steam steam = new();

string? BrawlhallaFolder = steam.FindGameFolder("291550"); //TODO: handle no path found here perhaps?
string SwzPath = $"{BrawlhallaFolder}\\SWZ\\837857090";

AnmFile thing = new();
thing.Parse($"{BrawlhallaFolder}\\anims\\Animation_Aang.anm");
thing.ToXml(@".\out.xml");

/*
string xml = File.ReadAllText($"{SwzPath}\\Engine\\LanguageTypes.xml");
LanguageType.Parse(xml);
StringTable.LoadLanguageBins($"{BrawlhallaFolder}\\languages");
*/

HurtboxType hbt = new();
hbt.Parse(File.OpenRead($"{SwzPath}\\Game\\hurtboxTypes.csv"));

PowerType pwt = new();
pwt.Parse(File.OpenRead($"{SwzPath}\\Game\\powerTypes.csv"));

SpriteData spd = new();
spd.Parse(File.OpenRead($"{SwzPath}\\Game\\spriteData.csv"));

Logger.Log("Done.");