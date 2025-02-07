using KirbyLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirbyLib.Mint
{
    public class Module
    {
        public XData XData { get; private set; } = new XData();

        public string Name;

        public ModuleFormat Format { get; internal set; } = ModuleFormat.Mint;

        public List<byte> SData = new List<byte>();
        public List<uint> XRef = new List<uint>();
        public List<ObjectType> ObjectTypes = new List<ObjectType>();
        public List<uint> Implements = new List<uint>();

        public Module() { }

        public Module(EndianBinaryReader reader)
        {
            Read(reader);
        }

        public void Read(EndianBinaryReader reader)
        {
            XData.Read(reader);

            Name = reader.ReadStringOffset();
            if (Format >= ModuleFormat.Basil)
                reader.ReadUInt32(); // Hash for the Module, introduced in Basil

            uint sdataAddr = reader.ReadUInt32();
            uint xrefAddr = reader.ReadUInt32();
            uint objTypesAddr = reader.ReadUInt32();

            reader.BaseStream.Position = sdataAddr;
            SData = reader.ReadBytes(reader.ReadInt32()).ToList();

            reader.BaseStream.Position = xrefAddr;
            XRef = new List<uint>();
            uint xrefCount = reader.ReadUInt32();
            for (int i = 0; i < xrefCount; i++)
                XRef.Add(reader.ReadUInt32());

            reader.BaseStream.Position = objTypesAddr;
            ObjectTypes = new List<ObjectType>();
            uint objTypeCount = reader.ReadUInt32();
            for (uint i = 0; i < objTypeCount; i++)
            {
                reader.BaseStream.Position = objTypesAddr + 4 + (i * 4);
                reader.BaseStream.Position = reader.ReadUInt32();

                ObjectType obj = new ObjectType();
                obj.Name = reader.ReadStringOffset();
                reader.ReadUInt32(); // Inverted CRC32-C Object name hash, don't really need this because we can just calculate it whenever
                uint varAddr = reader.ReadUInt32();
                uint funcAddr = reader.ReadUInt32();
                uint enumAddr = reader.ReadUInt32();

                uint implAddr = 0;
                uint extAddr = 0;
                if (Format >= ModuleFormat.Mint)
                    implAddr = reader.ReadUInt32();
                if (Format >= ModuleFormat.Basil)
                    extAddr = reader.ReadUInt32();
                obj.Flags = reader.ReadUInt32();

                reader.BaseStream.Position = varAddr;
                uint varCount = reader.ReadUInt32();
                for (int v = 0; v < varCount; v++)
                {
                    reader.BaseStream.Position = varAddr + 4 + (v * 4);
                    reader.BaseStream.Position = reader.ReadUInt32();

                    Variable variable = new Variable();
                    variable.Name = reader.ReadStringOffset();
                    reader.ReadUInt32(); // Inverted CRC32-C variable name hash, can be calculated whenever
                    variable.Type = reader.ReadStringOffset();
                    variable.Flags = reader.ReadUInt32();

                    obj.Variables.Add(variable);
                }

                reader.BaseStream.Position = funcAddr;
                uint funcCount = reader.ReadUInt32();
                for (uint f = 0; f < funcCount; f++)
                {
                    reader.BaseStream.Position = funcAddr + 4 + (f * 4);
                    uint addr = reader.ReadUInt32();
                    // Get end address of function data block for very easy reading
                    uint endAddr = f < funcCount - 1 ? reader.ReadUInt32() : enumAddr;
                    reader.BaseStream.Position = addr;

                    Function func = new Function();
                    func.Name = reader.ReadStringOffset();
                    reader.ReadUInt32(); // Inverted CRC32-C function name hash, can just be calculated whenever so again we don't really need it
                    if (Format >= ModuleFormat.Basil)
                    {
                        func.Arguments = reader.ReadUInt32();
                        func.Registers = reader.ReadUInt32();
                    }

                    uint dataAddr = reader.ReadUInt32();
                    if (Format >= ModuleFormat.Mint)
                        func.Flags = reader.ReadUInt32();

                    reader.BaseStream.Position = dataAddr;
                    func.Data = reader.ReadBytes((int)(endAddr - dataAddr));

                    obj.Functions.Add(func);
                }

                reader.BaseStream.Position = enumAddr;
                uint enumCount = reader.ReadUInt32();
                for (uint e = 0; e < enumCount; e++)
                {
                    reader.BaseStream.Position = enumAddr + 4 + (e * 4);
                    reader.BaseStream.Position = reader.ReadUInt32();

                    MintEnum mintEnum = new MintEnum();
                    mintEnum.Name = reader.ReadStringOffset();
                    mintEnum.Value = reader.ReadInt32();

                    obj.Enums.Add(mintEnum);
                }

                if (Format >= ModuleFormat.Mint)
                {
                    reader.BaseStream.Position = implAddr;
                    int implCount = reader.ReadInt32();
                    for (int m = 0; m < implCount; m++)
                    {
                        Implements.Add(XRef[reader.ReadUInt16()]);
                    }
                }

                if (Format >= ModuleFormat.Basil)
                {
                    reader.BaseStream.Position = extAddr;
                    int extCount = reader.ReadInt32();
                    for (int e = 0; e < extCount; e++)
                    {

                    }
                }

                ObjectTypes.Add(obj);
            }
        }

        public bool ObjectTypeExists(string name)
        {
            for (int i = 0; i < ObjectTypes.Count; i++)
            {
                if (ObjectTypes[i].Name == name)
                    return true;
            }

            return false;
        }

        public ObjectType GetObjectType(string name)
        {
            for (int i = 0; i < ObjectTypes.Count; i++)
            {
                if (ObjectTypes[i].Name == name)
                    return ObjectTypes[i];
            }

            return null;
        }
    }
}
