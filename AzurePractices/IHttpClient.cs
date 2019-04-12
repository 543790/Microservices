using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzurePractices
{
    public interface IHttpClient
    {
        /// <summary>
        /// Gets the string asynchronous.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <param name="authorizationMethod">The authorization method.</param>
        /// <returns></returns>
        Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer", string headerString = null, IDictionary<string, string> headers = null);

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
        Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer", IDictionary<string, string> headers = null, JsonSerializerSettings jsonSettings = null);

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="authorizationMethod">The authorization method.</param>
        /// <returns></returns>
        Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");

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
        Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");
        /// <summary>
        /// Posts the asynchronous.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="RequestData">The request data.</param>
        /// <param name="serviceTimeOut">The service time out.</param>
        /// <returns></returns>
        Task<HttpResponseMessage> PostAsync(string uri, MultipartFormDataContent RequestData, int serviceTimeOut, string headerString = null);
        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="headerString">The header string.</param>
        /// <returns></returns>
        Task<HttpResponseMessage> GetStreamAsync(string uri, string headerString = null);

    }
}

