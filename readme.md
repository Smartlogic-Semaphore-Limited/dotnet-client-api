![VSO build status](https://smartlogicvso.visualstudio.com/_apis/public/build/definitions/86c4d2f5-f6a1-49ee-80ea-892ba9863df0/14/badge)

#Smartlogic Semaphore .Net Client Api

###Install from NuGet
Although you can certainly clone this repo and build your own binaries, it's also possible to use a pre-compiled version from NuGet by following these steps:

1. In Visual Studio, from the Tools menu select Library Package Manager and then click Package Manager Console.

2. In the Package Manager Console window that is displayed simply type:

```
install-package Smartlogic.Semaphore.Api
```

and you're done!

###An example Console Application that classifies a document using C# #

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
