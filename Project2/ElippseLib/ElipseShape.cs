using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using MyLib;
using System.Security.Policy;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Text;

namespace ElippseLib
{
    public class EllipseShape : IShape
    {
        private const char minor_separator_1 = '!';
        private const char minor_separator_2 = ';';
        public override string Name => "Ellipse";
        public override BitmapImage Icon => new BitmapImage(new Uri("Images/ellipse.png", UriKind.Relative));
        public override SolidColorBrush Brush { get; set; } = new SolidColorBrush(Colors.Transparent);
        public override SolidColorBrush Background { get; set; } = new SolidColorBrush(Colors.Transparent);
        public override int Thickness { get; set; } = -1;
        public override DoubleCollection StrokeDash { get; set; } = null;
        public override IShape Clone()
        {
            return new EllipseShape();
        }

        public override IShape deepCopy()
        {
            EllipseShape cloneShape = (EllipseShape)this.MemberwiseClone();

            
            cloneShape.Points = Points.Select(point => new Point(point.X, point.Y)).ToList();

            if (Brush != null) cloneShape.Brush = Brush.Clone();
            if (Background != null) cloneShape.Background = Background.Clone();
            if (StrokeDash != null) cloneShape.StrokeDash = StrokeDash.Clone();

            return cloneShape;
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
            return Util.isBetween(x, Points[1].X, Points[0].X) && Util.isBetween(y, Points[1].Y, Points[0].Y);

        }
        public override UIElement Draw(SolidColorBrush brush, int thickness, DoubleCollection dash, SolidColorBrush b)
        {
            if (Brush == new SolidColorBrush(Colors.Transparent)) { Brush = brush; }

            if (Thickness == -1) { Thickness = thickness; }
            if (StrokeDash == null) { StrokeDash = dash; }
            // TODO: can dam bao Diem 0 < Diem 1
            double width = Math.Abs(Points[1].X - Points[0].X);
            double height = Math.Abs(Points[1].Y - Points[0].Y);

            var element = new Ellipse()
            {
                Width = width,
                Height = height,
                Stroke = brush,
                StrokeThickness = thickness,
                StrokeDashArray = dash,
                Fill= b
            };
            if (Points[1].X > Points[0].X && Points[1].Y > Points[0].Y)
            {
                Canvas.SetLeft(element, Points[0].X);
                Canvas.SetTop(element, Points[0].Y);
            }
            else if (Points[1].X < Points[0].X && Points[1].Y > Points[0].Y)
            {
                Canvas.SetLeft(element, Points[1].X);
                Canvas.SetTop(element, Points[0].Y);
            }
            else if (Points[1].X > Points[0].X && Points[1].Y < Points[0].Y)
            {
                Canvas.SetLeft(element, Points[0].X);
                Canvas.SetTop(element, Points[1].Y);
            }
            else
            {
                Canvas.SetLeft(element, Points[1].X);
                Canvas.SetTop(element, Points[1].Y);
            }


            return element;
        }

        public override void HandleShiftMode()
        {
            // Calculate the center of the ellipse
            double centerX = (Points[0].X + Points[1].X) / 2;
            double centerY = (Points[0].Y + Points[1].Y) / 2;

            // Calculate the new width and height to make the ellipse a circle
            double radius = Math.Min(Math.Abs(Points[1].X - Points[0].X), Math.Abs(Points[1].Y - Points[0].Y)) / 2;
            double newWidth = radius * 2;
            double newHeight = radius * 2;

            // Update the Points to create a bounding box for the circle
            Points[0] = new Point(centerX - radius, centerY - radius);
            Points[1] = new Point(centerX + radius, centerY + radius);
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
            EllipseShape shape = new EllipseShape();

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
