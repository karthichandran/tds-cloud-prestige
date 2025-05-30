using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;

namespace ReProServices.Infrastructure.OneDrive
{
    public class OneDriveService
    {
        private readonly HttpClient _httpClient;
        // ReproDrive
        //const string clientId = "99296f10-2f81-4f02-9741-3b736ceb615c";
        //const string clientSecret = "xrl8Q~oVXvKc0yk_Xi2x_zvjNV6Hu5FXAZd6Abkl";
        //const string clientSecretID = "620ed7c7-aaa9-429f-bf8d-9efe4bb77c7a";
        // const string tenantId = "f8cdef31-a31e-4b4a-93e4-5f571e91255a";
        string tenantId = "common";
        //string[] scopes = new string[] { "User.Read", "Files.ReadWrite" };
        string[] scopes = new string[] { "Files.ReadWrite offline_access" };

        private static string refreshToken = "YOUR_REFRESH_TOKEN";
        private static string tokenEndpoint = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
        string clientId = "4c43ba9b-ef5f-41af-be9b-1f2c005f8456";
        //const string clientSecret = "zGY8Q~AKIMV~fetvLMYuVueAQI2bnl1c3TUT2cUq";
        //const string clientSecretID = "f39df767-a597-4d0b-9fb0-c9f4c23ac08e";
        //string tenantId = "f8cdef31-a31e-4b4a-93e4-5f571e91255a";
        public OneDriveService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAccessToken()
        {
            using (HttpClient client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("refresh_token", refreshToken),
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("scope", "Files.ReadWrite offline_access"),
                    new KeyValuePair<string, string>("redirect_uri", "http://localhost/44301") // Must match app registration
                });

                var response = await client.PostAsync(tokenEndpoint, content);
                var responseString = await response.Content.ReadAsStringAsync();
                var json = JsonSerializer.Deserialize<JsonElement>(responseString);

                return json.GetProperty("access_token").GetString();
            }


            //clientId = "113910d4-4f4c-4aea-bf68-faddc3692308";
            //var clientSecret = "B8-8Q~fyuoMfENjTNCkXBPPyrHfyZPqTroD23a8U";
            //var app = PublicClientApplicationBuilder.Create(clientId)
            //    .WithRedirectUri("http://localhost")
            //    .Build();
            //var result = await app.AcquireTokenWithDeviceCode(new[] { "Files.ReadWrite" },
            //    deviceCodeCallback => {
            //        Console.WriteLine($"Go to {deviceCodeCallback.VerificationUrl} and enter: {deviceCodeCallback.UserCode}");
            //        return Task.CompletedTask;
            //    }).ExecuteAsync();
            //return result.AccessToken;
            //var app = PublicClientApplicationBuilder.Create(clientId)
            //    .WithRedirectUri("http://localhost")
            //    .Build();

            //var authResult = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
            //var accessToken = authResult.AccessToken;
            //return accessToken;

            //var app = ConfidentialClientApplicationBuilder.Create(clientId)
            //    .WithClientSecret(clientSecret)
            //    .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
            //    .Build();

            //var result = await app.AcquireTokenForClient(new[] { "https://graph.microsoft.com/.default" })
            //    .ExecuteAsync();

            //return result.AccessToken;

            //var app = PublicClientApplicationBuilder.Create(clientId)

            //    .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
            //    .WithRedirectUri("http://localhost")
            //    .Build();

            //var result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();

            //return result.AccessToken;


            //========
            //using (var client = new HttpClient())
            //{
            //    var tokenEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

            //    var requestBody = new Dictionary<string, string>
            //    {
            //        { "client_id", clientId },
            //        { "client_secret", clientSecret },
            //        { "grant_type", "client_credentials" },
            //        { "scope", "https://graph.microsoft.com/.default" }
            //    };

            //    var response = await client.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestBody));
            //    var responseContent = await response.Content.ReadAsStringAsync();

            //    if (response.IsSuccessStatusCode)
            //    {
            //        using (JsonDocument document = JsonDocument.Parse(responseContent))
            //        {
            //            JsonElement root = document.RootElement;
            //            string accessToken = root.GetProperty("access_token").GetString();
            //            return accessToken;
            //        }
            //    }
            //    else
            //    {
            //        throw new Exception($"Failed to get token: {responseContent}");
            //    }
            //}

            //==============

            //var request = new HttpRequestMessage(HttpMethod.Post, $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token");

            //request.Content = new FormUrlEncodedContent(new[]
            //{
            //    new KeyValuePair<string, string>("client_id", clientId),
            //    new KeyValuePair<string, string>("client_secret", clientSecret),
            //    new KeyValuePair<string, string>("scope", "https://graph.microsoft.com/.default"),
            //    new KeyValuePair<string, string>("grant_type", "client_credentials")
            //});

            //request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            //var response = await _httpClient.SendAsync(request);

            //response.EnsureSuccessStatusCode();

            //var responseContent = await response.Content.ReadAsStringAsync();

            //using (JsonDocument document = JsonDocument.Parse(responseContent))
            //{
            //    JsonElement root = document.RootElement;
            //    string accessToken = root.GetProperty("access_token").GetString();
            //    return accessToken;
            //}
        }

        public async Task<string> UploadFileAsync( MemoryStream ms, string fileName)
        {
            var token =await GetAccessToken();
            return await UploadFileAsync(token, ms, fileName);
        }

        private async Task<string> UploadFileAsync(string accessToken, MemoryStream ms, string fileName)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"https://graph.microsoft.com/v1.0/me/drive/root:/{fileName}:/content");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            ms.Position = 0; // Reset the position of the MemoryStream
            request.Content = new StreamContent(ms);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                using (JsonDocument document = JsonDocument.Parse(responseContent))
                {
                    JsonElement root = document.RootElement;
                    string fileId = root.GetProperty("id").GetString();
                    return fileId;
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                // Log or handle the error content as needed
                throw new Exception($"File upload failed with status code {response.StatusCode}: {errorContent}");
            }
            return "";
        }

        public async Task<MemoryStream> DownloadFileAsync( string fileName)
        {
            var token = await GetAccessToken();
            return await DownloadFileAsync(token, fileName);
        }
        public async Task<MemoryStream> DownloadFileAsByIdAsync(string fileId)
        {
            var token = await GetAccessToken();
            return await DownloadFileByIdAsync(token, fileId);
        }

        public async Task<MemoryStream> DownloadFileAsync(string accessToken, string fileName)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://graph.microsoft.com/v1.0/me/drive/root:/ReproDoc/{fileName}:/content");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var memoryStream = new MemoryStream();
                await response.Content.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reset the position of the MemoryStream
                return memoryStream;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                // Log or handle the error content as needed
                throw new Exception($"File download failed with status code {response.StatusCode}: {errorContent}");
            }
        }
        private async Task<MemoryStream> DownloadFileByIdAsync(string accessToken, string fileId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://graph.microsoft.com/v1.0/me/drive/items/{fileId}/content");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var memoryStream = new MemoryStream();
                await response.Content.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reset the position of the MemoryStream
                return memoryStream;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                // Log or handle the error content as needed
                throw new Exception($"File download failed with status code {response.StatusCode}: {errorContent}");
            }
        }
        public async Task DeleteFileByIdAsync(string accessToken, string fileId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"https://graph.microsoft.com/v1.0/me/drive/items/{fileId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                // Log or handle the error content as needed
                throw new Exception($"File deletion failed with status code {response.StatusCode}: {errorContent}");
            }
        }

    }
}
