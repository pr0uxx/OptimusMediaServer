using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusSite.Services
{
    public class AppSettings
    {
        public string MediaFileOutputLocation { get; set; }
        public int MaxConcurrentFileTranfers { get; set; }
        public string FileOutputFolder { get; set; }
        public string FileEditPath { get; set; }
        public string TvdbApiKey { get; set; }
        public string TvdbUserKey { get; set; }
        public string TvdbUsername { get; set; }
        public string TvdbEndpoint { get; set; }
        public string TvdbImagesEndpoint { get; set; }
    }
}
