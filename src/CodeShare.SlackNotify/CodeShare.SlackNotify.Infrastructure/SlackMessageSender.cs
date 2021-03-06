﻿using CodeShare.SlaclNotify.Core.Interfaces;
using CodeShare.SlaclNotify.Core.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CodeShare.SlackNotify.Infrastructure
{
    public class SlackMessageSender : ISlackMessageSender
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public SlackMessageSender(IConfiguration config, 
            HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        public class SendMessageModel
        {
            public string text { get; set; }
        }

        public async Task<Response<string>> SendMessageAsync(SendMessageRequest model)
        {
            var url = "";
            if (model.Type == SlaclNotify.Core.Constants.SLACK_TYPE_ERROR)
                url = _config["slack:error:channelurl"];
            else
                url = _config["slack:message:channelurl"];
             
            var jsonBody = JsonConvert.SerializeObject(new SendMessageModel
            {
                text = model.Message
            });

            //return await SendViaHttpClientAsync(jsonBody, url);

            //send using RestSharp. this works
         
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await Task.FromResult(new Response<string>
                {
                    Success = true,
                    Data = response.Content,
                    Message = "Request Successful"
                });
            }

            return await Task.FromResult(new Response<string>
            {
                Message = response.Content
            });
           

        }

        public async Task<Response<string>> SendMessageHttpClientAsync(SendMessageRequest model)
        {
            var url = "";
            if (model.Type == SlaclNotify.Core.Constants.SLACK_TYPE_ERROR)
                url = _config["slack:error:channelurl"];
            else
                url = _config["slack:message:channelurl"];

            var jsonBody = JsonConvert.SerializeObject(new SendMessageModel
            {
                text = model.Message
            });

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var resultContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await Task.FromResult(new Response<string>
                {
                    Success = true,
                    Data = resultContent,
                    Message = "Request Successful"
                });
            }

            return await Task.FromResult(new Response<string>
            {
                Message = resultContent
            });

        }

        public async Task<Response<string>> SendViaHttpClientAsync(string jsonBody, string url)
        {
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var resultContent = await response.Content.ReadAsStringAsync();
             
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await Task.FromResult(new Response<string>
                {
                    Success = true,
                    Data = resultContent,
                    Message = "Request Successful"
                });
            }

            return await Task.FromResult(new Response<string>
            {
                Message = resultContent
            });
        }
    }
}
