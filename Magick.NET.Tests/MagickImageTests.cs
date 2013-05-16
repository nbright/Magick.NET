﻿//=================================================================================================
// Copyright 2013 Dirk Lemstra <http://magick.codeplex.com/>
//
// Licensed under the ImageMagick License (the "License"); you may not use this file except in 
// compliance with the License. You may obtain a copy of the License at
//
//   http://www.imagemagick.org/script/license.php
//
// Unless required by applicable law or agreed to in writing, software distributed under the
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
// express or implied. See the License for the specific language governing permissions and
// limitations under the License.
//=================================================================================================

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ImageMagick;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Magick.NET.Tests
{
	//==============================================================================================
	[TestClass]
	public class MagickImageTests
	{
		//===========================================================================================
		private const string _Category = "MagickImage";
		//===========================================================================================
		private static void Test_ToBitmap(MagickImage image, ImageFormat format)
		{
			Bitmap bmp = image.ToBitmap(format);
			Assert.AreEqual(format, bmp.RawFormat);
			bmp.Dispose();
		}
		//===========================================================================================
		[TestMethod, TestCategory(_Category)]
		public void Test_Constructor()
		{
			ExceptionAssert.Throws<ArgumentException>(delegate()
			{
				MagickImage image = new MagickImage(new byte[0]);
			});

			ExceptionAssert.Throws<ArgumentNullException>(delegate()
			{
				MagickImage image = new MagickImage((byte[])null);
			});

			ExceptionAssert.Throws<ArgumentNullException>(delegate()
			{
				MagickImage image = new MagickImage((Bitmap)null);
			});

			ExceptionAssert.Throws<ArgumentNullException>(delegate()
			{
				MagickImage image = new MagickImage((Stream)null);
			});

			ExceptionAssert.Throws<ArgumentNullException>(delegate()
			{
				MagickImage image = new MagickImage((string)null);
			});

			ExceptionAssert.Throws<ArgumentException>(delegate()
			{
				MagickImage image = new MagickImage(Settings.ImageDir);
			});

			using (Bitmap bitmap = new Bitmap(Settings.ImageDir + @"Snakeware.png"))
			{
				using (MagickImage bitmapImage = new MagickImage(bitmap))
				{
					Assert.IsTrue(bitmapImage.Format == MagickFormat.Png);
				}
			}
		}
		//===========================================================================================
		[TestMethod, TestCategory(_Category)]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void Test_Dispose()
		{
			MagickImage image = new MagickImage();
			image.Dispose();
			image.Verbose = true;
		}
		//===========================================================================================
		[TestMethod, TestCategory(_Category)]
		public void Test_IComparable()
		{
			MagickImage first = new MagickImage(Color.Red, 10, 5);

			Assert.AreEqual(0, first.CompareTo(first));
			Assert.AreEqual(1, first.CompareTo(null));
			Assert.IsFalse(first < null);
			Assert.IsFalse(first <= null);
			Assert.IsTrue(first > null);
			Assert.IsTrue(first >= null);
			Assert.IsTrue(null < first);
			Assert.IsTrue(null <= first);
			Assert.IsFalse(null > first);
			Assert.IsFalse(null >= first);

			MagickImage second = new MagickImage(Color.Green, 5, 5);

			Assert.AreEqual(1, first.CompareTo(second));
			Assert.IsFalse(first < second);
			Assert.IsFalse(first <= second);
			Assert.IsTrue(first > second);
			Assert.IsTrue(first >= second);

			second = new MagickImage(Color.Red, 5, 10);

			Assert.AreEqual(0, first.CompareTo(second));
			Assert.IsFalse(first == second);
			Assert.IsFalse(first < second);
			Assert.IsTrue(first <= second);
			Assert.IsFalse(first > second);
			Assert.IsTrue(first >= second);

			first.Dispose();
			second.Dispose();
		}
		//===========================================================================================
		[TestMethod, TestCategory(_Category)]
		public void Test_IEquatable()
		{
			MagickImage first = new MagickImage(Color.Red, 10, 10);

			Assert.IsFalse(first == null);
			Assert.IsFalse(first.Equals(null));
			Assert.IsTrue(first.Equals(first));
			Assert.IsTrue(first.Equals((object)first));

			MagickImage second = new MagickImage(Color.Red, 10, 10);

			Assert.IsTrue(first == second);
			Assert.IsTrue(first.Equals(second));
			Assert.IsTrue(first.Equals((object)second));

			second = new MagickImage(Color.Green, 10, 10);

			Assert.IsTrue(first != second);
			Assert.IsFalse(first.Equals(second));

			first.Dispose();
			second.Dispose();
		}
		//===========================================================================================
		[TestMethod, TestCategory(_Category)]
		public void Test_Read()
		{
			MagickImage image = new MagickImage();

			ExceptionAssert.Throws<ArgumentException>(delegate()
			{
				image.Read(new byte[0]);
			});

			ExceptionAssert.Throws<ArgumentNullException>(delegate()
			{
				image.Read((byte[])null);
			});

			ExceptionAssert.Throws<ArgumentNullException>(delegate()
			{
				image.Read((Bitmap)null);
			});

			ExceptionAssert.Throws<ArgumentNullException>(delegate()
			{
				image.Read((Stream)null);
			});

			ExceptionAssert.Throws<ArgumentNullException>(delegate()
			{
				image.Read((string)null);
			});

			ExceptionAssert.Throws<ArgumentException>(delegate()
			{
				image.Read(Settings.ImageDir);
			});

			image.Read(File.ReadAllBytes(Settings.ImageDir + @"Snakeware.png"));

			using (Bitmap bitmap = new Bitmap(Settings.ImageDir + @"Snakeware.png"))
			{
				image.Read(bitmap);
				Assert.IsTrue(image.Format == MagickFormat.Png);
			}

			using (FileStream fs = File.OpenRead(Settings.ImageDir + @"Snakeware.png"))
			{
				image.Read(fs);
			}

			image.Read(Settings.ImageDir + @"Snakeware.png");

			image.Dispose();
		}
		//===========================================================================================
		[TestMethod, TestCategory(_Category)]
		public void Test_ReadSettings()
		{
			using (MagickImage image = new MagickImage())
			{
				MagickReadSettings settings = new MagickReadSettings();
				settings.ColorSpace = ColorSpace.RGB;
				settings.Density = new MagickGeometry(150, 150);

				image.Read(Settings.ImageDir + @"Snakeware.png", settings);

				Assert.AreEqual(ColorSpace.RGB, settings.ColorSpace);
				Assert.AreEqual(150, settings.Density.Width);
			}
		}
		//===========================================================================================
		[TestMethod, TestCategory(_Category)]
		public void Test_ToBitmap()
		{
			MagickImage image = new MagickImage(Color.Red, 10, 10);

			ExceptionAssert.Throws<NotSupportedException>(delegate()
			{
				image.ToBitmap(ImageFormat.Exif);
			});

			Bitmap bmp = image.ToBitmap();
			bmp.Dispose();

			Test_ToBitmap(image, ImageFormat.Bmp);
			Test_ToBitmap(image, ImageFormat.Gif);
			Test_ToBitmap(image, ImageFormat.Icon);
			Test_ToBitmap(image, ImageFormat.Jpeg);
			Test_ToBitmap(image, ImageFormat.Png);
			Test_ToBitmap(image, ImageFormat.Tiff);

			image.Dispose();
		}
		//===========================================================================================
	}
	//==============================================================================================
}