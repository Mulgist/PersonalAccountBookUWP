using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

namespace PersonalAccountBookUWP.Controller
{
    public sealed partial class AddPage : Page
    {
        private List<Grid> detailGridArray = new List<Grid>();
        private StorageFile file;

        public AddPage()
        {
            this.InitializeComponent();
            detailGridArray.Add(NewDetailGrid());
            UIStackPanel.Children.Add(detailGridArray[0]);
        }

        private async void ReceiptImageButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            await FilePickAsync();
        }

        private void AddDetailButton_Click(object sender, RoutedEventArgs e)
        {
            detailGridArray.Add(NewDetailGrid());
            UIStackPanel.Children.Add(detailGridArray.Last());
        }

        private Grid NewDetailGrid()
        {
            // 세부내역 항목에 대한 Grid
            Grid detailGrid = new Grid();
            ColumnDefinition detailColumn = new ColumnDefinition();
            ColumnDefinition costColumn = new ColumnDefinition();
            TextBox detailTextBox = new TextBox();
            TextBox costTextBox = new TextBox();
            detailColumn.Width = new GridLength(210);
            costColumn.Width = new GridLength(150);
            detailTextBox.Margin = new Thickness(10, 10, 10, 10);
            detailTextBox.PlaceholderText = "구체적인 항목 작성";
            detailTextBox.SetValue(Grid.ColumnProperty, 0);
            costTextBox.Margin = new Thickness(10, 10, 10, 10);
            costTextBox.PlaceholderText = "금액 작성";
            costTextBox.SetValue(Grid.ColumnProperty, 1);

            detailGrid.ColumnDefinitions.Add(detailColumn);
            detailGrid.ColumnDefinitions.Add(costColumn);
            detailGrid.Children.Add(detailTextBox);
            detailGrid.Children.Add(costTextBox);

            return detailGrid;
        }

        private void EraseButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // 이미지 파일 열기
        private async Task FilePickAsync()
        {
            var open = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.Desktop
            };
            open.FileTypeFilter.Add(".jpg");
            open.FileTypeFilter.Add(".png");
            file = await open.PickSingleFileAsync();

            if (file != null)
            {
                // 파일 로드됨. 버튼 이미지를 바꿈
                // FilePathText.Text = file.Path;
                BitmapImage loadedImage = new BitmapImage();
                loadedImage = await LoadImage(file);
                ReceiptImage.Source = loadedImage;
            }
        }

        // StorageFile to BitmapImage
        private static async Task<BitmapImage> LoadImage(StorageFile file)
        {
            BitmapImage bitmapImage = new BitmapImage();
            FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);
            bitmapImage.SetSource(stream);

            return bitmapImage;
        }
    }
}
