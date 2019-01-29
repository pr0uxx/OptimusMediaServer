using Optimus.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimus.Services.WriteService
{
    public interface IWriteService
    {
        bool WriteFilenamesToFile(List<FileInformationModel> files, string filename = null);
        void CopyMediaFiles(List<SeriesFileStructureModel> seriesFileStructures, List<FileInformationModel> files);
        void WriteMediaFilesToCSV(List<FileInformationModel> files);
    }
}
