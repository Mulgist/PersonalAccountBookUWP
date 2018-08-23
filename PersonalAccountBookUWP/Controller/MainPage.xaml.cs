using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using System.Diagnostics;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PersonalAccountBookUWP.Controller;
using Windows.UI.Popups;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Core;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace PersonalAccountBookUWP
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            // 초기 작업
            // 뒤로가기 버튼 추가 (Desktop에서만)
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            // Mobile에서 Status Bar 색 변경
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();

                if (statusBar != null)
                {
                    statusBar.BackgroundOpacity = 1;
                    statusBar.BackgroundColor = Colors.White;
                    statusBar.ForegroundColor = Colors.Black;
                }
            }

            App.titleStack.Push("홈");
            MainSplitViewContent.Navigate(typeof(HomePage));
            HomeListBoxItem.IsSelected = false;

            SystemNavigationManager.GetForCurrentView().BackRequested += (s, a) =>
            {
                Debug.WriteLine("BackRequested");

                if ((MainSplitView.Content as Frame).CanGoBack)
                {
                    App.titleStack.Pop();
                    Title.Text = App.titleStack.Peek();
                    (MainSplitView.Content as Frame).GoBack();
                    a.Handled = true;
                }
                else
                {
                    if ((bool)(App.localSettings.Values["IsBackExit"]) == true)
                        CoreApplication.Exit();
                }
            };
        }

        private void HambergerButton_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void HambergerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainSplitView.IsPaneOpen = false;
            if (HomeListBoxItem.IsSelected)
            {
                App.titleStack.Push("홈");
                Title.Text = App.titleStack.Peek();
                (MainSplitView.Content as Frame).Navigate(typeof(HomePage));
                return;
            }
            if (HistoryListBoxItem.IsSelected)
            {
                App.titleStack.Push("히스토리");
                Title.Text = App.titleStack.Peek();
                (MainSplitView.Content as Frame).Navigate(typeof(HistoryListPage));
                return;
            }
            if (AddBoxItem.IsSelected)
            {
                App.titleStack.Push("추가하기");
                Title.Text = App.titleStack.Peek();
                (MainSplitView.Content as Frame).Navigate(typeof(AddPage));
                return;
            }
            /*
            if (SeminarListBoxItem.IsSelected)
            {
                App.titleStack.Push("세미나");
                Title.Text = App.titleStack.Peek();
                if (App.currentUserInfo.currentUserLevel == "미승인")
                {
                    MessageBoxOpen("권한이 없습니다.");
                    (MainSplitView.Content as Frame).Navigate(typeof(BlankPage));
                }
                else
                    (MainSplitView.Content as Frame).Navigate(typeof(DocumentsListPage), "173");
                return;
            }
            if (PlayListBoxItem.IsSelected)
            {
                App.titleStack.Push("플레이스토밍");
                Title.Text = App.titleStack.Peek();
                if (App.currentUserInfo.currentUserLevel == "미승인")
                {
                    MessageBoxOpen("권한이 없습니다.");
                    (MainSplitView.Content as Frame).Navigate(typeof(BlankPage));
                }
                else
                    (MainSplitView.Content as Frame).Navigate(typeof(DocumentsListPage), "345");
                return;
            }
            if (WorkshopListBoxItem.IsSelected)
            {
                App.titleStack.Push("워크샵");
                Title.Text = App.titleStack.Peek();
                if (App.currentUserInfo.currentUserLevel == "미승인")
                {
                    MessageBoxOpen("권한이 없습니다.");
                    (MainSplitView.Content as Frame).Navigate(typeof(BlankPage));
                }
                else
                    (MainSplitView.Content as Frame).Navigate(typeof(DocumentsListPage), "248");
                return;
            }
            if (StudyListBoxItem.IsSelected)
            {
                App.titleStack.Push("스터디");
                Title.Text = App.titleStack.Peek();
                if (App.currentUserInfo.currentUserLevel == "미승인")
                {
                    MessageBoxOpen("권한이 없습니다.");
                    (MainSplitView.Content as Frame).Navigate(typeof(BlankPage));
                }
                else
                    (MainSplitView.Content as Frame).Navigate(typeof(DocumentsListPage), "145");
                return;
            }
            if (StudentListBoxItem.IsSelected)
            {
                App.titleStack.Push("재학생 게시판");
                Title.Text = App.titleStack.Peek();
                (MainSplitView.Content as Frame).Navigate(typeof(DocumentsListPage), "158");
            }
            if (GraduateListBoxItem.IsSelected)
            {
                App.titleStack.Push("졸업생 게시판");
                Title.Text = App.titleStack.Peek();
                if (App.currentUserInfo.currentUserLevel != "관리그룹")
                {
                    MessageBoxOpen("권한이 없습니다.");
                    (MainSplitView.Content as Frame).Navigate(typeof(BlankPage));
                    // 페이지 로드 만들기
                }
            }
            */
        }

        private void MembersButton_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = false;
            /*
            App.titleStack.Push("자람 회원 목록");
            Title.Text = App.titleStack.Peek();
            (MainSplitView.Content as Frame).Navigate(typeof(MembersPage));
            */
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = false;
            /*
            App.titleStack.Push("계정");
            Title.Text = App.titleStack.Peek();
            (MainSplitView.Content as Frame).Navigate(typeof(AccountPage));
            */
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = false;
            /*
            App.titleStack.Push("설정");
            Title.Text = App.titleStack.Peek();
            (MainSplitView.Content as Frame).Navigate(typeof(SettingPage));
            */
        }
    }
}
