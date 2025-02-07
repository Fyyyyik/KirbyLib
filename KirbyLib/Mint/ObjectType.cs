using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirbyLib.Mint
{
    public class ObjectType
    {
        public string Name;
        public List<Variable> Variables = new List<Variable>();
        public List<Function> Functions = new List<Function>();
        public List<MintEnum> Enums = new List<MintEnum>();
        public uint Flags;

        public bool VariableExists(string name)
        {
            for (int i = 0; i < Variables.Count; i++)
            {
                if (Variables[i].Name == name)
                    return true;
            }

            return false;
        }

        public Variable GetVariable(string name)
        {
            for (int i = 0; i < Variables.Count; i++)
            {
                if (Variables[i].Name == name)
                    return Variables[i];
            }

            return null;
        }

        public bool FunctionExists(string name)
        {
            for (int i = 0; i < Functions.Count; i++)
            {
                if (Functions[i].Name == name)
                    return true;
            }

            return false;
        }

        public Function GetFunction(string name)
        {
            for (int i = 0; i < Functions.Count; i++)
            {
                if (Functions[i].Name == name)
                    return Functions[i];
            }

            return null;
        }

        public bool EnumExists(string name)
        {
            for (int i = 0; i < Enums.Count; i++)
            {
                if (Enums[i].Name == name)
                    return true;
            }

            return false;
        }

        public string GetEnumName(uint value)
        {
            for (int i = 0; i < Enums.Count; i++)
            {
                if (Enums[i].Value == value)
                    return Enums[i].Name;
            }

            return null;
        }

        public int GetEnumValue(string name)
        {
            for (int i = 0; i < Enums.Count; i++)
            {
                if (Enums[i].Name == name)
                    return Enums[i].Value;
            }

            return 0;
        }

        public string[] GetEnumNames()
        {
            string[] names = new string[Enums.Count];
            for (int i = 0; i < names.Length; i++)
                names[i] = Enums[i].Name;

            return names;
        }

        public int[] GetEnumValues()
        {
            int[] values = new int[Enums.Count];
            for (int i = 0; i < values.Length; i++)
                values[i] = Enums[i].Value;

            return values;
        }
    }
}
