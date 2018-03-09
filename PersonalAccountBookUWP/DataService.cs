using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace PersonalAccountBookUWP
{
    class DataService
    {
        public static DataService instance = new DataService();
        private HttpClient restful = new HttpClient();

        public JArray GetJsonArrayFromDB(string method)
        {
            // 요청문을 만든다.
            var request = new HttpRequestMessage(HttpMethod.Get, App.RestfulUrl + "?method=" + (string)App.MethodElement.Element(method));
            var response = new HttpResponseMessage();
            try
            {
                // 이 한줄로 DB에 요청한 다음 응답을 받는다. 실패하면 catch문으로 이동
                response = Task.Run(async () => { return await restful.SendAsync(request); }).Result;
            }
            catch (Exception e)
            {
                if (e.Source != null)
                {
                    Debug.WriteLine("source: {0}", e.Source);
                }
                throw;
            }

            // 응답을 json화를 시킨다.
            var json = response.Content.ReadAsStringAsync().Result;
            var objects = JArray.Parse(json);

            return objects;
        }
    }
}
