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
    public class MatrixTransform3
    {
        public double[,] mat = new double[4, 4];
        public const double EPSILON = 1E-9;

        protected MatrixTransform3()
        {
            for (int i = 0; i < 4; i++)
            {
                mat[i, i] = 1.0d;
            }
        }
        // public static virtual Point operator*(MatrixTransform3 T, Point p);
        // public static abstract Vector operator*(MatrixTransform3 T, Vector v);
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
                mat[0,0] + " " + mat[0, 1] + " " + mat[0, 2] + " " + mat[0, 3] + "\n" +
                mat[1,0] + " " + mat[1, 1] + " " + mat[1, 2] + " " + mat[1, 3] + "\n" +
                mat[2,0] + " " + mat[2, 1] + " " + mat[2, 2] + " " + mat[2, 3] + "\n" + 
                mat[3,0] + " " + mat[3, 1] + " " + mat[3, 2] + " " + mat[3, 3]; 
            return s;
        }
        protected static double[,] MatrixProduct(double[,] mat1, double[,] mat2)
        {
            double[,] result = new double[4, 4];
   

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    double tally = 0.0d;
                    for (int k = 0; k < 4; k++)
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
