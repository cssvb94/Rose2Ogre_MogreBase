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

using Revise.Exceptions;
using Revise.ZMO.Attributes;
using Revise.ZMO.Channels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Revise.ZMO
{
    /// <summary>
    /// Provides the ability to create, open and save ZMO files used for motion.
    /// </summary>
    public class MotionFile : FileLoader
    {
        #region Constants

        private const string FILE_IDENTIFIER = "ZMO0002";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the frames per second rate.
        /// </summary>
        public int FramesPerSecond { get; set; }

        /// <summary>
        /// Gets or sets the frame count.
        /// </summary>
        public int FrameCount
        {
            get => frameCount;
            set
            {
                frameCount = value;
                Channels.ForEach(channel => channel.FrameCount = frameCount);
            }
        }

        /// <summary>
        /// Gets the channel count.
        /// </summary>
        public int ChannelCount => Channels.Count;

        #endregion

        private int frameCount;
        public List<MotionChannel> Channels { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionFile"/> class.
        /// </summary>
        public MotionFile()
        {
            Channels = new List<MotionChannel>();
            Reset();
        }

        /// <summary>
        /// Gets the specified <see cref="Revise.ZMO.Channels.MotionChannel"/>.
        /// </summary>
        public MotionChannel this[int channel]
        {
            get
            {
                if (channel < 0 || channel > Channels.Count - 1)
                    throw new ArgumentOutOfRangeException("channel", "Channel is out of range");
                return Channels[channel];
            }
        }

        /// <summary>
        /// Loads the file from the specified stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        public override void Load(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream, Encoding.GetEncoding("us-ascii"));

            string identifier = reader.ReadNullTerminatedString();

            if (string.Compare(identifier, FILE_IDENTIFIER, false) != 0)
                throw new FileIdentifierMismatchException(FilePath, FILE_IDENTIFIER, identifier);

            FramesPerSecond = reader.ReadInt32();
            FrameCount = reader.ReadInt32();
            int channelCount = reader.ReadInt32();

            for (int i = 0; i < channelCount; i++)
            {
                ChannelType type = (ChannelType)reader.ReadInt32();

                if (!Enum.IsDefined(typeof(ChannelType), type))
                    throw new InvalidMotionChannelType((int)type);

                MotionChannel channel = (MotionChannel)type.GetAttributeValue<MotionChannelTypeAttribute, object>(x => x.CreateInstance());
                channel.Index = reader.ReadInt32();
                channel.FrameCount = FrameCount;

                Channels.Add(channel);
            }

            for (int i = 0; i < FrameCount; i++)
                for (int j = 0; j < channelCount; j++)
                {
                    MotionChannel channel = Channels[j];
                    channel.ReadFrame(reader, i);
                }
        }

        /// <summary>
        /// Saves the file to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        public override void Save(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream, Encoding.GetEncoding("us-ascii"));

            writer.WriteString(FILE_IDENTIFIER);
            writer.Write((byte)0);

            writer.Write(FramesPerSecond);
            writer.Write(FrameCount);
            writer.Write(ChannelCount);

            Channels.ForEach(channel =>
            {
                writer.Write((int)channel.Type);
                writer.Write(channel.Index);
            });

            for (int i = 0; i < FrameCount; i++)
                Channels.ForEach(channel => channel.WriteFrame(writer, i));
        }

        /// <summary>
        /// Adds the specified channel type.
        /// </summary>
        /// <param name="type">The type of channel to add.</param>
        /// <returns>The channel created.</returns>
        public MotionChannel AddChannel(ChannelType type)
        {
            if (!Enum.IsDefined(typeof(ChannelType), type))
                throw new InvalidMotionChannelType((int)type);

            MotionChannel channel = (MotionChannel)type.GetAttributeValue<MotionChannelTypeAttribute, object>(x => x.CreateInstance());
            AddChannel(channel);

            return channel;
        }

        /// <summary>
        /// Adds the specified channel.
        /// </summary>
        /// <param name="channel">The channel to add.</param>
        public void AddChannel(MotionChannel channel)
        {
            channel.FrameCount = frameCount;
            Channels.Add(channel);
        }

        /// <summary>
        /// Removes the specified channel.
        /// </summary>
        /// <param name="channel">The channel to remove.</param>
        public void RemoveChannel(int channel)
        {
            if (channel < 0 || channel > Channels.Count - 1)
                throw new ArgumentOutOfRangeException("channel", "Channel is out of range");

            Channels.RemoveAt(channel);
        }

        /// <summary>
        /// Removes the specified channel.
        /// </summary>
        /// <param name="channel">The channel to remove.</param>
        public void RemoveChannel(MotionChannel channel)
        {
            if (!Channels.Contains(channel))
                throw new ArgumentException("channel", "Channel list does not contain the specified channel");

            int channelIndex = Channels.IndexOf(channel);
            RemoveChannel(channelIndex);
        }

        /// <summary>
        /// Clears all channels.
        /// </summary>
        public void Clear() => Channels.Clear();

        /// <summary>
        /// Resets properties to their default values.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            FramesPerSecond = 30;
            frameCount = 0;

            Clear();
        }
    }
}
