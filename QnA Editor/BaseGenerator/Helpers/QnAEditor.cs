using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BaseGenerator.Helpers
{
    public class QnAEditor
    {
        static string knowledgeBaseEndpoint = string.Empty;
        static string service = "/qnamaker/v4.0";
        static string method = "/knowledgebases/";
        static string knowledgeBaseKey = string.Empty;
        static string knowledgeBaseName = string.Empty;

        public struct Response
        {
            public HttpResponseHeaders headers;
            public string response;

            public Response(HttpResponseHeaders headers, string response)
            {
                this.headers = headers;
                this.response = response;
            }
        }

        public static void AssignCredentials(string qnaEndpoint, string knowledgeBase, string authorizationToken)
        {
            knowledgeBaseEndpoint = qnaEndpoint;
            knowledgeBaseName = knowledgeBase;
            knowledgeBaseKey = authorizationToken;
        }  

        public static void AddNewAnswer(string answer, string question)
        {
            string new_kb = String.Format(@"{{
  'add': {{
    'qnaList': [
      {{
        'id': 1,
        'answer': '{1}',
        'source': 'My second editor app',
        'questions': [
          '{0}'
        ],
        'metadata': []
      }}
    ],
    'urls': [
    ]
  }},
  'update' : {{
    'name' : 'Generic QnA'
  }}
}}
", question, answer);

            UpdateKB(knowledgeBaseName, new_kb);
        }

        static string PrettyPrint(string s)
        {
            return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(s), Formatting.Indented);
        }

        async static Task<Response> Patch(string uri, string body)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("PATCH");
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", knowledgeBaseKey);

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                return new Response(response.Headers, responseBody);
            }
        }

        async static Task<Response> Get(string uri)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(uri);
                request.Headers.Add("Ocp-Apim-Subscription-Key", knowledgeBaseKey);

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                return new Response(response.Headers, responseBody);
            }
        }

        async static Task<Response> PostUpdateKB(string kb, string new_kb)
        {
            string uri = knowledgeBaseEndpoint + service + method + kb;
            return await Patch(uri, new_kb);
        }

        async static Task<Response> GetStatus(string operation)
        {
            string uri = knowledgeBaseEndpoint + service + operation;
            return await Get(uri);
        }

        public async static void UpdateKB(string kb, string new_kb)
        {
            var response = await PostUpdateKB(kb, new_kb);
            var operation = response.headers.GetValues("Location").First();
            
            var done = false;
            while (true != done)
            {
                response = await GetStatus(operation);

                var fields = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.response);

                String state = fields["operationState"];
                if (state.CompareTo("Running") == 0 || state.CompareTo("NotStarted") == 0)
                {
                    var wait = response.headers.GetValues("Retry-After").First();
                }
                else
                {
                    done = true;
                }
            }
        }
    }
}
