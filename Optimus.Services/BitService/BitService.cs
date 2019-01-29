using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using Microsoft.Extensions.Options;
using Optimus.Services.TvdbService;
using OptimusSite.Services;

namespace Optimus.Services.BitService
{
    public class BitService : IBitService
    {
        private readonly ITvdbService tvdbService;

        private static readonly HttpClient httpClient = new HttpClient();
        private const string API_ENDPOINT = @"https://torrentapi.org/pubapi_v2.php";
        private const string APP_ID = "OptimusMediaServer_v0_1";
        private static string apiToken = string.Empty;
        private static DateTime tokenExpiry = new DateTime();
        private static DateTime lastRequest = new DateTime();
        private AppSettings appSettings;

        public BitService(IOptions<AppSettings> AppSettings, ITvdbService TvdbService)
        {
            appSettings = AppSettings.Value;
            tvdbService = TvdbService;
        }

        public async Task<SearchResult> Search()
        {
            var p = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("mode", "search"),
                new KeyValuePair<string, string>("category", "tv"),
                //new KeyValuePair<string, string>("min_seeders", "1000"),
                //new KeyValuePair<string, string>("search_string", "Vikings"),
                new KeyValuePair<string, string>("sort", "seeders"),
                new KeyValuePair<string, string>("format", "json_extended")
            };

            var httpResult = await HttpGet(p);
                
            var result = await httpResult.Content.ReadAsStringAsync();

            var convertedResult = JsonConvert.DeserializeObject<SearchResult>(result);

            if (convertedResult?.torrent_results?.Count > 0)
            {
                foreach (var c in convertedResult.torrent_results)
                {
                    c.ImageUrl = await tvdbService.GetSeriesImage(c.episode_info.tvdb);
                }
            }

            return convertedResult;
        }

        private static async Task<HttpResponseMessage> HttpGet(List<KeyValuePair<string, string>> parameters, bool forceNoGetToken = false, bool usePost = false)
        {
            while (DateTime.Now < lastRequest.AddSeconds(2))
            {
                Thread.Sleep(2000);
            }

            httpClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
            httpClient.DefaultRequestHeaders.Add("Accept", @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "utf-8");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");

            string result = string.Empty;
            string url = string.Concat(API_ENDPOINT, '?');

            //do we need a token?
            if ((string.IsNullOrEmpty(apiToken) || DateTime.Now > tokenExpiry) && forceNoGetToken == false)
            {
                await GetToken();
            }

            foreach (var p in parameters)
            {
                if (parameters.IndexOf(p) > 0)
                {
                    url += '&';
                }

                url += string.Concat(p.Key, '=', p.Value);
            }

            if (!string.IsNullOrEmpty(apiToken))
            {
                url += string.Concat("&token=", apiToken);
            }

            url += string.Concat("&app_id=", APP_ID);

            tokenExpiry = DateTime.Now.AddMinutes(15);

            if (usePost)
            {
                return await httpClient.PostAsync(url, new ByteArrayContent(new byte[' '], 0, 0));
            }
            else
            {
                return await httpClient.GetAsync(url);
            }


        }

        private static async Task GetToken()
        {
            var parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("get_token", "get_token"),
            };

            var asyncResult = await HttpGet(parameters, true);

            if (asyncResult.IsSuccessStatusCode)
            {
                var stringResult = await asyncResult.Content.ReadAsStringAsync();


                stringResult = HttpUtility.HtmlDecode(stringResult);

                var convertResult = JsonConvert.DeserializeObject<ApiKeyObject>(stringResult);

                apiToken = convertResult.Token;
            }
            else
            {
                throw new TransactionException(string.Concat("Failed to get token", await asyncResult.Content.ReadAsStringAsync()));
            }
        }
    }

    public class ApiKeyObject
    {
        public string Token { get; set; }
    }

    public class EpisodeInfo
    {
        public string imdb { get; set; }
        public string tvrage { get; set; }
        public string tvdb { get; set; }
        public string themoviedb { get; set; }
        public string airdate { get; set; }
        public string epnum { get; set; }
        public string seasonnum { get; set; }
        public string title { get; set; }
    }

    public class TorrentResult
    {
        public string filename { get; set; }
        public string title { get; set; }
        public string category { get; set; }
        public string download { get; set; }
        public int seeders { get; set; }
        public int leechers { get; set; }
        public object size { get; set; }
        public string pubdate { get; set; }
        public EpisodeInfo episode_info { get; set; }
        public int ranked { get; set; }
        public string info_page { get; set; }
        public string ImageUrl { get; set; }
    }

    public class SearchResult
    {
        public List<TorrentResult> torrent_results { get; set; }
    }
}


