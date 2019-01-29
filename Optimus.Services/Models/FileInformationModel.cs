using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimus.Services.Models
{
    public class FileInformationModel
    {
        public string Filetype { get; set; }
        public DateTime LastUsed { get; set; }
        public string Filename { get; set; }
        public long FileSizeBytes { get; set; }
        //public FileAttributes FileAttributes { get; set; }
        public FileInfo FileInfo { get; set; }
        public MediaFileModel MediaInformation { get; set; }
        public string FileImageUrl { get; set; }
        public String TvdbSeriesId { get; set; }
    }
}
