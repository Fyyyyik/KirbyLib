using KirbyLib.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirbyLib.Mint
{
    public static class MintUtil
    {
        public static uint CalculateHash(string typeName, string name)
        {
            if (!name.Contains('('))
                return BitConverter.ToUInt32(Crc32C.CalculateInv(typeName + "." + name), 0);

            // Search function signature to trim off the return type
            bool searchBackwards = false;
            int trimIdx = 0;
            for (int i = 0; i < name.Length && i >= 0;)
            {
                if (searchBackwards)
                {
                    if (name[i] == ' ')
                    {
                        trimIdx = i + 1;
                        break;
                    }
                }
                else
                {
                    if (name[i] == '(')
                        searchBackwards = true;
                }

                i += searchBackwards ? -1 : 1;
            }

            string str = typeName + "." + new string(name.Skip(trimIdx).ToArray());
            return BitConverter.ToUInt32(Crc32C.CalculateInv(str), 0);
        }
    }
}
