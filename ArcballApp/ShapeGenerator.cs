using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace GraphicsBook
{
    public static class ShapeGenerator
    {
        /// <summary>
        /// Creates a simple solid material from the given color
        /// </summary>
        /// <param name="c">color</param>
        /// <returns></returns>
        public static Material GetSimpleMaterial(Color c)
        {
            return new DiffuseMaterial(new SolidColorBrush(c));
        }
        
        /// <summary>
        /// Creates the geometry for a unit cube centered at the origin.
        /// </summary>
        /// <param name="material">A material to use when texturing the cube</param>
        public static ModelVisual3D GenerateUnitCube(Material material)
        {
            Model3DGroup cube = new Model3DGroup();

            Point3D p0 = new Point3D(-.5, -.5, -.5);
            Point3D p1 = new Point3D(.5, -.5, -.5);
            Point3D p2 = new Point3D(.5, -.5, .5);
            Point3D p3 = new Point3D(-.5, -.5, .5);
            Point3D p4 = new Point3D(-.5, .5, -.5);
            Point3D p5 = new Point3D(.5, .5, -.5);
            Point3D p6 = new Point3D(.5, .5, .5);
            Point3D p7 = new Point3D(-.5, .5, .5);

            //front
            cube.Children.Add(CreateQuad(p3, p2, p6, p7, material));
            //right
            cube.Children.Add(CreateQuad(p2, p1, p5, p6, material));
            //back
            cube.Children.Add(CreateQuad(p1, p0, p4, p5, material));
            //left
            cube.Children.Add(CreateQuad(p0, p3, p7, p4, material));
            //top
            cube.Children.Add(CreateQuad(p7, p6, p5, p4, material));
            //bottom
            cube.Children.Add(CreateQuad(p2, p3, p0, p1, material));

            ModelVisual3D cubeModel = new ModelVisual3D();
            cubeModel.Content = cube;
            return cubeModel;
        }

        /// <summary>
        /// Creates the model/geometry for a unit sphere centered at the origin.
        /// </summary>
        /// <param name="thetaSteps">The number of "around" steps to use when creating the sphere mesh</param>
        /// <param name="phiSteps">The number of "top to bottom" steps to use when creating the sphere mesh</param>
        /// <param name="material">A material to use when texturing the sphere</param>
        public static ModelVisual3D GenerateUnitSphere(int thetaSteps, int phiSteps, Material material)
        {
            if (thetaSteps < 3)
            {
                throw new ArgumentException("Must give at least three theta steps");
            }
            if (phiSteps < 3)
            {
                throw new ArgumentException("Must give at least three phi steps");
            }

            const double radius = 0.5; // always building a unit sphere.
            double x, y, z;
            double theta, phi;
            double tPercent, pPercent;
            int totalPoints = thetaSteps * phiSteps;

            MeshGeometry3D mesh = new MeshGeometry3D();

            // add all the points
            for( int tStep = 0; tStep <= thetaSteps; tStep++)
            {
                for (int pStep = 0; pStep < phiSteps; pStep++)
                {
                    tPercent = (double)tStep / (double)thetaSteps;
                    theta = 2.0 * Math.PI * tPercent;
                    pPercent = (double)pStep / (double)(phiSteps - 1);
                    phi = Math.PI * pPercent;
                    x = radius * Math.Cos(theta) * Math.Sin(phi);
                    y = radius * Math.Cos(phi);
                    z = radius * Math.Sin(theta) * Math.Sin(phi);

                    mesh.Positions.Add(new Point3D(x, y, z));
                    mesh.Normals.Add(new Vector3D(x, y, z));
                    mesh.TextureCoordinates.Add(new Point(tPercent, pPercent));
                }
            }

            // go back over all the points and add triangles
            for( int tStep = 0; tStep < thetaSteps; tStep++)
            {
                for (int pStep = 0; pStep < phiSteps - 1; pStep++)
                {
                    int topLeft = pStep + tStep * phiSteps;
                    int botLeft = topLeft + 1 ;
                    int topRight = topLeft + phiSteps;
                    int botRight = topRight + 1;

                    mesh.TriangleIndices.Add(botLeft);
                    mesh.TriangleIndices.Add(topLeft);
                    mesh.TriangleIndices.Add(botRight);

                    mesh.TriangleIndices.Add(botRight);
                    mesh.TriangleIndices.Add(topLeft);
                    mesh.TriangleIndices.Add(topRight);
                }
            }

            GeometryModel3D model = new GeometryModel3D(mesh, material);
            ModelVisual3D sphere = new ModelVisual3D();
            sphere.Content = model;
            return sphere;
        }

        /// <summary>
        /// Helper method that creates and textures a quad fromt he given points.
        /// </summary>
        public static Model3DGroup CreateQuad(Point3D topLeft, Point3D topRight,
            Point3D botRight, Point3D botLeft, Material material)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(topLeft);
            mesh.Positions.Add(topRight);
            mesh.Positions.Add(botRight);
            mesh.Positions.Add(botLeft);

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);

            //Set the texture to cover the quad.
            mesh.TextureCoordinates.Add(new Point(0, 1));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(1, 0));
            mesh.TextureCoordinates.Add(new Point(0, 0));

            Vector3D normal = CalculateNormal(topLeft, topRight, botRight);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);

            GeometryModel3D model = new GeometryModel3D(
                mesh, material);
            Model3DGroup quad = new Model3DGroup();
            quad.Children.Add(model);
            return quad;
        }

        /// <summary>
        /// Finds the normal for the triangle defined by the three points.
        /// </summary>
        private static Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            var v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            var v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }
    }
}
