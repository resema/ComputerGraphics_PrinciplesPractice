using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using MathNet.Numerics;
using System.Windows.Media.Media3D;

namespace LIN_ALG_TEST
{
    // SVD? Rotation about a vector and point. 
    // Constructors from matrices, so you can build your own shear xform, etc. 
    // TODO: add comments
    public class LinearTransform3 : MatrixTransform3
    {
        public LinearTransform3()
        {
            for (int i = 0; i < 4; i++)
            {
                mat[i, i] = 1.0d;
            }
        }

        public LinearTransform3(Vector3D u, Vector3D v, Vector3D w)
        {
            CopyVecToColumn(mat, u, 0);
            CopyVecToColumn(mat, v, 1);
            CopyVecToColumn(mat, w, 2);
        }
        private void CopyVecToColumn(double[,] mat, Vector3D u, int col)
        {
            mat[0, col] = u.X;
            mat[1, col] = u.Y;
            mat[2, col] = u.Z;
        }

        public override String TransformName()
        {
            return "Linear";
        }

        public static LinearTransform3 VectorsToVectors(Vector3D v1, Vector3D v2, Vector3D v3, Vector3D w1, Vector3D w2, Vector3D w3)
        {
            LinearTransform3 V = new LinearTransform3(v1, v2, v3);
            LinearTransform3 W = new LinearTransform3(w1, w2, w3);
            return W * V.InverseTransform();
        }

        public static LinearTransform3 RotateXY(double angle)
        {
            return RotMat(0, 1, angle);
        }
        private static LinearTransform3 RotMat(int first, int second, double angle)
        {
            LinearTransform3 T = new LinearTransform3();
            T.mat[first, first] = Math.Cos(angle);
            T.mat[second, second] = T.mat[first, first];
            T.mat[second, first] = Math.Sin(angle);
            T.mat[first, second] = -T.mat[second, first];
            return T;
        }

        public static LinearTransform3 RotateYZ(double angle)
        {
            return RotMat(1, 2, angle);
        }

        public static LinearTransform3 RotateZX(double angle)
        {
            return RotMat(2, 0, angle);
        }

        public static LinearTransform3 AxisScale(double xscale, double yscale, double zscale)
        {
            LinearTransform3 T = new LinearTransform3();
            T.mat[0, 0] = xscale;
            T.mat[1, 1] = yscale;
            T.mat[2, 2] = zscale;

            return T;
        }

        public LinearTransform3 InverseTransform()
        {
            double[,] m = MatrixInverse(mat);
            LinearTransform3 T = new LinearTransform3();
            T.mat = m;
            return T;
        }

        public static LinearTransform3 operator *(LinearTransform3 T1, LinearTransform3 T2)
        {
            LinearTransform3 S = new LinearTransform3();
            S.mat = MatrixProduct(T1.mat, T2.mat);
            return S;
        }

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

        public double Det()
        {
            MathNet.Numerics.LinearAlgebra.Matrix m = new MathNet.Numerics.LinearAlgebra.Matrix(mat);
            return m.Determinant();
        }

        public double Trace()
        {
            return mat[0, 0] + mat[1, 1] + mat[2, 2];
        }
    }
}
