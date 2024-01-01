using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyLib
{
    public abstract class IShape
    {
        public abstract string Name { get; }
        public abstract BitmapImage Icon { get; }
        public List<Point> Points { get; set; } = new List<Point>();


        public abstract SolidColorBrush Brush { get; set; }
        public abstract SolidColorBrush Background { get; set; }
        public abstract int Thickness { get; set; }
        public abstract DoubleCollection StrokeDash { get; set; }

        public abstract void HandleShiftMode();




        public abstract bool isHovering(double x, double y); 
        public abstract UIElement Draw(SolidColorBrush brush, int thickness, DoubleCollection dash, SolidColorBrush background );
        public abstract IShape Clone();
        public abstract IShape deepCopy();
        public abstract void pasteShape(Point startPoint, IShape shape);


        public abstract string FromShapeToString();

        public abstract IShape FromStringToShape(string str);

    }
}
