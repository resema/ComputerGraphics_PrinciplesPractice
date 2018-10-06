using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using MathNet.Numerics.LinearAlgebra;

//
// Differential for Projective xform? Unhomogenized version for Projective xform?  
// 

namespace LIN_ALG_TEST
{
    public class MatrixTransform2
    {
        public double[,] mat = new double[3, 3];
        public const double EPSILON = 1E-9;

        protected MatrixTransform2()
        {
            for (int i = 0; i < 3; i++)
            {
                mat[i, i] = 1.0d;
            }
        }
        // public static virtual Point operator*(MatrixTransform2 T, Point p);
        // public static abstract Vector operator*(MatrixTransform2 T, Vector v);
        public double[,] Matrix()
        {
            return mat;
        }
        protected static double[,] MatrixInverse(double[,] mat)
        {
            Matrix m = new Matrix(mat);
            Matrix k = m.Inverse();
            return k;
        }
        public virtual String TransformName() {
            return "Matrix";
        }

        public override String ToString() {
            String s = TransformName() + " transform, matrix = \n" + 
                mat[0,0] + " " + mat[0, 1] + " " + mat[0, 2] + "\n" +
                mat[1,0] + " " + mat[1, 1] + " " + mat[1, 2] + "\n" +
                mat[2,0] + " " + mat[2, 1] + " " + mat[2, 2];
            return s;
        }
        protected static double[,] MatrixProduct(double[,] mat1, double[,] mat2)
        {
            double[,] result = new double[3, 3];
   

            for (int i = 0; i < 3; i++)
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
    }

}
