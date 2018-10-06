using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using MathNet.Numerics;
using System.Windows.Media.Media3D;

namespace LIN_ALG
{
    /// <summary>
    /// A class for representing linear transforms of the plane. 
    /// </summary>    
    public class LinearTransform3 : MatrixTransform3
    {
        /// <summary>
        /// Construct the identity transformation
        /// </summary>
        public LinearTransform3()
        {
            for (int i = 0; i < 4; i++)
            {
                mat[i, i] = 1.0d;
            }
        }

        /// <summary>
        /// Build a transformation whose matrix has "u" as its first column, "v" as its second, and w as its third 
        /// </summary>
        /// <param name="u">The first column of the matrix</param>
        /// <param name="v">The second column of the matrix</param>
        /// <param name="w">The 3rd column of the matrix</param>
        public LinearTransform3(Vector3D u, Vector3D v, Vector3D w)
        {
            CopyVecToColumn(mat, u, 0);
            CopyVecToColumn(mat, v, 1);
            CopyVecToColumn(mat, w, 2);
        }

        /// <summary>
        /// Copy the vector u into the "col" column of the  matrix m
        /// </summary>
        /// <param name="mat">The matrix to be altered</param>
        /// <param name="u">The vector to copy into the matrix</param>
        /// <param name="col">The column into which to copy the vector</param>
        private void CopyVecToColumn(double[,] mat, Vector3D u, int col)
        {
            mat[0, col] = u.X;
            mat[1, col] = u.Y;
            mat[2, col] = u.Z;
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
        /// Build a linear transformation taking three independent vectors v1, v2, v3 to vectors w1, w2, w3 respectively. 
        /// </summary>
        /// <param name="v1">1st source vector</param>
        /// <param name="v2">2nd source vector</param>
        /// <param name="v3">3rd source vector</param>
        /// <param name="w1">1st target vector (i.e., Mv1 = w1)</param>
        /// <param name="w2">2nd target vector</param>
        /// <param name="w3">3rd target vector</param>
        /// <returns></returns>
        public static LinearTransform3 VectorsToVectors(Vector3D v1, Vector3D v2, Vector3D v3, Vector3D w1, Vector3D w2, Vector3D w3)
        {
            LinearTransform3 V = new LinearTransform3(v1, v2, v3);
            LinearTransform3 W = new LinearTransform3(w1, w2, w3);
            return W * V.InverseTransform();
        }


        /// <summary>
        /// Build a rotation about the origin that moves the positive X-axis towards the postive Y-axis by the specified angle. 
        /// </summary>
        /// <param name="angle">The angle to rotate, in radians</param>
        /// <returns>A rotation transformation</returns>
        public static LinearTransform3 RotateXY(double angle)
        {
            return RotMat(0, 1, angle);
        }

        /// <summary>
        /// Copy the [c -s; s c] matrix into a pair of rows and colums of the identity matrix, where c and s are the cosine and sine of a given angle. 
        /// </summary>
        /// <param name="first">The index of the first column into which to copy the matrix</param>
        /// <param name="second">The index of the first column into which to copy the matrix</param>
        /// <param name="angle">The angle to rotate, in radians</param>
        /// <returns>A rotation transformation</returns>
        private static LinearTransform3 RotMat(int first, int second, double angle)
        {
            LinearTransform3 T = new LinearTransform3();
            T.mat[first, first] = Math.Cos(angle);
            T.mat[second, second] = T.mat[first, first];
            T.mat[second, first] = Math.Sin(angle);
            T.mat[first, second] = -T.mat[second, first];
            return T;
        }

        /// <summary>
        /// Build a rotation about the origin that moves the positive Y-axis towards the postive Z-axis by the specified angle. 
        /// </summary>
        /// <param name="angle">The angle to rotate, in radians</param>
        /// <returns>A rotation transformation</returns>
        public static LinearTransform3 RotateYZ(double angle)
        {
            return RotMat(1, 2, angle);
        }

        /// <summary>
        /// Build a rotation about the origin that moves the positive Z-axis towards the postive X-axis by the specified angle. 
        /// </summary>
        /// <param name="angle">The angle to rotate, in radians</param>
        /// <returns>A rotation transformation</returns>
        public static LinearTransform3 RotateZX(double angle)
        {
            return RotMat(2, 0, angle);
        }

        /// <summary>
        /// Build the transformation (x, y, z) -> (ax, by, cz)
        /// </summary>
        /// <param name="xscale">The scale factor for y-coordinates</param>
        /// <param name="yscale">The scale factor for y-coordinates</param>
        /// <param name="zscale">The scale factor for z-coordinates</param>
        /// <returns></returns>
        public static LinearTransform3 AxisScale(double xscale, double yscale, double zscale)
        {
            LinearTransform3 T = new LinearTransform3();
            T.mat[0, 0] = xscale;
            T.mat[1, 1] = yscale;
            T.mat[2, 2] = zscale;

            return T;
        }

        /// <summary>
        /// Compute the inverse of the transformation, if it exists. 
        /// </summary>
        /// <returns>The inverse transform</returns>
        public LinearTransform3 InverseTransform()
        {
            double[,] m = MatrixInverse(mat);
            LinearTransform3 T = new LinearTransform3();
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
        public static LinearTransform3 operator *(LinearTransform3 T1, LinearTransform3 T2)
        {
            LinearTransform3 S = new LinearTransform3();
            S.mat = MatrixProduct(T1.mat, T2.mat);
            return S;
        }

        /// <summary>
        /// Apply the transformation T to the vector v
        /// </summary>
        /// <param name="T">A linear transformation of the plane</param>
        /// <param name="v">A vector to which its applied</param>
        /// <returns>The resulting vector</returns>
        public static Vector operator *(LinearTransform3 T, Vector3D v)
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
            MathNet.Numerics.LinearAlgebra.Matrix m = new MathNet.Numerics.LinearAlgebra.Matrix(mat);
            return m.Determinant();
        }

        /// <summary>
        /// Compute the trace of a transformation, i.e., the sum of the diagonal entries of its matrix.  
        /// </summary>
        /// <returns>the trace</returns>
        public double Trace()
        {
            return mat[0, 0] + mat[1, 1] + mat[2, 2];
        }
    }
}
