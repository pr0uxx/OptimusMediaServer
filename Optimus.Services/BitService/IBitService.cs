using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Optimus.Services.BitService
{
    public interface IBitService
    {
        Task<SearchResult> Search();
    }
}
