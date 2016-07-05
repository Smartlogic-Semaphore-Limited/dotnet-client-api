using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Fakes;
using System.Reflection;
using Microsoft.QualityTools.Testing.Fakes;

namespace Smartlogic.Semaphore.Api.Tests
{
    [ExcludeFromCodeCoverage]
    internal class ShimSESConversationContext : IDisposable
    {
        private const int WEB_SERVICE_TIMEOUT = 30;
        private static readonly ILogger logger = new TestLogger();
        private readonly Uri _serverUrl = new Uri("http://myserver2");
        private readonly IDisposable _wrappedContext;

        public ShimSESConversationContext(string responseResourceName = "", Uri expectedUri = null, string serverUrl = "")
        {
            _wrappedContext = ShimsContext.Create();
            var url = _serverUrl;

            if (!string.IsNullOrEmpty(serverUrl))
            {
                url = new Uri(serverUrl);
            }

            Server = new SemanticEnhancement(WEB_SERVICE_TIMEOUT, url, logger);
            
            var fakeResponse = new ShimHttpWebResponse();
            if (string.IsNullOrWhiteSpace(responseResourceName))
            {
                ShimHttpWebRequest.AllInstances.GetResponse = foo => { throw new WebException("Some test web exception"); };
            }
            else
            {
                var testResponse = Assembly.GetExecutingAssembly().GetManifestResourceStream(responseResourceName);
                ShimHttpWebRequest.AllInstances.GetResponse = foo => fakeResponse;
                fakeResponse.GetResponseStream = () => testResponse;
            }
            fakeResponse.StatusCodeGet = () => HttpStatusCode.OK;
            fakeResponse.Close = () => { };

            RequestStream = new MemoryStream();
            RequestStream.Seek(0, SeekOrigin.Begin);
            ShimHttpWebRequest.AllInstances.GetRequestStream = foo => RequestStream;
            ShimHttpWebRequest.AllInstances.RequestUriGet = foo => expectedUri;


        }

        public SemanticEnhancement Server { get; }
        public MemoryStream RequestStream { get; }

        #region IDisposable Members

        public void Dispose()
        {
            _wrappedContext.Dispose();
        }

        #endregion
    }
}