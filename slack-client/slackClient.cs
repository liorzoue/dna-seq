using Newtonsoft.Json;
using System;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using log4net;
using log4net.Config;
using System.Configuration;

namespace slack_client
{

    //A simple C# class to post messages to a Slack channel
    //Note: This class uses the Newtonsoft Json.NET serializer available via NuGet
    public class SlackClient
    {
        private readonly Uri _uri;
        private readonly Encoding _encoding = new UTF8Encoding();
        private static readonly ILog log = LogManager.GetLogger(typeof(SlackClient));

        [Obsolete("Non recommandé")]
        public SlackClient(string urlWithAccessToken)
        {
            _uri = new Uri(urlWithAccessToken);
        }

        public SlackClient()
        {

            var slackUrl = ConfigurationManager.AppSettings["Slack_Url"];
            var slackApiKey = ConfigurationManager.AppSettings["Slack_ApiKey"];

            _uri = new Uri(slackUrl + slackApiKey);
        }

        //Post a message using simple strings
        public void PostMessage(string text, string username = null, string channel = null)
        {
            Payload payload = new Payload()
            {
                Channel = channel,
                Username = username,
                Text = text
            };

            PostMessage(payload);
        }

        //Post a message using a Payload object
        public void PostMessage(Payload payload)
        {
            try
            {
                string payloadJson = JsonConvert.SerializeObject(payload);

                using (WebClient client = new WebClient())
                {
                    NameValueCollection data = new NameValueCollection();
                    data["payload"] = payloadJson;

                    var response = client.UploadValues(_uri, "POST", data);

                    //The response text is usually "ok"
                    string responseText = _encoding.GetString(response);
                }
            }
            catch (Exception ex)
            {
                log.Info("Erreur Slack", ex);
            }
        }
    }

    //This class serializes into the Json payload required by Slack Incoming WebHooks
    public class Payload
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
