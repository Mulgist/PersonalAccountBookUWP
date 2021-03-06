﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace PersonalAccountBookUWP
{
    class DataService
    {
        public static DataService instance = new DataService();
        private HttpClient restful = new HttpClient();

        public JArray GetJsonArrayFromDB(Dictionary<string, string> requestDic)
        {
            var requestString = "";
            // 요청문을 만든다.
            foreach (KeyValuePair<string, string> element in requestDic)
            {
                requestString += element.Key + "=" + element.Value + "&";
            }

            var uriString = App.RestfulUrl + "?" + requestString;
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(uriString));
            var response = new HttpResponseMessage();
            try
            {
                // 이 한줄로 DB에 요청한 다음 응답을 받는다. 실패하면 catch문으로 이동
                response = Task.Run(async () => { return await restful.SendRequestAsync(request); }).Result;
            }
            catch (Exception e)
            {
                if (e.Source != null)
                {
                    Debug.WriteLine("source: {0}", e.Source);
                }
                throw;
            }

            // 응답을 json화 시킨다.
            var json = response.Content.ReadAsStringAsync().GetResults();
            var objects = JArray.Parse(json);

            return objects;
        }

        public async void UploadImageFileAsync(StorageFile image, string filename)
        {
            // StorageFile을 HttpBufferContent로 바꾸는 과정이다. (HttpBufferContent는 IHttpContent와 호환됨)
            byte[] fileBytes = null;
            
            using (IRandomAccessStreamWithContentType stream = await image.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (DataReader reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }

            IHttpContent content = new HttpBufferContent(fileBytes.AsBuffer());

            // 요청을 만들고 보낸다.
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(App.UploadUrl));
            var response = new HttpResponseMessage();
            var multipart = new HttpMultipartFormDataContent
            {
                { content, "uploadfile", filename }
            };
            request.Content = multipart;
            response = await restful.SendRequestAsync(request);

            // 결과 콘솔 출력
            // Debug.WriteLine(response.Content.ReadAsStringAsync().GetResults());
        }

        public IBuffer DownloadImageBuffer(string filename)
        {
            // .jpg로 1차 시도
            var uri = new Uri(App.DownloadUrl + filename + ".jpg");
            restful = new HttpClient();
            IBuffer buffer = null;
            try
            {
                // 파일이 없으면 Exception 발생
                buffer = Task.Run(async () => { return await restful.GetBufferAsync(uri); }).Result;
            }
            catch (Exception)
            {
                // .PNG로 2차 시도
                uri = new Uri(App.DownloadUrl + filename + ".PNG");
                restful = new HttpClient();

                try
                {
                    buffer = Task.Run(async () => { return await restful.GetBufferAsync(uri); }).Result;
                }
                catch (Exception)
                {
                    // .png로 3차 시도
                    uri = new Uri(App.DownloadUrl + filename + ".png");
                    restful = new HttpClient();

                    try
                    {
                        buffer = Task.Run(async () => { return await restful.GetBufferAsync(uri); }).Result;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }

            return buffer;
        }
    }
}
