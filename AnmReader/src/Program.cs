﻿using BrawlhallaANMReader.Anm;
using BrawlhallaANMReader.utils;
using Microsoft.Win32;

string?[] InstallPath = new string?[] {
    Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", null) as string,
    Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "SteamPath", null) as string
};

InstallPath?.ToList().ForEach(item => Console.WriteLine("Got: \""+ item + "\""));

AnmFile thing = new();
// thing.Parse(@"E:\SteamLibrary\steamapps\common\Brawlhalla\anims\Animation_Aang.anm");
thing.Parse(@"C:\Program Files (x86)\Steam\steamapps\common\Brawlhalla\anims\Animation_Aang.anm");
thing.ToXml(@".\out.xml");