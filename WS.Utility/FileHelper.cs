using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Utility
{
    public static class FileHelper
    {
        public static bool Delete(string filename)
        {

            try
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);

                }
                return true;

            }
            catch (Exception ex)
            {

                return false;
            }

        }

        public static void DeleteFilesBeforeTime(string path, DateTime datetime)
        {
            DirectoryInfo forder = new DirectoryInfo(path);
            List<FileInfo> list = forder.GetFiles().Where(a => a.LastWriteTime < datetime).ToList();

            foreach (FileInfo info in list)
            {
                try
                {
                    if (File.Exists(info.FullName))
                    {
                        File.Delete(info.FullName);

                    }


                }
                catch (Exception ex)
                {


                }
            }

        }

    }
}
