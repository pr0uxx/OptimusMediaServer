using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Optimus.Data.Models;
using Optimus.Services.DataAccessService;
using Optimus.Services.Models;
using OptimusSite.Services;

namespace Optimus.Services.TvdbService
{
    public class TvdbService : ITvdbService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly string API_ENDPOINT;
        private readonly string API_IMAGES_ENDPOINT;
        private readonly string API_KEY;
        private readonly string USER_KEY;
        private readonly string USER_NAME;
        private static string apiToken = string.Empty;
        

        private static DateTime tokenExpiry = new DateTime();
        private static DateTime lastRequest = new DateTime();

        private AppSettings appSettings;
        private readonly IDataAccessService dataAccessService;

        public TvdbService(IOptions<AppSettings> AppSettings, IDataAccessService DataAccessService)
        {
            appSettings = AppSettings.Value;
            dataAccessService = DataAccessService;

            API_KEY = appSettings.TvdbApiKey;
            USER_KEY = appSettings.TvdbUserKey;
            USER_NAME = appSettings.TvdbUsername;
            API_ENDPOINT = appSettings.TvdbEndpoint;
            API_IMAGES_ENDPOINT = appSettings.TvdbImagesEndpoint;
        }

        public async Task<(T Content, bool Success)> ApiGet<T>(string route/*, List<KeyValuePair<string, string>> queryParameters*/)
        {
            var apiResult = await HttpGet(route);

            if (apiResult.IsSuccessStatusCode)
            {
                var model = JsonConvert.DeserializeObject<T>(await apiResult.Content.ReadAsStringAsync());

                return (model, true);
            }
            else
            {
                return (default(T), false);
            }
        }

        public async Task<Optimus.Data.Models.SavedFile> GetFileCachedInfo(FileInformationModel file)
        {
            var cachedFileInfo = await dataAccessService.GetSavedFileAsync(file.MediaInformation.Name);

            if (cachedFileInfo != null)
            {
                return cachedFileInfo;
            }
            else
            {
                string seriesId = await GetSeriesId(file.MediaInformation.Name, true);

                if (!string.IsNullOrEmpty(seriesId))
                {
                    string seriesImage = await GetSeriesImage(seriesId);

                    var newRecord = new SavedFile()
                    {
                        FileFullPath = file.FileInfo.FullName,
                        Filename = file.MediaInformation.Name,
                        SiteRelativeImageUrl = seriesImage,
                        TvdbSeriesId = seriesId,
                        Watched = false,
                        WatchedTime = new TimeSpan()
                    };

                    await dataAccessService.AddSavedFileAsync(newRecord);

                    return newRecord;
                }
            }

            return null;
        }

        public async Task<string> GetSeriesId(string seriesName)
        {
            var seriesId = string.Empty;

            var route = string.Concat("/search/series?name=", seriesName);

            var (Content, Success) = await ApiGet<SeriesSearchQueryResults>(route);

            if (Success)
            {
                seriesId = Content.data.FirstOrDefault().id.ToString();
            }

            return seriesId;
        }

        public async Task<string> GetSeriesId(string seriesName, bool fixPeriodBug)
        {
            if (fixPeriodBug)
            {
                seriesName = seriesName.Replace(".", "%20");
            }

            return await GetSeriesId(seriesName);
        }

        public async Task<string> GetSeriesImage(string seriesId)
        {
            const string programFileDir = @"O:/Projects/OptimusMediaServer/OptimusSite/wwwroot/";
            const string imageDir = @"/img/series/";
            string imageDirFullPath = string.Concat(programFileDir, imageDir);

            if (!Directory.Exists(imageDirFullPath))
            {
                Directory.CreateDirectory(imageDirFullPath);
            }

            var files = Directory.EnumerateFiles(imageDirFullPath).ToList();

            var imageFileString = files.FirstOrDefault(x => x.Contains(seriesId));

            //var f = File.OpenRead(files[0]);

            if (!string.IsNullOrEmpty(imageFileString))
            {
                return string.Concat(imageDir, seriesId, '.', imageFileString.Split('.').LastOrDefault());
            }
            else
            {
                var route = string.Concat("/series/", seriesId, "/images/query?keyType=poster");

                var (Content, Success) = await ApiGet<SeriesImageQueryResults>(route);

                if (Success)
                {
                    if (Content.data.Count > 0)
                    {
                        WebClient webClient = new WebClient();

                        var contentUrl = string.Concat(API_IMAGES_ENDPOINT, Content.data[0].fileName);
                        var ext = contentUrl.Split('.').Last();

                        Uri uri = new Uri(contentUrl);

                        string fileLocation = string.Concat(imageDirFullPath, seriesId, '.', ext);

                        webClient.DownloadFileAsync(uri, fileLocation);
                        webClient.Dispose();

                        return fileLocation;
                    }
                }
            }

            return string.Empty;
        }

        public async Task GetToken()
        {
            //httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "utf-8");
            //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

            var parameters = new Dictionary<string, string>();

            parameters.Add("apikey", API_KEY);
            parameters.Add("userkey", USER_KEY);
            parameters.Add("username", USER_NAME);

            var route = "/login";

            var result = await HttpPost(route, parameters, true);

            if (result.IsSuccessStatusCode)
            {
                tokenExpiry = DateTime.Now.AddHours(23);
                lastRequest = DateTime.Now;
                apiToken = JsonConvert.DeserializeObject<TokenResult>(await result.Content.ReadAsStringAsync()).Token;
            }
            else
            {
                throw new TransactionException(string.Concat("Failed to get token", await result.Content.ReadAsStringAsync()));
            }
        }

        public async Task<HttpResponseMessage> HttpPost(string route, Dictionary<string, string> parameters, bool forceNoGetToken = false)
        {
            if ((string.IsNullOrEmpty(apiToken) || DateTime.Now > tokenExpiry) && forceNoGetToken == false)
            {
                await GetToken();
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);

            //httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "utf-8");
            //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            //httpClient.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("text/html; charset=utf-8"));
            //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            if (!httpClient.DefaultRequestHeaders.Accept.Contains(
                new MediaTypeWithQualityHeaderValue("application/json")))
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            string result = string.Empty;

            string url = string.Concat(API_ENDPOINT, route);

            parameters.Add("Content-Type", "application/json");

            var content = JsonConvert.SerializeObject(parameters);

            return await httpClient.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> HttpGet(string route, bool forceNoGetToken = false/*, List<KeyValuePair<string, string>> queryParameters = null*/)
        {
            if ((string.IsNullOrEmpty(apiToken) || DateTime.Now > tokenExpiry) && forceNoGetToken == false)
            {
                await GetToken();
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);

            if (!httpClient.DefaultRequestHeaders.Accept.Contains(new MediaTypeWithQualityHeaderValue("application/json")))
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            


            //httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "utf-8");
            //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

            var url = string.Concat(API_ENDPOINT, route);

            return await httpClient.GetAsync(url);
        }

        public class TokenResult
        {
            public string Token { get; set; }
        }

        //public class SeriesImageQueryParamDatum
        //{
        //    public string keyType { get; set; }
        //    public string languageId { get; set; }
        //    public List<string> resolution { get; set; }
        //    public List<string> subKey { get; set; }
        //}

        //public class SeriesImageQueryParamResults
        //{
        //    public List<Datum> data { get; set; }
        //}

        public class SeriesSearchResult
        {
            public List<string> aliases { get; set; }
            public string banner { get; set; }
            public string firstAired { get; set; }
            public int id { get; set; }
            public string network { get; set; }
            public string overview { get; set; }
            public string seriesName { get; set; }
            public string slug { get; set; }
            public string status { get; set; }
        }

        public class SeriesSearchQueryResults
        {
            public List<SeriesSearchResult> data { get; set; }
        }

        public class RatingsInfo
        {
            public decimal average { get; set; }
            public decimal count { get; set; }
        }

        public class SeriesImageQueryDatum
        {
            public string fileName { get; set; }
            public int id { get; set; }
            public string keyType { get; set; }
            public int languageId { get; set; }
            public RatingsInfo ratingsInfo { get; set; }
            public string resolution { get; set; }
            public string subKey { get; set; }
            public string thumbnail { get; set; }
        }

        public class Errors
        {
            public List<string> invalidFilters { get; set; }
            public string invalidLanguage { get; set; }
            public List<string> invalidQueryParams { get; set; }
        }

        public class SeriesImageQueryResults
        {
            public List<SeriesImageQueryDatum> data { get; set; }
            public Errors errors { get; set; }
        }

        public class APILoginForm
        {
            public string apikey { get; set; }
            public string userkey { get; set; }
            public string username { get; set; }
        }
    }


}
