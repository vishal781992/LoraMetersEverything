using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SQLTest
{
    public static class AssemblyInfo
    {
        #region AssemblyTitle

        public static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }

                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        #endregion AssemblyTitle

        #region AssemblyDescription

        public static string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }

                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        #endregion AssemblyDescription

        #region AssemblyCompany

        public static string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }

                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        #endregion AssemblyCompany

        #region AssemblyVersion

        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        #endregion AssemblyVersion

        #region AssemblyCopyright

        public static string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }

                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        #endregion AssemblyCopyright

        #region Executing Assembly

        public static System.Reflection.Assembly ExecutingAssembly
        {
            get { return executingAssembly ?? (executingAssembly = System.Reflection.Assembly.GetExecutingAssembly()); }
        }
        private static System.Reflection.Assembly executingAssembly;

        #endregion Executing Assembly

        #region Executing Assembly Version

        public static System.Version ExecutingAssemblyVersion
        {
            get { return executingAssemblyVersion ?? (executingAssemblyVersion = ExecutingAssembly.GetName().Version); }
        }
        private static System.Version executingAssemblyVersion;

        #endregion Executing Assembly Version

        #region Compile Date

        public static System.DateTime CompileDate
        {
            get
            {
                if (!compileDate.HasValue)
                    compileDate = RetrieveLinkerTimestamp(ExecutingAssembly.Location);
                return compileDate ?? new System.DateTime();
            }
        }
        private static System.DateTime? compileDate;

        #endregion Compile Date

        #region Retrieve Linker Timestamp

        // http://www.codinghorror.com/blog/2005/04/determining-build-date-the-hard-way.html
        private static System.DateTime RetrieveLinkerTimestamp(string filePath)
        {
            const int peHeaderOffset = 60;
            const int linkerTimestampOffset = 8;
            var b = new byte[2048];
            System.IO.FileStream s = null;
            try
            {
                s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                    s.Close();
            }
            var dt = new System.DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(System.BitConverter.ToInt32(b, System.BitConverter.ToInt32(b, peHeaderOffset) + linkerTimestampOffset));
            return dt.AddHours(System.TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
            //var dateUtcKind = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            //return dateUtcKind;
        }

        #endregion Retrieve Linker Timestamp
    }
}