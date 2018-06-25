﻿using Newtonsoft.Json;
using RestSharp;
using RuiJi.Net.Core;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Extensions;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net;
using RuiJi.Net.Node.Extractor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using RuiJi.Net.NodeVisitor;

namespace RuiJi.Net.Owin.Controllers
{
    public class ExtractorProxyApiController : ApiController
    {
        [HttpPost]
        //[WebApiCacheAttribute(Duration = 10)]
        public List<ExtractResult> Extract([FromBody]string json)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            if (node.NodeType == Node.NodeTypeEnum.ExtractorPROXY)
            {

                var result = ExtractorManager.Instance.Elect();
                if (result == null)
                    return new List<ExtractResult>();

                var client = new RestClient("http://" + result.BaseUrl);
                var restRequest = new RestRequest("api/extract");
                restRequest.Method = Method.POST;
                restRequest.JsonSerializer = new NewtonJsonSerializer();
                restRequest.AddJsonBody(json);
                restRequest.Timeout = 15000;

                var restResponse = client.Execute(restRequest);

                var response = JsonConvert.DeserializeObject<List<ExtractResult>>(restResponse.Content);

                return response;
            }
            else
            {
                var request = JsonConvert.DeserializeObject<ExtractRequest>(json);
                return Extractor.Extract(request);
            }
        }
    }
}
