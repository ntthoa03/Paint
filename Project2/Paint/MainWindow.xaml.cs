using MyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyLib;
using Fluent;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Newtonsoft.Json;
using System.Windows.Controls.Ribbon;
using System.ComponentModel;

using System.Diagnostics;
using Paint.Features;


namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Fluent.RibbonWindow, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            KeyDown += new KeyEventHandler(OnButtonKeyDown);
            KeyUp += new KeyEventHandler(OnButtonKeyUp);
            this.DataContext = this;
        }

        ShapeFactory _factory;


        bool isDrawing = false;
        bool _isFilling = false;
        bool _isSaved = false;

        Point _start;
        Point _end;
        public Point _newStartPoint;

        private string _choice=""; // Line

        public List<IShape> _shapes = new List<IShape>();
        public IShape _preview = null;

        public List<IShape> abilities = new List<IShape>();

        // Shapes properties
        public static int _currentThickness = 1;
        public SolidColorBrush _currentColor = new SolidColorBrush(Colors.Black);
        public static DoubleCollection _currentDash = null;
        public SolidColorBrush _currentBackGround = Brushes.Transparent;
        public string _backgroundImagePath = "";
        
        private MyFile MyFile;
        
        public IShape? _chooseShape = null;
        public IShape? _clipBoard = null;

        bool shiftMode;
        bool ctrlMode;
        Dictionary<int, List<Image>> images = new Dictionary<int, List<Image>>();
        List<IShape> redos = new List<IShape>();

        public float zoomRatio { get; set; } = Zoom.DEFAULT_VALUE;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Do tim cac kha nang
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            var fis = (new DirectoryInfo(folder)).GetFiles("*.dll");

            foreach (var fi in fis)
            {
                var assembly = Assembly.LoadFrom(fi.FullName);
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsClass & (!type.IsAbstract))
                    {
                        if (typeof(IShape).IsAssignableFrom(type))
                        {
                            var shape = Activator.CreateInstance(type) as IShape;
                            abilities.Add(shape!);
                        }
                    }
                }
            }

            iconListView.ItemsSource = abilities;

            _factory = new ShapeFactory();
            foreach (var ability in abilities)
            {
                _factory.Prototypes.Add(
                    ability.Name, ability
                );         
            };

            if (abilities.Count > 0)
            {
                _choice = abilities[0].Name;
                _preview = _factory.Create(_choice);
            }
            redoButton.IsEnabled = undoButton.IsEnabled = false;

            MyFile = new MyFile();
            MyFile.ReferenceAbilities = _factory.Prototypes;
        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.abilities.Count == 0)
                return;


            if (e.ChangedButton == MouseButton.Left)
            {
                isDrawing = true;
                _start = e.GetPosition(drawingCanvas);
                _preview.Points.Add(_start);
            }
            
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {

            if (isDrawing)
            {
                _end = e.GetPosition(drawingCanvas);

                Title = $"({_start.X}, {_start.Y}; {_end.X}, {_end.Y})";

                _preview = _factory.Create(_choice);
                _preview.Points.Add(_start);
                _preview.Points.Add(_end);

                drawingCanvas.Children.Clear();
                if (shiftMode)
                {
                    _preview.HandleShiftMode();
                }
                foreach (var shape in _shapes)
                {

                    drawingCanvas.Children.Add(shape.Draw(shape.Brush, shape.Thickness, shape.StrokeDash,shape.Background));
                }

                drawingCanvas.Children.Add(_preview.Draw(_currentColor, _currentThickness, _currentDash, _currentBackGround));
            }       
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {

            if (e.ChangedButton == MouseButton.Left)
            {
                _end = e.GetPosition(drawingCanvas);
                _preview.Points.Add(_end);
                _preview.Brush = _currentColor;
                _preview.Thickness = _currentThickness;
                _preview.StrokeDash = _currentDash;
                _preview.Background = _currentBackGround;

                if (Math.Abs(_start.X - _end.X) < 2 && Math.Abs(_start.Y - _end.Y) < 2)
                {
                    foreach (var item in _shapes)
                    {
                        if (item.isHovering(_start.X, _start.Y))
                        {
                            _chooseShape = item;

                            if (_isFilling == true)
                            {
                                Command.actionCommand(new Fill(this));
                                RedrawCanvas();

                                _isFilling = false;
                            }
                        }
                    }
                }
                if (shiftMode)
                {
                    _preview.HandleShiftMode();
                }
                Command.actionCommand(new Draw(this));
                undoButton.IsEnabled = true;
                redoButton.IsEnabled = false;
              
                isDrawing = false;
                _isSaved = false;
                // Move to new preview 
                _preview = _factory.Create(_choice);
                RedrawCanvas();

            }       

        }


        private void ResetToDefault()
        {
            if (this.abilities.Count == 0)
                return;

            _isSaved = false;
            isDrawing = false;
           
            _shapes.Clear();

            _choice = abilities[0].Name;
            _preview = _factory.Create(_choice);

            _currentThickness = 1;
            _currentColor = new SolidColorBrush(Colors.Black);
            _currentDash = null;

            _backgroundImagePath = "";

            dashComboBox.SelectedIndex = 0;
            sizeComboBox.SelectedIndex = 0;

            drawingCanvas.Children.Clear();
            drawingCanvas.Background = new SolidColorBrush(Colors.White);
        }

        private void CreateNewButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isSaved)
            {
                ResetToDefault();
                return;
            }
            if (_shapes.Count == 0)
            {
                return;
            }

            if (_backgroundImagePath.Length > 0 && _shapes.Count == 0)
            {
                _backgroundImagePath = "";
                drawingCanvas.Background = new SolidColorBrush(Colors.White);
            }
            var result = MessageBox.Show("Save your changes to this file?", "Unsaved changes detected", MessageBoxButton.YesNoCancel);

            if (MessageBoxResult.Yes == result)
            {

                Save_File();
                ResetToDefault();
                _isSaved = true;
            }
            else if (MessageBoxResult.No == result)
            {
                ResetToDefault();
                return;
            }
            else if (MessageBoxResult.Cancel == result)
            {
                return;
            }

        }

        private void Save_File()
        {
            if (MyFile.isNewFile())
            {
                MyFile.SaveFileDialog.CheckFileExists = false;
                bool? check = MyFile.SaveFileDialog.ShowDialog();
            
                if (check != null && check == true)
                {
                    string path = MyFile.SaveFileDialog.FileName;
                    MyFile.CurrentStoredPath = path;
                    int write_mode = 1;
                    if (MyFile.SaveFileDialog.FilterIndex == 1)
                    {
                        write_mode = MyFile.BINARY_FILE;
                    }
                    else if (MyFile.SaveFileDialog.FilterIndex == 2)
                    {
                        write_mode = MyFile.XML_FILE;
                    }
                    MyFile.WriteTo(MyFile.CurrentStoredPath, _shapes, write_mode);
                }
            }
            else
            {
                string? ext = System.IO.Path.GetExtension(MyFile.CurrentStoredPath);
                if (ext != null)
                {
                    Debug.WriteLine(ext);

                    if (ext.Equals(MyFile.MPXML_EXT)) // xml mode
                    {
                        MyFile.WriteTo(MyFile.CurrentStoredPath!, _shapes, MyFile.XML_FILE);
                    }
                    else //binary mode
                    {
                        MyFile.WriteTo(MyFile.CurrentStoredPath!, _shapes, MyFile.BINARY_FILE);
                    }
                }

            }
        }
        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {

            if (_shapes.Count == 0)
            {
                return;
            }


            if (_isSaved)
            {
                ResetToDefault();
                return;
            }

            var result = MessageBox.Show("Do you want to save current file?", "Unsaved changes detected", MessageBoxButton.YesNoCancel);

            if (MessageBoxResult.Yes == result)
            {
                // save 
                Save_File();
                _isSaved = true;
            }
            else if (MessageBoxResult.No == result)
            {
                return;
            }
            else if (MessageBoxResult.Cancel == result)
            {
                return;
            }
        }

        private void Open_File()
        {
            drawingCanvas.Children.Clear();
            string SelectedFile = "";
            bool? check = MyFile.OpenFileDialog.ShowDialog();
            if (check != null && check == true)
            {
                int selectedIndex = MyFile.OpenFileDialog.FilterIndex;
                int mode = 0;
                if (selectedIndex == 1)
                {
                    mode = MyFile.BINARY_FILE;
                }
                else if (selectedIndex == 2)
                {
                    mode = MyFile.XML_FILE;
                }

                SelectedFile = MyFile.OpenFileDialog.FileName;
                Debug.WriteLine($"{SelectedFile} is selected");
                MyFile.CurrentStoredPath = SelectedFile;
                _shapes = MyFile.ReadFrom(MyFile.CurrentStoredPath, mode);


            }
            foreach (var shape in _shapes)
            {
                var element = shape.Draw(shape.Brush, shape.Thickness, shape.StrokeDash,shape.Background);
                drawingCanvas.Children.Add((UIElement)element);
            }

        }
        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            Open_File();
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.Filter = "PNG (*.png)|*.png| JPEG (*.jpeg)|*.jpeg| BMP (*.bmp)|*.bmp | TIFF (*.tiff)|*.tiff";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;
                string extension = System.IO.Path.GetExtension(path).TrimStart('.');
                SaveCanvasToImage(drawingCanvas, path, extension);
            }
            _isSaved = true;
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "PNG (*.png)|*.png| JPEG (*.jpeg)|*.jpeg| BMP (*.bmp)|*.bmp";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;
                _backgroundImagePath = path;
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(path, UriKind.Absolute));
                drawingCanvas.Background = brush;
            }

        }
        private static void SaveCanvasToImage(Canvas canvas, string filename, string extension = "png")
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
             (int)canvas.Width, (int)canvas.Height,
             96d, 96d, PixelFormats.Pbgra32);
            // needed otherwise the image output is black
            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));
            renderBitmap.Render(canvas);

            switch (extension)
            {
                case "png":
                    PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
                    pngEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    using (FileStream file = File.Create(filename))
                    {
                        pngEncoder.Save(file);
                    }
                    break;
                case "jpeg":
                    JpegBitmapEncoder jpegEncoder = new JpegBitmapEncoder();
                    jpegEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    using (FileStream file = File.Create(filename))
                    {
                        jpegEncoder.Save(file);
                    }
                    break;
                case "tiff":
                    TiffBitmapEncoder tiffEncoder = new TiffBitmapEncoder();
                    tiffEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    using (FileStream file = File.Create(filename))
                    {
                        tiffEncoder.Save(file);
                    }
                    break;
                case "bmp":

                    BmpBitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                    bitmapEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    using (FileStream file = File.Create(filename))
                    {
                        bitmapEncoder.Save(file);
                    }
                    break;
                default:
                    break;
            }
        }



        private void iconListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.abilities.Count == 0)
                return;

            var index = iconListView.SelectedIndex;

            _choice = abilities[index].Name;

            _preview = _factory.Create(_choice);
        }

        private void dashComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = dashComboBox.SelectedIndex;

            switch (index)
            {
                case 0:
                    _currentDash = null;
                    break;
                case 1:
                    _currentDash = new DoubleCollection() { 4, 1, 1, 1, 1, 1 };
                    break;
                case 2:
                    _currentDash = new DoubleCollection() { 1, 1 };
                    break;
                case 3:
                    _currentDash = new DoubleCollection() { 6, 1 };
                    break;
                default:
                    break;
            }
        }

        private void sizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = sizeComboBox.SelectedIndex;

            switch (index)
            {
                case 0:
                    _currentThickness = 1;
                    break;
                case 1:
                    _currentThickness = 2;
                    break;
                case 2:
                    _currentThickness = 3;
                    break;
                case 3:
                    _currentThickness = 5;
                    break;
                default:
                    break;
            }
        }

        private void btnBasicBlack_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        }

        private void btnBasicOrange_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(255, 165, 0));
        }

        private void btnBasicYellow_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(255, 255, 0));
        }

        private void btnBasicBlue_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(0, 0, 255));
        }

        private void btnBasicGreen_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(0, 255, 0));
        }

        private void btnBasicPurple_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(191, 64, 191));
        }

        private void btnBasicPink_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(255, 182, 193));
        }

        private void btnBasicBrown_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(160, 82, 45));
        }

        private void btnBasicGray_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(192, 192, 192));
        }

        private void btnBasicRed_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        }

        private void editColorButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorPicker = new System.Windows.Forms.ColorDialog();

            if (colorPicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _currentColor = new SolidColorBrush(Color.FromRgb(colorPicker.Color.R, colorPicker.Color.G, colorPicker.Color.B));
            }
        }


        private void undoButton_Click(object sender, RoutedEventArgs e)
        {           

            Command.actionCommand(new Undo(this));
            RedrawCanvas();
        }

        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
           
            Command.actionCommand(new Redo(this));
            RedrawCanvas();
        }

        private void CutButton_Click(object sender, RoutedEventArgs e)
        {
            Command.actionCommand(new Cut(this));
            RedrawCanvas();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            
            Command.actionCommand(new Copy(this));


        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            Command.actionCommand(new Paste(this));
            RedrawCanvas();
        }
        private void RedrawCanvas()
        {
            drawingCanvas.Children.Clear();

            redoButton.IsEnabled = Command._redo.count() > 0;
            undoButton.IsEnabled = Command._undo.count() > 0 && _shapes.Count > 0;

            foreach (var shape in _shapes)
            {
                drawingCanvas.Children.Add(shape.Draw(shape.Brush, shape.Thickness, shape.StrokeDash, shape.Background));
            }


        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _newStartPoint = e.GetPosition(drawingCanvas);
        }

        private void OnButtonKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                shiftMode = false;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                ctrlMode = false;
            }
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                shiftMode = true;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                ctrlMode = true;
            }
            else if (e.Key == Key.Z && ctrlMode)
            {
                if (images.ContainsKey(0) && _shapes.Count == 0)
                {
                    if (images[0].Count > 0)
                    {
                        images[0].RemoveAt(images[0].Count - 1);
                        drawingCanvas.Children.RemoveAt(drawingCanvas.Children.Count - 1);
                    }
                }
                if (_shapes.Count > 0)
                {
                    if (images.ContainsKey(_shapes.Count) && images[_shapes.Count].Count > 0)
                    {
                        images[_shapes.Count].RemoveAt(images[_shapes.Count].Count - 1);
                    }
                    else
                    {
                        redos.Add(_shapes[_shapes.Count - 1]);
                        _shapes.RemoveAt(_shapes.Count - 1);
                    }
                    drawingCanvas.Children.RemoveAt(drawingCanvas.Children.Count - 1);
                }
            }
            else if (e.Key == Key.Y && ctrlMode)
            {
                if (redos.Count > 0)
                {

                    _shapes.Add(redos[redos.Count - 1]);
                    drawingCanvas.Children.Add(_shapes[_shapes.Count - 1].Draw(_currentColor, _currentThickness, _currentDash, _currentBackGround));
                    redos.RemoveAt(redos.Count - 1);
                }
            }
        }
        private void uiZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }


        private void zoomInButton_Click(object sender, RoutedEventArgs e)
        {
            Command.actionCommand(new Zoom(this, ZoomType.IN));
        }

        private void zoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            Command.actionCommand(new Zoom(this, ZoomType.OUT));
        }

        private void zoom100Button_Click(object sender, RoutedEventArgs e)
        {
            Command.actionCommand(new Zoom(this, ZoomType.DEFAULT));
        }
        private void fillButton_Click(object sender, RoutedEventArgs e)
        {
            _isFilling = !_isFilling;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
