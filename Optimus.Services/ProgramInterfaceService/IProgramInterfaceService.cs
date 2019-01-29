using Optimus.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimus.Services.ProgramInterfaceService
{
    public interface IProgramInterfaceService
    {
        List<FileInformationModel> UserManualFixFiles(List<FileInformationModel> files);
    }
}
