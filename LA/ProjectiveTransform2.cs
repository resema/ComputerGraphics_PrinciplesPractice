using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace LIN_ALG_TEST
{
    // TODO: add comments
    public class ProjectiveTransform2 : MatrixTransform2
    {
        public ProjectiveTransform2()
        {
            for (int i = 0; i < 3; i++)
            {
                mat[i, i] = 1.0d;
            }
        }
        public override String TransformName()
        {
            return "Projective";
        }

        public static ProjectiveTransform2 RotateXY(double angle)
        {
            AffineTransform2 T = AffineTransform2.RotateXY(angle);
            ProjectiveTransform2 T2 = new ProjectiveTransform2();
            T2.mat = T.mat;
            T.mat = null;
            return T2;
        }

        public static ProjectiveTransform2 Translate(Vector v)
        {
            AffineTransform2 T = AffineTransform2.Translate(v);
            ProjectiveTransform2 T2 = new ProjectiveTransform2();
            T2.mat = T.mat;
            T.mat = null;
            return T2;
        }

        public static ProjectiveTransform2 Translate(Point p, Point q)
        {
            AffineTransform2 T = AffineTransform2.Translate(p, q);
            ProjectiveTransform2 T2 = new ProjectiveTransform2();
            T2.mat = T.mat;
            T.mat = null;
            return T2;
        }

        public static ProjectiveTransform2 AxisScale(double xamount, double yamount)
        {
            AffineTransform2 T = AffineTransform2.AxisScale(xamount, yamount);
            ProjectiveTransform2 T2 = new ProjectiveTransform2();
            T2.mat = T.mat;
            T.mat = null;
            return T2;
        }

        public static ProjectiveTransform2 RotateAboutPoint(Point p, double angle)
        {
            Point origin = new Point();
            ProjectiveTransform2 T1 = Translate(origin - p);
            ProjectiveTransform2 T2 = RotateXY(angle);
            ProjectiveTransform2 T3 = Translate(p - origin);
            return T3 * T2 * T1;
        }

        public static ProjectiveTransform2 PointsToPoints(
            Point p0, Point p1, Point p2, Point p3,
                Point q0, Point q1, Point q2, Point q3)
        {
            ProjectiveTransform2 Step1 = StandardFrameToPoints(p0, p1, p2, p3).InverseTransform();
            ProjectiveTransform2 Step2 = StandardFrameToPoints(q0, q1, q2, q3);
            return Step2 * Step1;
        }

        public static ProjectiveTransform2 StandardFrameToPoints(Point p0, Point p1, Point p2, Point p3)
        {
            // 
            ProjectiveTransform2 T = new ProjectiveTransform2();
            // idea: 
            // Send e1, e2, e3 to p0, p1, p2 by a map K. 
            // Let L be Kinverse.
            // Then L sends p0, p1, p2 to e1, e2 and e3 . See where p4 goes; call this q. 
            // build projective map P sending e1, e2, e3, and u= (e1+e2+e3) to e1, e2, e3, and q. 
            // then let L = Kinverse; K * P sends e1 to p1; e2 to p2; e3 to p3; and u to q to e4. 
            ProjectiveTransform2 K = new ProjectiveTransform2();
            for (int i = 0; i < 3; i++)
            {
                K.mat[2, i] = 1.0d;
            }
            K.mat[0, 0] = p0.X;
            K.mat[1, 0] = p0.Y;
            K.mat[0, 1] = p1.X;
            K.mat[1, 1] = p1.Y;
            K.mat[0, 2] = p2.X;
            K.mat[1, 2] = p2.Y;

            ProjectiveTransform2 L = new ProjectiveTransform2();
            L.mat = LinearTransform2.MatrixInverse(K.mat);
            double[] v = new double[3];
            v[0] = p3.X;
            v[1] = p3.Y;
            v[2] = 1.0d;

            double[] q = new double[3];
            for (int i = 0; i < 3; i++)
            {
                double tally = 0.0d;
                for (int j = 0; j < 3; j++)
                {
                    tally += L.mat[i, j] * v[j];
                }
                q[i] = tally;
            }
            double[,] p = new double[3, 3];
            for (int i = 0; i < 3; i++)
            {
                p[i, i] = q[i];
            }
            ProjectiveTransform2 S = new ProjectiveTransform2();
            S.mat = ProjectiveTransform2.MatrixProduct(K.mat, p);
            return S;
        }


        public ProjectiveTransform2 InverseTransform()
        {
            double[,] m = MatrixInverse(mat);
            ProjectiveTransform2 T = new ProjectiveTransform2();
            T.mat = m;
            return T;
        }

        public static ProjectiveTransform2 operator *(ProjectiveTransform2 T1, ProjectiveTransform2 T2)
        {
            ProjectiveTransform2 S = new ProjectiveTransform2();
            S.mat = MatrixTransform2.MatrixProduct(T1.mat, T2.mat);
            return S;
        }

        public static Vector operator *(ProjectiveTransform2 T, Point p)
        {
            double[] vv = new double[3];
            vv[0] = p.X;
            vv[1] = p.Y;
            vv[2] = 1.0d;
            double[] res = new double[3];

            for (int i = 0; i < 3; i++)
            {
                double tally = 0.0d;
                for (int k = 0; k < 3; k++)
                {
                    tally += T.mat[i, k] * vv[k];
                }
                res[i] = tally;

            }
            return new Vector(res[0] / res[2], res[1] / res[2]);
        }
    }
}
