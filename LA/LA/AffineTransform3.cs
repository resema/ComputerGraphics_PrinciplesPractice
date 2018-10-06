using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;

namespace LIN_ALG
{
    /// <summary>
    /// A class for representing affine transformations of 3-space, and their associated transformations on 3-vectors
    /// </summary>
    public class AffineTransform3 : MatrixTransform3
    {
        /// <summary>
        /// Construct the identity transformation
        /// </summary>
        public AffineTransform3()
        {
            for (int i = 0; i < 3; i++)
            {
                mat[i, i] = 1.0d;
            }
        }
        /// <summary>
        /// The transform's name is "Affine"
        /// </summary>
        public override String TransformName()
        {
            return "Affine";
        }

        /// <summary>
        /// Construct a rotation that moves the positive X-axis towards the postive Y-axis by an amount "angle". 
        /// <param name="angle">The rotation amount, in radians</param>
        /// </summary>
        public static AffineTransform3 RotateXY(double angle)
        {
            return RotMat(0, 1, angle);
        }

        /// <summary>
        /// Construct a rotation matrix with cosines and sines in the rows/cols indicated by the "first" and "second" indices, 
        /// where the sinces nad cosines are of the specified angle.  
        /// <param name="first">The "first" row-column index</param>
        /// <param name="second">The "second" row-column index</param>
        /// <param name="angle">The rotation amount, in radians</param>
        /// <remarks> the [second, first] entry of the matrix is sin(angle). </remarks>
        /// </summary>
        private static AffineTransform3 RotMat(int first, int second, double angle)
        {
            AffineTransform3 T = new AffineTransform3();
            T.mat[first, first] = Math.Cos(angle);
            T.mat[second, second] = T.mat[first, first];
            T.mat[second, first] = Math.Sin(angle);
            T.mat[first, second] = -T.mat[second, first];
            return T;
        }

        /// <summary>
        /// Construct a rotation that moves the positive Y-axis towards the postive Z-axis by an amount "angle". 
        /// <param name="angle">The rotation amount, in radians</param>
        /// </summary>
        public static AffineTransform3 RotateYZ(double angle)
        {
            return RotMat(1, 2, angle);
        }

        /// <summary>
        /// Construct a rotation that moves the positive Z-axis towards the postive X-axis by an amount "angle". 
        /// <param name="angle">The rotation amount, in radians</param>
        /// </summary>
        public static AffineTransform3 RotateZX(double angle)
        {
            return RotMat(2, 0, angle);
        }

        /// <summary>
        /// Construct a scaling transformation of the form (x, y, z) -> (ax, by, cz)
        /// <param name="xscale">The multiplier for x-coordinates.</param>
        /// <param name="yscale">The multiplier for y-coordinates.</param>
        /// <param name="zscale">The multiplier for z-coordinates.</param>
        /// </summary>
        public static AffineTransform3 AxisScale(double xscale, double yscale, double zscale)
        {
            AffineTransform3 T = new AffineTransform3();
            T.mat[0, 0] = xscale;
            T.mat[1, 1] = yscale;
            T.mat[2, 2] = zscale;

            return T;
        }

        /// <summary>
        /// Construct a translation that displaces any point by the amount specified by the vector "v" 
        /// <param name="v">The displacement vector</param>
        /// </summary>
        public static AffineTransform3 Translate(Vector3D v)
        {
            AffineTransform3 T = new AffineTransform3();
            T.mat[0, 2] = v.X;
            T.mat[1, 2] = v.Y;
            return T;
        }

        /// <summary>
        /// Construct a translation that displaces the Point p to the Point q 
        /// <param name="P">A point that will be translated.</param>
        /// <param name="Q">The point where P will end up after translation.</param>
        /// </summary>
        public static AffineTransform3 Translate(Point3D p, Point3D q)
        {
            return Translate(q - p);
        }

        /// <summary>
        /// Build the unique transformation taking four independent points to any four other points.  
        /// <param name="p1">The 1st point to be moved</param>
        /// <param name="p2">The 2nd point to be moved</param>
        /// <param name="p3">The 3rd point to be moved</param>
        /// <param name="p4">The 4th point to be moved</param>
        /// <param name="q1">The point to which p1 will be moved</param>
        /// <param name="q2">The point to which p2 will be moved</param>
        /// <param name="q3">The point to which p3 will be moved</param>
        /// <param name="q4">The point to which p4 will be moved</param>
        /// </summary>
        public static AffineTransform3 PointsToPoints(
            Point3D p1, Point3D p2, Point3D p3, Point3D p4,
                Point3D q1, Point3D q2, Point3D q3, Point3D q4)
        {
            Vector3D v1 = p2 - p1;
            Vector3D v2 = p3 - p1;
            Vector3D v3 = p4 - p1;
            Vector3D w1 = q2 - q1;
            Vector3D w2 = q3 - q1;
            Vector3D w3 = q4 - q1;

            return AffineTransform3.PointAndVectorsToPointAndVectors(p1, v1, v2, v3, q1, w1, w2, w3);
        }

        /// <summary>
        /// Given a point p1 and linearly independent vectors v1, v2, and v3 build the unique transformation taking p1 to q1, v1 to w1,  v2 to w2, and v3 to w3. 
        /// <param name="p1">The point to be moved</param>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <param name="v3">The third vector</param>
        /// <param name="q1">The point to which p1 will be moved</param>
        /// <param name="w1">The vector to which v1 will be moved</param>
        /// <param name="w2">The vector to which v2 will be moved</param>
        /// <param name="w3">The vector to which v3 will be moved</param>
        /// </summary>
        public static AffineTransform3 PointAndVectorsToPointAndVectors(
            Point3D p1, Vector3D v1, Vector3D v2, Vector3D v3,
                Point3D q1, Vector3D w1, Vector3D w2, Vector3D w3)
        {
            AffineTransform3 Trans1 = AffineTransform3.Translate(p1, new Point3D(0,0, 0));
            LinearTransform3 T = LinearTransform3.VectorsToVectors(v1, v2, v3, w1, w2, w3);
            AffineTransform3 Trans2 = AffineTransform3.Translate(new Point3D(0,0,0), q1);
            AffineTransform3 S = new AffineTransform3();
            S.mat = T.Matrix();

            return Trans2 * S * Trans1;
        }

        /// <summary>
        /// Given two point p1 and p2, and linearly independent vectors v1, v2 whose span doesn't contain p2 - p1, build the transformation taking p1 to q1, p2 to q2, v1 to w1, and v2 to w2. 
        /// <param name="p1">The first point to be moved</param>
        /// <param name="p2">The second point to be moved</param>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <param name="q1">The point to which p1 will be moved</param>
        /// <param name="q2">The point to which p2 will be moved</param>
        /// <param name="w1">The vector to which v1 will be moved</param>
        /// <param name="w2">The vector to which v2 will be moved</param>
        /// </summary>
        public static AffineTransform3 PointsAndVectorsToPointsAndVectors(
            Point3D p1, Point3D p2, Vector3D v1, Vector3D v2,
                Point3D q1, Point3D q2, Vector3D w1, Vector3D w2)
        {
            Vector3D v3 = p2 - p1;
            Vector3D w3 = q2 - q1;

            return AffineTransform3.PointAndVectorsToPointAndVectors(p1, v1, v2, v3, q1, w1, w2, w3);
        }

        /// <summary>
        /// Given noncollinear points p1, p2, and p3, and a vector v1 outside the sapce of p2-p1 and p3-p1, build the transformation taking p1 to q1, p2 to q2, p3 to q3, and v1 to w1. 
        /// <param name="p1">The first point to be moved</param>
        /// <param name="p2">The second point to be moved</param>
        /// <param name="p3">The third point to be moved</param>
        /// <param name="v1">The first vector</param>
        /// <param name="q1">The point to which p1 will be moved</param>
        /// <param name="q2">The point to which p2 will be moved</param>
        /// <param name="q3">The point to which p3 will be moved</param>
        /// <param name="w1">The vector to which v1 will be moved</param>
        /// </summary>
        public static AffineTransform3 PointsAndVectorToPointsAndVector(
            Point3D p1, Point3D p2, Point3D p3, Vector3D v1,
                Point3D q1, Point3D q2, Point3D q3, Vector3D w1)
        {
            Vector3D v2 = p2 - p1;
            Vector3D w2 = q2 - q1;
            Vector3D v3 = p3 - p1;
            Vector3D w3 = q3 - q1;

            return AffineTransform3.PointAndVectorsToPointAndVectors(p1, v1, v2, v3, q1, w1, w2, w3);
        }

        /// <summary>
        /// Return the inverse of this transformation, if it's invertible. 
        /// </summary>
        public AffineTransform3 InverseTransform()
        {
            double[,] m = MatrixInverse(mat);
            AffineTransform3 T = new AffineTransform3();
            T.mat = m;
            return T;
        }

        /// <summary>
        /// Return the composition T2 o T1 of two transformations, T1 and T2.
        /// <remarks>Note the order: In the composition, T1 is performed first, then T2 afterwards!</remarks>
        /// </summary>
        public static AffineTransform3 operator *(AffineTransform3 T1, AffineTransform3 T2)
        {
            AffineTransform3 S = new AffineTransform3();
            S.mat = MatrixProduct(T1.mat, T2.mat);
            return S;
        }

        /// <summary>
        /// Apply the transform T to the point P, returning a new point.  
        /// <param name="T">An affine transformation of the plane </param>
        /// <param name="p">A point of the plane </param>       
        /// <returns> The point T(P) </returns>
        /// </summary>
        public static Vector3D operator *(AffineTransform3 T, Point3D p)
        {
            double[] vv = new double[4];
            vv[0] = p.X;
            vv[1] = p.Y;
            vv[2] = p.Z;
            vv[3] = 1.0d;
            double[] res = new double[3];

            for (int i = 0; i < 3; i++)
            {
                double tally = 0.0d;
                for (int k = 0; k < 4; k++)
                {
                    tally += T.mat[i, k] * vv[k];
                }
                res[i] = tally;
            }
            return new Vector3D(res[0], res[1], res[2]);
        }

        /// <summary>
        /// Apply the transform T to the vector v, returning a new vector.  
        /// <param name="T">An affine transformation of the plane </param>
        /// <param name="p">A vector of the plane </param>       
        /// <returns> The vector T(v) </returns>
        /// <remarks> T(v) denotes the value of the associated vector-transformation on the vector v</remarks>
        /// </summary>
        public static Vector3D operator *(AffineTransform3 T, Vector3D v)
        {
            double[] vv = new double[3];
            vv[0] = v.X;
            vv[1] = v.Y;
            vv[2] = v.Z;
            vv[3] = 0.0d;
            double[] res = new double[3];

            for (int i = 0; i < 3; i++)
            {
                double tally = 0.0d;
                for (int k = 0; k < 4; k++)
                {
                    tally += T.mat[i, k] * vv[k];
                }
                res[i] = tally;

            }
            return new Vector3D(res[0], res[1], res[2]);
        }

    }
}
