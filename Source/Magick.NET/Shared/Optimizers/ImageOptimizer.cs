﻿// Copyright 2013-2017 Dirk Lemstra <https://github.com/dlemstra/Magick.NET/>
//
// Licensed under the ImageMagick License (the "License"); you may not use this file except in
// compliance with the License. You may obtain a copy of the License at
//
//   https://www.imagemagick.org/script/license.php
//
// Unless required by applicable law or agreed to in writing, software distributed under the
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
// either express or implied. See the License for the specific language governing permissions
// and limitations under the License.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using ImageMagick.ImageOptimizers;

namespace ImageMagick
{
    /// <summary>
    /// Class that can be used to optimize an image.
    /// </summary>
    public sealed class ImageOptimizer
    {
        private readonly Collection<IImageOptimizer> _optimizers = CreateImageOptimizers();

        /// <summary>
        /// Gets or sets a value indicating whether various compression types will be used to find
        /// the smallest file. This process will take extra time because the file has to be written
        /// multiple times.
        /// </summary>
        public bool OptimalCompression
        {
            get;
            set;
        }

        private string SupportedFormats
        {
            get
            {
                List<string> formats = new List<string>();

                foreach (IImageOptimizer optimizer in _optimizers)
                {
                    formats.Add(optimizer.Format.Module.ToString());
                }

                return string.Join(", ", formats.ToArray());
            }
        }

        /// <summary>
        /// Performs compression on the specified the file. With some formats the image will be decoded
        /// and encoded and this will result in a small quality reduction. If the new file size is not
        /// smaller the file won't be overwritten.
        /// </summary>
        /// <param name="file">The image file to compress</param>
        /// <returns>True when the image could be compressed otherwise false.</returns>
        public bool Compress(FileInfo file)
        {
            Throw.IfNull(nameof(file), file);

            return DoCompress(file);
        }

        /// <summary>
        /// Performs compression on the specified the file. With some formats the image will be decoded
        /// and encoded and this will result in a small quality reduction. If the new file size is not
        /// smaller the file won't be overwritten.
        /// </summary>
        /// <param name="fileName">The file name of the image to compress</param>
        /// <returns>True when the image could be compressed otherwise false.</returns>
        public bool Compress(string fileName)
        {
            string filePath = FileHelper.CheckForBaseDirectory(fileName);
            Throw.IfNullOrEmpty(nameof(fileName), filePath);

            return DoCompress(new FileInfo(filePath));
        }

        /// <summary>
        /// Returns true when the supplied file name is supported based on the extension of the file.
        /// </summary>
        /// <param name="file">The file to check.</param>
        /// <returns>True when the supplied file name is supported based on the extension of the file.</returns>
        /// <returns>True when the image could be compressed otherwise false.</returns>
        public bool IsSupported(FileInfo file)
        {
            return IsSupported(MagickFormatInfo.Create(file));
        }

        /// <summary>
        /// Returns true when the supplied formation information is supported.
        /// </summary>
        /// <param name="formatInfo">The format information to check.</param>
        /// <returns>True when the supplied formation information is supported.</returns>
        public bool IsSupported(MagickFormatInfo formatInfo)
        {
            Throw.IfNull(nameof(formatInfo), formatInfo);

            foreach (IImageOptimizer optimizer in _optimizers)
            {
                if (optimizer.Format.Format == formatInfo.Module)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true when the supplied file name is supported based on the extension of the file.
        /// </summary>
        /// <param name="fileName">The name of the file to check.</param>
        /// <returns>True when the supplied file name is supported based on the extension of the file.</returns>
        public bool IsSupported(string fileName)
        {
            return IsSupported(MagickFormatInfo.Create(fileName));
        }

        /// <summary>
        /// Performs lossless compression on the specified the file. If the new file size is not smaller
        /// the file won't be overwritten.
        /// </summary>
        /// <param name="file">The image file to compress</param>
        /// <returns>True when the image could be compressed otherwise false.</returns>
        public bool LosslessCompress(FileInfo file)
        {
            Throw.IfNull(nameof(file), file);

            return DoLosslessCompress(file);
        }

        /// <summary>
        /// Performs lossless compression on the specified file. If the new file size is not smaller
        /// the file won't be overwritten.
        /// </summary>
        /// <param name="fileName">The file name of the image to compress</param>
        /// <returns>True when the image could be compressed otherwise false.</returns>
        public bool LosslessCompress(string fileName)
        {
            string filePath = FileHelper.CheckForBaseDirectory(fileName);
            Throw.IfNullOrEmpty(nameof(fileName), filePath);

            return DoLosslessCompress(new FileInfo(filePath));
        }

        private static Collection<IImageOptimizer> CreateImageOptimizers()
        {
            return new Collection<IImageOptimizer>
            {
                new JpegOptimizer(),
                new PngOptimizer(),
                new GifOptimizer(),
            };
        }

        private static MagickFormatInfo GetFormatInformation(FileInfo file)
        {
            MagickFormatInfo info = MagickNET.GetFormatInformation(file);
            if (info != null)
                return info;

            MagickImageInfo imageInfo = new MagickImageInfo(file);
            return MagickNET.GetFormatInformation(imageInfo.Format);
        }

        private bool DoLosslessCompress(FileInfo file)
        {
            IImageOptimizer optimizer = GetOptimizer(file);
            if (optimizer == null)
                return false;

            optimizer.OptimalCompression = OptimalCompression;
            return optimizer.LosslessCompress(file);
        }

        private bool DoCompress(FileInfo file)
        {
            IImageOptimizer optimizer = GetOptimizer(file);
            if (optimizer == null)
                return false;

            optimizer.OptimalCompression = OptimalCompression;
            return optimizer.Compress(file);
        }

        private IImageOptimizer GetOptimizer(FileInfo file)
        {
            MagickFormatInfo info = GetFormatInformation(file);
            DebugThrow.IfNull(nameof(info), info);

            foreach (IImageOptimizer optimizer in _optimizers)
            {
                if (optimizer.Format.Module == info.Module)
                    return optimizer;
            }

            throw new MagickCorruptImageErrorException($"Invalid format, supported formats are: {SupportedFormats}");
        }
    }
}
