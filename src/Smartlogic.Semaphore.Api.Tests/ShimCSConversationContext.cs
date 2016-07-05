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
    internal class ShimCSConversationContext : IDisposable
    {
        private const int WEB_SERVICE_TIMEOUT = 30;
        private static readonly ILogger logger = new TestLogger();
        private readonly Uri _serverUrl = new Uri("http://myserver2");
        private readonly IDisposable _wrappedContext;

        public ShimCSConversationContext(string responseResourceName = "",
            Uri expectedUri = null,
            string serverUrl = "")
        {
            _wrappedContext = ShimsContext.Create();
            var url = _serverUrl;

            if (!string.IsNullOrEmpty(serverUrl))
            {
                url = new Uri(serverUrl);
            }

            Server = new ClassificationServer(WEB_SERVICE_TIMEOUT, url, logger)
            {
                BoundaryString = "5d4f77f9e9494794bd5d23c6b729ceb0"
            };

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

        public ClassificationServer Server { get; }
        public MemoryStream RequestStream { get; }

        #region IDisposable Members

        public void Dispose()
        {
            _wrappedContext.Dispose();
        }

        #endregion
    }
}