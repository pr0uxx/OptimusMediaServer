using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimus.Services.Models
{
    public class SeriesFileStructureModel
    {
        public string SeriesName { get; set; }
        public List<string> Seasons { get; set; }
    }
}
