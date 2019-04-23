using Newtonsoft.Json;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Configuration;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string appKey = appSettings["AppKey"];
                var responseFromSafeBrowsing = ConsumeSafeBrowsingAsync(appKey);
                JObject responseObject = JObject.Parse(responseFromSafeBrowsing.Result);
                if(responseObject.ContainsKey("matches"))
                {
                    JArray matches = (JArray)responseObject["matches"];
                    foreach (var item in matches)
                    {
                        string threatType = item["threatType"].ToString();
                        JObject threat = (JObject)item["threat"];
                        string url = threat["url"].ToString();
                        Console.WriteLine($"{url} belongs to {threatType.ToLower()}.");
                    }
                }
                else if(responseObject.Count == 0)
                {
                    Console.WriteLine("Every url is secure according to SafeBrowsing.");
                }
                else
                {
                    Console.WriteLine("Look at fiddler.");
                }
                Console.ReadLine();
            }
            catch (ConfigurationErrorsException err)
            {
                Console.WriteLine(err.Message);
            }
        }

        static async Task<string> ConsumeSafeBrowsingAsync(string api_key)
        {
            using (var httpclient = new HttpClient())
            {
                try
                {
                    JArray threatEntries = new JArray();
                    List<string> listOfUrls = new List<string>
                    {
                        "http://222.186.3.73",
                        "http://storage.duapp.com/",
                        "http://api.ctkj.org/",
                        "http://white.fjxmxslaw.com",
                        "http://wallet.ctkj.org/"
                    };
                    foreach (var item in listOfUrls)
                    {
                        JObject tempUrl = new JObject();
                        tempUrl["url"] = item;
                        threatEntries.Add(tempUrl);
                    }
                    JArray threatTypes = new JArray();
                    threatTypes.Add("MALWARE");
                    threatTypes.Add("SOCIAL_ENGINEERING");
                    JArray platformTypes = new JArray();
                    platformTypes.Add("WINDOWS");
                    JArray threatEntryTypes = new JArray();
                    threatEntryTypes.Add("URL");

                    JObject threatInfoObject = new JObject();
                    threatInfoObject["threatTypes"] = threatTypes;
                    threatInfoObject["platformTypes"] = platformTypes;
                    threatInfoObject["threatEntryTypes"] = threatEntryTypes;
                    threatInfoObject["threatEntries"] = threatEntries;

                    JObject clientObject = new JObject();
                    clientObject["clientId"] = "Student";
                    clientObject["clientVersion"] = "0.0.1";

                    JObject requestObject = new JObject();
                    requestObject["client"] = clientObject;
                    requestObject["threatInfo"] = threatInfoObject;

                    string requestBody = JsonConvert.SerializeObject(requestObject);

                    string requestUrl = $"https://safebrowsing.googleapis.com/v4/threatMatches:find?key={api_key}";

                    HttpResponseMessage responseTask = await httpclient.PostAsync(requestUrl, new StringContent(requestBody));
                    string responseContent = await responseTask.Content.ReadAsStringAsync();
                    return responseContent;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return "";
                }
            }
        }
    }
}
