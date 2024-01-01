
using MyLib;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.IO;
using Path = System.Windows.Shapes.Path;

namespace HeartLib
{
    public class Heart : IShape
    {
        private const char minor_separator_1 = '!';
        private const char minor_separator_2 = ';';
        public override string Name => "Heart";
        public override BitmapImage Icon => new BitmapImage(new Uri("Images/heart.png", UriKind.Relative));
        public override SolidColorBrush Brush { get; set; } = new SolidColorBrush(Colors.Transparent);
        public override SolidColorBrush Background { get; set; } = new SolidColorBrush(Colors.Transparent);
        public override int Thickness { get; set; } = -1;
        public override DoubleCollection StrokeDash { get; set; } = null;
        public override IShape Clone()
        {
            return new Heart();
        }

        public override IShape deepCopy()
        {
            Heart cloneShape = (Heart)this.MemberwiseClone();


            cloneShape.Points = Points.Select(point => new Point(point.X, point.Y)).ToList();

            if (Brush != null) cloneShape.Brush = Brush.Clone();
            if (Background != null) cloneShape.Background = Background.Clone();
            if (StrokeDash != null) cloneShape.StrokeDash = StrokeDash.Clone();

            return cloneShape;
        }
        private Geometry CreateGeometry()
        {
            double width = Math.Abs(Points[1].X - Points[0].X);
            double height = Math.Abs(Points[1].Y - Points[0].Y);

            var pathGeometry = new PathGeometry();

            for (double t = 0; t <= 2 * Math.PI; t += 0.01)
            {
                double x = 17 * Math.Pow(Math.Sin(t), 3);
                double y = 15 * Math.Cos(t) - 5 * Math.Cos(2 * t) - 2 * Math.Cos(3 * t) - Math.Cos(4 * t);

                double screenX = width / 2 + x * (width / 40);
                double screenY = height / 2 - y * (height / 40);

                var point = new Point(screenX, screenY);

                if (t == 0)
                {
                    pathGeometry.Figures.Add(new PathFigure(point, new List<PathSegment>(), false));
                }
                else
                {
                    pathGeometry.Figures[0].Segments.Add(new LineSegment(point, true));
                }
            }

            return pathGeometry;
        }

        public override void pasteShape(Point startPoint, IShape shape)
        {
            double offsetX = startPoint.X - Points[0].X;
            double offsetY = startPoint.Y - Points[0].Y;

            // Update the Points of the pasted shape based on the offset
            Points[0] = new Point(Points[0].X + offsetX, Points[0].Y + offsetY);
            Points[1] = new Point(Points[1].X + offsetX, Points[1].Y + offsetY);
        }
        public override bool isHovering(double x, double y)
        {
            Geometry geometry = CreateGeometry();
            double width = Math.Abs(Points[1].X - Points[0].X);
            double height = Math.Abs(Points[1].Y - Points[0].Y);
            double left = Math.Min(Points[1].X, Points[0].X);
            double top = Math.Max(Points[1].Y, Points[0].Y);

            // Check if the transformed mouse coordinates (x, y) are within the transformed heart shape
            return geometry.FillContains(new Point(x - left, y - top));

        }
        public override UIElement Draw(SolidColorBrush brush, int thickness, DoubleCollection dash, SolidColorBrush b)
        {
            if (Brush == new SolidColorBrush(Colors.Transparent)) { Brush = brush; }

            if (Thickness == -1) { Thickness = thickness; }
            if (StrokeDash == null) { StrokeDash = dash; }
           
            double width = Math.Abs(Points[1].X - Points[0].X);
            double height = Math.Abs(Points[1].Y - Points[0].Y);
            double left = Math.Min(Points[1].X, Points[0].X);
            double top = Math.Max(Points[1].Y, Points[0].Y);

            var element = new Path()
            {
                Width = width,
                Height = height,
                Stroke = brush,
                StrokeThickness = thickness,
                StrokeDashArray = dash,
                Fill = b
            };
            var geometry = new PathGeometry();

            for (double t = 0; t <= 2 * Math.PI; t += 0.01)
            {
                double x = 17 * Math.Pow(Math.Sin(t), 3);
                double y = 15 * Math.Cos(t) - 5 * Math.Cos(2 * t) - 2 * Math.Cos(3 * t) - Math.Cos(4 * t);

                double screenX = width / 2 + x * (width / 40);
                double screenY = height / 2 - y * (height / 40);

                var point = new Point(screenX, screenY);

                if (t == 0)
                {
                    geometry.Figures.Add(new PathFigure(point, new List<PathSegment>(), false));
                }
                else
                {
                    geometry.Figures[0].Segments.Add(new LineSegment(point, true));
                }
            }

            element.Data = geometry;

            // Set the Canvas.Left and Canvas.Top attached properties to position the heart shape
            Canvas.SetLeft(element, left);
            Canvas.SetTop(element, top);

            return element;
        }

        public override void HandleShiftMode()
        {
           
        }

        public override string FromShapeToString()
        {
            string constructed_string = "";

            //<Stroke><Type>:<Brush>;<Thickness>;<Start>;<End>;<Stroke>|....

            constructed_string = new StringBuilder().Append(Name).Append(minor_separator_1).Append(Brush.ToString()).
                Append(minor_separator_2).Append(Thickness).Append(minor_separator_2).Append(Points[0]).
                Append(minor_separator_2).Append(Points[1]).ToString();

            // Check if StrokeDash is null before appending it to the string
            if (StrokeDash != null)
            {
                constructed_string += minor_separator_2 + StrokeDash.ToString();
            }
           
            return constructed_string;
        }

        public override IShape FromStringToShape(string constructed_str)
        {
            if (constructed_str == null)
            {
                throw new ArgumentNullException("Constructed string is null");
            }

            string[] details = constructed_str.Split(new char[] { minor_separator_1, minor_separator_2 });
            Heart shape = new Heart();

            shape.Brush = (SolidColorBrush)new BrushConverter().ConvertFromString(details[1]);
            shape.Thickness = Convert.ToInt32(details[2]);
            if (shape.Points.Count < 2)
            {
                shape.Points.AddRange(new Point[2]);
            }

            // Parse and assign Points

            shape.Points[0] = System.Windows.Point.Parse(details[3]);
            shape.Points[1] = System.Windows.Point.Parse(details[4]);
            if (details.Length > 5)
            {
                // Check if details[5] is not null before attempting to parse
                if (details[5] != null)
                {
                    shape.StrokeDash = DoubleCollection.Parse(details[5]);
                }
                else
                {
                    shape.StrokeDash = null;
                }
            }
            else
            {
                shape.StrokeDash = null;
            }

            return shape;
        }
    }

}
