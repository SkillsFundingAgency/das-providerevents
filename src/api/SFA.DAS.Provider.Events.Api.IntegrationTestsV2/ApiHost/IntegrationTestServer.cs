using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Microsoft.Owin.Testing;
using Owin;
using SFA.DAS.Provider.Events.Api.Controllers;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2.ApiHost
{
    public class IntegrationTestServer
    {
        private static IntegrationTestServer _instance;

        public static IntegrationTestServer GetInstance()
        {
            return _instance ?? (_instance = new IntegrationTestServer());
        }

        private TestServer _server;

        public HttpClient Client { get; set; }

        protected IntegrationTestServer()
        {
            StartServer();
            Client = StartClient();
        }

        void StartServer()
        {
            _server = TestServer.Create<Startup>();
        }

        public void Shutdown()
        {
            _server.Dispose();
        }

        HttpClient StartClient()
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

