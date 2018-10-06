using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace LIN_ALG
{
    /// <summary>
    /// A class for representing projective transformations of the Euclidean plane using 3x3 matrices.
    /// </summary>
    public class ProjectiveTransform2 : MatrixTransform2
    {
        /// <summary>
        /// Construct the identity transformation
        /// </summary>
        public ProjectiveTransform2()
        {
            for (int i = 0; i < 3; i++)
            {
                mat[i, i] = 1.0d;
            }
        }
        /// <summary>
        /// The name for this class of transformations is "Projective"
        /// </summary>
        /// <returns>The String "Projective"</returns>
        public override String TransformName()
        {
            return "Projective";
        }


        /// <summary>
        /// Construct a rotation that moves the positive X-axis towards the postive Y-axis by an amount "angle". 
        /// <param name="angle">The rotation amount, in radians</param>
        /// </summary>
        public static ProjectiveTransform2 RotateXY(double angle)
        {
            AffineTransform2 T = AffineTransform2.RotateXY(angle);
            ProjectiveTransform2 T2 = new ProjectiveTransform2();
            T2.mat = T.mat;
            T.mat = null;
            return T2;
        }

        /// <summary>
        /// Construct a translation that displaces any point by the amount specified by the vector "v" 
        /// <param name="v">The displacement vector</param>
        /// <returns>The translation transform. </returns>
        /// </summary>
        public static ProjectiveTransform2 Translate(Vector v)
        {
            AffineTransform2 T = AffineTransform2.Translate(v);
            ProjectiveTransform2 T2 = new ProjectiveTransform2();
            T2.mat = T.mat;
            T.mat = null;
            return T2;
        }

        /// <summary>
        /// Construct a translation that displaces the Point p to the Point q 
        /// <param name="P">A point that will be translated.</param>
        /// <param name="Q">The point where P will end up after translation.</param>
        /// <returns>The translation transform. </returns>
        /// </summary>
        public static ProjectiveTransform2 Translate(Point p, Point q)
        {
            AffineTransform2 T = AffineTransform2.Translate(p, q);
            ProjectiveTransform2 T2 = new ProjectiveTransform2();
            T2.mat = T.mat;
            T.mat = null;
            return T2;
        }

        /// <summary>
        /// Construct a scaling transformation of the form (x, y) -> (ax, by)
        /// <param name="xamount">The multiplier for x-coordinates.</param>
        /// <param name="yamount">The multiplier for y-coordinates.</param>
        /// <returns>The scaling transform. </returns>
        /// <remarks>Note that if the two scale amounts are equal, the transformation has no effect, when regarded in homogeneous coordinates. </remarks>
        /// </summary>
        public static ProjectiveTransform2 AxisScale(double xamount, double yamount)
        {
            AffineTransform2 T = AffineTransform2.AxisScale(xamount, yamount);
            ProjectiveTransform2 T2 = new ProjectiveTransform2();
            T2.mat = T.mat;
            T.mat = null;
            return T2;
        }

        /// <summary>
        /// Construct a rotation by amount "angle" around the point "p". The resulting transformation leaves "p" unmoved. 
        /// <param name="p">The center point for the rotation.</param>
        /// <param name="angle">The rotation angle, in radians.</param>
        /// <returns>The rotation transform. </returns>
        /// </summary>
        public static ProjectiveTransform2 RotateAboutPoint(Point p, double angle)
        {
            Point origin = new Point();
            ProjectiveTransform2 T1 = Translate(origin - p);
            ProjectiveTransform2 T2 = RotateXY(angle);
            ProjectiveTransform2 T3 = Translate(p - origin);
            return T3 * T2 * T1;
        }

        /// <summary>
        /// Build the unique transformation taking four independent points to any four other points.  
        /// <param name="p0">The 0th point to be moved</param>
        /// <param name="p1">The 1st point to be moved</param>
        /// <param name="p2">The 2nd point to be moved</param>
        /// <param name="p3">The 3rd point to be moved</param>
        /// <param name="q0">The point to which p0 will be moved</param>
        /// <param name="q1">The point to which p1 will be moved</param>
        /// <param name="q2">The point to which p2 will be moved</param>
        /// <param name="q3">The point to which p3 will be moved</param>
        /// <returns>The projective transform. </returns>
        /// </summary>
        public static ProjectiveTransform2 PointsToPoints(
            Point p0, Point p1, Point p2, Point p3,
                Point q0, Point q1, Point q2, Point q3)
        {
            ProjectiveTransform2 Step1 = StandardFrameToPoints(p0, p1, p2, p3).InverseTransform();
            ProjectiveTransform2 Step2 = StandardFrameToPoints(q0, q1, q2, q3);
            return Step2 * Step1;
        }

        /// <summary>
        /// Build a transformation taking the standard frame (0,0), (1, 0), (0, 1), and (1, 1) to the points 
        /// p0, p1, p2, and p3. 
        /// </summary>
        /// <param name="p0">Where (0, 0) is sent</param>
        /// <param name="p1">Where (1, 0) is sent</param>
        /// <param name="p2">Where (0, 1) is sent</param>
        /// <param name="p3">Where (1, 1) is sent</param>
        /// <returns>The projective transformation effecting the specified mappings</returns>
        public static ProjectiveTransform2 StandardFrameToPoints(Point p0, Point p1, Point p2, Point p3)
        {
            // 
            ProjectiveTransform2 T = new ProjectiveTransform2();
            // idea: 
            // Send e1, e2, e3 to p0, p1, p2 by a map K. 
            // Let L be Kinverse.
            // Then L sends p0, p1, p2 to e1, e2 and e3 . See where p4 goes; call this q. 
            // build projective map P sending e1, e2, e3, and u= (e1+e2+e3) to e1, e2, e3, and q. 
            // then let L = Kinverse; K * P sends e1 to p1; e2 to p2; e3 to p3; and u to q to e4. 
            ProjectiveTransform2 K = new ProjectiveTransform2();
            for (int i = 0; i < 3; i++)
            {
                K.mat[2, i] = 1.0d;
            }
            K.mat[0, 0] = p0.X;
            K.mat[1, 0] = p0.Y;
            K.mat[0, 1] = p1.X;
            K.mat[1, 1] = p1.Y;
            K.mat[0, 2] = p2.X;
            K.mat[1, 2] = p2.Y;

            ProjectiveTransform2 L = new ProjectiveTransform2();
            L.mat = LinearTransform2.MatrixInverse(K.mat);
            double[] v = new double[3];
            v[0] = p3.X;
            v[1] = p3.Y;
            v[2] = 1.0d;

            double[] q = new double[3];
            for (int i = 0; i < 3; i++)
            {
                double tally = 0.0d;
                for (int j = 0; j < 3; j++)
                {
                    tally += L.mat[i, j] * v[j];
                }
                q[i] = tally;
            }
            double[,] p = new double[3, 3];
            for (int i = 0; i < 3; i++)
            {
                p[i, i] = q[i];
            }
            ProjectiveTransform2 S = new ProjectiveTransform2();
            S.mat = ProjectiveTransform2.MatrixProduct(K.mat, p);
            return S;
        }


        /// <summary>
        /// Return the inverse of this transformation, if it's invertible. 
        /// <returns>The inverse transform. </returns>
        /// </summary>
        public ProjectiveTransform2 InverseTransform()
        {
            double[,] m = MatrixInverse(mat);
            ProjectiveTransform2 T = new ProjectiveTransform2();
            T.mat = m;
            return T;
        }

        public static ProjectiveTransform2 operator *(ProjectiveTransform2 T1, ProjectiveTransform2 T2)
        {
            ProjectiveTransform2 S = new ProjectiveTransform2();
            S.mat = MatrixTransform2.MatrixProduct(T1.mat, T2.mat);
            return S;
        }

        /// <summary>
        /// Return the composition T2 o T1 of two transformations, T1 and T2.
        /// <remarks>Note the order: In the composition, T1 is performed first, then T2 afterwards!</remarks>
        /// <returns>The composite transform. </returns>
        /// </summary>
        public static Vector operator *(ProjectiveTransform2 T, Point p)
        {
            double[] vv = new double[3];
            vv[0] = p.X;
            vv[1] = p.Y;
            vv[2] = 1.0d;
            double[] res = new double[3];

            for (int i = 0; i < 3; i++)
            {
                double tally = 0.0d;
                for (int k = 0; k < 3; k++)
                {
                    tally += T.mat[i, k] * vv[k];
                }
                res[i] = tally;

            }
            return new Vector(res[0] / res[2], res[1] / res[2]);
        }
    }
}
