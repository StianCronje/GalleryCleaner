using System;
using System.Threading.Tasks;
using GalleryCleaner.ViewModels;
using Xamarin.Forms;

namespace GalleryCleaner.Views
{
    public partial class ImageStackPage : ContentPage
    {
        private double _startPosX = 0;
        private double _startPosY = 0;
        private double _rotationRadius = 0;
        private double _screenWidth = 0;
        private double _screenHeight = 0;

        private double _x = 0;
        private double _y = 0;
        private double _time = 0;
        private double _prevX = 0;
        private double _prevY = 0;
        private double _prevTime = 0;
        private double _deltaX = 0;
        private double _deltaY = 0;
        private double _deltaTime = 0;

        private bool _canMove = true;

        private ImageStackViewModel _viewModel;

        public ImageStackPage()
        {
            InitializeComponent();

            _screenWidth = Application.Current.MainPage.Width;
            _screenHeight = Application.Current.MainPage.Height;
            _rotationRadius = _screenHeight * 2;

            _viewModel = this.BindingContext as ImageStackViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _viewModel.LoadImages();
        }

        async void PanGestureRecognizer_PanUpdated(System.Object sender, Xamarin.Forms.PanUpdatedEventArgs e)
        {
            if (!_canMove)
                return;

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    _startPosX = Frame.TranslationX;
                    _startPosY = Frame.TranslationY;
                    break;
                case GestureStatus.Running:
                    _x = _startPosX + e.TotalX;
                    _y = _startPosY + e.TotalY;
                    _time = DateTime.Now.Millisecond;
                    _deltaX = _x - _prevX;
                    _deltaY = _y - _prevY;
                    _deltaTime = _time - _prevTime;


                    Frame.TranslationX = _x;
                    Frame.TranslationY = _y;
                    Frame.Rotation = GetRotationAngle(_x, _y, _rotationRadius);

                    acceptBox.Opacity = -Frame.TranslationX / (_screenWidth / 2);
                    rejectBox.Opacity = Frame.TranslationX / (_screenWidth / 2);
                    specialBox.Opacity = -Frame.TranslationY / (_screenHeight / 2);
                    break;
                case GestureStatus.Completed:
                    var maxTagetX = _screenWidth;
                    var maxTagetY = _screenHeight;

                    var stepsToTargetX = Math.Abs(maxTagetX / _deltaX);
                    var stepsToTargetY = Math.Abs(maxTagetY / _deltaY);
                    var stepsToTarget = Math.Min(stepsToTargetX, stepsToTargetY);

                    var targetX = _deltaX * stepsToTarget;
                    var targetY = _deltaY * stepsToTarget;
                    var targetTime = _deltaTime * stepsToTarget;
                    uint animTime = Math.Min((uint)targetTime, 350);
                    
                    Console.WriteLine($"tx: {targetX}, ty: {targetY}, time: { targetTime}");

                    if(!double.IsInfinity(targetTime) && !double.IsNaN(targetTime) && targetTime > 0)
                    {
                        _canMove = false;
                        await Task.WhenAll(
                            Frame.TranslateTo(targetX, targetY, animTime),
                            Frame.RotateTo(GetRotationAngle(targetX, targetY, _rotationRadius), animTime)
                        );
                        _canMove = true;
                    }

                    Frame.TranslationX = 0;
                    Frame.TranslationY = 0;
                    Frame.Rotation = 0;
                    acceptBox.Opacity = 0;
                    rejectBox.Opacity = 0;
                    specialBox.Opacity = 0;
                    break;
                default:
                    break;
            }

            _prevX = _x;
            _prevY = _y;
            _prevTime = _time;
        }

        double GetRotationAngle(double posX, double posY, double radius)
        {
            var rad = Math.Atan2(posY - radius, posX);
            var deg = rad * (180 / Math.PI);
            return deg + 90;
        }

        double Lerp(double from, double to, double by)
        {
            return from * (1 - by) + to * by;
        }
    }
}
