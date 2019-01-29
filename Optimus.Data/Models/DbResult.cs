using System;
using System.Collections.Generic;
using System.Text;

namespace Optimus.Data.Models
{
    public class DbResult
    {
        public int Id { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
