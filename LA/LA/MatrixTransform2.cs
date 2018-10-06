using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using MathNet.Numerics.LinearAlgebra;

namespace LIN_ALG
{
    /// <summary>
    /// A class for representing transformations of the Euclidean plane using 3x3 matrices. The base class for AffineTransform2
    /// and ProjectiveTransform2. 
    /// </summary>
    public class MatrixTransform2
    {
        public double[,] mat = new double[3, 3];
        public const double EPSILON = 1E-9;


        /// <summary>
        /// Construct the identity transformation
        /// </summary>
        protected MatrixTransform2()
        {
            for (int i = 0; i < 3; i++)
            {
                mat[i, i] = 1.0d;
            }
        }

        /// <summary>
        /// Return the 3x3 matrix of doubles that's the internal representation of a MatrixTransform2. 
        /// </summary>
        /// <returns>the 3x3 matrix</returns>
        public double[,] Matrix()
        {
            return mat;
        }
        
        /// <summary>
        /// Return the inverse of a 3x3 matrix, if it exists. 
        /// </summary>
        /// <param name="mat">The matrix</param>
        /// <returns>its inverse</returns>
        protected static double[,] MatrixInverse(double[,] mat)
        {
            Matrix m = new Matrix(mat);
            Matrix k = m.Inverse();
            return k;
        }

        /// <summary>
        /// The name of this class of transforms is "Matrix"
        /// </summary>
        /// <returns>The String "Matrix"</returns>
        public virtual String TransformName() {
            return "Matrix";
        }

        /// <summary>
        /// Provide a string representation of a 3x3 matrix
        /// </summary>
        /// <returns>A string representation of the matrix. </returns>
        public override String ToString() {
            String s = TransformName() + " transform, matrix = \n" + 
                mat[0,0] + " " + mat[0, 1] + " " + mat[0, 2] + "\n" +
                mat[1,0] + " " + mat[1, 1] + " " + mat[1, 2] + "\n" +
                mat[2,0] + " " + mat[2, 1] + " " + mat[2, 2];
            return s;
        }

        /// <summary>
        /// Compute the product of two 3x3 matrices
        /// </summary>
        /// <param name="mat1">The first matrix mat1</param>
        /// <param name="mat2">The second matrix mat2</param>
        /// <returns>The matrix mat1 * mat2</returns>
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
