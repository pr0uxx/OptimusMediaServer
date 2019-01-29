using Microsoft.EntityFrameworkCore;
using Optimus.Data;
using Optimus.Data.Models;
using Optimus.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimus.Services.DataAccessService
{
    //When one cannot be arsed to make a new project, one makes a new service
    public class DataAccessService : IDataAccessService
    {
        private readonly ApplicationDbContext db;

        public DataAccessService(ApplicationDbContext dbContext)
        {
            db = dbContext;
        }

        public async Task<DbResult> AddSavedFileAsync(SavedFile entity)
        {
            var result = new DbResult();

            try
            {
                await db.AddAsync(entity);
                await db.SaveChangesAsync();
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;            
        }

        protected async Task<DbResult> AddSavedFileRangeAsync(IEnumerable<SavedFile> entityList)
        {
            DbResult result = new DbResult();

            try
            {
                await db.AddRangeAsync(entityList);
                await db.SaveChangesAsync();
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public async Task<bool> CacheLookupFileExists(string queryParam, SavedFileQueryType queryType)
        {
            switch (queryType)
            {
                case SavedFileQueryType.Filename:
                    return await db.SavedFiles.AnyAsync(x => x.Filename.Equals(queryParam));
                case SavedFileQueryType.TvdbId:
                    return await db.SavedFiles.AllAsync(x => x.TvdbSeriesId.Equals(queryParam));
                default:
                    return false;
            }
        }

        protected async Task<SavedFile> GetSavedFileAsync(int id)
        {
            return await db.SavedFiles.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<SavedFile> GetSavedFileAsync(string filename)
        {
            return await db.SavedFiles.FirstOrDefaultAsync(x => x.Filename.Equals(filename));
        }


        public async Task<DbResult> AddFileInformationAsync(FileInformationModel fileInformation)
        {
            var entity = new SavedFile()
            {
                FileFullPath = fileInformation.FileInfo.FullName,
                Filename = fileInformation.Filename,
                SiteRelativeImageUrl = fileInformation.FileImageUrl,
                TvdbSeriesId = fileInformation.TvdbSeriesId
            };

            var result = await AddSavedFileAsync(entity);

            return result;
        }
    }
}
