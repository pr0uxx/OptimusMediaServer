using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimus.Services.Models
{
    public class MediaFileCSVModel
    {
        public string CurrentName { get; set; }
        public string NewName { get; set; }
        public bool IsMovie { get; set; }
        public bool IsPersonal { get; set; }
    }
}
