//********************************************************************************************************
// Product Name: MapWindow.dll Alpha
// Description:  The basic module for MapWindow version 6.0
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
// The Initial Developer of this Original Code is Ted Dunsford. Created 8/20/2009 2:59:32 PM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using MapWindow.Main;
using MapWindow.Data;
using MapWindow.Drawing;
using MapWindow.Geometries;
using MapWindow.Map;

namespace MapWindow.Analysis
{


    /// <summary>
    /// VectorToRaster uses the help of GDI+ to create a bitmap, draws the features to
    /// the bitmap, and then converts the color coded cells to a raster format.
    /// This is limited to bitmaps that are within the 8,000 x 8,0000 size limits
    /// </summary>
    public class VectorToRaster
    {

        
        /// <summary>
        /// Creates a new raster with the specified cell size.  If the cell size
        /// is zero, this will default to the shorter of the width or height
        /// divided by 256.  If the cell size produces a raster that is greater
        /// than 8,000 pixels in either dimension, it will be re-sized to
        /// create an 8,000 length or width raster.
        /// </summary>
        /// <param name="fs">The featureset to convert to a raster</param>
        /// <param name="cellSize">The double extent of the cell.</param>
        /// <param name="fieldName">The integer field index of the file.</param>
        /// <param name="destFilename">The filename of the raster to create</param>
        /// <returns>IRaster</returns>
        public static IRaster ToRaster(IFeatureSet fs, ref double cellSize, string fieldName, string destFilename)
        {
            return ToRaster(fs, ref cellSize, fieldName, destFilename, "", new string[] {}, null);
        }


        /// <summary>
        /// Creates a new raster with the specified cell size.  If the cell size
        /// is zero, this will default to the shorter of the width or height
        /// divided by 256.  If the cell size produces a raster that is greater
        /// than 8,000 pixels in either dimension, it will be re-sized to
        /// create an 8,000 length or width raster.
        /// </summary>
        /// <param name="fs">The featureset to convert to a raster</param>
        /// <param name="cellSize">The double extent of the cell.</param>
        /// <param name="fieldName">The integer field index of the file.</param>
        /// <param name="destFilename">The filename of the raster to create</param>
        /// <param name="driverCode">The optional GDAL driver code to use if using GDAL 
        /// for a format that is not discernable from the file extension.  An empty string
        ///  is usually perfectly acceptable here.</param>
        /// <param name="options">For GDAL rasters, they can be created with optional parameters
        ///  passed in as a string array.  In most cases an empty string is perfectly acceptable.</param>
        /// <param name="progressHandler">An interface for handling the progress messages.</param>
        /// <returns>Generates a raster from the vectors.</returns>
        public static IRaster ToRaster(IFeatureSet fs, ref double cellSize, string fieldName, string destFilename, string driverCode, string[] options, IProgressHandler progressHandler)
        {
            IEnvelope env = fs.Envelope;
            if(cellSize == 0)
            {
                if(fs.Envelope.Width < fs.Envelope.Height)
                {
                    cellSize = env.Width/256;
                }
                else
                {
                    cellSize = env.Height/256;
                }
            }
            int w = (int)Math.Ceiling(env.Width/cellSize);
            if (w > 8000)
            {
                w = 8000;
                cellSize = env.Width/8000;
            }
            int h = (int) Math.Ceiling(env.Height/cellSize);
            if (h > 8000)
            {
                cellSize = env.Height/8000;
                h = 8000;
            }
            Bitmap bmp = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.Transparent);
            g.SmoothingMode = SmoothingMode.None;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            Hashtable colorTable;
            MapArgs args = new MapArgs(new Rectangle(0, 0, w, h), env, g);

            switch (fs.FeatureType)
            {
                case FeatureTypes.Polygon:
                    {
                        MapPolygonLayer mpl = new MapPolygonLayer(fs);
                        PolygonScheme ps = new PolygonScheme();
                        colorTable = ps.GenerateUniqueColors(fs, fieldName);
                        mpl.Symbology = ps;
                        mpl.DrawRegions(args, new List<IEnvelope> {env});
                    }
                    break;
                case FeatureTypes.Line:
                    {
                        MapLineLayer mpl = new MapLineLayer(fs);
                        LineScheme ps = new LineScheme();
                        colorTable = ps.GenerateUniqueColors(fs, fieldName);
                        mpl.Symbology = ps;
                        mpl.DrawRegions(args, new List<IEnvelope> { env });
                    }
                    break;
                default:
                    {
                        MapPointLayer mpl = new MapPointLayer(fs);
                        PointScheme ps = new PointScheme();
                        colorTable = ps.GenerateUniqueColors(fs, fieldName);
                        mpl.Symbology = ps;
                        mpl.DrawRegions(args, new List<IEnvelope> { env });
                    }
                    break;
            }
            Type tp = fieldName == "FID" ? typeof(int) : fs.DataTable.Columns[fieldName].DataType;
           
            if (tp == typeof(string)) tp = typeof (double); // We will try to convert to double if it is a string
            Raster output = new Raster();
            MWImageData image = new MWImageData(bmp, env);
            ProgressMeter pm = new ProgressMeter(progressHandler, "Converting To Raster Cells", h);
            
            output.CreateNew(destFilename, driverCode, w, h, 1, tp, options);
            output.Bounds = new RasterBounds(h, w, env);
            List<RcIndex> locations = new List<RcIndex>();
            List<string> failureList = new List<string>();
            for (int row = 0; row < h; row++)
            {
                for (int col = 0; col < w; col++)
                {
                    Color c = image.GetColor(row, col);
                    if (c.A == 0)
                    {
                        output.Value[row, col] = output.NoDataValue;
                    }
                    else
                    {
                        if (colorTable.ContainsKey(c) == false)
                        {
                            if (c.A < 125)
                            {
                                output.Value[row, col] = output.NoDataValue;
                                continue;
                            }
                            // Use a color matching distance to pick the closest member
                            object val = GetCellValue(w, h, row, col, image, c, colorTable, locations);

                            output.Value[row, col] = GetDouble(val, failureList);
                        }
                        else
                        {
                            
                            output.Value[row, col] = GetDouble(colorTable[c], failureList);
                        }

                    }


                }
                pm.CurrentValue = row;
            }
            const int maxIterations = 5;
            int iteration = 0;
            while(locations.Count > 0)
            {
                List<RcIndex> newLocations = new List<RcIndex>();
                foreach (RcIndex location in locations)
                {
                    object val = GetCellValue(w, h, location.Row, location.Column, image, image.GetColor(location.Row, location.Column), colorTable, newLocations);
                    output.Value[location.Row, location.Column] = GetDouble(val, failureList);
                }
                locations = newLocations;
                iteration++;
                if(iteration > maxIterations) break;
            }

            pm.Reset();
            return output;
        }

        private static double GetDouble(object val, List<string> failureList)
        {
            double dVal;
            string sVal = val.ToString();
            if (double.TryParse(sVal, out dVal) == false)
            {
                if (failureList.Contains(sVal))
                {
                    dVal = failureList.IndexOf(sVal);
                }
                else
                {
                    failureList.Add(sVal);
                    dVal = failureList.Count - 1;
                }
            }
            return dVal;
        }

        private static object GetCellValue(int w, int h, int row, int col, MWImageData image, Color c, Hashtable colorTable, List<RcIndex> locations)
        {
            double dmin = double.MaxValue;
            object val = 0;
            bool empty = true;
            // Search 8 neighbor cells for likely blended neighbors
            // (otherwise distant shapes might match better incorrectly)
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == j && j == 0) continue;
                    if (row + i < 0 || row + i >= h) continue;
                    if (col + j < 0 || col + j >= w) continue;
                    Color nc = image.GetColor(row + i, col + j);
                    if (colorTable.ContainsKey(nc) == false) continue;
                    double d = (nc.R - c.R) * (nc.R - c.R) + (nc.G - c.G) * (nc.G - c.G) +
                               (nc.B - c.B) * (nc.B - c.B);
                    if (d >= dmin) continue;

                    val = colorTable[nc];
                    dmin = d;
                    empty = false;
                    image.SetColor(row, col, nc);
                }
            }
            if (empty)
            {
                locations.Add(new RcIndex(row, col));
            }
            return val;
        }
    }
}
