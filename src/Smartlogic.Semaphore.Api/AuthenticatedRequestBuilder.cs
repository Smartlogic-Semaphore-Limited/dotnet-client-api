using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

        private void AddAuthenticationHeaders(HttpWebRequest request, string apiKey, ILogger logger)
        {
            logger.WriteLow("Adding authentication header");

            if (!_keyCache.Keys.Contains(apiKey) || _keyCache[apiKey].Expires <= DateTime.UtcNow)
            {
                var baseUrl = request.RequestUri.GetLeftPart(UriPartial.Authority);
                var httpWebRequest = (HttpWebRequest) WebRequest.Create(new Uri($"{baseUrl}/token"));
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
                    httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                }
                catch (WebException ex)
                {
                    logger.WriteLow("Access token request returned {0}", ex.Message);
                    httpWebResponse = (HttpWebResponse) ex.Response;
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

            logger.WriteLow("Adding 'Authorization' header with access token");
            var bearer = $"bearer {_keyCache[apiKey].AccessToken}";
            request.Headers.Add(HttpRequestHeader.Authorization, bearer);
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