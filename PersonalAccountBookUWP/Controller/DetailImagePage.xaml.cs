using System;
using System.Threading.Tasks;
using Windows.Storage;
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
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class DetailImagePage : Page
    {
        private StorageFile file;

        public DetailImagePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            
            file = e.Parameter as StorageFile;

            // 비동기를 동기 위에서 돌리기 (데드락 주의)
            // loadedImage = Task.Run(async () => { return await LoadImage(file); }).Result;
        }

        private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {
            // 이미지 로드하기
            BitmapImage loadedImage = new BitmapImage();
            loadedImage = await Utility.instance.LoadImage(file);
            DetailedImage.Source = loadedImage;
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
