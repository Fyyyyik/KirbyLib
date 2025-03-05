using KirbyLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirbyLib.Mint
{
    public class Archive
    {
        public XData XData { get; private set; } = new XData();

        public byte[] Version = new byte[4];

        public List<Namespace> Namespaces = new List<Namespace>();
        public List<Module> Modules = new List<Module>();

        public Archive(byte[] version)
        {
            if (version.Length != 4)
                throw new InvalidDataException("Mint Archive version must be 4 bytes long.");

            Version = version;
        }

        public Archive(EndianBinaryReader reader)
        {
            Read(reader);
        }

        public void Read(EndianBinaryReader reader)
        {
            XData.Read(reader);

            long header = reader.BaseStream.Position;

            Version = reader.ReadBytes(4);
            // For some reason, all indicies related to namespaces are 1 more than they should be
            int namespaceCount = reader.ReadInt32() - 1;
            uint headerLength = reader.ReadUInt32(); // Always 0x24
            uint moduleCount = reader.ReadUInt32();
            uint moduleAddr = reader.ReadUInt32();
            uint endOfDataAddr = reader.ReadUInt32();
            reader.ReadUInt64();
            int rootNamespaces = reader.ReadInt32();

            reader.BaseStream.Position = header + headerLength;
            uint indexTable = reader.ReadUInt32();

            long namespaceListAddr = reader.BaseStream.Position;
            reader.BaseStream.Position = indexTable;
            Namespaces = new List<Namespace>();
            for (int i = 0; i < namespaceCount; i++)
            {
                reader.BaseStream.Position = indexTable + (i * 4);
                int index = reader.ReadInt32() - 1;

                reader.BaseStream.Position = namespaceListAddr + (index * 0x14);

                Namespace n = new Namespace();
                n.Name = reader.ReadStringOffset();
                n.Modules = reader.ReadInt32();
                n.TotalModules = reader.ReadInt32();
                n.ChildNamespaces = reader.ReadInt32();
                // HAL Labs you are so weird why do you do this?
                n.Unknown = reader.ReadIntOffset(-4) - 1;

                Namespaces.Add(n);
            }

            Modules = new List<Module>();
            for (uint i = 0; i < moduleCount; i++)
            {
                reader.BaseStream.Position = moduleAddr + (i * 8);
                
                string moduleName = reader.ReadStringOffset();

                reader.BaseStream.Position = reader.ReadUInt32();

                Module module = ReadModule(reader);
                if (module.Name != moduleName)
                    Console.WriteLine($"Warning: Module {i} name does not match! Archive: \"{moduleName}\", Module: \"{module.Name}\"");

                Modules.Add(module);
            }

            reader.BaseStream.Position = endOfDataAddr;
            //Console.WriteLine("Unknown 1: " + reader.ReadUInt32());
            //Console.WriteLine("Unknown 2: " + reader.ReadUInt32());
        }

        public void Write(EndianBinaryWriter writer)
        {
            StringHelperContainer strings = new StringHelperContainer();

            /*
            // Not doing this until I know everything about that stupid unknown value because everything other than that is very easy to calculate
            
            // Auto-generate namespace list based on module names
            List<string> namespaceList = new List<string>();
            for (int i = 0; i < Modules.Count; i++)
            {
                string[] modNameSplit = Modules[i].Name.Split('.');
                for (int s = 0; s < modNameSplit.Length - 1; s++)
                {
                    string nSpace = string.Join(".", modNameSplit.Take(s + 1));
                    if (!namespaceList.Contains(nSpace))
                        namespaceList.Add(nSpace);
                }
            }
            namespaceList = namespaceList.OrderBy(x => x.ToLower()).OrderBy(x => x.Count(x => x == '.')).ToList();
            */

            XData.WriteHeader(writer);

            writer.Write(Version);

            long header = writer.BaseStream.Position;
            writer.Write(Namespaces.Count + 1);
            writer.Write(0x24);
            writer.Write(Modules.Count);
            writer.Write(-1);
            writer.Write(-1);
            writer.Write(Version[0] >= 7 ? 1 : 0);
            writer.Write(0);
            writer.Write(Namespaces.Count(x => !x.Name.Contains('.')));

            long indexTableStart = writer.BaseStream.Position + (Namespaces.Count * 0x14) + 4;
            writer.Write((uint)indexTableStart);
            long namespaceListStart = writer.BaseStream.Position;

            var writeOrder = Namespaces.Select(x => x.Name).ToList();
            writeOrder.Sort(StringComparer.Ordinal);

            int moduleCount = Modules.Count(x => !x.Name.Contains('.'));
            for (int i = 0; i < writeOrder.Count; i++)
            {
                var nSpace = Namespaces.First(x => x.Name == writeOrder[i]);
                strings.Add(writer.BaseStream.Position, nSpace.Name);
                writer.Write(-1);
                int modules = Modules.Count(x => x.Name != nSpace.Name && x.Name.StartsWith(nSpace.Name + ".") && !x.Name.Remove(0, nSpace.Name.Length + 1).Contains('.'));
                writer.Write(modules);
                writer.Write(moduleCount);
                moduleCount += modules;
                writer.Write(Namespaces.Count(x => x.Name != nSpace.Name && x.Name.StartsWith(nSpace.Name + ".") && !x.Name.Remove(0, nSpace.Name.Length + 1).Contains('.')));

                writer.Write((int)indexTableStart + 4);
            }

            for (int i = 0; i < Namespaces.Count; i++)
            {
                int index = writeOrder.IndexOf(Namespaces[i].Name);
                writer.Write(index + 1);
                /*
                for (int n = 0; n < writeOrder.Count; n++)
                {
                    if (Namespaces.First(x => x.Name == writeOrder[n]).Unknown == index)
                        writer.WritePositionAt(namespaceListStart + (n * 0x14) + 0x10);
                }
                */
            }

            writer.WritePositionAt(header + 0xC);

            long moduleAddr = writer.BaseStream.Position;
            for (int i = 0; i < Modules.Count; i++)
            {
                strings.Add(writer.BaseStream.Position, Modules[i].Name);
                writer.Write(-1);
                writer.Write(-1);
            }

            for (int i = 0; i < Modules.Count; i++)
            {
                writer.WritePositionAt(moduleAddr + (i * 8) + 4);
                using (MemoryStream stream = new MemoryStream())
                {
                    using (EndianBinaryWriter moduleWriter = new EndianBinaryWriter(stream))
                        Modules[i].Write(moduleWriter);

                    writer.Write(stream.ToArray());
                }
            }

            writer.WritePositionAt(header + 0x10);
            writer.Write(0);
            writer.Write(0);
            strings.WriteAll(writer);

            XData.WriteFilesize(writer);
            XData.WriteFooter(writer);
        }

        public bool NamespaceExists(string name)
        {
            for (int i = 0; i < Namespaces.Count; i++)
            {
                if (Namespaces[i].Name == name)
                    return true;
            }

            return false;
        }

        public Namespace GetNamespace(string name)
        {
            for (int i = 0; i < Namespaces.Count; i++)
            {
                if (Namespaces[i].Name == name)
                    return Namespaces[i];
            }

            return null;
        }

        public bool ModuleExists(string name)
        {
            for (int i = 0; i < Modules.Count; i++)
            {
                if (Modules[i].Name == name)
                    return true;
            }

            return false;
        }

        public Module GetModule(string name)
        {
            for (int i = 0; i < Modules.Count; i++)
            {
                if (Modules[i].Name == name)
                    return Modules[i];
            }

            return null;
        }

        public string GetVersionString() => string.Join('.', Version);

        private Module ReadModule(EndianBinaryReader reader)
        {
            byte[] rawModule = XData.ExtractFile(reader);

            Module module = new Module();
            if (Version[0] < 2 && Version[1] < 1)
                module.Format = ModuleFormat.MintOld;
            else if (Version[0] >= 7)
                module.Format = ModuleFormat.Basil;

            using (MemoryStream stream = new MemoryStream(rawModule))
            using (EndianBinaryReader moduleReader = new EndianBinaryReader(stream))
                module.Read(moduleReader);

            return module;
        }

        public Module this[int index] => Modules[index];

        public Module this[string name] => GetModule(name);
    }
}
