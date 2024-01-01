using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Paint.Features
{
    public enum ZoomType
    {
        IN,
        OUT,
        DEFAULT
    }

    internal class Zoom : Command
    {
        public static readonly float[] ZOOM_VALUE = { 0.25f, 0.5f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f};

        public static readonly float MIN_VALUE = ZOOM_VALUE[0];
        public static readonly float MAX_VALUE = ZOOM_VALUE.Last();
        public static readonly float DEFAULT_VALUE = 1f;

        ZoomType _zoomType;

        public float findNearestValue(float[] a, float value)
        {
            float result = a[0] ;
            for (int i = 0; i < a.Length - 1; i++)
            {
                if (value >= a[i] && value <= a[i + 1])
                {
                    if (Math.Abs(value - a[i]) < Math.Abs(value - a[i + 1])) 
                        result = a[i];
                    else
                        result = a[i + 1];
                }
            }
            return result;
        }

        public Zoom(MainWindow app, ZoomType zoomType) : base(app)
        {
            _zoomType = zoomType;
        }

        public override bool Action()
        {
            float zoomRatio = findNearestValue(ZOOM_VALUE, _app.zoomRatio);
            int index = Array.IndexOf(ZOOM_VALUE, zoomRatio);

            float newZoomRatio = zoomRatio;
            switch (_zoomType)
            {
                case ZoomType.IN:
                    if (zoomRatio > MIN_VALUE) newZoomRatio = ZOOM_VALUE[--index];
                    break;
                case ZoomType.OUT:
                    if (zoomRatio < MAX_VALUE) newZoomRatio = ZOOM_VALUE[++index];
                    break;
                default:
                    newZoomRatio = DEFAULT_VALUE;
                    break;
            }

            if (zoomRatio != newZoomRatio)
            {
                _app.zoomRatio = newZoomRatio;
            }

            return false;
        }
    }
}
