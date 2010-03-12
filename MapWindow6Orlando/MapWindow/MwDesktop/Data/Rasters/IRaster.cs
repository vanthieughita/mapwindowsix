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
// The Original Code is MapWindow.dll for the MapWindow 6.0 project
//
// The Initial Developer of this Original Code is Ted Dunsford. Created in January 2008.
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************
using System;
using System.Collections.Generic;
using MapWindow.Geometries;
using MapWindow.Main;
namespace MapWindow.Data
{
    /// <summary>
    /// A general 
    /// </summary>
   
    public interface IRaster : IDataSet, ICloneable
    {


        #region Methods

        /// <summary>
        /// This only works for in-ram rasters.  This basically makes a new raster that has all the same
        /// in memory values 
        /// </summary>
        /// <returns>An IRaster that is a duplicate of this class</returns>
        IRaster Copy();

        /// <summary>
        /// Creates a duplicate version of this file.  If copyValues is set to false, then a raster of NoData values is created
        /// that has the same georeferencing information as the source file of this Raster, even if this raster is just a window.
        /// </summary>
        /// <param name="filename">The string filename specifying where to create the new file.</param>
        /// <param name="copyValues">If this is false, the same size and georeferencing values are used, but they are all set to NoData.</param>
        void Copy(string filename, bool copyValues);

       

        /// <summary>
        /// Even if this is a window, this will cause this raster to show statistics calculated from the entire file.
        /// </summary>
        void GetStatistics();


        /// <summary>
        /// This method instructs the raster to read values from the file into memory.  
        /// </summary>
        void Open();
        
        /// <summary>
        /// Saves changes from any values that are in memory to the file.  This will preserve the existing
        /// structure and attempt to only write values to the parts of the file that match the loaded window.
        /// </summary>
        void Save();

        /// <summary>
        /// This will save whatever is specified in the startRow, endRow, startColumn, endColumn bounds
        /// to a new file with the specified filename.
        /// </summary>
        /// <param name="filename">The string filename to save the current raster to.</param>
        void SaveAs(string filename);

       
        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the RasterBounds class that contains georeference information.
        /// </summary>
        IRasterBounds Bounds
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the integer size of each data member of the raster in bytes.
        /// </summary>
        int ByteSize
        { 
            get;
        }

        /// <summary>
        /// Gets or sets the list of bands, which are in turn rasters.  The rasters
        /// contain only one band each, instead of the list of all the bands like the
        /// parent raster.
        /// </summary>
        IList<IRaster> Bands
        {
            get;
            set;
        }

        /// <summary>
        /// The geographic height of a cell the projected units.  Setting this will
        /// automatically adjust the affine coefficient to a negative value.
        /// </summary>
        double CellHeight
        {
            get;
            set;
          
        }


        /// <summary>
        /// The geographic width of a cell in the projected units
        /// </summary>
        double CellWidth
        {
            get;
            set;
        }

        /// <summary>
        /// This provides a zero-based integer band index that specifies which of the internal bands
        /// is currently being used for requests for data.
        /// </summary>
        int CurrentBand
        {
            get;
        }


        /// <summary>
        /// This does nothing unless the FileType property is set to custom.
        /// In such a case, this string allows new file types to be managed.
        /// </summary>
        string CustomFileType
        {
            get;
        }



        /// <summary>
        /// This reveals the underlying data type
        /// </summary>
        Type DataType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a short string to identify which driver to use.  This is primarilly used by GDAL rasters.
        /// </summary>
        string DriverCode
        {
            get; set;
        }

      

        /// <summary>
        /// The integer column index for the right column of this raster.  Most of the time this will 
        /// be NumColumns - 1.  However, if this raster is a window taken from a larger raster, then
        /// it will be the index of the endColumn from the window.
        /// </summary>
        int EndColumn
        {
            get;
        }


        /// <summary>
        /// The integer row index for the end row of this raster.  Most of the time this will 
        /// be numRows - 1.  However, if this raster is a window taken from a larger raster, then
        /// it will be the index of the endRow from the window.
        /// </summary>
        int EndRow
        {
            get;
        }

        

        /// <summary>
        /// Gets the complete path and filename of the current file
        /// </summary>
        string Filename
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the file type of this grid.
        /// </summary>
        RasterFileTypes FileType
        {
            get;
            set;
        }

       
        

        /// <summary>
        /// Gets a boolean that is true if the data for this raster is in memory.
        /// </summary>
        bool IsInRam
        {
            get;
        }

      

        /// <summary>
        /// Gets the maximum data value, not counting no-data values in the grid.
        /// </summary>
        double Maximum
        {
            get;
        }

        /// <summary>
        /// Gets the mean of the non-NoData values in this grid.  If the data is not InRam, then
        /// the GetStatistics method must be called before these values will be correct.
        /// </summary>
        double Mean
        {
            get;
        }

        /// <summary>
        /// Gets the minimum data value that is not classified as a no data value in this raster.
        /// </summary>
        double Minimum
        {
            get;
        }
       

        /// <summary>
        /// A float showing the no-data values
        /// </summary>
        double NoDataValue
        {
            get;
            set;
        }

       

        

        /// <summary>
        /// For binary rasters this will get cut to only 256 characters.
        /// </summary>
        string Notes
        {
            get;
            set;
        }


       

        /// <summary>
        /// Gets the number of bands.  In most traditional grid formats, this is 1.  For RGB images,
        /// this would be 3.  Some formats may have many bands.
        /// </summary>
        int NumBands
        {
            get;
        }

        /// <summary>
        /// Gets the horizontal count of the cells in the raster.
        /// </summary>
        int NumColumns
        {
            get;
        }
        
        /// <summary>
        /// Gets the integer count of columns in the file, as opposed to the 
        /// number represented by this raster, which might just represent a window.
        /// </summary>
        int NumColumnsInFile
        {
            get;
        }

        /// <summary>
        /// Gets the vertical count of the cells in the raster.
        /// </summary>
        int NumRows
        {
            get;
        }

        /// <summary>
        /// Gets the number of rows in the source file, as opposed to the number 
        /// represented by this raster, which might just represent part of a file.
        /// </summary>
        int NumRowsInFile
        {
            get;
        }


        /// <summary>
        /// Gets the count of the cells that are not no-data.  If the data is not InRam, then
        /// you will have to first call the GetStatistics() method to gain meaningul values.
        /// </summary>
        long NumValueCells
        {
            get;
        }

        /// <summary>
        /// Gets or sets the string array of options to use when saving this raster.
        /// </summary>
        string[] Options
        {
            get; set;
        }

        /// <summary>
        /// Gets the last progress handler that was set for this raster.
        /// </summary>
        IProgressHandler ProgressHandler
        {
            get;
            set;
        }


        /// <summary>
        /// Gets a list of the rows in this raster that can be accessed independantly.
        /// </summary>
        List<IValueRow> Rows
        {
            get;
        }





        /// <summary>
        /// The integer column index for the left column of this raster.  Most of the time this will 
        /// be 0.  However, if this raster is a window taken from a file, then
        /// it will be the row index in the file for the top row of this raster.
        /// </summary>
        int StartColumn
        {
            get;
        }

        /// <summary>
        /// The integer row index for the top row of this raster.  Most of the time this will 
        /// be 0.  However, if this raster is a window taken from a file, then
        /// it will be the row index in the file for the left row of this raster.
        /// </summary>
        int StartRow
        {
            get;
        }

        /// <summary>
        /// Gets the standard deviation of all the Non-nodata cells.  If the data is not InRam,
        /// then you will have to first call the GetStatistics() method to get meaningful values.
        /// </summary>
        double StdDeviation
        {
            get;
        }

        /// <summary>
        /// This is provided for future developers to link this raster to other entities.
        /// It has no function internally, so it can be manipulated safely.
        /// </summary>
        object Tag
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the value on the CurrentBand given a row and column undex
        /// </summary>
        IValueGrid Value
        {
            get;
            set;
        }

      


        /// <summary>
        /// The horizontal or longitude coordinate for the lower left cell in the grid
        /// </summary>
        double Xllcenter
        {
            get;
            set;
        }

        /// <summary>
        /// The vertical or latitude coordinate for the lower left cell in the grid
        /// </summary>
        double Yllcenter
        {
            get;
            set;
        }


       

       

        #endregion


        #region Events

        /// <summary>
        /// Occurs when attempting to copy or save to a filename that already exists.  A developer can tap into this event
        /// in order to display an appropriate message.  A cancel property allows the developer (and ultimately the user)
        /// decide if the specified event should ultimately be cancelled.
        /// </summary>
        event EventHandler<MessageCancel> FileExists;

       

        #endregion
    }
}
