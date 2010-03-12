﻿//********************************************************************************************************
// Product Name: MapWindow.Tools.mwRasterDistance
// Description:  This tool calculates the euclidean distance from each raster cell to the nearest target cell.
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
// The Original Code is Toolbox.dll for the MapWindow 4.6/6 ToolManager project
//
// The Initializeializeial Developer of this Original Code is Jiri Kadlec. Created in June, 2009.
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
// -----------------------|------------------------|--------------------------------------------
// Ted Dunsford           |  8/24/2009             |  Cleaned up some formatting issues using re-sharper 
// KP                     |  9/2009                |  Used IDW as model for RasterDistance
// Ping Yang              |  12/2009               |  Cleaning code and fixing bugs. 
//********************************************************************************************************


using System;
using MapWindow.Data;
using MapWindow.Tools;
using MapWindow.Tools.Param;

namespace MapWindow.Tools
{
    public class RasterDistance:ITool
    {
        private string _workingPath;
        private Parameter[] _inputParam;
        private Parameter[] _outputParam;

        #region ITool Members

        /// <summary>
        /// Returns the Version of the tool
        /// </summary>
        public Version Version
        {
            get { return (new Version(1, 0, 0, 0)); }
        }

        /// <summary>
        /// Returns the Author of the tool's name
        /// </summary>
        public string Author
        {
            get { return (TextStrings.MapWindowDevelopmentTeam); }
        }

        /// <summary>
        /// Returns the category of tool that the ITool should be added to
        /// </summary>
        public string Category
        {
            get { return (TextStrings.RasterOverlay); }
        }

        /// <summary>
        /// Returns a description of what the tool does for inclusion in the help section of the toolbox list
        /// </summary>
        public string Description
        {
            get { return (TextStrings.Calculateseuclideandistance); }
        }

        /// <summary>
        /// Once the Parameter have been configured the Execute command can be called, it returns true if succesful
        /// </summary>
        public bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            IRaster inputRaster = _inputParam[0].Value as IRaster;
            
            IRaster outputRaster = _outputParam[0].Value as IRaster;
            double maximumDistance = (double)_inputParam[1].Value;

            return Execute(inputRaster, outputRaster, maximumDistance, cancelProgressHandler);

        }
        /// <summary>
        /// Executes the Raster distance calculation
        /// Ping Yang deleted static for external testing 10/2010
        /// </summary>
        /// <param name="input">The input raster</param>
        /// <param name="output">The output raster</param>
        /// <param name="maxDistance">The maximum distance value. Cells with a larger distance to nearest
        /// target cell than maxDistance will be assigned 'no data' value.</param>
        /// <param name="cancelProgressHandler">The progress handler</param>
        /// <returns>true if execution successful, false otherwise</returns>
        public bool Execute(IRaster input, IRaster output, double maxDistance, ICancelProgressHandler cancelProgressHandler)
        {
            //Validates the input and output data
            if (input == null || output == null)
            {
                return false;
            }

            //Creates the output raster with the same bounds as the input
            RasterBounds bounds = (RasterBounds)input.Bounds.Copy();

            output = Raster.Create(output.Filename, "", bounds.NumColumns, bounds.NumRows, 1, typeof(int), new[] { "" });
            //output.CreateNew(output.Filename, "", bounds.NumColumns, bounds.NumRows, 1, typeof(int), new string[] { "" });

            //output = Raster.CreateNewRaster(output.Filename, bounds.NumRows, bounds.NumColumns, RasterDataTypes.INTEGER);
            output.Bounds = bounds;

            // internally we reference output as an integer type raster.
            IntRaster outRaster = output as IntRaster;

            if (outRaster != null)
            {
                int numColumns = outRaster.NumColumns;
                int numRows = outRaster.NumRows;
                int lastUpdate = 0;

                // declare two jagged arrays for storing the (Rx, Ry) vectors. 
                //rX is distance from current cell to nearest target cell measured in the x-direction
                //rY is distance from current cell to nearest target cell measured in the y-direction
                // the actual distance can be calculated as sqrt(rX^2 + rY^2).
                // in the resulting distance raster we store the squared distance as well as the rX, rY relative coordinates
                // to improve computation speed

                int[][] aRx = new int[numRows][];
                int[][] aRy = new int[numRows][];
                int[][] aSqDist = new int[numRows][];

                const int infD = int.MaxValue;
                const int targetVal = 0;

                //initialize the arrays
                for (int i = 0; i < numRows; i++)
                {
                    aRx[i] = new int[numColumns];
                    aRy[i] = new int[numColumns];
                    aSqDist[i] = new int[numColumns];
                    ReadInputRow(input, i, aSqDist[i], targetVal, infD);
                }

                // *******************************************************************
                // raster distance calculation pass one - top to bottom, left to right
                // *******************************************************************
                //int[] row1 = new int[numColumns];
                //int[] row2 = new int[numColumns];
                int[] aNcels = new int[4]; //the values of four neighbouring cells (W, NW, N, NE)
                int[] aDiff = new int[4]; //the | y coordinate distances to nearest target cell

                for(int row = 1; row < numRows; row++)
                {
                    ////read the row from input raster
                    //ReadInputRow(input, rowIndex, row2, targetVal, infD);
                
                    for (int col = 1; col < numColumns - 1; col ++)
                    {
                        int val = aSqDist[row][col];
                    
                        //Continue processing only if the current cell is not a target
                        if (val == targetVal)
                        {
                            continue;
                        }
                    
                        //read the values of the cell's neighbours
                        aNcels[0] = aSqDist[row][col - 1]; // W
                        aNcels[1] = aSqDist[row-1][col - 1]; // NW
                        aNcels[2] = aSqDist[row-1][col];     // N
                        aNcels[3] = aSqDist[row-1][col + 1]; // NE

                        //calculate the squared euclidean distances to each neighbouring cell and to the nearest target cell
                        aDiff[0] = (aNcels[0] < infD) ? aNcels[0] + 2 * aRx[row][col - 1] + 1 : infD;
                        aDiff[1] = (aNcels[1] < infD) ? aNcels[1] + 2 * (aRx[row - 1][col - 1] + aRy[row - 1][col - 1] + 1) : infD;
                        aDiff[2] = (aNcels[2] < infD) ? aNcels[2] + 2 * aRy[row - 1][col] + 1 : infD;
                        aDiff[3] = (aNcels[3] < infD) ? aNcels[3] + 2 * (aRx[row - 1][col + 1] + aRy[row - 1][col + 1] + 1) : infD;

                        //find neighbouring cell with minimum distance difference
                        int minDiff = aDiff[0];
                        int minDiffCell = 0;
                        for (int i = 1; i < 4; i++)
                        {
                            if(aDiff[i] < minDiff)
                            {
                                minDiff = aDiff[i];
                                minDiffCell = i;
                            }
                        }

                        //if a neighbouring cell with known distance was found:
                        if (minDiff < infD)
                        {
                            //assign the minimum euclidean distance
                            aSqDist[row][col] = minDiff;
                        
                            //update the (rX, rY) cell-to-nearest-target vector
                            switch(minDiffCell)
                            {
                                case 0: // W
                                    aRx[row][col] = aRx[row][col - 1] + 1;
                                    aRy[row][col] = aRy[row][col - 1];
                                    break;
                                case 1: // NW
                                    aRx[row][col] = aRx[row-1][col-1] + 1;
                                    aRy[row][col] = aRy[row-1][col-1] + 1;
                                    break;
                                case 2: // N
                                    aRx[row][col] = aRx[row-1][col];
                                    aRy[row][col] = aRy[row-1][col] + 1;
                                    break;
                                case 3: // NE
                                    aRx[row][col] = aRx[row-1][col+1] + 1;
                                    aRy[row][col] = aRy[row-1][col+1] + 1;
                                    break;
                            }
                        } 
                        //end of update (rX, rY) cell-to-nearest-target vector
                    
                    } 
                    //end or current row processing - report progress

                    int percent = (int)(row / (double)numRows * 100f);
                    if (percent > lastUpdate)
                    {
                        lastUpdate += 1;
                        cancelProgressHandler.Progress("", lastUpdate, TextStrings.Pass1 + lastUpdate + TextStrings.progresscompleted);
                        if (cancelProgressHandler.Cancel)
                        {
                            return false;
                        }
                    }
                }
                // *******************************************************************
                // end of first pass for loop
                // *******************************************************************

                // *******************************************************************
                // raster distance calculation PASS TWO - bottom to top, right to left
                // *******************************************************************
                lastUpdate = 0;
                for (int row = numRows - 2; row > 0; row--)
                {
                    for (int col = numColumns - 2; col > 0; col--)
                    {
                        int val = aSqDist[row][col];

                        //Continue processing only if the current cell is not a target
                        if (val == targetVal)
                        {
                            continue;
                        }

                        //read the values of the cell's neighbours
                        aNcels[0] = aSqDist[row][col + 1];     // E
                        aNcels[1] = aSqDist[row + 1][col + 1]; // SE
                        aNcels[2] = aSqDist[row + 1][col];     // S
                        aNcels[3] = aSqDist[row + 1][col - 1]; // SW

                        //calculate the squared euclidean distances to each neighbouring cell and to the nearest target cell
                        aDiff[0] = (aNcels[0] < infD) ? aNcels[0] + 2 * aRx[row][col + 1] + 1 : infD;
                        aDiff[1] = (aNcels[1] < infD) ? aNcels[1] + 2 * (aRx[row + 1][col + 1] + aRy[row + 1][col + 1] + 1) : infD;
                        aDiff[2] = (aNcels[2] < infD) ? aNcels[2] + 2 * aRy[row + 1][col] + 1 : infD;
                        aDiff[3] = (aNcels[3] < infD) ? aNcels[3] + 2 * (aRx[row + 1][col - 1] + aRy[row + 1][col - 1] + 1) : infD;

                        //find neighbouring cell with minimum distance difference
                        int minDiff = aDiff[0];
                        int minDiffCell = 0;
                        for (int i = 1; i < 4; i++)
                        {
                            if (aDiff[i] < minDiff)
                            {
                                minDiff = aDiff[i];
                                minDiffCell = i;
                            }
                        }

                        //if a neighbouring cell with known distance smaller than current known distance was found:
                        if (minDiff < val)
                        {
                            //assign the minimum euclidean distance
                            aSqDist[row][col] = minDiff;

                            //update the (rX, rY) cell-to-nearest-target vector
                            switch (minDiffCell)
                            {
                                case 0: // E
                                    aRx[row][col] = aRx[row][col + 1] + 1;
                                    aRy[row][col] = aRy[row][col + 1];
                                    break;
                                case 1: // SE
                                    aRx[row][col] = aRx[row + 1][col + 1] + 1;
                                    aRy[row][col] = aRy[row + 1][col + 1] + 1;
                                    break;
                                case 2: // S
                                    aRx[row][col] = aRx[row + 1][col];
                                    aRy[row][col] = aRy[row + 1][col] + 1;
                                    break;
                                case 3: // SW
                                    aRx[row][col] = aRx[row + 1][col - 1] + 1;
                                    aRy[row][col] = aRy[row + 1][col - 1] + 1;
                                    break;
                            }
                        }
                        //end of update (rX, rY) cell-to-nearest-target vector

                    }
                
                    //Write row to output raster
                    writeOutputRow(outRaster, row, aSqDist[row]);

                    //Report progress
                    int percent = (int)(row / (double)numRows * 100f);
                    if (percent > lastUpdate)
                    {
                        lastUpdate += 1;
                        cancelProgressHandler.Progress("", lastUpdate, TextStrings.Pass2 + lastUpdate + TextStrings.progresscompleted);
                        if (cancelProgressHandler.Cancel)
                        {
                            return false;
                        }
                    }
                }
            }
            // *******************************************************************
            // end of second pass proximity calculation loop
            // *******************************************************************


            //save output raster
            output.Save();

            return true;
        }

        /// <summary>
        /// Writes the integer row array to the output raster. The square distance is
        /// converted to a normal distance. Unknown distance is converted to 'no data' value
        /// </summary>
        /// <param name="output"></param>
        /// <param name="rowNumber"></param>
        /// <param name="rowArray"></param>
        private static void writeOutputRow(IntRaster output, int rowNumber, int[] rowArray)
        {
            int noDataVal = (int)output.NoDataValue;
            
            for (int col=0; col < rowArray.Length; col++)
            {
                int val = rowArray[col];
                if (val == int.MaxValue)
                {
                    output.Data[rowNumber][col] = noDataVal;
                }
                else
                {
                    output.Data[rowNumber][col] = (int)(Math.Sqrt(val));
                }
            }
        }


        /// <summary>
        /// Reads and converts the specified row from input raster to an array of integer
        /// It assigns noDataVal to 'noData' values and assigns dataVal to other values 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="rowNumber"></param>
        /// <param name="rowArray">The array where the row is saved. This array must be correctly dimensioned.</param>
        /// <param name="dataVal">New value which will be assigned to value cells</param>
        /// <param name="noDataVal">New value which will be assigned to 'no data value' cells</param>
        /// <returns></returns>
        private static void ReadInputRow(IRaster input, int rowNumber, int[] rowArray, int dataVal, int noDataVal)
        {
            //IValueGrid vals = input.Value;
            double inputNoDataVal = input.NoDataValue; 
            
            int numColumns = input.NumColumns;
            
            for(int column = 0; column < numColumns; column++)
            {
                if (input.Value[rowNumber, column] == inputNoDataVal)
                {
                    rowArray[column] = noDataVal;
                }
                else
                {
                    rowArray[column] = dataVal;
                }
            }
        }


        /// <summary>
        /// Image displayed in the help area when no input field is selected
        /// </summary>
        public System.Drawing.Bitmap HelpImage
        {
            get { return null; }
        }

        /// <summary>
        /// Help text to be displayed when no input field is selected
        /// </summary>
        public string HelpText
        {
            get
            {
                return (TextStrings.Calculatesproximitydistance);
            }
        }

        /// <summary>
        /// Returns the address of the tools help web page in HTTP://... format. Return a empty string to hide the help hyperlink.
        /// </summary>
        public string HelpURL
        {
            get { return ("HTTP://www.mapwindow.org"); }
        }


        /// <summary>
        /// 32x32 Bitmap - The Large icon that will appears in the Tool Dialog Next to the tools name
        /// </summary>
        public System.Drawing.Bitmap Icon
        {
            get { return (null); }
        }

        /// <summary>
        /// 16x16 Bitmap - The small icon that appears in the Tool Manager Tree
        /// </summary>
        public System.Drawing.Bitmap IconSmall
        {
            get { return (null); }
        }

        /// <summary>
        /// The Parameter array should be populated with default values here
        /// </summary>
        /// <returns></returns>
        public void Initialize()
        {
            _inputParam = new Parameter[2];
            _inputParam[0] = new RasterParam(TextStrings.inputRaster);
            _inputParam[0].HelpText = TextStrings.InputRastercontainingtargetcells;
            _inputParam[1] = new DoubleParam(TextStrings.Maximumdistance,100.0);
            _inputParam[1].HelpText = TextStrings.Maximumdistancetobecalculated;
            _outputParam = new Parameter[1];
            _outputParam[0] = new RasterParam(TextStrings.OutputRaster);
            _outputParam[0].HelpText = TextStrings.Selectresultrasterfilename;

        }

        /// <summary>
        /// Gets or Sets the input paramater array
        /// </summary>
        public Parameter[] InputParameters
        {
            get { return _inputParam; }
        }

        /// <summary>
        /// Returns the name of the tool
        /// </summary>
        public string Name
        {
            get { return (TextStrings.RasterDistanceproximity); }
        }

        /// <summary>
        /// Gets or Sets the output paramater array
        /// </summary>
        public Parameter[] OutputParameters
        {
            get { return _outputParam; }
        }

        /// <summary>
        /// Fires when one of the paramters value has beend changed, usually when a user changes a input or output Parameter value, this can be used to populate input2 Parameter default values.
        /// </summary>
        void ITool.ParameterChanged(Parameter sender)
        {
            return;
        }

        /// <summary>
        /// Returns a brief description displayed when the user hovers over the tool in the toolbox
        /// </summary>
        public string ToolTip
        {
            get { return (TextStrings.RasterDistance); }
        }

        /// <summary>
        /// A UniqueName Identifying this Tool, if aninput2 tool with the same UniqueName exists this tool will not be loaded
        /// </summary>
        public string UniqueName
        {
            get { return (TextStrings.MapWindowRasterEuclideanDistance); }
        }

        /// <summary>
        /// This is set before the tool is executed to provide a folder where the tool can save temporary data
        /// </summary>
        public string WorkingPath
        {
            set { _workingPath = value; }
        }

        #endregion
    }
}
