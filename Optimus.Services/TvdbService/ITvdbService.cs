using Optimus.Data.Models;
using Optimus.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Optimus.Services.TvdbService
{
    public interface ITvdbService
    {
        Task<string> GetSeriesImage(string seriesId);
        Task<string> GetSeriesId(string seriesName);
        Task<string> GetSeriesId(string seriesName, bool fixPeriodBug);
        Task<SavedFile> GetFileCachedInfo(FileInformationModel file);
    }
}
