using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KirbyLib.Mint
{
    /// <summary>
    /// A representation of a function found in Mint Objects.
    /// </summary>
    public class MintFunction
    {
        /// <summary>
        /// The fully typed name of the function.
        /// </summary>
        public string Name;
        /// <summary>
        /// The amount of arguments the function has.<br/>
        /// Only used in Basil.
        /// </summary>
        public uint Arguments;
        /// <summary>
        /// The amount of registers the function has.<br/>
        /// Only used in Basil.
        /// </summary>
        public uint Registers;
        /// <summary>
        /// The raw data of instructions that make up the function.
        /// </summary>
        public byte[] Data;
        /// <summary>
        /// Bitflags that determine how the Mint VM treats the function.
        /// </summary>
        public uint Flags;

        /// <summary>
        /// Returns the function name without the return type.
        /// </summary>
        public string NameWithoutType() => MintUtil.TrimFunctionType(Name);

        /// <summary>
        /// Returns the function name without the return type or arguments.
        /// </summary>
        public string GetShortName() => MintUtil.TrimFunctionSymbols(Name);
    }
}
