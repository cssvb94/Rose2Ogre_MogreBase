﻿#region License

/**
 * Copyright (C) 2012 Jack Wakefield
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

#endregion

using System.Numerics;
using System.IO;

namespace Revise.PTL.Events
{
    /// <summary>
    /// Represents an event to change the scale of the particle.
    /// </summary>
    public class ScaleEvent : ParticleEvent
    {
        #region Properties

        /// <summary>
        /// Gets the event type.
        /// </summary>
        public override ParticleEventType Type
        {
            get
            {
                return ParticleEventType.Scale;
            }
        }

        /// <summary>
        /// Gets or sets the scale range.
        /// </summary>
        public MinMax<Vector2> Scale
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Reads the event data from the underlying stream.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public override void Read(BinaryReader reader)
        {
            base.Read(reader);

            Scale = new MinMax<Vector2>(reader.ReadVector2(), reader.ReadVector2());
        }

        /// <summary>
        /// Writes the event data to the underlying stream.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write(Scale.Minimum);
            writer.Write(Scale.Maximum);
        }
    }
}