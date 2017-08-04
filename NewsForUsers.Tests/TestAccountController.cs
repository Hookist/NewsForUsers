using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewsForUsers.Controllers;
using NewsForUsers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace NewsForUsers.Tests
{
    [TestClass]
    public class TestAccountController
    {
        [TestMethod]
        public void Registr()
        {
            AccountController accountController = new AccountController();
            accountController.Request = new HttpRequestMessage();
            accountController.Configuration = new HttpConfiguration();

            var responce = accountController.Register(new UserModel() { UserName = "SuperGamer5", Password = "qwerty1", ConfirmPassword = "qwerty" }).Result;
            Assert.AreEqual(new OkResult(accountController).GetType(), responce.GetType());
        }

        [TestMethod]
        public void GetToken()
        {
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("grant_type", "password"));
            nvc.Add(new KeyValuePair<string, string>("username", "SuperGamer5"));
            nvc.Add(new KeyValuePair<string, string>("password", "qwerty1"));

            AccountController accountController = new AccountController();
            accountController.Request = new HttpRequestMessage() { Content = new FormUrlEncodedContent(nvc) };
            accountController.Configuration = new HttpConfiguration();

            var responce = accountController.GetToken();
            Assert.AreEqual(new OkResult(accountController).GetType(), responce.GetType());

        }
    }
}
