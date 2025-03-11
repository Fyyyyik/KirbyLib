using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirbyLib.Mint
{
    /// <summary>
    /// A representation of an enum name/value pair as found in Mint Objects.
    /// </summary>
    public class MintEnum
    {
        /// <summary>
        /// The name of the enum.
        /// </summary>
        public string Name;
        /// <summary>
        /// The value of the enum.
        /// </summary>
        public int Value;
        /// <summary>
        /// Bitflags that tell the Mint VM how to treat the enum.<br/>
        /// <b>Note:</b> Introduced in Basil 7.0.6.
        /// </summary>
        public uint Flags;

        public MintEnum(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public MintEnum(KeyValuePair<string, int> pair)
        {
            Name = pair.Key;
            Value = pair.Value;
        }
    }
}
