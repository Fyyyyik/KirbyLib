using KirbyLib;
using KirbyLib.IO;
using KirbyLib.Mapping;
using KirbyLib.Mint;
using System;
using System.IO;

namespace KirbyLib_Tests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void MintRtDLTest()
        {
            const string IN_PATH = @"D:\Game Dumps\Kirby's Return to Dreamland\DATA\files\mint\Archive.bin";

            ArchiveRtDL mint;
            using (FileStream stream = new FileStream(IN_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                mint = new ArchiveRtDL(reader);

            Console.WriteLine($"Version: {mint.GetVersionString()}");
            if (mint.ModuleExists("User.Tsuruoka.MintTest"))
            {
                Console.WriteLine("User.Tsuruoka.MintTest:");
                MintObject obj = mint["User.Tsuruoka.MintTest"][0];
                for (int i = 0; i < obj.Variables.Count; i++)
                {
                    MintVariable var = obj.Variables[i];
                    Console.WriteLine($"  {var.Type} {var.Name}");
                }
                Console.WriteLine();
                for (int f = 0; f < obj.Functions.Count; f++)
                {
                    MintFunction func = obj.Functions[f];
                    Console.WriteLine("  " + func.Name + " {");
                    for (int i = 0; i < func.Data.Length; i += 4)
                    {
                        Console.WriteLine($"    {func.Data[i + 0]:X2} {func.Data[i + 1]:X2} {func.Data[i + 2]:X2} {func.Data[i + 3]:X2}");
                    }
                    Console.WriteLine("  }");
                    Console.WriteLine();
                }
            }
        }

        [TestMethod]
        public void MintRtDLIOTest()
        {
            const string IN_PATH = @"D:\Game Dumps\Kirby's Return to Dreamland\DATA\files\mint\Archive.bin";
            const string OUT_PATH = "rtdl_mint_out_test.bin";

            ArchiveRtDL archive;
            using (FileStream stream = new FileStream(IN_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                archive = new ArchiveRtDL(reader);

            Console.WriteLine("Successfully read archive");

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Create, FileAccess.Write))
            using (EndianBinaryWriter writer = new EndianBinaryWriter(stream))
                archive.Write(writer);

            Console.WriteLine("Successfully wrote archive");

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                archive.Read(reader);

            Console.WriteLine("Successfully re-read archive");
        }

        [TestMethod]
        public void MintKSATest()
        {
            const string IN_PATH = @"D:\Game Dumps\Kirby Star Allies\romfs\mint\Step.bin";
            const string MODULE_NAME = "Scn.Step.Hero.Common.StateSliding";
            const string FUNCTION_NAME = "procAnim";

            Archive mint;
            using (FileStream stream = new FileStream(IN_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                mint = new Archive(reader);

            Console.WriteLine($"Version: {mint.GetVersionString()}");
            if (mint.ModuleExists(MODULE_NAME))
            {
                Module mod = mint.GetModule(MODULE_NAME);
                if (mod[0].FunctionExistsByShortName(FUNCTION_NAME))
                {
                    MintFunction func = mod[0].GetFunctionByShortName(FUNCTION_NAME);
                    Console.WriteLine($"{mod.Name}.{func.NameWithoutType()}:");
                    for (int i = 0; i < func.Data.Length; i += 4)
                    {
                        Console.WriteLine($"  {func.Data[i + 0]:X2} {func.Data[i + 1]:X2} {func.Data[i + 2]:X2} {func.Data[i + 3]:X2}");
                    }
                }
            }
            /*
            Console.WriteLine($"Namespaces: {mint.Namespaces.Count}");
            for (int i = 0; i < mint.Namespaces.Count; i++)
            {
                Console.WriteLine($"- {i}: {mint.Namespaces[i].Name}");
                Console.WriteLine($"  - Scripts: {mint.Namespaces[i].Modules}");
                Console.WriteLine($"  - TotalScripts: {mint.Namespaces[i].TotalModules}");
                Console.WriteLine($"  - Children: {mint.Namespaces[i].ChildNamespaces}");
                Console.WriteLine($"  - Unknown: {mint.Namespaces[i].Unknown}");
            }
            */
        }

        [TestMethod]
        public void MintKSAIOTest()
        {
            const string IN_PATH = @"D:\Game Dumps\Kirby Star Allies\romfs\mint\Step.bin";
            const string OUT_PATH = "ksa_mint_step_out.bin";

            Archive archive;
            using (FileStream stream = new FileStream(IN_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                archive = new Archive(reader);

            Console.WriteLine("Successfully read archive");
            Console.WriteLine($"Namespaces: {archive.Namespaces.Count}");
            /*
            for (int i = 0; i < archive.Namespaces.Count; i++)
            {
                Console.WriteLine($"- {i}: {archive.Namespaces[i].Name}");
                Console.WriteLine($"  - Scripts: {archive.Namespaces[i].Modules}");
                Console.WriteLine($"  - TotalScripts: {archive.Namespaces[i].TotalModules}");
                Console.WriteLine($"  - Children: {archive.Namespaces[i].ChildNamespaces}");
                Console.WriteLine($"  - Unknown: {archive.Namespaces[i].Unknown} ({archive.Namespaces[archive.Namespaces[i].Unknown].Name})");
            }
            */

            var namespaces = archive.Namespaces;

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Create, FileAccess.Write))
            using (EndianBinaryWriter writer = new EndianBinaryWriter(stream))
                archive.Write(writer);

            Console.WriteLine("Successfully wrote archive");

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                archive.Read(reader);

            bool NamespaceEqual(MintNamespace a, MintNamespace b)
            {
                return a.Name == b.Name && a.Modules == b.Modules && a.TotalModules == b.TotalModules && a.ChildNamespaces == b.ChildNamespaces && a.Unknown == b.Unknown;
            }

            Console.WriteLine("Successfully re-read archive");
            Console.WriteLine($"Namespaces: {archive.Namespaces.Count}");
            for (int i = 0; i < archive.Namespaces.Count; i++)
            {
                if (!NamespaceEqual(namespaces[i], archive.Namespaces[i]))
                {
                    Console.WriteLine($" - Namespace {i} differs!");
                    Console.WriteLine($"  - Name:          Original = {namespaces[i].Name}, New = {archive.Namespaces[i].Name}");
                    Console.WriteLine($"  - Modules:       Original = {namespaces[i].Modules}, New = {archive.Namespaces[i].Modules}");
                    Console.WriteLine($"  - Total Modules: Original = {namespaces[i].TotalModules}, New = {archive.Namespaces[i].TotalModules}");
                    Console.WriteLine($"  - Children:      Original = {namespaces[i].ChildNamespaces}, New = {archive.Namespaces[i].ChildNamespaces}");
                    Console.WriteLine($"  - Unknown:       Original = {namespaces[i].Unknown}, New = {archive.Namespaces[i].Unknown}");
                }
            }

            /*
            for (int i = 0; i < archive.Namespaces.Count; i++)
            {
                Console.WriteLine($"- {i}: {archive.Namespaces[i].Name}");
                Console.WriteLine($"  - Scripts: {archive.Namespaces[i].Modules}");
                Console.WriteLine($"  - TotalScripts: {archive.Namespaces[i].TotalModules}");
                Console.WriteLine($"  - Children: {archive.Namespaces[i].ChildNamespaces}");
                Console.WriteLine($"  - Unknown: {archive.Namespaces[i].Unknown} ({archive.Namespaces[archive.Namespaces[i].Unknown].Name})");
            }
            */
        }

        [TestMethod]
        public void MintKatFLIOTest()
        {
            const string IN_PATH = @"D:\Game Dumps\Kirby and the Forgotten Land\romfs\basil\Scn.bin";
            const string OUT_PATH = "katfl_basil_scn_out.bin";

            Archive archive;
            using (FileStream stream = new FileStream(IN_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                archive = new Archive(reader);

            Console.WriteLine("Successfully read archive");
            Console.WriteLine($"Namespaces: {archive.Namespaces.Count}");
            /*
            for (int i = 0; i < archive.Namespaces.Count; i++)
            {
                Console.WriteLine($"- {i}: {archive.Namespaces[i].Name}");
                Console.WriteLine($"  - Scripts: {archive.Namespaces[i].Modules}");
                Console.WriteLine($"  - TotalScripts: {archive.Namespaces[i].TotalModules}");
                Console.WriteLine($"  - Children: {archive.Namespaces[i].ChildNamespaces}");
                Console.WriteLine($"  - Unknown: {archive.Namespaces[i].Unknown} ({archive.Namespaces[archive.Namespaces[i].Unknown].Name})");
            }
            */

            var namespaces = archive.Namespaces;

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Create, FileAccess.Write))
            using (EndianBinaryWriter writer = new EndianBinaryWriter(stream))
                archive.Write(writer);

            Console.WriteLine("Successfully wrote archive");

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                archive.Read(reader);

            bool NamespaceEqual(MintNamespace a, MintNamespace b)
            {
                return a.Name == b.Name && a.Modules == b.Modules && a.TotalModules == b.TotalModules && a.ChildNamespaces == b.ChildNamespaces && a.Unknown == b.Unknown;
            }

            Console.WriteLine("Successfully re-read archive");
            Console.WriteLine($"Namespaces: {archive.Namespaces.Count}");
            for (int i = 0; i < archive.Namespaces.Count; i++)
            {
                if (!NamespaceEqual(namespaces[i], archive.Namespaces[i]))
                {
                    Console.WriteLine($" - Namespace {i} differs!");
                    Console.WriteLine($"  - Name:          Original = {namespaces[i].Name}, New = {archive.Namespaces[i].Name}");
                    Console.WriteLine($"  - Modules:       Original = {namespaces[i].Modules}, New = {archive.Namespaces[i].Modules}");
                    Console.WriteLine($"  - Total Modules: Original = {namespaces[i].TotalModules}, New = {archive.Namespaces[i].TotalModules}");
                    Console.WriteLine($"  - Children:      Original = {namespaces[i].ChildNamespaces}, New = {archive.Namespaces[i].ChildNamespaces}");
                    Console.WriteLine($"  - Unknown:       Original = {namespaces[i].Unknown}, New = {archive.Namespaces[i].Unknown}");
                }
            }

            /*
            for (int i = 0; i < archive.Namespaces.Count; i++)
            {
                Console.WriteLine($"- {i}: {archive.Namespaces[i].Name}");
                Console.WriteLine($"  - Scripts: {archive.Namespaces[i].Modules}");
                Console.WriteLine($"  - TotalScripts: {archive.Namespaces[i].TotalModules}");
                Console.WriteLine($"  - Children: {archive.Namespaces[i].ChildNamespaces}");
                Console.WriteLine($"  - Unknown: {archive.Namespaces[i].Unknown} ({archive.Namespaces[archive.Namespaces[i].Unknown].Name})");
            }
            */
        }

        [TestMethod]
        public void MintRtDLDXIOTest()
        {
            const string IN_PATH = @"D:\Game Dumps\Kirby's Return to Dream Land Deluxe\romfs\basil\ScnStep.bin";
            const string OUT_PATH = "rtdldx_basil_scnstep_out.bin";

            Archive archive;
            using (FileStream stream = new FileStream(IN_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                archive = new Archive(reader);

            Console.WriteLine("Successfully read archive");
            Console.WriteLine($"Namespaces: {archive.Namespaces.Count}");
            /*
            for (int i = 0; i < archive.Namespaces.Count; i++)
            {
                Console.WriteLine($"- {i}: {archive.Namespaces[i].Name}");
                Console.WriteLine($"  - Scripts: {archive.Namespaces[i].Modules}");
                Console.WriteLine($"  - TotalScripts: {archive.Namespaces[i].TotalModules}");
                Console.WriteLine($"  - Children: {archive.Namespaces[i].ChildNamespaces}");
                Console.WriteLine($"  - Unknown: {archive.Namespaces[i].Unknown} ({archive.Namespaces[archive.Namespaces[i].Unknown].Name})");
            }
            */

            var namespaces = archive.Namespaces;

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Create, FileAccess.Write))
            using (EndianBinaryWriter writer = new EndianBinaryWriter(stream))
                archive.Write(writer);

            Console.WriteLine("Successfully wrote archive");

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                archive.Read(reader);

            bool NamespaceEqual(MintNamespace a, MintNamespace b)
            {
                return a.Name == b.Name && a.Modules == b.Modules && a.TotalModules == b.TotalModules && a.ChildNamespaces == b.ChildNamespaces && a.Unknown == b.Unknown;
            }

            Console.WriteLine("Successfully re-read archive");
            Console.WriteLine($"Namespaces: {archive.Namespaces.Count}");
            for (int i = 0; i < archive.Namespaces.Count; i++)
            {
                if (!NamespaceEqual(namespaces[i], archive.Namespaces[i]))
                {
                    Console.WriteLine($" - Namespace {i} differs!");
                    Console.WriteLine($"  - Name:          Original = {namespaces[i].Name}, New = {archive.Namespaces[i].Name}");
                    Console.WriteLine($"  - Modules:       Original = {namespaces[i].Modules}, New = {archive.Namespaces[i].Modules}");
                    Console.WriteLine($"  - Total Modules: Original = {namespaces[i].TotalModules}, New = {archive.Namespaces[i].TotalModules}");
                    Console.WriteLine($"  - Children:      Original = {namespaces[i].ChildNamespaces}, New = {archive.Namespaces[i].ChildNamespaces}");
                    Console.WriteLine($"  - Unknown:       Original = {namespaces[i].Unknown}, New = {archive.Namespaces[i].Unknown}");
                }
            }

            /*
            for (int i = 0; i < archive.Namespaces.Count; i++)
            {
                Console.WriteLine($"- {i}: {archive.Namespaces[i].Name}");
                Console.WriteLine($"  - Scripts: {archive.Namespaces[i].Modules}");
                Console.WriteLine($"  - TotalScripts: {archive.Namespaces[i].TotalModules}");
                Console.WriteLine($"  - Children: {archive.Namespaces[i].ChildNamespaces}");
                Console.WriteLine($"  - Unknown: {archive.Namespaces[i].Unknown} ({archive.Namespaces[archive.Namespaces[i].Unknown].Name})");
            }
            */
        }

        [TestMethod]
        public void YamlIOTest()
        {
            const string IN_PATH = @"D:\Game Dumps\Kirby's Return to Dream Land Deluxe\romfs\exlyml\Omen\MasterSheet.bin";
            const string OUT_PATH = "yaml_out_test.bin";

            Yaml yaml;
            using (FileStream stream = new FileStream(IN_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                yaml = new Yaml(reader);

            Console.WriteLine(yaml);

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Create, FileAccess.Write))
            using (EndianBinaryWriter writer = new EndianBinaryWriter(stream))
                yaml.Write(writer);

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                yaml.Read(reader);

            Console.WriteLine(yaml);
        }

        [TestMethod]
        public void YamlTest()
        {
            const string IN_PATH = @"D:\Game Dumps\Kirby's Return to Dream Land Deluxe\romfs\exlyml\Omen\MasterSheet.bin";

            Yaml yaml;
            using (FileStream stream = new FileStream(IN_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                yaml = new Yaml(reader);

            Console.WriteLine("XData Version: " + yaml.XData.Version[0] + "." + yaml.XData.Version[1]);
            Console.WriteLine("Yaml Version: " + yaml.Version);
            Console.WriteLine(yaml);
        }

        void RecurseYaml(YamlNode node, int indent)
        {
            switch (node.Type)
            {
                default:
                    Console.WriteLine(node.ToString());
                    break;
                case YamlType.Hash:
                case YamlType.Array:
                    Console.WriteLine(new string('\t', indent));
                    for (int i = 0; i < node.Length; i++)
                    {
                        Console.Write(new string('\t', indent) + node.Key(i) + ": ");
                        RecurseYaml(node[i], indent + 1);
                    }
                    break;
            }
        }

        [TestMethod]
        public void MapTestWii()
        {
            const string PATH = @"D:\Game Dumps\Kirby's Return to Dreamland\DATA\files\map";

            string[] maps = Directory.GetFiles(PATH, "*.dat", SearchOption.AllDirectories);
            for (int i = 0; i < maps.Length; i++)
            {
                Console.WriteLine(maps[i].Remove(0, PATH.Length));
                using (FileStream stream = new FileStream(maps[i], FileMode.Open, FileAccess.Read))
                using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                {
                    MapRtDL map = new MapRtDL(reader);

                    PrintMapInfo(map);
                }
            }
        }

        [TestMethod]
        public void MapTestDeluxe()
        {
            const string PATH = @"D:\Game Dumps\Kirby's Return to Dream Land Deluxe\romfs\map\Step";

            string[] maps = Directory.GetFiles(PATH, "*.bin", SearchOption.AllDirectories);
            for (int i = 0; i < maps.Length; i++)
            {
                Console.WriteLine(maps[i].Remove(0, PATH.Length));
                using (FileStream stream = new FileStream(maps[i], FileMode.Open, FileAccess.Read))
                using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                {
                    MapRtDL map = new MapRtDL(reader);

                    PrintMapInfo(map);
                }
            }

            /*
            const string IN_PATH = @"D:\Game Dumps\Kirby's Return to Dream Land Deluxe\romfs\map\Step\Level1\Stage1\Step02.bin";
            const string OUT_PATH = "DX_L1S1S2.bin";

            MapRtDL map;
            using (FileStream stream = new FileStream(IN_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                map = new MapRtDL(reader);

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Create, FileAccess.Write))
            using (EndianBinaryWriter writer = new EndianBinaryWriter(stream))
                map.Write(writer);

            using (FileStream stream = new FileStream(IN_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                map = new MapRtDL(reader);
            */
        }

        [TestMethod]
        public void MapTestTDX()
        {
            const string PATH = @"D:\Game Dumps\Kirby Triple Deluxe\romfs\map\Step";

            string[] maps = Directory.GetFiles(PATH, "*.dat", SearchOption.AllDirectories);
            for (int i = 0; i < maps.Length; i++)
            {
                Console.WriteLine(maps[i].Remove(0, PATH.Length));
                using (FileStream stream = new FileStream(maps[i], FileMode.Open, FileAccess.Read))
                using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                {
                    MapTDX map = new MapTDX(reader);

                    Console.WriteLine($"\t- Width: {map.Width}");
                    Console.WriteLine($"\t- Height: {map.Height}");

                    Console.WriteLine($"\t- BGM:   {map.BGM}");
                    Console.WriteLine($"\t- Unk1:  {map.Unknown1}");
                    Console.WriteLine($"\t- ScreenSplitKind:  {map.ScreenSplitKind}");
                    Console.WriteLine($"\t- Unk3:  {map.Unknown3}");
                    Console.WriteLine($"\t- Unk4:  {map.Unknown4}");
                    Console.WriteLine($"\t- Unk5:  {map.Unknown5}");
                    Console.WriteLine($"\t- Unk6:  {map.Unknown6}");
                    Console.WriteLine($"\t- Unk7:  {map.Unknown7}");
                    Console.WriteLine($"\t- Unk8:  {map.Unknown8}");
                    Console.WriteLine($"\t- Unk9:  {map.Unknown10}");
                    Console.WriteLine($"\t- Unk10: {map.Unknown11}");
                    Console.WriteLine($"\t- Unk11: {map.Unknown12}");
                    Console.WriteLine($"\t- Unk12: {map.Unknown13}");

                    /*
                    for (int j = 0; j < map.Enemies.Length; j++)
                    {
                        var enemy = map.Enemies[j];
                        Console.WriteLine($"\t- Enemy {j}");
                        Console.WriteLine($"\t\t- Kind: {enemy.Name}");
                        Console.WriteLine($"\t\t- Variation: {enemy.Variation}");
                        Console.WriteLine($"\t\t- Param 1: {enemy.Param1}");
                        Console.WriteLine($"\t\t- Param 2: {enemy.Param2}");
                        Console.WriteLine($"\t\t- Param 3: {enemy.Param3}");
                        Console.WriteLine($"\t\t- X: {enemy.X}");
                        Console.WriteLine($"\t\t- Y: {enemy.Y}");
                        Console.WriteLine($"\t\t- Unknown: {enemy.Unknown}");
                        Console.WriteLine($"\t\t- Terrain Group: {enemy.TerrainGroup}");
                    }
                    */
            /*
            for (int j = 0; j < map.CarryItems.Length; j++)
            {
                var item = map.CarryItems[j];
                Console.WriteLine($"\t- Carry Item {j}:");
                Console.WriteLine($"\t\t- Kind: {item.Kind}");
                Console.WriteLine($"\t\t- Variation: {item.Variation}");
                Console.WriteLine($"\t\t- Can Respawn: {item.CanRespawn}");
                Console.WriteLine($"\t\t- X: {item.X}");
                Console.WriteLine($"\t\t- Y: {item.Y}");
            }

            for (int j = 0; j < map.Items.Length; j++)
            {
                var item = map.Items[j];
                Console.WriteLine($"\t- Item {j}:");
                Console.WriteLine($"\t\t- Kind: {item.Kind}");
                Console.WriteLine($"\t\t- Variation: {item.Variation}");
                Console.WriteLine($"\t\t- SubKind: {item.SubKind}");
                Console.WriteLine($"\t\t- X: {item.X}");
                Console.WriteLine($"\t\t- Y: {item.Y}");
                Console.WriteLine($"\t\t- HideModeKind: {item.HideModeKind}");
            }
            */
                }
    }
        }

        [TestMethod]
        public void MapTestKPR()
        {
            const string PATH = @"D:\Game Dumps\Kirby Planet Robobot\romfs\map\Step";

            string[] maps = Directory.GetFiles(PATH, "*.dat", SearchOption.AllDirectories);
            for (int i = 0; i < maps.Length; i++)
            {
                Console.WriteLine(maps[i].Remove(0, PATH.Length));
                using (FileStream stream = new FileStream(maps[i], FileMode.Open, FileAccess.Read))
                using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                {
                    MapKPR map = new MapKPR(reader);

                    Console.WriteLine($"\t- Width: {map.Width}");
                    Console.WriteLine($"\t- Height: {map.Height}");
                    Console.WriteLine($"\t- Background: {map.Background}");
                    Console.WriteLine($"\t- Decor Set: {map.DecorSet}");
                    Console.WriteLine($"\t- BGM: {map.BGM}");
                    Console.WriteLine($"\t- Light Set: {map.LightSet}");
                    Console.WriteLine($"\t- Screen Split Kind:  {map.ScreenSplitKind}");
                    Console.WriteLine($"\t- Unknown 2:  {map.Unknown2}");
                    Console.WriteLine($"\t- Unknown 3:  {map.Unknown3}");
                    Console.WriteLine($"\t- Unknown 4:  {map.Unknown4}");
                    Console.WriteLine($"\t- Unknown 5:  {map.Unknown5}");
                    Console.WriteLine($"\t- Unknown 6:  {map.Unknown6}");
                    Console.WriteLine($"\t- Unknown 7:  {map.Unknown7}");
                    Console.WriteLine($"\t- Unknown 8:  {map.Unknown8}");
                    Console.WriteLine($"\t- Unknown 9:  {map.Unknown9}");
                    Console.WriteLine($"\t- Unknown 10: {map.Unknown10}");
                    Console.WriteLine($"\t- Unknown 11: {map.Unknown11}");
                    Console.WriteLine($"\t- Unknown 12: {map.Unknown12}");
                    Console.WriteLine($"\t- Unknown 13: {map.Unknown13}");
                    Console.WriteLine($"\t- Unknown 14: {map.Unknown14}");
                    
                    /*
                    if (map.Unknown2 == 0)
                        Console.WriteLine("\tUnk6 is 0!");
                    else
                    {
                        if (map.Width % (map.Unknown2 + 1) == 0)
                            Console.WriteLine($"\tWidth ({map.Width}) is divisible by Unk2 + 1 ({map.Unknown2})!");
                        else
                            Console.WriteLine($"\tWidth ({map.Width}) is NOT divisible by Unk2 + 1 ({map.Unknown2})!");

                        if (map.Height % (map.Unknown2 + 1) == 0)
                            Console.WriteLine($"\tHeight ({map.Height}) is divisible by Unk2 + 1 ({map.Unknown2})!");
                        else
                            Console.WriteLine($"\tHeight ({map.Height}) is NOT divisible by Unk2 + 1 ({map.Unknown2})!");
                    }
                    */
                }
            }
        }

        [TestMethod]
        public void MapTestKSA()
        {
            /*
            {
                const string IN_PATH = @"D:\Game Dumps\Kirby Star Allies\romfs\map\Step\Level3\Stage4\Step06.dat";
                const string OUT_PATH = "KSA_L3_S4_S06.dat";

                MapKSA map;
                using (FileStream stream = new FileStream(IN_PATH, FileMode.Open, FileAccess.Read))
                using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                    map = new MapKSA(reader);

                using (FileStream stream = new FileStream(OUT_PATH, FileMode.Create, FileAccess.Write))
                using (EndianBinaryWriter writer = new EndianBinaryWriter(stream))
                    map.Write(writer);

                using (FileStream stream = new FileStream(OUT_PATH, FileMode.Open, FileAccess.Read))
                using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                    map = new MapKSA(reader);

                return;
            }
            */

            const string PATH = @"D:\Game Dumps\Kirby Star Allies\romfs\map\Step";

            string[] maps = Directory.GetFiles(PATH, "*.dat", SearchOption.AllDirectories);
            for (int i = 0; i < maps.Length; i++)
            {
                Console.WriteLine(maps[i].Remove(0, PATH.Length));
                using (FileStream stream = new FileStream(maps[i], FileMode.Open, FileAccess.Read))
                using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                {
                    MapKSA map = new MapKSA(reader);

                    Console.WriteLine("- General:");
                    Console.WriteLine($"  - Unknown1: {map.Unknown1}");
                    Console.WriteLine($"  - Unknown2: {map.Unknown2}");
                    Console.WriteLine($"  - LightSet: {map.LightSet}");
                    Console.WriteLine($"  - CustomRespawn: {map.CustomRespawn}");
                    Console.WriteLine($"  - RespawnStepShift: {map.RespawnStepShift}");
                    Console.WriteLine($"  - RespawnStartPortal: {map.RespawnStartPortal}");
                    Console.WriteLine($"  - BGCameraPos: {map.BGCameraPos}");
                    Console.WriteLine($"  - BGCameraMoveRate: {map.BGCameraMoveRate}");
                    Console.WriteLine($"  - Unknown6: {map.Unknown6}");
                    Console.WriteLine($"  - Unknown7: {map.Unknown7}");
                    Console.WriteLine($"  - BGMIndex: {map.BGMIndex}");
                    Console.WriteLine($"  - Unknown9: {map.Unknown9}");
                }
            }
        }

        //[TestMethod]
        public void MapTestGeneral()
        {
            const string PATH = @"D:\Game Dumps\Kirby's Return to Dream Land Deluxe\romfs\map\Step";

            string[] maps = Directory.GetFiles(PATH, "*.bin", SearchOption.AllDirectories);
            for (int i = 0; i < maps.Length; i++)
            {
                using (FileStream stream = new FileStream(maps[i], FileMode.Open, FileAccess.Read))
                using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                {
                    MapRtDL map = new MapRtDL(reader);

                    Console.WriteLine(maps[i].Remove(0, PATH.Length));
                    Console.WriteLine($"\t- BGM: {map.BGM}");
                    Console.WriteLine($"\t- CamPos: {map.BGCameraPos}");
                    Console.WriteLine($"\t- CamRot: {map.BGCameraRot}");
                    Console.WriteLine($"\t- FOVY: {map.BGCameraFOV}");
                    Console.WriteLine($"\t- MoveH: {map.BGCameraMoveRateH}");
                    Console.WriteLine($"\t- MoveV: {map.BGCameraMoveRateV}");
                    Console.WriteLine($"\t- SFX: {map.SFX}");
                    Console.WriteLine($"\t- Type: {map.Type}");
                    Console.WriteLine($"\t- Custom Respawn: {map.CustomRespawn}");
                    Console.WriteLine($"\t- Respawn Step Shift: {map.RespawnStepShift}");
                    Console.WriteLine($"\t- Respawn Start Portal: {map.RespawnStartPortal}");
                }
            }
        }

        [TestMethod]
        public void MapTestKF2()
        {
            const string IN_PATH = @"D:\Game Dumps\Kirby Fighters 2\romfs\map\Fight\Game\Main\DededeRing.bin";
            const string OUT_PATH = "KF2_DededeRing.bin";

            MapFighters map;
            using (FileStream stream = new FileStream(IN_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                map = new MapFighters(reader);

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Create, FileAccess.Write))
            using (EndianBinaryWriter writer = new EndianBinaryWriter(stream))
                map.Write(writer);

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                map = new MapFighters(reader);
        }

        [TestMethod]
        public void MapConvertTest()
        {
            const string IN_PATH = "Step01.dat";
            const string OUT_PATH = "Step01.bin";

            MapRtDL map;
            using (FileStream stream = new FileStream(IN_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                map = new MapRtDL(reader);

            map.XData.Version = new byte[2] { 5, 0 };
            map.XData.Endianness = Endianness.Little;

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Create, FileAccess.Write))
            using (EndianBinaryWriter writer = new EndianBinaryWriter(stream))
                map.Write(writer);

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                map = new MapRtDL(reader);
        }

        [TestMethod]
        public void MapTest2Layer()
        {
            const string PATH = @"D:\Game Dumps\Kirby's Return to Dreamland\DATA\files\map\step\level1\stage1\Step01.dat";
            const string OUT_PATH = @"C:\Users\firubii\Documents\Dolphin Emulator\Load\Riivolution\riivolution\test_lvl\map\step\level1\stage1\Step01.dat";

            MapRtDL map;
            using (FileStream stream = new FileStream(PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                map = new MapRtDL(reader);

            CollisionTile[,] col = new CollisionTile[map.Collision[0].GetLength(0), map.Collision[0].GetLength(1)];
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    col[x, y] = new CollisionTile(LandGridShapeKind.Cube, LandGridProperty.None, 0, 0);
                }
            }
            map.Collision.Add(col);
            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Create, FileAccess.Write))
            using (EndianBinaryWriter writer = new EndianBinaryWriter(stream))
                map.Write(writer);
        }

        void PrintMapInfo(MapRtDL map)
        {
            /*
            if (map.Bosses.Count > 0)
            {
                for (int i = 0; i < map.Bosses.Count; i++)
                {
                    Console.WriteLine($"\t- Boss {i}:");
                    Console.WriteLine($"\t\t- Kind: {map.Bosses[i].Kind}");
                    Console.WriteLine($"\t\t- Variation: {map.Bosses[i].Variation}");
                    Console.WriteLine($"\t\t- Level: {map.Bosses[i].Level}");
                    Console.WriteLine($"\t\t- TerrainGroup: {map.Bosses[i].TerrainGroup}");
                    Console.WriteLine($"\t\t- Has Super Ability: {map.Bosses[i].HasSuperAbility}");
                    Console.WriteLine($"\t\t- X: {map.Bosses[i].X}");
                    Console.WriteLine($"\t\t- Y: {map.Bosses[i].Y}");
                    Console.WriteLine($"\t\t- Unknown: {map.Bosses[i].Unknown}");
                }
            }
            */
            /*
            if (map.Enemies.Count > 0)
            {
                for (int i = 0; i < map.Enemies.Count; i++)
                {
                    Console.WriteLine($"\t- Enemy {i}:");
                    Console.WriteLine($"\t\t- Kind: {map.Enemies[i].Kind}");
                    Console.WriteLine($"\t\t- Variation: {map.Enemies[i].Variation}");
                    Console.WriteLine($"\t\t- Level: {map.Enemies[i].Level}");
                    Console.WriteLine($"\t\t- Dir Type: {map.Enemies[i].Direction}");
                    Console.WriteLine($"\t\t- Another Dimension Size: {map.Enemies[i].AnotherDimensionSize}");
                    Console.WriteLine($"\t\t- Extra Mode Size: {map.Enemies[i].ExtraModeSize}");
                    Console.WriteLine($"\t\t- Terrain Group: {map.Enemies[i].TerrainGroup}");
                    Console.WriteLine($"\t\t- Has Super Ability: {map.Enemies[i].HasSuperAbility}");
                    Console.WriteLine($"\t\t- X: {map.Enemies[i].X}");
                    Console.WriteLine($"\t\t- Y: {map.Enemies[i].Y}");
                    Console.WriteLine($"\t\t- Is Normal Only: {map.Enemies[i].IsNormalOnly}");
                    Console.WriteLine($"\t\t- Is EX Only: {map.Enemies[i].IsEXOnly}");
                }
            }
            */
            if (map.DecorationObjects.Count > 0)
            {
                for (int l = 0; l < map.DecorationObjects.Count; l++)
                {
                    Console.WriteLine($"\t- Decor Object {l}:");
                    Console.WriteLine($"\t\t- Kind: " + map.DecorationObjects[l].Kind);
                    Console.WriteLine($"\t\t- X1: " + map.DecorationObjects[l].X1);
                    Console.WriteLine($"\t\t- Y1: " + map.DecorationObjects[l].Y1);
                    Console.WriteLine($"\t\t- X2: " + map.DecorationObjects[l].X2);
                    Console.WriteLine($"\t\t- Y2: " + map.DecorationObjects[l].Y2);
                    Console.WriteLine($"\t\t- Color 1: " + map.DecorationObjects[l].Color1);
                    Console.WriteLine($"\t\t- Color 2: " + map.DecorationObjects[l].Color2);
                    Console.WriteLine($"\t\t- Radius: " + map.DecorationObjects[l].Radius);
                    Console.WriteLine($"\t\t- Terrain Group: " + map.DecorationObjects[l].TerrainGroup);
                    Console.WriteLine($"\t\t- Param 1: " + map.DecorationObjects[l].Param1);
                    Console.WriteLine($"\t\t- Param 2: " + map.DecorationObjects[l].Param2);
                    Console.WriteLine($"\t\t- Param 3: " + map.DecorationObjects[l].Param3);
                    Console.WriteLine($"\t\t- Param 4: " + map.DecorationObjects[l].Param4);
                    Console.WriteLine($"\t\t- Param 5: " + map.DecorationObjects[l].Param5);
                    Console.WriteLine($"\t\t- Param 6: " + map.DecorationObjects[l].Param6);
                    Console.WriteLine($"\t\t- Param 7: " + map.DecorationObjects[l].Param7);
                    Console.WriteLine($"\t\t- Param 8: " + map.DecorationObjects[l].Param8);
                }
            }
            /*
            if (map.Type != MapRtDL.MapType.Normal)
            {
                Console.WriteLine($"{maps[i].Remove(0, PATH.Length)} is of type {map.Type}");
            }
            */
            /*
            if (map.CollisionMoveGroups.Count(x => x.IsValid) > 0)
            {
                Console.WriteLine(name);
                for (int j = 0; j < map.CollisionMoveGroups.Length; j++)
                {
                    var group = map.CollisionMoveGroups[j];
                    if (group.IsValid)
                    {
                        Console.WriteLine($"\t- Group {j}:");
                        Console.WriteLine($"\t\t- X: {group.X}");
                        Console.WriteLine($"\t\t- Y: {group.Y}");
                        Console.WriteLine($"\t\t- Width: {group.Width}");
                        Console.WriteLine($"\t\t- Height: {group.Height}");

                        if (group.Action.IsValid)
                        {
                            Console.WriteLine($"\t\t- Action:");
                            Console.WriteLine($"\t\t\t- Event: {group.Action.Event}");
                            Console.WriteLine($"\t\t\t- Param 1: {group.Action.Param1}");
                            Console.WriteLine($"\t\t\t- Param 2: {group.Action.Param2}");
                            Console.WriteLine($"\t\t\t- Start Immediately: {group.Action.StartImmediately}");

                            var events = group.Action.Events;
                            if (events.Count > 0)
                            {
                                for (int e = 0; e < events.Count; e++)
                                {
                                    Console.WriteLine($"\t\t\t- Event {e}:");
                                    Console.WriteLine($"\t\t\t\t- Direction: {events[e].Direction}");
                                    Console.WriteLine($"\t\t\t\t- Distance: {events[e].Distance}");
                                    Console.WriteLine($"\t\t\t\t- Delay: {events[e].Delay}");

                                    if (map.XData.Version[0] == 5)
                                        Console.WriteLine($"\t\t\t\t- DX Unknown 1: {events[e].DXUnknown1}");

                                    Console.WriteLine($"\t\t\t\t- Unknown1: {events[e].Unknown1}");

                                    if (map.XData.Version[0] == 5)
                                        Console.WriteLine($"\t\t\t\t- DX Unknown 2: {events[e].DXUnknown2}");

                                    Console.WriteLine($"\t\t\t\t- Time: {events[e].Time}");

                                    if (map.XData.Version[0] == 5)
                                        Console.WriteLine($"\t\t\t\t- DX Unknown 3: {events[e].DXUnknown3}");

                                    Console.WriteLine($"\t\t\t\t- Is End: {events[e].IsEnd}");
                                    Console.WriteLine($"\t\t\t\t- Unknown2: {events[e].Unknown2}");
                                    Console.WriteLine($"\t\t\t\t- Unknown3: {events[e].Unknown3}");
                                    Console.WriteLine($"\t\t\t\t- Unknown4: {events[e].Unknown4}");
                                    Console.WriteLine($"\t\t\t\t- Unknown5: {events[e].Unknown5}");
                                    Console.WriteLine($"\t\t\t\t- Unknown6: {events[e].Unknown6}");
                                    Console.WriteLine($"\t\t\t\t- Unknown7: {events[e].Unknown7}");
                                    Console.WriteLine($"\t\t\t\t- Unknown8: {events[e].Unknown8}");
                                    Console.WriteLine($"\t\t\t\t- Unknown9: {events[e].Unknown9}");
                                    Console.WriteLine($"\t\t\t\t- Unknown10: {events[e].Unknown10}");
                                    Console.WriteLine($"\t\t\t\t- Unknown11: {events[e].Unknown11}");
                                    Console.WriteLine($"\t\t\t\t- Unknown12: {events[e].Unknown12}");
                                    Console.WriteLine($"\t\t\t\t- Accel Type: {events[e].AccelType}");
                                    Console.WriteLine($"\t\t\t\t- Accel Time: {events[e].AccelTime}");
                                    Console.WriteLine($"\t\t\t\t- Unknown13: {events[e].Unknown13}");
                                }
                            }
                        }
                    }
                }
            }
            */
        }

        [TestMethod]
        public void CinemoTest()
        {
            const string PATH = @"D:\Game Dumps\Kirby's Return to Dream Land Deluxe\romfs\cinemoparam\Yy\Tps\HeroActionParam.cndbin";
            const string OUT_PATH = "HeroActionParam_outtest.cndbin";

            Cinemo cnd;
            using (FileStream stream = new FileStream(PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                cnd = new Cinemo(reader);

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Create, FileAccess.Write))
            using (EndianBinaryWriter writer = new EndianBinaryWriter(stream))
                cnd.Write(writer);

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                cnd = new Cinemo(reader);
        }

        [TestMethod]
        public void CinemoTestKSA()
        {
            const string PATH = @"D:\Game Dumps\Kirby Star Allies\romfs\map\Step\Level4\Stage1\Step01.cndbin";
            const string OUT_PATH = "KSA_L4_S1_S01.cndbin";

            CinemoKSA cnd;
            using (FileStream stream = new FileStream(PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                cnd = new CinemoKSA(reader);

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Create, FileAccess.Write))
            using (EndianBinaryWriter writer = new EndianBinaryWriter(stream))
                cnd.Write(writer);

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                cnd = new CinemoKSA(reader);
        }

        [TestMethod]
        public void FDGTestV2()
        {
            const string PATH = @"D:\Game Dumps\Kirby's Return to Dreamland\DATA\files\fdg\Archive.dat";
            const string OUT_PATH = "RTDL_FDG.dat";

            FDG fdg;
            using (FileStream stream = new FileStream(PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                fdg = new FDG(reader);

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Create, FileAccess.Write))
            using (EndianBinaryWriter writer = new EndianBinaryWriter(stream))
                fdg.Write(writer);

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                fdg = new FDG(reader);

            for (int i = 0; i < fdg.Scenes.Count; i++)
            {
                if (!fdg.SceneOrder.Contains(fdg.Scenes[i].Name))
                    Console.WriteLine($"{fdg.Scenes[i].Name} unreferenced in Scene Order");
            }

            for (int i = 0; i < fdg.SceneOrder.Count; i++)
            {
                Console.WriteLine(fdg.SceneOrder[i]);
            }

        }

        [TestMethod]
        public void FDGTestV3()
        {
            const string PATH = @"D:\Game Dumps\Kirby and the Forgotten Land\romfs\fdg\Archive.dat";
            const string OUT_PATH = "KatFL_FDG.dat";

            FDG fdg;
            using (FileStream stream = new FileStream(PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                fdg = new FDG(reader);

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Create, FileAccess.Write))
            using (EndianBinaryWriter writer = new EndianBinaryWriter(stream))
                fdg.Write(writer);

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                fdg = new FDG(reader);
        }

        [TestMethod]
        public void FilterTest()
        {
            const string PATH = @"D:\Game Dumps\Kirby and the Forgotten Land\romfs\msg\Kirby15\US_English\Filter.bin";
            const string OUT_PATH = "KatFL_Filter.bin";

            MsgFilter filter;
            using (FileStream stream = new FileStream(PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                filter = new MsgFilter(reader);

            for (int i = 0; i < filter.Filters.Count; i++)
            {
                Console.WriteLine($"- {filter.Filters[i].Font}");
                Console.WriteLine($"\t- {filter.Filters[i].Characters}");
            }

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Create, FileAccess.Write))
            using (EndianBinaryWriter writer = new EndianBinaryWriter(stream))
                filter.Write(writer);

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                filter = new MsgFilter(reader);

            Console.WriteLine("-------------");

            for (int i = 0; i < filter.Filters.Count; i++)
            {
                Console.WriteLine($"- {filter.Filters[i].Font}");
                Console.WriteLine($"\t- {filter.Filters[i].Characters}");
            }
        }

        [TestMethod]
        public void Map3DTest()
        {
            const string PATH = @"D:\Game Dumps\Kirby Star Allies\romfs\map3d\LvMap\LvMap3\LvMap3Bg\LvMap3\TopL.bin";
            const string OUT_PATH = "LvMap3_test.bin";

            Map3D map3d;
            using (FileStream stream = new FileStream(PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                map3d = new Map3D(reader);

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Create, FileAccess.Write))
            using (EndianBinaryWriter writer = new EndianBinaryWriter(stream))
                map3d.Write(writer);

            using (FileStream stream = new FileStream(OUT_PATH, FileMode.Open, FileAccess.Read))
            using (EndianBinaryReader reader = new EndianBinaryReader(stream))
                map3d = new Map3D(reader);
        }
    }
}