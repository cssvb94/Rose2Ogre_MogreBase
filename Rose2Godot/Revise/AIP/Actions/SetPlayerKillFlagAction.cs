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

namespace Revise.AIP.Actions
{
    /// <summary>
    /// Represents an action to enable or disable the player kill flag for the specified map.
    /// </summary>
    public class SetPlayerKillFlagAction : IArtificialIntelligenceAction {
        #region Properties

        /// <summary>
        /// Gets the action type.
        /// </summary>
        public ArtificialIntelligenceAction Type {
            get {
                return ArtificialIntelligenceAction.SetPlayerKillFlag;
            }
        }

        /// <summary>
        /// Gets or sets the map, if this value is 0 the current map is used.
        /// </summary>
        public short Map {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable playing killing.
        /// </summary>
        public bool IsEnabled {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Reads the condition data from the underlying stream.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public void Read(BinaryReader reader) {
            Map = reader.ReadInt16();
            IsEnabled = reader.ReadBoolean();
        }

        /// <summary>
        /// Writes the condition data to the underlying stream.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Write(BinaryWriter writer) {
            writer.Write(Map);
            writer.Write(IsEnabled);
        }
    }
}