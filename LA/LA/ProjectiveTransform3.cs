using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;

namespace LIN_ALG
{
    // TODO: add comments
    public class ProjectiveTransform3 : MatrixTransform3
    {
        public ProjectiveTransform3()
        {
            for (int i = 0; i < 4; i++)
            {
                mat[i, i] = 1.0d;
            }
        }
        public override String TransformName()
        {
            return "Projective";
        }

        public static ProjectiveTransform3 RotateXY(double angle)
        {
            AffineTransform3 T = AffineTransform3.RotateXY(angle);
            ProjectiveTransform3 T2 = new ProjectiveTransform3();
            T2.mat = T.mat;
            T.mat = null;
            return T2;
        }

        public static ProjectiveTransform3 RotateYZ(double angle)
        {
            AffineTransform3 T = AffineTransform3.RotateYZ(angle);
            ProjectiveTransform3 T2 = new ProjectiveTransform3();
            T2.mat = T.mat;
            T.mat = null;
            return T2;
        }
        public static ProjectiveTransform3 RotateZX(double angle)
        {
            AffineTransform3 T = AffineTransform3.RotateZX(angle);
            ProjectiveTransform3 T2 = new ProjectiveTransform3();
            T2.mat = T.mat;
            T.mat = null;
            return T2;
        }

        public static ProjectiveTransform3 Translate(Vector3D v)
        {
            AffineTransform3 T = AffineTransform3.Translate(v);
            ProjectiveTransform3 T2 = new ProjectiveTransform3();
            T2.mat = T.mat;
            T.mat = null;
            return T2;
        }

        public static ProjectiveTransform3 Translate(Point3D p, Point3D q)
        {
            AffineTransform3 T = AffineTransform3.Translate(p, q);
            ProjectiveTransform3 T2 = new ProjectiveTransform3();
            T2.mat = T.mat;
            T.mat = null;
            return T2;
        }

        public static ProjectiveTransform3 AxisScale(double xamount, double yamount, double zamount)
        {
            AffineTransform3 T = AffineTransform3.AxisScale(xamount, yamount, zamount);
            ProjectiveTransform3 T2 = new ProjectiveTransform3();
            T2.mat = T.mat;
            T.mat = null;
            return T2;
        }

        public static ProjectiveTransform3 PointsToPoints(
            Point3D p0, Point3D p1, Point3D p2, Point3D p3, Point3D p4,
                Point3D q0, Point3D q1, Point3D q2, Point3D q3, Point3D q4)
        {
            ProjectiveTransform3 Step1 = StandardFrameToPoints(p0, p1, p2, p3, p4).InverseTransform();
            ProjectiveTransform3 Step2 = StandardFrameToPoints(q0, q1, q2, q3, q4);
            return Step2 * Step1;
        }

        private static ProjectiveTransform3 StandardFrameToPoints(Point3D p0, Point3D p1, Point3D p2, Point3D p3, Point3D p4)
        {
            // 
            ProjectiveTransform3 T = new ProjectiveTransform3();
            // idea: send p0, p1, p2, and p3 to e1, e2, e3 and e4 by an linear transformation K of R^3; see where p4 goes; call this q. 
            // build projective map P sending e1, e2, e3, e4, and u= (e1+e2+e3) to e1, e2, e3, d4, and q. 
            // then let L = Kinverse; K * P sends e1 to p1; e2 to p2; e3 to p3; e4 to p4, and u to q to e4. 
            ProjectiveTransform3 K = new ProjectiveTransform3();
            for (int i = 0; i < 4; i++)
            {
                K.mat[3, i] = 1.0d;
            }
            K.mat[0, 0] = p0.X;
            K.mat[1, 0] = p0.Y;
            K.mat[2, 0] = p0.Z;
            K.mat[0, 1] = p1.X;
            K.mat[1, 1] = p1.Y;
            K.mat[2, 1] = p1.Z;
            K.mat[0, 2] = p2.X;
            K.mat[1, 2] = p2.Y;
            K.mat[2, 2] = p2.Z;
            K.mat[0, 3] = p3.X;
            K.mat[1, 3] = p3.Y;
            K.mat[2, 3] = p3.Z;

            ProjectiveTransform3 L = new ProjectiveTransform3();
            L.mat = LinearTransform3.MatrixInverse(K.mat);
            double[] v = new double[3];
            v[0] = p3.X;
            v[1] = p3.Y;
            v[2] = p4.Z;
            v[3] = 1.0d;

            double[] q = new double[4];
            for (int i = 0; i < 4; i++)
            {
                double tally = 0.0d;
                for (int j = 0; j < 4; j++)
                {
                    tally += L.mat[i, j] * v[j];
                }
                q[i] = tally;
            }
            double[,] p = new double[4, 4];
            for (int i = 0; i < 4; i++)
            {
                p[i, i] = q[i];
            }
            ProjectiveTransform3 S = new ProjectiveTransform3();
            S.mat = ProjectiveTransform3.MatrixProduct(p, K.mat);
            return S;
        }


        public ProjectiveTransform3 InverseTransform()
        {
            double[,] m = MatrixInverse(mat);
            ProjectiveTransform3 T = new ProjectiveTransform3();
            T.mat = mat;
            return T;
        }

        public static ProjectiveTransform3 operator *(ProjectiveTransform3 T1, ProjectiveTransform3 T2)
        {
            ProjectiveTransform3 S = new ProjectiveTransform3();
            S.mat = MatrixTransform3.MatrixProduct(T1.mat, T2.mat);
            return S;
        }

        public static Vector3D operator *(ProjectiveTransform3 T, Point3D p)
        {
            double[] vv = new double[4];
            vv[0] = p.X;
            vv[1] = p.Y;
            vv[2] = p.Z;
            vv[3] = 1.0d;
            double[] res = new double[4];

            for (int i = 0; i < 4; i++)
            {
                double tally = 0.0d;
                for (int k = 0; k < 4; k++)
                {
                    tally += T.mat[i, k] * vv[k];
                }
                res[i] = tally;

            }
            return new Vector3D(res[0] / res[3], res[1] / res[3], res[2]/res[3]);
        }
    }
}
