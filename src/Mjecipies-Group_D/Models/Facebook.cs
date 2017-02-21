using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Mjecipies_Group_D.Models
{
    public class Facebook
    {
        public string app_id { get; set; }
        public string application { get; set; }
        public string expires_at { get; set; }
        public string is_valid { get; set; }
        public string issued_at { get; set; }
        public string[] scopes { get; set; }
        public string user_id { get; set; }

        public async Task<FacebookResponse> GetFacebookResponse(string token)
        {
            long appID = 1892694070952149;
            string secret = "642902d50e743e4b60f52226b0c59f81";
            string url = "https://graph.facebook.com/debug_token?access_token=" + appID + "|" + secret + "&input_token=" + token;
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string data = reader.ReadToEnd();
                FacebookResponse fbres = JsonConvert.DeserializeObject<FacebookResponse>(data);

                return fbres;
            }

            return new FacebookResponse();
        }
    }

    public class FacebookResponse
    {
        public Facebook data;
    }
}
