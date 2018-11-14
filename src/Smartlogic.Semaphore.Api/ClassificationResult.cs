using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Smartlogic.Semaphore.Api
{
    public class ClassificationResult
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly XmlDocument _results;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClassificationResult" /> class.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <remarks></remarks>
        public ClassificationResult(string results)
        {
            _results = new XmlDocument();
            _results.LoadXml(results);
        }

        /// <summary>
        ///     Gets the response.
        /// </summary>
        /// <remarks></remarks>
        public XmlDocument Response => _results;

        /// <summary>
        ///     Returns a list of classifications for all classes
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ClassificationException"></exception>
        /// <remarks></remarks>
        public IEnumerable<ClassificationItem> GetClassifications()
        {
            var results = new List<ClassificationItem>();

            var oIDs = _results.SelectNodes(".//STRUCTUREDDOCUMENT/META[@score]");
            if (oIDs != null && oIDs.Count > 0)
            {
                for (var c = 0; c < oIDs.Count; c++)
                {
                    var element = (XmlElement) oIDs[c];
                    var value = element.GetAttribute("value");
                    var classname = element.GetAttribute("name");
                    var score = float.Parse(element.GetAttribute("score"), CultureInfo.InvariantCulture.NumberFormat);

                    if (element.HasAttribute("id"))
                    {
                        var id = element.GetAttribute("id");
                        results.Add(new ClassificationItem(classname, value, score, id));
                    }
                    else
                    {
                        results.Add(new ClassificationItem(classname, value, score));
                    }
                }
            }

            return results.OrderByDescending(r => r.Score);
        }

        /// <summary>
        ///     Returns a list of nesting classifications for all classes
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ClassificationException"></exception>
        /// <remarks></remarks>
        public IEnumerable<MetaNode> GetMetaNodes()
        {
            var oResults = new List<MetaNode>();

            var oIDs = _results.SelectNodes(".//STRUCTUREDDOCUMENT/META[@score]");
            if (oIDs != null && oIDs.Count > 0)
            {
                for (var c = 0; c < oIDs.Count; c++)
                {
                    var element = (XmlElement)oIDs[c];
                    var value = element.GetAttribute("value");
                    var classname = element.GetAttribute("name");
                    var score = float.Parse(element.GetAttribute("score"), CultureInfo.InvariantCulture.NumberFormat);

                    if (element.HasAttribute("id"))
                    {
                        var id = element.GetAttribute("id");
                        oResults.Add(new MetaNode(classname, value, score, id, element));
                    }
                    else
                    {
                        oResults.Add(new MetaNode(classname, value, score, element));
                    }
                }
            }
            else
            {
                // else do we have a response? If no, log an error
                if (_results.DocumentElement != null)
                {
                    var oResponse = _results.DocumentElement.SelectSingleNode("STRUCTUREDDOCUMENT");
                    var oError = _results.DocumentElement.SelectSingleNode("error");

                    if ((oResponse == null) && (oError == null))
                    {
                        throw new ClassificationException("Get Classifications: No Response received");
                    }

                    // else if check and log if there is an error.
                    if (oError?.Attributes != null)
                        throw new ClassificationException(oError.Attributes["id"].Value + " : " + oError.InnerText);
                }
            }
            return oResults.OrderByDescending(r => r.Score);
        }


        /// <summary>
        ///     Returns a list of classifications for a particular class
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ClassificationException"></exception>
        /// <remarks></remarks>
        public IEnumerable<ClassificationItem> GetClassifications(string className)
        {
            if (string.IsNullOrEmpty(className)) return GetClassifications();

            var oResults = new List<ClassificationItem>();

            var oIDs =
                _results.SelectNodes($".//STRUCTUREDDOCUMENT/META[@name='{className.Trim()}' and @score]");

            // If we have results we can use, return them
            if (oIDs != null && oIDs.Count > 0)
            {
                for (var c = 0; c < oIDs.Count; c++)
                {
                    var element = (XmlElement) oIDs[c];
                    var value = element.GetAttribute("value");
                    var classname = element.GetAttribute("name");
                    var score = float.Parse(element.GetAttribute("score"), CultureInfo.InvariantCulture.NumberFormat);

                    if (element.HasAttribute("id"))
                    {
                        var id = element.GetAttribute("id");
                        var item = new ClassificationItem(classname, value, score, id);
                        if (!oResults.Contains(item)) oResults.Add(item);
                    }
                    else
                    {
                        var item = new ClassificationItem(classname, value, score);
                        if (!oResults.Contains(item)) oResults.Add(item);
                    }
                }
            }
            else
            {
                // else do we have a response? If no, log an error
                if (_results.DocumentElement != null)
                {
                    var oResponse = _results.DocumentElement.SelectSingleNode("STRUCTUREDDOCUMENT");
                    var oError = _results.DocumentElement.SelectSingleNode("error");

                    if ((oResponse == null) && (oError == null))
                    {
                        throw new ClassificationException("Get Classifications: No Response received");
                    }

                    // else if check and log if there is an error.
                    if (oError?.Attributes != null)
                        throw new ClassificationException(oError.Attributes["id"].Value + " : " + oError.InnerText);
                }
            }
            return oResults.OrderByDescending(r => r.Score);
        }
    }
}