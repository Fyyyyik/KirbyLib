using KirbyLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirbyLib.Mint
{
    /// <summary>
    /// An older Mint Archive version, for Mint 0.2.<br/>
    /// Found in:
    /// <list type="bullet">
    ///     <item>Kirby's Return to Dream Land</item>
    ///     <item>Kirby's Dream Collection</item>
    /// </list>
    /// </summary>
    public class ArchiveRtDL
    {
        public XData XData { get; private set; } = new XData();

        public List<ModuleRtDL> Modules = new List<ModuleRtDL>();

        public ArchiveRtDL() { }

        public ArchiveRtDL(EndianBinaryReader reader)
        {
            Read(reader);
        }

        public void Read(EndianBinaryReader reader)
        {
            XData.Read(reader);

            long moduleAddr = reader.BaseStream.Position;
            uint moduleCount = reader.ReadUInt32();

            Modules = new List<ModuleRtDL>();
            for (uint i = 0; i < moduleCount; i++)
            {
                reader.BaseStream.Position = moduleAddr + 4 + (i * 8);

                string moduleName = reader.ReadStringOffset();

                reader.BaseStream.Position = reader.ReadUInt32();

                ModuleRtDL module = ReadModule(reader);
                if (module.Name != moduleName)
                    Console.WriteLine($"Warning: Module {i} name does not match! Archive: \"{moduleName}\", Module: \"{module.Name}\"");

                Modules.Add(module);
            }
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
        public ModuleRtDL GetModule(string name)
        {
            for (int i = 0; i < Modules.Count; i++)
            {
                if (Modules[i].Name == name)
                    return Modules[i];
            }

            return null;
        }

        public string GetVersionString() => "0.2.0.0";

        protected ModuleRtDL ReadModule(EndianBinaryReader reader)
        {
            byte[] rawModule = XData.ExtractFile(reader);

            ModuleRtDL module = new ModuleRtDL();

            using (MemoryStream stream = new MemoryStream(rawModule))
            using (EndianBinaryReader moduleReader = new EndianBinaryReader(stream))
                module.Read(moduleReader);

            return module;
        }
    }
}
