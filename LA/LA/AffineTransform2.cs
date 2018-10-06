using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace LIN_ALG
{
    public class AffineTransform2 : MatrixTransform2
    {
        /// <summary>
        /// Construct the identity transformation
        /// </summary>
        public AffineTransform2()
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
        public static AffineTransform2 RotateXY(double angle)
        {
            AffineTransform2 T = new AffineTransform2();
            T.mat[0, 0] = Math.Cos(angle);
            T.mat[1, 1] = T.mat[0, 0];
            T.mat[1, 0] = Math.Sin(angle);
            T.mat[0, 1] = -T.mat[1, 0];
            return T;
        }

        /// <summary>
        /// Construct a translation that displaces any point by the amount specified by the vector "v" 
        /// <param name="v">The displacement vector</param>
        /// <returns>The translation transform. </returns>
        /// </summary>
        public static AffineTransform2 Translate(Vector v)
        {
            AffineTransform2 T = new AffineTransform2();
            T.mat[0, 2] = v.X;
            T.mat[1, 2] = v.Y;
            return T;
        }

        /// <summary>
        /// Construct a translation that displaces the Point p to the Point q 
        /// <param name="P">A point that will be translated.</param>
        /// <param name="Q">The point where P will end up after translation.</param>
        /// <returns>The translation transform. </returns>
        /// </summary>
        public static AffineTransform2 Translate(Point p, Point q)
        {
            return Translate(q - p);
        }

        /// <summary>
        /// Construct a scaling transformation of the form (x, y) -> (ax, by)
        /// <param name="xamount">The multiplier for x-coordinates.</param>
        /// <param name="yamount">The multiplier for y-coordinates.</param>
        /// <returns>The scaling transform. </returns>
        /// </summary>
        public static AffineTransform2 AxisScale(double xamount, double yamount)
        {
            AffineTransform2 T = new AffineTransform2();
            T.mat[0, 0] = xamount;
            T.mat[1, 1] = yamount;
            T.mat[2, 2] = 1.0d;
            return T;
        }

        /// <summary>
        /// Construct a rotation by amount "angle" around the point "p". The resulting transformation leaves "p" unmoved. 
        /// <param name="p">The center point for the rotation.</param>
        /// <param name="angle">The rotation angle, in radians.</param>
        /// <returns>The rotation transform. </returns>
        /// </summary>
        public static AffineTransform2 RotateAboutPoint(Point p, double angle)
        {
            Point origin = new Point();
            AffineTransform2 T1 = Translate(origin - p);
            AffineTransform2 T2 = RotateXY(angle);
            AffineTransform2 T3 = Translate(p - origin);
            return T3 * T2 * T1;
        }

        /// <summary>
        /// Build the unique transformation taking three independent points to any three other points.  
        /// <param name="p1">The 1st point to be moved</param>
        /// <param name="p2">The 2nd point to be moved</param>
        /// <param name="p3">The 3rd point to be moved</param>
        /// <param name="q1">The point to which p1 will be moved</param>
        /// <param name="q2">The point to which p2 will be moved</param>
        /// <param name="q3">The point to which p3 will be moved</param>
        /// <returns>The affine transform. </returns>
        /// </summary>
        public static AffineTransform2 PointsToPoints(
            Point p1, Point p2, Point p3,
                Point q1, Point q2, Point q3)
        {
            Vector v1 = p2 - p1;
            Vector v2 = p3 - p1;
            Vector w1 = q2 - q1;
            Vector w2 = q3 - q1;

            return AffineTransform2.PointAndVectorsToPointAndVectors(p1, v1, v2, q1, w1, w2);
        }

        /// <summary>
        /// Given a point p1 and linearly independent vectors v1 and v2, build the unique transformation taking p1 to q1, v1 to w1, and v2 to w2. 
        /// <param name="p1">The point to be moved</param>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <param name="q1">The point to which p1 will be moved</param>
        /// <param name="w1">The vector to which v1 will be moved</param>
        /// <param name="w2">The vector to which v2 will be moved</param>
        /// <returns>The affine transform. </returns>
        /// </summary>
        public static AffineTransform2 PointAndVectorsToPointAndVectors(
            Point p1, Vector v1, Vector v2,
                Point q1, Vector w1, Vector w2)
        {
            AffineTransform2 Trans1 = AffineTransform2.Translate(p1, new Point(0,0));
            LinearTransform2 T = LinearTransform2.VectorsToVectors(v1, v2, w1, w2);
            AffineTransform2 Trans2 = AffineTransform2.Translate(new Point(0,0), q1);
            AffineTransform2 S = new AffineTransform2();
            S.mat = T.Matrix();

            return Trans2 * S * Trans1;
            /*
            double[,] linmat = T.Matrix();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    W.mat[i, j] = linmat[i, j];
                }
            }
            return W;
             */
        }

        /// <summary>
        /// Given a point p and linearly independent vectors v1 and v2, build the unique transformation taking p to q, v1 to w1, and v2 to w2. 
        /// <param name="p1">The point to be moved</param>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <param name="q1">The point to which p1 will be moved</param>
        /// <param name="w1">The vector to which v1 will be moved</param>
        /// <param name="w2">The vector to which v2 will be moved</param>
        /// <returns>The affine transform. </returns>
        /// </summary>
        public static AffineTransform2 PointsAndVectorToPointsAndVector(
            Point p1, Point p2, Vector v1,
                Point q1, Point q2, Vector w1)
        {
            Vector v2 = p2 - p1;
            Vector w2 = q2 - q1;

            return AffineTransform2.PointAndVectorsToPointAndVectors(p1, v1, v2, q1, w1, w2);
        }

        /// <summary>
        /// Return the inverse of this transformation, if it's invertible. 
        /// <returns>The inverse transform. </returns>
        /// </summary>
        public AffineTransform2 InverseTransform()
        {
            double[,] m = MatrixInverse(mat);
            AffineTransform2 T = new AffineTransform2();
            T.mat = m;
            return T;
        }

        /// <summary>
        /// Return the composition T2 o T1 of two transformations, T1 and T2.
        /// <remarks>Note the order: In the composition, T1 is performed first, then T2 afterwards!</remarks>
        /// <returns>The composite transform. </returns>
        /// </summary>
        public static AffineTransform2 operator *(AffineTransform2 T1, AffineTransform2 T2)
        {
            AffineTransform2 S = new AffineTransform2();
            S.mat = MatrixProduct(T1.mat, T2.mat);
            return S;
        }

        /// <summary>
        /// Apply the transform T to the point P, returning a new point.  
        /// <param name="T">An affine transformation of the plane </param>
        /// <param name="p">A point of the plane </param>       
        /// <returns> The point T(P) </returns>
        /// </summary>
        public static Vector operator *(AffineTransform2 T, Point p)
        {
            double[] vv = new double[3];
            vv[0] = p.X;
            vv[1] = p.Y;
            vv[2] = 1.0d;
            double[] res = new double[2];

            for (int i = 0; i < 2; i++)
            {
                double tally = 0.0d;
                for (int k = 0; k < 3; k++)
                {
                    tally += T.mat[i, k] * vv[k];
                }
                res[i] = tally;

            }
            return new Vector(res[0], res[1]);

        }

        /// <summary>
        /// Apply the transform T to the vector v, returning a new vector.  
        /// <param name="T">An affine transformation of the plane </param>
        /// <param name="p">A vector of the plane </param>       
        /// <returns> The vector T(v) </returns>
        /// <remarks> T(v) denotes the value of the associated vector-transformation on the vector v</remarks>
        /// </summary>
        public static Vector operator *(AffineTransform2 T, Vector v)
        {
            double[] vv = new double[3];
            vv[0] = v.X;
            vv[1] = v.Y;
            vv[2] = 0.0d;
            double[] res = new double[2];

            for (int i = 0; i < 2; i++)
            {
                double tally = 0.0d;
                for (int k = 0; k < 3; k++)
                {
                    tally += T.mat[i, k] * vv[k];
                }
                res[i] = tally;

            }
            return new Vector(res[0], res[1]);
        }


        /// <summary>
        /// Compute the product of two matrices. 
        /// <param name="mat1">A 3 x 3 matrix </param>
        /// <param name="mat2">A 3 x 3 matrix </param>
        /// <returns> The matrix mat1 * mat2 </returns>
        /// </summary>
        new private static double[,] MatrixProduct(double[,] mat1, double[,] mat2)
        {
            double[,] result = new double[3, 3];
            result[2, 2] = 1.0d;

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double tally = 0.0d;
                    for (int k = 0; k < 3; k++)
                    {
                        tally += mat1[i, k] * mat2[k, j];
                    }
                    result[i, j] = tally;
                }
            }
            return result;
        }

        /// <summary>
        /// Compute the inverse of a 3 x 3 matrix. 
        /// <param name="mat">A 3 x 3 matrix </param>
        /// <returns> The inverse of matrix mat1</returns>
        /// </summary>
        new private static double[,] MatrixInverse(double[,] mat)
        {
            double[] translation = new double[3];
            translation[0] = mat[0, 2];
            translation[1] = mat[1, 2];
            translation[2] = 1.0d;

            double[,] mm = new double[3, 3];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    mm[i, j] = mat[i, j];
                }
            }
            mm[2, 2] = 1.0d;

            double[,] minv = LinearTransform2.MatrixInverse(mm);
            double[] reverseTranslation = new double[3];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    reverseTranslation[i] -= minv[i, j]*translation[j];
                }
            }
            reverseTranslation[2] = 1.0d;


            double[,] res = new double[3, 3];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    res[i, j] = minv[i, j];
                }
                res[i, 2] = reverseTranslation[i];
            }
            res[2, 2] = 1.0d;
            return res;
        }
    }
}
