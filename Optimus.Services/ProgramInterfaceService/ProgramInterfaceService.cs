using Optimus.Services.Models;
using Optimus.Services.ReadService;
using Optimus.Services.UtilitiesService;
using Optimus.Services.WriteService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optimus.Services.ProgramInterfaceService;

namespace Optimus.Services.ProgramInterfaceService
{
    public class ProgramInterfaceService : IProgramInterfaceService
    {
        private readonly IWriteService writeService;
        private readonly IReadService readService;
        private readonly IFileUtilities fileUtilities;

        private readonly string nameChangePath = @"\MediaFileNames.csv";
        private readonly string outputDirectoryPath = @".\Output";

        public ProgramInterfaceService(IWriteService WriteService, IReadService ReadService, IFileUtilities FileUtilities)
        {
            writeService = WriteService;
            readService = ReadService;
            fileUtilities = FileUtilities;
        }

        public List<FileInformationModel> UserManualFixFiles(List<FileInformationModel> files)
        {
            Console.WriteLine("Writing files to csv");

            writeService.WriteMediaFilesToCSV(files);

            Console.WriteLine("Opening default csv editor, program will continue after you close the file");

            ProcessStartInfo startInfo = new ProcessStartInfo(string.Concat(outputDirectoryPath, nameChangePath));
            Process.Start(startInfo);

            fileUtilities.WaitForFileUnlock(string.Concat(outputDirectoryPath, nameChangePath));

            Console.WriteLine("File is unlocked, press enter to continue");
            Console.ReadLine();

            files = readService.ReadMediaFilesFromCSV(files);

            return files;
        }
    }
}
