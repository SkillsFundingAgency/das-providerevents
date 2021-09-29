using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Microsoft.Owin.Testing;
using Owin;
using SFA.DAS.Provider.Events.Api.Controllers;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.ApiHost
{
    internal class IntegrationTestServer 
    {
        private static readonly IntegrationTestServer Instance = new IntegrationTestServer();

        private HttpClient TestClient { get; }

        public static HttpClient Client => Instance.TestClient;
        private static TestServer _server;

        protected IntegrationTestServer()
        {
            StartServer();
            TestClient = StartClient();
        }

        private void StartServer()
        {
            _server = TestServer.Create<Startup>();
        }

        public static void Shutdown()
        {
            _server.Dispose();
        }

        private HttpClient StartClient()
        {
            var client = new HttpClient(_server.Handler)
            {
                BaseAddress = new Uri("http://localhost/"),
            };
            return client;
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);
           
            config.Services.Replace(typeof(IAssembliesResolver), new TestWebApiResolver());

            appBuilder.UseWebApi(config);
        }
    }

    public class TestWebApiResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            return new List<Assembly> { typeof(PaymentsController).Assembly };
        }
    }
}

