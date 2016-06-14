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
        private Uri _uri;
        private readonly Encoding _encoding = new UTF8Encoding();
        private static readonly ILog log = LogManager.GetLogger(typeof(SlackClient));
        private string _channel;
        private string _userName;

        [Obsolete("Non recommandé")]
        public SlackClient(string urlWithAccessToken)
        {
            this.Initialize(urlWithAccessToken);
        }

        public SlackClient()
        {

            var slackUrl = ConfigurationManager.AppSettings["Slack_Url"];
            var slackApiKey = ConfigurationManager.AppSettings["Slack_ApiKey"];

            this.Initialize(slackUrl + slackApiKey);
        }

        private void Initialize(string slackUrl)
        {
            _uri = new Uri(slackUrl);

            _channel = ConfigurationManager.AppSettings["Slack_Channel"];
            if (string.IsNullOrEmpty(_channel)) _channel = "#dev";

            _userName = ConfigurationManager.AppSettings["Slack_UserName"];
            if (string.IsNullOrEmpty(_channel)) _channel = "Mr. Cicharpe";

        }

        //Post a message using simple strings
        public void PostMessage(string text, string username = null, string channel = null)
        {
            if (string.IsNullOrEmpty(channel)) channel = _channel;
            if (string.IsNullOrEmpty(username)) username = _userName;

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
