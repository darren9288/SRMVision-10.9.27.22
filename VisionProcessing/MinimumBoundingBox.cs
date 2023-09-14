using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
namespace VisionProcessing
{
    public class MinimumBoundingBox
    {
        private static float m_fMinBoxArea = 0;
        private static float m_fBoxArea = 0;

        public static List<PointF> CalculateMinimumBoundingBox(PointF[] arrPoints, ref float fAngle)
        {
            PointF[] arrHullPoints = MonotoneChainConvexHull(arrPoints);

            //check if no bounding box available
            if (arrHullPoints.Length <= 1)
                return arrHullPoints.ToList();

            PointF[] minBox = null;
            double dMinAngle = 0d;

            //foreach edge of the convex hull
            for (int i = 0; i < arrHullPoints.Length; i++)
            {
                int intNextIndex = i + 1;

                PointF pCurrentPoint = arrHullPoints[i];
                PointF pNextPoint = arrHullPoints[intNextIndex % arrHullPoints.Length];

                PointF pDelta = new PointF(pCurrentPoint.X - pNextPoint.X, pCurrentPoint.Y - pNextPoint.Y);

                //get angle of segment to x axis between two points
                double dAngle = -Math.Atan(pDelta.Y / pDelta.X);

                //min / max points
                float top = float.MinValue;
                float bottom = float.MaxValue;
                float left = float.MaxValue;
                float right = float.MinValue;

                //rotate every point and get min and max values for each direction
                foreach (PointF p in arrHullPoints)
                {
                    PointF rotatedPoint = RotateToXAxis(p, dAngle);
                    
                    top = Math.Max(top, rotatedPoint.Y);
                    bottom = Math.Min(bottom, rotatedPoint.Y);

                    left = Math.Min(left, rotatedPoint.X);
                    right = Math.Max(right, rotatedPoint.X);
                }

                //create axis aligned bounding box
                PointF[] box = RectPoints(new PointF(left, bottom), new PointF(right, top));

                if (minBox == null || m_fMinBoxArea > m_fBoxArea)
                {
                    minBox = box;
                    m_fMinBoxArea = m_fBoxArea;
                    dMinAngle = dAngle;
                }
            }


            //rotate axis algined box back
            List<PointF> minimalBoundingBox = new List<PointF>();

            minimalBoundingBox = minBox.Select(p => RotateToXAxis(p, -dMinAngle)).ToList();
            fAngle = (float)(dMinAngle * 180 / Math.PI);
            return minimalBoundingBox;
        }

        public static PointF[] MonotoneChainConvexHull(PointF[] arrPoints)
        {
            // 2020-12-17 ZJYEOH : Monotone Chain Convex Hull explanation
            // Sort all points based on x axis, will get Left Most Point
            // Then try to go clockwise until reach the right most point. These points will form upper hull. 
            // Then from the right most point, go clockwise and reach left most point.

            //Array.Sort<PointF>(arrPoints);
            List<PointF> sorted = arrPoints.ToList().OrderBy(point => point.X).ToList();

            arrPoints = sorted.ToArray();
            PointF[] arrHullPoints = new PointF[2 * arrPoints.Length];

            //break if only one point as input
            if (arrPoints.Length <= 1)
                return arrPoints;

            int intPointLength = arrPoints.Length;
            int intCounter = 0;

            //iterate for lowerHull
            for (var i = 0; i < intPointLength; ++i)
            {
                while (intCounter >= 2 && Cross(arrHullPoints[intCounter - 2],
                           arrHullPoints[intCounter - 1],
                           arrPoints[i]) <= 0)
                    intCounter--;
                arrHullPoints[intCounter++] = arrPoints[i];
            }

            //iterate for upperHull
            for (int i = intPointLength - 2, j = intCounter + 1; i >= 0; i--)
            {
                while (intCounter >= j && Cross(arrHullPoints[intCounter - 2],
                           arrHullPoints[intCounter - 1],
                           arrPoints[i]) <= 0)
                    intCounter--;
                arrHullPoints[intCounter++] = arrPoints[i];
            }

            //remove duplicate start points
            PointF[] result = new PointF[intCounter - 1];
            Array.Copy(arrHullPoints, 0, result, 0, intCounter - 1);
            return result;
        }

        /// <summary>
		/// Cross the specified o, a and b.
		/// Zero if collinear
		/// Positive if counter-clockwise
		/// Negative if clockwise
		/// </summary>
		/// <param name="o">O.</param>
		/// <param name="a">The alpha component.</param>
		/// <param name="b">The blue component.</param>
		public static double Cross(PointF o, PointF a, PointF b)
        {
            //2020-12-17 ZJYEOH : Cross Product formula
            //k = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);

            //if (k == 0): They are all colinear
            //if (k > 0) : They are all clockwise
            //if (k < 0) : They are counter clockwise
            return (a.X - o.X) * (b.Y - o.Y) - (a.Y - o.Y) * (b.X - o.X);
        }

        public static PointF RotateToXAxis(PointF p, double angle)
        {
            double newX = p.X * Math.Cos(angle) - p.Y * Math.Sin(angle);
            double newY = p.X * Math.Sin(angle) + p.Y * Math.Cos(angle);

            return new PointF((float)newX, (float)newY);
        }

        public static PointF[] RectPoints(PointF A, PointF B)
        {

            PointF Size = new PointF(B.X - A.X, B.Y - A.Y);
            m_fBoxArea = Size.X * Size.Y;
            return new PointF[] {
                    new PointF (A.X, A.Y),
                    new PointF (A.X + Size.X, A.Y),
                    new PointF (A.X + Size.X, A.Y + Size.Y),
                    new PointF (A.X, A.Y + Size.Y)
                };

        }

    }
}
