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
using Revise.Exceptions;
using Revise.PTL.Attributes;
using Revise.PTL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Revise.PTL
{
    /// <summary>
    /// Provides the ability to create, open and save PTL files for particle data.
    /// </summary>
    public class ParticleFile : FileLoader
    {
        #region Properties

        /// <summary>
        /// Gets the sequences.
        /// </summary>
        public List<ParticleSequence> Sequences
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleFile"/> class.
        /// </summary>
        public ParticleFile()
        {
            Sequences = new List<ParticleSequence>();

            Reset();
        }

        /// <summary>
        /// Loads the file from the specified stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        public override void Load(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream, Encoding.GetEncoding("us-ascii"));

            int sequenceCount = reader.ReadInt32();

            for (int i = 0; i < sequenceCount; i++)
            {
                ParticleSequence sequence = new ParticleSequence
                {
                    Name = reader.ReadIntString(),
                    Lifetime = new MinMax<float>(reader.ReadSingle(), reader.ReadSingle()),
                    EmitRate = new MinMax<float>(reader.ReadSingle(), reader.ReadSingle()),
                    LoopCount = reader.ReadInt32(),
                    SpawnDirection = new MinMax<Vector3>(reader.ReadVector3(), reader.ReadVector3()),
                    EmitRadius = new MinMax<Vector3>(reader.ReadVector3(), reader.ReadVector3()),
                    Gravity = new MinMax<Vector3>(reader.ReadVector3(), reader.ReadVector3()),
                    TextureFileName = reader.ReadIntString(),
                    ParticleCount = reader.ReadInt32(),
                    Alignment = (AlignmentType)reader.ReadInt32(),
                    UpdateCoordinate = (CoordinateType)reader.ReadInt32(),
                    TextureWidth = reader.ReadInt32(),
                    TextureHeight = reader.ReadInt32(),
                    Implementation = (ImplementationType)reader.ReadInt32(),
                    DestinationBlendMode = (Types.Utils.Blend)reader.ReadInt32(),
                    SourceBlendMode = (Types.Utils.Blend)reader.ReadInt32(),
                    BlendOperation = (Types.Utils.BlendOperation)reader.ReadInt32()
                };

                int eventCount = reader.ReadInt32();

                for (int j = 0; j < eventCount; j++)
                {
                    ParticleEventType type = (ParticleEventType)reader.ReadInt32();

                    if (!Enum.IsDefined(typeof(ParticleEventType), type))
                    {
                        throw new InvalidParticleEventTypeException((int)type);
                    }

                    Type classType = type.GetAttributeValue<ParticleEventTypeAttribute, Type>(x => x.Type);
                    IParticleEvent @event = (IParticleEvent)Activator.CreateInstance(classType);
                    @event.Read(reader);

                    sequence.Events.Add(@event);
                }

                Sequences.Add(sequence);
            }
        }

        /// <summary>
        /// Saves the file to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        public override void Save(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream, Encoding.GetEncoding("us-ascii"));

            writer.Write(Sequences.Count);

            Sequences.ForEach(sequence =>
            {
                writer.WriteIntString(sequence.Name);
                writer.Write(sequence.Lifetime.Minimum);
                writer.Write(sequence.Lifetime.Maximum);
                writer.Write(sequence.EmitRate.Minimum);
                writer.Write(sequence.EmitRate.Maximum);
                writer.Write(sequence.LoopCount);
                writer.Write(sequence.SpawnDirection.Minimum);
                writer.Write(sequence.SpawnDirection.Maximum);
                writer.Write(sequence.EmitRadius.Minimum);
                writer.Write(sequence.EmitRadius.Maximum);
                writer.Write(sequence.Gravity.Minimum);
                writer.Write(sequence.Gravity.Maximum);
                writer.WriteIntString(sequence.TextureFileName);
                writer.Write(sequence.ParticleCount);
                writer.Write((int)sequence.Alignment);
                writer.Write((int)sequence.UpdateCoordinate);
                writer.Write(sequence.TextureWidth);
                writer.Write(sequence.TextureHeight);
                writer.Write((int)sequence.Implementation);
                writer.Write((int)sequence.DestinationBlendMode);
                writer.Write((int)sequence.SourceBlendMode);
                writer.Write((int)sequence.BlendOperation);

                writer.Write(sequence.Events.Count);

                sequence.Events.ForEach(@event =>
                {
                    writer.Write((int)@event.Type);
                    @event.Write(writer);
                });
            });
        }

        /// <summary>
        /// Removes all sequences.
        /// </summary>
        public void Clear()
        {
            Sequences.Clear();
        }

        /// <summary>
        /// Resets properties to their default values.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Clear();
        }
    }
}