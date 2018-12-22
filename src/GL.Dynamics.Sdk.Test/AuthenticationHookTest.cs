using GL.Dynamics.Sdk.ApplicationUser.Auth;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Net;

namespace GL.Dynamics.Sdk.Test
{
    [TestClass]
    public class AuthenticationHookTest
    {
        private const string APPSECRET = "";
        private const string ORGURL = "";
        private const string APPLICATIONID = "";
        private const string TENANTID = "";
        private const string APPUSERID = "";

        [TestMethod]
        public void Access_Token_Retrieve_Test()
        {
            var hook = new ApplicationUserAuthHook(APPSECRET, ORGURL, APPLICATIONID, TENANTID);
            var token = hook.GetAuthToken(new Uri("https://dontcare.com"));
            Assert.IsFalse(string.IsNullOrEmpty(token));
        }

        [TestMethod]
        public void Login_As_Application_User_Test()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            CrmServiceClient.AuthOverrideHook = new ApplicationUserAuthHook(APPSECRET, ORGURL, APPLICATIONID, TENANTID);

            var dynamics = new Uri(ORGURL);
            var client = new CrmServiceClient(dynamics, useUniqueInstance: true);

            var req = new WhoAmIRequest();
            var resp = client.Execute(req) as WhoAmIResponse;

            Assert.AreEqual(resp.UserId, Guid.Parse(APPUSERID));
        }
    }
}
