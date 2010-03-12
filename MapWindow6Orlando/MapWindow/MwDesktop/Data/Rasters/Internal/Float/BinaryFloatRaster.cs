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
// The Initial Developer of this Original Code is Ted Dunsford. Created 2/19/2008 10:34:52 AM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************

using System;
using System.IO;
using System.Text;
using MapWindow.Main;
using MapWindow.Projections;

namespace MapWindow.Data
{
    /// <summary>
    /// BinaryFloatRaster
    /// </summary>
    internal class BinaryFloatRaster : FloatRaster
    {
        #region Private Variables

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a completely empty raster that can be custom configured
        /// </summary>
        public BinaryFloatRaster()
        {
            // This is basically an empty place holder 
        }


        /// <summary>
        /// Creates a new BinaryIntRaster with the specified rows and columns.
        /// If if the raster is less than 64 Million cells, it will be created only in memory,
        /// and a Save method should be called when ready to save it to a file.  Otherwise, it creates a blank file with
        /// NoData values...which start out as 0.
        /// </summary>
        /// <param name="filename">The filename to write to</param>
        /// <param name="numRows">Integer number of rows</param>
        /// <param name="numColumns">Integer number of columns</param>
        public BinaryFloatRaster(string filename, int numRows, int numColumns) :
            this(filename, numRows, numColumns, true)
        {
            // this just forces the inRam to default to true.
        }

        /// <summary>
        /// Creates a new BinaryIntRaster with the specified rows and columns.
        /// If inRam is true and the raster is less than 64 Million cells, it will be created only in memory,
        /// and a Save method should be called when ready to save it to a file.  Otherwise, it creates a blank file with
        /// NoData values.
        /// </summary>
        /// <param name="filename">The filename to write to</param>
        /// <param name="numRows">Integer number of rows</param>
        /// <param name="numColumns">Integer number of columns</param>
        /// <param name="inRam">If this is true and the raster is small enough, it will load this into memory and not save anything to the file.</param>
        public BinaryFloatRaster(string filename, int numRows, int numColumns, bool inRam)
        {
            // Since we know that we are working with the integer type, we can specify an integer calculator here.

            if (File.Exists(filename))
                if (OnFileExists(filename)) return; // user cancelled
            base.Filename = filename;

            if (inRam  && numColumns*numRows < 64000000)
                base.IsInRam = true;
            else
                base.IsInRam = false;
            base.NumRows = numRows;
            base.NumColumns = numColumns;
            Initialize();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a duplicate version of this file.  If copyValues is set to false, then a raster of NoData values is created
        /// that has the same georeferencing information as the source file of this Raster, even if this raster is just a window.
        /// If the specified filename exists, rather than throwing an exception or taking an "overwrite" parameter, this
        /// will throw the FileExists event, and cancel the copy if the cancel argument is set to true.
        /// </summary>
        /// <param name="filename">The string filename specifying where to create the new file.</param>
        /// <param name="copyValues">If this is false, the same size and georeferencing values are used, but they are all set to NoData.</param>
        public override void Copy(string filename, bool copyValues)
        {
            if (filename == Filename)
                throw new ArgumentException(MessageStrings.CannotCopyToSelf_S.Replace("%S", filename));
            if (File.Exists(filename))
            {
                if (OnFileExists(filename))
                    return; // The copy event was cancelled
                // The copy event was not cancelled, so overwrite the file
                File.Delete(filename);
            }
            if (copyValues)
            {
                // this should be faster than copying values in code
                File.Copy(Filename, filename);
            }
            else
            {
                // since at this point, there is no file, a blank file will be created with empty values.
                Write(filename);
            }
        }

        /// <summary>
        /// This creates a completely new raster from the windowed domain on the original raster.  This new raster
        /// will have a separate source file, and values like NumRowsInFile will correspond to the newly created file.
        /// All the values will be copied to the new source file.  If inRam = true and the new raster is small enough,
        /// the raster values will be loaded into memory.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="startRow">The 0 based integer index of the top row to copy from this raster.  If this raster is itself a window, 0 represents the startRow from the file.</param>
        /// <param name="endRow">The integer index of the bottom row to copy from this raster.  The largest allowed value is NumRows - 1.</param>
        /// <param name="startColumn">The 0 based integer index of the leftmost column to copy from this raster.  If this raster is a window, 0 represents the startColumn from the file.</param>
        /// <param name="endColumn">The 0 based integer index of the rightmost column to copy from this raster.  The largest allowed value is NumColumns - 1</param>
        /// <param name="copyValues">If this is true, the valeus are saved to the file.  If this is false and the data can be loaded into Ram, no file handling is done.  Otherwise, a file of NoData values is created.</param>
        /// <param name="inRam">Boolean.  If this is true and the window is small enough, a copy of the values will be loaded into memory.</param>
        /// <returns>An implementation of IRaster</returns>
        public new IRaster CopyWindow(string filename, int startRow, int endRow, int startColumn, int endColumn,
                                  bool copyValues, bool inRam)
        {
            int numCols = endColumn - startColumn + 1;
            int numRows = endRow - startRow + 1;


            BinaryFloatRaster result = new BinaryFloatRaster(filename, numCols, numRows, inRam);

            result.Projection = Projection;

            // The affine coefficients defining the world file are the same except that they are translated over.  Only the position of the
            // upper left corner changes.  Everything else is the same as the previous raster.

            // X = [0] + [1] * column + [2] * row;
            // Y = [3] + [4] * column + [5] * row;
            result.Bounds = new RasterBounds(result.NumRows, result.NumColumns, new double[6]);
            result.Bounds.AffineCoefficients[0] = Bounds.AffineCoefficients[0] +
                                                  Bounds.AffineCoefficients[1]*startColumn +
                                                  Bounds.AffineCoefficients[2]*startRow;
            result.Bounds.AffineCoefficients[1] = Bounds.AffineCoefficients[1];
            result.Bounds.AffineCoefficients[2] = Bounds.AffineCoefficients[2];
            result.Bounds.AffineCoefficients[3] = Bounds.AffineCoefficients[3] +
                                                  Bounds.AffineCoefficients[4]*startColumn +
                                                  Bounds.AffineCoefficients[5]*startRow;
            result.Bounds.AffineCoefficients[4] = Bounds.AffineCoefficients[4];
            result.Bounds.AffineCoefficients[5] = Bounds.AffineCoefficients[5];

            if (IsInRam)
            {
                ProgressMeter pm = new ProgressMeter(ProgressHandler, MessageStrings.CopyingValues, numRows);
                // copy values directly using both data structures
                for (int row = 0; row < numRows; row++)
                {
                    for (int col = 0; col < numCols; col++)
                    {
                        result.Data[row][col] = Data[startRow + row][startColumn + col];
                    }
                    pm.CurrentValue = row;
                }
                pm.Reset();

                if (result.IsInRam == false)
                {
                    // Force the result raster to write itself to a file and then purge its memory.
                    result.Write(filename);
                    result.Data = null;
                }
            }
            else
            {
                if (result.IsInRam)
                {
                    // the source is not in memory, so we just read the values from the file as if opening it directly from the file.
                    result.OpenWindow(Filename, startRow, endRow, startColumn, endColumn, true);
                }
                else
                {
                    // Both sources are file based so we basically copy rows of bytes from one to the other.
                    FileStream source = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                    result.WriteHeader(filename);
                    FileStream dest = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.None);
                    source.Seek(HeaderSize, SeekOrigin.Begin);
                    BinaryReader bReader = new BinaryReader(source);
                    BinaryWriter bWriter = new BinaryWriter(dest);
                    ProgressMeter pm = new ProgressMeter(ProgressHandler, MessageStrings.CopyingValues, numRows);
                    // copy values directly using both data structures
                    source.Seek(NumColumnsInFile*startRow*ByteSize, SeekOrigin.Current);
                    for (int row = 0; row < numRows; row++)
                    {
                        source.Seek(numCols*ByteSize, SeekOrigin.Current);
                        byte[] rowData = bReader.ReadBytes(ByteSize*numCols);
                        bWriter.Write(rowData);
                        source.Seek(NumColumnsInFile - endColumn + 1, SeekOrigin.Current);
                        bWriter.Flush();
                        pm.CurrentValue = row;
                    }
                    pm.Reset();
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the statistics for the entire file, not just the window portion specified for this raster.
        /// </summary>
        public override void GetStatistics()
        {
            if (IsInRam && this.IsFullyWindowed())
            {
                // The in-memory version of this is a little faster, so use it, but only if we can.
                base.GetStatistics();
                return;
            }
            // If we get here, we either need to check the file because no data is in memory or because
            // the window that is in memory does not have all the values.

            FileStream fs = new FileStream(Filename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(HeaderSize, SeekOrigin.Begin);
            ProgressMeter pm = new ProgressMeter(ProgressHandler,
                                                 "Calculating Statistics for the entire raster " + Filename,
                                                 NumRowsInFile);
            float min = float.MaxValue;
            float max = float.MinValue;

            double total = 0;
            double sqrTotal = 0;
            int count = 0;

            for (int row = 0; row < NumRowsInFile; row++)
            {
                for (int col = 0; col < NumColumnsInFile; col++)
                {
                    float val = br.ReadSingle();

                    if (val != FloatNoDataValue)
                    {
                        if (val > max) max = val;
                        if (val < min) min = val;
                        double dblVal = val;
                        total += dblVal;
                        sqrTotal += dblVal*dblVal;
                        count++;
                    }
                }
                pm.CurrentValue = row;
            }
            Minimum = min;
            Maximum = max;
            NumValueCells = count;
            StdDeviation = (float) Math.Sqrt((sqrTotal/NumValueCells) - (total/NumValueCells)*(total/NumValueCells));
            br.Close();
        }

        /// <summary>
        /// This creates a window from this raster.  The window will still save to the same
        /// source file, but only has access to a small window of data, so it can be loaded like a buffer.
        /// The georeferenced extents will be for the new window, not the original raster.  startRow and endRow
        /// will exist in the new raster, however, so that it knows how to copy values back to the original raster.
        /// </summary>
        /// <param name="startRow">The 0 based integer index of the top row to get from this raster.  If this raster is itself a window, 0 represents the startRow from the file.</param>
        /// <param name="endRow">The integer index of the bottom row to get from this raster.  The largest allowed value is NumRows - 1.</param>
        /// <param name="startColumn">The 0 based integer index of the leftmost column to get from this raster.  If this raster is a window, 0 represents the startColumn from the file.</param>
        /// <param name="endColumn">The 0 based integer index of the rightmost column to get from this raster.  The largest allowed value is NumColumns - 1</param>
        /// <param name="inRam">Boolean.  If this is true and the window is small enough, a copy of the values will be loaded into memory.</param>
        /// <returns>An implementation of IRaster</returns>
        public new IRaster GetWindow(int startRow, int endRow, int startColumn, int endColumn, bool inRam)
        {
            int numCols = endColumn - startColumn + 1;
            int numRows = endRow - startRow + 1;
            BinaryFloatRaster result = new BinaryFloatRaster();
            result.Filename = Filename;
            result.Projection = Projection;
            result.NumRows = endRow - startRow + 1;
            result.NumColumns = endColumn - startColumn + 1;
            result.NumRowsInFile = NumRowsInFile;
            result.NumColumnsInFile = NumColumnsInFile;
            result.NoDataValue = NoDataValue;
            result.StartColumn = startColumn + StartColumn;
            result.StartRow = startRow + StartRow;
            result.EndColumn = endColumn + StartColumn;
            result.EndRow = EndRow + StartRow;


            // Reposition the "raster" so that it matches the window, not the whole raster
            // X = [0] + [1] * column + [2] * row;
            // Y = [3] + [4] * column + [5] * row;
            result.Bounds = new RasterBounds(result.NumRows, result.NumColumns, new double[6]);
            result.Bounds.AffineCoefficients[0] = Bounds.AffineCoefficients[0] +
                                                  Bounds.AffineCoefficients[1]*startColumn +
                                                  Bounds.AffineCoefficients[2]*startRow;
            result.Bounds.AffineCoefficients[1] = Bounds.AffineCoefficients[1];
            result.Bounds.AffineCoefficients[2] = Bounds.AffineCoefficients[2];
            result.Bounds.AffineCoefficients[3] = Bounds.AffineCoefficients[3] +
                                                  Bounds.AffineCoefficients[4]*startColumn +
                                                  Bounds.AffineCoefficients[5]*startRow;
            result.Bounds.AffineCoefficients[4] = Bounds.AffineCoefficients[4];
            result.Bounds.AffineCoefficients[5] = Bounds.AffineCoefficients[5];

            result.Data = new float[numRows][];


            // Now we can copy any values currently in memory.
            if (IsInRam)
            {
                //result.ReadHeader(Filename);
                result.Data = new float[numRows][];
                ProgressMeter pm = new ProgressMeter(ProgressHandler, MessageStrings.CopyingValues, endRow);
                pm.StartValue = startRow;
                // copy values directly using both data structures
                for (int row = 0; row < numRows; row++)
                {
                    result.Data[row] = new float[numCols];
                    for (int col = 0; col < numCols; col++)
                    {
                        result.Data[row][col] = Data[startRow + row][startColumn + col];
                    }
                    pm.CurrentValue = row;
                }
                pm.Reset();
            }
            else
                result.OpenWindow(Filename, startRow, endRow, startColumn, endColumn, inRam);
            result.Value = new FloatValueGrid(result);
            return result;
        }


        /// <summary>
        /// Obtains only the statistics for the small window specified by startRow, endRow etc.
        /// </summary>
        public new void GetWindowStatistics()
        {
            if (IsInRam)
            {
                // don't bother to do file calculations if the whole raster is in memory
                base.GetWindowStatistics();
                return;
            }

            // The window was not in memory, so go ahead and get statistics for the window from the file.

            FileStream fs = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read, NumColumns*ByteSize);
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(HeaderSize, SeekOrigin.Begin);
            ProgressMeter pm = new ProgressMeter(ProgressHandler,
                                                 "Calculating Statistics for the entire raster " + Filename, NumRows);


            double total = 0;
            double sqrTotal = 0;
            int count = 0;
            int byteSize = ByteSize; // cache this for faster calcs
            float min = float.MaxValue;
            float max = float.MinValue;
            fs.Seek(StartRow*ByteSize*NumColumnsInFile, SeekOrigin.Current); // To top edge of the Window


            for (int row = 0; row < NumRows; row++)
            {
                fs.Seek(StartColumn*byteSize, SeekOrigin.Current); // to the left edge of the window
                for (int col = 0; col < NumColumns; col++)
                {
                    float val = br.ReadSingle();

                    if (val != FloatNoDataValue)
                    {
                        if (val > max) max = val;
                        if (val < min) min = val;
                        double dblVal = val;
                        total += dblVal;
                        sqrTotal += dblVal*dblVal;
                        count++;
                    }
                }
                fs.Seek(NumColumnsInFile - EndRow - 1, SeekOrigin.Current); // skip to the end of this row.
                pm.CurrentValue = row;
            }
            Minimum = min;
            Maximum = max;
            NumValueCells = count;
            StdDeviation = (float) Math.Sqrt((sqrTotal/NumValueCells) - (total/NumValueCells)*(total/NumValueCells));
            br.Close();
        }

        /// <summary>
        /// Opens the specified file into this raster.
        /// </summary>
        /// <param name="filename">The string filename to open</param>
        public override void Open(string filename)
        {
            Open(filename, true);
        }

        /// <summary>
        /// Opens a new instance of the BinaryRaster
        /// </summary>
        /// <param name="filename">The string filename of the raster to open</param>
        /// <param name="inRam">Boolean, if true this will attempt to load all the values into memory</param>
        public virtual void Open(string filename, bool inRam)
        {
            Filename = filename;
            ReadHeader(filename);
            if (inRam)
            {
                if (NumRowsInFile*NumColumnsInFile < 64000000)
                {
                    IsInRam = true;
                    Read();
                }
            }
            Value = new FloatValueGrid(this);
        }

        /// <summary>
        /// This converts this object into a raster defined by the specified window dimensions.
        /// </summary>
        /// <param name="filename">The string filename to open</param>
        /// <param name="startRow">The integer row index to become the first row to load into this raster.</param>
        /// <param name="endRow">The 0 based integer row index to become the last row included in this raster.</param>
        /// <param name="startColumn">The 0 based integer column index for the first column of the raster.</param>
        /// <param name="endColumn">The 0 based integer column index for the last column to include in this raster.</param>
        /// <param name="inRam">Boolean.  If this is true and the window is small enough, this will load the window into ram.</param>
        public virtual void OpenWindow(string filename, int startRow, int endRow, int startColumn, int endColumn,
                                       bool inRam)
        {
            Filename = filename;
            ReadHeader(filename);
            NumRows = endRow - startRow + 1;
            NumColumns = endColumn - startColumn + 1;


            StartColumn = startColumn;
            StartRow = startRow;
            EndColumn = endColumn;
            EndRow = EndRow;

            // Reposition the "raster" so that it matches the window, not the whole raster
            // X = [0] + [1] * column + [2] * row;
            // Y = [3] + [4] * column + [5] * row;

            // Translation only needs to change two coefficients
            Bounds.AffineCoefficients[0] = Bounds.AffineCoefficients[0] + Bounds.AffineCoefficients[1]*startColumn +
                                           Bounds.AffineCoefficients[2]*startRow;

            Bounds.AffineCoefficients[3] = Bounds.AffineCoefficients[3] + Bounds.AffineCoefficients[4]*startColumn +
                                           Bounds.AffineCoefficients[5]*startRow;


            if (inRam)
            {
                if (NumRows*NumColumns < 64000000)
                {
                    IsInRam = true;
                    Read();
                }
            }


            Value = new FloatValueGrid(this);
        }

        /// <summary>
        /// Copies the contents from the specified sourceRaster into this sourceRaster.  If both rasters are InRam, this does not affect the files.
        /// </summary>
        /// <param name="sourceRaster">The raster of values to paste into this raster.  If the CellWidth and CellHeight values do not match between the files,
        /// an exception will be thrown.  If the sourceRaster overlaps with the edge of this raster, only the intersecting region will be
        /// pasted.</param>
        /// <param name="startRow">Specifies the row in this raster where the top row of the sourceRaster will be pasted </param>
        /// <param name="startColumn">Specifies the column in this raster where the left column of the sourceRaster will be pasted.</param>
        public void PasteRaster(IRaster sourceRaster, int startRow, int startColumn)
        {
            const int byteSize = 4;

            if (sourceRaster.DataType != typeof (float))
            {
                throw new ArgumentException(
                    MessageStrings.ArgumentOfWrongType_S1_S2.Replace("%S1", "sourceRaster").Replace("%S2",
                                                                                                    "BinaryFloatRaster"));
            }
            FloatRaster sourceRasterT = (FloatRaster) sourceRaster;
            if (startRow + sourceRaster.NumRows <= 0) return; // sourceRaster is above this raster
            if (startColumn + sourceRaster.NumColumns <= 0) return; // sourceRaster is left of this raster
            if (startRow > NumRows) return; // sourceRaster is below this raster
            if (startColumn > NumColumns) return; // sourceRaster is to the right of this raster
            if (sourceRaster.CellWidth != CellWidth || sourceRaster.CellHeight != CellHeight)
                throw new ArgumentException(MessageStrings.RastersNeedSameCellSize);

            // These are specified in coordinates that match the source raster
            int sourceStartColumn = 0;
            int sourceStartRow = 0;
            int destStartColumn = startColumn;
            int destStartRow = startRow;

            int numPasteColumns = sourceRaster.NumColumns;
            int numPasteRows = sourceRaster.NumRows;


            // adjust range to cover only the overlapping sections
            if (startColumn < 0)
            {
                sourceStartColumn = -startColumn;
                destStartColumn = 0;
            }
            if (startRow < 0)
            {
                sourceStartRow = -startRow;
                destStartRow = 0;
            }

            if (numPasteRows + destStartRow > NumRows) numPasteRows = (NumRows - destStartRow);
            if (numPasteColumns + destStartColumn > NumColumns) numPasteColumns = (NumColumns - destStartRow);


            if (IsInRam) // ---------------------- RAM BASED ------------------------------------------------------
            {
                if (sourceRaster.IsInRam)
                {
                    // both members are inram, so directly copy values.
                    for (int row = 0; row < numPasteRows; row++)
                    {
                        for (int col = 0; col < numPasteColumns; col++)
                        {
                            // since we are copying direct, we don't have to do a type check on T 
                            Data[destStartRow + row][destStartColumn + col] =
                                sourceRasterT.Data[sourceStartRow + row][sourceStartColumn + col];
                        }
                    }
                }
                else
                {
                    FileStream fs = new FileStream(sourceRaster.Filename, FileMode.Open, FileAccess.Write,
                                                   FileShare.None, (numPasteColumns)*byteSize);
                    ProgressMeter pm = new ProgressMeter(ProgressHandler,
                                                         MessageStrings.ReadingValuesFrom_S.Replace("%S",
                                                                                                    sourceRaster.
                                                                                                        Filename),
                                                         numPasteRows);
                    fs.Seek(HeaderSize, SeekOrigin.Begin);

                    // Position the binary reader at the top of the "sourceRaster"
                    fs.Seek(sourceStartRow*sourceRaster.NumColumnsInFile*byteSize, SeekOrigin.Current);
                    BinaryReader br = new BinaryReader(fs);


                    for (int row = 0; row < numPasteRows; row++)
                    {
                        // Position the binary reader at the beginning of the sourceRaster
                        fs.Seek(byteSize*sourceStartColumn, SeekOrigin.Current);

                        for (int col = 0; col < numPasteColumns; col++)
                        {
                            Data[destStartRow + row][destStartColumn + col] = br.ReadSingle();
                        }
                        pm.CurrentValue = row;
                        fs.Seek(byteSize*(NumColumnsInFile - sourceStartColumn - numPasteColumns), SeekOrigin.Current);
                    }


                    br.Close();
                }
                // The statistics will have changed with the newly pasted data involved
                GetStatistics();
            }
            else // ----------------------------------------- FILE BASED ---------------------------------
            {
                FileStream writefs = new FileStream(Filename, FileMode.Open, FileAccess.Write, FileShare.None,
                                                    NumColumns*byteSize);
                BinaryWriter bWriter = new BinaryWriter(writefs);

                ProgressMeter pm = new ProgressMeter(ProgressHandler,
                                                     MessageStrings.WritingValues_S.Replace("%S", Filename),
                                                     numPasteRows);


                writefs.Seek(HeaderSize, SeekOrigin.Begin);
                writefs.Seek(destStartRow*NumColumnsInFile*byteSize, SeekOrigin.Current);
                    // advance to top of paste window area

                if (sourceRaster.IsInRam)
                {
                    // we can just write values

                    for (int row = 0; row < numPasteColumns; row++)
                    {
                        // Position the binary reader at the beginning of the sourceRaster
                        writefs.Seek(byteSize*destStartColumn, SeekOrigin.Current);

                        for (int col = 0; col < numPasteColumns; col++)
                        {
                            float val = sourceRasterT.Data[sourceStartRow + row][sourceStartColumn + col];
                            bWriter.Write(val);
                        }
                        pm.CurrentValue = row;
                        writefs.Seek(byteSize*(NumColumnsInFile - destStartColumn - numPasteColumns), SeekOrigin.Current);
                    }
                }
                else
                {
                    // Since everything is handled from a file, we don't have to type check.  Just copy the bytes.

                    FileStream readfs = new FileStream(sourceRaster.Filename, FileMode.Open, FileAccess.Read,
                                                       FileShare.Read, numPasteColumns*byteSize);
                    BinaryReader bReader = new BinaryReader(readfs);
                    readfs.Seek(HeaderSize, SeekOrigin.Begin);
                    readfs.Seek(sourceStartRow*sourceRaster.NumColumnsInFile*byteSize, SeekOrigin.Current);
                        // advances to top of paste window area

                    for (int row = 0; row < numPasteRows; row++)
                    {
                        readfs.Seek(sourceStartColumn*byteSize, SeekOrigin.Current);
                        writefs.Seek(destStartColumn*byteSize, SeekOrigin.Current);
                        byte[] rowData = bReader.ReadBytes(numPasteColumns*byteSize);
                        bWriter.Write(rowData);
                        readfs.Seek(sourceRaster.NumColumnsInFile - sourceStartColumn - numPasteColumns,
                                    SeekOrigin.Current);
                        writefs.Seek(NumColumnsInFile - destStartColumn - numPasteColumns, SeekOrigin.Current);
                    }
                    bReader.Close();
                }
                bWriter.Close();
            }
        }

        /// <summary>
        /// Reads the the contents for the "window" specified by the start and end values
        /// for the rows and columns.
        /// </summary>
        public void Read()
        {
            FileStream fs = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read, NumColumns*ByteSize);
            ProgressMeter pm = new ProgressMeter(ProgressHandler,
                                                 MessageStrings.ReadingValuesFrom_S.Replace("%S", Filename), NumRows);
            fs.Seek(HeaderSize, SeekOrigin.Begin);

            // Position the binary reader at the top of the "window"
            fs.Seek(StartRow*NumColumnsInFile*ByteSize, SeekOrigin.Current);
            BinaryReader br = new BinaryReader(fs);

            double total = 0;
            double sqrTotal = 0;
            Data = new float[NumRows][];


            float min = float.MaxValue;
            float max = float.MinValue;

            for (int row = 0; row < NumRows; row++)
            {
                Data[row] = new float[NumColumns];


                // Position the binary reader at the beginning of the window
                fs.Seek(4*StartColumn, SeekOrigin.Current);

                for (int col = 0; col < NumColumns; col++)
                {
                    float val = br.ReadSingle();

                    Data[row][col] = val;

                    if (val != FloatNoDataValue)
                    {
                        if (val > max) max = val;
                        if (val < min) min = val;
                        total += val;
                        sqrTotal += val*val;
                        NumValueCells++;
                    }
                }
                pm.CurrentValue = row;
                fs.Seek(ByteSize*(NumColumnsInFile - EndColumn - 1), SeekOrigin.Current);
            }
            Maximum = max;
            Minimum = min;


            StdDeviation = Math.Sqrt((sqrTotal/NumValueCells) - (total/NumValueCells)*(total/NumValueCells));

            br.Close();
        }

        /// <summary>
        /// Writes the header, regardless of which subtype of binary raster this is written for
        /// </summary>
        /// <param name="filename">The string filename specifying what file to load</param>
        public void ReadHeader(string filename)
        {
            // Our older formats used ASCII encoding for the characters, not Unicode.  The difference is that in the old fashion method,
            // of ASCII, each character is a single byte.  In Unicode, it is two bytes.  if we write Future binary handlers, however, they should use unicode,
            // not ASCII, and strings should just use the converter which has a string length followed by the unicode strings.
            BinaryReader br = new BinaryReader(new FileStream(filename, FileMode.Open));
            StartColumn = 0;
            NumColumns = br.ReadInt32();
            NumColumnsInFile = NumColumns;
            EndColumn = NumColumns - 1;
            StartRow = 0;
            NumRows = br.ReadInt32();
            NumRowsInFile = NumRows;
            EndRow = NumRows - 1;
            Bounds = new RasterBounds(NumRows, NumColumns, new[] {0.0, 1.0, 0.0, NumRows, 0.0, -1.0});
            CellWidth = br.ReadDouble();
            Bounds.AffineCoefficients[5] = -br.ReadDouble(); // dy
            Xllcenter = br.ReadDouble();
            Yllcenter = br.ReadDouble();
            RasterDataTypes dataType = (RasterDataTypes) br.ReadInt32();
            if (dataType != RasterDataTypes.SINGLE)
            {
                throw new ArgumentException(
                    MessageStrings.ArgumentOfWrongType_S1_S2.Replace("%S1", filename).Replace("%S2", "BinaryFloatRaster"));
            }
            FloatNoDataValue = br.ReadSingle();

            string proj = Encoding.ASCII.GetString(br.ReadBytes(255)).Replace('\0', ' ').Trim();
            Projection = new ProjectionInfo();
            Projection.ReadProj4String(proj);
            Notes = Encoding.ASCII.GetString(br.ReadBytes(255)).Replace('\0', ' ').Trim();
            if (Notes.Length == 0) Notes = null;
            br.Close();
        }

        /// <summary>
        /// Read optimization for "not" in ram files.  This only buffers a little bit, and will drastically improve performance, without loading
        /// the whole grid at a time.  For an average 3 x 3 filter, you have to read 9 times to write once.  It only makes sense to buffer
        /// values when reading because it takes about the same amount of time to read a whole row as it does to read one value.
        /// </summary>
        /// <param name="row">Specifies the row in the file to read.  This uses the file row index, not any windowed value.</param>
        public override float[] ReadRow(int row)
        {
            FileStream fs = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read,
                                           NumColumnsInFile*ByteSize);
            fs.Seek(HeaderSize, SeekOrigin.Begin);
            fs.Seek(row*NumColumnsInFile*ByteSize, SeekOrigin.Current);
            BinaryReader br = new BinaryReader(fs);
            float[] result = new float[NumColumnsInFile];

            for (int col = 0; col < NumColumnsInFile; col++)
            {
                result[col] = br.ReadSingle();
            }


            br.Close();
            return result;
        }

        /// <summary>
        /// Saves the current memory content to the file.
        /// </summary>
        public override void Save()
        {
            Write(Filename);
        }

        /// <summary>
        /// The string filename where this will begin to write data by clearing the existing file
        /// </summary>
        /// <param name="filename">a filename to write data to</param>
        public void WriteHeader(string filename)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(filename, FileMode.OpenOrCreate));
            bw.Write(NumColumnsInFile);
            bw.Write(NumRowsInFile);
            bw.Write(CellWidth);
            bw.Write(CellHeight);
            bw.Write(Xllcenter);
            bw.Write(Yllcenter);
            bw.Write((int) RasterDataTypes.SINGLE);
            bw.Write(FloatNoDataValue);

            // These are each 255 bytes because they are ASCII encoded, not the standard DotNet Unicode

            byte[] proj = new byte[255];
            if (Projection != null)
            {
                byte[] temp = Encoding.ASCII.GetBytes(Projection.ToProj4String());
                int len = Math.Min(temp.Length, 255);
                for (int i = 0; i < len; i++)
                {
                    proj[i] = temp[i];
                }
            }
            bw.Write(proj);
            byte[] note = new byte[255];
            if (Notes != null)
            {
                byte[] temp = Encoding.ASCII.GetBytes(Notes);
                int len = Math.Min(temp.Length, 255);
                for (int i = 0; i < len; i++)
                {
                    note[i] = temp[i];
                }
            }
            bw.Write(note);
            bw.Close();
        }

        /// <summary>
        /// This would be a horrible choice for any kind of serious process, but is provided as 
        /// a way to write values directly to the file.
        /// </summary>
        /// <param name="row">The 0 based integer row index for the file to write to.</param>
        /// <param name="column">The 0 based column index for the file to write to.</param>
        /// <param name="value">The actual value to write.</param>
        public override void WriteValue(int row, int column, float value)
        {
            FileStream fs = new FileStream(Filename, FileMode.Open, FileAccess.Write, FileShare.None);
            fs.Seek(HeaderSize, SeekOrigin.Begin);
            fs.Seek(row*NumColumnsInFile*ByteSize, SeekOrigin.Current);
            fs.Seek(column*ByteSize, SeekOrigin.Current);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(value);
            bw.Close();
        }

        /// <summary>
        /// If no file exists, this writes the header and no-data values.  If a file exists, it will assume
        /// that data already has been filled in the file and will attempt to insert the data values
        /// as a window into the file.  If you want to create a copy of the file and values, just use
        /// System.IO.File.Copy, it almost certainly would be much more optimized.
        /// </summary>
        /// <param name="filename">The string filename to write values to.</param>
        public void Write(string filename)
        {
            FileStream fs;
            BinaryWriter bw;
            ProgressMeter pm = new ProgressMeter(ProgressHandler, "Writing values to " + filename, NumRows);
            long expectedByteCount = NumRows*NumColumns*ByteSize;
            if (expectedByteCount < 1000000) pm.StepPercent = 5;
            if (expectedByteCount < 5000000) pm.StepPercent = 10;
            if (expectedByteCount < 100000) pm.StepPercent = 50;

            if (File.Exists(filename))
            {
                FileInfo fi = new FileInfo(filename);
                // if the following test fails, then the target raster doesn't fit the bill for pasting into, so clear it and write a new one.
                if (fi.Length == HeaderSize + ByteSize*NumColumnsInFile*NumRowsInFile)
                {
                    WriteHeader(filename);
                    // assume that we already have a file set up for us, and just write the window of values into the appropriate place.
                    fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None,
                                        ByteSize*NumColumns);
                    fs.Seek(HeaderSize, SeekOrigin.Begin);
                    fs.Seek(ByteSize*StartRow, SeekOrigin.Current);
                    bw = new BinaryWriter(fs); // encoding doesn't matter because we don't have characters


                    for (int row = 0; row < NumRows; row++)
                    {
                        fs.Seek(StartColumn*ByteSize, SeekOrigin.Current);
                        for (int col = 0; col < NumColumns; col++)
                        {
                            // this is the only line that is type dependant, but I don't want to type check on every value
                            bw.Write(Data[row][col]);
                        }
                        fs.Flush(); // Since I am buffering, make sure that I write the buffered data before seeking
                        fs.Seek((NumColumnsInFile - EndColumn - 1)*ByteSize, SeekOrigin.Current);
                        pm.CurrentValue = row;
                    }


                    pm.Reset();
                    bw.Close();
                    return;
                }
            }

            if (File.Exists(filename)) File.Delete(filename);
            WriteHeader(filename);

            // Open as append and it will automatically skip the header for us.
            fs = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.None, ByteSize*NumColumnsInFile);
            bw = new BinaryWriter(fs);
            // the row and column counters here are relative to the whole file, not just the window that is currently in memory.
            pm.EndValue = NumRowsInFile;


            for (int row = 0; row < NumRowsInFile; row++)
            {
                for (int col = 0; col < NumColumnsInFile; col++)
                {
                    if (row < StartRow || row > EndRow || col < StartColumn || col > EndColumn)
                        bw.Write(FloatNoDataValue);
                    else
                        bw.Write(Data[row - StartRow][col - StartColumn]);
                }
                pm.CurrentValue = row;
            }


            fs.Flush(); // flush anything that hasn't gotten written yet.
            pm.Reset();
            bw.Close();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the size of T in bytes.  This should be overridden, but
        /// exists as a "just-in-case" implementation that works for structs,
        /// but definitely won't work correctly for objects.
        /// </summary>
        public override int ByteSize
        {
            get { return 4; }
        }

        /// <summary>
        /// All the binary rasters use the Binary file type
        /// </summary>
        public override RasterFileTypes FileType
        {
            get { return RasterFileTypes.BINARY; }
        }

        /// <summary>
        /// This is always 1 band
        /// </summary>
        public override int NumBands
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets the size of the header.  There is one no-data value in the header.
        /// </summary>
        public virtual int HeaderSize
        {
            get { return 554 + ByteSize; }
        }

        #endregion
    }
}