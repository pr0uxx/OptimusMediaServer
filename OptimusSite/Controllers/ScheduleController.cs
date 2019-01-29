using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Optimus.Services.AnalysisService;
using Optimus.Services.BitService;
using Optimus.Services.ReadService;
using Optimus.Services.TvdbService;
using OptimusSite.Models;

namespace OptimusSite.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly IAnalysisService analysisService;
        private readonly IBitService bitService;
        private readonly IReadService readService;
        private readonly ITvdbService tvdbService;

        private const string ShowSearchLocation = @"O:\Media";
        private const string DefaultImaageUrl = @"/img/bcs.jpg";

        public ScheduleController(IAnalysisService AnalysisService, IBitService BitService, IReadService ReadService,
                                    ITvdbService TvdbService)
        {
            analysisService = AnalysisService;
            bitService = BitService;
            readService = ReadService;
            tvdbService = TvdbService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new SearchResultViewModel
            {
                SearchResult = await bitService.Search()
            };

            return View(model);
        }

        public async Task<IActionResult> TvShows(string order = null)
        {
            var model = new TvShowViewModel
            {
                Order = order,
                files = analysisService.FindVideo(ShowSearchLocation)
            };

            model.files = readService.ExtractMetaInfoFromMediaTitle(model.files).SuccessFiles;

            if (string.IsNullOrEmpty(order))
            {
                model.files = model.files.Where(x => x.MediaInformation.IsMovie == false && x.MediaInformation.IsPersonal == false).OrderBy(x => x.MediaInformation.Name).ToList();

                List<string> searchedFiles = new List<string>();

                foreach (var f in model.files)
                {
                    var matchName = searchedFiles.FirstOrDefault(x => x.Equals(f.MediaInformation.Name));

                    if (string.IsNullOrEmpty(matchName))
                    {
                        searchedFiles.Add(f.MediaInformation.Name);

                        var cacheResult = await tvdbService.GetFileCachedInfo(f);

                        if (cacheResult != null)
                        {
                            f.FileImageUrl = cacheResult.SiteRelativeImageUrl;
                        }
                        else
                        {
                            f.FileImageUrl = DefaultImaageUrl;
                        } 
                    }
                    else
                    {
                        f.FileImageUrl = model.files.FirstOrDefault(x => x.MediaInformation.Name.Equals(f.MediaInformation.Name)).FileImageUrl;
                    }

                    
                }
            }
            else if (order.Equals("CreatedDesc", StringComparison.OrdinalIgnoreCase))
            {
                model.files = model.files
                    .Where(x => x.MediaInformation.IsMovie == false && x.MediaInformation.IsPersonal == false)
                    .OrderByDescending(x => x.FileInfo.CreationTime).ToList();
            }            

            return View(model);
        }
    }
}