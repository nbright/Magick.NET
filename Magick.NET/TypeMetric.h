//=================================================================================================
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
#pragma once

namespace ImageMagick
{
	///=============================================================================================
	///<summary>
	/// Encapsulation of the ImageMagick TypeMetric object.
	///</summary>
	public ref class TypeMetric sealed
	{
		//===========================================================================================
	private:
		//===========================================================================================
		double _Ascent;
		double _Descent;
		double _MaxHorizontalAdvance;
		double _TextWidth;
		double _TextHeight;
		//===========================================================================================
	internal:
		//===========================================================================================
		TypeMetric(Magick::TypeMetric* metrics);
		//===========================================================================================
	public:
		///==========================================================================================
		///<summary>
		/// Ascent, the distance in pixels from the text baseline to the highest/upper grid coordinate
		/// used to place an outline point.
		///</summary>
		property double Ascent
		{
			double get();
		}
		///==========================================================================================
		///<summary>
		/// Descent, the distance in pixels from the baseline to the lowest grid coordinate used to
		/// place an outline point. Always a negative value.
		///</summary>
		property double Descent
		{
			double get();
		}
		///==========================================================================================
		///<summary>
		/// Maximum horizontal advance in pixels.
		///</summary>
		property double MaxHorizontalAdvance
		{
			double get();
		}
		///==========================================================================================
		///<summary>
		/// Text height in pixels.
		///</summary>
		property double TextHeight
		{
			double get();
		}
		///==========================================================================================
		///<summary>
		/// Text width in pixels.
		///</summary>
		property double TextWidth
		{
			double get();
		}
		//===========================================================================================
	};
	//==============================================================================================
}