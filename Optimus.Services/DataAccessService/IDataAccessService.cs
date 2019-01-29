using Optimus.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Optimus.Services.DataAccessService
{
    public enum SavedFileQueryType
    {
        Filename,
        TvdbId
    }

    public interface IDataAccessService
    {
        Task<SavedFile> GetSavedFileAsync(string filename);
        Task<DbResult> AddSavedFileAsync(SavedFile entity);
    }
}
