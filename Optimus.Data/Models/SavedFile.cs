using System;
using System.Collections.Generic;
using System.Text;

namespace Optimus.Data.Models
{
    public class SavedFile
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public string TvdbSeriesId { get; set; }
        public string SiteRelativeImageUrl { get; set; }
        public string FileFullPath { get; set; }
        public bool Watched { get; set; }
        public TimeSpan WatchedTime { get; set; }
    }
}
