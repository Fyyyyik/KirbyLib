using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirbyLib
{
    internal class ArrayEqualityComparer<T> : IEqualityComparer<T[]>
    {
        bool IEqualityComparer<T[]>.Equals(T[]? x, T[]? y)
        {
            if (x == null || y == null
                || x.Length != y.Length)
                return false;

            for (int i = 0; i < x.Length; i++)
            {
                if (!x[i]!.Equals(y[i]))
                    return false;
            }

            return true;
        }

        public int GetHashCode([DisallowNull] T[] obj)
        {
            return obj.GetHashCode();
        }
    }
}
