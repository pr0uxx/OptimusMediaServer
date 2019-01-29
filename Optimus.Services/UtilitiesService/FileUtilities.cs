using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Optimus.Services.UtilitiesService
{
    public class FileUtilities : IFileUtilities
    {
        public void WaitForFileUnlock(string filepath)
        {
            bool fileIsLocked = true;
            FileInfo fileInfo = new FileInfo(filepath);

            while (fileIsLocked)
            {
                fileIsLocked = IsFileLocked(fileInfo);
                Thread.Sleep(2500);
            }
        }

        private bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
    }
}
