using Smartlogic.Semaphore.Api;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Fakes;
using System.Reflection;
using System.Text;
using KellermanSoftware.CompareNetObjects;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Smartlogic.Semaphore.Api.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ClassificationServerTest
    {

        [TestMethod, TestCategory("CS"), ExpectedException(typeof(ArgumentException))]
        public void CS_InvalidApiKeyTest()
        {
            var logger = new TestLogger();
            var ses = new ClassificationServer(120, new Uri("http://someserver"), logger, "non-base64apikey");
        }

        [TestMethod, TestCategory("CS")]
        public void ClassificationServerConstructorTest()
        {
            var webServiceTimeout = 30;
            ILogger logger = new TestLogger();
            Uri serverUrl;
            // ReSharper disable UnusedVariable
            try
            {
                var target = new ClassificationServer(webServiceTimeout, null, logger);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex,
                    typeof(ArgumentException),
                    "Constructor did not throw argument exception with a null url");
            }

            try
            {
                serverUrl = new Uri("test://nonsense");
                webServiceTimeout = 30;
                var target = new ClassificationServer(webServiceTimeout, serverUrl, logger);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex,
                    typeof(ArgumentException),
                    "Constructor did not throw argument exception with an invalid url");
            }

            try
            {
                serverUrl = new Uri("http://myurl");
                webServiceTimeout = 0;
                var target = new ClassificationServer(webServiceTimeout, serverUrl, logger);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex,
                    typeof(ArgumentException),
                    "Constructor did not throw argument exception with a zero timeout value");
            }

            try
            {
                serverUrl = new Uri("http://myurl");
                webServiceTimeout = int.MaxValue / 1000 + 1;
                var target = new ClassificationServer(webServiceTimeout, serverUrl, logger);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex,
                    typeof(ArgumentException),
                    "Constructor did not throw argument exception with a maximum timeout value");
            }
            // ReSharper restore UnusedVariable
        }

        [TestMethod, TestCategory("CS")]
        public void ClassificationServerNoLoggingConstructorTest()
        {
            var webServiceTimeout = 30;
            Uri serverUrl;
            // ReSharper disable UnusedVariable
#pragma warning disable 618 //ignore obsolete attribute
            try
            {
                var target = new ClassificationServer(webServiceTimeout, null);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex,
                    typeof(ArgumentException),
                    "Constructor did not throw argument exception with a null url");
            }

            try
            {
                serverUrl = new Uri("test://nonsense");
                webServiceTimeout = 30;
                var target = new ClassificationServer(webServiceTimeout, serverUrl);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex,
                    typeof(ArgumentException),
                    "Constructor did not throw argument exception with an invalid url");
            }

            try
            {
                serverUrl = new Uri("http://myurl");
                webServiceTimeout = 0;
                var target = new ClassificationServer(webServiceTimeout, serverUrl);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex,
                    typeof(ArgumentException),
                    "Constructor did not throw argument exception with a zero timeout value");
            }

            try
            {
                serverUrl = new Uri("http://myurl");
                webServiceTimeout = int.MaxValue / 1000 + 1;
                var target = new ClassificationServer(webServiceTimeout, serverUrl);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex,
                    typeof(ArgumentException),
                    "Constructor did not throw argument exception with a maximum timeout value");
            }
#pragma warning restore 618
            // ReSharper restore UnusedVariable
        }


        [TestMethod, TestCategory("CS")]
        public void ClassifyTest_Error()
        {
            var title = string.Empty;
            var fileName = string.Empty;
            Dictionary<string, string> metaValues = null;
            var altBody = string.Empty;

            using (var ctx = new ShimCSConversationContext())
            {
                var target = ctx.Server;
                try
                {
                    target.Classify(title, null, fileName, null, altBody, null);
                }
                catch (Exception ex)
                {
                    Assert.IsInstanceOfType(ex,
                        typeof(ArgumentException),
                        "Classify did not throw argument exception with a null metaValues");
                }

                try
                {
                    metaValues = new Dictionary<string, string>
                    {
                        {"FOO", "bar"}
                    };
                    var document = new byte[500];
                    target.Classify(title, document, fileName, metaValues, altBody, null);
                }
                catch (Exception ex)
                {
                    Assert.IsInstanceOfType(ex,
                        typeof(ArgumentException),
                        "Classify did not throw argument exception with a document and a null filename");
                }

                try
                {
                    metaValues = new Dictionary<string, string>
                    {
                        {"FOO", "bar"}
                    };
                    fileName = "mydoc.txt";
                    target.Classify(title, null, fileName, metaValues, altBody, null);
                }
                catch (Exception ex)
                {
                    Assert.IsInstanceOfType(ex,
                        typeof(ArgumentException),
                        "Classify did not throw argument exception with a null document and a filename");
                }


                try
                {
                    var document = new byte[500];
                    target.Classify(title, document, fileName, metaValues, altBody, null);
                }
                catch (Exception ex)
                {
                    Assert.IsInstanceOfType(ex, typeof(WebException), "Classify did not re-throw web exception");
                }
            }
        }

        [TestMethod, TestCategory("CS")]
        public void TextMineTest_Error()
        {
            var title = string.Empty;
            var fileName = string.Empty;
            Dictionary<string, string> metaValues = null;
            var altBody = string.Empty;

            using (var ctx = new ShimCSConversationContext())
            {
                var target = ctx.Server;
                try
                {
                    target.TextMine(title, null, fileName, null, altBody, null);
                }
                catch (Exception ex)
                {
                    Assert.IsInstanceOfType(ex,
                        typeof(ArgumentException),
                        "TextMine did not throw argument exception with a null metaValues");
                }

                try
                {
                    metaValues = new Dictionary<string, string>
                    {
                        {"FOO", "bar"}
                    };
                    var document = new byte[500];
                    target.TextMine(title, document, fileName, metaValues, altBody, null);
                }
                catch (Exception ex)
                {
                    Assert.IsInstanceOfType(ex,
                        typeof(ArgumentException),
                        "TextMine did not throw argument exception with a document and a null filename");
                }

                try
                {
                    metaValues = new Dictionary<string, string>
                    {
                        {"FOO", "bar"}
                    };
                    fileName = "mydoc.txt";
                    target.TextMine(title, null, fileName, metaValues, altBody, null);
                }
                catch (Exception ex)
                {
                    Assert.IsInstanceOfType(ex,
                        typeof(ArgumentException),
                        "TextMine did not throw argument exception with a null document and a filename");
                }


                try
                {
                    var document = new byte[500];
                    target.TextMine(title, document, fileName, metaValues, altBody, null);
                }
                catch (Exception ex)
                {
                    Assert.IsInstanceOfType(ex, typeof(WebException), "TextMine did not re-throw web exception");
                }
            }
        }

        [TestMethod, TestCategory("CS")]
        public void ClassifyTest_Happy()
        {
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.CSTestResponse.xml";

            using (var ctx = new ShimCSConversationContext(resource))
            {
                var testDoc = GetResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.TestDocument.txt");

                const string title = "Cambridge CC- schools list.txt";
                const string fileName = "Cambridge CC - school list.txt";
                var altBody = string.Empty;
                var document = ReadFully(testDoc);
                var metaValues = new Dictionary<string, string>
                {
                    {"FOO", "bar"}
                };

                var actual = ctx.Server.Classify(title, document, fileName, metaValues, altBody, null);

                var expectedStream = GetResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.HttpPost_CSTestRequest.txt");
                var request = ReadFully(expectedStream);

                CollectionAssert.AreEqual(request,
                    ctx.RequestStream.GetBuffer(),
                    "Generated Request does not match expected request");

                Assert.AreEqual(1,
                    actual.GetClassifications("Generic_ID").Count(),
                    "Generated result does not contain expected elements");
            }
        }

        [TestMethod, TestCategory("CS")]
        public void ClassifyTestOptions_Happy()
        {
            var options = new ClassificationOptions
            {
                ArticleType = ArticleType.MultiArticle,
                Threshold = 50,
                ClusteringThreshold = 43,
                ClusteringType = "FOO",
                Type = "BAR"
            };

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.CSTestResponse.xml";

            using (var ctx = new ShimCSConversationContext(resource))
            {
                var testDoc = GetResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.TestDocument.txt");

                const string title = "Cambridge CC- schools list.txt";
                const string fileName = "Cambridge CC - school list.txt";
                var altBody = string.Empty;
                var document = ReadFully(testDoc);
                var metaValues = new Dictionary<string, string>
                {
                    {"FOO", "bar"}
                };

                var actual = ctx.Server.Classify(title, document, fileName, metaValues, altBody, options);

                var expectedStream = GetResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.HttpPost_CSTestRequest_Options.txt");
                var request = ReadFully(expectedStream);

                CollectionAssert.AreEqual(request,
                    ctx.RequestStream.GetBuffer(),
                    "Generated Request does not match expected request");

                Assert.AreEqual(1,
                    actual.GetClassifications("Generic_ID").Count(),
                    "Generated result does not contain expected elements");
            }
        }

        [TestMethod, TestCategory("CS")]
        public void ClassifyTestMetaNode_Happy()
        {
            var options = new ClassificationOptions
            {
                ArticleType = ArticleType.MultiArticle,
                Threshold = 50,
                ClusteringThreshold = 43,
                ClusteringType = "FOO",
                Type = "BAR"
            };

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.CSTestResponseNesting.xml";

            using (var ctx = new ShimCSConversationContext(resource))
            {
                var testDoc = GetResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.TestDocument.txt");

                const string title = "Cambridge CC- schools list.txt";
                const string fileName = "Cambridge CC - school list.txt";
                var altBody = string.Empty;
                var document = ReadFully(testDoc);
                var metaValues = new Dictionary<string, string>
                {
                    {"FOO", "bar"}
                };

                var actual = ctx.Server.Classify(title, document, fileName, metaValues, altBody, options);

                var expectedStream = GetResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.HttpPost_CSTestRequest_Options.txt");
                var request = ReadFully(expectedStream);

                CollectionAssert.AreEqual(request,
                    ctx.RequestStream.GetBuffer(),
                    "Generated Request does not match expected request");

                // Note, because at the top level all the fact extraction META's at the same, only one will actually be returned.
                Assert.AreEqual(1,
                    actual.GetClassifications("FactExtraction-Example").Count(),
                    "Generated result does not contain expected elements");

                // When asking for all classifications, there is no merging of identical metas
                Assert.AreEqual(22,
                    actual.GetClassifications().Count(),
                    "Generated result does not contain expected elements");

                Assert.AreEqual(22,
                    actual.GetMetaNodes().Count(),
                    "Generated result does not contain expected elements");

                MetaNode thirdItem = actual.GetMetaNodes().ToArray<MetaNode>()[2];
                Assert.AreEqual("FactExtraction-Example", thirdItem.ClassName, "thirdItem ClassName");
                Assert.AreEqual("", thirdItem.Value, "thirdItem Value");
                Assert.AreEqual("", thirdItem.Id, "thirdItem Id");
                Assert.AreEqual(1.0f, thirdItem.Score, "thirdItem Score");
                Assert.AreEqual(1, thirdItem.Children.Count(), "thirdItem Children.Count");

                MetaNode child1 = thirdItem.Children.First();
                Assert.AreEqual("Doc 1", child1.ClassName, "child1 ClassName");
                Assert.AreEqual("", child1.Value, "child1 Value");
                Assert.AreEqual("", child1.Id, "child1 Id");
                Assert.AreEqual(1.0f, child1.Score, "child1 Score");
                Assert.AreEqual(1, child1.Children.Count(), "child1 Children.Count");

                MetaNode child2 = child1.Children.First();
                Assert.AreEqual("DF1", child2.ClassName, "child2 ClassName");
                Assert.AreEqual("", child2.Value, "child2 Value");
                Assert.AreEqual("", child2.Id, "child2 Id");
                Assert.AreEqual(1.0f, child2.Score, "child2 Score");
                Assert.AreEqual(3, child2.Children.Count(), "child2 Children.Count");

                MetaNode child3 = child2.Children.ToArray()[2];
                Assert.AreEqual("group ID", child3.ClassName, "child3 ClassName");
                Assert.AreEqual("a4ee6a2f-5b50-4fa5-a238-3ad827667448::::DF1", child3.Value, "child3 Value");
                Assert.AreEqual("a4ee6a2f-5b50-4fa5-a238-3ad827667448::::DF1", child3.Id, "child3 Id");
                Assert.AreEqual(1.0f, child3.Score, "child3 Score");
                Assert.AreEqual(0, child3.Children.Count(), "child3 Children.Count");
            }
        }


        [TestMethod, TestCategory("CS")]
        public void TextMineTestOptions_Happy()
        {
            var options = new ClassificationOptions
            {
                ArticleType = ArticleType.MultiArticle,
                Threshold = 50,
                ClusteringThreshold = 43,
                ClusteringType = "FOO",
                Type = "BAR"
            };

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.CSTestResponse.xml";

            using (var ctx = new ShimCSConversationContext(resource))
            {
                var testDoc = GetResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.TestDocument.txt");

                const string title = "Cambridge CC- schools list.txt";
                const string fileName = "Cambridge CC - school list.txt";
                var altBody = string.Empty;
                var document = ReadFully(testDoc);
                var metaValues = new Dictionary<string, string>
                {
                    {"FOO", "bar"}
                };

                var actual = ctx.Server.TextMine(title, document, fileName, metaValues, altBody, options);

                var expectedStream = GetResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.HttpPost_TMTestRequest_Options.txt");
                var request = ReadFully(expectedStream);

                Console.Write(Encoding.UTF8.GetString(ctx.RequestStream.GetBuffer()));

                CollectionAssert.AreEqual(request,
                    ctx.RequestStream.GetBuffer(),
                    "Generated Request does not match expected request");

                Assert.AreEqual(1,
                    actual.GetClassifications("Generic_ID").Count(),
                    "Generated result does not contain expected elements");
            }
        }


        [TestMethod, TestCategory("CS")]
        public void GetClassificationClassesTest()
        {
            var expected = new Collection<string>
            {
                "Generic_ID",
                "Generic",
                "Generic_RAW"
            };

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetClassificationClassesResponse.xml";
            var expectedUri = new Uri("http://myserver3/?operation=LISTRULENETCLASSES");

            using (var ctx = new ShimCSConversationContext(resource, expectedUri, "http://myserver3"))
            {
                var target = ctx.Server;
                var actual = target.GetClassificationClasses();
                CollectionAssert.AreEqual(expected, actual, "Actual does not contain expected class");
            }
        }

        [TestMethod, TestCategory("CS")]
        public void GetLanguages_Happy()
        {
            var expected = new Collection<ClassificationLanguage>
            {
                new ClassificationLanguage
                {
                    DisplayName = "English (English)",
                    HasRulesDefined = true,
                    Id = "en",
                    IsDefault = true,
                    Name = "English",
                    Type = "LinguistX"
                }
            };
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetLanguagesResponse1.xml";
            var expectedUri = new Uri("http://myserver2/?operation=listlanguages");

            using (var ctx = new ShimCSConversationContext(resource, expectedUri, "http://myserver2"))
            {
                var target = ctx.Server;

                var actual = target.GetLanguages();

                var compareObjects = new CompareLogic();
                var comparison = compareObjects.Compare(expected, actual);
                Assert.IsTrue(comparison.AreEqual, comparison.DifferencesString);
            }
        }

        [TestMethod, TestCategory("CS")]
        public void GetLanguages2_Happy()
        {
            var expected = new Collection<ClassificationLanguage>
            {
                new ClassificationLanguage
                {
                    DisplayName = "English (English)",
                    HasRulesDefined = false,
                    Id = "en",
                    IsDefault = true,
                    Name = "English",
                    Type = "LinguistX"
                }
            };

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetLanguagesResponse2.xml";
            var expectedUri = new Uri("http://myserver/?operation=listlanguages");

            using (var ctx = new ShimCSConversationContext(resource, expectedUri, "http://myserver"))
            {
                var target = ctx.Server;

                var actual = target.GetLanguages();

                var compareObjects = new CompareLogic();
                var comparison = compareObjects.Compare(expected, actual);
                Assert.IsTrue(comparison.AreEqual, comparison.DifferencesString);
            }
        }

        [TestMethod, TestCategory("CS")]
        public void GetTextMiningClassesTest()
        {
            var expected = new Collection<string>
            {
                "Generic_ID",
                "Generic",
                "Generic_RAW",
                "TEXTMINE_Test",
                "TEXTMINE_Test2"
            };

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetTMClassesResponse.xml";
            var expectedUri = new Uri("http://myserver8/?operation=LISTRULENETCLASSES");


            using (var ctx = new ShimCSConversationContext(resource, expectedUri, "http://myserver8"))
            {
                var target = ctx.Server;

                var actual = target.GetTextMiningClasses();

                CollectionAssert.AreEqual(expected, actual, "Actual does not contain expected class");
            }
        }

        [TestMethod, TestCategory("CS")]
        public void GetVersion3_6Test()
        {
            var expected = new Version(3, 6, 0, 47553);
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetCSVersions36Result.xml";
            var expectedUri = new Uri("http://myserver2/?operation=version");

            using (var ctx = new ShimCSConversationContext(resource, expectedUri, "http://myserver2"))
            {
                var target = ctx.Server;
                var actual = target.GetVersion();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod, TestCategory("CS")]
        public void GetVersionPre3_6Test()
        {
            var expected = new Version(7, 14, 0, 43753);
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetCSVersionsResult.xml";
            var expectedUri = new Uri("http://myserver4/?operation=version");


            using (var ctx = new ShimCSConversationContext(resource, expectedUri, "http://myserver4"))
            {
                var target = ctx.Server;
                var actual = target.GetVersion();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod, TestCategory("CS")]
        public void TextMineTest_Happy()
        {
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.TMTestResponse.xml";
            var expectedUri = new Uri("http://myserver7");


            using (var ctx = new ShimCSConversationContext(resource, expectedUri, "http://myserver7"))
            {
                var target = ctx.Server;

                var testDoc = GetResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.TestDocument.txt");

                const string title = "Cambridge CC- schools list.txt";
                const string fileName = "Cambridge CC - school list.txt";
                var altBody = string.Empty;
                var document = ReadFully(testDoc);
                var metaValues = new Dictionary<string, string>
                {
                    {"FOO", "bar"}
                };

                var actual = target.TextMine(title, document, fileName, metaValues, altBody, null);

                var expectedStream = GetResourceStream(
                    "Smartlogic.Semaphore.Api.Tests.SampleFiles.HttpPost_TMTestRequest.txt");
                var request = ReadFully(expectedStream);

                CollectionAssert.AreEqual(request,
                    ctx.RequestStream.GetBuffer(),
                    "Generated Request does not match expected request");

                Assert.AreEqual(1,
                    actual.GetClassifications("Generic_ID").Count(),
                    "Generated result does not contain expected elements");
            }
        }

        private static void CopyTo(Stream source,
            Stream destination,
            int bufferSize)
        {
            int num;
            var buffer = new byte[bufferSize];
            while ((num = source.Read(buffer, 0, buffer.Length)) != 0)
            {
                destination.Write(buffer, 0, num);
            }
        }

        private static Stream GetResourceStream(string filename)
        {
            var expectedStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filename);
            return expectedStream;
        }


        private static byte[] ReadFully(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                CopyTo(input, ms, 4096);
                return ms.ToArray();
            }
        }

        [TestMethod(), TestCategory("CS")]
        public void ClassificationServer_ApiKeyTest()
        {
            var url = "https://myserver9/bapi/svc/89c018e5-cbdb-48c7-b620-ee0f2c335226/";
            var apiKey = "9F9SwG+M6IzmwM/nmVoQdA==";

            var logger = new TestLogger();
            bool tokenRequested = false;

            using (ShimsContext.Create())
            {
                var csResponse = Assembly.GetExecutingAssembly().GetManifestResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.GetClassificationClassesResponse.xml");
                var fakeCSResponse = new ShimHttpWebResponse
                {
                    GetResponseStream = () => csResponse,
                    StatusCodeGet = () => HttpStatusCode.OK,
                    Close = () => { }
                };

                var tokenResponse = Assembly.GetExecutingAssembly().GetManifestResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.TokenResponse.json");
                var fakeTokenResponse = new ShimHttpWebResponse
                {
                    GetResponseStream = () => tokenResponse,
                    StatusCodeGet = () => HttpStatusCode.OK,
                    Close = () => { }
                };


                var tokenRequestStream = new MemoryStream();
                tokenRequestStream.Seek(0, SeekOrigin.Begin);
                var csRequestStream = new MemoryStream();
                csRequestStream.Seek(0, SeekOrigin.Begin);

                var tokenRequest = (HttpWebRequest)WebRequest.Create("https://myserver9/token");
                var shimTokenRequest = new ShimHttpWebRequest(tokenRequest)
                {
                    GetRequestStream = () => tokenRequestStream,
                    GetResponse = () => fakeTokenResponse
                };

                var csRequest = (HttpWebRequest)WebRequest.Create($"{url}?operation=LISTRULENETCLASSES");
                var shimCSRequest = new ShimHttpWebRequest(csRequest)
                {
                    GetRequestStream = () => csRequestStream,
                    GetResponse = () => fakeCSResponse
                };

                ShimWebRequest.CreateUri = (serverUri) =>
                {
                    if (serverUri.ToString() == "https://myserver9/token")
                    {
                        tokenRequested = true;
                        shimTokenRequest.RequestUriGet = () => serverUri;
                        return shimTokenRequest;
                    }
                    if (serverUri.ToString() == $"{url}?operation=LISTRULENETCLASSES")
                    {
                        shimCSRequest.RequestUriGet = () => serverUri;
                        return shimCSRequest;
                    }
                    return null;
                };


                var cs = new ClassificationServer(120, new Uri(url), logger, apiKey);

                var expected = new Collection<string> { "Generic_ID", "Generic", "Generic_RAW" };

                var actual = cs.GetClassificationClasses();

                Assert.IsTrue(tokenRequested, "Access token was not requested");
                
                var expectedTokenRequest = Encoding.UTF8.GetString(ReadFully(Assembly.GetExecutingAssembly().GetManifestResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.TokenRequest.txt")));

                var actualTokenRequest = Encoding.UTF8.GetString(tokenRequestStream.GetBuffer());

                Assert.IsNotNull(csRequest.Headers[HttpRequestHeader.Authorization],"Authorization header was not set"); 
                Assert.AreEqual(csRequest.Headers[HttpRequestHeader.Authorization],"bearer someverylongbase64encodedaccesstoken", "authorization header was set to an unexpected value");

                Assert.AreEqual(expectedTokenRequest, actualTokenRequest, "Generated token request does not match expected request");
                CollectionAssert.AreEqual(expected, actual, "Actual does not contain expected class");
            }
        }

        [TestMethod(), TestCategory("CS")]
        public void ClassificationServer_ApiKey_CacheTest()
        {
            var url = "https://myserver10/bapi/svc/89c018e5-cbdb-48c7-b620-ee0f2c335226/";
            var apiKey = "cachetestkey";

            var logger = new TestLogger();
            int tokenRequestCount = 0;

            using (ShimsContext.Create())
            {
                var fakeCSResponse = new ShimHttpWebResponse
                {
                    GetResponseStream = () => Assembly.GetExecutingAssembly().GetManifestResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.GetClassificationClassesResponse.xml"),
                    StatusCodeGet = () => HttpStatusCode.OK,
                    Close = () => { }
                };
                
                var fakeTokenResponse = new ShimHttpWebResponse
                {
                    GetResponseStream = () => Assembly.GetExecutingAssembly().GetManifestResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.TokenResponse.json"),
                    StatusCodeGet = () => HttpStatusCode.OK,
                    Close = () => { }
                };


                var tokenRequestStream = new MemoryStream();
                tokenRequestStream.Seek(0, SeekOrigin.Begin);
                var csRequestStream = new MemoryStream();
                csRequestStream.Seek(0, SeekOrigin.Begin);

                var tokenRequest = (HttpWebRequest)WebRequest.Create("https://myserver10/token");
                var shimTokenRequest = new ShimHttpWebRequest(tokenRequest)
                {
                    GetRequestStream = () => tokenRequestStream,
                    GetResponse = () => fakeTokenResponse
                };

                var csRequest = (HttpWebRequest)WebRequest.Create($"{url}?operation=LISTRULENETCLASSES");
                var shimCSRequest = new ShimHttpWebRequest(csRequest)
                {
                    GetRequestStream = () => csRequestStream,
                    GetResponse = () => fakeCSResponse
                };

                ShimWebRequest.CreateUri = (serverUri) =>
                {
                    if (serverUri.ToString() == "https://myserver10/token")
                    {
                        tokenRequestCount++;
                        shimTokenRequest.RequestUriGet = () => serverUri;
                        return shimTokenRequest;
                    }
                    if (serverUri.ToString() == $"{url}?operation=LISTRULENETCLASSES")
                    {
                        shimCSRequest.RequestUriGet = () => serverUri;
                        return shimCSRequest;
                    }
                    return null;
                };


                var cs = new ClassificationServer(120, new Uri(url), logger, apiKey);

                var expected = new Collection<string> { "Generic_ID", "Generic", "Generic_RAW" };

                var actual = cs.GetClassificationClasses();
                var actual2 = cs.GetClassificationClasses();

                Assert.AreEqual(1, tokenRequestCount, "Access token was not cached");
            }
        }

        [TestMethod(), TestCategory("CS")]
        public void ClassificationServer_ApiKey_CacheExpiryTest()
        {
            var url = "https://myserver11/bapi/svc/89c018e5-cbdb-48c7-b620-ee0f2c335226/";
            var apiKey = "c29tZWFwaWtleQ==";

            var logger = new TestLogger();
            int tokenRequestCount = 0;

            using (ShimsContext.Create())
            {
                var fakeCSResponse = new ShimHttpWebResponse
                {
                    GetResponseStream = () => Assembly.GetExecutingAssembly().GetManifestResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.GetClassificationClassesResponse.xml"),
                    StatusCodeGet = () => HttpStatusCode.OK,
                    Close = () => { }
                };

                var fakeTokenResponse = new ShimHttpWebResponse
                {
                    GetResponseStream = () => Assembly.GetExecutingAssembly().GetManifestResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.TokenResponseExpired.json"),
                    StatusCodeGet = () => HttpStatusCode.OK,
                    Close = () => { }
                };


                MemoryStream tokenRequestStream;
                var csRequestStream = new MemoryStream();
                csRequestStream.Seek(0, SeekOrigin.Begin);

                var tokenRequest = (HttpWebRequest)WebRequest.Create("https://myserver11/token");
                var shimTokenRequest = new ShimHttpWebRequest(tokenRequest)
                {
                    GetRequestStream = () =>
                    {
                        tokenRequestStream = new MemoryStream();
                        tokenRequestStream.Seek(0, SeekOrigin.Begin);
                        return tokenRequestStream;
                    },
                    GetResponse = () => fakeTokenResponse
                };

                var csRequest = (HttpWebRequest)WebRequest.Create($"{url}?operation=LISTRULENETCLASSES");
                var shimCSRequest = new ShimHttpWebRequest(csRequest)
                {
                    GetRequestStream = () => csRequestStream,
                    GetResponse = () => fakeCSResponse
                };

                ShimWebRequest.CreateUri = (serverUri) =>
                {
                    if (serverUri.ToString() == "https://myserver11/token")
                    {
                        tokenRequestCount++;
                        shimTokenRequest.RequestUriGet = () => serverUri;
                        return shimTokenRequest;
                    }
                    if (serverUri.ToString() == $"{url}?operation=LISTRULENETCLASSES")
                    {
                        shimCSRequest.RequestUriGet = () => serverUri;
                        return shimCSRequest;
                    }
                    return null;
                };


                var cs = new ClassificationServer(120, new Uri(url), logger, apiKey);

                var expected = new Collection<string> { "Generic_ID", "Generic", "Generic_RAW" };

                var actual = cs.GetClassificationClasses();
                var actual2 = cs.GetClassificationClasses();

                Assert.AreEqual(2, tokenRequestCount, "Access token cache was not refreshed");
            }
        }
    }
}