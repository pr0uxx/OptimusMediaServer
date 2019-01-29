using Optimus.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.VisualBasic.FileIO;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using CsvHelper;
using Microsoft.Extensions.Options;
using OptimusSite.Services;

namespace Optimus.Services.WriteService
{
    public class WriteService : IWriteService
    {
        private AppSettings appSettings { get; set; }

        private readonly string nameChangePath;
        private readonly string outputDirectoryPath;

        public WriteService(IOptions<AppSettings> AppSettings)
        {
            appSettings = AppSettings.Value;
            nameChangePath = appSettings.FileEditPath;
            outputDirectoryPath = appSettings.FileOutputFolder;
        }

        


        public bool WriteFilenamesToFile(List<FileInformationModel> files, string filename = null)
        {
            var success = false;
            string filepath = outputDirectoryPath;

            try
            {
                var lines = files.Select(x => x.Filename).ToArray();

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                if (string.IsNullOrEmpty(filename))
                {
                    File.WriteAllLines(filepath + "allFiles.txt", lines);
                }
                else
                {
                    File.WriteAllLines(filepath + filename, lines);
                }

                success = true;
            }
            catch (Exception)
            {
                //nothing to do
            }

            return success;

        }

        public void WriteMediaFilesToCSV(List<FileInformationModel> files)
        {
            var nameList = new List<MediaFileCSVModel>();

            foreach (var f in files)
            {
                var n = new MediaFileCSVModel()
                {
                    CurrentName = f.Filename,
                    NewName = string.Empty,
                    IsMovie = f.MediaInformation.IsMovie,
                    IsPersonal = f.MediaInformation.IsPersonal
                };

                nameList.Add(n);
            }

            var path = string.Concat(outputDirectoryPath, nameChangePath);

            try
            {
                using (TextWriter tw = new StreamWriter(path))
                {
                    var csv = new CsvWriter(tw);
                    csv.WriteRecords(nameList);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Could not write to csv file. Caught Exception: {0}", ex.Message);
            }

            
        }

        public void CopyMediaFiles(List<SeriesFileStructureModel> seriesFileStructures, List<FileInformationModel> files)
        {
            string writeLocation = appSettings.MediaFileOutputLocation;

            if (!Directory.Exists(writeLocation))
            {
                Directory.CreateDirectory(writeLocation);
            }

            var movies = files.Where(x => x.MediaInformation.IsMovie == true);
            files.RemoveAll(x => x.MediaInformation.IsMovie == true);
            var personal = files.Where(x => x.MediaInformation.IsPersonal == true);
            files.RemoveAll(x => x.MediaInformation.IsPersonal == true);

            Console.WriteLine("Building File Structure");
            //Write the files structure
            foreach (var struc in seriesFileStructures)
            {
                Directory.CreateDirectory(writeLocation + struc.SeriesName);
                foreach (var season in struc.Seasons)
                {
                    Directory.CreateDirectory(writeLocation + struc.SeriesName + "//" + "Season" + season);
                }
            }

            if (movies.Count() > 0)
            {
                if (!Directory.Exists(writeLocation + "Movies//"))
                {
                    Directory.CreateDirectory(writeLocation + "Movies//");
                }
            }

            if (personal.Count() > 0)
            {
                if (!Directory.Exists(writeLocation + "Personal//"))
                {
                    Directory.CreateDirectory(writeLocation + "Personal//");
                }
            }

            Console.WriteLine("Copying Movies");

            foreach (var movie in movies)
            {
                File.Copy(movie.FileInfo.FullName, writeLocation + "Movies//" + movie.Filename);
            }

            Console.WriteLine("Copying Personal Files");

            foreach (var pers in personal)
            {
                File.Copy(pers.FileInfo.FullName, writeLocation + "Personal//" + pers.Filename);
            }

            var dirList = Directory.GetDirectories(writeLocation);

            //Write the files
            //List<FileInfo> transferringFiles = new List<FileInfo>();
            List<(Thread thread, FileInfo fileInfo)> runningThreads = new List<(Thread thread, FileInfo fileInfo)>();
            List<int> threads = new List<int>();
            long count = 0;
            var maxThreads = 10;

            var totalTransferSize = files.Sum(x => x.FileInfo.Length);
            long totalTransferred = 0;




                foreach (var file in files)
                {

                    string fileCheckPath = string.Concat(writeLocation, file.MediaInformation.Name, "//", "Season", file.MediaInformation.Season, "//", file.Filename);
                    //make a temp object
                    FileInformationModel tmp = file;
                    //spin up a new thread

                    var thisThread = new Thread(new ThreadStart(() => FileCopyThread(tmp, writeLocation)));
                    try
                    {
                        while (runningThreads.Count > maxThreads - 1)
                        {
                            List<int> rmFiles = new List<int>();
                            var rmThreads = new List<(Thread thread, FileInfo fileInfo)>();

                            foreach (var x in runningThreads)
                            {
                                if (!x.thread.IsAlive)
                                {
                                    rmThreads.Add(x);
                                }
                            }

                            if (rmThreads.Count <= 0)
                            {
                                Thread.Sleep(1000);
                            }

                            foreach (var rm in rmThreads)
                            {
                                runningThreads.RemoveAll(x => x.thread == rm.thread);

                                totalTransferred += rm.fileInfo.Length;
                            }
                        }
                        //File.Copy(file.FileInfo.FullName, , false);
                        thisThread.Start();
                        count++;



                        //transferringFiles.Add(file.FileInfo);
                        (Thread Thread, FileInfo fileInfo) temp = (thisThread, file.FileInfo);
                        runningThreads.Add(temp);
                        //Console.SetCursorPosition(0, Console.CursorTop);
                        //Console.Write("Copying File {0} of {1}" + Environment.NewLine, count, files.Count);
                        //FileSystem.CopyFile(file.FileInfo.FullName, string.Concat(writeLocation, file.MediaInformation.Name, "//", "Season", file.MediaInformation.Season, "//", file.Filename), UIOption.AllDialogs, UICancelOption.ThrowException);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("File {0} could not be copied. Caught Exception {1}", file.Filename, ex);
                    }

                }
        }

        public static void FileCopyThread(FileInformationModel file, string writeLocation)
        {
            //FileSystem.CopyFile(file.FileInfo.FullName, string.Concat(writeLocation, file.MediaInformation.Name, "//", "Season", file.MediaInformation.Season, "//", file.Filename), UIOption.AllDialogs, UICancelOption.ThrowException);
            File.Copy(file.FileInfo.FullName, string.Concat(writeLocation, file.MediaInformation.Name, "//", "Season", file.MediaInformation.Season, "//", file.Filename), false);
        }

        
    }
}
