using Optimus.Services.Models;
using Optimus.Services.ReadService;
using Optimus.Services.ProgramInterfaceService;
using Optimus.Services.WriteService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimus.Services.AnalysisService
{
    public class AnalysisService : IAnalysisService
    {
        private readonly IReadService readService;
        private readonly IWriteService writeService;
        private readonly IProgramInterfaceService programInterfaceService;

        public AnalysisService(IReadService ReadService, IWriteService WriteService, IProgramInterfaceService ProgramInterfaceService)
        {
            readService = ReadService;
            writeService = WriteService;
            programInterfaceService = ProgramInterfaceService;
        }

        public List<FileInformationModel> FindVideo(string input)
        {
            List<string> paths = input.Split(',').ToList();

            var result = new List<FileInformationModel>();

            foreach (var p in paths)
            {
                var files = GetMediaFiles(p);

                result.AddRange(files);
            }

            return result;
        }

        public void OrganiseCopyMediaFiles(string input)
        {
            var videos = FindVideo(input);

            videos = readService.ExtractMetaInfoFromMediaTitle(videos).SuccessFiles;

            videos = programInterfaceService.UserManualFixFiles(videos);

            var movies = videos.Where(x => x.MediaInformation.IsMovie == true).ToList();
            videos.RemoveAll(x => x.MediaInformation.IsMovie == true);
            var personal = videos.Where(x => x.MediaInformation.IsPersonal == true).ToList();
            videos.RemoveAll(x => x.MediaInformation.IsPersonal == true);

            List<SeriesFileStructureModel> seriesFileStructures = new List<SeriesFileStructureModel>();
            List<FileInformationModel> unknownFiles = new List<FileInformationModel>();

            foreach (var video in videos)
            {
                if (seriesFileStructures.Count > 0)
                {
                    try
                    {
                        if (!seriesFileStructures.Any(x => x.SeriesName.Equals(video.MediaInformation.Name)))
                        {
                            SeriesFileStructureModel seriesFileStructure = new SeriesFileStructureModel();
                            seriesFileStructure.Seasons = new List<string>();
                            seriesFileStructure.Seasons.Add(video.MediaInformation.Season);
                            seriesFileStructure.SeriesName = video.MediaInformation.Name;
                            seriesFileStructures.Add(seriesFileStructure);
                        }
                        else
                        {
                            if (!seriesFileStructures.Where(x => x.SeriesName.Equals(video.MediaInformation.Name)).Any(x => x.Seasons.Any( o => o.Equals(video.MediaInformation.Season))))
                            {
                                int index = seriesFileStructures.FindIndex(x => x.SeriesName.Equals(video.MediaInformation.Name));
                                seriesFileStructures[index].Seasons.Add(video.MediaInformation.Season);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        unknownFiles.Add(video);
                    }
                    
                }
                else
                {
                    SeriesFileStructureModel seriesFileStructure = new SeriesFileStructureModel();
                    seriesFileStructure.Seasons = new List<string>();
                    seriesFileStructure.Seasons.Add(video.MediaInformation.Season);
                    seriesFileStructure.SeriesName = video.MediaInformation.Name;
                    seriesFileStructures.Add(seriesFileStructure);
                }

            }

            if (unknownFiles.Count > 0)
            {
                Console.WriteLine("Found {0} unknown files", unknownFiles.Count);
                Console.WriteLine("Writing unkown files to disk...");
                var writeSuccess = writeService.WriteFilenamesToFile(unknownFiles);
                if (writeSuccess)
                {
                    Console.WriteLine("Write Successful");
                }
                else
                {
                    Console.WriteLine("Write Failed");
                }
            }

            var allVideos = new List<FileInformationModel>();
            allVideos.AddRange(videos);
            allVideos.AddRange(movies);
            allVideos.AddRange(personal);

            Console.WriteLine("Found {0} movies", movies.Count());
            foreach (var item in movies)
            {
                Console.WriteLine("{0} ({1})", item.MediaInformation.Name, item.MediaInformation.Year);
            }
            Console.WriteLine("Found {0} personal files", personal.Count());

            foreach (var series in seriesFileStructures)
            {
                Console.WriteLine("{0}. {1} Seasons", series.SeriesName, series.Seasons.Count);
            }

            Console.WriteLine("Would you like to copy these series/movies?");
            var uInput = Console.ReadLine();

            if (uInput.Contains('y') || uInput.Contains('Y'))
            {
                writeService.CopyMediaFiles(seriesFileStructures, allVideos.Where(x => !string.IsNullOrEmpty(x.MediaInformation.Name)).ToList());
            }

            Console.WriteLine("Could not read {0} files", unknownFiles.Count);

            Console.ReadLine();

        }

        public List<FileInformationModel> GetMediaFiles(string directory)
        {
            var files = readService.GetFolderInformation(directory);
            var result = new List<FileInformationModel>();

            foreach (var file in files)
            {
                file.MediaInformation = new MediaFileModel();
                if (IsVideoFile(file))
                {
                    result.Add(file);
                }
            }

            return result;
        }

        public bool IsVideoFile(FileInformationModel file)
        {
            bool result = false;

            switch (file.Filetype)
            {
                case ".webm":
                case ".mkv":
                case ".flv":
                case ".vob":
                case ".ogv":
                case ".ogg":
                case ".drc":
                case ".gif":
                case ".gifv":
                case ".mng":
                case ".avi":
                case ".mov":
                case ".qt":
                case ".wmv":
                case ".yuv":
                case ".rm":
                case ".rmvb":
                case ".asf":
                case ".amv":
                case ".mp4":
                case ".m4p":
                case ".m4v":
                case ".mpg":
                case ".mp2":
                case ".mpeg":
                case ".mpe":
                case ".mpv":
                case ".m2v":
                case ".svi":
                case ".3gp":
                case ".3g2":
                case ".mxf":
                case ".roq":
                case ".nsv":
                case ".f4v":
                case ".f4p":
                case ".f4a":
                case ".f4b":
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }

            return result;
        }

        public bool IsImageFile(string filepath)
        {
            throw new NotImplementedException();
        }
    }
}
