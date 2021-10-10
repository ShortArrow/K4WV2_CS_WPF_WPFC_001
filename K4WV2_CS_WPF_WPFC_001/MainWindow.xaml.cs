using System;
using System.Windows;
using Microsoft.Kinect;
using Microsoft.Kinect.Wpf.Controls;
using System.Windows.Controls;
using Microsoft.Kinect.Toolkit.Input;
using Microsoft.Kinect.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace K4WV2_CS_WPF_WPFC_001
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        
        KinectSensor kinect;
        public MainWindow()
        {
            InitializeComponent();
        }
        void InitializeKinectControls()
        {
            KinectRegion.SetKinectRegion(this, kinectRegion);
            this.kinectRegion.KinectSensor = KinectSensor.GetDefault();
        }
    }
    public class DragDropDecorator : Decorator, IKinectControl
    {
        public bool IsManipulatable
        {
            get
            {
                return true;
            }
        }
        public bool IsPressable
        {
            get
            {
                return false;
            }
        }
        public IKinectController CreateController(IInputModel inputModel,KinectRegion kinectRegion)
        {
            return new DragDropDecoratorController(inputModel, kinectRegion);
        }
    }
    class DragDropDecoratorController : IKinectManipulatableController
    {
        private ManipulatableModel _inputModel;
        private KinectRegion _kinectRegion;
        private DragDropDecorator _dragDropDecorator;
        bool _disposed = false;

        //コンストラクタ
        public DragDropDecoratorController(IInputModel inputModel,KinectRegion kinectRegion)
        {
            _inputModel = inputModel as ManipulatableModel;
            _kinectRegion = kinectRegion;
            _dragDropDecorator = inputModel.Element as DragDropDecorator;
            _inputModel.ManipulationUpdated += inputModel_ManipulationUpdated;    
        }

        //DragDropDecoratorのCanvas.Top,Canvas.Leftプロパティを更新
        void inputModel_ManipulationUpdated(object sender, KinectManipulationUpdatedEventArgs e)
        {
            //DragDropDecoratorの親はCanvas
            Canvas canvas = _dragDropDecorator.Parent as Canvas;
            if (canvas!=null)
            {
                double y = Canvas.GetTop(_dragDropDecorator);
                double x = Canvas.GetLeft(_dragDropDecorator);
                if (double.IsNaN(x)||double.IsNaN(y))
                {
                    return;
                }
                PointF delta = e.Delta.Translation;
                //deltaは-1.0..1.0の相対値で表されているので
                //KinectRegionに合わせて拡大
                var Dy = delta.Y * _kinectRegion.ActualHeight;
                var Dx = delta.X * _kinectRegion.ActualWidth;
                Canvas.SetTop(_dragDropDecorator, y + Dy);
                Canvas.SetLeft(_dragDropDecorator, x + Dx);
            }
        }

        FrameworkElement IKinectController.Element
        {
            get
            {
                return _inputModel.Element as FrameworkElement;
            }
        }

        ManipulatableModel IKinectManipulatableController.ManipulatableInputModel
        {
            get
            {
                return _inputModel;
            }
        }

        void System.IDisposable.Dispose()
        {
            if (!_disposed)
            {
                _kinectRegion = null;
                _inputModel = null;
                _dragDropDecorator = null;
                _inputModel.ManipulationUpdated -= inputModel_ManipulationUpdated;
                _disposed = true;
            }
        }
    }
}
