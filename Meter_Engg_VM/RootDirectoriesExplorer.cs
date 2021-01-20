//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
////using System.Windows.Forms;
////using Microsoft.VisualBasic.FileIO;
//using System.IO;
//using System.Diagnostics;
//using System.Runtime.InteropServices;
//using System.Security.Cryptography.X509Certificates;

//namespace Meter_Engg_VM
//{
//    class RootDirectoriesExplorer
//    {
//        #region Declaration
//        public List<string> DirNames = new List<string>();
//        public List<string> FileNames = new List<string>();
//        public List<string> FileDirecrtory = new List<string>();
//        public List<string> FilenamesForSearch = new List<string>();
//        public List<string> TempFileNames = new List<string>();
//        public List<string> TempFileDirecrtory = new List<string>();

//        public List<int> IndexOF = new List<int>();

//        public string DirectoryFilename { get; set; }
//        public string FileFullPath { get; set; }
//        public string FileNameOnly { get; set; }
//        public string XMLFileName { get; set; }
//        public string XMLFileFullPath { get; set; }
//        public string CompanyName { get; set; }

//        public const string rootDirForXMLFiles = @"\\Netserver3\DATA\ShipmentsXMLfiles"; // the path is used for taking the files

//        public string File1FullPath = string.Empty,
//                        File2FullPath = string.Empty,
//                        File1Name = string.Empty,
//                        File2Name = string.Empty,
//                        File1NameTrimmed = string.Empty,
//                        FilePathOfXMLtemp,
//                        ExportXlSXfilePath;
//        #endregion Declaration

//        #region Constructor Empty inputs
//        public RootDirectoriesExplorer() { }
//        #endregion Constructor Empty inputs

//        #region GET Directory
//        public void DirectoriesExplorer([Optional] string rootD, [Optional] string formatTolookUP)
//        {
//            if (string.IsNullOrEmpty(rootD))
//                rootD = rootDirForXMLFiles;
//            if (string.IsNullOrEmpty(formatTolookUP))
//                formatTolookUP = "*.xml";
//            // Get a list of all subdirectories
//            try
//            {
//                var dirs = from dir in
//               Directory.EnumerateDirectories(rootD)
//                           select dir;

//                foreach (var dir in dirs)
//                {
//                    this.DirNames.Add(dir.Substring(dir.LastIndexOf("\\") + 1));

//                    DateTime lastupdated = DateTime.MinValue;
//                    DirectoryFilename = rootD + "\\" + dir.Substring(dir.LastIndexOf("\\") + 1);
//                    string Folder = DirectoryFilename;

//                    var files = new DirectoryInfo(Folder).GetFiles(formatTolookUP);//*.*//"*.xml"
//                }
//            }
//            catch { }
//        }
//        #endregion GET Directory

//        #region dataBaseFind
//        public void DataBaseFinder(string DataBaseName, [Optional] string AlternativeDataBasename)
//        {
//            if (!string.IsNullOrEmpty(AlternativeDataBasename))
//                DataBaseName = AlternativeDataBasename;
//            FilenamesForSearch.Clear();
//            IEnumerable<string> matchingList;

//            if (!string.IsNullOrEmpty(DataBaseName))
//            {
//                matchingList = databaseList.Where(x => x.ToUpper().Contains(DataBaseName.ToUpper()));
//                if (matchingList != null)
//                {
//                    FilenamesForSearch = matchingList.ToList();
//                }
//            }
//        }
//        #endregion dataBaseFind
//    }
//}
