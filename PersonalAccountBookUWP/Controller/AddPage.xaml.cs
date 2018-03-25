using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Newtonsoft.Json.Linq;
using Windows.UI.Popups;
using System.Collections.ObjectModel;
using System.Text;

namespace PersonalAccountBookUWP.Controller
{
    public sealed partial class AddPage : Page
    {
        private List<Grid> detailGridArray = new List<Grid>();
        private StorageFile file;

        // 요청할 때 사용하는 자료구조
        private Dictionary<string, string> requestDic = new Dictionary<string, string>();

        // DB에 요청, 응답받을 때 필요한 것들
        private JArray objects;

        // 이 페이지에 필요한 리스트. 이 클래스 전역에서 필요함
        private List<Account> accountList = new List<Account>();
        private List<TransactionType> transactionTypelist = new List<TransactionType>();
        private ObservableCollection<string> bankBookSuggestions = new ObservableCollection<string>();
        private ObservableCollection<string> cardBookSuggestions = new ObservableCollection<string>();
        private ObservableCollection<string> detailBookSuggestions = new ObservableCollection<string>();

        public AddPage()
        {
            InitializeComponent();

            // ComboBox의 항목 추가
            var accountStringList = new List<string>();
            var incOrDecList = new List<string>();
            var currencyList = new List<string>();

            TransactionDatePicker.Date = Convert.ToDateTime(Convert.ToString(App.localSettings.Values["date"]));

            requestDic.Clear();
            requestDic.Add((string)App.MethodElement.Element("do"), (string)App.MethodElement.Element("getAccounts"));

            objects = DataService.instance.GetJsonArrayFromDB(requestDic);

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
            IncOrDecToggleSwitch.IsOn = Convert.ToBoolean(App.localSettings.Values["inOrDec"]);

            // -------------------------------------------------------------------------

            currencyList.Add("\\");
            // currencyList.Add("$");
            // currencyList.Add("€");
            // currencyList.Add("£");
            // currencyList.Add("¥");
            CurrencyComboBox.ItemsSource = currencyList;
            CurrencyComboBox.SelectedIndex = Convert.ToInt32(App.localSettings.Values["currencyIndex"]);

            // -------------------------------------------------------------------------

            SetTransactionTypeComboBox(IncOrDecToggleSwitch.IsOn);

            // -------------------------------------------------------------------------

            // 상세정보입력 레이아웃 추가
            detailGridArray.Add(NewDetailGrid());
            UIStackPanel.Children.Add(detailGridArray[0]);
        }

        // 오늘 체크박스 체크할 시 작업하는 함수
        private void TodayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            TransactionDatePicker.Date = DateTime.Today;
            TransactionDatePicker.IsEnabled = false;
        }

        // 오늘 체크박스 체크 해제 시
        private void TodayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            TransactionDatePicker.Date = Convert.ToDateTime(Convert.ToString(App.localSettings.Values["date"]));
            TransactionDatePicker.IsEnabled = true;
        }

        // + / - 전환 시 거래유형의 항목이 바뀜
        private void IncOrDecToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            SetTransactionTypeComboBox(IncOrDecToggleSwitch.IsOn);
        }

        // 계좌 선택 시 잔액 출력 기능
        private void AccountChooseBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int beforeBalance = accountList[AccountChooseBox.SelectedIndex].Balance;
            // string.Format("{0}", beforeBalance.ToString("#,##0")) 은 천단위로 콤마 찍어주는 코드
            BeforeBalanceTextBlock.Text = "잔액 " + string.Format("{0}", beforeBalance.ToString("#,##0")) + " 원";
        }

        // 영수증 이미지 버튼을 눌렀을 때
        private async void ReceiptImageButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            await FilePickAsync();
        }

        // 세부 내역 관련 기능
        private void AddDetailButton_Click(object sender, RoutedEventArgs e)
        {
            detailGridArray.Add(NewDetailGrid());
            UIStackPanel.Children.Add(detailGridArray.Last());
        }

        private void RemoveDetailButton_Click(object sender, RoutedEventArgs e)
        {
            if (detailGridArray.Count > 0)
            {
                detailGridArray.RemoveAt(detailGridArray.Count - 1);
                UIStackPanel.Children.RemoveAt(detailGridArray.Count - 1 + 9);
            }
        }

        private Grid NewDetailGrid()
        {
            // 세부내역 항목에 대한 Grid
            Grid detailGrid = new Grid();
            ColumnDefinition detailColumn = new ColumnDefinition();
            ColumnDefinition costColumn = new ColumnDefinition();
            ColumnDefinition emptyColumn = new ColumnDefinition();
            AutoSuggestBox detailSuggestBox = new AutoSuggestBox();
            AutoSuggestBox costTextBox = new AutoSuggestBox();
            detailColumn.Width = new GridLength(50, GridUnitType.Star);
            costColumn.Width = new GridLength(30, GridUnitType.Star);
            emptyColumn.Width = new GridLength(20, GridUnitType.Star);
            detailSuggestBox.Margin = new Thickness(10, 10, 10, 10);
            detailSuggestBox.PlaceholderText = "구체적인 항목 작성";
            detailSuggestBox.TextChanged += DetailSuggestBox_TextChanged;
            detailSuggestBox.SuggestionChosen += DetailSuggestBox_SuggestionChosen;
            detailSuggestBox.SetValue(Grid.ColumnProperty, 0);
            costTextBox.Margin = new Thickness(10, 10, 10, 10);
            costTextBox.PlaceholderText = "금액 작성";
            costTextBox.SetValue(Grid.ColumnProperty, 1);

            detailGrid.ColumnDefinitions.Add(detailColumn);
            detailGrid.ColumnDefinitions.Add(costColumn);
            detailGrid.ColumnDefinitions.Add(emptyColumn);
            detailGrid.Children.Add(detailSuggestBox);
            detailGrid.Children.Add(costTextBox);

            return detailGrid;
        }

        // 지우개 버튼을 누르면 초기화함
        private void EraseButton_Click(object sender, RoutedEventArgs e)
        {
            TodayCheckBox.IsChecked = false;
            TransactionDatePicker.IsEnabled = true;
            TransactionDatePicker.Date = Convert.ToDateTime(Convert.ToString(App.localSettings.Values["date"]));
            AccountChooseBox.SelectedIndex = Convert.ToInt32(App.localSettings.Values["accountIndex"]);
            BeforeBalanceTextBlock.Text = "계산 전 잔액";
            IncOrDecToggleSwitch.IsOn = Convert.ToBoolean(App.localSettings.Values["inOrDec"]);
            CurrencyComboBox.SelectedIndex = Convert.ToInt32(App.localSettings.Values["currencyIndex"]);
            AmountTextBox.Text = "";
            SetTransactionTypeComboBox(IncOrDecToggleSwitch.IsOn);
            BankBookSuggestBox.Text = "";
            CardBookSuggestBox.Text = "";
            file = null;
            ReceiptImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/AddImage.png", UriKind.Absolute));
            AmountTextBlock.Text = "합계";
            for (int i = detailGridArray.Count - 1; i >= 0; i--)
            {
                // 8은 세부내역들이 있는 Grid의 StackPanel 인덱스
                UIStackPanel.Children.RemoveAt(i + 8);
            }
            detailGridArray.Clear();
            detailGridArray.Add(NewDetailGrid());
            UIStackPanel.Children.Add(detailGridArray[0]);
        }

        // 저장 버튼을 누르면 검증 후 업로드함
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            int tempInt = 0;
            bool canConvert = false;

            // 테스트 업로드
            // Task<string> result = DataService.instance.UploadImageFileAsync(file, "aaa.jpg");
            // MessageBoxOpen(result.Result);
            // DataService.instance.UploadImageFileAsync(file, "aaa.jpg");

            // 검증 1. 세부 내역 항목이 하나도 없는지
            if (detailGridArray.Count == 0)
            {
                MessageBoxOpen("세부 내역에 내용을 작성해주세요.");
                return;
            }

            // 검증 2. 내용 부족이 있는지. (빈칸이 하나라도 있으면 안된다. 영수증 이미지 제외)
            if (AmountTextBox.Text == "" || BankBookSuggestBox.Text == "" || CardBookSuggestBox.Text == "")
            {
                MessageBoxOpen("내용이 부족합니다.");
                return;
            }

            for (int i = 0; i < detailGridArray.Count; i++)
            {
                if (detailGridArray[i].Children.Cast<AutoSuggestBox>().Where(j => Grid.GetColumn(j) == 0).First().Text == "" ||
                    detailGridArray[i].Children.Cast<AutoSuggestBox>().Where(j => Grid.GetColumn(j) == 1).First().Text == "")
                {
                    MessageBoxOpen("내용이 부족합니다.");
                    return;
                }
            }

            // 검증 3. 거래금액이 숫자가 맞는지
            if (!int.TryParse(AmountTextBox.Text, out tempInt))
            {
                // 거래금액이 숫자가 아니면 메시지처리하고 return함
                MessageBoxOpen("금액에 숫자만 적어주세요.");
                return;
            }

            for (int i = 0; i < detailGridArray.Count; i++)
            {
                canConvert = int.TryParse(detailGridArray[i].Children.Cast<AutoSuggestBox>().Where(j => Grid.GetColumn(j) == 1).First().Text, out tempInt);
                if (!canConvert)
                {
                    MessageBoxOpen("금액에 숫자만 적어주세요.");
                    return;
                }
            }

            // 검증 4. 세부 내역의 금액의 합계가 거래 금액과 같은지
            int costSum = 0;
            for (int i = 0; i < detailGridArray.Count; i++)
            {
                costSum += int.Parse(detailGridArray[i].Children.Cast<AutoSuggestBox>().Where(j => Grid.GetColumn(j) == 1).First().Text);
            }

            if (int.Parse(AmountTextBox.Text) != costSum)
            {
                MessageBoxOpen("세부 내역 금액의 합계가 거래 금액과 같지 않습니다.");
                return;
            }

            // 내용 검증 통과 완료. 내용을 서버로 업로드한다.

            // 업로드하기 전에 ID를 정한다. 거래날짜의 내역을 뒤져 그날 최근에 만들어진 내역을 찾아 새로운 ID를 만든다.
            
            // ex. "2018-02-11 오전 12:00:00 +09:00"
            var date = TransactionDatePicker.Date.ToString();
            var blankIndex = date.IndexOf(' ');
            var getIdString = "";
            int getIdNumber = 0;
            var result = "";
            // ex. "2018-02-11"
            date = date.Substring(0, blankIndex);

            requestDic.Clear();
            requestDic.Add((string)App.MethodElement.Element("do"), (string)App.MethodElement.Element("findRecentHistory"));
            requestDic.Add((string)App.MethodElement.Element("day"), date);
            objects = DataService.instance.GetJsonArrayFromDB(requestDic);
            
            if (objects.Count != 0)
            {
                foreach (JObject element in objects)
                {
                    getIdString = element["id"].ToString().Substring(9);
                    int.TryParse(getIdString, out getIdNumber);
                }
            }
            var year = date.Substring(0, 4);
            var month = date.Substring(5, 2);
            var day = date.Substring(8, 2);
            var newId = year + month + day + "_" + string.Format("{0:D2}", ++getIdNumber);

            // 새로운 ID를 가지고 내역을 추가한다.
            requestDic.Clear();
            requestDic.Add((string)App.MethodElement.Element("do"), (string)App.MethodElement.Element("registerHistory"));
            requestDic.Add("new_id", newId);
            requestDic.Add("date", date);
            requestDic.Add("account", accountList[AccountChooseBox.SelectedIndex].Id.ToString());
            requestDic.Add("type", transactionTypelist[TransactionTypeComboBox.SelectedIndex].Id.ToString());
            requestDic.Add("bankbook", BankBookSuggestBox.Text);
            requestDic.Add("cardbook", CardBookSuggestBox.Text);
            requestDic.Add("amount", AmountTextBox.Text);

            objects = DataService.instance.GetJsonArrayFromDB(requestDic);

            // 이미지가 있으면 저장한다.
            if (file != null)
            {
                var newFileName = newId + "번거래_명세표.jpg";
                // byte[] bytes = Encoding.ASCII.GetBytes(newFileName);
                // newFileName = Encoding.UTF8.GetString(bytes);
                DataService.instance.UploadImageFileAsync(file, newFileName);
            }

            foreach (JObject element in objects)
            {
                result = element["result"].ToString();
            }
            // MessageBoxOpen(result);

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

        // 거래 대상에 글자를 입력할때마다
        private void BankBookSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing,
            // otherwise assume the value got filled in by TextMemberPath
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                //sender.ItemsSource = dataset;
                bankBookSuggestions.Clear();
                // 텍스트가 비면 나오는 오류 방지
                if (sender.Text == "")
                {
                    sender.ItemsSource = bankBookSuggestions;
                    return;
                }

                requestDic.Clear();
                requestDic.Add((string)App.MethodElement.Element("do"), (string)App.MethodElement.Element("getBankSuggestions"));
                requestDic.Add((string)App.MethodElement.Element("keyword"), sender.Text);

                objects = DataService.instance.GetJsonArrayFromDB(requestDic);
                foreach (JObject element in objects)
                {
                    bankBookSuggestions.Add(element["result"].ToString());
                }
            }
            sender.ItemsSource = bankBookSuggestions;
        }

        // 거래 대상의 추천단어를 선택하면
        private void BankBookSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }

        // 자세한 거래 대상에 글자를 입력할때마다
        private void CardBookSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                cardBookSuggestions.Clear();
                if (sender.Text == "")
                {
                    sender.ItemsSource = cardBookSuggestions;
                    return;
                }

                requestDic.Clear();
                requestDic.Add((string)App.MethodElement.Element("do"), (string)App.MethodElement.Element("getCardSuggestions"));
                requestDic.Add((string)App.MethodElement.Element("keyword"), sender.Text);

                objects = DataService.instance.GetJsonArrayFromDB(requestDic);
                foreach (JObject element in objects)
                {
                    cardBookSuggestions.Add(element["result"].ToString());
                }
            }
            sender.ItemsSource = cardBookSuggestions;
        }

        // 자세한 거래 대상의 추천단어를 선택하면
        private void CardBookSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }

        // 세부 내역에 글자를 입력할때마다
        private void DetailSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                detailBookSuggestions.Clear();
                if (sender.Text == "")
                {
                    sender.ItemsSource = detailBookSuggestions;
                    return;
                }

                requestDic.Clear();
                requestDic.Add((string)App.MethodElement.Element("do"), (string)App.MethodElement.Element("getDetailSuggestions"));
                requestDic.Add((string)App.MethodElement.Element("keyword"), sender.Text);

                objects = DataService.instance.GetJsonArrayFromDB(requestDic);
                foreach (JObject element in objects)
                {
                    detailBookSuggestions.Add(element["result"].ToString());
                }
            }
            sender.ItemsSource = detailBookSuggestions;
        }

        // 세부 내역의 추천단어를 선택하면
        private void DetailSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }

        // 거래유형의 항목을 바꾸는 기능
        private void SetTransactionTypeComboBox(bool isOn)
        {
            var transactionTypeStringlist = new List<string>();

            transactionTypelist.Clear();
            requestDic.Clear();
            if (isOn) { requestDic.Add((string)App.MethodElement.Element("do"), (string)App.MethodElement.Element("getAddingTypes")); }
            else
            { requestDic.Add((string)App.MethodElement.Element("do"), (string)App.MethodElement.Element("getSubtractingTypes")); }

            objects = DataService.instance.GetJsonArrayFromDB(requestDic);
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

        private async void MessageBoxOpen(string showString)
        {
            var dialog = new MessageDialog(showString);
            await dialog.ShowAsync();
        }
    }
}
