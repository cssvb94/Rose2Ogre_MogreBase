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

using System.IO;
using Revise.AIP.Interfaces;

namespace Revise.AIP.Conditions
{
    /// <summary>
    /// Represents a condition to check the current world time.
    /// </summary>
    public class CheckWorldTimeCondition : IArtificialIntelligenceCondition {
        #region Properties

        /// <summary>
        /// Gets the condition type.
        /// </summary>
        public ArtificialIntelligenceCondition Type {
            get {
                return ArtificialIntelligenceCondition.CheckWorldTime;
            }
        }

        /// <summary>
        /// Gets or sets the minimum time value the world time can be.
        /// </summary>
        public int MinimumTime {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum time value the world time can be.
        /// </summary>
        public int MaximumTime {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Reads the condition data from the underlying stream.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public void Read(BinaryReader reader) {
            MinimumTime = reader.ReadInt32();
            MaximumTime = reader.ReadInt32();
        }

        /// <summary>
        /// Writes the condition data to the underlying stream.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Write(BinaryWriter writer) {
            writer.Write(MinimumTime);
            writer.Write(MaximumTime);
        }
    }
}