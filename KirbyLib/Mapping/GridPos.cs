using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirbyLib.Mapping
{
    /// <summary>
    /// A fixed-point representation of an object position within a map.<br/><br/>
    /// Its raw data is a 28-bit whole number and 4-bit decimal, as a 32-bit integer.
    /// </summary>
    public struct GridPos
    {
        private uint _value;

        /// <summary>
        /// Constructs a GridPos using a raw value as stored in level data.
        /// </summary>
        public GridPos(uint rawPos)
        {
            _value = rawPos;
        }

        /// <summary>
        /// Constructs a GridPos using a whole number and a given decimal value between 0 and 16.
        /// </summary>
        public GridPos(int whole, byte _decimal)
        {
            _value = (uint)(whole << 4) | (uint)(_decimal & 0xF);
        }

        /// <summary>
        /// Constructs a GridPos using a floating-point value.<br/>
        /// The value will be truncated to 16 possible decimal values.
        /// </summary>
        public GridPos(float pos)
        {
            uint wholePart = (uint)Math.Floor(pos);
            uint decimalPart = (uint)Math.Floor((pos - wholePart) * 16f);
            _value = (wholePart << 4) | decimalPart;
        }

        /// <summary>
        /// Constructs a GridPos using a 64-bit floating-point value.<br/>
        /// The value will be truncated to 16 possible decimal values.
        /// </summary>
        public GridPos(double pos)
        {
            uint wholePart = (uint)Math.Floor(pos);
            uint decimalPart = (uint)Math.Floor((pos - wholePart) * 16d);
            _value = (wholePart << 4) | decimalPart;
        }

        /// <summary>
        /// Creates a GridPos using a raw value as stored in level data.
        /// </summary>
        public static GridPos FromRaw(uint rawPos)
        {
            return new GridPos(rawPos);
        }

        /// <summary>
        /// Creates a GridPos using an integer value.<br/>
        /// The GridPos will have a decimal place of 0.
        /// </summary>
        public static GridPos FromInt(int pos)
        {
            return new GridPos(pos, 0);
        }

        /// <summary>
        /// Creates a GridPos using a floating-point value.<br/>
        /// The value will be truncated to 16 possible decimal values.
        /// </summary>
        public static GridPos FromFloat(float pos)
        {
            return new GridPos(pos);
        }

        /// <summary>
        /// Gets the whole number component of the fixed-point value.
        /// </summary>
        public uint GetWholeNumber()
        {
            return (_value & 0xFFFFFFF0) >> 4;
        }

        /// <summary>
        /// Gets the decimal component of the fixed-point value
        /// </summary>
        public byte GetDecimal()
        {
            return (byte)(_value & 0xF);
        }

        /// <summary>
        /// Gets the raw, unsigned integer value
        /// </summary>
        public uint GetRawValue()
        {
            return _value;
        }

        /// <summary>
        /// Gets the floating-point representation of the value
        /// </summary>
        public float AsFloat()
        {
            return _value / 16f;
        }

        /// <summary>
        /// Gets the 64-bit floating-point representation of the value
        /// </summary>
        public double AsDouble()
        {
            return _value / 16d;
        }

        /// <summary>
        /// Gets the decimal component as a floating-point value
        /// </summary>
        public float GetDecimalFloat()
        {
            return (_value & 0xF) / 16f;
        }

        /// <summary>
        /// Gets the decimal component as a 64-bit floating-point value
        /// </summary>
        public double GetDecimalDouble()
        {
            return (_value & 0xF) / 16d;
        }

        #region Casts

        public static implicit operator uint(GridPos m)
        {
            return m._value;
        }

        public static implicit operator GridPos(uint m)
        {
            return new GridPos(m);
        }

        public static implicit operator float(GridPos m)
        {
            return m.AsFloat();
        }

        public static implicit operator GridPos(float m)
        {
            return new GridPos(m);
        }

        public static implicit operator double(GridPos m)
        {
            return m.AsDouble();
        }

        public static implicit operator GridPos(double m)
        {
            return new GridPos(m);
        }

        #endregion

        #region Operators

        public static GridPos operator +(GridPos a, GridPos b)
        {
            return new GridPos(a._value + b._value);
        }

        public static GridPos operator -(GridPos a, GridPos b)
        {
            return new GridPos(a._value - b._value);
        }

        public static GridPos operator *(GridPos a, GridPos b)
        {
            return new GridPos(a.AsDouble() * b.AsDouble());
        }

        public static GridPos operator /(GridPos a, GridPos b)
        {
            return new GridPos(a.AsDouble() / b.AsDouble());
        }

        public static bool operator ==(GridPos a, GridPos b)
        {
            return a._value == b._value;
        }

        public static bool operator !=(GridPos a, GridPos b)
        {
            return a._value != b._value;
        }

        public static bool operator >(GridPos a, GridPos b)
        {
            return a._value > b._value;
        }

        public static bool operator <(GridPos a, GridPos b)
        {
            return a._value < b._value;
        }

        public static bool operator >=(GridPos a, GridPos b)
        {
            return a._value >= b._value;
        }

        public static bool operator <=(GridPos a, GridPos b)
        {
            return a._value <= b._value;
        }

        #endregion

        public override string ToString()
        {
            return AsDouble().ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is GridPos)
                return (GridPos)obj == this;

            return false;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}
