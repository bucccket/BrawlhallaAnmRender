using BrawlhallaANMReader.Anm.Anm;
using BrawlhallaANMReader.Anm.Utils;
using BrawlhallaANMReader.Steam;
using BrawlhallaANMReader.Swz;
using BrawlhallaANMReader.Swz.AbstractTypes;
using BrawlhallaANMReader.Swz.Lang;

Logger.Log("Launched.");

Steam steam = new();

string? BrawlhallaFolder = steam.FindGameFolder("291550"); //TODO: handle no path found here perhaps?

SwzFile.DecryptKey = 837857090;
SwzFile.Decrypt(File.OpenRead(BrawlhallaFolder + @"\Engine.swz"));

string SwzPath = $"{BrawlhallaFolder}\\SWZ\\837857090";

AnmFile thing = new();
thing.Parse($"{BrawlhallaFolder}\\anims\\Animation_Aang.anm");
thing.ToXml(@".\out.xml");

LanguageType.Parse(File.OpenRead($"{SwzPath}\\Engine\\LanguageTypes.xml"));
StringTable.LoadLanguageBins($"{BrawlhallaFolder}\\languages");

HurtboxType.Parse(File.OpenRead($"{SwzPath}\\Game\\hurtboxTypes.csv"));

PowerType.Parse(File.OpenRead($"{SwzPath}\\Game\\powerTypes.csv"));

SpriteData.Parse(File.OpenRead($"{SwzPath}\\Game\\spriteData.csv"));

Logger.Log("Done.");