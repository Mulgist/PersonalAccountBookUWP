using System;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace PersonalAccountBookUWP
{
    public sealed partial class DetailImagePage : Page
    {
        private IBuffer buffer;

        public DetailImagePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Bitmap Parameter 문제와 페이지들 간의 문제로 이미지를 띄우는 방식을
            // 일단 IBuffer로 Parameter를 받은 후 BitmapImage로 변환 뒤 이미지를 뛰우게 구성되었다.

            buffer = e.Parameter as IBuffer;

            // 비동기를 동기 위에서 돌리기 (데드락 주의)
            // loadedImage = Task.Run(async () => { return await LoadImage(file); }).Result;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // 버퍼를 이미지로 변환
            BitmapImage image = Utility.instance.BufferToImageAsync(buffer).Result;
            
            DetailedImage.Source = image;
        }

        // 더블클릭하면 확대됨
        private async void ScrollViewer_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            var doubleTapPoint = e.GetPosition(scrollViewer);

            if (scrollViewer.ZoomFactor != 1)
            {
                scrollViewer.ChangeView(null, null, 1);
            }
            else if (scrollViewer.ZoomFactor == 1)
            {
                scrollViewer.ChangeView(null, null, 2);

                var dispatcher = Window.Current.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    scrollViewer.ChangeView(doubleTapPoint.X, doubleTapPoint.Y, 2);
                });
            }

        }
    }
}
