using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel.Security;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;
using AmenityFinder.exceptions;
using Polly;

namespace AmenityFinder.utils
{
    public class HttpUtils
    {
        private ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
        private CancellationTokenSource _cts;

        public enum HttpMethod
        {
            Post,
            Get,
            Delete,
            Put,
            Patch,
        }

        public void PrepareRequest(out HttpStringContent reqMsg, string postData)
        {
            reqMsg = new HttpStringContent(postData);
            reqMsg.Headers.ContentType = new Windows.Web.Http.Headers.HttpMediaTypeHeaderValue("content-type")
            {
                MediaType = "application/json",
                CharSet = UnicodeEncoding.Utf8.ToString()
            };
        }

        private async Task<HttpResponseMessage> MakeRequestHelper(HttpClient httpClient, Uri uri, HttpStringContent reqMsg, HttpMethod httpMethod)
        {
            HttpResponseMessage response = null;
            _cts = new CancellationTokenSource(Constants.Timeout);
            switch (httpMethod)
            {
                case HttpMethod.Get:
                    response = await httpClient.GetAsync(uri).AsTask(_cts.Token);
                    break;
                case HttpMethod.Post:
                    response = await httpClient.PostAsync(uri, reqMsg).AsTask(_cts.Token);
                    break;
                case HttpMethod.Delete:
                    response = await httpClient.DeleteAsync(uri).AsTask(_cts.Token);
                    break;
                case HttpMethod.Put:
                    response = await httpClient.PutAsync(uri, reqMsg).AsTask(_cts.Token);
                    break;
                case HttpMethod.Patch:
                    throw new NotImplementedException("Patch is not implemented");
                default:
                    throw new NotImplementedException("Unexpected stuff happend");
            }

            return response;
        }

        public async Task<HttpResponseMessage> MakeRequest(string url, HttpStringContent reqMsg, HttpMethod httpMethod)
        {
            var uri = new Uri(url);
            var httpClient = new HttpClient();
            AddAuthToken(ref httpClient);

            HttpResponseMessage response = null;

            var policy = Policy
                .Handle<COMException>()
                .WaitAndRetryAsync(5, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            try
            {
                response = await policy.ExecuteAsync(() => MakeRequestHelper(httpClient, uri, reqMsg, httpMethod));
            }
            catch (COMException)
            {
                throw new HttpRequestFailedException("Unable to make internet call");
            }


            return response;

        }

        public void ProcessResponse(HttpResponseMessage response)
        {
            
        }

        private void AddAuthToken(ref HttpClient httpClient)
        {
            var userToken = _localSettings.Values[Constants.UserTokenName] as string;
            if (userToken != null)
            {
                httpClient.DefaultRequestHeaders.Add("auth-token", userToken);
            }
        }
    }
}