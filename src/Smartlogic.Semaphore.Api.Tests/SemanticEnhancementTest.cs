using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Fakes;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using KellermanSoftware.CompareNetObjects;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Smartlogic.Semaphore.Api.JSON;

namespace Smartlogic.Semaphore.Api.Tests
{
    /// <summary>
    ///     This is a test class for SemanticEnhancementTest and is intended
    ///     to contain all SemanticEnhancementTest Unit Tests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SemanticEnhancementTest
    {
        private const string TAXONOMY_INDEX = "disp_taxonomy";
        private readonly string _facet = string.Empty;
        private readonly string _filter = string.Empty;

        public TestContext TestContext { get; set; }

        [TestMethod, TestCategory("SES")]
        public void GetAtoZInformationTest()
        {
            const string query = "A";

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetAZResponse.xml";
            var expectedUri = new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.xml&SERVICE=az&AZ={query}");

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetAtoZInformation(TAXONOMY_INDEX, query);

                var expectedResponse = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
                Debug.Assert(expectedResponse != null, "expectedResponse != null");
                var expected = XDocument.Load(expectedResponse);

                var nav = actual.CreateNavigator();
                if (nav == null) throw new Exception("Unable to create navigator");
                var actualDoc = XDocument.Parse(nav.OuterXml);

                Assert.IsNotNull(actual);

                Assert.IsTrue(XNode.DeepEquals(expected, actualDoc));

                var isError = actualDoc.Descendants("Error").Any();
                Assert.IsFalse(isError, "The request was not correctly generated. An error was thrown when reading the response.");
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetAtoZInformationTest_Error()
        {
            const string query = "A";

            using (var ctx = new ShimSESConversationContext())
            {
                var target = ctx.Server;

                var actual = target.GetAtoZInformation(TAXONOMY_INDEX, query);

                var nav = actual.CreateNavigator();
                if (nav == null) throw new Exception("Unable to create navigator");
                var actualDoc = XDocument.Parse(nav.OuterXml);

                Assert.IsNotNull(actual);

                var isError = actualDoc.Descendants("Error").Any();
                Assert.IsTrue(isError, "No error message was returned");
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetConceptMapTest()
        {
            const string keywords = "Housing";
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetConceptMapResult.xml";
            var expectedUri = new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.xml&SERVICE=conceptmap&QUERY={keywords}&stop_cm_after_stage=2");
            var expectedResponse = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            Debug.Assert(expectedResponse != null, "expectedResponse != null");

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetConceptMap(TAXONOMY_INDEX, _facet, _filter, keywords, 2);

                var expected = XDocument.Load(expectedResponse);

                var nav = actual.CreateNavigator();
                if (nav == null) throw new Exception("Unable to create navigator");
                var actualDoc = XDocument.Parse(nav.OuterXml);

                Assert.IsNotNull(actual);

                Assert.IsTrue(XNode.DeepEquals(expected, actualDoc));

                var isError = actualDoc.Descendants("Error").Any();
                Assert.IsFalse(isError, "The request was not correctly generated. An error was thrown when reading the response.");
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GeTermInformationByIdsTest()
        {
            const string termIds = "OMITERMO460";

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetTermInformationResult.xml";
            var expectedUri = new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.xml&SERVICE=term&ID={termIds}");

            var expectedResponse = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            Debug.Assert(expectedResponse != null, "expectedResponse != null");

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetTermInformationByIds(TAXONOMY_INDEX, _facet, _filter, termIds);

                var expected = XDocument.Load(expectedResponse);

                var nav = actual.CreateNavigator();
                if (nav == null) throw new Exception("Unable to create navigator");
                var actualDoc = XDocument.Parse(nav.OuterXml);

                Assert.IsNotNull(actual);

                Assert.IsTrue(XNode.DeepEquals(expected, actualDoc));

                var isError = actualDoc.Descendants("Error").Any();
                Assert.IsFalse(isError,
                    "The request was not correctly generated. An error was thrown when reading the response.");
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetFacetsListTest()
        {
            var expected = new Dictionary<string, string>
            {
                {"OMITERMO692", "Business and industry"},
                {"OMITERMO726", "Economics and finance"},
                {"OMITERMO439", "Education and skills"},
                {"OMITERMO981", "Employment, jobs and careers"},
                {"OMITERMO499", "Environment"},
                {"OMITERMO760", "Government, politics and public administration"},
                {"OMITERMO557", "Health, well-being and care"},
                {"OMITERMO460", "Housing"},
                {"OMITERMO758", "Information and communication"},
                {"OMITERMO911", "International affairs and defence"},
                {"OMITERMO616", "Leisure and culture"},
                {"OMITERMO642", "Life in the community"},
                {"OMITERMO6999", "People and organisations"},
                {"OMITERMO564", "Public order, justice and rights"},
                {"OMITERMO652", "Science, technology and innovation"},
                {"OMITERMO521", "Transport and infrastructure"}
            };

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetFacetsListResult.xml";
            var expectedUri =
                new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.xml&SERVICE=facetslist");

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;
                var actual = target.GetFacetsList(TAXONOMY_INDEX);

                var compareObjects = new CompareLogic();
                var comparison = compareObjects.Compare(expected, actual);
                Assert.IsTrue(comparison.AreEqual, comparison.DifferencesString);
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetJsonAtoZInformationTest()
        {
            const string query = "A";
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetJsonAZResponse.json";
            var expectedUri = new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.json&SERVICE=az&AZ={query}");

            var expectedString = GetExpectedString(resource);
            var expected = ParseTermInfoJson(expectedString);

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetJsonAtoZInformation(TAXONOMY_INDEX, query);

                var compareObjects = new CompareLogic();
                var comparison = compareObjects.Compare(expected, actual);
                Assert.IsTrue(comparison.AreEqual, comparison.DifferencesString);
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetJsonConceptMapTest()
        {
            const string keywords = "Housing";
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetJsonConceptMapResult.json";
            var expectedUri = new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.json&SERVICE=conceptmap&QUERY={keywords}&stop_cm_after_stage=2");

            var expectedString = GetExpectedString(resource);
            var expected = TermInformationResponse.FromJsonString(expectedString);
            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetJsonConceptMap(TAXONOMY_INDEX, _facet, _filter, keywords, false);
                var compareObjects = new CompareLogic();
                var comparison = compareObjects.Compare(expected, actual);
                Assert.IsTrue(comparison.AreEqual, comparison.DifferencesString);
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetJsonModelSearchTest()
        {
            const string keywords = "housing";
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetJsonModelSearchResult.json";
            var expectedUri = new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.json&SERVICE=search&QUERY={keywords}&stop_cm_after_stage=2");

            var expectedString = GetExpectedString(resource);
            var expected = ParseSearchResponseJson(expectedString);

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetJsonModelSearchResults(TAXONOMY_INDEX, _facet, _filter, keywords, false);
                var compareObjects = new CompareLogic();
                var comparison = compareObjects.Compare(expected, actual);
                Assert.IsTrue(comparison.AreEqual, comparison.DifferencesString);
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetJsonRelatedTermsTest()
        {
            const string termId = "OMITERMO460";
            const string relationship = "NT";

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetJsonRelatedTermsResult.json";
            var expectedUri = new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.json&SERVICE=browse&ID={termId}&HIERTYPE={relationship}");

            var expectedString = GetExpectedString(resource);

            var expected = BrowseResponse.FromJsonString(expectedString);

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetJsonRelatedTerms(TAXONOMY_INDEX,
                    _facet,
                    _filter,
                    termId,
                    relationship);

                var compareObjects = new CompareLogic();
                var comparison = compareObjects.Compare(expected, actual);
                Assert.IsTrue(comparison.AreEqual, comparison.DifferencesString);
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetJsonRootTermsTest()
        {
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetJsonRootTermsResult.json";
            var expectedUri =
                new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.json&SERVICE=browse&ID=");

            var expectedString = GetExpectedString(resource);

            var expected = BrowseResponse.FromJsonString(expectedString);

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetJsonRootTerms(TAXONOMY_INDEX, _facet, _filter);
                var compareObjects = new CompareLogic();
                var comparison = compareObjects.Compare(expected, actual);
                Assert.IsTrue(comparison.AreEqual, comparison.DifferencesString);
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetJsonTermByIdTest()
        {
            const string termId = "OMITERMO460";

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetJsonTermByIdResult.json";
            var expectedUri =
                new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.json&SERVICE=browse&ID={termId}");

            var expectedString = GetExpectedString(resource);

            var expected = BrowseResponse.FromJsonString(expectedString);

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetJsonTermById(TAXONOMY_INDEX, _facet, _filter, termId);

                var compareObjects = new CompareLogic();
                var comparison = compareObjects.Compare(expected, actual);
                Assert.IsTrue(comparison.AreEqual, comparison.DifferencesString);
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetJsonTermInformationByNameTest()
        {
            const string termName = "Buildings";

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetJsonTermInformationByNameResult.json";
            var expectedUri =
                new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.json&SERVICE=term&TERM={termName}");

            var expectedString = GetExpectedString(resource);

            var expected = TermInformationResponse.FromJsonString(expectedString);

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetJsonTermInformationByName(TAXONOMY_INDEX, termName);

                var compareObjects = new CompareLogic();
                var comparison = compareObjects.Compare(expected, actual);
                Assert.IsTrue(comparison.AreEqual, comparison.DifferencesString);
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetJsonTermInformationTest()
        {
            const string termIds = "OMITERMO460";

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetJsonTermInformationResult.json";
            var expectedUri =
                new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.json&SERVICE=term&ID={termIds}");

            var expectedString = GetExpectedString(resource);

            var expected = TermInformationResponse.FromJsonString(expectedString);

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetJsonTermInformation(TAXONOMY_INDEX, _facet, _filter, termIds, "");

                var compareObjects = new CompareLogic();
                var comparison = compareObjects.Compare(expected, actual);
                Assert.IsTrue(comparison.AreEqual, comparison.DifferencesString);
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetJsonTermListByPrefixTest()
        {
            const string prefix = "fis";

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetJsonTermListByPrefixResult.json";
            var expectedUri = new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.json&SERVICE=prefix&TERM_PREFIX={prefix}&prefix_results_limit=10");

            var expectedString = GetExpectedString(resource);

            var expected = PrefixResponse.FromJsonString(expectedString);

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetJsonTermListByPrefix(TAXONOMY_INDEX, _facet, _filter, prefix, 10);

                var compareObjects = new CompareLogic();
                var comparison = compareObjects.Compare(expected, actual);
                Assert.IsTrue(comparison.AreEqual, comparison.DifferencesString);
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetModelSearchResultsTest()
        {
            const string keywords = "housing";

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetModelSearchResults.xml";
            var expectedUri = new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.xml&SERVICE=search&QUERY={keywords}&stop_cm_after_stage=2");

            var expectedString = GetExpectedString(resource);

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetModelSearchResults(TAXONOMY_INDEX, _facet, _filter, keywords, false);

                var expected = XDocument.Parse(expectedString);

                var nav = actual.CreateNavigator();
                if (nav == null) throw new Exception("Unable to create navigator");
                var actualDoc = XDocument.Parse(nav.OuterXml);

                Assert.IsNotNull(actual);

                Assert.IsTrue(XNode.DeepEquals(expected, actualDoc));

                var isError = actualDoc.Descendants("Error").Any();
                Assert.IsFalse(isError,
                    "The request was not correctly generated. An error was thrown when reading the response.");
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetRelatedTermsAsXdocTest()
        {
            const string termId = "OMITERMO460";
            const string relationship = "NT";

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetRelatedTermsResult.xml";
            var expectedUri = new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.xml&SERVICE=browse&ID={termId}&HIERTYPE={relationship}");

            var expectedString = GetExpectedString(resource);

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actualDoc = target.GetRelatedTermsAsXDoc(TAXONOMY_INDEX, _facet, _filter, termId, relationship, "");

                var expected = XDocument.Parse(expectedString);

                Assert.IsNotNull(actualDoc);

                Assert.IsTrue(XNode.DeepEquals(expected, actualDoc));

                var isError = actualDoc.Descendants("Error").Any();
                Assert.IsFalse(isError,
                    "The request was not correctly generated. An error was thrown when reading the response.");
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetRootTermsAsXdocTest()
        {
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetRootTermsResult.xml";
            var expectedUri =
                new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.xml&SERVICE=browse&ID=");

            var expectedString = GetExpectedString(resource);

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actualDoc = target.GetRootTermsAsXDoc(TAXONOMY_INDEX, _facet, _filter, "");

                var expected = XDocument.Parse(expectedString);

                Assert.IsNotNull(actualDoc);

                Assert.IsTrue(XNode.DeepEquals(expected, actualDoc));

                var isError = actualDoc.Descendants("Error").Any();
                Assert.IsFalse(isError,
                    "The request was not correctly generated. An error was thrown when reading the response.");
            }
        }


        [TestMethod, TestCategory("SES")]
        public void GetRootTermsAsXdocTest_WithHierarchy()
        {
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetRootTermsResult.xml";
            var expectedUri =
                new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.xml&SERVICE=browse&ID=&HIERTYPE=FOO");

            var expectedString = GetExpectedString(resource);

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actualDoc = target.GetRootTermsAsXDoc(TAXONOMY_INDEX, _facet, _filter, "","FOO");

                var expected = XDocument.Parse(expectedString);

                Assert.IsNotNull(actualDoc);

                Assert.IsTrue(XNode.DeepEquals(expected, actualDoc));

                var isError = actualDoc.Descendants("Error").Any();
                Assert.IsFalse(isError,
                    "The request was not correctly generated. An error was thrown when reading the response.");
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetRootTermsTest()
        {
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetRootTermsResult.xml";
            var expectedUri =
                new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX + "1"}&TEMPLATE=service.xml&SERVICE=browse&ID=");

            var expectedString = GetExpectedString(resource);

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetRootTerms(TAXONOMY_INDEX + "1", _facet, _filter, "");

                var expected = XDocument.Parse(expectedString);

                var nav = actual.CreateNavigator();
                if (nav == null) throw new Exception("Unable to create navigator");
                var actualDoc = XDocument.Parse(nav.OuterXml);

                Assert.IsNotNull(actual);
             
                Assert.IsTrue(XNode.DeepEquals(expected, actualDoc));

                var isError = actualDoc.Descendants("Error").Any();
                Assert.IsFalse(isError,
                    "The request was not correctly generated. An error was thrown when reading the response.");
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetRootTermsTest_WithHierarchy()
        {
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetRootTermsResult.xml";
            var expectedUri =
                new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX + "1"}&TEMPLATE=service.xml&SERVICE=browse&ID=&HIERTYPE=FOO");

            var expectedString = GetExpectedString(resource);

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetRootTerms(TAXONOMY_INDEX + "1", _facet, _filter, "","FOO");

                var expected = XDocument.Parse(expectedString);

                var nav = actual.CreateNavigator();
                if (nav == null) throw new Exception("Unable to create navigator");
                var actualDoc = XDocument.Parse(nav.OuterXml);

                Assert.IsNotNull(actual);

                Assert.IsTrue(XNode.DeepEquals(expected, actualDoc));

                var isError = actualDoc.Descendants("Error").Any();
                Assert.IsFalse(isError,
                    "The request was not correctly generated. An error was thrown when reading the response.");
            }
        }


        [TestMethod, TestCategory("SES")]
        public void GetStructureTest()
        {
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetStructure.xml";

            var expectedUri = new Uri($"http://myserver/{TAXONOMY_INDEX}");

            var expectedRoot = DeserializeResource<Schemas.Structure.Semaphore>(resource);
            Debug.Assert(expectedRoot != null, "structure is null");

            var expected = expectedRoot.Structure;

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;
                var actual = target.GetStructure(TAXONOMY_INDEX);

                var compareObjects = new CompareLogic();
                var comparison = compareObjects.Compare(expected, actual);
                Assert.IsTrue(comparison.AreEqual, comparison.DifferencesString);
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetTaxonomyIndexListTest()
        {
            var expected = new Collection<string>
            {
                "disp_taxonomy",
                "disp_taxonomy2",
                "disp_taxonomy3"
            };

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetTaxonomyIndexResult.xml";
            var expectedUri = new Uri("http://myserver/?TEMPLATE=service.xml&SERVICE=modelslist");

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetTaxonomyIndexList();

                CollectionAssert.AreEqual(expected, actual.ToList(), "Actual does not contain expected class");
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetTermByIdasXDocTest()
        {
            const string termId = "OMITERMO460";
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetTermByIdResult.xml";
            var expectedUri =
                new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX + "2"}&TEMPLATE=service.xml&SERVICE=browse&ID={termId}");

            var expectedString = GetExpectedString(resource);

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actualDoc = target.GetTermByIdAsXDoc(TAXONOMY_INDEX + "2", _facet, _filter, termId, "");
                var expected = XDocument.Parse(expectedString);

                Assert.IsTrue(XNode.DeepEquals(expected, actualDoc));

                var isError = actualDoc.Descendants("Error").Any();
                Assert.IsFalse(isError,
                    "The request was not correctly generated. An error was thrown when reading the response.");
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetTermByIdTest()
        {
            const string termId = "OMITERMO460";

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetTermByIdResult.xml";
            var expectedUri =
                new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.xml&SERVICE=browse&ID={termId}");

            var expectedString = GetExpectedString(resource);

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetTermById(TAXONOMY_INDEX, _facet, _filter, termId, "");

                Assert.IsNotNull(actual);

                var expected = XDocument.Parse(expectedString);

                var nav = actual.CreateNavigator();
                if (nav == null) throw new Exception("Unable to create navigator");
                var actualDoc = XDocument.Parse(nav.OuterXml);

                Assert.IsTrue(XNode.DeepEquals(expected, actualDoc));

                var isError = actualDoc.Descendants("Error").Any();
                Assert.IsFalse(isError,
                    "The request was not correctly generated. An error was thrown when reading the response.");
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetTermInformationByNameTest()
        {
            const string term = "Buildings";

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetTermInformationByNameResult.xml";
            var expectedUri =
                new Uri($"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.xml&SERVICE=term&TERM={term}");

            var expectedString = GetExpectedString(resource);

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetTermInformationByName(TAXONOMY_INDEX, term);

                Assert.IsNotNull(actual);

                var expected = XDocument.Parse(expectedString);

                var nav = actual.CreateNavigator();
                if (nav == null) throw new Exception("Unable to create navigator");
                var actualDoc = XDocument.Parse(nav.OuterXml);

                Assert.IsTrue(XNode.DeepEquals(expected, actualDoc));

                var isError = actualDoc.Descendants("Error").Any();
                Assert.IsFalse(isError,
                    "The request was not correctly generated. An error was thrown when reading the response.");
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetTermListByPrefixTest()
        {
            const string prefix = "fis";

            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetTermListByPrefixResult.xml";
            var expectedUri =
                new Uri(
                    $"http://myserver/?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.xml&SERVICE=prefix&TERM_PREFIX={prefix}&prefix_results_limit=10");

            var expectedString = GetExpectedString(resource);

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;

                var actual = target.GetTermListByPrefix(TAXONOMY_INDEX, _facet, _filter, prefix, 10);

                Assert.IsNotNull(actual);

                var expected = XDocument.Parse(expectedString);

                var nav = actual.CreateNavigator();
                if (nav == null) throw new Exception("Unable to create navigator");
                var actualDoc = XDocument.Parse(nav.OuterXml);

                Assert.IsTrue(XNode.DeepEquals(expected, actualDoc));

                var isError = actualDoc.Descendants("Error").Any();
                Assert.IsFalse(isError,
                    "The request was not correctly generated. An error was thrown when reading the response.");
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetVersion3_6Test()
        {
            var expected = new Version(3, 6, 0, 47664);
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetVersions36Result.xml";
            var expectedUri = new Uri("http://myserver/?TEMPLATE=service.xml&SERVICE=versions");

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;
                var actual = target.GetVersion();
                Assert.AreEqual(expected, actual);
            }
        }


        [TestMethod, TestCategory("SES"), ExpectedException(typeof(SemaphoreConnectionException))]
        public void GetVersion3_6Test_Error()
        {
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.EmptyXml.xml";
            var expectedUri = new Uri("http://myserver/?TEMPLATE=service.xml&SERVICE=versions");

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;
                target.GetVersion();
            }
        }

        [TestMethod, TestCategory("SES")]
        public void GetVersionPre3_6Test()
        {
            var expected = new Version(3, 4, 0, 43843);
            const string resource = "Smartlogic.Semaphore.Api.Tests.SampleFiles.GetVersionsResult.xml";
            var expectedUri = new Uri("http://myserver/?TEMPLATE=service.xml&SERVICE=versions");

            using (var ctx = new ShimSESConversationContext(resource, expectedUri))
            {
                var target = ctx.Server;
                var actual = target.GetVersion();
                Assert.AreEqual(expected, actual);
            }
        }

        public static SearchResponse ParseSearchResponseJson(string value)
        {
            //added to ensure JSON dll is output
            return JsonConvert.DeserializeObject<SearchResponse>(value);
        }

        public static TermInformationResponse ParseTermInfoJson(string value)
        {
            //added to ensure JSON dll is output
            return JsonConvert.DeserializeObject<TermInformationResponse>(value);
        }

        [TestMethod, TestCategory("SES")]
        public void SemanticEnhancementConstructorTest()
        {
            var timeout = 30;
            Uri serverUrl;
            ILogger logger = new TestLogger();
            // ReSharper disable UnusedVariable

            try
            {
#pragma warning disable 168
                var target = new SemanticEnhancement(timeout, null, null);
#pragma warning restore 168
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex,
                    typeof(ArgumentException),
                    "Constructor did not throw argument exception with a null logger");
            }

            try
            {
#pragma warning disable 168
                var target = new SemanticEnhancement(timeout, null, logger);
#pragma warning restore 168
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
                timeout = 30;
                var target = new SemanticEnhancement(timeout, serverUrl, logger);
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
                timeout = 0;
                var target = new SemanticEnhancement(timeout, serverUrl, logger);
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
                timeout = int.MaxValue/1000 + 1;
                var target = new SemanticEnhancement(timeout, serverUrl, logger);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex,
                    typeof(ArgumentException),
                    "Constructor did not throw argument exception with a maximum timeout value");
            }
            // ReSharper restore UnusedVariable
        }

        [TestMethod, TestCategory("SES")]
        public void SemanticEnhancementNoLoggingConstructorTest()
        {
            var timeout = 30;
            Uri serverUrl;
            // ReSharper disable UnusedVariable
#pragma warning disable CS0618 // Type or member is obsolete
            try
            {
#pragma warning disable 168
                var target = new SemanticEnhancement(timeout, null);
#pragma warning restore 168
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
                timeout = 30;
                var target = new SemanticEnhancement(timeout, serverUrl);
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
                timeout = 0;
                var target = new SemanticEnhancement(timeout, serverUrl);
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
                timeout = int.MaxValue/1000 + 1;
                var target = new SemanticEnhancement(timeout, serverUrl);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex,
                    typeof(ArgumentException),
                    "Constructor did not throw argument exception with a maximum timeout value");
            }
#pragma warning restore CS0618 // Type or member is obsolete
            // ReSharper restore UnusedVariable
        }

        [TestMethod, TestCategory("SES"), ExpectedException(typeof(ArgumentException))]
        public void SES_InvalidApiKeyTest()
        {
            var logger = new TestLogger();
            var ses = new SemanticEnhancement(120, new Uri("http://someserver"), logger, "non-base64apikey");
        }

        [TestMethod, TestCategory("SES")]
        public void SES_ApiKeyTest()
        {
            var url = "https://myserver9/bapi/svc/89c018e5-cbdb-48c7-b620-ee0f2c335226/";
            var apiKey = "sesrequestapikey";
            const string query = "A";

            var logger = new TestLogger();
            var tokenRequested = false;

            using (ShimsContext.Create())
            {
                var sesResponse = Assembly.GetExecutingAssembly().GetManifestResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.GetJsonAZResponse.json");
                var fakeCSResponse = new ShimHttpWebResponse
                {
                    GetResponseStream = () => sesResponse,
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

                var tokenRequest = (HttpWebRequest) WebRequest.Create("https://myserver9/token");
                var shimTokenRequest = new ShimHttpWebRequest(tokenRequest)
                {
                    GetRequestStream = () => tokenRequestStream,
                    GetResponse = () => fakeTokenResponse
                };

                var sesRequest = (HttpWebRequest) WebRequest.Create($"{url}?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.json&SERVICE=az&AZ={query}");
                var shimCSRequest = new ShimHttpWebRequest(sesRequest)
                {
                    GetRequestStream = () => csRequestStream,
                    GetResponse = () => fakeCSResponse
                };

                ShimWebRequest.CreateUri = serverUri =>
                {
                    if (serverUri.ToString() == "https://myserver9/token")
                    {
                        tokenRequested = true;
                        shimTokenRequest.RequestUriGet = () => serverUri;
                        return shimTokenRequest;
                    }
                    if (serverUri.ToString() == $"{url}?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.json&SERVICE=az&AZ={query}")
                    {
                        shimCSRequest.RequestUriGet = () => serverUri;
                        return shimCSRequest;
                    }
                    return null;
                };


                var target = new SemanticEnhancement(120, new Uri(url), logger, apiKey);

                target.GetJsonAtoZInformation(TAXONOMY_INDEX, query);

                Assert.IsTrue(tokenRequested, "Access token was not requested");

                Assert.IsNotNull(sesRequest.Headers[HttpRequestHeader.Authorization], "Authorization header was not set");
                Assert.AreEqual(sesRequest.Headers[HttpRequestHeader.Authorization],
                    "bearer someverylongbase64encodedaccesstoken",
                    "authorization header was set to an unexpected value");
            }
        }

        [TestMethod, TestCategory("SES"),ExpectedException(typeof(SemaphoreConnectionException))]
        public void SES_UnableToGetTokenTest()
        {
            var url = "https://myserver9/bapi/svc/89c018e5-cbdb-48c7-b620-ee0f2c335226/";
            var apiKey = "somedodgyaccesstoken";
            const string query = "A";

            var logger = new TestLogger();
            var tokenRequested = false;

            using (ShimsContext.Create())
            {
                var sesResponse = Assembly.GetExecutingAssembly().GetManifestResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.GetJsonAZResponse.json");
                var fakeCSResponse = new ShimHttpWebResponse
                {
                    GetResponseStream = () => sesResponse,
                    StatusCodeGet = () => HttpStatusCode.OK,
                    Close = () => { }
                };

                var tokenResponse = Assembly.GetExecutingAssembly().GetManifestResourceStream("Smartlogic.Semaphore.Api.Tests.SampleFiles.TokenErrorResponse.json");
                var fakeTokenResponse = new ShimHttpWebResponse
                {
                    GetResponseStream = () => tokenResponse,
                    StatusCodeGet = () => HttpStatusCode.BadRequest,
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
                    GetResponse = () => { throw new WebException("foo", null, WebExceptionStatus.UnknownError, fakeTokenResponse); }
                };

                var sesRequest = (HttpWebRequest)WebRequest.Create($"{url}?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.json&SERVICE=az&AZ={query}");
                var shimCSRequest = new ShimHttpWebRequest(sesRequest)
                {
                    GetRequestStream = () => csRequestStream,
                    GetResponse = () => fakeCSResponse
                };

                ShimWebRequest.CreateUri = serverUri =>
                {
                    if (serverUri.ToString() == "https://myserver9/token")
                    {
                        tokenRequested = true;
                        shimTokenRequest.RequestUriGet = () => serverUri;
                        return shimTokenRequest;
                    }
                    if (serverUri.ToString() == $"{url}?TBDB={TAXONOMY_INDEX}&TEMPLATE=service.json&SERVICE=az&AZ={query}")
                    {
                        shimCSRequest.RequestUriGet = () => serverUri;
                        return shimCSRequest;
                    }
                    return null;
                };


                var target = new SemanticEnhancement(120, new Uri(url), logger, apiKey)
                {
                    ThrowExceptions = true
                };

                target.GetJsonAtoZInformation(TAXONOMY_INDEX, query);

                Assert.IsTrue(tokenRequested, "Access token was not requested");
            }
        }
        
        private static T DeserializeResource<T>(string resourceName) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (s != null)
                {
                    var result = serializer.Deserialize(s) as T;
                    return result;
                }
            }

            return null;
        }

        private static string GetExpectedString(string resource)
        {
            var expectedResponse = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            Debug.Assert(expectedResponse != null, "expectedResponse != null");
            var expectedString = new StreamReader(expectedResponse).ReadToEnd();
            return expectedString;
        }
    }
}