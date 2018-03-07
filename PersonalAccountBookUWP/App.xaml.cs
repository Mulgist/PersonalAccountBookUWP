using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PersonalAccountBookUWP
{
    sealed partial class App : Application
    {
        public static Stack<string> titleStack = new Stack<string>();
        private static string restfulUrl = "";
        public static string RestfulUrl
        {
            get => restfulUrl;
        }

        // 나중에 DB에 다방면으로 접근할 때 사용된다.
        private static XElement methodElement;
        public static XElement MethodElement {
            get => methodElement;
        }
        
        // 응용 프로그램 개체를 초기화한다.
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            // DBInfo.xml을 연다.
            string XMLFilePath = Path.Combine(Package.Current.InstalledLocation.Path, "DBInfo.xml");
            var loadedData = XDocument.Load(XMLFilePath);
            var databaseElement = loadedData.Element("Database");
            methodElement = databaseElement.Element("Method");

            restfulUrl = (string)databaseElement.Element("URL");
        }

        // 최종 사용자가 응용 프로그램을 정상적으로 시작할 때 호출된다. 다른 진입점은 특정 파일을 여는 등 응용 프로그램을 시작할 때
        // 파라미터 e는 시작 요청 및 프로세스에 대한 정보이다.
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // 창에 콘텐츠가 이미 있는 경우 앱 초기화를 반복하지 말고, 창이 활성화되어 있는지 확인하십시오.
            if (rootFrame == null)
            {
                // 탐색 컨텍스트로 사용할 프레임을 만들고 첫 페이지로 이동합니다.
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 이전에 일시 중지된 응용 프로그램에서 상태를 로드합니다.
                }

                // 현재 창에 프레임 넣기
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // 탐색 스택이 복원되지 않으면 첫 번째 페이지로 돌아가고 필요한 정보를 탐색 매개 변수로 전달하여 새 페이지를 구성합니다.
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // 현재 창이 활성 창인지 확인
                Window.Current.Activate();
            }
        }

        // 특정 페이지 탐색에 실패한 경우 호출된다. 파라이터 sender는 탐색에 실패한 프레임, 파라미터 e는 탐색 실패에 대한 정보이다.
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        // 응용 프로그램 실행이 일시 중단된 경우 호출된다.  응용 프로그램이 종료될지 또는 메모리 콘텐츠를 변경하지 않고 다시 시작할지 여부를 결정하지 않은 채 응용 프로그램 상태가 저장된다.
        // 파라이터 sender는 일시 중단 요청의 소스이고, 파라미터 e는 일시 중단 요청에 대한 세부 정보이다.
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 응용 프로그램 상태를 저장하고 백그라운드 작업을 모두 중지합니다.
            deferral.Complete();
        }
    }
}
