using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using Smartlogic.Semaphore.Api.JSON;

namespace Smartlogic.Semaphore.Api
{
    internal sealed class AuthenticatedRequestBuilder
    {
        private static volatile AuthenticatedRequestBuilder _instance;
        private static readonly object syncRoot = new object();
        private readonly string _correlationIdHeaderName = "X-Correlation_Id";

        private readonly Dictionary<string, TokenResponse> _keyCache = new Dictionary<string, TokenResponse>();

        private AuthenticatedRequestBuilder()
        {
        }

        public static AuthenticatedRequestBuilder Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new AuthenticatedRequestBuilder();
                    }
                }

                return _instance;
            }
        }

        public HttpWebRequest Build(Uri serverUrl, string apiKey, ILogger logger, Guid correlationId)
        {
            var request = (HttpWebRequest) WebRequest.Create(serverUrl);

            if (!string.IsNullOrEmpty(apiKey))
            {
                AddAuthenticationHeaders(request, apiKey, logger);
            }
            else
            {
                logger.WriteLow("No authentication header required");
            }

            if (!(correlationId == default))
            {
                request.Headers.Add(_correlationIdHeaderName, correlationId.ToString());
            }

            return request;
        }

        public HttpClient GetAuthorizedClient(Uri serverUrl, string apiKey, ILogger logger, Guid correlationId)
        {
            var client = new HttpClient();
            client.BaseAddress = serverUrl;
            if (!string.IsNullOrEmpty(apiKey))
            {
                AddAuthenticationHeaders(client, apiKey, logger);
            }
            else
            {
                logger.WriteLow("No authentication header required");
            }
            if (!(correlationId == default))
            {
                client.DefaultRequestHeaders.Add(_correlationIdHeaderName, correlationId.ToString());
            }
            return client;
        }

        private void GetTokenFromApiKey(string baseUrl, string apiKey, ILogger logger)
        {
            if (!_keyCache.Keys.Contains(apiKey) || _keyCache[apiKey].Expires <= DateTime.UtcNow)
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri($"{baseUrl}/token"));
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                string postData = $"grant_type=apikey&key={HttpUtility.UrlEncode(apiKey)}";
                var byteArray = Encoding.UTF8.GetBytes(postData);
                httpWebRequest.ContentLength = byteArray.Length;
                var dataStream = httpWebRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                logger.WriteLow("Requesting access token");

                HttpWebResponse httpWebResponse;

                try
                {
                    httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                }
                catch (WebException ex)
                {
                    logger.WriteLow("Access token request returned {0}", ex.Message);
                    httpWebResponse = (HttpWebResponse)ex.Response;
                    if (httpWebResponse.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var errorResponse = DeserializeJsonResponse<TokenErrorResponse>(httpWebResponse);
                        throw new SemaphoreConnectionException($"{errorResponse.Error}: {errorResponse.Description}", ex);
                    }
                    throw new SemaphoreConnectionException($"{ex.Status}: {ex.Message}", ex);
                }

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    var tokenResponse = DeserializeJsonResponse<TokenResponse>(httpWebResponse);
                    if (_keyCache.ContainsKey(apiKey))
                    {
                        logger.WriteLow("Updating access token cache");
                        _keyCache[apiKey] = tokenResponse;
                    }
                    else
                    {
                        logger.WriteLow("Adding entry to access token cache");
                        _keyCache.Add(apiKey, tokenResponse);
                    }
                }
            }
            else
            {
                logger.WriteLow("Using cached access token");
            }
        }

        private void AddAuthenticationHeaders(HttpWebRequest request, string apiKey, ILogger logger)
        {
            logger.WriteLow("Adding authentication header");
            GetTokenFromApiKey(request.RequestUri.GetLeftPart(UriPartial.Authority), apiKey, logger);
            logger.WriteLow("Adding 'Authorization' header with access token");
            var bearer = $"bearer {_keyCache[apiKey].AccessToken}";
            request.Headers.Add(HttpRequestHeader.Authorization, bearer);
        }

        private void AddAuthenticationHeaders(HttpClient client, string apiKey, ILogger logger)
        {
            logger.WriteLow("Adding authentication header");
            GetTokenFromApiKey(client.BaseAddress.GetLeftPart(UriPartial.Authority), apiKey, logger);
            logger.WriteLow("Adding 'Authorization' header with access token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _keyCache[apiKey].AccessToken);
        }

        private T DeserializeJsonResponse<T>(HttpWebResponse httpWebResponse)
        {
            var sResponse = string.Empty;
            using (var s = httpWebResponse.GetResponseStream())
            {
                if (s != null)
                    using (var oReader = new StreamReader(s, Encoding.GetEncoding("utf-8")))
                    {
                        sResponse = oReader.ReadToEnd();
                    }
                return JsonConvert.DeserializeObject<T>(sResponse);
            }
        }
    }
}