using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
namespace VisionProcessing
{
    public class Intersection
    {
        private static int m_intTolerance = 0;
        
        public static bool CheckRectRectIntersection(Point[] arrPoints1, Point[] arrPoints2, int intTolerance)
        {
            //arrPoints Index : 0=TopLeft , 1=TopRight , 2=BottomRight , 3=BottomLeft
            m_intTolerance = intTolerance;
            //
            // Check Rect1 edges
            // 
            if (DoAxisSeparationTest(arrPoints1[0], arrPoints1[1], arrPoints1[2], arrPoints2))
            {
                return false;
            }

            if (DoAxisSeparationTest(arrPoints1[0], arrPoints1[3], arrPoints1[2], arrPoints2))
            {
                return false;
            }

            if (DoAxisSeparationTest(arrPoints1[2], arrPoints1[3], arrPoints1[0], arrPoints2))
            {
                return false;
            }

            if (DoAxisSeparationTest(arrPoints1[2], arrPoints1[1], arrPoints1[0], arrPoints2))
            {
                return false;
            }

            //
            // Check Rect2 edges
            // 
            if (DoAxisSeparationTest(arrPoints2[0], arrPoints2[1], arrPoints2[2], arrPoints1))
            {
                return false;
            }

            if (DoAxisSeparationTest(arrPoints2[0], arrPoints2[3], arrPoints2[2], arrPoints1))
            {
                return false;
            }

            if (DoAxisSeparationTest(arrPoints2[2], arrPoints2[3], arrPoints2[0], arrPoints1))
            {
                return false;
            }

            if (DoAxisSeparationTest(arrPoints2[2], arrPoints2[1], arrPoints2[0], arrPoints1))
            {
                return false;
            }

            // If found no separating axis, then the quadrilaterals intersect.
            return true;
        }

        /// <summary>
        /// Does axis separation test for a convex quadrilateral.
        /// </summary>
        /// <param name="x1">Defines together with x2 the edge of quad1 to be checked whether its a separating axis.</param>
        /// <param name="x2">Defines together with x1 the edge of quad1 to be checked whether its a separating axis.</param>
        /// <param name="x3">One of the remaining two points of quad1.</param>
        /// <param name="otherQuadPoints">The four points of the other quad.</param>
        /// <returns>Returns <c>true</c>, if the specified edge is a separating axis (and the quadrilaterals therefor don't 
        /// intersect). Returns <c>false</c>, if it's not a separating axis.</returns>
        private static bool DoAxisSeparationTest(Point x1, Point x2, Point x3, Point[] otherQuadPoints)
        {
            //Point[] otherPoints = new Point[4];
            //otherPoints[0] = new Point(otherQuadPoints[0].X - m_intTolerance, otherQuadPoints[0].Y - m_intTolerance);
            //otherPoints[1] = new Point(otherQuadPoints[1].X + m_intTolerance, otherQuadPoints[1].Y - m_intTolerance);
            //otherPoints[2] = new Point(otherQuadPoints[2].X + m_intTolerance, otherQuadPoints[2].Y + m_intTolerance);
            //otherPoints[3] = new Point(otherQuadPoints[3].X - m_intTolerance, otherQuadPoints[3].Y + m_intTolerance);

            Vector vec = x2 - x1;
            Vector rotated = new Vector(-vec.Y, vec.X); //perpendicular vector
            
            bool refSide = (rotated.X * (x3.X - x1.X)
                          + rotated.Y * (x3.Y - x1.Y)) >= 0;

            foreach (Point pt in otherQuadPoints)//otherPoints
            {
                bool side = (rotated.X * (pt.X - x1.X)
                           + rotated.Y * (pt.Y - x1.Y)) >= 0;
                if (side == refSide)
                {
                    // At least one point of the other quad is one the same side as x3. Therefor the specified edge can't be a
                    // separating axis anymore.
                    return false;
                }
            }

            // All points of the other quad are on the other side of the edge. Therefor the edge is a separating axis and
            // the quads don't intersect.
            return true;
        }
    }
}
