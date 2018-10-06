using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using MathNet.Numerics.LinearAlgebra;

namespace LIN_ALG
{
    /// <summary>
    /// A class for representing transformations of the Euclidean 3-space using 4x4 matrices. The base class for AffineTransform3
    /// and ProjectiveTransform3. 
    /// </summary>    
    public class MatrixTransform3
    {
        public double[,] mat = new double[4, 4];
        public const double EPSILON = 1E-9;

        /// <summary>
        /// Construct the identity transformation
        /// </summary>
        protected MatrixTransform3()
        {
            for (int i = 0; i < 4; i++)
            {
                mat[i, i] = 1.0d;
            }
        }

        /// <summary>
        /// Return the 4x4 matrix of doubles that's the internal representation of a MatrixTransform2. 
        /// </summary>
        /// <returns>a 4x4 matrix</returns>
        public double[,] Matrix()
        {
            return mat;
        }


        /// <summary>
        /// Return the inverse of a 4x4 matrix, if it exists. 
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
        /// Provide a string representation of a 4x4 matrix transformation
        /// </summary>
        /// <returns>A string representation of the matrix. </returns>
        public override String ToString()
        {
            String s = TransformName() + " transform, matrix = \n" + 
                mat[0,0] + " " + mat[0, 1] + " " + mat[0, 2] + " " + mat[0, 3] + "\n" +
                mat[1,0] + " " + mat[1, 1] + " " + mat[1, 2] + " " + mat[1, 3] + "\n" +
                mat[2,0] + " " + mat[2, 1] + " " + mat[2, 2] + " " + mat[2, 3] + "\n" + 
                mat[3,0] + " " + mat[3, 1] + " " + mat[3, 2] + " " + mat[3, 3]; 
            return s;
        }


        /// <summary>
        /// Compute the product of two 4x4 matrices
        /// </summary>
        /// <param name="mat1">The first matrix mat1</param>
        /// <param name="mat2">The second matrix mat2</param>
        /// <returns>The matrix mat1 * mat2</returns>
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
