using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;

namespace Application.Services.Report
{
    public class PowerBiAuthenticator
    {
        private static OAuthResult _cachedResult;

        private readonly PowerBIOptions _powerBiOptions;

        public PowerBiAuthenticator()
        {
            _powerBiOptions = new PowerBIOptions
            {
                ResourceUrl = "https://analysis.windows.net/powerbi/api",
                AuthorityUrl = "https://login.windows.net/common/oauth2/token",
                ApiUrl = "https://api.powerbi.com/",
                ClientId = "6fab63a6-30ca-475b-aeb3-697894cd0c32",
                ClientSecret = "XbBw0BEnpHbiz8YahRF4+bg1GWp9TQdPMQs9+GL5B7E=",
                Username = "admin@poolingme.onmicrosoft.com",
                Password = "LWE1945ewl!"
            };
        }

        public async Task<OAuthResult> AuthenticateAsync()
        {
            if (_cachedResult != null)
            {
                var expireDateTime = DateTimeOffset.FromUnixTimeSeconds(_cachedResult.ExpiresOn);
                var currentDateTime = DateTimeOffset.Now.UtcDateTime;

                if (currentDateTime < expireDateTime)
                {
                    return _cachedResult;
                }
            }

            OAuthResult authToken = await GetAuthToken();
            _cachedResult = authToken;

            return authToken;
        }

        private async Task<OAuthResult> GetAuthToken() 
        {
            string commonRequestGuid = Guid.NewGuid().ToString();
            OAuthResult oauthResult;

            UserRealm userRealm = await GetUserRealm(commonRequestGuid);
            if (userRealm.account_type.Equals("Federated"))
            {
                XmlDocument metadata
                    = await GetFederationMetadata(commonRequestGuid, userRealm.federation_metadata_url);

                string trustBinding = GetFederatedUserTrustBinding(metadata);
                XmlDocument trustDocument
                    = await GetFederatedUserTrust(commonRequestGuid, trustBinding);

                var userAssertionNodes = trustDocument.GetElementsByTagName("saml:Assertion");
                var userAssertionNode = userAssertionNodes[0].OuterXml;

                using (var client = new HttpClient())
                {
                    string tokenUri = "https://login.windows.net/common/oauth2/token";
                    var ua = new UserAssertion(
                        userAssertionNode,
                        "urn:ietf:params:oauth:grant-type:saml1_1-bearer",
                        Uri.EscapeDataString(_powerBiOptions.Username));

                    UTF8Encoding encoding = new UTF8Encoding();
                    Byte[] byteSource = encoding.GetBytes(ua.Assertion);
                    string base64ua = Convert.ToBase64String(byteSource);

                    var tokenForm = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("resource", _powerBiOptions.ResourceUrl),
                        new KeyValuePair<string, string>("client_id", _powerBiOptions.ClientId),
                        new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:saml1_1-bearer"),
                        new KeyValuePair<string, string>("assertion", base64ua),
                        new KeyValuePair<string, string>("scope", "openid"),
                    });

                    var tokenResult = await client.PostAsync(tokenUri, tokenForm);
                    var tokenContent = await tokenResult.Content.ReadAsStringAsync();

                    oauthResult = JsonConvert.DeserializeObject<OAuthResult>(tokenContent);
                }
            }
            else
            {
                using (var client = new HttpClient())
                {
                    var result = await client.PostAsync(_powerBiOptions.AuthorityUrl, new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("resource", _powerBiOptions.ResourceUrl),
                        new KeyValuePair<string, string>("client_id", _powerBiOptions.ClientId),
                        new KeyValuePair<string, string>("client_secret", _powerBiOptions.ClientSecret),
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("username", _powerBiOptions.Username),
                        new KeyValuePair<string, string>("password", _powerBiOptions.Password),
                        new KeyValuePair<string, string>("scope", "openid"),
                    }));

                    var tokenContent = await result.Content.ReadAsStringAsync();
                    oauthResult = JsonConvert.DeserializeObject<OAuthResult>(tokenContent);
                }
            }
            return oauthResult;
        }

        private async Task<UserRealm> GetUserRealm(string commonRequestGuid)
        {
            UserRealm userRealm = new UserRealm();

            using (var client = new HttpClient())
            {
                string userRealmUri = $"https://login.windows.net/common/UserRealm/{_powerBiOptions.Username}?api-version=1.0";

                HttpRequestMessage realmRequest = new HttpRequestMessage(HttpMethod.Get, userRealmUri);
                realmRequest.Headers.Add("Accept", "application/json");
                realmRequest.Headers.Add("return-client-request-id", "true");
                realmRequest.Headers.Add("client-request-id", commonRequestGuid);

                HttpResponseMessage realmResponse = client.SendAsync(realmRequest).Result;
                string realmString = await realmResponse.Content.ReadAsStringAsync();

                userRealm = JsonConvert.DeserializeObject<UserRealm>(realmString);
            }
            return userRealm;
        }

        private async Task<XmlDocument> GetFederationMetadata(string commonRequestGuid, string adfsMetadataUri) 
        {
            string metadataString = string.Empty;

            using (var client = new HttpClient())
            {
                HttpRequestMessage metadataRequest = new HttpRequestMessage(HttpMethod.Get, adfsMetadataUri);
                metadataRequest.Headers.Add("Accept", "application/json");
                metadataRequest.Headers.Add("return-client-request-id", "true");
                metadataRequest.Headers.Add("client-request-id", commonRequestGuid);

                HttpResponseMessage metadataResponse = client.SendAsync(metadataRequest).Result;
                metadataString = await metadataResponse.Content.ReadAsStringAsync();
            }

            XmlDocument metadataDoc = new XmlDocument();
            metadataDoc.LoadXml(metadataString);

            return metadataDoc;
        }

        private async Task<XmlDocument> GetFederatedUserTrust(string commonRequestGuid, string trustBindingUri)
        {
            string trustString = null;

            using (var client = new HttpClient())
            {
                HttpRequestMessage trustRequest = new HttpRequestMessage(HttpMethod.Post, trustBindingUri);
                trustRequest.Headers.Add("Accept", "application/json");
                trustRequest.Headers.Add("return-client-request-id", "true");
                trustRequest.Headers.Add("client-request-id", commonRequestGuid);
                trustRequest.Headers.Add("SOAPAction", "http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue");

                DateTime now = DateTime.UtcNow;
                DateTime then = now.AddMinutes(10);

                string trustBody = __federatedUserTrustBody.
                    Replace("{messageGuid}", Guid.NewGuid().ToString()).
                    Replace("{UserEndpoint}", trustBindingUri).
                    Replace("{SecurityCreated}", now.ToString("o")).
                    Replace("{SecurityExpires}", then.ToString("o")).
                    Replace("{tokenGuid}", Guid.NewGuid().ToString()).
                    Replace("{username}", _powerBiOptions.Username).
                    Replace("{password}", _powerBiOptions.Password);

                trustRequest.Content = new StringContent(trustBody, Encoding.UTF8, "application/soap+xml");

                HttpResponseMessage userTrustResponse = client.SendAsync(trustRequest).Result;
                trustString = await userTrustResponse.Content.ReadAsStringAsync();
            }

            XmlDocument trustDocument = new XmlDocument();
            trustDocument.LoadXml(trustString);

            return trustDocument;
        }

        private string GetFederatedUserTrustBinding(XmlDocument metadata)
        {
            XmlNodeList services = metadata.GetElementsByTagName("wsdl:service");
            List<XmlNode> ports = new List<XmlNode>();
            foreach (XmlNode node in services[0])
            {
                if (node.Name.Equals("wsdl:port"))
                    ports.Add(node);
            }

            XmlNode trustPort = ports.FirstOrDefault(p => p.Attributes["name"] != null 
                                                          && p.Attributes["name"].Value.Equals("UserNameWSTrustBinding_IWSTrust13Async"));

            XmlNode trustAddress = null;
            foreach (XmlNode node in trustPort.ChildNodes)
            {
                if (node.Name.Equals("soap12:address"))
                {
                    trustAddress = node;
                    break;
                }
            }

            return trustAddress.Attributes["location"].Value;
        }

        public class UserRealm
        {
            public string ver { get; set; }
            public string account_type { get; set; }
            public string domain_name { get; set; }
            public string federation_protocol { get; set; }
            public string federation_metadata_url { get; set; }
            public string federation_active_auth_url { get; set; }
            public string cloud_instance_name { get; set; }
            public string cloud_audience_urn { get; set; }
        }

        public class PowerBIOptions
        {
            [JsonProperty("resourceUrl")]
            public string ResourceUrl { get; set; }
            [JsonProperty("authorityUrl")]
            public string AuthorityUrl { get; set; }
            [JsonProperty("apiUrl")]
            public string ApiUrl { get; set; }
            [JsonProperty("clientId")]
            public string ClientId { get; set; }
            [JsonProperty("client_secret")]
            public string ClientSecret { get; set; }
            [JsonProperty("username")]
            public string Username { get; set; }
            [JsonProperty("password")]
            public string Password { get; set; }
        }

        public class OAuthResult
        {
            [JsonProperty("token_type")]
            public string TokenType { get; set; }
            [JsonProperty("scope")]
            public string Scope { get; set; }
            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }
            [JsonProperty("ext_expires_in")]
            public int ExtExpiresIn { get; set; }
            [JsonProperty("expires_on")]
            public int ExpiresOn { get; set; }
            [JsonProperty("not_before")]
            public int NotBefore { get; set; }
            [JsonProperty("resource")]
            public Uri Resource { get; set; }
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }
        }

        private string __federatedUserTrustBody = 
            @"<s:Envelope xmlns:s='http://www.w3.org/2003/05/soap-envelope' xmlns:a='http://www.w3.org/2005/08/addressing' xmlns:u='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd'>
          <s:Header>
          <a:Action s:mustUnderstand='1'>http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue</a:Action>
          <a:messageID>urn:uuid:{messageGuid}</a:messageID>
          <a:ReplyTo><a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address></a:ReplyTo>
          <a:To s:mustUnderstand='1'>{adfsUserEndpoint}</a:To>
          <o:Security s:mustUnderstand='1' xmlns:o='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'><u:Timestamp u:Id='_0'><u:Created>{SecurityCreated}</u:Created><u:Expires>{SecurityExpires}</u:Expires></u:Timestamp><o:UsernameToken u:Id='uuid-{tokenGuid}'><o:Username>{username}</o:Username><o:Password>{password}</o:Password></o:UsernameToken></o:Security>
          </s:Header>
          <s:Body>
          <trust:RequestSecurityToken xmlns:trust='http://docs.oasis-open.org/ws-sx/ws-trust/200512'>
          <wsp:AppliesTo xmlns:wsp='http://schemas.xmlsoap.org/ws/2004/09/policy'>
          <a:EndpointReference>
          <a:Address>urn:federation:MicrosoftOnline</a:Address>
          </a:EndpointReference>
          </wsp:AppliesTo>
          <trust:KeyType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Bearer</trust:KeyType>
          <trust:RequestType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Issue</trust:RequestType>
          </trust:RequestSecurityToken>
          </s:Body>
          </s:Envelope>";
    }
}