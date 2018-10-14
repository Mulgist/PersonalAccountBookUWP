using Newtonsoft.Json.Linq;
using PersonalAccountBookUWP.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            historyCells = getHistories(false, null);
        }

        // 기간 열기 및 닫기 버튼 클릭 시
        private void PeriodButton_Click(object sender, RoutedEventArgs e)
        {
            if (PeriodButtonsGrid.Visibility == Visibility.Collapsed)
            {
                PeriodButtonsGrid.Visibility = Visibility.Visible;
                PeriodButton.Content = "기간 닫기";
            }
            else
            {
                PeriodButtonsGrid.Visibility = Visibility.Collapsed;
                PeriodButton.Content = "기간 열기";
            }
        }

        // 1일, 1주 등 기간 버튼들 클릭 시
        private void PeriodButtons_Click(object sender, RoutedEventArgs e)
        {
            EndDatePicker.Date = DateTime.Today;
            if ((Button)sender == OneDayButton)
            {
                StartDatePicker.Date = DateTime.Today.AddDays(-1);
            }
            else if ((Button)sender == OneWeekButton)
            {
                StartDatePicker.Date = DateTime.Today.AddDays(-7);
            }
            else if ((Button)sender == OneMonthButton)
            {
                StartDatePicker.Date = DateTime.Today.AddMonths(-1);
            }
            else if ((Button)sender == OneYearButton)
            {
                StartDatePicker.Date = DateTime.Today.AddYears(-1);
            }
        }

        // 검색 버튼 클릭 시
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // 요청할 때 사용하는 자료
            Dictionary<string, string> requestDic = new Dictionary<string, string>();

            var searchWord = SearchTextBox.Text;
            var startDayString = StartDatePicker.Date.ToString();
            var endDayString = EndDatePicker.Date.ToString();

            requestDic.Clear();
            requestDic.Add((string)App.MethodElement.Element("do"), (string)App.MethodElement.Element("getSearchHistoryList"));
            
            if (searchWord != "")
            {
                requestDic.Add((string)App.MethodElement.Element("searchWord"), searchWord);
            }
            if (startDayString != "")
            {
                var blankIndex = startDayString.IndexOf(' ');
                startDayString = startDayString.Substring(0, blankIndex); // ex. "2018-02-11"
                requestDic.Add((string)App.MethodElement.Element("startDay"), startDayString);
            }
            if (endDayString != "")
            {
                var blankIndex = endDayString.IndexOf(' ');
                endDayString = endDayString.Substring(0, blankIndex); // ex. "2018-02-11"
                requestDic.Add((string)App.MethodElement.Element("endDay"), endDayString);
            }

            historyCells = getHistories(true, requestDic);

            // 검색 결과를 화면에 적용한다.
            HistoryList.ItemsSource = historyCells;
        }

        // 리스트 아이템 클릭 시
        private void HistoryList_ItemClick(object sender, ItemClickEventArgs e)
        {
            // 일반 모드일 때만(선택 모드 아닐 때) 페이지 이동
            if (HistoryList.SelectionMode == ListViewSelectionMode.None)
            {
                // 부모 페이지에 접근한다.
                var parent = Utility.instance.FindParent<MainPage>(this);
                App.titleStack.Push("히스토리 자세히 보기");
                parent.SetTitle(App.titleStack.Peek());

                Frame.Navigate(typeof(HistoryViewPage), e.ClickedItem as HistoryListCell);
            }
        }

        // 하단 버튼 클릭 시
        private void CommandButton_Click(object sender, RoutedEventArgs e)
        {
            if ((AppBarButton)sender == AddButton)
            {
                // 부모 페이지에 접근한다.
                var parent = Utility.instance.FindParent<MainPage>(this);
                App.titleStack.Push("추가하기");
                parent.SetTitle(App.titleStack.Peek());
                parent.SetSelectedMenu(2);

                Frame.Navigate(typeof(AddPage));
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
                List<HistoryListCell> removeList = new List<HistoryListCell>();

                foreach (HistoryListCell cell in HistoryList.SelectedItems)
                {
                    removeList.Add(cell);
                }

                Utility.instance.MessageBoxOpen(removeList.First().HistoryId);
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

        private List<HistoryListCell> getHistories(bool option, Dictionary<string, string> perRequestDic)
        {
            var list = new List<HistoryListCell>();
            var accountString = "";
            var bookString = "";
            var amountString = "";

            // DB에 요청, 응답받을 때 필요한 것들
            JArray objects = null;

            // 검색해서 조회할 때
            if (option)
            {
                objects = DataService.instance.GetJsonArrayFromDB(perRequestDic);
            }
            // 검색아닌 조회할 때 (초반 오픈시)
            else
            {
                // 요청할 때 사용하는 자료구조
                Dictionary<string, string> requestDic = new Dictionary<string, string>();

                // History List 받기
                requestDic.Clear();
                requestDic.Add((string)App.MethodElement.Element("do"), (string)App.MethodElement.Element("getRecentHistoryList"));
                objects = DataService.instance.GetJsonArrayFromDB(requestDic);
            }
            
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
