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

using System;
using System.IO;

namespace Revise.Exceptions {
    /// <summary>
    /// The exception that is thrown when the file being opened is in use by another process.
    /// </summary>
    public class FileInUseException : Exception {
        /// <summary>
        /// The format of the exception message.
        /// </summary>
        private const string MESSAGE_FORMAT = "File '{0}' is in use by another process";

        #region Properties

        /// <summary>
        /// Gets the file path of the file which threw the exception.
        /// </summary>
        public string FilePath {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FileInUseException"/> class.
        /// </summary>
        /// <param name="filePath">The file path of the file which threw the exception.</param>
        public FileInUseException(string filePath)
            : base(string.Format(MESSAGE_FORMAT, filePath)) {
            FilePath = Path.GetFullPath(filePath);
        }
    }
}