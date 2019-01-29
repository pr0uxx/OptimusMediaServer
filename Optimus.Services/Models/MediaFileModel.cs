using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimus.Services.Models
{
    public class MediaFileModel
    {
        public string Name { get; set; }
        public String Season { get; set; }
        public string Episode { get; set; }
        public string TheRest { get; set; }
        public bool IsMovie { get; set; }
        public bool IsPersonal { get; set; }
        public string Year { get; set; }
        public string Title { get; set; }
    }
}
