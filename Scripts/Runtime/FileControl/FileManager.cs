#if UNITY_EDITOR || UNITY_STANDALONE_WIN
using System.Windows.Forms;
using System;
using System.Runtime.InteropServices;
using Ookii.Dialogs;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace FileControl
{
    public class WindowWrapper : IWin32Window
    {
        private IntPtr _hwnd;
        public WindowWrapper(IntPtr handle) { _hwnd = handle; }
        public IntPtr Handle { get { return _hwnd; } }
    }

    public struct ExtensionFilter
    {
        public string Name;
        public string[] Extensions;

        public ExtensionFilter(string filterName, params string[] filterExtensions)
        {
            Name = filterName;
            Extensions = filterExtensions;
        }
    }

    public class FileManager
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();



        public FileManager()
        { 
         
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        /// <summary>
        /// 讀取檔案
        /// </summary>
        /// <param name="title">視窗標題 </param>
        /// <param name="directory">預設路徑</param>
        /// <param name="extensions">檔案篩選</param>
        /// <param name="multiselect">是否可以多選</param>
        /// <returns></returns>
        public static string OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            FileManager fm = new FileManager();
            var fd = new VistaOpenFileDialog();

            fd.Title = title;

            if (extensions != null)
            {
                fd.Filter = fm.GetFilterFromFileExtensionList(extensions);
                fd.FilterIndex = 1;
            }
            else
            {
                fd.Filter = string.Empty;
            }
            fd.Multiselect = multiselect;

            if (!string.IsNullOrEmpty(directory))
            {
                fd.FileName = fm.GetDirectoryPath(directory);          
            }
            var res = fd.ShowDialog(new WindowWrapper(GetActiveWindow()));

            var filenames = res == DialogResult.OK ? fd.FileNames : new string[0];

            fd.Dispose();

            return (filenames.Length >0) ? filenames[0]: "";
        }

        /// <summary>
        /// 寫入檔案
        /// </summary>
        /// <param name="title">視窗標題 </param>
        /// <param name="directory">預設路徑</param>
        /// <param name="defaultName">預設檔案名稱</param>
        /// <param name="extensions">檔案篩選</param>
        /// <returns></returns>
        public static string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
        {
            FileManager fm = new FileManager();
            var fd = new VistaSaveFileDialog();
            fd.Title = title;
     
            var finalFilename = "";
           
            if (!string.IsNullOrEmpty(directory))
            {
                finalFilename = fm.GetDirectoryPath(directory);
            }

            if (!string.IsNullOrEmpty(defaultName))
            {
                finalFilename += defaultName;
            }

            fd.FileName = finalFilename;
            if (extensions != null)
            {
                fd.Filter = fm.GetFilterFromFileExtensionList(extensions);
                fd.FilterIndex = 1;
                fd.DefaultExt = extensions[0].Extensions[0];
                fd.AddExtension = true;
            }
            else
            {
                fd.DefaultExt = string.Empty;
                fd.Filter = string.Empty;
                fd.AddExtension = false;
            }
            var res = fd.ShowDialog(new WindowWrapper(GetActiveWindow()));
            var filename = res == DialogResult.OK ? fd.FileName : "";
            fd.Dispose();
            return filename;
        }

        /// <summary>
        /// 取得物件的資料夾路徑
        /// </summary>
        /// <param name="oldPath"></param>
        /// <returns></returns>
        public static string GetPathFolder(string oldPath)
        {
            string result = "";

            List<string> arr = oldPath.Split('\\').ToList();

            if (arr.Count > 0)
            {
                arr.RemoveAt(arr.Count-1);
                result = String.Join("\\", arr) + "\\";
            }

            return result;
        }

        /// <summary>
        /// 取得路徑的檔案名稱
        /// </summary>
        /// <returns></returns>
        public static string GetPathDataName(string oldPath)
        {
            string name = "";
            List<string> arr = oldPath.Split('\\').ToList();

            if (arr.Count > 0)
            {
                name = arr[arr.Count - 1];
            }

            return name;
        }

        /// <summary>
        /// 設定物件篩選
        /// </summary>
        /// <returns></returns>
        private string GetFilterFromFileExtensionList(ExtensionFilter[] extensions)
        {
            var filterString = "";
            foreach (var filter in extensions)
            {
                filterString += filter.Name + "(";

                foreach (var ext in filter.Extensions)
                {
                    filterString += "*." + ext + ",";
                }

                filterString = filterString.Remove(filterString.Length - 1);
                filterString += ") |";

                foreach (var ext in filter.Extensions)
                {
                    filterString += "*." + ext + "; ";
                }

                filterString += "|";
            }
            filterString = filterString.Remove(filterString.Length - 1);
            return filterString;
        }

        /// <summary>
        /// 取得路徑
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private string GetDirectoryPath(string directory)
        {
            var directoryPath = Path.GetFullPath(directory);

            if (Path.GetPathRoot(directoryPath) == directoryPath)
            {
                return directory;
            }
            return Path.GetDirectoryName(directoryPath) + Path.DirectorySeparatorChar;
        }

    }
}
#endif