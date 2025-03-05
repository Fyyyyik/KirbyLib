using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Force.Crc32;

namespace KirbyLib.Crypto
{
    public static class Crc32C
    {
        private static uint Calculate(string str, bool bigEndian = false)
        {
            Crc32CAlgorithm crc = new Crc32CAlgorithm(bigEndian);
            return BitConverter.ToUInt32(crc.ComputeHash(Encoding.UTF8.GetBytes(str)));
        }

        /// <summary>
        /// Calculates an inverted Crc32C hash
        /// </summary>
        public static uint CalculateInv(string str, bool bigEndian = false)
        {
            return Calculate(str, bigEndian) ^ 0xFFFFFFFF;
        }
    }
}
