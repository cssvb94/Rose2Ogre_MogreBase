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
    /// Represents an action to attack the nearest character using the maximum amount of a certain monster within the specified distance.
    /// </summary>
    public class AttackNearestCharacterUsingNumberOfMonstersAction : IArtificialIntelligenceAction {
        #region Properties

        /// <summary>
        /// Gets the action type.
        /// </summary>
        public ArtificialIntelligenceAction Type {
            get {
                return ArtificialIntelligenceAction.AttackNearestCharacterUsingNumberOfMonsters;
            }
        }

        /// <summary>
        /// Gets or sets the monster to attack the nearest character.
        /// </summary>
        public short Monster {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of monsters.
        /// </summary>
        public short Count {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the distance in which to search for monsters.
        /// </summary>
        public int Distance {
            get;
            set;
        }
        
        #endregion

        /// <summary>
        /// Reads the condition data from the underlying stream.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public void Read(BinaryReader reader) {
            Monster = reader.ReadInt16();
            Count = reader.ReadInt16();
            Distance = reader.ReadInt32();
        }

        /// <summary>
        /// Writes the condition data to the underlying stream.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Write(BinaryWriter writer) {
            writer.Write(Monster);
            writer.Write(Count);
            writer.Write(Distance);
        }
    }
}