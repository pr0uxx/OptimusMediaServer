using Optimus.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimus.Services.ReadService
{
    public interface IReadService
    {
        List<FileInformationModel> GetFolderInformation(string directory, bool recursive = true);
        (List<FileInformationModel> SuccessFiles, List<FileInformationModel> FailedFiles) ExtractMetaInfoFromMediaTitle(List<FileInformationModel> files);
        (List<FileInformationModel> ParsedFiles, List<FileInformationModel> FailedFiles) BruteForceMetaInformationFromMediaTitle(List<FileInformationModel> files);
        List<FileInformationModel> ReadMediaFilesFromCSV(List<FileInformationModel> files);
    }
}
