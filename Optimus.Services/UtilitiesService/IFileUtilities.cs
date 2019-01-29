using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimus.Services.UtilitiesService
{
    public interface IFileUtilities
    {
        void WaitForFileUnlock(string filepath);
    }
}
