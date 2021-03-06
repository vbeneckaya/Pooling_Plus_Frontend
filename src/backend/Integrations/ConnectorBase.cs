using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DAL.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Integrations
{
    public abstract class ConnectorBase :  IDisposable
    {
        private string _login;
        private string _password;
        protected string _accessToken;
        private string _baseUrl;
        protected readonly ICommonDataService _dataService;

        protected ConnectorBase(string baseUrl, string login, string password, ICommonDataService dataService)
        {
            _login = login;
            _password = password;
            _baseUrl = baseUrl;
            _dataService = dataService;
        }

        protected byte[] DownloadFile(string url)
        {
            return new byte[5];
        }

        protected T Get<T>(string url, object model = null)
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
            
            var response = httpClient.GetAsync(_baseUrl + url).GetAwaiter().GetResult();

            var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            
            var stringReader = new StringReader(responseString);
            return JsonSerializer.Create().Deserialize<T>(new JsonTextReader(stringReader));
            
/*            
            var request = (HttpWebRequest)WebRequest.Create(config.Url + "/api/" + url);

            var json = JsonConvert.SerializeObject(model);

            var data = Encoding.UTF8.GetBytes(json);

            request.Method = method;
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = data.Length;
            request.Headers.Add("Authorization", "Bearer " + config.AccessToken);

            using (WebResponse response = await request.GetResponseAsync())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    var responseString = await new StreamReader(stream).ReadToEndAsync();
                    return JsonSerializer.Create().Deserialize<T>(new JsonTextReader(new StringReader(responseString)));
                }
            }*/
        }        
        
        protected IntegrationAnswer Delete(string url)
        {
            var httpClient = new HttpClient();
            
            if(_accessToken != null)
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
            
            var response = httpClient.DeleteAsync(_baseUrl + url).GetAwaiter().GetResult();

            var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (string.IsNullOrEmpty(responseString))
            {
                return new IntegrationAnswer(JObject.Parse("{}"));
            }
            var jObject = JObject.Parse(responseString);

            return  new IntegrationAnswer(jObject);
        }

        protected IntegrationAnswer Put(string url, object model = null)
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            
            if(_accessToken != null)
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
            
            var response = httpClient.PutAsync(_baseUrl + url, content).GetAwaiter().GetResult();

            var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            try
            {
                var jObject = JObject.Parse(responseString);
                return new IntegrationAnswer(jObject);
            }
            catch (Exception e)
            {
                return null;
            }

        }

        protected IntegrationAnswer Post(string url, object model = null)
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            
            if(_accessToken != null)
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
            
            var response = httpClient.PostAsync(_baseUrl + url, content).GetAwaiter().GetResult();

            var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            JObject jObject;
            try
            {
                jObject = JObject.Parse(responseString);
            }
            catch (Exception e)
            {
                throw new Exception($"ResponseParseError: url: {url}, responseString: {responseString}");
            }

            return  new IntegrationAnswer(jObject);
        }
        
        protected IntegrationAnswerArr PostArr(string url, object model = null)
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            
            if(_accessToken != null)
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
            
            var response = httpClient.PostAsync(_baseUrl + url, content).GetAwaiter().GetResult();

            var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            JArray jObject;
            try
            {
                jObject = JArray.Parse(responseString);
            }
            catch (Exception e)
            {
                throw new Exception($"ResponseParseError: url: {url}, responseString: {responseString}");
            }

            return  new IntegrationAnswerArr(jObject, responseString);
        }

        protected IIntegrationAnswer<string> Get(string url, object model = null)
        {
            var httpClient = new HttpClient();
            
            if(_accessToken != null)
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
            
            var response = httpClient.GetAsync(_baseUrl + url).GetAwaiter().GetResult();

            var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            var jObject = JObject.Parse(responseString);

            return  new IntegrationAnswer(jObject);
        }
        protected IIntegrationAnswer<IEnumerable<JToken>> GetArr(string url, object model = null)
        {
            var httpClient = new HttpClient();
            
            if(_accessToken != null)
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
            
            var response = httpClient.GetAsync(_baseUrl + url).GetAwaiter().GetResult();

            var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            var jArray = JArray.Parse(responseString);
            return  new IntegrationAnswerArr(jArray, responseString);
        }
        
        protected T Post<T>(string url, object model)
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");;

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
            
            var response = httpClient.PostAsync(_baseUrl + url, content).GetAwaiter().GetResult();

            var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            
            var stringReader = new StringReader(responseString);
            return JsonSerializer.Create().Deserialize<T>(new JsonTextReader(stringReader));
        }

        public void Dispose()
        {
        }
    }

    public class IntegrationAnswer : IIntegrationAnswer<string>
    {
        private readonly JObject _jObject;

        public IntegrationAnswer(JObject jObject)
        {
            _jObject = jObject;
        }

        public T Get<T>(string path)
        {
            return _jObject.SelectToken(path).Value<T>();
        }
        public string Get(string path)
        {
            return Get<string>(path);
        }
    }

    public class IntegrationAnswerArr : IIntegrationAnswer<IEnumerable<JToken>>
    {
        private readonly JArray _jArray;
        public readonly string ResponseString;

        public IntegrationAnswerArr(JArray jArray, string responseString)
        {
            _jArray = jArray;
            ResponseString = responseString;
        }

        public IEnumerable<JToken> Get(string path)
        {
            return _jArray.SelectTokens(path);
        }
    }

    public interface IIntegrationAnswer<T>
    {
        T Get(string path);
    }
}