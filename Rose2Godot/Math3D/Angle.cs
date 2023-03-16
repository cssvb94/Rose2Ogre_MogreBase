// Copyright (C) Amer Koleci
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Math3d
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Angle : IEquatable<Angle>, IComparable<Angle>
    {
        private readonly float _value;

        public Angle(float value) => _value = value;

        public static implicit operator Radian(Angle angle) => new Radian(Math.AngleUnitsToRadians(angle._value));

        public static implicit operator Degree(Angle angle) => new Degree(Math.AngleUnitsToDegrees(angle._value));

        public int CompareTo(Angle other)
        {
            if (_value < other._value) return -1;
            if (_value > other._value) return 1;
            return 0;
        }

        /// <summary>
        /// Returns a boolean indicating whether the given Angle is equal to this Angle instance.
        /// </summary>
        /// <param name="other">The Angle to compare this instance to.</param>
        /// <returns>True if the other Angle is equal to this instance; False otherwise.</returns>
        public bool Equals(ref Angle other) => _value == other._value;

        /// <summary>
        /// Returns a boolean indicating whether the given Angle is equal to this Angle instance.
        /// </summary>
        /// <param name="other">The Angle to compare this instance to.</param>
        /// <returns>True if the other Angle is equal to this instance; False otherwise.</returns>
        public bool Equals(Angle other) => Equals(ref other);

        /// <summary>
        /// Returns a boolean indicating whether the given Object is equal to this Angle instance.
        /// </summary>
        /// <param name="obj">The Object to compare against.</param>
        /// <returns>True if the Object is equal to this Angle; False otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => obj is Angle && Equals((Angle)obj);

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => _value.GetHashCode();
    }
}
