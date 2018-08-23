using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XEditor
{
    public static class RecentFilesController
    {
        public static int MaxFiles = 5;

        public static List<string> GetRecentFiles()
        {
            List<string> files = new List<string>();

            for(int I = 1; I <= MaxFiles; I++)
            {
                if (Global.Preferences.KeyExists("RecentFile" + I.ToString()))
                    files.Add(Global.Preferences.Read("RecentFile"+I.ToString()));
            }

            return files;
;        }

        public static void AddFile(string file)
        {
            List<string> files = GetRecentFiles();

            if (files.Count > 0 && files.Last() == file)
                return;

            if (files.Count >= MaxFiles)
                files.RemoveAt(0);

            files.Add(file);

            for (int I = 1; I <= MaxFiles+1; I++)
                Global.Preferences.DeleteKey("RecentFile" + I.ToString());

            for (int I = 1; I < files.Count+1; I++)
                Global.Preferences.Write("RecentFile" + I.ToString(), files[I-1]);

            MainWindow.Instance.LoadRecentFiles();
        }

        public static void Clear()
        {
            for (int I = 1; I <= MaxFiles; I++)
                Global.Preferences.DeleteKey("RecentFile" + I.ToString());

            MainWindow.Instance.LoadRecentFiles();
        }
    }
}
