using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirbyLib.Mint
{
    public class Namespace
    {
        public int Unknown;
        public string Name;
        public int Modules;
        public int TotalModules;
        public int ChildNamespaces;
    }

    internal class NamespaceComparer : IComparer<Namespace>
    {
        readonly string CharSort = "!\"#$%&\\'()*+,.-/:;<=>?@[\\]^_`{|}~0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public int Compare(Namespace x, Namespace y)
        {
            string xName = x.Name;
            string yName = y.Name;
            if (xName == yName)
                return 0;

            for (int i = 0; i < xName.Length; i++)
            {
                if (i >= yName.Length)
                    return -1;

                if (xName[i] == yName[i])
                    continue;

                if (CharSort.IndexOf(xName[i]) < CharSort.IndexOf(yName[i]))
                    return -1;
                else
                    return 1;
            }

            return 0;
        }
    }
}
