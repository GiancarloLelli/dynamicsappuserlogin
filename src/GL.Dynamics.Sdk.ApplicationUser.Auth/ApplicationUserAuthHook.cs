using Microsoft.Xrm.Tooling.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace GL.Dynamics.Sdk.ApplicationUser.Auth
{
    public class ApplicationUserAuthHook : IOverrideAuthHookWrapper
    {
        private readonly string m_secret;
        private readonly string m_resource;
        private readonly string m_application;
        private readonly string m_tenant;

        public ApplicationUserAuthHook(string secret, string resource, string application, string tenantId)
        {
            m_secret = secret;
            m_resource = resource;
            m_application = application;
            m_tenant = tenantId;
        }

        public string GetAuthToken(Uri connectedUri)
        {
            var token = string.Empty;

            using (var client = new HttpClient())
            {
                var uri = new Uri(BuildAuthUri());
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
                var httpResult = client.PostAsync(uri, new FormUrlEncodedContent(BuildAuthBody())).GetAwaiter().GetResult();
                if (httpResult.IsSuccessStatusCode)
                {
                    var stringResult = httpResult.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var jObject = JObject.Parse(stringResult);
                    token = jObject["access_token"].ToString();
                }
            }

            return token;
        }

        private string BuildAuthUri() => $"https://login.windows.net/{m_tenant}/oauth2/token/";

        private IEnumerable<KeyValuePair<string, string>> BuildAuthBody()
        {
            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("client_id", m_application),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("resource", m_resource),
                new KeyValuePair<string, string>("client_secret", m_secret)
            };
        }
    }
}
