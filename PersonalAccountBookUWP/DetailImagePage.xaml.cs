using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        private StorageFile file = null;

        public DetailImagePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            file = e.Parameter as StorageFile;

            // file로 이미지 로드
            BitmapImage loadedImage = new BitmapImage();

            // 비동기를 동기 위에서 돌리기 (데드락 주의)
            // loadedImage = Task.Run(async () => { return await LoadImage(file); }).Result;
        }

        // StorageFile to BitmapImage
        private static async Task<BitmapImage> LoadImage(StorageFile file)
        {
            BitmapImage bitmapImage = new BitmapImage();
            FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);
            bitmapImage.SetSource(stream);

            return bitmapImage;
        }

        private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {
            // 이미지 로드하기
            BitmapImage loadedImage = new BitmapImage();
            loadedImage = await LoadImage(file);
            DetailedImage.Source = loadedImage;
        }
    }
}
