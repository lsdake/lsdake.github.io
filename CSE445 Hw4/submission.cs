using System;
using System.Xml.Schema;
using System.Xml;
using Newtonsoft.Json;
using System.IO;



/**
 * This template file is created for ASU CSE445 Distributed SW Dev Assignment 4.
 * Please do not modify or delete any existing class/variable/method names. However, you can add more variables and functions.
 * Uploading this file directly will not pass the autograder's compilation check, resulting in a grade of 0.
 * **/


namespace ConsoleApp1
{


    public class Program
    {
        public static string xmlURL = "https://lsdake.github.io/CSE445%20Hw4/Hotels.xml";
        public static string xmlErrorURL = "Your Error XML URL";
        public static string xsdURL = "https://lsdake.github.io/CSE445%20Hw4/Hotels.xsd";

         public static void Main(string[] args)
        {
            // (1) Validate correct XML
            string result = Verification(xmlURL, xsdURL);
            if (result == "No Error")
                Console.WriteLine("No errors are found.");
            else
                Console.WriteLine(result);

            // (2) Validate erroneous XML
            result = Verification(xmlErrorURL, xsdURL);
            if (result == "No Error")
                Console.WriteLine("No errors are found.");
            else
                Console.WriteLine(result);

            // (3) Convert valid XML to JSON
            string json = Xml2Json(xmlURL);
            Console.WriteLine(json);
        }

        // Q2.1
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            string validationError = null;

            // Configure schema settings
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas.Add(null, xsdUrl);
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += (sender, e) =>
            {
                if (validationError == null)
                {
                    var ex = e.Exception;
                    validationError = $"{e.Severity}: {e.Message} (Line {ex.LineNumber}, Position {ex.LinePosition})";
                }
            };

            try
            {
                using (XmlReader reader = XmlReader.Create(xmlUrl, settings))
                {
                    while (reader.Read()) { /* Just parse through */ }
                }
            }
            catch (Exception ex)
            {
                // Capture errors such as URL not found or malformed XML
                return ex.Message;
            }

            return validationError ?? "No Error";
        }

        
        // Q2.2: Convert XML to JSON string
        
        // Q2.2: Convert XML to JSON string
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                // Load XML
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlUrl);

                // Convert XML to JSON with attribute prefix '_' and arrays for repeating elements
                var settings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter>
                    {
                        new XmlNodeConverter
                        {
                            // prefix attributes with underscore
                            AttributePrefix = "_",
                            // ignore duplicate element name handling to form arrays automatically
                            IsArray = (name, node) => node.ParentNode.SelectNodes(name).Count > 1
                        }
                    },
                    Formatting = Formatting.Indented,
                    // do not include the XML declaration
                    DateParseHandling = DateParseHandling.None
                };

                // Serialize with root object preserved
                string jsonText = JsonConvert.SerializeObject(doc.DocumentElement, settings);
                return jsonText;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
