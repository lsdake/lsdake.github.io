using System;
using System.Xml.Schema;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Runtime;



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
        public static string xmlErrorURL = "https://lsdake.github.io/CSE445%20Hw4/HotelsErrors.xml";
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

        // Q2.2: Convert XML to JSON string with desired structure
        // Q2.2: Convert XML to JSON string with desired structure
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(xmlUrl);
                var root = doc.DocumentElement;  // <Hotels>

                var hotelArray = new Newtonsoft.Json.Linq.JArray();
                foreach (XmlNode hotelNode in root.SelectNodes("Hotel"))
                {
                    var jHotel = new Newtonsoft.Json.Linq.JObject();
                    // Name
                    jHotel["Name"] = hotelNode.SelectSingleNode("Name").InnerText;
                    // Phone array
                    var phones = new Newtonsoft.Json.Linq.JArray();
                    foreach (XmlNode phone in hotelNode.SelectNodes("Phone"))
                        phones.Add(phone.InnerText);
                    jHotel["Phone"] = phones;
                    // Address object
                    var addrNode = hotelNode.SelectSingleNode("Address");
                    var jAddr = new Newtonsoft.Json.Linq.JObject();
                    jAddr["Number"] = addrNode.SelectSingleNode("Number").InnerText;
                    jAddr["Street"] = addrNode.SelectSingleNode("Street").InnerText;
                    jAddr["City"] = addrNode.SelectSingleNode("City").InnerText;
                    jAddr["State"] = addrNode.SelectSingleNode("State").InnerText;
                    jAddr["Zip"] = addrNode.SelectSingleNode("Zip").InnerText;
                    // NearestAirport attribute as _NearestAirport
                    var attr = ((XmlElement)addrNode).GetAttribute("NearestAirport");
                    jAddr["_NearestAirport"] = attr;
                    jHotel["Address"] = jAddr;
                    // Optional Rating attribute
                    var ratingAttr = ((XmlElement)hotelNode).GetAttribute("Rating");
                    if (!string.IsNullOrEmpty(ratingAttr))
                        jHotel["_Rating"] = ratingAttr;

                    hotelArray.Add(jHotel);
                }

                // Assemble final structure
                var rootObj = new Newtonsoft.Json.Linq.JObject(
                    new Newtonsoft.Json.Linq.JProperty("Hotels",
                        new Newtonsoft.Json.Linq.JObject(
                            new Newtonsoft.Json.Linq.JProperty("Hotel", hotelArray)
                        )
                    )
                );
                return rootObj.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
