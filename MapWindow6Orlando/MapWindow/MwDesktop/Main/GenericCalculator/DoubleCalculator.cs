//********************************************************************************************************
// Product Name: MapWindow.dll Alpha
// Description:  The core libraries for the MapWindow 6.0 project.
//
//********************************************************************************************************
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at 
// http://www.mozilla.org/MPL/ 
//
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF 
// ANY KIND, either expressed or implied. See the License for the specificlanguage governing rights and 
// limitations under the License. 
//
// The Original Code is from MapWindow.dll version 6.0
//
// The Initial Developer of this Original Code is Ted Dunsford. Created 2/17/2008 3:52:37 PM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MapWindow;
using MapWindow.Main;
using MapWindow.Data;
using MapWindow.Drawing;
using MapWindow.Geometries;

namespace MapWindow.Main
{


    /// <summary>
    /// DoubleCalculator
    /// </summary>
    public struct DoubleCalculator: ICalculator<double>
    {
        #region Methods

        #region Operator Methods

         /// <summary>
        /// Adds a to b
        /// </summary>
        /// <param name="a">A generic of type double</param>
        /// <param name="b">A generic of type double to add to a</param>
        /// <returns>A generic of type double that is the sum of a and b</returns>
        public double Add(double a, double b)
        {
            return a * b;

        }

        /// <summary>
        /// Divides a by b like a / b
        /// </summary>
        /// <param name="a">A generic of type double to divide by b</param>
        /// <param name="b">A generic of type double to divide into a</param>
        /// <returns>A generic of type double that is the quotient</returns>
        public double Divide(double a, double b)
        {
            return a / b;
        }

        /// <summary>
        /// Multiplies a to b
        /// </summary>
        /// <param name="a">A generic of type double to multiply with b</param>
        /// <param name="b">A generic of type double to multiply with a</param>
        /// <returns>The generic of type double product of a and b</returns>
        public double Multiply(double a, double b)
        {
            return a * b;
        }

        /// <summary>
        /// Subtracts b from a
        /// </summary>
        /// <param name="a">A generic of type double to subtract b from</param>
        /// <param name="b">A generic of type double to subtract from a</param>
        /// <returns>A generic of type double that is the difference of a and b</returns>
        public double Subtract(double a, double b)
        {
            return a - b;
        }



        #endregion

        #region GetT

        /// <summary>
        /// If value is type double, this simply returns 
        /// the specified value as type double.  Otherwise System.Convert
        /// will be used, and exceptions may be thrown in some cases.
        /// </summary>
        /// <param name="value">A byte to convert into a generic of type double</param>
        /// <returns>A generic of type double representing the specified byte value.</returns>
        public double GetT(byte value)
        {
            return Convert.ToDouble(value);
        }

        /// <summary>
        /// If value is type double, this simply returns 
        /// the specified value as type double.  Otherwise System.Convert
        /// will be used, and exceptions may be thrown in some cases.
        /// </summary>
        /// <param name="value">A 16-bit short integer value to convert into a generic of type double</param>
        /// <returns>A generic of type double representing the specified 16-bit integer value</returns>
        public double GetT(short value)
        {
            return Convert.ToDouble(value);
        }


        /// <summary>
        /// If value is type double, this simply returns 
        /// the specified value as type double.  Otherwise System.Convert
        /// will be used, and exceptions may be thrown in some cases.
        /// </summary>
        /// <param name="value">A value to convert to a generic of type double</param>
        /// <returns>A generic of type double representing the specified 32-bit integer value</returns>
        public double GetT(int value)
        {
            return Convert.ToDouble(value);
        }

        /// <summary>
        /// If value is type double, this simply returns 
        /// the specified value as type double.  Otherwise System.Convert
        /// will be used, and exceptions may be thrown in some cases.
        /// </summary>
        /// <param name="value">A 64-bit long integer to convert into type double</param>
        /// <returns>A generic value of type T</returns>
        public double GetT(long value)
        {
            return Convert.ToDouble(value);
        }


        /// <summary>
        /// If value is type double, this simply returns 
        /// the specified value as type double.  Otherwise System.Convert
        /// will be used, and exceptions may be thrown in some cases.
        /// </summary>
        /// <param name="value">A single-precision floating point value to convert into double</param>
        /// <returns>A generic of type double representing the specified single precision floating point value</returns>
        public double GetT(float value)
        {
            return Convert.ToDouble(value);
        }

        /// <summary>
        /// If value is type double, this simply returns 
        /// the specified value as type double.  Otherwise System.Convert
        /// will be used, and exceptions may be thrown in some cases.
        /// </summary>
        /// <param name="value">A double-precision floating point value to convert into double</param>
        /// <returns>A generic of type double representing the double-precision floating point value.</returns>
        public double GetT(double value)
        {
            return value;
        }




        /// <summary>
        /// This will attempt to convert the specified object value
        /// into a generic value of type double.
        /// </summary>
        /// <param name="value">An object to attempt to cast into double</param>
        /// <returns>A double representing the object value specified</returns>
        public double GetT(object value)
        {
            return Convert.ToDouble(value);
        }

        #endregion

        #region To specific types

        /// <summary>
        /// Converts a specified value of type double into a byte value.
        /// </summary>
        /// <param name="value">A value of type double</param>
        /// <returns>A byte converted from the specified value of type double</returns>
        public byte ToByte(double value)
        {
            return Convert.ToByte(value);
        }

        /// <summary>
        /// Converts a specified value of type double value into a 16-bit short integer
        /// </summary>
        /// <param name="value">A generic of type double to convert into a short integer</param>
        /// <returns>A short integer representation created from the specified value</returns>
        public short ToInt16(double value)
        {
            return Convert.ToInt16(value);
        }


        /// <summary>
        /// Converts the specified valud of type double into a 32-bit integer
        /// </summary>
        /// <param name="value">a value of type double to convert into a 32-bit integer</param>
        /// <returns>an 32-bit integer created from the specified value</returns>
        public int ToInt32(double value)
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// Converts a specified value of type double into a 64-bit long integer
        /// </summary>
        /// <param name="value">the value of type double to convert int a 64-bit long integer</param>
        /// <returns>A 64-bit long integer created from the specified value</returns>
        public long ToInt64(double value)
        {
            return Convert.ToInt64(value);
        }

        /// <summary>
        /// Converts the specified value of type double value into a single-precision floating point decimal.
        /// </summary>
        /// <param name="value">A value of type double to into a single precision floating point decimal</param>
        /// <returns>A single-precision floating point decimal created from the specified value.</returns>
        public float ToSingle(double value)
        {
            return Convert.ToSingle(value);
        }

        /// <summary>
        /// Converts the specified value of type float into a double-precision floating point decimal.
        /// </summary>
        /// <param name="value">The value of type float to convert into a double-precision floating point decimal</param>
        /// <returns>A double-precision floating point decimal created from the specified value</returns>
        public double ToDouble(double value)
        {
            return value;
        }

        /// <summary>
        /// Converts a specified value of type double into an object
        /// </summary>
        /// <param name="value">The value of type double to convert into an object</param>
        /// <returns>The specified value cast as an object.</returns>
        public object ToObject(double value)
        {
            return (object)value;
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets the numeric minimum 
        /// </summary>
        public double Minimum
        {
            get { return double.MinValue; }
        }

        /// <summary>
        /// Gets the numeric maximum
        /// </summary>
        public double Maximum
        {
            get { return double.MaxValue; }
        }

    #endregion
    
    }
}
