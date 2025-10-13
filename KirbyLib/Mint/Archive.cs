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
            uint namespaceCount = reader.ReadUInt32();
            uint namespaceAddr = reader.ReadUInt32();
            uint moduleCount = reader.ReadUInt32();
            uint moduleAddr = reader.ReadUInt32();

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
        }

        public void Write(EndianBinaryWriter writer)
        {
            StringHelperContainer strings = new StringHelperContainer();

            // Auto-generate namespace list based on module names
            List<string> namespaces = new List<string>();
            // Blank namespace to handle orphan modules
            namespaces.Add("");
            for (int i = 0; i < Modules.Count; i++)
            {
                string[] modNameSplit = Modules[i].Name.Split('.');
                for (int s = 0; s < modNameSplit.Length - 1; s++)
                {
                    string nSpace = string.Join(".", modNameSplit.Take(s + 1));
                    if (!namespaces.Contains(nSpace))
                        namespaces.Add(nSpace);
                }
            }
            namespaces.Sort(StringComparer.Ordinal);

            XData.WriteHeader(writer);

            writer.Write(Version);

            long header = writer.BaseStream.Position;
            writer.Write(namespaces.Count);
            writer.Write(-1);
            writer.Write(Modules.Count);
            writer.Write(-1);

            long namespaceListStart = writer.BaseStream.Position;
            writer.WritePositionAt(header + 0x4);

            // Pad out the namespace section to streamline writing children lists
            for (int i = 0; i < namespaces.Count; i++)
            {
                writer.Write(-1);
                writer.Write(-1);
                writer.Write(-1);
                writer.Write(-1);
                writer.Write(-1);
            }

            int moduleCount = 0;
            for (int i = 0; i < namespaces.Count; i++)
            {
                /*
                 * Namespace format goes as follows:
                 * 0x0  -- Name
                 * 0x4  -- Module count
                 * 0x8  -- Total module count
                 * 0x10 -- Child namespace count
                 * 0x14 -- Offset to child namespace index list
                 */

                writer.BaseStream.Position = namespaceListStart + (i * 0x14);

                var nSpace = namespaces[i];
                strings.Add(writer.BaseStream.Position, nSpace);
                writer.Write(-1);
                // This is an incredibly janky one-line solution but it works
                int modules = Modules.Count(x => x.Name != nSpace && (x.Name.StartsWith(nSpace + ".") || nSpace.Length == 0) && !x.Name.Remove(0, nSpace.Length + 1).Contains('.'));
                writer.Write(modules);
                writer.Write(moduleCount);
                moduleCount += modules;

                // This is also an incredibly janky one-line solution but it works as well
                var childNamespaces = namespaces.Where(x => x != nSpace && (x.StartsWith(nSpace + ".") || nSpace.Length == 0) && !x.Remove(0, nSpace.Length + 1).Contains('.'));
                writer.Write(childNamespaces.Count());

                writer.BaseStream.Seek(0, SeekOrigin.End);
                writer.WritePositionAt(namespaceListStart + (i * 0x14) + 0x10);
                for (int c = 0; c < childNamespaces.Count(); c++)
                    writer.Write(namespaces.IndexOf(childNamespaces.ElementAt(c)));
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
                    {
                        Modules[i].XData.Version = XData.Version;
                        Modules[i].XData.Endianness = XData.Endianness;

                        Modules[i].Format = GetModuleFormat();
                        Modules[i].Write(moduleWriter);
                    }

                    writer.Write(stream.ToArray());
                }
            }

            writer.WritePositionAt(header + 0x10);
            strings.WriteAll(writer);

            XData.WriteFilesize(writer);
            XData.WriteFooter(writer);
        }

        /// <summary>
        /// Returns true if the given Module exists in the archive.
        /// </summary>
        public bool ModuleExists(string name)
        {
            for (int i = 0; i < Modules.Count; i++)
            {
                if (Modules[i].Name == name)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a given Module from the archive.<br/>
        /// If it doesn't exist, null is returned.
        /// </summary>
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
            module.Format = GetModuleFormat();

            using (MemoryStream stream = new MemoryStream(rawModule))
            using (EndianBinaryReader moduleReader = new EndianBinaryReader(stream))
                module.Read(moduleReader);

            return module;
        }

        public ModuleFormat GetModuleFormat()
        {
            ModuleFormat fmt = ModuleFormat.Mint;

            if (Version[0] < 2 && Version[1] < 1)
                fmt = ModuleFormat.MintOld;
            else if (Version[0] >= 7 && Version[2] < 6)
                fmt = ModuleFormat.BasilKatFL;
            else if (Version[0] >= 7)
                fmt = ModuleFormat.Basil;

            return fmt;
        }

        public Module this[int index] => Modules[index];

        public Module this[string name] => GetModule(name);
    }
}
