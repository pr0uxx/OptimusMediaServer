using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Optimus.Services.Models;

namespace OptimusSite.Models
{
    public class TvShowViewModel 
    {
        public List<FileInformationModel> files { get; set; }
        public List<IGrouping<string, FileInformationModel>> GroupedFiles { get; set; }
        public string Order { get; set; }
    }


}
