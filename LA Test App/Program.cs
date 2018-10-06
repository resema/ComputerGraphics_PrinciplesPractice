using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Windows;

using System.Diagnostics;
namespace LIN_ALG
{
    class Program
    {
        /// <summary>
        /// A program to test the linear algebra package. Output in Immediate Window in Visual Studio. 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            testLT();
            testAT();
            testPT();
        }
        private static void testLT()
        {

            // Results: inverse is broken for LinearTransform; otherwise OK. 
            Debug.Print(new LinearTransform2() + "\nshould be identity\n");
            Vector v1 = new Vector(2, 3); Vector v2 = new Vector(-1, 4);
            LinearTransform2 T1 = new LinearTransform2(v1, v2);
            Debug.Print(T1 + "\nshould be 2, -1; 3 4.\n");
            Debug.Print(T1.Det() + "\n should be 11; " + T1.Trace() + "\nshould be 6\n");
            Vector w1 = new Vector(0, 1); Vector w2 = new Vector(1, 1);
            LinearTransform2 Tvw = LinearTransform2.VectorsToVectors(v1, v2, w1, w2);
            LinearTransform2 T30 = LinearTransform2.RotateXY(30 * Math.PI / 180.0);
            LinearTransform2 Ti = T1.InverseTransform();
            Debug.Print(T30 + "\nshould be Rot30; lower left = 0.5\n");
            Debug.Print(Ti * v1 + "\nshould be e1\n");
            Debug.Print(Ti * v2 + "\nshould be e2\n");
            Debug.Print(T1 * T1 + "\nshould be [1 -6; 18 13]\n");
            Debug.Print(T1 * Ti + "\n should be identity\n");
        }
        private static void testAT()
        {
            // Results: inverse is broken for LinearTransform; otherwise OK. 
            Debug.Print("T0:" + new AffineTransform2() + "\nshould be identity\n");
            Vector v1 = new Vector(2, 3); Vector v2 = new Vector(-1, 4);
            Vector w1 = new Vector(0, 1); Vector w2 = new Vector(1, 1);
            Point p1 = new Point(1, 5);
            Point p2 = new Point(1, 1);
            Point p3 = new Point(4, 4);
            Point p4 = new Point(2, 5);
            Point q1 = new Point(1, 1);
            Point q2 = new Point(0, 0);
            Point q3 = new Point(1, 2);
            Point q4 = new Point(-1, 0);
            Point pt = p1 + 0.5 * (p2 - p1);
            Point qt = q1 + 0.5 * (q2 - q1);

            AffineTransform2 T1 = AffineTransform2.Translate(p1, q1);

            Debug.Print("T1:" + T1 * p1 + "\n should be " + q1 + "\n");
            AffineTransform2 T2 = AffineTransform2.PointAndVectorsToPointAndVectors(p1, v1, v2, q1, w1, w2);
            Debug.Print("T2:" + T2 * (v1 + v2) + "\n should be " + (w1 + w2) + "\n"); /// Broken
            Debug.Print("T2:" + T2 * p1 + "\n should be " + q1 + "\n");
            AffineTransform2 T3 = AffineTransform2.RotateXY(30 * Math.PI / 180);
            LinearTransform2 T4 = LinearTransform2.RotateXY(30 * Math.PI / 180);
            Debug.Print("T3,4:" + T3 + "\n should equal " + T4 + "\n");
            AffineTransform2 T5 = AffineTransform2.AxisScale(2, -3);
            Debug.Print("T5:" + T5 + "\n should be [2 0 ; 0 -3]\n");
            AffineTransform2 T6 = AffineTransform2.RotateAboutPoint(p1, 30 * Math.PI / 180);
            Debug.Print("T6:" + T6 * p1 + "\n should be " + p1 + "\n");
            Debug.Print("T6:" + T6 * new Vector(1, 0) + "\n should be [.866, .5]\n");

            AffineTransform2 TPV = AffineTransform2.PointAndVectorsToPointAndVectors(p1, v1, v2, q1, w1, w2); // Broken
            Debug.Print("TPV:" + TPV * p1 + "\n should be " + q1 + "\n");
            Debug.Print("TPV:" + TPV * (p1 + 0.3 * v2) + "\n should be " + (q1 + 0.3 * w2) + "\n");

            AffineTransform2 T7 = AffineTransform2.PointsToPoints(p1, p2, p3, q1, q2, q3);
            Debug.Print("T7:" + T7 * pt + "\n should be " + qt + "\n");
            Debug.Print("T7:" + T7 * p1 + "\n should be " + q1 + "\n");
            Debug.Print("T7:" + T7 * p2 + "\n should be " + q2 + "\n");
            Debug.Print("T7:" + T7 * p3 + "\n should be " + q3 + "\n");
            Debug.Print("Inverse:" + T7 * (T7.InverseTransform()) + "\n should be identity\n"); //BROKEN
            AffineTransform2 T7i = T7.InverseTransform();
            Debug.Print("T7i:" + T7i * q1 + "\n should be " + p1 + "\n");
            Debug.Print("T7i:" + T7i * q2 + "\n should be " + p2 + "\n");
            Debug.Print("T7i:" + T7i * q3 + "\n should be " + p3 + "\n");

            AffineTransform2 T8 = AffineTransform2.PointsAndVectorToPointsAndVector(p1, p2, p3 - p1, q1, q2, q3 - q1);
            Debug.Print("T8:" + T7 * (T8.InverseTransform()) + "\n should be identity\n"); //BROKEN
            Debug.Print("T8:" + T8 * p1 + "\n should be " + q1 + "\n");
            Debug.Print("T8:" + T8 * p2 + "\n should be " + q2 + "\n");
            Debug.Print("T8:" + T8 * p3 + "\n should be " + q3 + "\n");
        }
        private static void testPT()
        {
            Debug.Print(new ProjectiveTransform2() + "\n should be identity\n");
            Vector v1 = new Vector(2, 3); Vector v2 = new Vector(-1, 4);
            Point p1 = new Point(1, 5);
            Point p2 = new Point(1, 1);
            Point p3 = new Point(4, 4);
            Point p4 = new Point(2, 5);
            Point q1 = new Point(1, 1);
            Point q2 = new Point(0, 0);
            Point q3 = new Point(1, 2);
            Point q4 = new Point(-1, 0);
            Point pt = p1 + 0.5 * (p2 - p1);
            Point qt = q1 + 0.5 * (q2 - q1);

            ProjectiveTransform2 T1 = ProjectiveTransform2.Translate(p1, q1);
            Debug.Print("T1:" + T1 * p1 + "\n should be " + q1 + "\n");
            ProjectiveTransform2 T3 = ProjectiveTransform2.RotateXY(30 * Math.PI / 180);
            LinearTransform2 T4 = LinearTransform2.RotateXY(30 * Math.PI / 180);
            Debug.Print("T3,4:" + T3 + "\n should equal " + T4 + "\n");
            ProjectiveTransform2 T5 = ProjectiveTransform2.AxisScale(2, -3);
            Debug.Print("T5:" + T5 + "\n should be [2 0 ; 0 -3]\n");
            ProjectiveTransform2 T6 = ProjectiveTransform2.RotateAboutPoint(p1, 30 * Math.PI / 180);
            Debug.Print("T6:" + T6 * p1 + "\n should be " + p1 + "\n");

            ProjectiveTransform2 T2 = ProjectiveTransform2.PointsToPoints(p1, p2, p3, p4, q1, q2, q3, q4);
            Debug.Print("T2:" + T2 * (p1 + 0.5 * (p2 - p1)) + "\n should be " + (q1 + 0.5 * (q2 - q1)) + "\n");
            Debug.Print("T2:" + T2 * p1 + "\n should be " + q1 + "\n");
            Debug.Print("T2:" + T2 * p2 + "\n should be " + q2 + "\n");
            Debug.Print("T2:" + T2 * p3 + "\n should be " + q3 + "\n");
            Debug.Print("T2:" + T2 * p4 + "\n should be " + q4 + "\n");
            Debug.Print("InverseTransform:" + T2.InverseTransform() * q4 + "\n should be " + p4 + "\n");
            Debug.Print("Composition:" + T2.InverseTransform() * T2 + "\n should be identity\n");

        }

    }
}
