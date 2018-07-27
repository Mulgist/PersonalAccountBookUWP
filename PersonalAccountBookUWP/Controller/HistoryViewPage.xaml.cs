using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace PersonalAccountBookUWP
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class HistoryViewPage : Page
    {
        private HistoryListCell history = null;
        private List<DetailHistory> details = new List<DetailHistory>();
        private BitmapImage image = null;

        public HistoryViewPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            history = e.Parameter as HistoryListCell;
            ViewDidAppear();
        }

        private void ViewDidAppear()
        {
            // 받아온 HistoryListCell를 바탕으로 내용을 채운다.
            DateTextBlock.Text = history.Date;
            AccountTextBlock.Text = history.AccountName;
            TypeTextBlock.Text = history.TypeName;
            BankbookTextBlock.Text = history.Bankbook;

            // 자세한 거래 대상은 없을 시엔 해당 레이아웃이 사라진다.
            if (history.Cardbook == "")
            {
                CardbookLabel.Visibility = Visibility.Collapsed;
                CardbookTextBlock.Visibility = Visibility.Collapsed;
                CardbookLine.Visibility = Visibility.Collapsed;
            }
            else
            {
                CardbookTextBlock.Text = history.Cardbook;
            }

            AmountTextBlock.Text = history.Amount;

            // 자세한 거래 내역을 가져와 ListView에 ItemsSource로 지정한다.
            details = GetDetailHistories(history.HistoryId);
            DetailHistoryList.ItemsSource = details;


            // 비동기 처리 필요없었음..
            image = DataService.instance.DownloadImageAsync(history.HistoryId).Result;

            if (image != null)
            {
                ReceiptImage.Source = image;
            }
            else
            {
                // 이미지 없으면 레이아웃을 지운다.
                ReceiptLine.Visibility = Visibility.Collapsed;
                ReceiptLabel.Visibility = Visibility.Collapsed;
                ReceiptImageButton.Visibility = Visibility.Collapsed;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReceiptImageButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private List<DetailHistory> GetDetailHistories(string historyId)
        {
            var list = new List<DetailHistory>();

            // 요청할 때 사용하는 자료구조
            Dictionary<string, string> requestDic = new Dictionary<string, string>();

            // DB에 요청, 응답받을 때 필요한 것들
            JArray objects = null;

            requestDic.Clear();
            requestDic.Add((string)App.MethodElement.Element("do"), (string)App.MethodElement.Element("getDetailHistoryList"));
            requestDic.Add((string)App.MethodElement.Element("historyId"), historyId);
            objects = DataService.instance.GetJsonArrayFromDB(requestDic);

            foreach (JObject element in objects)
            {
                list.Add(new DetailHistory(element["id"].ToObject<int>(), element["history"].ToString(), element["name"].ToString(), element["price"].ToObject<int>()));
            }

            return list;
        }

        private async void MessageBoxOpen(string showString)
        {
            var dialog = new MessageDialog(showString);
            await dialog.ShowAsync();
        }
    }
}
