using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read a file by stream
            StreamReader streamReader1 = new StreamReader("../../JsonFileDiretory/JsonFile1.txt");
            string line1 = streamReader1.ReadLine();
            string jsonData1 = null;
            while(line1 != null)
            {
                jsonData1 += line1;
                line1 = streamReader1.ReadLine();
            }
            JObject jObject1 = new JObject();
            jObject1 = JObject.Parse(jsonData1);
            if(jObject1.ContainsKey("matches"))
            {
                try
                {
                    JArray matches = (JArray)jObject1["matches"];
                    foreach (var item in matches)
                    {
                        string threatType = item["threatType"].ToString();
                        JObject threat = (JObject)item["threat"];
                        string url = threat["url"].ToString();
                        Console.WriteLine($"{url} belongs to {threatType.ToLower()}.");
                    }
                }
                catch(Exception err)
                {
                    Console.WriteLine(err.Message);
                }
            }
        }
    }
}
