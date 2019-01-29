using Optimus.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimus.Services.AnalysisService
{
    public interface IAnalysisService
    {
        List<FileInformationModel> GetMediaFiles(string directory);

        List<FileInformationModel> FindVideo(string input);

        void OrganiseCopyMediaFiles(string input);
    }
}
