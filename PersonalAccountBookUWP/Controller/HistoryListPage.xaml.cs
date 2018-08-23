using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace PersonalAccountBookUWP
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class HistoryListPage : Page
    {
        // historycell 리스트
        private List<HistoryListCell> historyCells = new List<HistoryListCell>();
        
        public HistoryListPage()
        {
            InitializeComponent();
            ViewDidAppear();
            HistoryList.ItemsSource = historyCells;
        }

        private void ViewDidAppear()
        {
            historyCells = getHistories();
        }

        // 하단 버튼 클릭시
        private void CommandButton_Click(object sender, RoutedEventArgs e)
        {
            if ((AppBarButton)sender == AddButton)
            {
                
            }
            else if ((AppBarButton)sender == SelectModeButton)
            {
                AddButton.Visibility = Visibility.Collapsed;
                SelectModeButton.Visibility = Visibility.Collapsed;
                RemoveButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Visible;
                HistoryList.SelectionMode = ListViewSelectionMode.Multiple;
            }
            else if ((AppBarButton)sender == RemoveButton)
            {

            }
            else if ((AppBarButton)sender == CancelButton)
            {
                AddButton.Visibility = Visibility.Visible;
                SelectModeButton.Visibility = Visibility.Visible;
                RemoveButton.Visibility = Visibility.Collapsed;
                CancelButton.Visibility = Visibility.Collapsed;
                HistoryList.SelectionMode = ListViewSelectionMode.None;
            }
        }

        private void SelectModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryList.SelectionMode == ListViewSelectionMode.None)
            {
                HistoryList.SelectionMode = ListViewSelectionMode.Multiple;
            }
            else
            {
                HistoryList.SelectionMode = ListViewSelectionMode.None;
            }
        }

        private void HistoryList_ItemClick(object sender, ItemClickEventArgs e)
        {
            // titleStack에 Push해도 상위 Page인 MainPage는 바뀌지 않는다.
            App.titleStack.Push("히스토리 자세히 보기");
            this.Frame.Navigate(typeof(HistoryViewPage), e.ClickedItem as HistoryListCell);
        }


        private List<HistoryListCell> getHistories()
        {
            var list = new List<HistoryListCell>();
            var accountString = "";
            var bookString = "";
            var amountString = "";

            // 요청할 때 사용하는 자료구조
            Dictionary<string, string> requestDic = new Dictionary<string, string>();

            // DB에 요청, 응답받을 때 필요한 것들
            JArray objects = null;
            
            // History List 받기
            requestDic.Clear();
            requestDic.Add((string)App.MethodElement.Element("do"), (string)App.MethodElement.Element("getHistoryList"));
            objects = DataService.instance.GetJsonArrayFromDB(requestDic);

            foreach (JObject element in objects)
            {
                accountString = element["bank"].ToString() + " " + element["accountname"].ToString() + " " + element["number"].ToString();
                // 현금같은 경우는 은행이 없어 첫 문자가 띄어쓰기이다. 이것을 제거해준다.
                if (accountString.IndexOf(' ') == 0)
                {
                    accountString = accountString.Substring(1);
                }

                if (element["cardbook"].ToString() == "")
                {
                    bookString = element["bankbook"].ToString();
                }
                else
                {
                    bookString = element["cardbook"].ToString();
                }

                if (Convert.ToInt32(element["amount"]) >= 0)
                {
                    amountString = "\\ " + Convert.ToInt32(element["amount"]).ToString("#,##0");
                }
                else
                {
                    amountString = "- \\ " + (Convert.ToInt32(element["amount"]) * -1).ToString("#,##0");
                } 
                list.Add(new HistoryListCell(element["id"].ToString(), accountString, element["typename"].ToString(), bookString, element["bankbook"].ToString(), element["cardbook"].ToString(), "1,000", amountString, element["transactiondate"].ToString()));
            }

            return list;
        }
    }
}
