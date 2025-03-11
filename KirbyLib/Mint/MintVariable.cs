using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirbyLib.Mint
{
    /// <summary>
    /// A representation of variable information as found in Mint Objects.
    /// </summary>
    public class MintVariable
    {
        /// <summary>
        /// The name of the variable.
        /// </summary>
        public string Name;
        /// <summary>
        /// The type of the variable.<br/>
        /// Used by the game to know how much memory to allocate for the variable.
        /// </summary>
        public string Type;
        /// <summary>
        /// Bitflags that tell the Mint VM how to treat this variable.
        /// </summary>
        public uint Flags;

        public MintVariable(string type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}
