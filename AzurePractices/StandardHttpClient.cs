using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AzurePractices
{

    public class StandardHttpClient : IHttpClient
    {
        /// <summary>
        /// The client
        /// </summary>
        private readonly HttpClient _client;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<StandardHttpClient> _logger;
        /// <summary>
        /// The HTTP context accessor
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardHttpClient" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public StandardHttpClient(ILogger<StandardHttpClient> logger, IHttpContextAccessor httpContextAccessor)
        {
            _client = new HttpClient();
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the string asynchronous.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <param name="authorizationMethod">The authorization method.</param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer", string headerString = null, IDictionary<string, string> requestHeaders = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }
            else if (headerString != null)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", headerString);
            }
            else
            {
                SetAuthorizationHeader(requestMessage);
            }

            if (requestHeaders != null)
            {
                foreach (KeyValuePair<string, string> pair in requestHeaders)
                {
                    requestMessage.Headers.Add(pair.Key, pair.Value);
                }
            }

            var response = await _client.SendAsync(requestMessage);
            //_client.Dispose();
            ////The below statement is suppressing the actual issue
            //response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            throw new HttpRequestException(response.Content?.ReadAsStringAsync().Result);
        }

        /// <summary>
        /// Does the post put asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="item">The item.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="authorizationMethod">The authorization method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Value must be either post or put. - method</exception>
        /// <exception cref="HttpRequestException"></exception>
        private async Task<HttpResponseMessage> DoPostPutAsync<T>(HttpMethod method, string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer", IDictionary<string, string> headers = null, JsonSerializerSettings jsonSettings = null)
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
            {
                throw new ArgumentException("Value must be either post or put.", nameof(method));
            }

            // a new StringContent must be created for each retry
            // as it is disposed after each call
            var requestMessage = new HttpRequestMessage(method, uri);

            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }
            else
            {
                SetAuthorizationHeader(requestMessage);
            }

            headers = headers ?? new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in headers)
            {
                requestMessage.Headers.Add(pair.Key, pair.Value);
            }

            requestMessage.Content = jsonSettings != null ? new StringContent(JsonConvert.SerializeObject(item, jsonSettings), System.Text.Encoding.UTF8, "application/json") : new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");


            if (requestId != null)
            {
                requestMessage.Headers.Add("x-requestid", requestId);
            }

            var response = await _client.SendAsync(requestMessage);
            //_client.Dispose();
            // raise exception if HttpResponseCode 500
            // needed for circuit breaker to track fails

            //if (response.StatusCode == HttpStatusCode.InternalServerError)
            //{
            //    throw new HttpRequestException();
            //}
            //The below statement is suppressing the actual issue
            //response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            throw new HttpRequestException(response.Content?.ReadAsStringAsync().Result);
        }


        /// <summary>
        /// Posts the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri">The URI.</param>
        /// <param name="item">The item.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="authorizationMethod">The authorization method.</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer", IDictionary<string, string> headers = null, JsonSerializerSettings jsonSettings = null)
        {
            return await DoPostPutAsync(HttpMethod.Post, uri, item, authorizationToken, requestId, authorizationMethod, headers, jsonSettings);
        }

        /// <summary>
        /// Puts the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri">The URI.</param>
        /// <param name="item">The item.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="authorizationMethod">The authorization method.</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            return await DoPostPutAsync(HttpMethod.Put, uri, item, authorizationToken, requestId, authorizationMethod);
        }
        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="authorizationMethod">The authorization method.</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }
            else
            {
                SetAuthorizationHeader(requestMessage);
            }

            if (requestId != null)
            {
                requestMessage.Headers.Add("x-requestid", requestId);
            }

            return await _client.SendAsync(requestMessage);
        }

        /// <summary>
        /// Sets the authorization header.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        private void SetAuthorizationHeader(HttpRequestMessage requestMessage)
        {
            var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                requestMessage.Headers.Add("Authorization", new List<string> { authorizationHeader });
            }
        }
        /// <summary>
        /// Posts the asynchronous.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="RequestData">The request data.</param>
        /// <param name="serviceTimeOut">The service time out.</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<HttpResponseMessage> PostAsync(string uri, MultipartFormDataContent RequestData, int serviceTimeOut, string headerString = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            if (headerString != null)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", headerString);
            }
            else
            {
                SetAuthorizationHeader(requestMessage);
            }

            //_client.Timeout = TimeSpan.FromMinutes(serviceTimeOut); Need to work on this
            HttpResponseMessage response = await _client.PostAsync(uri, RequestData);
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }
            return response;
        }
        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="headerString">The header string.</param>
        /// <returns></returns>
        /// TODO: Need to refactor GetStringAsync Method to return HttpResponseMessage
        public async Task<HttpResponseMessage> GetStreamAsync(string uri, string headerString = null)
        {
            if (headerString != null)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", headerString);
            }

            HttpResponseMessage response = _client.GetAsync(uri).Result;
            //It is not returning the correct value
            ////response.EnsureSuccessStatusCode();
            //return  await Task.FromResult(response);
            if (response.IsSuccessStatusCode)
            {
                return await Task.FromResult(response);
            }

            throw new HttpRequestException(response.Content?.ReadAsStringAsync().Result);
        }
    }
}


