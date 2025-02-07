using KirbyLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirbyLib.Mint
{
    public class ModuleRtDL
    {
        public XData XData { get; private set; } = new XData();

        public string Name;

        public ModuleFormat Format { get; } = ModuleFormat.MintOld;

        public List<byte> SData = new List<byte>();
        public List<string> XRef = new List<string>();
        public List<ObjectType> ObjectTypes = new List<ObjectType>();

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
            uint objTypesAddr = reader.ReadUInt32();

            reader.BaseStream.Position = sdataAddr;
            SData = reader.ReadBytes(reader.ReadInt32()).ToList();

            reader.BaseStream.Position = xrefAddr;
            XRef = new List<string>();
            uint xrefCount = reader.ReadUInt32();
            for (int i = 0; i < xrefCount; i++)
                XRef.Add(reader.ReadStringOffset());

            reader.BaseStream.Position = objTypesAddr;
            ObjectTypes = new List<ObjectType>();
            uint objTypeCount = reader.ReadUInt32();
            for (uint i = 0; i < objTypeCount; i++)
            {
                reader.BaseStream.Position = objTypesAddr + 4 + (i * 4);
                uint objAddr = reader.ReadUInt32();
                // Used for reading the final function with relative ease
                uint objEndAddr = i < objTypeCount - 1 ? reader.ReadUInt32() : nameAddr;

                reader.BaseStream.Position = objAddr;

                ObjectType obj = new ObjectType();
                obj.Name = reader.ReadStringOffset();
                uint varAddr = reader.ReadUInt32();
                uint funcAddr = reader.ReadUInt32();

                reader.BaseStream.Position = varAddr;
                uint varCount = reader.ReadUInt32();
                for (int v = 0; v < varCount; v++)
                {
                    reader.BaseStream.Position = varAddr + 4 + (v * 4);
                    reader.BaseStream.Position = reader.ReadUInt32();

                    Variable variable = new Variable();
                    variable.Name = reader.ReadStringOffset();
                    variable.Type = reader.ReadStringOffset();

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

                    Function func = new Function();
                    func.Name = reader.ReadStringOffset();
                    uint dataAddr = reader.ReadUInt32();

                    reader.BaseStream.Position = dataAddr;
                    func.Data = reader.ReadBytes((int)(endAddr - dataAddr));

                    obj.Functions.Add(func);
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
