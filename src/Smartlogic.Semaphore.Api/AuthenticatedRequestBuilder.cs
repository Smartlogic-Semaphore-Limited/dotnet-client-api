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
    static class AuthenticatedRequestBuilder
    {
        private static readonly Dictionary<string, TokenResponse> keyCache = new Dictionary<string, TokenResponse>();

        public static HttpWebRequest Build(Uri serverUrl, string apiKey, ILogger logger)
        {
            var request = (HttpWebRequest)WebRequest.Create(serverUrl);

            if (!string.IsNullOrEmpty(apiKey))
            {
                AddAuthenticationHeaders(request, apiKey, logger);
            }
            else
            {
                logger.WriteLow("No authentication header required");
            }

            return request;
        }

        private static void AddAuthenticationHeaders(HttpWebRequest request, string apiKey, ILogger logger)
        {
            logger.WriteLow("Adding authentication header");

            if (!keyCache.Keys.Contains(apiKey) || keyCache[apiKey].Expires <= DateTime.UtcNow)
            {
                var baseUrl = request.RequestUri.GetLeftPart(UriPartial.Authority);
                var oRequest = (HttpWebRequest) WebRequest.Create(new Uri($"{baseUrl}/token"));
                oRequest.Method = "POST";
                oRequest.ContentType = "application/x-www-form-urlencoded";
                string postData = $"grant_type=apikey&key={HttpUtility.UrlEncode(apiKey)}";
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                oRequest.ContentLength = byteArray.Length;
                Stream dataStream = oRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                logger.WriteLow("Requesting access token");

                var oResponse = (HttpWebResponse) oRequest.GetResponse();

                var status = (int) oResponse.StatusCode;
                if (status >= 400)
                {
                    throw new SemaphoreConnectionException($"{status}: {oResponse.StatusDescription}");
                }

                if (oResponse.StatusCode == HttpStatusCode.OK)
                {
                    var sResponse = string.Empty;
                    var s = oResponse.GetResponseStream();
                    if (s != null)
                        using (var oReader = new StreamReader(s, Encoding.GetEncoding("utf-8")))
                        {
                            sResponse = oReader.ReadToEnd();
                        }
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(sResponse);
                    if (keyCache.ContainsKey(apiKey))
                    {
                        logger.WriteLow("Updating access token cache");
                        keyCache[apiKey] = tokenResponse;
                    }
                    else
                    {
                        logger.WriteLow("Adding entry to access token cache");
                        keyCache.Add(apiKey, tokenResponse);
                    }
                }
            }
            else
            {
                logger.WriteLow("Using cached access token");
            }

            logger.WriteLow("Adding 'Authorization' header with access token");
            var bearer = $"bearer {keyCache[apiKey].AccessToken}";
            request.Headers.Add(HttpRequestHeader.Authorization, bearer);
        }
    }
}
