using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace LIN_ALG_TEST
{
    // TODO: add comments; NORMAL XFORM
    public class AffineTransform2 : MatrixTransform2
    {
        public AffineTransform2()
        {
            for (int i = 0; i < 3; i++)
            {
                mat[i, i] = 1.0d;
            }
        }
        public override String TransformName()
        {
            return "Affine";
        }

        public static AffineTransform2 RotateXY(double angle)
        {
            AffineTransform2 T = new AffineTransform2();
            T.mat[0, 0] = Math.Cos(angle);
            T.mat[1, 1] = T.mat[0, 0];
            T.mat[1, 0] = Math.Sin(angle);
            T.mat[0, 1] = -T.mat[1, 0];
            return T;
        }

        public static AffineTransform2 Translate(Vector v)
        {
            AffineTransform2 T = new AffineTransform2();
            T.mat[0, 2] = v.X;
            T.mat[1, 2] = v.Y;
            return T;
        }

        public static AffineTransform2 Translate(Point p, Point q)
        {
            return Translate(q - p);
        }

        public static AffineTransform2 AxisScale(double xamount, double yamount)
        {
            AffineTransform2 T = new AffineTransform2();
            T.mat[0, 0] = xamount;
            T.mat[1, 1] = yamount;
            T.mat[2, 2] = 1.0d;
            return T;
        }

        public static AffineTransform2 RotateAboutPoint(Point p, double angle)
        {
            Point origin = new Point();
            AffineTransform2 T1 = Translate(origin - p);
            AffineTransform2 T2 = RotateXY(angle);
            AffineTransform2 T3 = Translate(p - origin);
            return T3 * T2 * T1;
        }

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

        public static AffineTransform2 PointsAndVectorToPointsAndVector(
            Point p1, Point p2, Vector v1,
                Point q1, Point q2, Vector w1)
        {
            Vector v2 = p2 - p1;
            Vector w2 = q2 - q1;

            return AffineTransform2.PointAndVectorsToPointAndVectors(p1, v1, v2, q1, w1, w2);
        }

        public AffineTransform2 InverseTransform()
        {
            double[,] m = MatrixInverse(mat);
            AffineTransform2 T = new AffineTransform2();
            T.mat = m;
            return T;
        }

        public static AffineTransform2 operator *(AffineTransform2 T1, AffineTransform2 T2)
        {
            AffineTransform2 S = new AffineTransform2();
            S.mat = MatrixProduct(T1.mat, T2.mat);
            return S;
        }

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
