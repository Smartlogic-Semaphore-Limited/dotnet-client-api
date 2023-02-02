using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using Newtonsoft.Json;
using Smartlogic.Semaphore.Api.JSON;
using Smartlogic.Semaphore.Api.Schemas.Structure;

namespace Smartlogic.Semaphore.Api
{
    /// <summary>
    ///     Proxy class used for communicating with Semantic Enhancement Server
    /// </summary>
    public class SemanticEnhancement : LoggingProxyBase
    {
        private const int WEBSERVICE_TIMEOUT_DEFAULT = 120;
        private readonly string _apiKey;
        private readonly Uri _serverUrl;
        private readonly int _timeout;
        private readonly Guid _correlationId;

        /// <summary>
        ///     Constructs a SearchEnhancement class for use with communicating with a particular instance of Search Enhancement
        ///     Server
        /// </summary>
        /// <param name="serverUrl">
        ///     An absolute <see cref="Uri" /> indicating the Search Enhancement Server endpoint
        /// </param>
        /// <exception cref="System.ArgumentException">Missing server Url;serverUrl</exception>
        /// <exception cref="ArgumentException"></exception>
        public SemanticEnhancement(Uri serverUrl) : this(WEBSERVICE_TIMEOUT_DEFAULT, serverUrl, new DefaultTraceLogger())
        {
        }

        /// <summary>
        ///     Constructs a SearchEnhancement class for use with communicating with a particular instance of Search Enhancement
        ///     Server
        /// </summary>
        /// <param name="webServiceTimeout">An integer representing the number of seconds to wait before timing out</param>
        /// <param name="serverUrl">
        ///     An absolute <see cref="Uri" /> indicating the Search Enhancement Server endpoint
        /// </param>
        /// <exception cref="System.ArgumentException">Missing server Url;serverUrl</exception>
        /// <exception cref="ArgumentException"></exception>
        public SemanticEnhancement(int webServiceTimeout, Uri serverUrl) : this(webServiceTimeout, serverUrl, new DefaultTraceLogger())
        {
        }

        /// <summary>
        ///     Constructs a SearchEnhancement class for use with communicating with a particular instance of Search Enhancement
        ///     Server
        /// </summary>
        /// <param name="webServiceTimeout">An integer representing the number of seconds to wait before timing out</param>
        /// <param name="serverUrl">
        ///     An absolute <see cref="Uri" /> indicating the Search Enhancement Server endpoint
        /// </param>
        /// <param name="logger"></param>
        /// <exception cref="System.ArgumentException">Missing server Url;serverUrl</exception>
        /// <exception cref="ArgumentException"></exception>
        public SemanticEnhancement(int webServiceTimeout, Uri serverUrl, ILogger logger)
            : base(logger)
        {
            if (serverUrl == null) throw new ArgumentException("Missing server Url", nameof(serverUrl));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (webServiceTimeout <= 0)
                throw new ArgumentException("Web service timeout must be greater than 0",
                    nameof(webServiceTimeout));
            if (webServiceTimeout > int.MaxValue/1000)
                throw new ArgumentException(
                    "Web service timeout must be less than " + short.MaxValue/1000,
                    nameof(webServiceTimeout));
            if (!serverUrl.IsAbsoluteUri)
                throw new ArgumentException("Server Url must be an absolute Uri", nameof(serverUrl));

            _serverUrl = serverUrl;
            _timeout = webServiceTimeout;
        }

        /// <summary>
        ///     Constructs a SearchEnhancement class for use with communicating with a particular instance of Search Enhancement
        ///     Server
        /// </summary>
        /// <param name="webServiceTimeout">An integer representing the number of seconds to wait before timing out</param>
        /// <param name="serverUrl">
        ///     An absolute <see cref="Uri" /> indicating the Search Enhancement Server endpoint
        /// </param>
        /// <param name="logger"></param>
        /// <param name="apiKey">An optional apikey string used for connecting to OAuth 2.0 secured services</param>
        /// <param name="correlationId">An optional correlationId guid passed to SES for logging purposes to make it easier to trace a request</param>
        /// <exception cref="System.ArgumentException">Missing server Url;serverUrl</exception>
        /// <exception cref="ArgumentException"></exception>
        public SemanticEnhancement(int webServiceTimeout, Uri serverUrl, ILogger logger, string apiKey = "", Guid correlationId = default) : this(webServiceTimeout, serverUrl, logger)
        {
            if (!string.IsNullOrEmpty(apiKey))
            {
                var regex = new Regex("^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{4})$");
                if (!regex.IsMatch(apiKey))
                {
                    throw new ArgumentException("apikey is invalid", nameof(apiKey));
                }
            }
            _apiKey = apiKey;
            _correlationId = correlationId;
        }


        /// <summary>
        ///     Gets or sets a value indicating whether exceptions should be thown or serialized and returned as XML or JSON. The
        ///     default value is false (ie. serialize exceptions).
        /// </summary>
        /// <value>
        ///     <c>true</c> if exceptions should be thrown; otherwise, <c>false</c>.
        /// </value>
        public bool ThrowExceptions { get; set; }

        /// <summary>
        ///     This service returns a list of all terms in the model that have an attribute of “A-Z Entry” set, whose first letter
        ///     is the letter
        ///     specified as a parameter (or all letters if “all” is specified).
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="query">
        ///     The first letter of all terms required or “all” if all A-Z terms are required. If this value is
        ///     null 'all' is assumed
        /// </param>
        /// <returns>
        ///     <see cref="XmlDocument" /> containing matching terms
        /// </returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        public IXPathNavigable GetAtoZInformation(string taxonomyIndex, string query)
        {
            if (taxonomyIndex == null) throw new ArgumentNullException(nameof(taxonomyIndex));
            if (query == null) query = "all";

            var oDoc = new XmlDocument();

            var sb = new StringBuilder(_serverUrl.ToString());
            sb.Append("?TBDB=");
            sb.Append(HttpUtility.UrlEncode(taxonomyIndex));
            sb.Append("&TEMPLATE=service.xml&SERVICE=az&AZ=");
            sb.Append(HttpUtility.UrlEncode(query));

            try
            {
                WriteMedium("SES Request: " + sb, null);
                oDoc.LoadXml(GetServerResponse(new Uri(sb.ToString())));
            }
            catch (Exception oX)
            {
                WriteException(oX);
                if (ThrowExceptions) throw;
                oDoc.LoadXml($"<Error><Message>{oX.Message}</Message></Error>");
            }
            return oDoc;
        }

        /// <summary>
        ///     Get a list of concept mapped terms for the supplied keywords
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="keywords">The keywords to be matched</param>
        /// <param name="bestMatch">
        ///     <see langword="true" /> will return best match results only, <see langword="false" /> will return all
        ///     results regardless of relevance
        /// </param>
        /// <returns>An XML document containing the suggested terms</returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        [Obsolete("Use method with explicit stopCMStage parameter")]
        public IXPathNavigable GetConceptMap(string taxonomyIndex,
            string facet,
            string filter,
            string keywords,
            bool bestMatch)
        {
            if (bestMatch)
                return GetConceptMap(taxonomyIndex, facet, filter, keywords, -1);
            return GetConceptMap(taxonomyIndex, facet, filter, keywords, 2);
        }

        /// <summary>
        ///     Get a list of concept mapped terms for the supplied keywords
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="keywords">The keywords to be matched</param>
        /// <param name="stopCmStage">
        ///     the CM stop stage (stop_cm_after_stage), or no value sent to SES if lt 0.
        /// </param>
        /// <returns>An XML document containing the suggested terms</returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        public IXPathNavigable GetConceptMap(string taxonomyIndex,
            string facet,
            string filter,
            string keywords,
            int stopCmStage)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));
            if (string.IsNullOrEmpty(keywords)) throw new ArgumentNullException(nameof(keywords));
            var oDoc = new XmlDocument();

            var sb = new StringBuilder(_serverUrl.ToString());
            sb.Append("?TBDB=");
            sb.Append(HttpUtility.UrlEncode(taxonomyIndex));
            sb.Append("&TEMPLATE=service.xml&SERVICE=conceptmap&QUERY=");
            sb.Append(HttpUtility.UrlEncode(keywords));

            AddFacetAndFiler(facet, filter, sb);

            if (stopCmStage >= 0 && stopCmStage < 3)
            {
                sb.Append($"&stop_cm_after_stage={stopCmStage}");
            }

            try
            {
                WriteMedium("SES Request: " + sb, null);
                oDoc.LoadXml(GetServerResponse(new Uri(sb.ToString())));
            }
            catch (Exception oX)
            {
                WriteException(oX);
                if (ThrowExceptions) throw;
                oDoc.LoadXml($"<Error><Message>{oX.Message}</Message></Error>");
            }
            return oDoc;
        }

        /// <summary>
        ///     Returns a list of facets configured for a particular index
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <returns>Dictionary{System.StringSystem.String}.</returns>
        /// <exception cref="SemaphoreConnectionException"></exception>
        public Dictionary<string, string> GetFacetsList(string taxonomyIndex)
        {
            var facets = new Dictionary<string, string>();
            var req = new Uri($"{_serverUrl}?TBDB={HttpUtility.UrlEncode(taxonomyIndex)}&TEMPLATE=service.xml&SERVICE=facetslist");
            WriteMedium("SES Request: " + req, null);

            var webRequest = AuthenticatedRequestBuilder.Instance.Build(req, _apiKey, Logger, _correlationId);
            webRequest.Method = "GET";
            webRequest.Timeout = _timeout*1000;

            try
            {
                var oStop = new Stopwatch();
                oStop.Start();
                var oResponse = (HttpWebResponse) webRequest.GetResponse();
                oStop.Stop();

                WriteLow(
                    "Facets Response received from SES. Time elapsed {0}:{1}.{2}",
                    oStop.Elapsed.Minutes,
                    oStop.Elapsed.Seconds.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                    oStop.Elapsed.Milliseconds);

                var facetsList = new List<KeyValuePair<string, string>>();
                var result = oResponse.GetResponseStream();
                if (result != null)
                    using (var oReader = new StreamReader(result))
                    {
                        var xDoc = new XmlDocument();
                        xDoc.LoadXml(oReader.ReadToEnd());
                        var nodeList = xDoc.SelectNodes("//FACET");

                        if (nodeList == null || nodeList.Count == 0)
                        {
                            throw new SemaphoreConnectionException($"No facets retrieved for {taxonomyIndex} from {webRequest.RequestUri.AbsolutePath}");
                        }

                        // ReSharper disable LoopCanBeConvertedToQuery
                        foreach (XmlNode node in nodeList)
                            // ReSharper restore LoopCanBeConvertedToQuery
                        {
                            if (node.Attributes == null) continue;

                            var id = node.Attributes["ID"].InnerText.Trim();
                            var name = node.Attributes["NAME"].InnerText.Trim();
                            facetsList.Add(new KeyValuePair<string, string>(id, name));
                        }
                    }
                facetsList.Sort(new KeyValueComparer());

                foreach (var entry in facetsList)
                {
                    facets.Add(entry.Key, entry.Value);
                }

                oResponse.Close();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                if (ThrowExceptions) throw;
                throw;
            }
            return facets;
        }

        /// <summary>
        ///     This service returns a list of all terms in the model that have an attribute of “A-Z Entry” set, whose first letter
        ///     is the letter
        ///     specified as a parameter (or all letters if “all” is specified).
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="query">
        ///     The first letter of all terms required or “all” if all A-Z terms are required. If this value is
        ///     null 'all' is assumed
        /// </param>
        /// <returns>
        ///     <see cref="XmlDocument" /> containing matching terms
        /// </returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        public TermInformationResponse GetJsonAtoZInformation(string taxonomyIndex, string query)
        {
            if (taxonomyIndex == null) throw new ArgumentNullException(nameof(taxonomyIndex));
            if (query == null) query = "all";

            string result;

            var sb = new StringBuilder(_serverUrl.ToString());
            sb.Append("?TBDB=");
            sb.Append(HttpUtility.UrlEncode(taxonomyIndex));
            sb.Append("&TEMPLATE=service.json&SERVICE=az&AZ=");
            sb.Append(HttpUtility.UrlEncode(query));

            try
            {
                WriteMedium("SES Request: " + sb, null);
                result = GetServerResponse(new Uri(sb.ToString()));
            }
            catch (Exception oX)
            {
                WriteException(oX);
                if (ThrowExceptions) throw;
                result = "{\"Error\":{\"Message\":\"" + oX.Message + "\"}}";
            }
            return JsonConvert.DeserializeObject<TermInformationResponse>(result);
        }

        /// <summary>
        ///     Get a list of concept mapped terms for the supplied keywords
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="keywords">The keywords to be matched</param>
        /// <param name="bestMatch">
        ///     <see langword="true" /> will return best match results only, <see langword="false" /> will return all
        ///     results regardless of relevance
        /// </param>
        /// <returns>A JSON document containing the suggested terms</returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        public TermInformationResponse GetJsonConceptMap(string taxonomyIndex,
            string facet,
            string filter,
            string keywords,
            bool bestMatch)
        {
            if (bestMatch)
                return GetJsonConceptMap(taxonomyIndex, facet, filter, keywords, -1);
            return GetJsonConceptMap(taxonomyIndex, facet, filter, keywords, 2);
        }

        /// <summary>
        ///     Get a list of concept mapped terms for the supplied keywords
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="keywords">The keywords to be matched</param>
        /// <param name="stopCmStage">
        ///     the CM stop stage (stop_cm_after_stage), or no value sent to SES if lt 0.
        /// </param>
        /// <returns>A JSON document containing the suggested terms</returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        public TermInformationResponse GetJsonConceptMap(string taxonomyIndex,
            string facet,
            string filter,
            string keywords,
            int stopCmStage)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));
            if (string.IsNullOrEmpty(keywords)) throw new ArgumentNullException(nameof(keywords));

            var sb = new StringBuilder(_serverUrl.ToString());
            sb.Append("?TBDB=");
            sb.Append(HttpUtility.UrlEncode(taxonomyIndex));
            sb.Append("&TEMPLATE=service.json&SERVICE=conceptmap&QUERY=");
            sb.Append(HttpUtility.UrlEncode(keywords));

            AddFacetAndFiler(facet, filter, sb);

            if (stopCmStage >= 0 && stopCmStage < 3)
            {
                sb.Append($"&stop_cm_after_stage={stopCmStage}");
            }

            string result;
            try
            {
                WriteMedium("SES Request: " + sb, null);
                result = GetServerResponse(new Uri(sb.ToString()));
            }
            catch (Exception oX)
            {
                WriteException(oX);
                if (ThrowExceptions) throw;
                result = "{\"Error\":{\"Message\":\"" + oX.Message + "\"}}";
            }
            return JsonConvert.DeserializeObject<TermInformationResponse>(result);
        }

        /// <summary>
        ///     Search for a list of terms matching the supplied keywords
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="keywords">The keywords to be matched</param>
        /// <param name="bestMatch">
        ///     <see langword="true" /> will return best match results only, <see langword="false" /> will return all
        ///     results regardless of relevance
        /// </param>
        /// <returns>An XML document containing the suggested terms</returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        public SearchResponse GetJsonModelSearchResults(string taxonomyIndex,
            string facet,
            string filter,
            string keywords,
            bool bestMatch)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));
            if (string.IsNullOrEmpty(keywords)) throw new ArgumentNullException(nameof(keywords));

            string result;
            var sb = new StringBuilder(_serverUrl.ToString());
            sb.Append("?TBDB=");
            sb.Append(HttpUtility.UrlEncode(taxonomyIndex));
            sb.Append("&TEMPLATE=service.json&SERVICE=search&QUERY=");
            sb.Append(HttpUtility.UrlEncode(keywords));

            AddFacetAndFiler(facet, filter, sb);

            if (!bestMatch)
            {
                sb.Append("&stop_cm_after_stage=2");
            }

            try
            {
                WriteMedium("SES Request: " + sb, null);
                result = GetServerResponse(new Uri(sb.ToString()));
            }
            catch (Exception oX)
            {
                WriteException(oX);
                if (ThrowExceptions) throw;
                result = "{\"Error\":{\"Message\":\"" + oX.Message + "\"}}";
            }
            return SearchResponse.FromJsonString(result);
        }


        /// <summary>
        ///     Gets the json related terms.
        /// </summary>
        /// <param name="taxonomyIndex">Index of the taxonomy.</param>
        /// <param name="facet">The facet.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="termId">The term id.</param>
        /// <param name="relationship">The relationship.</param>
        /// <returns>RelatedTermsResponse.</returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        public BrowseResponse GetJsonRelatedTerms(string taxonomyIndex,
            string facet,
            string filter,
            string termId,
            string relationship)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));
            if (string.IsNullOrEmpty(termId)) throw new ArgumentNullException(nameof(termId));
            if (string.IsNullOrEmpty(relationship)) throw new ArgumentNullException(nameof(relationship));

            string result;

            var query = BuildBrowseQueryString(taxonomyIndex, facet, filter, termId, relationship, true);

            try
            {
                WriteMedium("SES Request: " + query, null);
                result = GetServerResponse(new Uri(query));
            }
            catch (Exception oX)
            {
                WriteException(oX);
                if (ThrowExceptions) throw;
                result = "{\"Error\":{\"Message\":\"" + oX.Message + "\"}}";
            }
            return BrowseResponse.FromJsonString(result);
        }


        /// <summary>
        ///     Gets the json root terms.
        /// </summary>
        /// <param name="taxonomyIndex">Index of the taxonomy.</param>
        /// <param name="facet">The facet.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="lang">The language.</param>
        /// <returns>BrowseResponse.</returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        public BrowseResponse GetJsonRootTerms(string taxonomyIndex, string facet, string filter, string lang = "")
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));

            var result = BrowseTerms(taxonomyIndex, facet, filter, string.Empty, true, lang);

            return BrowseResponse.FromJsonString(result);
        }

        /// <summary>
        ///     Gets the json term by id.
        /// </summary>
        /// <param name="taxonomyIndex">Index of the taxonomy.</param>
        /// <param name="facet">The facet.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="termId">The term id.</param>
        /// <param name="lang">The language.</param>
        /// <param name="rows">The number of rows.</param>
        /// <returns>BrowseResponse.</returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        public BrowseResponse GetJsonTermById(string taxonomyIndex,
            string facet,
            string filter,
            string termId,
            string lang = "",
            int? rows = null)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));
            if (string.IsNullOrEmpty(termId)) throw new ArgumentNullException(nameof(termId));

            var result = BrowseTerms(taxonomyIndex, facet, filter, termId, true, lang, "", rows);

            return BrowseResponse.FromJsonString(result);
        }

        /// <summary>
        ///     Returns term information for a collection of term identifiers
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="delimitedIds">A semi-colon(;) delimited list of identifiers for which information is required</param>
        /// <returns>TermInformationResponse.</returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        /// <exception cref="SemaphoreConnectionException"></exception>
        [Obsolete("Use overloaded method with optional language parameter")]
        public TermInformationResponse GetJsonTermInformation(string taxonomyIndex,
            string facet,
            string filter,
            string delimitedIds)
        {
            return GetJsonTermInformation(taxonomyIndex, facet, filter, delimitedIds, null);
        }

        /// <summary>
        ///     Returns term information for a collection of term identifiers
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="delimitedIds">A semi-colon(;) delimited list of identifiers for which information is required</param>
        /// <param name="language">An optional language string</param>
        /// <returns></returns>
        /// <exception cref="SemaphoreConnectionException"></exception>
        public TermInformationResponse GetJsonTermInformation(string taxonomyIndex,
            string facet,
            string filter,
            string delimitedIds,
            string language)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));
            if (string.IsNullOrEmpty(delimitedIds)) throw new ArgumentNullException(nameof(delimitedIds));

            string result;
            var sb = new StringBuilder(_serverUrl.ToString());
            sb.Append("?TBDB=");
            sb.Append(HttpUtility.UrlEncode(taxonomyIndex));
            sb.Append("&TEMPLATE=service.json&SERVICE=term&ID=");
            sb.Append(delimitedIds);
            if (!string.IsNullOrEmpty(language))
            {
                sb.Append("&LANG=");
                sb.Append(HttpUtility.UrlEncode(language));
                sb.Append("&LANGUAGE=");
                sb.Append(HttpUtility.UrlEncode(language));
            }

            AddFacetAndFiler(facet, filter, sb);

            try
            {
                WriteMedium("SES Request: " + sb, null);
                result = GetServerResponse(new Uri(sb.ToString()));
            }
            catch (Exception oX)
            {
                WriteException(oX);
                if (ThrowExceptions) throw;
                result = "{\"Error\":{\"Message\":\"" + oX.Message + "\"}}";
            }
            return TermInformationResponse.FromJsonString(result);
        }

        /// <summary>
        ///     Gets the name of the json term information by.
        /// </summary>
        /// <param name="taxonomyIndex">Index of the taxonomy.</param>
        /// <param name="termName">Name of the term.</param>
        /// <returns>TermInformationResponse.</returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        public TermInformationResponse GetJsonTermInformationByName(string taxonomyIndex, string termName)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));
            if (string.IsNullOrEmpty(termName)) throw new ArgumentNullException(nameof(termName));

            var sb = new StringBuilder(_serverUrl.ToString());
            sb.Append("?TBDB=");
            sb.Append(HttpUtility.UrlEncode(taxonomyIndex));
            sb.Append("&TEMPLATE=service.json&SERVICE=term&TERM=");
            sb.Append(HttpUtility.UrlEncode(termName));

            // Doesn't make sense to use facets or filters - we're getting a specific term
            string result;
            try
            {
                WriteMedium("SES Request: " + sb, null);
                result = GetServerResponse(new Uri(sb.ToString()));
            }
            catch (Exception oX)
            {
                WriteException(oX);
                if (ThrowExceptions) throw;
                result = "{\"Error\":{\"Message\":\"" + oX.Message + "\"}}";
            }
            return TermInformationResponse.FromJsonString(result);
        }

        /// <summary>
        ///     Gets the json term list by prefix.
        /// </summary>
        /// <param name="taxonomyIndex">Index of the taxonomy.</param>
        /// <param name="facet">The facet.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="maxResults">The max results.</param>
        /// <returns>PrefixResponse.</returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        public PrefixResponse GetJsonTermListByPrefix(string taxonomyIndex,
            string facet,
            string filter,
            string prefix,
            int? maxResults)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));
            if (string.IsNullOrEmpty(prefix)) throw new ArgumentNullException(nameof(prefix));

            var sb = new StringBuilder(_serverUrl.ToString());
            sb.Append("?TBDB=");
            sb.Append(HttpUtility.UrlEncode(taxonomyIndex));
            sb.Append("&TEMPLATE=service.json&SERVICE=prefix&TERM_PREFIX=");
            sb.Append(HttpUtility.UrlEncode(prefix));

            string result;

            if (maxResults != null)
            {
                sb.Append("&prefix_results_limit=");
                sb.Append(maxResults.Value.ToString(CultureInfo.InvariantCulture));
            }

            AddFacetAndFiler(facet, filter, sb);

            try
            {
                WriteMedium("SES Request: " + sb, null);
                result = GetServerResponse(new Uri(sb.ToString()));
            }
            catch (Exception oX)
            {
                WriteException(oX);
                if (ThrowExceptions) throw;
                result = "{\"Error\":{\"Message\":\"" + oX.Message + "\"}}";
            }
            return PrefixResponse.FromJsonString(result);
        }

        /// <summary>
        ///     Search for a list of terms matching the supplied keywords
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="keywords">The keywords to be matched</param>
        /// <param name="bestMatch">
        ///     <see langword="true" /> will return best match results only, <see langword="false" /> will return all
        ///     results regardless of relevance
        /// </param>
        /// <returns>An XML document containing the suggested terms</returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        public IXPathNavigable GetModelSearchResults(string taxonomyIndex,
            string facet,
            string filter,
            string keywords,
            bool bestMatch)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));
            if (string.IsNullOrEmpty(keywords)) throw new ArgumentNullException(nameof(keywords));
            var oDoc = new XmlDocument();

            var sb = new StringBuilder(_serverUrl.ToString());
            sb.Append("?TBDB=");
            sb.Append(HttpUtility.UrlEncode(taxonomyIndex));
            sb.Append("&TEMPLATE=service.xml&SERVICE=search&QUERY=");
            sb.Append(HttpUtility.UrlEncode(keywords));

            AddFacetAndFiler(facet, filter, sb);

            if (!bestMatch)
            {
                sb.Append("&stop_cm_after_stage=2");
            }

            try
            {
                WriteMedium("SES Request: " + sb, null);
                oDoc.LoadXml(GetServerResponse(new Uri(sb.ToString())));
            }
            catch (Exception oX)
            {
                WriteException(oX);
                if (ThrowExceptions) throw;
                oDoc.LoadXml($"<Error><Message>{oX.Message}</Message></Error>");
            }
            return oDoc;
        }

        /// <summary>
        ///     Returns details of terms related to a configured term
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="termId">The identifier of the term for which information is required</param>
        /// <param name="relationship">The relationship type. For example: NT=Narrower Term, BT=Broader Term</param>
        /// <returns>XDocument.</returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        [Obsolete("Use overloaded method with optional language parameter")]
        public XDocument GetRelatedTermsAsXDoc(string taxonomyIndex,
            string facet,
            string filter,
            string termId,
            string relationship)
        {
            return GetRelatedTermsAsXDoc(taxonomyIndex, facet, filter, termId, relationship, null);
        }

        /// <summary>
        ///     Returns details of terms related to a configured term
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="termId">The identifier of the term for which information is required</param>
        /// <param name="relationship">The relationship type. For example: NT=Narrower Term, BT=Broader Term</param>
        /// <param name="language"></param>
        /// <returns></returns>
        public XDocument GetRelatedTermsAsXDoc(string taxonomyIndex,
            string facet,
            string filter,
            string termId,
            string relationship,
            string language)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));
            if (string.IsNullOrEmpty(termId)) throw new ArgumentNullException(nameof(termId));
            if (string.IsNullOrEmpty(relationship)) throw new ArgumentNullException(nameof(relationship));

            XDocument oDoc;

            var query = BuildBrowseQueryString(taxonomyIndex, facet, filter, termId, relationship, false, language);

            try
            {
                WriteMedium("SES Request: " + query, null);
                oDoc = XDocument.Parse(GetServerResponse(new Uri(query)));
            }
            catch (Exception oX)
            {
                WriteException(oX);
                if (ThrowExceptions) throw;
                oDoc =
                    XDocument.Parse($"<Error><Message>{oX.Message}</Message></Error>");
            }
            return oDoc;
        }


        /// <summary>
        ///     Use to return details of the root terms for a configured taxonomy
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <returns>
        ///     <see cref="XmlDocument" /> containing root terms
        /// </returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        [Obsolete("Use overloaded method with optional language parameter")]
        public IXPathNavigable GetRootTerms(string taxonomyIndex, string facet, string filter)
        {
            return GetRootTerms(taxonomyIndex, facet, filter, null);
        }

        /// <summary>
        ///     Use to return details of the root terms for a configured taxonomy
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="language"></param>
        /// <returns>
        ///     <see cref="XmlDocument" /> containing root terms
        /// </returns>
        public IXPathNavigable GetRootTerms(string taxonomyIndex, string facet, string filter, string language)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));

            var oDoc = new XmlDocument();

            oDoc.LoadXml(BrowseTerms(taxonomyIndex, facet, filter, string.Empty, false, language));

            return oDoc;
        }


        /// <summary>
        ///     Use to return details of the root terms for a configured taxonomy
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="language">The language specific version of the index to use</param>
        /// <param name="hierType">The child relationship type</param>
        /// <returns>
        ///     <see cref="XmlDocument" /> containing root terms
        /// </returns>
        public IXPathNavigable GetRootTerms(string taxonomyIndex, string facet, string filter, string language, string hierType)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));

            var oDoc = new XmlDocument();

            oDoc.LoadXml(BrowseTerms(taxonomyIndex, facet, filter, string.Empty, false, language, hierType));

            return oDoc;
        }

        /// <summary>
        ///     Use to return details of the root terms for a configured taxonomy
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <returns>
        ///     <see cref="XDocument" /> containing root terms
        /// </returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        [Obsolete("Use overloaded method with optional language parameter")]
        public XDocument GetRootTermsAsXDoc(string taxonomyIndex, string facet, string filter)
        {
            return GetRootTermsAsXDoc(taxonomyIndex, facet, filter, null);
        }

        /// <summary>
        ///     Use to return details of the root terms for a configured taxonomy
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="language">An optional parameter denoting the language variant to be returned</param>
        /// <returns>
        ///     <see cref="XDocument" /> containing root terms
        /// </returns>
        public XDocument GetRootTermsAsXDoc(string taxonomyIndex, string facet, string filter, string language)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));

            var oDoc = XDocument.Parse(BrowseTerms(taxonomyIndex, facet, filter, string.Empty, false, language));

            return oDoc;
        }


         /// <summary>
        ///     Use to return details of the root terms for a configured taxonomy
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="language">An optional parameter denoting the language variant to be returned</param>
        ///  /// <param name="hierType">The child relationship type</param>
        /// <returns>
        ///     <see cref="XDocument" /> containing root terms
        /// </returns>
        public XDocument GetRootTermsAsXDoc(string taxonomyIndex, string facet, string filter, string language, string hierType)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));

            var oDoc = XDocument.Parse(BrowseTerms(taxonomyIndex, facet, filter, string.Empty, false, language,hierType));

            return oDoc;
        }


        /// <summary>
        ///     Returns an xml document that describes the structure of the model.
        ///     Only available on v3.3 and later
        /// </summary>
        /// <param name="taxonomyIndex">Index of the taxonomy.</param>
        /// <returns>Structure.</returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        /// <exception cref="ArgumentNullException"></exception>
        public Structure GetStructure(string taxonomyIndex)
        {
            if (taxonomyIndex == null) throw new ArgumentNullException(nameof(taxonomyIndex));

            var restUri =
                new Uri(_serverUrl + (_serverUrl.ToString().EndsWith("/") ? string.Empty : "/") +
                        taxonomyIndex);

            try
            {
                WriteMedium("SES Request: {0}", restUri);
                var response = GetServerResponse<Schemas.Structure.Semaphore>(restUri);
                return response?.Structure;
            }
            catch (Exception oX)
            {
                WriteException(oX);
                if (ThrowExceptions) throw;
            }

            return null;
        }

        /// <summary>
        ///     Returns a list of taxonomies configured on the current Search Enhancement Server instance
        /// </summary>
        /// <returns>IEnumerable{System.String}.</returns>
        /// <exception cref="SemaphoreConnectionException"></exception>
        public IEnumerable<string> GetTaxonomyIndexList()
        {
            var indices = new List<string>();

            var req = new Uri(_serverUrl + "?TEMPLATE=service.xml&SERVICE=modelslist");
            WriteMedium("SES Request: " + req, null);

            var oRequest = AuthenticatedRequestBuilder.Instance.Build(req, _apiKey, Logger, _correlationId);
            oRequest.Method = "GET";
            oRequest.Timeout = _timeout*1000;

            try
            {
                var oStop = new Stopwatch();
                oStop.Start();
                var oResponse = (HttpWebResponse) oRequest.GetResponse();
                oStop.Stop();

                WriteLow("Response received from SES. Time elapsed {0}:{1}.{2}",
                    oStop.Elapsed.Minutes,
                    oStop.Elapsed.Seconds.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                    oStop.Elapsed.Milliseconds);

                var result = oResponse.GetResponseStream();
                if (result != null)
                    using (var oReader = new StreamReader(result))
                    {
                        var xDoc = new XmlDocument();
                        xDoc.LoadXml(oReader.ReadToEnd());
                        var nodeList = xDoc.SelectNodes("//MODEL");

                        if (nodeList == null || nodeList.Count == 0)
                        {
                            throw new SemaphoreConnectionException(
                                $"No Models retrieved from {oRequest.RequestUri.AbsolutePath}");
                        }

                        // ReSharper disable LoopCanBeConvertedToQuery
                        foreach (XmlNode node in nodeList)
                            // ReSharper restore LoopCanBeConvertedToQuery
                        {
                            var name = node.SelectSingleNode("NAME");
                            indices.Add(name?.InnerText.Trim() ?? node.InnerText.Trim());
                        }
                    }
                oResponse.Close();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                if (ThrowExceptions) throw;
            }
            indices.Sort();
            return indices;
        }

        /// <summary>
        ///     Returns a list of languages available for a model on the current Search Enhancement Server instance
        /// </summary>
        /// <returns>IEnumerable{System.String}.</returns>
        /// <exception cref="SemaphoreConnectionException"></exception>
        public IEnumerable<string> GetModelLanguageList(string taxonomyIndex)
        {
            var languages = new List<string>();

            var req = new Uri(_serverUrl + "?TEMPLATE=service.xml&SERVICE=modelslist");
            WriteMedium("SES Request: " + req, null);

            var oRequest = AuthenticatedRequestBuilder.Instance.Build(req, _apiKey, Logger, _correlationId);
            oRequest.Method = "GET";
            oRequest.Timeout = _timeout * 1000;

            try
            {
                var oStop = new Stopwatch();
                oStop.Start();
                var oResponse = (HttpWebResponse)oRequest.GetResponse();
                oStop.Stop();

                WriteLow("Response received from SES. Time elapsed {0}:{1}.{2}",
                    oStop.Elapsed.Minutes,
                    oStop.Elapsed.Seconds.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                    oStop.Elapsed.Milliseconds);

                var result = oResponse.GetResponseStream();
                if (result != null)
                    using (var oReader = new StreamReader(result))
                    {
                        var xDoc = new XmlDocument();
                        xDoc.LoadXml(oReader.ReadToEnd());
                        var nodeList = xDoc.SelectNodes("//MODEL");

                        if (nodeList == null || nodeList.Count == 0)
                        {
                            throw new SemaphoreConnectionException(
                                $"No Models retrieved from {oRequest.RequestUri.AbsolutePath}");
                        }

                        foreach (XmlNode node in nodeList)
                        {
                            var name = node.SelectSingleNode("NAME");
                            string innerName = name?.InnerText.Trim() ?? name.InnerText.Trim();
                            if (!string.Equals(innerName, taxonomyIndex)) continue;
                            XmlNodeList languageNodes = node.SelectNodes("LANGUAGE");
                            if (languageNodes == null) continue;
                            foreach (XmlNode languageNode in languageNodes)
                            {
                                languages.Add(languageNode.InnerText.Trim());
                            }
                        }
                    }

                oResponse.Close();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                if (ThrowExceptions) throw;
            }

            return languages;
        }

        /// <summary>
        ///     Returns information for a particular term specified by ID
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="termId">The term id.</param>
        /// <returns>XmlDocument containing term information</returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        [Obsolete("Use overloaded method with optional language parameter")]
        public IXPathNavigable GetTermById(string taxonomyIndex,
            string facet,
            string filter,
            string termId)
        {
            return GetTermById(taxonomyIndex, facet, filter, termId, null);
        }

        /// <summary>
        ///     Returns information for a particular term specified by ID
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="termId"></param>
        /// <param name="language">An optional parameter denoting the language variant to be returned</param>
        /// <returns>XmlDocument containing term information</returns>
        public IXPathNavigable GetTermById(string taxonomyIndex,
            string facet,
            string filter,
            string termId,
            string language)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));
            if (string.IsNullOrEmpty(termId)) throw new ArgumentNullException(nameof(termId));

            var oDoc = new XmlDocument();

            oDoc.LoadXml(BrowseTerms(taxonomyIndex, facet, filter, termId, false, language));

            return oDoc;
        }

        /// <summary>
        ///     Returns information for a particular term specified by ID
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="termId">The identifier of the term for which information is required</param>
        /// <returns>
        ///     <see cref="XDocument" /> containing term information
        /// </returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        [Obsolete("Use overloaded method with optional language parameter")]
        public XDocument GetTermByIdAsXDoc(string taxonomyIndex,
            string facet,
            string filter,
            string termId)
        {
            return GetTermByIdAsXDoc(taxonomyIndex, facet, filter, termId, null);
        }

        /// <summary>
        ///     Returns information for a particular term specified by ID
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="termId">The identifier of the term for which information is required</param>
        /// <param name="language"></param>
        /// <returns>
        ///     <see cref="XDocument" /> containing term information
        /// </returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        public XDocument GetTermByIdAsXDoc(string taxonomyIndex,
            string facet,
            string filter,
            string termId,
            string language)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));
            if (string.IsNullOrEmpty(termId)) throw new ArgumentNullException(nameof(termId));

            var oDoc = XDocument.Parse(BrowseTerms(taxonomyIndex, facet, filter, termId, false, language));

            return oDoc;
        }

        /// <summary>
        ///     Returns term information for a collection of term identifiers
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="delimitedIds">A semi-colon(;) delimited list of identifiers for which information is required</param>
        /// <returns>IXPathNavigable.</returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        /// <exception cref="SemaphoreConnectionException"></exception>
        public IXPathNavigable GetTermInformationByIds(string taxonomyIndex,
            string facet,
            string filter,
            string delimitedIds)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));
            if (string.IsNullOrEmpty(delimitedIds)) throw new ArgumentNullException(nameof(delimitedIds));
            var oDoc = new XmlDocument();

            var sb = new StringBuilder(_serverUrl.ToString());
            sb.Append("?TBDB=");
            sb.Append(HttpUtility.UrlEncode(taxonomyIndex));
            sb.Append("&TEMPLATE=service.xml&SERVICE=term&ID=");
            sb.Append(HttpUtility.UrlEncode(delimitedIds));

            AddFacetAndFiler(facet, filter, sb);

            try
            {
                WriteMedium("SES Request: " + sb, null);
                oDoc.LoadXml(GetServerResponse(new Uri(sb.ToString())));
            }
            catch (Exception oX)
            {
                WriteException(oX);
                if (ThrowExceptions) throw;
                oDoc.LoadXml($"<Error><Message>{oX.Message}</Message></Error>");
            }
            return oDoc;
        }

        /// <summary>
        ///     Get a list of term information for the supplied term name
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="termName">The term name to search the Semaphore SES server for</param>
        /// <returns>An XML document containing the term information</returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        public IXPathNavigable GetTermInformationByName(string taxonomyIndex, string termName)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));
            if (string.IsNullOrEmpty(termName)) throw new ArgumentNullException(nameof(termName));
            var oDoc = new XmlDocument();

            var sb = new StringBuilder(_serverUrl.ToString());
            sb.Append("?TBDB=");
            sb.Append(HttpUtility.UrlEncode(taxonomyIndex));
            sb.Append("&TEMPLATE=service.xml&SERVICE=term&TERM=");
            sb.Append(HttpUtility.UrlEncode(termName));

            // Doesn't make sense to use facets or filters - we're getting a specific term

            try
            {
                WriteMedium("SES Request: " + sb, null);
                oDoc.LoadXml(GetServerResponse(new Uri(sb.ToString())));
            }
            catch (Exception oX)
            {
                WriteException(oX);
                if (ThrowExceptions) throw;
                oDoc.LoadXml($"<Error><Message>{oX.Message}</Message></Error>");
            }
            return oDoc;
        }


        /// <summary>
        ///     Returns a list of terms beginning with the configured prefix
        /// </summary>
        /// <param name="taxonomyIndex">The name of the index to be searched</param>
        /// <param name="facet">The name of a particular facet to be searched. If empty, terms from all facets will be returned</param>
        /// <param name="filter">An optional additional filter string used to filter the taxonomy being queried</param>
        /// <param name="prefix">The prefix to be matched</param>
        /// <param name="maxResults">
        ///     the maximum number of results to be returned. If <see langword="null" /> is passed the server default will be used.
        /// </param>
        /// <returns>An XML document containing the suggested terms</returns>
        /// <exception cref="System.ArgumentNullException">taxonomyIndex</exception>
        /// <exception cref="SemaphoreConnectionException"></exception>
        public IXPathNavigable GetTermListByPrefix(string taxonomyIndex,
            string facet,
            string filter,
            string prefix,
            int? maxResults)
        {
            if (string.IsNullOrEmpty(taxonomyIndex))
                throw new ArgumentNullException(nameof(taxonomyIndex));
            if (string.IsNullOrEmpty(prefix)) throw new ArgumentNullException(nameof(prefix));

            var oDoc = new XmlDocument();

            var sb = new StringBuilder(_serverUrl.ToString());
            sb.Append("?TBDB=");
            sb.Append(HttpUtility.UrlEncode(taxonomyIndex));
            sb.Append("&TEMPLATE=service.xml&SERVICE=prefix&TERM_PREFIX=");
            sb.Append(HttpUtility.UrlEncode(prefix));

            if (maxResults != null)
            {
                sb.Append("&prefix_results_limit=");
                sb.Append(maxResults.Value.ToString(CultureInfo.InvariantCulture));
            }

            AddFacetAndFiler(facet, filter, sb);

            try
            {
                WriteMedium("SES Request: " + sb, null);
                oDoc.LoadXml(GetServerResponse(new Uri(sb.ToString())));
            }
            catch (Exception oX)
            {
                WriteException(oX);
                if (ThrowExceptions) throw;
                oDoc.LoadXml($"<Error><Message>{oX.Message}</Message></Error>");
            }
            return oDoc;
        }

        /// <summary>
        ///     Returns the version of Semantic Enhancement Server
        /// </summary>
        /// <returns>Version.</returns>
        /// <exception cref="SemaphoreConnectionException"></exception>
        public string GetVersion()
        {
            string response = null;

            var req = new Uri(_serverUrl + "?TEMPLATE=service.xml&SERVICE=versions");
            WriteMedium("SES Request: " + req, null);

            try
            {
                var xDoc = new XmlDocument();
                xDoc.LoadXml(GetServerResponse(req));
                var isOldVersionFormat = false;
                var nodeList = xDoc.SelectNodes("//MAIN_VERSIONS/VERSION[@NAME='SES version']");

                if (nodeList == null || nodeList.Count == 0)
                {
                    nodeList = xDoc.SelectNodes("//MAIN_VERSIONS/VERSION[@NAME='SES build']");
                    if (nodeList == null || nodeList.Count == 0)
                    {
                        throw new SemaphoreConnectionException(
                            $"No version retrieved from {req}");
                    }

                    isOldVersionFormat = true;
                }

                foreach (XmlNode node in nodeList)
                {
                    if (node.Attributes?["REVISION"] != null)
                    {
                        var revision = node.Attributes["REVISION"].Value;
                        if (isOldVersionFormat)
                        {
                            revision = revision.Replace("Semaphore ", "");
                            revision = revision.Replace(" - Semantic Enhancement Server ", "-");

                            var version = new Version(revision.Replace("-r", "."));
                            if (version.Revision <= 0)
                            {
                                //Make sure the revision is in the correct column (ie. if version is 3.5 r 12345, make it 3.5.0.12345 as opposed to 3.5.12345.0)
                                version = new Version(version.Major, version.Minor, 0, version.Build);
                            }

                            response = version.ToString();
                        }
                        else
                        {
                            response = revision;
                        }
                        
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }

            return response;
        }

        /// <summary>
        ///     Adds the facet and filer.
        /// </summary>
        /// <param name="facet">The facet.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="sb">The sb.</param>
        private static void AddFacetAndFiler(string facet, string filter, StringBuilder sb)
        {
            if (!string.IsNullOrEmpty(facet) && facet != "0")
            {
                sb.Append("&FILTER=FA");
                sb.Append(HttpUtility.UrlEncode(facet));
            }

            if (!string.IsNullOrEmpty(filter))
            {
                if (!filter.StartsWith("&"))
                {
                    sb.Append("&");
                }
                sb.Append(filter);
            }
        }

        /// <summary>
        ///     Browses the terms.
        /// </summary>
        /// <param name="taxonomyIndex">Index of the taxonomy.</param>
        /// <param name="facet">The facet.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="termId">The term id.</param>
        /// <param name="useJson">
        ///     if set to <c>true</c> [use json].
        /// </param>
        /// <param name="language"></param>
        /// <param name="hierType"></param>
        /// <param name="rows">The number of rows.</param>
        /// <returns>System.String.</returns>
        private string BrowseTerms(string taxonomyIndex,
            string facet,
            string filter,
            string termId,
            bool useJson = false,
            string language = "",
            string hierType = "",
            int? rows = null)
        {
            var query = BuildBrowseQueryString(taxonomyIndex, facet, filter, termId, hierType, useJson, language, rows);

            try
            {
                WriteMedium("SES Request: " + query, null);
                return GetServerResponse(new Uri(query));
            }
            catch (Exception oX)
            {
                WriteException(oX);
                if (ThrowExceptions) throw;
                if (useJson)
                {
                    return "{\"Error\":{\"Message\":\"" + oX.Message + "\"}}";
                }
                return $"<Error><Message>{oX.Message}</Message></Error>";
            }
        }


        /// <summary>
        ///     Builds the browse query string.
        /// </summary>
        /// <param name="taxonomyIndex">Index of the taxonomy.</param>
        /// <param name="facet">The facet.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="termId">The term id.</param>
        /// <param name="hiertype">The hiertype.</param>
        /// <param name="useJson">
        ///     if set to <c>true</c> [use json].
        /// </param>
        /// <param name="language">The language.</param>
        /// <param name="rows">The number of rows.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="SemaphoreConnectionException">Ontology Server URL not configured</exception>
        private string BuildBrowseQueryString(string taxonomyIndex,
            string facet,
            string filter,
            string termId,
            string hiertype,
            bool useJson = false,
            string language = "",
            int? rows = null)
        {
            if (_serverUrl == null)
                throw new SemaphoreConnectionException("Ontology Server URL not configured");

            var sb = new StringBuilder(_serverUrl.ToString());
            sb.Append("?TBDB=");
            sb.Append(HttpUtility.UrlEncode(taxonomyIndex));
            if (useJson)
            {
                sb.Append("&TEMPLATE=service.json&SERVICE=browse&ID=");
            }
            else
            {
                sb.Append("&TEMPLATE=service.xml&SERVICE=browse&ID=");
            }

            sb.Append(termId);

            if (!string.IsNullOrEmpty(hiertype))
            {
                sb.Append("&HIERTYPE=");
                sb.Append(HttpUtility.UrlEncode(hiertype));
            }

            if (!string.IsNullOrEmpty(language))
            {
                sb.Append("&LANG=");
                sb.Append(HttpUtility.UrlEncode(language));
                sb.Append("&LANGUAGE=");
                sb.Append(HttpUtility.UrlEncode(language));
            }

            if (rows != null)
            {
                sb.Append("&ROWS=");
                sb.Append(rows.Value.ToString());
            }

            AddFacetAndFiler(facet, filter, sb);

            return sb.ToString();
        }

        /// <summary>
        ///     Gets the server response.
        /// </summary>
        /// <param name="serverWithQuery">The server with query.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="SemaphoreConnectionException"></exception>
        private string GetServerResponse(Uri serverWithQuery)
        {
            var oRequest = AuthenticatedRequestBuilder.Instance.Build(serverWithQuery, _apiKey, Logger, _correlationId);
            oRequest.Method = "GET";
            oRequest.Timeout = _timeout*1000;
            try
            {
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
                    return sResponse;
                }
                WriteError("HTTP response code error: {0}", oResponse.StatusCode);
            }
            catch (Exception oX)
            {
                WriteError("Error connecting to SES", null);
                WriteException(oX);
                if (ThrowExceptions) throw;
            }
            return string.Empty;
        }

        /// <summary>
        ///     Gets the server response.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serverWithQuery">The server with query.</param>
        /// <returns>``0.</returns>
        /// <exception cref="SemaphoreConnectionException"></exception>
        private T GetServerResponse<T>(Uri serverWithQuery) where T : class
        {
            var oRequest = AuthenticatedRequestBuilder.Instance.Build(serverWithQuery, _apiKey, Logger, _correlationId);
            oRequest.Method = "GET";
            oRequest.Timeout = _timeout*1000;
            var serializer = new XmlSerializer(typeof(T));
            try
            {
                var oResponse = (HttpWebResponse) oRequest.GetResponse();

                var status = (int) oResponse.StatusCode;
                if (status >= 400)
                {
                    throw new SemaphoreConnectionException($"{status}: {oResponse.StatusDescription}");
                }

                if (oResponse.StatusCode == HttpStatusCode.OK)
                {
                    var s = oResponse.GetResponseStream();
                    if (s != null)
                    {
                        var result = serializer.Deserialize(s) as T;
                        return result;
                    }

                    return null;
                }
                WriteError("HTTP response code error: {0}", oResponse.StatusCode);
            }
            catch (Exception oX)
            {
                WriteError("Error connecting to SES", null);
                WriteException(oX);
                if (ThrowExceptions) throw;
            }
            return null;
        }

        #region Nested type: KeyValueComparer

        /// <summary>
        ///     Class KeyValueComparer
        /// </summary>
        private class KeyValueComparer : IComparer<KeyValuePair<string, string>>
        {
            #region IComparer<KeyValuePair<string,string>> Members

            /// <summary>
            ///     Compares the specified x.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <returns>System.Int32.</returns>
            public int Compare(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
            {
                return string.CompareOrdinal(x.Value, y.Value);
            }

            #endregion
        }

        #endregion
    }
}