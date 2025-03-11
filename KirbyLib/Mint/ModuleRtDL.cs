using KirbyLib.Crypto;
using KirbyLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirbyLib.Mint
{
    /// <summary>
    /// A representation of a Mint Module as found in version 0.2 Mint Archives.
    /// </summary>
    public class ModuleRtDL
    {
        public XData XData { get; private set; } = new XData();

        /// <summary>
        /// The name of the Module.
        /// </summary>
        public string Name;

        public ModuleFormat Format { get; } = ModuleFormat.RtDL;

        /// <summary>
        /// Raw bytes making up static constant data accessible by functions in the Module's Objects.
        /// </summary>
        public List<byte> SData = new List<byte>();
        /// <summary>
        /// External references.
        /// </summary>
        public List<string> XRef = new List<string>();
        /// <summary>
        /// A list of Objects in this Module.
        /// </summary>
        public List<MintObject> Objects = new List<MintObject>();

        public ModuleRtDL() { }

        public ModuleRtDL(EndianBinaryReader reader)
        {
            Read(reader);
        }

        public void Read(EndianBinaryReader reader)
        {
            XData.Read(reader);

            reader.ReadUInt32(); // Version number, always 0.2.0.0

            // Need this for later
            uint nameAddr = reader.ReadUInt32();
            reader.BaseStream.Position -= 4;

            Name = reader.ReadStringOffset();
            uint sdataAddr = reader.ReadUInt32();
            uint xrefAddr = reader.ReadUInt32();
            uint objectsAddr = reader.ReadUInt32();

            reader.BaseStream.Position = sdataAddr;
            SData = reader.ReadBytes(reader.ReadInt32()).ToList();

            reader.BaseStream.Position = xrefAddr;
            XRef = new List<string>();
            uint xrefCount = reader.ReadUInt32();
            for (int i = 0; i < xrefCount; i++)
                XRef.Add(reader.ReadStringOffset());

            reader.BaseStream.Position = objectsAddr;
            Objects = new List<MintObject>();
            uint objCount = reader.ReadUInt32();
            for (uint i = 0; i < objCount; i++)
            {
                reader.BaseStream.Position = objectsAddr + 4 + (i * 4);
                uint objAddr = reader.ReadUInt32();
                // Used for reading the final function with relative ease
                uint objEndAddr = i < objCount - 1 ? reader.ReadUInt32() : nameAddr;

                reader.BaseStream.Position = objAddr;

                MintObject obj = new MintObject();
                obj.Name = reader.ReadStringOffset();
                uint varAddr = reader.ReadUInt32();
                uint funcAddr = reader.ReadUInt32();

                reader.BaseStream.Position = varAddr;
                uint varCount = reader.ReadUInt32();
                for (int v = 0; v < varCount; v++)
                {
                    reader.BaseStream.Position = varAddr + 4 + (v * 4);
                    reader.BaseStream.Position = reader.ReadUInt32();

                    string name = reader.ReadStringOffset();
                    string type = reader.ReadStringOffset();
                    MintVariable variable = new MintVariable(type, name);

                    obj.Variables.Add(variable);
                }

                reader.BaseStream.Position = funcAddr;
                uint funcCount = reader.ReadUInt32();
                for (uint f = 0; f < funcCount; f++)
                {
                    reader.BaseStream.Position = funcAddr + 4 + (f * 4);
                    uint addr = reader.ReadUInt32();
                    // Get end address of function data block for very easy reading
                    uint endAddr = f < funcCount - 1 ? reader.ReadUInt32() : objEndAddr;
                    reader.BaseStream.Position = addr;

                    string name = reader.ReadStringOffset();
                    MintFunction func = new MintFunction(name);
                    uint dataAddr = reader.ReadUInt32();

                    reader.BaseStream.Position = dataAddr;
                    func.Data = reader.ReadBytes((int)(endAddr - dataAddr));

                    obj.Functions.Add(func);
                }

                Objects.Add(obj);
            }
        }

        public void Write(EndianBinaryWriter writer)
        {
            StringHelperContainer strings = new StringHelperContainer();

            XData.WriteHeader(writer);

            writer.Write(new byte[] { 0, 2, 0, 0 });

            strings.Add(writer.BaseStream.Position, Name);
            writer.Write(-1);

            long headerStart = writer.BaseStream.Position;
            writer.Write(-1);
            writer.Write(-1);
            writer.Write(-1);

            writer.WritePositionAt(headerStart);
            writer.Write(SData.Count);
            writer.Write(SData.ToArray());

            writer.WritePositionAt(headerStart + 4);
            writer.Write(XRef.Count);
            for (int i = 0; i < XRef.Count; i++)
            {
                strings.Add(writer.BaseStream.Position, XRef[i]);
                writer.Write(-1);
            }

            long objListStart = writer.BaseStream.Position;
            writer.WritePositionAt(headerStart + 8);
            writer.Write(Objects.Count);
            for (int i = 0; i < Objects.Count; i++)
                writer.Write(-1);

            for (int i = 0; i < Objects.Count; i++)
            {
                MintObject obj = Objects[i];

                long objPos = writer.BaseStream.Position;
                writer.WritePositionAt(objListStart + 4 + (i * 4));

                strings.Add(writer.BaseStream.Position, obj.Name);
                writer.Write(-1);
                writer.Write(-1);
                writer.Write(-1);

                long varListStart = writer.BaseStream.Position;
                writer.WritePositionAt(objPos + 4);
                writer.Write(obj.Variables.Count);
                for (int j = 0; j < obj.Variables.Count; j++)
                    writer.Write(-1);

                for (int j = 0; j < obj.Variables.Count; j++)
                {
                    MintVariable var = obj.Variables[j];
                    var.Name = var.Name.Trim();
                    var.Type = var.Type.Trim();

                    writer.WritePositionAt(varListStart + 4 + (j * 4));

                    strings.Add(writer.BaseStream.Position, var.Name);
                    writer.Write(-1);
                    strings.Add(writer.BaseStream.Position, var.Type);
                    writer.Write(-1);
                }

                long funcListStart = writer.BaseStream.Position;
                writer.WritePositionAt(objPos + 8);
                writer.Write(obj.Functions.Count);
                for (int j = 0; j < obj.Functions.Count; j++)
                    writer.Write(-1);

                for (int j = 0; j < obj.Functions.Count; j++)
                {
                    MintFunction func = obj.Functions[j];

                    writer.WritePositionAt(funcListStart + 4 + (j * 4));

                    strings.Add(writer.BaseStream.Position, func.Name);
                    writer.Write(-1);
                    long dataAddr = writer.BaseStream.Position;
                    writer.Write(-1);

                    writer.WritePositionAt(dataAddr);
                    writer.Write(func.Data);
                    writer.WritePadding();
                }
            }

            strings.WriteAll(writer);

            XData.WriteFilesize(writer);
            XData.WriteFooter(writer);
        }

        public bool ObjectExists(string name)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                if (Objects[i].Name == name)
                    return true;
            }

            return false;
        }

        public MintObject GetObject(string name)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                if (Objects[i].Name == name)
                    return Objects[i];
            }

            return null;
        }

        public MintObject this[int index] => Objects[index];

        public MintObject this[string name] => GetObject(name);
    }
}
