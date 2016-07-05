using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;

namespace Smartlogic.Semaphore.Api
{
    /// <summary>
    ///     Classification Server proxy
    /// </summary>
    public class ClassificationServer : LoggingProxyBase
    {
        private readonly Uri _serverUrl;
        private readonly int _timeout;

        /// <summary>
        ///     A proxy class used for communicating with an instance of Semaphore Classification Server
        /// </summary>
        /// <param name="webServiceTimeout">An integer representing the number of seconds to wait before timing out</param>
        /// <param name="serverUrl">
        ///     An absolute <see cref="Uri" /> indicating the Classification Server endpoint
        /// </param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentException"></exception>
        [Obsolete("Use contructor that accepts ILogger to capture logging information instead.")]
        public ClassificationServer(int webServiceTimeout, Uri serverUrl)
            : base()
        {
            if (serverUrl == null) throw new ArgumentException("Missing server Url", "serverUrl");
            if (webServiceTimeout <= 0) throw new ArgumentException("Web service timeout must be greater than 0", "webServiceTimeout");
            if (webServiceTimeout > int.MaxValue / 1000)
                throw new ArgumentException("Web service timeout must be less than " + Int16.MaxValue / 1000, "webServiceTimeout");
            if (!serverUrl.IsAbsoluteUri) throw new ArgumentException("Server Url must be an absolute Uri", "serverUrl");

            _serverUrl = serverUrl;
            _timeout = webServiceTimeout;
            BoundaryString = Guid.NewGuid().ToString().Replace("-", "");
        }

        /// <summary>
        ///     A proxy class used for communicating with an instance of Semaphore Classification Server
        /// </summary>
        /// <param name="webServiceTimeout">An integer representing the number of seconds to wait before timing out</param>
        /// <param name="serverUrl">
        ///     An absolute <see cref="Uri" /> indicating the Classification Server endpoint
        /// </param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentException"></exception>
        public ClassificationServer(int webServiceTimeout, Uri serverUrl, ILogger logger)
            : base(logger)
        {
            if (serverUrl == null) throw new ArgumentException("Missing server Url", "serverUrl");
            if (logger == null) throw new ArgumentNullException("logger");
            if (webServiceTimeout <= 0) throw new ArgumentException("Web service timeout must be greater than 0", "webServiceTimeout");
            if (webServiceTimeout > int.MaxValue / 1000)
                throw new ArgumentException("Web service timeout must be less than " + Int16.MaxValue / 1000, "webServiceTimeout");
            if (!serverUrl.IsAbsoluteUri) throw new ArgumentException("Server Url must be an absolute Uri", "serverUrl");

            _serverUrl = serverUrl;
            _timeout = webServiceTimeout;
            BoundaryString = Guid.NewGuid().ToString().Replace("-", "");
        }

        /// <summary>
        ///     Passes values and binary content to Classification Server for processing
        /// </summary>
        /// <param name="title">The title of the item being classified. This value is optional.</param>
        /// <param name="document">A byte array representing binary content to be classified</param>
        /// <param name="fileName">Where binary content is being classified, a filename is required</param>
        /// <param name="metaValues">A dictionary of values to be classified</param>
        /// <param name="altBody">
        ///     When classifying HTML content, <paramref name="altBody" /> is used to pass the body of the page
        /// </param>
        /// <param name="options">Additional classification options. If no values are passed, server defaults are used</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public ClassificationResult Classify(string title,
                                             byte[] document,
                                             string fileName,
                                             Dictionary<string, string> metaValues,
                                             string altBody,
                                             ClassificationOptions options)
        {
            if (metaValues == null) throw new ArgumentException("Missing metaValues", "metaValues");
            if (document != null && string.IsNullOrEmpty(fileName)) throw new ArgumentException("Documents must have an associated filename", "fileName");
            if (document == null && !string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Filename parameter should not be set unless an associated document is passed via the document parameter",
                                            "document");

            WriteLow("Calling Tagging API (Binary). Item Title={0}, FileName={1}", title, fileName);

            var oFrmFields =
                new Dictionary<string, string>
                    {
                        {"operation", "CLASSIFY"},
                        {"title", title},
                        {"body", altBody}
                    };

            if (options != null)
            {
                if (options.Threshold.HasValue)
                {
                    oFrmFields.Add("threshold", options.Threshold.Value.ToString(CultureInfo.InvariantCulture));
                }
                if (!string.IsNullOrEmpty(options.Type))
                {
                    oFrmFields.Add("type", options.Type);
                }
                if (!string.IsNullOrEmpty(options.ClusteringType))
                {
                    oFrmFields.Add("clustering_type", options.ClusteringType);
                }

                if (options.ClusteringThreshold.HasValue)
                {
                    oFrmFields.Add("clustering_threshold", options.ClusteringThreshold.Value.ToString(CultureInfo.InvariantCulture));
                }

                switch (options.ArticleType)
                {
                    case ArticleType.Default:
                    case ArticleType.ServerDefault:
                        break;
                    case ArticleType.SingleArticle:
                        oFrmFields.Add("singlearticle", "1");
                        break;
                    case ArticleType.MultiArticle:
                        oFrmFields.Add("multiarticle", "1");
                        break;
                }
            }

            foreach (string sKey in metaValues.Keys)
                oFrmFields.Add("meta_" + sKey, metaValues[sKey]);

            List<FileUpload> oFiles = null;

            if (!string.IsNullOrEmpty(fileName) && document != null)
            {
                oFiles = new List<FileUpload>
                    {
                        new FileUpload
                            {
                                FieldName = "UploadFile",
                                FileName = fileName,
                                Contents = document
                            }
                    };
            }
            return PerformWebClassify(oFrmFields, oFiles);
        }


        /// <summary>
        ///     Retrieves a list of the Classification Classes available on the current Classification Server instance
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SemaphoreConnectionException"></exception>
        public Collection<string> GetClassificationClasses()
        {
            var csClasses = new Collection<string>();
            var request = (HttpWebRequest)WebRequest.Create(new Uri(_serverUrl + "?operation=LISTRULENETCLASSES"));
            WriteLow("CS Request: " + request.RequestUri, null);
            request.Method = "GET";
            request.Timeout = (_timeout * 1000);

            try
            {
                var oStop = new Stopwatch();
                oStop.Start();
                var oResponse = (HttpWebResponse)request.GetResponse();
                oStop.Stop();

                WriteLow("Response received from CS. Time elapsed {0}:{1}.{2}",
                         oStop.Elapsed.Minutes,
                         oStop.Elapsed.Seconds.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0').PadLeft(2, '0'),
                         oStop.Elapsed.Milliseconds);

                Stream stream = oResponse.GetResponseStream();
                if (stream != null)
                    using (var oReader = new StreamReader(stream))
                    {
                        var xDoc = new XmlDocument();
                        xDoc.LoadXml(oReader.ReadToEnd());
                        XmlNodeList nodeList = xDoc.SelectNodes("//Class");

                        if (nodeList == null || nodeList.Count == 0)
                        {
                            throw new SemaphoreConnectionException(string.Format("No Classes retrieved from {0}",
                                                                                 request.RequestUri.AbsolutePath));
                        }

                        foreach (XmlNode node in nodeList)
                        {
                            if (node.Attributes != null)
                            {
                                string attrib = node.Attributes["Name"].Value;
                                if (!attrib.StartsWith("TEXTMINE_"))
                                    csClasses.Add(attrib);
                            }
                        }
                    }
                oResponse.Close();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }


            return csClasses;
        }

        /// <summary>
        ///     Returns a list of languages currently configured on Classification Server
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SemaphoreConnectionException"></exception>
        public Collection<ClassificationLanguage> GetLanguages()
        {
            var languages = new Collection<ClassificationLanguage>();
            var request = (HttpWebRequest)WebRequest.Create(new Uri(_serverUrl + "?operation=listlanguages"));
            WriteLow("CS Request: " + request.RequestUri, null);

            request.Method = "GET";
            request.Timeout = (_timeout * 1000);

            try
            {
                var oStop = new Stopwatch();
                oStop.Start();
                var oResponse = (HttpWebResponse)request.GetResponse();
                oStop.Stop();

                WriteLow(
                    string.Format("Response received from CS. Time elapsed {0}:{1}.{2}",
                                  oStop.Elapsed.Minutes,
                                  oStop.Elapsed.Seconds.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0').PadLeft(2, '0'),
                                  oStop.Elapsed.Milliseconds),
                    null);

                Stream stream = oResponse.GetResponseStream();
                if (stream != null)
                    using (var oReader = new StreamReader(stream))
                    {
                        var xDoc = new XmlDocument();
                        xDoc.LoadXml(oReader.ReadToEnd());
                        XmlNode top = xDoc.SelectSingleNode("//languages");
                        if (top != null && top.Attributes != null)
                        {
                            string type = top.Attributes["type"].Value;
                            XmlNodeList nodeList = xDoc.SelectNodes("//language");

                            if (nodeList == null || nodeList.Count == 0)
                            {
                                WriteMedium(
                                    "No languages retrieved from {0}",
                                    request.RequestUri.AbsolutePath);
                                return new Collection<ClassificationLanguage>();
                            }

                            foreach (XmlNode node in nodeList)
                            {
                                if (node.Attributes != null)
                                {
                                    try
                                    {
                                        string id = node.Attributes["id"].Value;
                                        bool hasRulesDefined = node.Attributes["has_rules_defined"] != null &&
                                                               node.Attributes["has_rules_defined"].Value == "true";
                                        bool isDefault = node.Attributes["default"].Value == "true";
                                        string display = node.Attributes["display"].Value;
                                        string name = node.Attributes["name"].Value;
                                        var newLang = new ClassificationLanguage
                                        {
                                            Id = id,
                                            HasRulesDefined = hasRulesDefined,
                                            IsDefault = isDefault,
                                            DisplayName = display,
                                            Name = name,
                                            Type = type
                                        };
                                        languages.Add(newLang);
                                    }
                                    catch (Exception ex)
                                    {
                                        WriteException(ex);
                                    }
                                }
                            }
                        }
                    }
                oResponse.Close();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }


            return languages;
        }

        /// <summary>
        ///     Retrieves a list of the text mining Classes available on the current Classification Server instance
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SemaphoreConnectionException"></exception>
        public Collection<string> GetTextMiningClasses()
        {
            var csClasses = new Collection<string>();
            var request = (HttpWebRequest)WebRequest.Create(new Uri(_serverUrl + "?operation=LISTRULENETCLASSES"));
            WriteLow("CS Request: " + request.RequestUri, null);
            request.Method = "GET";
            request.Timeout = (_timeout * 1000);

            try
            {
                var oStop = new Stopwatch();
                oStop.Start();
                var oResponse = (HttpWebResponse)request.GetResponse();
                oStop.Stop();

                WriteLow("Response received from CS. Time elapsed {0}:{1}.{2}",
                         oStop.Elapsed.Minutes,
                         oStop.Elapsed.Seconds.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0').PadLeft(2, '0'),
                         oStop.Elapsed.Milliseconds);

                Stream stream = oResponse.GetResponseStream();
                if (stream != null)
                    using (var oReader = new StreamReader(stream))
                    {
                        var xDoc = new XmlDocument();
                        xDoc.LoadXml(oReader.ReadToEnd());
                        XmlNodeList nodeList = xDoc.SelectNodes("//Class");

                        if (nodeList == null || nodeList.Count == 0)
                        {
                            throw new SemaphoreConnectionException(string.Format("No Classes retrieved from {0}",
                                                                                 request.RequestUri.AbsolutePath));
                        }

                        foreach (XmlNode node in nodeList)
                        {
                            if (node.Attributes != null)
                            {
                                string attrib = node.Attributes["Name"].Value;
                                //if (attrib.StartsWith("TEXTMINE_"))
                                csClasses.Add(attrib);
                            }
                        }
                    }
                oResponse.Close();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }


            return csClasses;
        }

        /// <summary>
        ///     Returns the version of Classification Server
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SemaphoreConnectionException"></exception>
        public Version GetVersion()
        {
            var result = new Version();
            var request = (HttpWebRequest)WebRequest.Create(new Uri(_serverUrl + "?operation=version"));
            WriteLow("CS Request: " + request.RequestUri, null);
            request.Method = "GET";
            request.Timeout = (_timeout * 1000);

            try
            {
                var oStop = new Stopwatch();
                oStop.Start();
                var oResponse = (HttpWebResponse)request.GetResponse();
                oStop.Stop();

                WriteLow("Response received from CS. Time elapsed {0}:{1}.{2}",
                         oStop.Elapsed.Minutes,
                         oStop.Elapsed.Seconds.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0').PadLeft(2, '0'),
                         oStop.Elapsed.Milliseconds);

                Stream stream = oResponse.GetResponseStream();
                if (stream != null)
                    using (var oReader = new StreamReader(stream))
                    {
                        var xDoc = new XmlDocument();
                        xDoc.LoadXml(oReader.ReadToEnd());
                        XmlNode top = xDoc.SelectSingleNode("//version");
                        if (top == null) return new Version(0, 0, 0, 0);
                        string versionstring = top.InnerText;
                        //Convert 7.13 {r42590 into 7.13.42590
                        //<version>7.14 {r43753 on Mar 16 2012 11:44:56 using Language Packs}</version> 
                        //Semaphore 3.6 {r47553 on Feb 15 2013 17:55:00} Windows
                        //Semaphore 3.6 - Classification Server r47614 { built on Feb 18 2013 17:12:57 using Language Packs} Windows
                        versionstring = versionstring.Replace("Semaphore ", string.Empty);
                        versionstring = versionstring.Replace(" - Classification Server r", ".");
                        versionstring = versionstring.Replace(" {r", ".");
                        //Truncate at the first space
                        versionstring = versionstring.Substring(0, versionstring.IndexOf(" ", StringComparison.Ordinal));
                        result = new Version(versionstring);
                        if (result.Revision <= 0)
                        {
                            //Make sure the 'revision' is in the correct column (ie. if version is 3.5 r12345, make it 3.5.0.12345 as opposed to 3.5.12345.0)
                            result = new Version(result.Major, result.Minor, 0, result.Build);
                        }
                    }
                oResponse.Close();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }


            return result;
        }

        /// <summary>
        ///     Passes values and binary content to Classification Server for processing
        /// </summary>
        /// <param name="title">The title of the item being classified. This value is optional.</param>
        /// <param name="document">A byte array representing binary content to be classified</param>
        /// <param name="fileName">Where binary content is being classified, a filename is required</param>
        /// <param name="metaValues">A dictionary of values to be classified</param>
        /// <param name="altBody">
        ///     When classifying HTML content, <paramref name="altBody" /> is used to pass the body of the page
        /// </param>
        /// <param name="options">Additional classification options. If no values are passed, server defaults are used</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public ClassificationResult TextMine(string title,
                                             byte[] document,
                                             string fileName,
                                             Dictionary<string, string> metaValues,
                                             string altBody,
                                             ClassificationOptions options)
        {
            if (metaValues == null) throw new ArgumentException("Missing metaValues", "metaValues");
            if (document != null && string.IsNullOrEmpty(fileName)) throw new ArgumentException("Documents must have an associated filename", "fileName");
            if (document == null && !string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Filename parameter should not be set unless an associated document is passed via the document parameter",
                                            "document");

            WriteLow(
                string.Format("Calling TextMining API (Binary). Item Title={0}, FileName={1}",
                              title,
                              fileName),
                null);

            var oFrmFields =
                new Dictionary<string, string>
                    {
                        {"operation", "CLASSIFY"},
                        {"title", title},
                        {"body", altBody},
                        {"operation_mode", "TextMine"}
                    };

            if (options != null)
            {
                if (options.Threshold.HasValue)
                {
                    oFrmFields.Add("threshold", options.Threshold.Value.ToString(CultureInfo.InvariantCulture));
                }
                if (!string.IsNullOrEmpty(options.Type))
                {
                    oFrmFields.Add("type", options.Type);
                }
                if (!string.IsNullOrEmpty(options.ClusteringType))
                {
                    oFrmFields.Add("clustering_type", options.ClusteringType);
                }

                if (options.ClusteringThreshold.HasValue)
                {
                    oFrmFields.Add("clustering_threshold", options.ClusteringThreshold.Value.ToString(CultureInfo.InvariantCulture));
                }

                switch (options.ArticleType)
                {
                    case ArticleType.Default:
                        break;
                    case ArticleType.SingleArticle:
                        oFrmFields.Add("singlearticle", string.Empty);
                        break;
                    case ArticleType.MultiArticle:
                        oFrmFields.Add("multiarticle", string.Empty);
                        break;
                }
            }

            foreach (string sKey in metaValues.Keys)
                oFrmFields.Add("meta_" + sKey, metaValues[sKey]);

            List<FileUpload> oFiles = null;

            if (!string.IsNullOrEmpty(fileName) && document != null)
            {
                oFiles = new List<FileUpload>
                    {
                        new FileUpload
                            {
                                FieldName = "UploadFile",
                                FileName = fileName,
                                Contents = document
                            }
                    };
            }
            return PerformWebClassify(oFrmFields, oFiles);
        }

        public string BoundaryString
        {
            get; set;
        }

        [SuppressMessage("Microsoft.Usage", "CA2202",
            Justification = "Using statement handles multiple Dispose appropriately")]
        private ClassificationResult PerformWebClassify(Dictionary<string, string> formFields, IEnumerable<FileUpload> files)
        {
            string sBoundary = BoundaryString;
            const string newLine = "\r\n";

            WriteLow(string.Format("Classifying data using server Url={0}", _serverUrl), null);

            var oRequest = (HttpWebRequest)WebRequest.Create(_serverUrl);
            oRequest.ContentType = string.Format("multipart/form-data; boundary={0}", sBoundary);
            oRequest.Method = "POST";
            oRequest.Timeout = (_timeout * 1000);

            try
            {
                using (var oMs = new MemoryStream())
                {
                    using (var oSw = new StreamWriter(oMs))
                    {
                        #region Upload form fields

                        if (formFields != null)
                        {
                            foreach (string s in formFields.Keys)
                            {
                                oSw.Write("--{0}{1}", sBoundary, newLine);
                                oSw.Write("Content-Disposition: form-data; name=\"{0}\"{1}{1}{2}{1}",
                                          s,
                                          newLine,
                                          formFields[s]);
                                WriteLow("Added form-data: name={0}, value={1}", s, formFields[s]);
                            }
                        }

                        #endregion

                        #region Upload files

                        if (files != null)
                        {
                            foreach (FileUpload oFile in files)
                            {
                                oSw.Write("--{0}{1}", sBoundary, newLine);
                                oSw.Write("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}",
                                          oFile.FieldName,
                                          oFile.FileName,
                                          newLine);
                                oSw.Write("Content-Type: application/octet-stream{0}{0}", newLine);
                                oSw.Flush();

                                oMs.Write(oFile.Contents, 0, oFile.Contents.Length);
                                oSw.Write(newLine);
                            }
                        }

                        #endregion

                        oSw.Write("--{0}--{1}", sBoundary, newLine);
                        oSw.Flush();

                        oRequest.ContentLength = oMs.Length;

                        WriteLow(string.Format("Sending {0} bytes", oMs.Length), null);

                        using (Stream s = oRequest.GetRequestStream())
                        {
                            oMs.WriteTo(s);
                            s.Close();
                        }
                        oSw.Close();
                    }
                    oMs.Close();
                }

                WriteLow("Requesting the response", null);
                HttpWebResponse oResponse;
                try
                {
                    var oStop = new Stopwatch();
                    oStop.Start();
                    oResponse = (HttpWebResponse)oRequest.GetResponse();
                    oStop.Stop();
                    WriteLow("Response received from CS. Time elapsed M: {0} S: {1} MS: {2}",
                             oStop.Elapsed.Minutes,
                             oStop.Elapsed.Seconds.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                             oStop.Elapsed.Milliseconds);
                }
                catch (WebException oWeX)
                {
                    //Record the error, however this is an expected response from CS when an error has occured,
                    //but this is not 100% true. So we will check the response property of the exception
                    //to determine if an error has actually occured in CS or whether it is a net exception e.g. timeout, invalid file etc
                    //If the response contains an error string, we will throw this as the exception later on
                    //otherwise we will throw the original exception
                    if (oWeX.Status == WebExceptionStatus.Timeout)
                    {
                        WriteError("Timeout Exception occurred. Timeout setting: {0}", _timeout);
                        throw new TimeoutException(string.Format("Call to Classification server timed out after {0} seconds", _timeout), oWeX);
                    }

                    WriteException(oWeX);
                    //throw;
                    oResponse = (HttpWebResponse)oWeX.Response;
                    if (oResponse == null) throw; //Re-throw the original exception (TimeOut)
                }
                WriteLow(
                    string.Format("API Response Code={0}", oResponse.StatusCode),
                    null);

                ClassificationResult oParser = null;
                Stream stream = oResponse.GetResponseStream();

                if (stream != null)
                    using (var oReader = new StreamReader(stream))
                    {
                        oParser = new ClassificationResult(oReader.ReadToEnd());
                    }
                oResponse.Close();
                WriteLow("Closed response", null);
                if (oParser != null)
                {
                    WriteLow("Received response: {0}", oParser.Response.OuterXml);
                }
                return oParser;
            }
            catch (ThreadAbortException oTeX)
            {
                WriteException(oTeX);
                throw;
            }
            catch (WebException oWeX)
            {
                WriteError("Web Exception occurred. Timeout setting: {0}", _timeout);
                WriteException(oWeX);
                throw;
            }
            catch (Exception oX)
            {
                WriteException(oX);
                throw;
            }
        }

        #region Nested type: FileUpload

        /// <summary>
        ///     Holds the information about the file(s) to be uploaded.
        /// </summary>
        internal struct FileUpload
        {
            /// <summary>
            ///     The byte array content to be uploaded.
            /// </summary>
            public byte[] Contents { get; set; }

            /// <summary>
            ///     The HTML form field the file should be uploaded into.
            /// </summary>
            public string FieldName { get; set; }

            /// <summary>
            ///     The name of the file to be uploaded.
            /// </summary>
            public string FileName { get; set; }
        }

        #endregion
    }
}