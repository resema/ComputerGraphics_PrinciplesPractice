using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;

namespace LIN_ALG_TEST
{
    // TODO: add comments; NORMAL XFORM
    public class AffineTransform3 : MatrixTransform3
    {
        public AffineTransform3()
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

        public static AffineTransform3 RotateXY(double angle)
        {
            return RotMat(0, 1, angle);
        }
        private static AffineTransform3 RotMat(int first, int second, double angle)
        {
            AffineTransform3 T = new AffineTransform3();
            T.mat[first, first] = Math.Cos(angle);
            T.mat[second, second] = T.mat[first, first];
            T.mat[second, first] = Math.Sin(angle);
            T.mat[first, second] = -T.mat[second, first];
            return T;
        }

        public static AffineTransform3 RotateYZ(double angle)
        {
            return RotMat(1, 2, angle);
        }

        public static AffineTransform3 RotateZX(double angle)
        {
            return RotMat(2, 0, angle);
        }

        public static AffineTransform3 AxisScale(double xscale, double yscale, double zscale)
        {
            AffineTransform3 T = new AffineTransform3();
            T.mat[0, 0] = xscale;
            T.mat[1, 1] = yscale;
            T.mat[2, 2] = zscale;

            return T;
        }

        public static AffineTransform3 Translate(Vector3D v)
        {
            AffineTransform3 T = new AffineTransform3();
            T.mat[0, 2] = v.X;
            T.mat[1, 2] = v.Y;
            return T;
        }

        public static AffineTransform3 Translate(Point3D p, Point3D q)
        {
            return Translate(q - p);
        }

        public static AffineTransform3 PointsToPoints(
            Point3D p1, Point3D p2, Point3D p3, Point3D p4,
                Point3D q1, Point3D q2, Point3D q3, Point3D q4)
        {
            Vector3D v1 = p2 - p1;
            Vector3D v2 = p3 - p1;
            Vector3D v3 = p4 - p1;
            Vector3D w1 = q2 - q1;
            Vector3D w2 = q3 - q1;
            Vector3D w3 = q4 - q1;

            return AffineTransform3.PointAndVectorsToPointAndVectors(p1, v1, v2, v3, q1, w1, w2, w3);
        }

        public static AffineTransform3 PointAndVectorsToPointAndVectors(
            Point3D p1, Vector3D v1, Vector3D v2, Vector3D v3,
                Point3D q1, Vector3D w1, Vector3D w2, Vector3D w3)
        {
            AffineTransform3 Trans1 = AffineTransform3.Translate(p1, new Point3D(0,0, 0));
            LinearTransform3 T = LinearTransform3.VectorsToVectors(v1, v2, v3, w1, w2, w3);
            AffineTransform3 Trans2 = AffineTransform3.Translate(new Point3D(0,0,0), q1);
            AffineTransform3 S = new AffineTransform3();
            S.mat = T.Matrix();

            return Trans2 * S * Trans1;
        }

        public static AffineTransform3 PointsAndVectorsToPointsAndVectors(
            Point3D p1, Point3D p2, Vector3D v1, Vector3D v2,
                Point3D q1, Point3D q2, Vector3D w1, Vector3D w2)
        {
            Vector3D v3 = p2 - p1;
            Vector3D w3 = q2 - q1;

            return AffineTransform3.PointAndVectorsToPointAndVectors(p1, v1, v2, v3, q1, w1, w2, w3);
        }

        public static AffineTransform3 PointsAndVectorToPointsAndVector(
            Point3D p1, Point3D p2, Point3D p3, Vector3D v1,
                Point3D q1, Point3D q2, Point3D q3, Vector3D w1)
        {
            Vector3D v2 = p2 - p1;
            Vector3D w2 = q2 - q1;
            Vector3D v3 = p3 - p1;
            Vector3D w3 = q3 - q1;

            return AffineTransform3.PointAndVectorsToPointAndVectors(p1, v1, v2, v3, q1, w1, w2, w3);
        }

        public AffineTransform3 InverseTransform()
        {
            double[,] m = MatrixInverse(mat);
            AffineTransform3 T = new AffineTransform3();
            T.mat = m;
            return T;
        }

        public static AffineTransform3 operator *(AffineTransform3 T1, AffineTransform3 T2)
        {
            AffineTransform3 S = new AffineTransform3();
            S.mat = MatrixProduct(T1.mat, T2.mat);
            return S;
        }

        public static Vector3D operator *(AffineTransform3 T, Point3D p)
        {
            double[] vv = new double[4];
            vv[0] = p.X;
            vv[1] = p.Y;
            vv[2] = p.Z;
            vv[3] = 1.0d;
            double[] res = new double[3];

            for (int i = 0; i < 3; i++)
            {
                double tally = 0.0d;
                for (int k = 0; k < 4; k++)
                {
                    tally += T.mat[i, k] * vv[k];
                }
                res[i] = tally;
            }
            return new Vector3D(res[0], res[1], res[2]);
        }

        public static Vector3D operator *(AffineTransform3 T, Vector3D v)
        {
            double[] vv = new double[3];
            vv[0] = v.X;
            vv[1] = v.Y;
            vv[2] = v.Z;
            vv[3] = 0.0d;
            double[] res = new double[3];

            for (int i = 0; i < 3; i++)
            {
                double tally = 0.0d;
                for (int k = 0; k < 4; k++)
                {
                    tally += T.mat[i, k] * vv[k];
                }
                res[i] = tally;

            }
            return new Vector3D(res[0], res[1], res[2]);
        }

    }
}
