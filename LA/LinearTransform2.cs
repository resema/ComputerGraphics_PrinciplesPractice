using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using MathNet.Numerics;

namespace LIN_ALG_TEST
{
    // TODO: add comments
    public class LinearTransform2 : MatrixTransform2
    {
        public LinearTransform2()
        {
            for (int i = 0; i < 3; i++)
            {
                mat[i, i] = 1.0d;
            }
        }

        public LinearTransform2(Vector v, Vector w)
        {
            mat[0, 0] = v.X; mat[1, 0] = v.Y;
            mat[0, 1] = w.X; mat[1, 1] = w.Y;
            mat[2, 2] = 1.0d;
        }
        public override String TransformName()
        {
            return "Linear";
        }

        public static LinearTransform2 VectorsToVectors(Vector v1, Vector v2, Vector w1, Vector w2)
        {
            LinearTransform2 V = new LinearTransform2(v1, v2);
            LinearTransform2 W = new LinearTransform2(w1, w2);
            return W * V.InverseTransform();
        }

        public static LinearTransform2 RotateXY(double angle)
        {
            LinearTransform2 T = new LinearTransform2();
            T.mat[0, 0] = Math.Cos(angle);
            T.mat[1, 1] = T.mat[0, 0];
            T.mat[1, 0] = Math.Sin(angle);
            T.mat[0, 1] = -T.mat[1, 0];
            return T;
        }

        public LinearTransform2 InverseTransform()
        {
            double[,] m = MatrixInverse(mat);
            LinearTransform2 T = new LinearTransform2();
            T.mat = m;
            return T;
        }

        public static LinearTransform2 operator *(LinearTransform2 T1, LinearTransform2 T2)
        {
            LinearTransform2 S = new LinearTransform2();
            S.mat = MatrixProduct(T1.mat, T2.mat);
            return S;
        }

        public static Vector operator *(LinearTransform2 T, Vector v)
        {
            double[] vv = new double[2];
            vv[0] = v.X;
            vv[1] = v.Y;
            double[] res = new double[2];

            for (int i = 0; i < 2; i++)
            {
                double tally = 0.0d;
                for (int k = 0; k < 2; k++)
                {
                    tally += T.mat[i, k] * vv[k];
                }
                res[i] = tally;

            }
            return new Vector(res[0], res[1]);

        }

        public double Det()
        {
            return Determinant(mat);
        }

        private static double Determinant(double[,] mat)
        {
            return mat[0, 0] * mat[1, 1] - mat[0, 1] * mat[1, 0];
        }

        public double Trace()
        {
            return mat[0, 0] + mat[1, 1];
        }

        new private static double[,] MatrixProduct(double[,] mat1, double[,] mat2)
        {
            double[,] result = new double[3, 3];
            result[2, 2] = 1.0d;

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    double tally = 0.0d;
                    for (int k = 0; k < 2; k++)
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
            double[,] result = new double[3, 3];
            result[2, 2] = 1.0d;

            double d = Determinant(mat);
            result[0, 0] = mat[1, 1] / d;
            result[1, 1] = mat[0, 0] / d;
            result[0, 1] = -mat[0, 1] / d;
            result[1, 0] = -mat[1, 0] / d;
            return result;
        }

    }

}
