using MyLib;
using System;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RectangleLib
{
    public class RectangleShape : IShape
    {
        private const char minor_separator_1 = '!';
        private const char minor_separator_2 = ';';
        public override string Name => "Rectangle";
        public override BitmapImage Icon => new BitmapImage(new Uri("Images/rectangle.png", UriKind.Relative));

        public override SolidColorBrush Brush { get; set; } = new SolidColorBrush(Colors.Transparent);
        public override SolidColorBrush Background { get; set; }
        public override int Thickness { get; set; } = -1;
        public override DoubleCollection StrokeDash { get; set; } = null;

        public override IShape Clone()
        {
            return new RectangleShape();
        }
        public override IShape deepCopy()
        {
            RectangleShape cloneShape = (RectangleShape)this.MemberwiseClone();

            cloneShape.Points = Points.Select(point => new Point(point.X, point.Y)).ToList();

            if (Brush != null) cloneShape.Brush = Brush.Clone();
            if (Background != null) cloneShape.Background = Background.Clone();
            if (StrokeDash != null) cloneShape.StrokeDash = StrokeDash.Clone();
            return cloneShape;
        }
        public override bool isHovering(double x, double y)
        {
            return Util.isBetween(x, Points[0].X, Points[1].X) && Util.isBetween(y, Points[0].Y, Points[1].Y);

        }
        public override void pasteShape(Point startPoint, IShape shape)
        {
            double offsetX = startPoint.X - Points[0].X;
            double offsetY = startPoint.Y - Points[0].Y;

            Points[0] = new Point(Points[0].X + offsetX, Points[0].Y + offsetY);
            Points[1] = new Point(Points[1].X + offsetX, Points[1].Y + offsetY);
                
        }
        public override UIElement Draw(SolidColorBrush brush, int thickness, DoubleCollection dash, SolidColorBrush b)
        {
            double width = Math.Abs(Points[1].X - Points[0].X);
            double height = Math.Abs(Points[1].Y - Points[0].Y);

            var element = new System.Windows.Shapes.Rectangle()
            {
                Width = width,
                Height = height,
                Stroke = brush,
                StrokeThickness = thickness,
                StrokeDashArray = dash,
                Fill = b

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
            // Calculate the center of the rectangle
            double centerX = (Points[0].X + Points[1].X) / 2;
            double centerY = (Points[0].Y + Points[1].Y) / 2;

            // Calculate the new width and height to make the rectangle a square
            double sideLength = Math.Min(Math.Abs(Points[1].X - Points[0].X), Math.Abs(Points[1].Y - Points[0].Y));
            double newWidth = sideLength;
            double newHeight = sideLength;

            // Update the Points to create a bounding box for the square
            Points[0] = new Point(centerX - sideLength / 2, centerY - sideLength / 2);
            Points[1] = new Point(centerX + sideLength / 2, centerY + sideLength / 2);
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
            RectangleShape shape = new RectangleShape();

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
