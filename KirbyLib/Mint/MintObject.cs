using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirbyLib.Mint
{
    /// <summary>
    /// A representation of a Mint Object as contained in a Mint Archive.
    /// </summary>
    public class MintObject
    {
        /// <summary>
        /// The name of the object.
        /// </summary>
        public string Name;
        /// <summary>
        /// A list of variables present in the object.
        /// </summary>
        public List<MintVariable> Variables = new List<MintVariable>();
        /// <summary>
        /// A list of functions present in the object.
        /// </summary>
        public List<MintFunction> Functions = new List<MintFunction>();
        /// <summary>
        /// A list of enum values present in the object.<br/>
        /// <b>Note:</b> Introduced in Mint 1.0.5.
        /// </summary>
        public List<MintEnum> Enums = new List<MintEnum>();
        /// <summary>
        /// A list of object types (hashes) this object implements.<br/>
        /// <b>Note:</b> Introduced in Mint 1.1.3.
        /// </summary>
        public List<uint> Implements = new List<uint>();
        /// <summary>
        /// A list of extra instructions that inform the Basil VM which types this object extends (such as type parameters).<br/>
        /// <b>Note:</b> Introduced in Basil 7.0.2.
        /// </summary>
        public List<byte[]> Extends = new List<byte[]>();
        /// <summary>
        /// Bitflags that tell the Mint VM how to treat this object.
        /// </summary>
        public uint Flags;

        /// <summary>
        /// Returns true if the given variable exists.
        /// </summary>
        public bool VariableExists(string name)
        {
            for (int i = 0; i < Variables.Count; i++)
            {
                if (Variables[i].Name == name)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a variable by its name. Returns null if it does not exist.
        /// </summary>
        public MintVariable GetVariable(string name)
        {
            for (int i = 0; i < Variables.Count; i++)
            {
                if (Variables[i].Name == name)
                    return Variables[i];
            }

            return null;
        }

        /// <summary>
        /// Returns true if the given function exists, by its fully typed name (i.e. "void procAnim()")
        /// </summary>
        public bool FunctionExists(string name)
        {
            for (int i = 0; i < Functions.Count; i++)
            {
                if (Functions[i].Name == name)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the given function exists, by its short name (i.e. "procAnim")
        /// </summary>
        public bool FunctionExistsByShortName(string name)
        {
            for (int i = 0; i < Functions.Count; i++)
            {
                if (Functions[i].GetShortName() == name)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a function by its fully typed name (i.e. "void procAnim()"). Returns null if it does not exist.
        /// </summary>
        public MintFunction GetFunction(string name)
        {
            for (int i = 0; i < Functions.Count; i++)
            {
                if (Functions[i].Name == name)
                    return Functions[i];
            }

            return null;
        }

        /// <summary>
        /// Gets a function by its short name (i.e. "procAnim"). Returns null if it does not exist.
        /// </summary>
        public MintFunction GetFunctionByShortName(string name)
        {
            for (int i = 0; i < Functions.Count; i++)
            {
                if (Functions[i].GetShortName() == name)
                    return Functions[i];
            }

            return null;
        }

        /// <summary>
        /// Returns true if the given enum exists.
        /// </summary>
        public bool EnumExists(string name)
        {
            for (int i = 0; i < Enums.Count; i++)
            {
                if (Enums[i].Name == name)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets an enum's name by its value. Returns null if it does not exist.
        /// </summary>
        public string GetEnumName(uint value)
        {
            for (int i = 0; i < Enums.Count; i++)
            {
                if (Enums[i].Value == value)
                    return Enums[i].Name;
            }

            return null;
        }

        /// <summary>
        /// Gets an enum's value by its name. Returns 0 if it does not exist.
        /// </summary>
        public int GetEnumValue(string name)
        {
            for (int i = 0; i < Enums.Count; i++)
            {
                if (Enums[i].Name == name)
                    return Enums[i].Value;
            }

            return 0;
        }

        /// <summary>
        /// Returns a list of the object's enum names.
        /// </summary>
        public string[] GetEnumNames()
        {
            string[] names = new string[Enums.Count];
            for (int i = 0; i < names.Length; i++)
                names[i] = Enums[i].Name;

            return names;
        }

        /// <summary>
        /// Returns a list of the object's enum values.
        /// </summary>
        public int[] GetEnumValues()
        {
            int[] values = new int[Enums.Count];
            for (int i = 0; i < values.Length; i++)
                values[i] = Enums[i].Value;

            return values;
        }

        /// <summary>
        /// Returns the object's enums as a dictionary.
        /// </summary>
        public Dictionary<string, int> GetEnumDictionary()
        {
            Dictionary<string, int> e = new Dictionary<string, int>();
            for (int i = 0; i < Enums.Count; i++)
                e.Add(Enums[i].Name, Enums[i].Value);

            return e;
        }

        /// <summary>
        /// Sets the object's enums with a dictionary.
        /// </summary>
        public void SetEnumsWithDictionary(Dictionary<string, int> dict)
        {
            Enums = new List<MintEnum>();
            foreach (var pair in dict)
                Enums.Add(new MintEnum(pair));
        }
    }
}
