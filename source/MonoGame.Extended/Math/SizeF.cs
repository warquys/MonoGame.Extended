using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    /// <summary>
    ///     A two dimensional size defined by two real numbers, a width and a height.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A size is a subspace of two-dimensional space, the area of which is described in terms of a two-dimensional
    ///         coordinate system, given by a reference point and two coordinate axes.
    ///     </para>
    ///     <para>
    ///         The two components are stored as a <see cref="Vector2" />, so arithmetic and comparison go through the
    ///         hardware accelerated paths of <see cref="System.Numerics" />.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IEquatable{T}" />
    /// <seealso cref="IEquatableByRef{SizeF}" />
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct SizeF : IEquatable<SizeF>, IEquatableByRef<SizeF>
    {
        #region Properties & Variables
        /// <summary>
        ///     Returns a <see cref="SizeF" /> with <see cref="Width" /> and <see cref="Height" /> equal to <c>0.0f</c>.
        /// </summary>
        public static readonly SizeF Empty = new SizeF();

        /// <summary>
        ///     The two components of this <see cref="SizeF" />, where X is the width and Y is the height.
        /// </summary>
        public Vector2 Vector;

        /// <summary>
        ///     The horizontal component of this <see cref="SizeF" />.
        /// </summary>
        public float Width
        {
            readonly get => Vector.X;
            set => Vector.X = value;
        }

        /// <summary>
        ///     The vertical component of this <see cref="SizeF" />.
        /// </summary>
        public float Height
        {
            readonly get => Vector.Y;
            set => Vector.Y = value;
        }

        /// <summary>
        ///     Gets a value that indicates whether this <see cref="SizeF" /> is empty.
        /// </summary>
        public readonly bool IsEmpty => Vector == Vector2.Zero;

        internal readonly string DebugDisplayString => ToString();
        #endregion

        #region Constructor & Destructor
        /// <summary>
        ///     Initializes a new instance of the <see cref="SizeF" /> structure from the specified dimensions.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public SizeF(float width, float height)
        {
            Vector = new Vector2(width, height);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SizeF" /> structure from the specified dimensions.
        ///     Where <see cref="Width"/> is <see cref="Vector2.X"/> and <see cref="Height"/> is <see cref="Vector2.Y"/>.
        /// </summary>
        /// <param name="size">Where Width = X and Height = Y</param>
        public SizeF(Vector2 size)
        {
            Vector = size;
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Calculates the <see cref="SizeF" /> representing the vector addition of two <see cref="SizeF" /> structures.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     The <see cref="SizeF" /> representing the vector addition of two <see cref="SizeF" /> structures.
        /// </returns>
        public static SizeF Add(SizeF first, SizeF second)
        {
            return new SizeF(first.Vector + second.Vector);
        }

        /// <summary>
        ///     Calculates the <see cref="SizeF" /> representing the vector subtraction of two <see cref="SizeF" /> structures.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     The <see cref="SizeF" /> representing the vector subtraction of two <see cref="SizeF" /> structures.
        /// </returns>
        public static SizeF Subtract(SizeF first, SizeF second)
        {
            return new SizeF(first.Vector - second.Vector);
        }

        /// <summary>
        ///     Indicates whether this <see cref="SizeF" /> is equal to another <see cref="SizeF" />.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="SizeF" /> is equal to the <paramref name="size" /> parameter; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public readonly bool Equals(SizeF size)
        {
            return Equals(in size);
        }

        /// <summary>
        ///     Indicates whether this <see cref="SizeF" /> is equal to another <see cref="SizeF" />.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="SizeF" /> is equal to the <paramref name="size" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public readonly bool Equals(ref readonly SizeF size)
        {
            return Vector.Equals(size.Vector);
        }

        /// <summary>
        ///     Returns a value indicating whether this <see cref="SizeF" /> is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to make the comparison with.</param>
        /// <returns>
        ///     <c>true</c> if this  <see cref="SizeF" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
        /// </returns>
        public override readonly bool Equals(object obj)
        {
            return obj is SizeF size && Equals(in size);
        }

        /// <summary>
        ///     Returns a hash code of this <see cref="SizeF" /> suitable for use in hashing algorithms and data
        ///     structures like a hash table.
        /// </summary>
        /// <returns>
        ///     A hash code of this <see cref="SizeF" />.
        /// </returns>
        public override readonly int GetHashCode()
        {
            return Vector.GetHashCode();
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="SizeF" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this <see cref="SizeF" />.
        /// </returns>
        public override readonly string ToString()
        {
            return $"Width: {Width}, Height: {Height}";
        }
        #endregion

        #region Operators
        /// <inheritdoc cref="Subtract"/>
        public static SizeF operator -(SizeF first, SizeF second)
        {
            return Subtract(first, second);
        }

        /// <inheritdoc cref="Subtract"/>
        public static SizeF operator -(SizeF first, Vector2 second)
        {
            return new SizeF(first.Vector - second);
        }

        /// <inheritdoc cref="Subtract"/>
        public static SizeF operator -(Vector2 first, SizeF second)
        {
            return new SizeF(first - second.Vector);
        }

        /// <summary>
        ///     Compares two <see cref="SizeF" /> structures. The result specifies
        ///     whether the values of the <see cref="Width" /> or <see cref="Height" />
        ///     fields of the two <see cref="SizeF" /> structures are unequal.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Width" /> or <see cref="Height" />
        ///     fields of the two <see cref="SizeF" /> structures are unequal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(SizeF first, SizeF second)
        {
            return !(first == second);
        }

        /// <summary>Multiplies the specified size by a specified scalar value.</summary>
        /// <param name="size">The size.</param>
        /// <param name="scalar">The scalar value.</param>
        /// <returns>The result of the product.</returns>
        public static SizeF operator *(SizeF size, float scalar)
        {
            return new SizeF(size.Vector * scalar);
        }

        /// <summary>Divides the specified size by a specified scalar value.</summary>
        /// <param name="size">The size.</param>
        /// <param name="scalar">The scalar value.</param>
        /// <returns>The result of the division.</returns>
        public static SizeF operator /(SizeF size, float scalar)
        {
            return new SizeF(size.Vector / scalar);
        }

        /// <inheritdoc cref="Add"/>
        public static SizeF operator +(SizeF first, SizeF second)
        {
            return Add(first, second);
        }

        /// <inheritdoc cref="Add"/>
        public static SizeF operator +(SizeF first, Vector2 second)
        {
            return new SizeF(first.Vector + second);
        }

        /// <inheritdoc cref="Add"/>
        public static SizeF operator +(Vector2 first, SizeF second)
        {
            return new SizeF(first + second.Vector);
        }

        /// <summary>
        ///     Compares two <see cref="SizeF" /> structures. The result specifies
        ///     whether the values of the <see cref="Width" /> and <see cref="Height" />
        ///     fields of the two <see cref="SizeF" /> structures are equal.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Width" /> and <see cref="Height" />
        ///     fields of the two <see cref="SizeF" /> structures are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(SizeF first, SizeF second)
        {
            return first.Equals(in second);
        }

        /// <summary>
        ///     Performs an explicit conversion from a <see cref="SizeF" /> to a <see cref="Point" />.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The resulting <see cref="Point" />.
        /// </returns>
        public static explicit operator Point(SizeF size)
        {
            return new Point((int)size.Width, (int)size.Height);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Size" /> to a <see cref="SizeF" />.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The resulting <see cref="SizeF" />.
        /// </returns>
        public static implicit operator SizeF(Size size)
        {
            return new SizeF((int)size.Width, (int)size.Height);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Vector2" /> to a <see cref="SizeF" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     The resulting <see cref="SizeF" />.
        /// </returns>
        public static implicit operator SizeF(Vector2 point)
        {
            return new SizeF(point);
        }

        /// <summary>
        ///     Performs an explicit conversion from a <see cref="Point" /> to a <see cref="SizeF" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     The resulting <see cref="SizeF" />.
        /// </returns>
        public static explicit operator SizeF(Point point)
        {
            return new SizeF(point.X, point.Y);
        }

        /// <summary>
        ///     Performs an explicit conversion from a <see cref="SizeF" /> to a <see cref="Vector2" />.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The resulting <see cref="Vector2" />.
        /// </returns>
        public static explicit operator Vector2(SizeF size)
        {
            return size.Vector;
        }
        #endregion
    }
}
