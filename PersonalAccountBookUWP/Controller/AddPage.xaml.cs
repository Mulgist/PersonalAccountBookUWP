using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Newtonsoft.Json.Linq;

namespace PersonalAccountBookUWP.Controller
{
    public sealed partial class AddPage : Page
    {
        private List<Grid> detailGridArray = new List<Grid>();
        private StorageFile file;

        // DB에 요청, 응답받을 때 필요한 것들
        private JArray objects;

        // 이 페이지에 필요한 리스트. 이 클래스 전역에서 필요함
        private List<Account> accountList = new List<Account>();
        private List<TransactionType> transactionTypelist = new List<TransactionType>();

        public AddPage()
        {
            this.InitializeComponent();

            // ComboBox의 항목 추가
            var accountStringList = new List<string>();
            var incOrDecList = new List<string>();
            var currencyList = new List<string>();

            TransactionDatePicker.Date = Convert.ToDateTime(Convert.ToString(App.localSettings.Values["date"]));

            objects = DataService.instance.GetJsonArrayFromDB("getAccounts");

            // json이 파싱된 객체인 objets를 분해하여 데이터를 알맞게 리스트에 넣는다.
            foreach (JObject element in objects)
            {
                accountList.Add(new Account(element["id"].ToObject<int>(), element["bank"].ToString(), element["name"].ToString(), element["number"].ToString(), element["balance"].ToObject<int>()));
            }

            // 계좌목록 Combobox에 표시할 string을 구성해 새 리스트에 넣고 있다.
            foreach (var account in accountList)
            {
                accountStringList.Add(account.Bank + " " + account.Name + ": " + account.Number);
            }

            // 만들어진 string 리스트를 ComboBox의 소스로 넣는다.
            AccountChooseBox.ItemsSource = accountStringList;

            // 선택 인덱스 설정값에 따라 선택된다.
            AccountChooseBox.SelectedIndex = Convert.ToInt32(App.localSettings.Values["accountIndex"]);

            // -------------------------------------------------------------------------

            // Default Selection
            IncOrDecToggleSwitch.IsOn = true;

            // -------------------------------------------------------------------------

            currencyList.Add("\\");
            currencyList.Add("$");
            currencyList.Add("€");
            currencyList.Add("£");
            currencyList.Add("¥");
            CurrencyComboBox.ItemsSource = currencyList;
            CurrencyComboBox.SelectedIndex = Convert.ToInt32(App.localSettings.Values["currencyIndex"]);

            // -------------------------------------------------------------------------

            SetTransactionTypeComboBox(IncOrDecToggleSwitch.IsOn);

            // -------------------------------------------------------------------------

            // 상세정보입력 레이아웃 추가
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
            ColumnDefinition emptyColumn = new ColumnDefinition();
            TextBox detailTextBox = new TextBox();
            TextBox costTextBox = new TextBox();
            detailColumn.Width = new GridLength(50, GridUnitType.Star);
            costColumn.Width = new GridLength(30, GridUnitType.Star);
            emptyColumn.Width = new GridLength(20, GridUnitType.Star);
            detailTextBox.Margin = new Thickness(10, 10, 10, 10);
            detailTextBox.PlaceholderText = "구체적인 항목 작성";
            detailTextBox.SetValue(Grid.ColumnProperty, 0);
            costTextBox.Margin = new Thickness(10, 10, 10, 10);
            costTextBox.PlaceholderText = "금액 작성";
            costTextBox.SetValue(Grid.ColumnProperty, 1);

            detailGrid.ColumnDefinitions.Add(detailColumn);
            detailGrid.ColumnDefinitions.Add(costColumn);
            detailGrid.ColumnDefinitions.Add(emptyColumn);
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

        private void BankBookSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing,
            // otherwise assume the value got filled in by TextMemberPath
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                //sender.ItemsSource = dataset;
                var items = new List<string>();
                items.Add("df");
                items.Add("sss");
                sender.ItemsSource = new List<string>();
            }
        }

        private void BankBookSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            // Set sender.Text. You can use args.SelectedItem to build your text string.
        }

        private void BankBookSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            /*
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
            }
            else
            {
                // Use args.QueryText to determine what to do.
            }
            */
        }

        private void CardBookSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }

        private void CardBookSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {

        }

        private void CardBookSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }

        private void TodayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            TransactionDatePicker.Date = DateTime.Today;
            TransactionDatePicker.IsEnabled = false;
        }

        private void TodayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            TransactionDatePicker.Date = Convert.ToDateTime(Convert.ToString(App.localSettings.Values["date"]));
            TransactionDatePicker.IsEnabled = true;
        }

        private void IncOrDecToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            SetTransactionTypeComboBox(IncOrDecToggleSwitch.IsOn);
        }

        private void SetTransactionTypeComboBox(bool isOn)
        {
            var transactionTypeStringlist = new List<string>();

            transactionTypelist.Clear();
            if (isOn)
            {
                objects = DataService.instance.GetJsonArrayFromDB("getAddingTypes");
            }
            else
            {
                objects = DataService.instance.GetJsonArrayFromDB("getSubtractingTypes");
            }

            foreach (JObject element in objects)
            {
                transactionTypelist.Add(new TransactionType(element["id"].ToObject<int>(), element["name"].ToString()));
            }

            foreach (var transactionType in transactionTypelist)
            {
                transactionTypeStringlist.Add(transactionType.Name);
            }
            TransactionTypeComboBox.ItemsSource = transactionTypeStringlist;
            if (isOn)
            {
                TransactionTypeComboBox.SelectedIndex = Convert.ToInt32(App.localSettings.Values["plusTtypeIndex"]);
            }
            else
            {
                TransactionTypeComboBox.SelectedIndex = Convert.ToInt32(App.localSettings.Values["minusTypeIndex"]);
            }
        }
    }
}
