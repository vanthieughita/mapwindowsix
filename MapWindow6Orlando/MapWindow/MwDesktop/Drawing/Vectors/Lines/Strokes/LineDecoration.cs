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
// The Initial Developer of this Original Code is Ted Dunsford. Created 4/9/2009 4:53:01 PM
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using MapWindow.Main;
using MapWindow.Serialization;

namespace MapWindow.Drawing
{


    /// <summary>
    /// LineDecoration
    /// </summary>
    [Serializable]
    public class LineDecoration : Descriptor, ILineDecoration
    {
        #region Private Variables

        private IPointSymbolizer _symbol;
        private bool _flipFirst;
        private bool _flipAll;
        private bool _rotateWithLine;
        private int _numSymbols;
        private double _offset;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of LineDecoration
        /// </summary>
        public LineDecoration()
        {
            _symbol = new PointSymbolizer(Global.RandomColor(), PointShapes.Triangle, 10);
            _flipAll = false;
            _flipFirst = true;
            _rotateWithLine = true;
            _numSymbols = 2;
        }

        #endregion

        #region Methods

      

        /// <summary>
        /// Given the points on this line decoration, this will cycle through and handle
        /// the drawing as dictated by this decoration.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="path"></param>
        /// <param name="scaleWidth">The double scale width for controling markers</param>
        public void Draw(Graphics g, GraphicsPath path, double scaleWidth)
        {
            if (NumSymbols == 0) return;      
            GraphicsPathIterator myIterator = new GraphicsPathIterator(path);
            myIterator.Rewind();
            int start, end;
            bool isClosed;
            Size2D symbolSize = _symbol.GetSize();
            Bitmap symbol = new Bitmap((int)symbolSize.Width, (int)symbolSize.Height);
            Graphics sg = Graphics.FromImage(symbol);
            _symbol.Draw(sg, new Rectangle(0, 0, (int)symbolSize.Width, (int)symbolSize.Height));
            sg.Dispose();


            Matrix oldMat = g.Transform;
            PointF[] points;
            if (path.PointCount == 0) return;
            try
            {
                points = path.PathPoints;
            }
            catch
            {
                return;
            }
            PointF offset;

            int count = 0;
            while (myIterator.NextSubpath(out start, out end, out isClosed) > 0)
            {
                count = count + 1;
                // First marker
                PointF startPoint = points[start];
                PointF stopPoint = points[start + 1];
                float angle = 0F;
                if(_rotateWithLine)angle = GetAngle(startPoint, stopPoint);
                if (FlipFirst && !FlipAll) FlipAngle(ref angle);
                offset = GetOffset(startPoint, stopPoint);
                startPoint = new PointF(startPoint.X + offset.X, startPoint.Y + offset.Y);
                Matrix rotated = g.Transform;
                rotated.RotateAt(angle, startPoint);
                g.Transform = rotated;
                DrawImage(g, startPoint, symbol);
                g.Transform = oldMat;
               
                // Second marker
                if (NumSymbols > 1)
                {
                    angle = 0F;
                    if (_rotateWithLine) angle = GetAngle(points[end-1], points[end]);
                    if (FlipAll) FlipAngle(ref angle);
                    offset = GetOffset(points[end-1], points[end]);
                    PointF endPoint = new PointF(points[end].X + offset.X, points[end].Y + offset.Y);
                    rotated = g.Transform;
                    rotated.RotateAt(angle, endPoint);
                    g.Transform = rotated;
                    DrawImage(g, endPoint, symbol);
                    g.Transform = oldMat;
                }
                if (NumSymbols > 2)
                {
                    double totalLength = GetLength(points, start, end);
                    double span = totalLength / (NumSymbols-1);
                    for (int i = 1; i < NumSymbols - 1; i++)
                    {
                        DecorationSpot spot = GetPosition(points, span * i, start, end);
                        angle = 0F;
                        if (_rotateWithLine) angle = GetAngle(spot.Before, spot.After);
                        offset = GetOffset(spot.Before, spot.After);
                        PointF location = new PointF(spot.Position.X + offset.X, spot.Position.Y + offset.Y);
                        if (FlipAll) FlipAngle(ref angle);
                        rotated = g.Transform;
                        rotated.RotateAt(angle, location);
                        g.Transform = rotated;
                        DrawImage(g, location, symbol);
                        g.Transform = oldMat;
                    }
                }
            }
  
        }



        private struct DecorationSpot
        {
            public PointF Position;
            public PointF Before;
            public PointF After;
        }

        /// <summary>
        /// Given the current point, this uses the vector pointing to the next point in the line
        /// to determine the direction of "left" and applies the offset in that direction.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="nextPoint"></param>
        /// <returns></returns>
        private PointF GetOffset(PointF point, PointF nextPoint)
        {
            double dx = nextPoint.X - (double)point.X;
            double dy = nextPoint.Y - (double)point.Y;
            double len = Math.Sqrt(dx * dx + dy * dy);
            dx = dx / len;
            dy = dy / len;
            if (dx * dy < 0)
            {
                // Quadrants 1 or 2
                return new PointF((float)(_offset * dy), (float)(_offset * dx));
            }
            return new PointF((float)(-_offset * dy), (float)(-_offset * dx));
        }

        private static DecorationSpot GetPosition(PointF[] points, double pathLength, int start, int end)
        {
            double lenSqr = 0;
            double pathLenSqr = pathLength * pathLength;
            DecorationSpot result = new DecorationSpot();
            for (int i = start; i < end; i++)
            {
                double dx = points[i + 1].X - points[i].X;
                double dy = points[i + 1].Y - points[i].Y;
                lenSqr += dx * dx + dy * dy;
                if (pathLenSqr >= lenSqr) continue;
                double segLen = Math.Sqrt(dx * dx + dy * dy);
                double backLen = Math.Sqrt(lenSqr) - Math.Sqrt(pathLenSqr);
                double rat = backLen/segLen;
                float x = (float)(points[i + 1].X - rat * dx);
                float y = (float)(points[i + 1].Y - rat * dy);
                    
                result.Position = new PointF(x, y);
                result.Before = points[i];
                result.After = points[i + 1];
                return result;
            }
            return result;
        }

        private static double GetLength(PointF[] points, int start, int end)
        {
            double lenSqr = 0;
            for (int i = start; i < end; i++)
            {
                double dx = points[i + 1].X - points[i].X;
                double dy = points[i + 1].Y - points[i].Y;
                lenSqr += dx * dx + dy * dy;
            }
            return Math.Sqrt(lenSqr);
        }

        private static void DrawImage(Graphics g, PointF center, Image image)
        {

            PointF topLeft = center;
            topLeft.X -= (float)image.Width / 2;
            topLeft.Y -= (float)image.Height / 2;
            g.DrawImage(image, topLeft);
        }

        private static void  FlipAngle(ref float angle)
        {
            angle = angle + 180;
            if (angle > 360) angle -= 360;
        }
     

        // Given a the start point and end point, find the angle
        private static float GetAngle(PointF start, PointF end)
        {
           
            double dx = end.X - start.X;
            double dy = end.Y - start.Y;
            if (dx == 0 || dy == 0) return 0;
            double angle = Math.Atan(dy / dx);
            return (float)(angle * 360 / Math.PI);
        }


        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the decorative symbol
        /// </summary>
		[Serialize("Symbol")]
		public IPointSymbolizer Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        /// <summary>
        /// Gets or sets a boolean that, if true, flips the first symbol in relation
        /// to the direction of the line.
        /// </summary>
		[Serialize("FlipFirst")]
		public bool FlipFirst
        {
            get { return _flipFirst; }
            set { _flipFirst = value; }
        }

        /// <summary>
        /// Gets or sets a boolean that, if true, reverses all of the symbols
        /// </summary>
		[Serialize("FlipAll")]
		public bool FlipAll
        {
            get { return _flipAll; }
            set { _flipAll = value; }
        }

        /// <summary>
        /// Gets or sets a boolean that, if true, will cause the symbol to
        /// be rotated according to the direction of the line.  Arrows
        /// at the ends, for instance, will point along the direction of
        /// the line, regardless of the direction of the line.
        /// </summary>
		[Serialize("RotateWithLine")]
		public bool RotateWithLine
        {
            get { return _rotateWithLine; }
            set { _rotateWithLine = value; }
        }

        /// <summary>
        /// Gets or sets the number of symbols that should be drawn on each
        /// line.  (not each segment).
        /// </summary>
		[Serialize("NumSymbols")]
		public int NumSymbols
        {
            get { return _numSymbols; }
            set { _numSymbols = value; }
        }

        /// <summary>
        /// Gets or sets the offset distance measured to the left of the line in pixels.
        /// </summary>
		[Serialize("Offset")]
		public double Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }
   

        #endregion

        #region Protected Methods

        /// <summary>
        /// Handles the creation of random content for the LineDecoration.
        /// </summary>
        /// <param name="generator">The Random class that generates random numbers</param>
        protected override void OnRandomize(Random generator)
        {
            base.OnRandomize(generator);
            // _symbol is randomizable so the base method already will have randomized this class
            _flipAll = generator.NextBool();
            _flipFirst = generator.NextBool();
            _rotateWithLine = generator.NextBool();
            _numSymbols = generator.Next(0, 10);
            _offset = generator.NextDouble() * 10;
        }

        #endregion

    }
}
