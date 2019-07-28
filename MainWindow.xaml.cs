using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf;


namespace Display3D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;



    using HelixToolkit.Wpf;



    /// <summary>
    /// Interaction logic for the main window.
    /// </summary>

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public partial class MainWindow
    {
        private readonly Stopwatch watch = new Stopwatch();

        private Model3DGroup MainModel3DGroup = new Model3DGroup();

        private int xmin, xmax, dx, dz, zmin, zmax, texture_xscale, texture_zscale;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DefineModel(MainModel3DGroup, MakeData());
            ModelVisual3D nvisual3D = new ModelVisual3D();
            nvisual3D.Content = MainModel3DGroup;
            View1.Children.Add(nvisual3D);
        }

        //private PointGeometryModel3D t;
        public MainWindow()
        {
            this.InitializeComponent();

            this.DataContext = this;

            View1.RotateGesture = new MouseGesture(MouseAction.LeftClick);
            

        }
        // Make the data.
        private double[,] MakeData()
        {
            double[,] values =
            {
                {0,0,0,1,2,2,1,0,0,0},
                {0,0,2,3,3,3,3,2,0,0},
                {0,2,3,4,4,4,4,3,2,0},
                {2,3,4,5,5,5,5,4,3,2},
                {3,4,5,6,7,7,6,5,4,3},
                {3,4,5,6,7,7,6,5,4,3},
                {2,3,4,5,5,5,5,4,3,2},
                {0,2,3,4,4,4,4,3,2,0},
                {0,0,2,3,3,3,3,2,0,0},
                {1,0,2,3,3,3,3,2,0,0}
            };

            xmin = 0;
            xmax = values.GetUpperBound(0);
            dx = 1;
            zmin = 0;
            zmax = values.GetUpperBound(1);
            dz = 1;

            texture_xscale = (xmax - xmin);
            texture_zscale = (zmax - zmin);

            return values;
        }

        // Add the model to the Model3DGroup.
        private void DefineModel(Model3DGroup model_group, double[,] values)
        {
            // Make a mesh to hold the surface.
            MeshGeometry3D mesh = new MeshGeometry3D();

            // Make the surface's points and triangles.
            float offset_x = xmax / 2f;
            float offset_z = zmax / 2f;
            for (int x = xmin; x <= xmax - dx; x += dx)
            {
                for (int z = zmin; z <= zmax - dz; z += dx)
                {
                    // Make points at the corners of the surface
                    // over (x, z) - (x + dx, z + dz).
                    Point3D p00 = new Point3D(x - offset_x, values[x, z], z - offset_z);
                    Point3D p10 = new Point3D(x - offset_x + dx, values[x + dx, z], z - offset_z);
                    Point3D p01 = new Point3D(x - offset_x, values[x, z + dz], z - offset_z + dz);
                    Point3D p11 = new Point3D(x - offset_x + dx, values[x + dx, z + dz], z - offset_z + dz);

                    // Add the triangles.
                    AddTriangle(mesh, p00, p01, p11);
                    AddTriangle(mesh, p00, p11, p10);
                }
            }
            Console.WriteLine(mesh.Positions.Count + " points");
            Console.WriteLine(mesh.TriangleIndices.Count / 3 + " triangles");
            Console.WriteLine();
         

            DiffuseMaterial surface_material = new DiffuseMaterial(Brushes.Orange);

            // Make the mesh's model.
            GeometryModel3D surface_model = new GeometryModel3D(mesh, surface_material);

            // Make the surface visible from both sides.
            surface_model.BackMaterial = surface_material;

            // Add the model to the model groups.
            model_group.Children.Add(surface_model);
        }

        private int AddPoint(Point3DCollection points, Point3D point)
        {
            // See if the point exists.
            for (int i = 0; i < points.Count; i++)
            {
                if ((point.X == points[i].X) && (point.Y == points[i].Y) && (point.Z == points[i].Z))
                    return i;
            }

            // We didn't find the point. Create it.
            points.Add(point);
            return points.Count - 1;
        }

        private void AddTriangle(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Point3D point3)
        {
            // Get the points' indices.
            int index1 = AddPoint(mesh.Positions, point1);
            int index2 = AddPoint(mesh.Positions, point2);
            int index3 = AddPoint(mesh.Positions, point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index1);
            mesh.TriangleIndices.Add(index2);
            mesh.TriangleIndices.Add(index3);           
        }
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}