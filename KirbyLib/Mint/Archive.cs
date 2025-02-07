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

            List<Namespace> readNamespaces = new List<Namespace>();
            for (int i = 0; i < namespaceCount; i++)
            {
                Namespace n = new Namespace();
                n.Name = reader.ReadStringOffset();
                n.Modules = reader.ReadInt32();
                n.TotalModules = reader.ReadInt32();
                n.ChildNamespaces = reader.ReadInt32();
                // HAL Labs you are so weird why do you do this?
                n.Unknown = reader.ReadIntOffset(-4) - 1;

                readNamespaces.Add(n);
            }

            Namespaces = new List<Namespace>();
            reader.BaseStream.Position = indexTable;
            for (int i = 0; i < namespaceCount; i++)
            {
                Namespaces.Add(readNamespaces[reader.ReadInt32() - 1]);
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

        protected Module ReadModule(EndianBinaryReader reader)
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
    }
}
