using CsvHelper;
using Optimus.Services.Models;
using Optimus.Services.WriteService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OptimusSite.Services;

namespace Optimus.Services.ReadService
{
    public class ReadService : IReadService
    {
        private readonly IWriteService writeService;
        private AppSettings appSettings;

        private readonly string nameChangePath;
        private readonly string outputDirectoryPath;

        public ReadService(IOptions<AppSettings> AppSettings, IWriteService WriteService)
        {
            appSettings = AppSettings.Value;
            writeService = WriteService;

            nameChangePath = appSettings.FileEditPath;
            outputDirectoryPath = appSettings.FileOutputFolder;
        }

        private static readonly Regex rx = new Regex
        (@"(.*?)\.S?(\d{1,2})E?(\d{2})\.(.*)", RegexOptions.IgnoreCase);

        public List<FileInformationModel> GetFolderInformation(string directory, bool recursive = true)
        {
            DirectoryInfo di = new DirectoryInfo(directory);
            List<FileInfo> fi = new List<FileInfo>();
            List<FileInformationModel> result = new List<FileInformationModel>();

            if (recursive)
            {
                //fi = di.GetFiles("*", SearchOption.AllDirectories).ToList();

                try
                {
                    var dirs = di.GetDirectories();

                    foreach (var dir in dirs)
                    {
                        try
                        {
                            fi.AddRange(dir.GetFiles("*", SearchOption.AllDirectories).ToList());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Couldn't get file {0}. {1}", dir.FullName, ex.Message);
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Failed to get directory {0}", di.FullName);
                    return result;
                }

            }
            else
            {
                try
                {
                    fi.AddRange(di.GetFiles().ToList());
                }
                catch
                {
                    Console.WriteLine("Failed to get directory {0}", di.FullName);
                    return result;
                }

            }


            foreach (var f in fi)
            {
                var fileInfo = new FileInformationModel();

                try
                {
                    fileInfo = new FileInformationModel()
                    {
                        //FileAttributes = File.GetAttributes(f.FullName),
                        FileInfo = f,
                        FileSizeBytes = f.Length,
                        Filename = f.Name,
                        Filetype = f.Extension,
                        LastUsed = f.LastAccessTime
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Caught exception trying to read file", f.Name);
                }


                result.Add(fileInfo);
            }

            return result;
        }

        public (List<FileInformationModel> SuccessFiles, List<FileInformationModel> FailedFiles) ExtractMetaInfoFromMediaTitle(List<FileInformationModel> files)
        {
            var failedFiles = new List<FileInformationModel>();
            var successFiles = new List<FileInformationModel>();

            //foreach (var file in files)
            //{
            //    file.MediaInformation = new MediaFileModel();

            //    MatchCollection matches = rx.Matches(file.Filename);

            //    foreach (Match match in matches)
            //    {
            //        file.MediaInformation.Name = match.Groups[1].ToString().Trim();
            //        file.MediaInformation.Season = match.Groups[2].ToString().Trim();
            //        file.MediaInformation.Episode = match.Groups[3].ToString().Trim();
            //        file.MediaInformation.TheRest = match.Groups[4].ToString().Trim();
            //        //Console.WriteLine("Name: {0}, Season: {1}, Ep: {2}, Stuff: {3}\n",
            //        //    match.Groups[1].ToString().Trim(), match.Groups[2],
            //        //    match.Groups[3], match.Groups[4].ToString().Trim());
            //    }

            //    if (String.IsNullOrEmpty(file.MediaInformation.Name))
            //    {
            //        failedFiles.Add(file);
            //    }
            //    else
            //    {
            //        successFiles.Add(file);
            //    }


            //}

            var (ParsedFiles1, FailedFiles1) = CheckIsFilePersonal(files);

            successFiles.AddRange(ParsedFiles1);

            (ParsedFiles1, FailedFiles1) = BruteForceMetaInformationFromMediaTitle(files);

            successFiles.AddRange(ParsedFiles1);

            (ParsedFiles1, FailedFiles1) = ExtractMovieMetaInformation(FailedFiles1);

            successFiles.AddRange(ParsedFiles1);

            (ParsedFiles1, FailedFiles1) = BruteForceMovieMetaInformation(FailedFiles1);

            successFiles.AddRange(ParsedFiles1);

            Console.WriteLine("Managed to parse {0}/{1} files. {2} Files could not be parsed", successFiles.Count, files.Count, FailedFiles1.Count);
            Console.WriteLine("Logging failed files to FailedFiles.txt");
            writeService.WriteFilenamesToFile(FailedFiles1, "FailedFiles.txt");

            return (successFiles, FailedFiles1);

        }

        public List<FileInformationModel> ReadMediaFilesFromCSV(List<FileInformationModel> files)
        {
            List<MediaFileCSVModel> readResult = new List<MediaFileCSVModel>();

            using (TextReader reader = File.OpenText(string.Concat(outputDirectoryPath, nameChangePath)))
            {
                CsvReader csv = new CsvReader(reader);
                csv.Configuration.HasHeaderRecord = true;
                readResult.AddRange(csv.GetRecords<MediaFileCSVModel>());
            }

            foreach (var f in files)
            {
                var r = readResult.First(x => x.CurrentName.Equals(f.Filename));

                f.Filename = r.NewName ?? f.Filename;
                f.MediaInformation.IsMovie = r.IsMovie;
                f.MediaInformation.IsPersonal = r.IsPersonal;
            }

            return files;
        }

        public (List<FileInformationModel> ParsedFiles, List<FileInformationModel> FailedFiles) CheckIsFilePersonal(List<FileInformationModel> files)
        {
            List<FileInformationModel> ParsedFiles = new List<FileInformationModel>();
            List<FileInformationModel> FailedFiles = new List<FileInformationModel>();

            foreach (var file in files)
            {
                Regex rgx = new Regex(@"\d{2}-\d{2}-\d{4}");
                if (rgx.Match(file.Filename).Success)
                {
                    file.MediaInformation.IsPersonal = true;
                    ParsedFiles.Add(file);
                }
                else
                {
                    FailedFiles.Add(file);
                }
            }

            return (ParsedFiles, FailedFiles);
        }



        public (List<FileInformationModel> ParsedFiles, List<FileInformationModel> FailedFiles) BruteForceMovieMetaInformation(List<FileInformationModel> files)
        {
            List<FileInformationModel> ParsedFiles = new List<FileInformationModel>();
            List<FileInformationModel> FailedFiles = new List<FileInformationModel>();

            foreach (var file in files)
            {
                if (!file.Filetype.Equals(".ogg"))
                {
                    //if (file.Filename.Equals("The.Lost.City.of.Z.2017.HDRip.XviD.AC3-EVO.avi"))
                    //{
                    //    //stop here
                    //    var x = 1;
                    //    var y = 2 + x;
                    //}

                    int index = 0;
                    foreach (char c in file.Filename)
                    {
                        if (Char.IsDigit(c) && (c.Equals('1') || c.Equals('2')) && file.Filename.Length > index + 4)
                        {
                            if (/*file.Filename[index -1].Equals('(') ||*/ file.Filename.Substring(index, 4).All(x => Char.IsDigit(x)))
                            {
                                file.MediaInformation.IsMovie = true;
                                file.MediaInformation.Name = file.Filename.Substring(0, index);
                                file.MediaInformation.Year = file.Filename.Substring(index, 4);
                                ParsedFiles.Add(file);
                            }
                            else
                            {
                                string y = file.Filename.Substring(index, 4);
                                FailedFiles.Add(file);
                            }
                        }

                        index++;
                    }
                }
                else
                {
                    FailedFiles.Add(file);
                }

            }

            return (ParsedFiles, FailedFiles);
        }

        public (List<FileInformationModel> ParsedFiles, List<FileInformationModel> FailedFiles) ExtractMovieMetaInformation(List<FileInformationModel> files)
        {
            List<FileInformationModel> ParsedFiles = new List<FileInformationModel>();
            List<FileInformationModel> FailedFiles = new List<FileInformationModel>();

            Regex regex = new Regex(@"^(?<MovieName>.+)\((?<Year>\d+)\)(?<AdditionalText>[^\.]*)\.(?<Extension>[^\.]*)$");

            foreach (var file in files)
            {
                Match match = regex.Match(file.Filename);

                if (!string.IsNullOrEmpty(match.Groups["MovieName"].Value))
                {
                    try
                    {
                        file.MediaInformation.IsMovie = true;
                        file.MediaInformation.Name = match.Groups["MovieName"].Value;
                        file.MediaInformation.Year = match.Groups["Year"].Value;
                        ParsedFiles.Add(file);
                    }
                    catch
                    {
                        FailedFiles.Add(file);
                    }
                }
                else
                {
                    FailedFiles.Add(file);
                }
            }

            return (ParsedFiles, FailedFiles);
        }

        public (List<FileInformationModel> ParsedFiles, List<FileInformationModel> FailedFiles) BruteForceMetaInformationFromMediaTitle(List<FileInformationModel> files)
        {
            List<FileInformationModel> ParsedFiles = new List<FileInformationModel>();
            List<FileInformationModel> FailedFiles = new List<FileInformationModel>();

            foreach (var file in files)
            {
                //if (file.Filename.Contains("Rick"))
                //{
                //    Console.WriteLine("Rick");
                //}
                string filename = file.Filename;

                string newFilename = string.Empty;
                var index = 0;
                foreach (var c in filename)
                {
                    if (index >= 1)
                    {
                        if (c.ToString().Equals("s", StringComparison.OrdinalIgnoreCase))
                        {
                            if ((char.IsDigit(filename[index + 1]) && filename[index + 2].ToString().Equals("e", StringComparison.OrdinalIgnoreCase)))
                            {
                                file.MediaInformation.Name = filename.Substring(0, index - 1);
                                file.MediaInformation.Season = filename.Substring(index + 1, 1);
                                file.MediaInformation.Episode = filename.Substring(index + 3, 1);
                                file.MediaInformation.TheRest = filename.Substring(index + 4, filename.Length - (index + 5));
                                break;
                            }
                            else if (char.IsDigit(filename[index + 1]) && char.IsDigit(filename[index + 2]) && filename[index + 3].ToString().Equals("e", StringComparison.OrdinalIgnoreCase))
                            {
                                file.MediaInformation.Name = filename.Substring(0, index - 1);
                                file.MediaInformation.Season = filename.Substring(index + 1, 2);
                                file.MediaInformation.Episode = filename.Substring(index + 4, 2);
                                file.MediaInformation.TheRest = filename.Substring(index + 5, (filename.Length - (index + 6)));
                                break;
                            }
                        }
                    }
                    

                    index++;
                }
                try
                {
                    if (file.MediaInformation.Name == null)
                    {
                        FailedFiles.Add(file);
                    }
                    else
                    {
                        ParsedFiles.Add(file);
                    }
                }
                catch
                {
                    FailedFiles.Add(file);
                }

            }

            return (ParsedFiles, FailedFiles);
        }
    }
}
