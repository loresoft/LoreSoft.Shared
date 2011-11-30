using System;
using System.Numerics;
using LoreSoft.Shared.Text;

namespace LoreSoft.Shared
{
    /// <summary>
    /// Represents a globally unique identifier (GUID) with a shorter string value.
    /// </summary>
    public struct ShortGuid : IComparable, IComparable<Guid>, IEquatable<Guid>, IComparable<ShortGuid>, IEquatable<ShortGuid>
    {
        /// <summary>
        /// A read-only instance of the <see cref="ShortGuid"/> structure whose value is all zeros.
        /// </summary>
        public static readonly ShortGuid Empty = new ShortGuid(Guid.Empty);

        /// <summary>
        /// Creates a <see cref="ShortGuid"/> from an encoded string.
        /// </summary>
        /// <param name="value">The encoded <see cref="ShortGuid"/> as a string.</param>
        public ShortGuid(string value)
        {
            _encodedValue = value;
            _guid = Decode(value);
        }

        /// <summary>
        /// Creates a ShortGuid from a <see cref="Guid"/>.
        /// </summary>
        /// <param name="guid">The <see cref="Guid"/> to encode</param>
        public ShortGuid(Guid guid)
        {
            _encodedValue = Encode(guid);
            _guid = guid;
        }

        private Guid _guid;

        /// <summary>
        /// Gets or sets the <see cref="Guid"/> the <see cref="ShortGuid"/> is based on.
        /// </summary>
        public Guid Guid
        {
            get { return _guid; }
            set
            {
                if (value != _guid)
                {
                    _guid = value;
                    _encodedValue = Encode(value);
                }
            }
        }

        string _encodedValue;

        /// <summary>
        /// Gets or sets the underlying encoded string.
        /// </summary>
        public string Value
        {
            get { return _encodedValue; }
            set
            {
                if (value != _encodedValue)
                {
                    _encodedValue = value;
                    _guid = Decode(value);
                }
            }
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared.
        /// </returns>
        /// <param name="obj">
        /// An object to compare with this instance. 
        /// </param>
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            if (obj is ShortGuid)
                return _guid.CompareTo(((ShortGuid)obj)._guid);
            if (obj is Guid)
                return _guid.CompareTo((Guid)obj);
            if (obj is string)
                return _encodedValue.CompareTo(((string)obj));

            return 0;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this instance. 
        /// </param>
        public int CompareTo(Guid other)
        {
            return _guid.CompareTo(other);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this instance. 
        /// </param>
        public int CompareTo(ShortGuid other)
        {
            return _guid.CompareTo(other._guid);
        }

        /// <summary>
        /// Returns the base64 encoded guid as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _encodedValue;
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a
        /// specified Object represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is ShortGuid)
                return _guid.Equals(((ShortGuid)obj)._guid);
            if (obj is Guid)
                return _guid.Equals((Guid)obj);
            if (obj is string)
                return _encodedValue.Equals(((string)obj));

            return false;
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a
        /// specified Object represent the same type and value.
        /// </summary>
        /// <param name="other">The object to compare</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ShortGuid other)
        {
            return _guid.Equals(other._guid);
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a
        /// specified Object represent the same type and value.
        /// </summary>
        /// <param name="other">The object to compare</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Guid other)
        {
            return _guid.Equals(other);
        }

        /// <summary>
        /// Returns the HashCode for underlying Guid.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }

        /// <summary>
        /// Initialises a new instance of the ShortGuid class
        /// </summary>
        /// <returns></returns>
        public static ShortGuid NewGuid()
        {
            return new ShortGuid(Guid.NewGuid());
        }

        /// <summary>
        /// Encodes the given Guid as an encoded string.
        /// </summary>
        /// <param name="guid">The Guid to encode</param>
        /// <returns>The encoded string.</returns>
        public static string Encode(Guid guid)
        {
            byte[] guidBytes = guid.ToByteArray();
            // add extra byte to make number positive
            byte[] sifted = new byte[guidBytes.Length + 1];
            Buffer.BlockCopy(guidBytes, 0, sifted, 0, guidBytes.Length);

            return BaseConvert.ToBaseString(sifted, BaseConvert.Base62); ;
        }

        /// <summary>
        /// Decodes the given encoded string to a Guid.
        /// </summary>
        /// <param name="value">The encoded string of a Guid.</param>
        /// <returns>A Guid that was represented by the encoded string.</returns>
        public static Guid Decode(string value)
        {
            BigInteger result = BaseConvert.FromBaseString(value, BaseConvert.Base62);

            byte[] resultBytes = result.ToByteArray();
            byte[] bytes = new byte[16];

            // size to guid
            int count = Math.Min(bytes.Length, resultBytes.Length);
            Buffer.BlockCopy(resultBytes, 0, bytes, 0, count);

            return new Guid(bytes);
        }

        /// <summary>
        /// Determines if both ShortGuids have the same underlying 
        /// Guid value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(ShortGuid x, ShortGuid y)
        {
            if ((object)x == null) return (object)y == null;
            return x._guid == y._guid;
        }

        /// <summary>
        /// Determines if both ShortGuids do not have the 
        /// same underlying Guid value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(ShortGuid x, ShortGuid y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Implicitly converts the ShortGuid to it's string equivilent
        /// </summary>
        /// <param name="shortGuid"></param>
        /// <returns></returns>
        public static implicit operator string(ShortGuid shortGuid)
        {
            return shortGuid._encodedValue;
        }

        /// <summary>
        /// Implicitly converts the ShortGuid to it's Guid equivilent
        /// </summary>
        /// <param name="shortGuid"></param>
        /// <returns></returns>
        public static implicit operator Guid(ShortGuid shortGuid)
        {
            return shortGuid._guid;
        }

        /// <summary>
        /// Implicitly converts the string to a ShortGuid
        /// </summary>
        /// <param name="shortGuid"></param>
        /// <returns></returns>
        public static implicit operator ShortGuid(string shortGuid)
        {
            return new ShortGuid(shortGuid);
        }

        /// <summary>
        /// Implicitly converts the Guid to a ShortGuid 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static implicit operator ShortGuid(Guid guid)
        {
            return new ShortGuid(guid);
        }
    }
}
