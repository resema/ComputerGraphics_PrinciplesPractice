using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using MathNet.Numerics;

namespace LIN_ALG
{
    /// <summary>
    /// A class for representing linear transforms of the plane. 
    /// </summary>
    public class LinearTransform2 : MatrixTransform2
    {
        /// <summary>
        /// Construct the identity transformation
        /// </summary>
        public LinearTransform2()
        {
            for (int i = 0; i < 3; i++)
            {
                mat[i, i] = 1.0d;
            }
        }

        /// <summary>
        /// Build a transformation whose matrix has "v" as its first column and "w" as its second. 
        /// </summary>
        /// <param name="v">The first column of the matrix</param>
        /// <param name="w">The second column of the matrix</param>
        public LinearTransform2(Vector v, Vector w)
        {
            mat[0, 0] = v.X; mat[1, 0] = v.Y;
            mat[0, 1] = w.X; mat[1, 1] = w.Y;
            mat[2, 2] = 1.0d;
        }

        /// <summary>
        /// The transformation name is "Linear"
        /// </summary>
        /// <returns></returns>
        public override String TransformName()
        {
            return "Linear";
        }

        /// <summary>
        /// Build a transformation taking the independent vectors v1 and v2 to w1 and w2, respectively. 
        /// </summary>
        /// <param name="v1">A first vector; must be nonzero. </param>
        /// <param name="v2">A second vector; must be linearly independent of the first</param>
        /// <param name="w1">Where the first vector will be sent</param>
        /// <param name="w2">Where the second vector will be sent</param>
        /// <returns>A linear transformation taking v1 to w1 and v2 to w2 </returns>
        public static LinearTransform2 VectorsToVectors(Vector v1, Vector v2, Vector w1, Vector w2)
        {
            LinearTransform2 V = new LinearTransform2(v1, v2);
            LinearTransform2 W = new LinearTransform2(w1, w2);
            return W * V.InverseTransform();
        }

        /// <summary>
        /// Build a rotation about the origin that moves the positive X-axis towards the postive Y-axis by the specified angle. 
        /// </summary>
        /// <param name="angle">The angle to rotate, in radians</param>
        /// <returns>A rotation transformation</returns>
        public static LinearTransform2 RotateXY(double angle)
        {
            LinearTransform2 T = new LinearTransform2();
            T.mat[0, 0] = Math.Cos(angle);
            T.mat[1, 1] = T.mat[0, 0];
            T.mat[1, 0] = Math.Sin(angle);
            T.mat[0, 1] = -T.mat[1, 0];
            return T;
        }

        /// <summary>
        /// Compute the inverse of this transformation, if it exists. 
        /// </summary>
        /// <returns></returns>
        public LinearTransform2 InverseTransform()
        {
            double[,] m = MatrixInverse(mat);
            LinearTransform2 T = new LinearTransform2();
            T.mat = m;
            return T;
        }

        /// <summary>
        /// Compose the linear transformation T1 with the linear transformation T2 to get T1 o T2.  
        /// </summary>
        /// <param name="T1">The first transformation</param>
        /// <param name="T2">The second transformation</param>
        /// <remarks> Note that when this composite is applied to a vector v, we first apply T2 to v, and then apply T1 to the result. </remarks>
        /// <returns>The composite transformation. </returns>
        public static LinearTransform2 operator *(LinearTransform2 T1, LinearTransform2 T2)
        {
            LinearTransform2 S = new LinearTransform2();
            S.mat = MatrixProduct(T1.mat, T2.mat);
            return S;
        }

        /// <summary>
        /// Apply the transformation T to the vector v
        /// </summary>
        /// <param name="T">A linear transformation of the plane</param>
        /// <param name="v">A vector to which its applied</param>
        /// <returns>The resulting vector</returns>
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

        /// <summary>
        /// Compute the determinant of a linear transformation, i.e., the determinant of its matrix when it's represented in the standard basis. 
        /// </summary>
        /// <returns>the determinant</returns>
        public double Det()
        {
            return Determinant(mat);
        }


        /// <summary>
        /// Compute the determinant of a 2 x 2 matrix 
        /// </summary>
        /// <returns>the determinant</returns>
        private static double Determinant(double[,] mat)
        {
            return mat[0, 0] * mat[1, 1] - mat[0, 1] * mat[1, 0];
        }

        /// <summary>
        /// Compute the trace of a transformation, i.e., the sum of the diagonal entries of its matrix.  
        /// </summary>
        /// <returns>the trace</returns>
        public double Trace()
        {
            return mat[0, 0] + mat[1, 1];
        }

        /// <summary>
        /// Compute the product mat1 * mat2 of two 2x2 matrices
        /// </summary>
        /// <param name="mat1">The first matrix</param>
        /// <param name="mat2">The second matrix</param>
        /// <returns>The product</returns>
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

        /// <summary>
        /// Compute the inverse of a 2x2 matrix, if the inverse exists. 
        /// </summary>
        /// <param name="mat">The matrix</param>
        /// <returns>Its inverse</returns>
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
