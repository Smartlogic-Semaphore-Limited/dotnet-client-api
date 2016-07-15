![VSO build status](https://smartlogicvso.visualstudio.com/_apis/public/build/definitions/86c4d2f5-f6a1-49ee-80ea-892ba9863df0/14/badge)

Smartlogic Semaphore Client Api for use with Microsoft .Net Framework applications

An example Console Application that classifies a document using C#

```
using System;
using System.Collections.Generic;
using System.IO;
using Smartlogic.Semaphore.Api;

namespace APIDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var client = new ClassificationServer(new Uri("http://your-classification-server-url"));

            var doc = File.ReadAllBytes(@"C:\path\to\somedocument.pdf");

            var options = new ClassificationOptions
            {
                Threshold = 10
            };

            var metadata = new Dictionary<string, string>();

            var result = client.Classify("My File", doc, "filename.pdf", metadata, "", options);

            var classifications = result.GetClassifications();

            Console.WriteLine("Results:-");
            foreach (var item in classifications)
            {
                Console.WriteLine($"{item.ClassName}    {item.Id}   {item.Score}    {item.Value}");
            }
            Console.ReadLine();
        }
    }
}

```
