using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Meter_Engg_VM
{
    #region SQLManager Class

    public static class SQLManager
    {
        #region Variables

        public static bool SQLExpressProblem = false;

        public static bool MeterTestHistoryTable = false;
        public static bool MeterTestCustomTable = false;
        public static bool MeterKVARTestTable = false;
        public static bool UserLogTable = false;
        public static int MaximumCustomFields = 0;
        public static bool PCBlot = false;
        public static bool VestaVersion = false;
        public static bool VestaFirmware = false;
        public static bool VestaScript = false;

        public static bool AFKW = false;                    // FlorenceDB
        public static bool ALKW = false;                    // FlorenceDB
        public static bool AFKWH = false;                   // OzarksDB / FlorenceDB
        public static bool ALKWH = false;                   // OzarksDB / FlorenceDB
        public static bool ModuleKWH = false;               // OzarksDB / FlorenceDB
        public static bool CompanyNo = false;               // FlorenceDB

        public static bool AFKWMeterTest = false;           // CarrollDB
        public static bool ALKWMeterTest = false;           // CarrollDB
        public static bool AFKWHMeterTest = false;          // CarrollDB
        public static bool ALKWHMeterTest = false;          // CarrollDB
        public static bool AFKVARMeterTest = false;         // CarrollDB
        public static bool ALKVARMeterTest = false;         // CarrollDB
        public static bool StatusCodeMeterTest = false;     // CarrollDB

        public static bool VestaDatabase = false;
        public static bool StatisticsTable = false;
        public static string VestaStatisticsTableName = "Stats";

        //public static string VestaConnectionString = @"Server=.\SQLExpress; Database=VestaDB; Integrated Security=yes; Uid=auth_windows;";
        public static string VestaConnectionString = @"Server=.\SQLExpress; Database=VestaDB; UId=sa; Password=visionmetering;";

        #endregion Variables

        #region Check For Null

        public static T CheckForNull<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default(T); // returns the default value for the type
            }
            else
            {
                return (T)obj;
            }
        }

        public static string CheckForNull(object obj)
        {
            if (!DBNull.Value.Equals(obj))
                return (string)obj;
            else return string.Empty;
        }

        #endregion Check For Null

        #region Load Parameter

        public static void LoadParameter(SqlCommand command, string field, SqlDbType type, object value)
        {
            command.Parameters.Add(field, type);
            command.Parameters[field].Value = value;

            //VestaDLL.DebugManager.UpdateLog("SQLManager.LoadParameter", "field=" + field + " value=" + value.ToString());
        }

        #endregion Load Parameter

        #region Execute Query



        //public static DataTable ExecuteQuery(string query)
        //{
        //    DataTable table = new DataTable();

        //    if (SQLManager.User.ConnectionString == string.Empty)
        //        SQLManager.User.SetConnectionString(SQLManager.User.Server, SQLManager.User.Database, SQLManager.User.DBCredentials.UserID, SQLManager.User.DBCredentials.Password);

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(SQLManager.User.ConnectionString))
        //        {
        //            using (SqlCommand cmd = new SqlCommand(query, conn))
        //            {
        //                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //                conn.Open();

        //                adapter.Fill(table);

        //                conn.Close();
        //            }
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        Utilities.ShowMessageBox(
        //            ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace,
        //            "Program Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }

        //    return table;
        //}

        public static DataTable ExecuteQuery(string query, string connectionString)
        {
            DataTable table = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        conn.Open();

                        adapter.Fill(table);

                        conn.Close();
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace,
                    "Program Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return table;
        }

        #endregion Execute Query

        #region Find Table

        //private static bool FindTable(string tableName)
        //{
        //    //+VestaDLL.DebugManager.UpdateLog("Enter FindTable");

        //    int count = 0;

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(StatusInformation.User.ConnectionString))
        //        {
        //            conn.Open();

        //            string[] columnRestrictions = new string[4];

        //            // For the array,
        //            //    0-member represents Catalog;
        //            //    1-member represents Schema;
        //            //    2-member represents Table Name;
        //            //    3-member represents Column Name.

        //            // Now we specify the Table_Name and Column_Name of the columns what we want to get schema information.
        //            columnRestrictions[2] = tableName;

        //            DataTable schemaTable = conn.GetSchema("Columns", columnRestrictions);

        //            var selectedRows =
        //                from info in schemaTable.AsEnumerable()
        //                select new
        //                {
        //                    TableCatalog = info["TABLE_CATALOG"],
        //                    TableSchema = info["TABLE_SCHEMA"],
        //                    TableName = info["TABLE_NAME"],
        //                    ColumnName = info["COLUMN_NAME"],
        //                    DataType = info["DATA_TYPE"]
        //                };

        //            foreach (var row in selectedRows)
        //                count++;

        //            conn.Close();
        //        }

        //        //+VestaDLL.DebugManager.UpdateLog("FindTable", "count=" + count);
        //        //+VestaDLL.DebugManager.UpdateLog("Leave FindTable");

        //        if (count > 0)
        //            return true;
        //        else
        //            return false;
        //    }

        //    catch (Exception ex)
        //    {
        //        Utilities.ShowMessageBox(
        //            ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace,
        //            "Program Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //        //+VestaDLL.DebugManager.UpdateLog("FindMeterTestHistoryTable", ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace);
        //        //+VestaDLL.DebugManager.UpdateLog("Leave FindMeterTestHistoryTable");

        //        return false;
        //    }
        //}

        private static bool FindTable(string tableName, string connectionString)
        {
            //+VestaDLL.DebugManager.UpdateLog("Enter FindTable");

            int count = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string[] columnRestrictions = new string[4];

                    // For the array,
                    //    0-member represents Catalog;
                    //    1-member represents Schema;
                    //    2-member represents Table Name;
                    //    3-member represents Column Name.

                    // Now we specify the Table_Name and Column_Name of the columns what we want to get schema information.
                    columnRestrictions[2] = tableName;

                    DataTable schemaTable = conn.GetSchema("Columns", columnRestrictions);

                    var selectedRows =
                        from info in schemaTable.AsEnumerable()
                        select new
                        {
                            TableCatalog = info["TABLE_CATALOG"],
                            TableSchema = info["TABLE_SCHEMA"],
                            TableName = info["TABLE_NAME"],
                            ColumnName = info["COLUMN_NAME"],
                            DataType = info["DATA_TYPE"]
                        };

                    foreach (var row in selectedRows)
                        count++;

                    conn.Close();
                }

                //+VestaDLL.DebugManager.UpdateLog("FindTable", "count=" + count);
                //+VestaDLL.DebugManager.UpdateLog("Leave FindTable");

                if (count > 0)
                    return true;
                else
                    return false;
            }

            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace,
                    "Program Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //+VestaDLL.DebugManager.UpdateLog("FindMeterTestHistoryTable", ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace);
                //+VestaDLL.DebugManager.UpdateLog("Leave FindMeterTestHistoryTable");

                return false;
            }
        }

        #endregion Find Table

        #region Find Field

        //private static bool FindField(string tableName, string fieldName)
        //{
        //    //+VestaDLL.DebugManager.UpdateLog("Enter FindField");

        //    //int count = 0;

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(StatusInformation.User.ConnectionString))
        //        {
        //            conn.Open();

        //            string[] columnRestrictions = new string[4];

        //            // For the array,
        //            //    0-member represents Catalog;
        //            //    1-member represents Schema;
        //            //    2-member represents Table Name;
        //            //    3-member represents Column Name.

        //            // Now we specify the Table_Name and Column_Name of the columns what we want to get schema information.
        //            columnRestrictions[2] = tableName;

        //            DataTable schemaTable = conn.GetSchema("Columns", columnRestrictions);

        //            var selectedRows =
        //                from info in schemaTable.AsEnumerable()
        //                select new
        //                {
        //                    TableCatalog = info["TABLE_CATALOG"],
        //                    TableSchema = info["TABLE_SCHEMA"],
        //                    TableName = info["TABLE_NAME"],
        //                    ColumnName = info["COLUMN_NAME"],
        //                    DataType = info["DATA_TYPE"]
        //                };

        //            foreach (var row in selectedRows)
        //            {
        //                string columnName = row.ColumnName.ToString();

        //                if (columnName == fieldName)
        //                {
        //                    //+VestaDLL.DebugManager.UpdateLog("FindField", "'PCBlot' found");
        //                    //+VestaDLL.DebugManager.UpdateLog("Leave FindField");
        //                    return true;
        //                }
        //            }

        //            conn.Close();
        //        }

        //        //+VestaDLL.DebugManager.UpdateLog("Leave FindField");
        //        return false;
        //    }

        //    catch (Exception ex)
        //    {
        //        Utilities.ShowMessageBox(
        //            ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace,
        //            "Program Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //        //+VestaDLL.DebugManager.UpdateLog("FindField", ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace);
        //        //+VestaDLL.DebugManager.UpdateLog("Leave FindField");

        //        return false;
        //    }
        //}

        //private static bool FindTable(string tableName)
        //{
        //    //+VestaDLL.DebugManager.UpdateLog("Enter FindTable");

        //    int count = 0;

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(SQLManager.User.ConnectionString))
        //        {
        //            conn.Open();

        //            string[] columnRestrictions = new string[4];

        //            // For the array,
        //            //    0-member represents Catalog;
        //            //    1-member represents Schema;
        //            //    2-member represents Table Name;
        //            //    3-member represents Column Name.

        //            // Now we specify the Table_Name and Column_Name of the columns what we want to get schema information.
        //            columnRestrictions[2] = tableName;

        //            DataTable schemaTable = conn.GetSchema("Columns", columnRestrictions);

        //            var selectedRows =
        //                from info in schemaTable.AsEnumerable()
        //                select new
        //                {
        //                    TableCatalog = info["TABLE_CATALOG"],
        //                    TableSchema = info["TABLE_SCHEMA"],
        //                    TableName = info["TABLE_NAME"],
        //                    ColumnName = info["COLUMN_NAME"],
        //                    DataType = info["DATA_TYPE"]
        //                };

        //            foreach (var row in selectedRows)
        //                count++;

        //            conn.Close();
        //        }

        //        //+VestaDLL.DebugManager.UpdateLog("FindTable", "count=" + count);
        //        //+VestaDLL.DebugManager.UpdateLog("Leave FindTable");

        //        if (count > 0)
        //            return true;
        //        else
        //            return false;
        //    }

        //    catch (Exception ex)
        //    {
        //        Utilities.ShowMessageBox(
        //            ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace,
        //            "Program Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //        //+VestaDLL.DebugManager.UpdateLog("FindMeterTestHistoryTable", ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace);
        //        //+VestaDLL.DebugManager.UpdateLog("Leave FindMeterTestHistoryTable");

        //        return false;
        //    }
        //}

        //private static bool FindTable(string tableName, string connectionString)
        //{
        //    //+VestaDLL.DebugManager.UpdateLog("Enter FindTable");

        //    int count = 0;

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();

        //            string[] columnRestrictions = new string[4];

        //            // For the array,
        //            //    0-member represents Catalog;
        //            //    1-member represents Schema;
        //            //    2-member represents Table Name;
        //            //    3-member represents Column Name.

        //            // Now we specify the Table_Name and Column_Name of the columns what we want to get schema information.
        //            columnRestrictions[2] = tableName;

        //            DataTable schemaTable = conn.GetSchema("Columns", columnRestrictions);

        //            var selectedRows =
        //                from info in schemaTable.AsEnumerable()
        //                select new
        //                {
        //                    TableCatalog = info["TABLE_CATALOG"],
        //                    TableSchema = info["TABLE_SCHEMA"],
        //                    TableName = info["TABLE_NAME"],
        //                    ColumnName = info["COLUMN_NAME"],
        //                    DataType = info["DATA_TYPE"]
        //                };

        //            foreach (var row in selectedRows)
        //                count++;

        //            conn.Close();
        //        }

        //        //+VestaDLL.DebugManager.UpdateLog("FindTable", "count=" + count);
        //        //+VestaDLL.DebugManager.UpdateLog("Leave FindTable");

        //        if (count > 0)
        //            return true;
        //        else
        //            return false;
        //    }

        //    catch (Exception ex)
        //    {
        //        Utilities.ShowMessageBox(
        //            ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace,
        //            "Program Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //        //+VestaDLL.DebugManager.UpdateLog("FindMeterTestHistoryTable", ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace);
        //        //+VestaDLL.DebugManager.UpdateLog("Leave FindMeterTestHistoryTable");

        //        return false;
        //    }
        //}

        //#endregion Find Table

        //#region Find Field

        //private static bool FindField(string tableName, string fieldName)
        //{
        //    //+VestaDLL.DebugManager.UpdateLog("Enter FindField");

        //    //int count = 0;

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(SQLManager.User.ConnectionString))
        //        {
        //            conn.Open();

        //            string[] columnRestrictions = new string[4];

        //            // For the array,
        //            //    0-member represents Catalog;
        //            //    1-member represents Schema;
        //            //    2-member represents Table Name;
        //            //    3-member represents Column Name.

        //            // Now we specify the Table_Name and Column_Name of the columns what we want to get schema information.
        //            columnRestrictions[2] = tableName;

        //            DataTable schemaTable = conn.GetSchema("Columns", columnRestrictions);

        //            var selectedRows =
        //                from info in schemaTable.AsEnumerable()
        //                select new
        //                {
        //                    TableCatalog = info["TABLE_CATALOG"],
        //                    TableSchema = info["TABLE_SCHEMA"],
        //                    TableName = info["TABLE_NAME"],
        //                    ColumnName = info["COLUMN_NAME"],
        //                    DataType = info["DATA_TYPE"]
        //                };

        //            foreach (var row in selectedRows)
        //            {
        //                string columnName = row.ColumnName.ToString();

        //                if (columnName == fieldName)
        //                {
        //                    //+VestaDLL.DebugManager.UpdateLog("FindField", "'PCBlot' found");
        //                    //+VestaDLL.DebugManager.UpdateLog("Leave FindField");
        //                    return true;
        //                }
        //            }

        //            conn.Close();
        //        }

        //        //+VestaDLL.DebugManager.UpdateLog("Leave FindField");
        //        return false;
        //    }

        //    catch (Exception ex)
        //    {
        //        Utilities.ShowMessageBox(
        //            ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace,
        //            "Program Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //        //+VestaDLL.DebugManager.UpdateLog("FindField", ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace);
        //        //+VestaDLL.DebugManager.UpdateLog("Leave FindField");

        //        return false;
        //    }
        //}

        #endregion Find Field

        #region Count Custom Fields

        //private static int CountCustomFields()
        //{
        //    //+VestaDLL.DebugManager.UpdateLog("Enter CountCustomFields");

        //    int count = 0;

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(StatusInformation.User.ConnectionString))
        //        {
        //            conn.Open();

        //            string[] columnRestrictions = new string[4];

        //            // For the array,
        //            //    0-member represents Catalog;
        //            //    1-member represents Schema;
        //            //    2-member represents Table Name;
        //            //    3-member represents Column Name.

        //            // Now we specify the Table_Name and Column_Name of the columns what we want to get schema information.
        //            columnRestrictions[2] = "MeterTest";

        //            DataTable schemaTable = conn.GetSchema("Columns", columnRestrictions);

        //            var selectedRows = from info in schemaTable.AsEnumerable()
        //                               select new
        //                               {
        //                                   TableCatalog = info["TABLE_CATALOG"],
        //                                   TableSchema = info["TABLE_SCHEMA"],
        //                                   TableName = info["TABLE_NAME"],
        //                                   ColumnName = info["COLUMN_NAME"],
        //                                   DataType = info["DATA_TYPE"]
        //                               };

        //            string columnName = string.Empty;

        //            foreach (var row in selectedRows)
        //            {
        //                columnName = row.ColumnName.ToString();

        //                if (columnName.Length == 10)
        //                {
        //                    if (columnName.Substring(0, 4) != "Test")
        //                        continue;

        //                    if (columnName.Substring(6, 4) != "Type")
        //                        continue;

        //                    count++;
        //                }
        //            }

        //            conn.Close();
        //        }

        //        //+VestaDLL.DebugManager.UpdateLog("CountCustomFields", "count=" + count);
        //        //+VestaDLL.DebugManager.UpdateLog("Leave CountCustomFields");

        //        return count;
        //    }

        //    catch (Exception ex)
        //    {
        //        Utilities.ShowMessageBox(
        //            ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace,
        //            "Program Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //        //+VestaDLL.DebugManager.UpdateLog("CountCustomFields", ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace);
        //        //+VestaDLL.DebugManager.UpdateLog("Leave CountCustomFields");

        //        return 0;
        //    }
        //}

        //private static int CountCustomFields()
        //{
        //    //+VestaDLL.DebugManager.UpdateLog("Enter CountCustomFields");

        //    int count = 0;

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(SQLManager.User.ConnectionString))
        //        {
        //            conn.Open();

        //            string[] columnRestrictions = new string[4];

        //            // For the array,
        //            //    0-member represents Catalog;
        //            //    1-member represents Schema;
        //            //    2-member represents Table Name;
        //            //    3-member represents Column Name.

        //            // Now we specify the Table_Name and Column_Name of the columns what we want to get schema information.
        //            columnRestrictions[2] = "MeterTest";

        //            DataTable schemaTable = conn.GetSchema("Columns", columnRestrictions);

        //            var selectedRows = from info in schemaTable.AsEnumerable()
        //                               select new
        //                               {
        //                                   TableCatalog = info["TABLE_CATALOG"],
        //                                   TableSchema = info["TABLE_SCHEMA"],
        //                                   TableName = info["TABLE_NAME"],
        //                                   ColumnName = info["COLUMN_NAME"],
        //                                   DataType = info["DATA_TYPE"]
        //                               };

        //            string columnName = string.Empty;

        //            foreach (var row in selectedRows)
        //            {
        //                columnName = row.ColumnName.ToString();

        //                if (columnName.Length == 10)
        //                {
        //                    if (columnName.Substring(0, 4) != "Test")
        //                        continue;

        //                    if (columnName.Substring(6, 4) != "Type")
        //                        continue;

        //                    count++;
        //                }
        //            }

        //            conn.Close();
        //        }

        //        //+VestaDLL.DebugManager.UpdateLog("CountCustomFields", "count=" + count);
        //        //+VestaDLL.DebugManager.UpdateLog("Leave CountCustomFields");

        //        return count;
        //    }

        //    catch (Exception ex)
        //    {
        //        Utilities.ShowMessageBox(
        //            ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace,
        //            "Program Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //        //+VestaDLL.DebugManager.UpdateLog("CountCustomFields", ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace);
        //        //+VestaDLL.DebugManager.UpdateLog("Leave CountCustomFields");

        //        return 0;
        //    }
        //}

        #endregion Count Custom Fields

        #region Read Database Configuration

        //public static void ReadDatabaseConfiguration()
        //{
        //    VestaDLL.DebugManager.UpdateLog("SQLManager", "Enter ReadDatabaseConfiguration");

        //    SQLManager.MeterTestHistoryTable = FindTable("MeterTestHistory");
        //    SQLManager.MeterTestCustomTable = FindTable("MeterTestCustom");
        //    SQLManager.MeterKVARTestTable = FindTable("MeterKVARTest");
        //    SQLManager.UserLogTable = FindTable("UserLog");

        //    SQLManager.MaximumCustomFields = CountCustomFields();

        //    SQLManager.PCBlot = FindField("Meter", "PCBlot");
        //    SQLManager.VestaVersion = FindField("MeterTest", "VestaVersion");
        //    SQLManager.VestaFirmware = FindField("MeterTest", "VestaFirmware");
        //    SQLManager.VestaScript = FindField("Meter", "Vesta_Script");

        //    // OzarksDB / FlorenceDB
        //    SQLManager.AFKWH = FindField("Meter", "AFKWH");
        //    SQLManager.ALKWH = FindField("Meter", "ALKWH");
        //    SQLManager.ModuleKWH = FindField("Meter", "ModuleKWH");

        //    // FlorenceDB
        //    SQLManager.AFKW = FindField("Meter", "AFKW");
        //    SQLManager.ALKW = FindField("Meter", "ALKW");
        //    SQLManager.CompanyNo = FindField("Meter", "CompanyNo");

        //    // CarrollDB
        //    // AFKW
        //    SQLManager.AFKWMeterTest = FindField("MeterTest", "AFKW");
        //    VestaDLL.DebugManager.UpdateLog("SQLManager", "AFKWMeterTest=" + SQLManager.AFKWMeterTest);

        //    // ALKW
        //    SQLManager.ALKWMeterTest = FindField("MeterTest", "ALKW");
        //    VestaDLL.DebugManager.UpdateLog("SQLManager", "ALKWMeterTest=" + SQLManager.ALKWMeterTest);

        //    // AFKWH
        //    SQLManager.AFKWHMeterTest = FindField("MeterTest", "AFKWH");
        //    VestaDLL.DebugManager.UpdateLog("SQLManager", "AFKWHMeterTest=" + SQLManager.AFKWHMeterTest);

        //    // ALKWH
        //    SQLManager.ALKWHMeterTest = FindField("MeterTest", "ALKWH");
        //    VestaDLL.DebugManager.UpdateLog("SQLManager", "ALKWHMeterTest=" + SQLManager.ALKWHMeterTest);

        //    // AFKVAR
        //    SQLManager.AFKVARMeterTest = FindField("MeterTest", "AFKVAR");
        //    VestaDLL.DebugManager.UpdateLog("SQLManager", "AFKVARMeterTest=" + SQLManager.AFKVARMeterTest);

        //    // ALKVAR
        //    SQLManager.ALKVARMeterTest = FindField("MeterTest", "ALKVAR");
        //    VestaDLL.DebugManager.UpdateLog("SQLManager", "ALKVARMeterTest=" + SQLManager.ALKVARMeterTest);

        //    // StatusCode
        //    SQLManager.StatusCodeMeterTest = FindField("MeterTest", "StatusCode");
        //    VestaDLL.DebugManager.UpdateLog("SQLManager", "StatusCodeMeterTest=" + SQLManager.StatusCodeMeterTest);

        //    StatusInformation.TestReasonList = PopulateTestReasonsList();

        //    StatusInformation.NoteList = PopulateNoteList();

        //    //if (Utilities.StringNotEquals(status.SiteID, Noun.Vision))
        //    //{
        //    //    if (SQLManager.MeterTestCustomTable)
        //    //    {
        //    //        if (!Directory.Exists(folders.SQLScriptsFolder))
        //    //            Directory.CreateDirectory(folders.SQLScriptsFolder);

        //    //        string path = Path.Combine(folders.SQLScriptsFolder, "Create Drop MeterTestCustom Table.sql");

        //    //        GenerateDropTableMeterTestCustom(SQLManager.User.Database
        //    //            , SQLManager.User.DBOwner, path);

        //    //        RunSqlScriptFile(path, SQLManager.User.ConnectionString);

        //    //        SQLManager.MeterTestCustomTable = false;
        //    //    }
        //    //}

        //    VestaDLL.DebugManager.UpdateLog("SQLManager", "Leave ReadDatabaseConfiguration");
        //}

        #endregion Read Database Configuration

        #region Get Databases

        //public static DataTable GetDatabases(string connectionstring)
        //{
        //    DataTable dt;

        //    using (SqlConnection conn = new SqlConnection(connectionstring))
        //    {
        //        conn.Open();

        //        // Get the schema information of Databases in your instance
        //        dt = conn.GetSchema("Databases");
        //    }

        //    return dt;
        //}

        //public static DataTable GetDatabases()
        //{
        //    DataTable dt;

        //    using (SqlConnection conn = new SqlConnection(SQLManager.User.ConnectionString))
        //    {
        //        conn.Open();

        //        // Get the schema information of Databases in your instance
        //        dt = conn.GetSchema("Databases");
        //    }

        //    return dt;
        //}

        public static DataTable GetDatabases(string connectionString)
        {
            DataTable dt;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Get the schema information of Databases in your instance
                dt = conn.GetSchema("Databases");
            }

            return dt;
        }

        #endregion Get Databases

        #region Load SQL Credentials

        //public static Credentials LoadSQLCredentials(string fileName)
        //{
        //    Credentials credentials = new Credentials();

        //    if (File.Exists(fileName))
        //    {
        //        try
        //        {
        //            // Load the Credentials object from the XML file using our custom class...
        //            credentials = ObjectXMLSerializer<Credentials>.Load(fileName);
        //        }

        //        catch
        //        {
        //            Utilities.ShowMessageBox(
        //                "Unable to load SQL credentials from file: " + Environment.NewLine + Environment.NewLine + "'" + fileName + "'",
        //                AssemblyInfo.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }

        //        if (credentials == null)
        //        {
        //            Utilities.ShowMessageBox(
        //                "Unable to load SQL credentials from file: " + Environment.NewLine + Environment.NewLine + "'" + fileName + "'",
        //                AssemblyInfo.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }

        //    return credentials;
        //}

        #endregion Load SQL Credentials

        #region Save SQL Credentials

        //public static void SaveSQLCredentials(FoldersTable folders, string fileName, Credentials credentials, bool showMessage)
        //{
        //    string fullFileName = Path.Combine(folders.VestaFolder, fileName);

        //    // save credentials object to XML file using our ObjectXMLSerializer class...
        //    try
        //    {
        //        ObjectXMLSerializer<Credentials>.Save(credentials, fullFileName);

        //        VestaDLL.Utilities.GrantAccess(fullFileName);

        //        if (showMessage)
        //        {
        //            Forms.UpdateRichTextBox(FrontPage.RichTextBoxDisplay, Forms.MessageType.Updated, "Credentials saved to: '" + fullFileName + "'", false);

        //            //Utilities.ShowMessageBox(
        //            //    "Configuration saved to: " + Environment.NewLine + Environment.NewLine + "'" + fileName + "'",
        //            //    AssemblyInfo.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        //Forms.UpdateRichTextBox(FrontPage.richTextBoxDisplay, Forms.MessageType.Error, "Unable to save configuration to: '" + fileName + "'", false);

        //        Utilities.ShowMessageBox(
        //            "Unable to save credentials to: " + Environment.NewLine + Environment.NewLine + "'" + fullFileName + "'" +
        //            Environment.NewLine + Environment.NewLine +
        //            ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace,
        //            AssemblyInfo.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        #endregion Save SQL Credentials

        #region Populate Test Reasons List

        //public static List<CodeTextElement> PopulateTestReasonsList()
        //{
        //    string query = "SELECT * FROM " + StatusInformation.User.DBOwner + ".TestReason";
        //    VestaDLL.DebugManager.UpdateLog("SQLManager (PopulateTestReasonsList)", "Query=" + query);

        //    DataTable dt = SQLManager.ExecuteQuery(query);
        //    //+VestaDLL.DebugManager.UpdateLog("SQLManager (PopulateTestReasonsList)", "Rows returned=" + dt.Rows.Count.ToString());

        //    if (dt.Rows.Count == 0)
        //        return null;

        //    List<CodeTextElement> list = new List<CodeTextElement>();

        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        string code = Utilities.CheckForNullString(SQLManager.CheckForNull<string>(dr["TestReasonCode"]));
        //        string text = Utilities.CheckForNullString(SQLManager.CheckForNull<string>(dr["Text"]));

        //        //VestaDLL.DebugManager.UpdateLog("Login (PopulateTestReasonsList)", "code=" + code);
        //        //VestaDLL.DebugManager.UpdateLog("Login (PopulateTestReasonsList)", "text=" + text);

        //        list.Add(new CodeTextElement(code, text));
        //    }

        //    return list;
        //}

        #endregion Populate Test Reasons List

        #region Populate Note List

        //public static List<CodeTextElement> PopulateNoteList()
        //{
        //    string query = "SELECT * FROM " + StatusInformation.User.DBOwner + ".Note";
        //    VestaDLL.DebugManager.UpdateLog("SQLManager (PopulateNoteList)", "Query=" + query);

        //    DataTable dt = SQLManager.ExecuteQuery(query);
        //    //+VestaDLL.DebugManager.UpdateLog("SQLManager (PopulateNoteList)", "Rows returned=" + dt.Rows.Count.ToString());

        //    if (dt.Rows.Count == 0)
        //        return null;

        //    List<CodeTextElement> list = new List<CodeTextElement>();

        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        string code = Utilities.CheckForNullString(SQLManager.CheckForNull<string>(dr["NoteCode"]));
        //        string text = Utilities.CheckForNullString(SQLManager.CheckForNull<string>(dr["Text"]));

        //        //VestaDLL.DebugManager.UpdateLog("Login (PopulateNoteList)", "code=" + code);
        //        //VestaDLL.DebugManager.UpdateLog("Login (PopulateNoteList)", "text=" + text);

        //        list.Add(new CodeTextElement(code, text));
        //    }

        //    return list;
        //}

        #endregion Populate Note List

        #region Create Script File

        //private static void CreateScriptFile(List<string> list, string path)
        //{
        //    using (FileStream strm = File.Create(path))
        //    using (StreamWriter sw = new StreamWriter(strm))
        //    {
        //        foreach (string s in list)
        //            sw.WriteLine(s);
        //    }
        //}

        #endregion Create Script File

        #region Execute Script File

        //private static bool RunSqlScriptFile(string pathStoreProceduresFile, string connectionString)
        //{
        //    try
        //    {
        //        string script = File.ReadAllText(pathStoreProceduresFile);

        //        // split script on GO command
        //        System.Collections.Generic.IEnumerable<string> commandStrings = Regex.Split(script, @"^\s*GO\s*$",
        //                                 RegexOptions.Multiline | RegexOptions.IgnoreCase);

        //        //using (SqlConnection connection = new SqlConnection(SQLManager.User.ConnectionString))
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();
        //            foreach (string commandString in commandStrings)
        //            {
        //                if (commandString.Trim() != "")
        //                {
        //                    using (var command = new SqlCommand(commandString, connection))
        //                    {
        //                        try
        //                        {
        //                            command.ExecuteNonQuery();
        //                        }

        //                        catch (SqlException ex)
        //                        {
        //                            string spError = commandString.Length > 100 ? commandString.Substring(0, 100) + " ...\n..." : commandString;

        //                            MessageBox.Show(string.Format("Please check the SqlServer script.\nFile: {0} \nLine: {1} \nError: {2} \nSQL Command: \n{3}", pathStoreProceduresFile, ex.LineNumber, ex.Message, spError), 
        //                                "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //                            return false;
        //                        }
        //                    }
        //                }
        //            }
        //            connection.Close();
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return false;
        //    }
        //}

        #endregion Execute Script File

        #region Generate Create Database VestDB

        //private static void GenerateCreateDatabaseVestaDB(string databaseName, string owner, string path)
        //{
        //    List<string> list = new List<string>();
        //    string mdfFilename = @"C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\";
        //    string ldfFilename = @"C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\";

        //    list.Add("USE [master]");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("CREATE DATABASE [" + databaseName + "]");
        //    list.Add(" CONTAINMENT = NONE");
        //    list.Add(" ON  PRIMARY");
        //    list.Add("( NAME = N'VestaDB', FILENAME = N'" + mdfFilename + databaseName + ".mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )");
        //    list.Add(" LOG ON");
        //    list.Add("( NAME = N'VestaDB_log', FILENAME = N'" + ldfFilename + databaseName + "_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET COMPATIBILITY_LEVEL = 120");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))");
        //    list.Add("begin");
        //    list.Add("EXEC [" + databaseName + "].[dbo].[sp_fulltext_database] @action = 'enable'");
        //    list.Add("end");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET ANSI_NULL_DEFAULT OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET ANSI_NULLS OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET ANSI_PADDING OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET ANSI_WARNINGS OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET ARITHABORT OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET AUTO_CLOSE OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET AUTO_SHRINK OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET AUTO_UPDATE_STATISTICS ON");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET CURSOR_CLOSE_ON_COMMIT OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET CURSOR_DEFAULT  GLOBAL");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET CONCAT_NULL_YIELDS_NULL OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET NUMERIC_ROUNDABORT OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET QUOTED_IDENTIFIER OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET RECURSIVE_TRIGGERS OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET  DISABLE_BROKER");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET AUTO_UPDATE_STATISTICS_ASYNC OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET DATE_CORRELATION_OPTIMIZATION OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET TRUSTWORTHY OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET ALLOW_SNAPSHOT_ISOLATION OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET PARAMETERIZATION SIMPLE");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET READ_COMMITTED_SNAPSHOT OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET HONOR_BROKER_PRIORITY OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET RECOVERY SIMPLE");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET  MULTI_USER");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET PAGE_VERIFY CHECKSUM");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET DB_CHAINING OFF");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF )");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET TARGET_RECOVERY_TIME = 0 SECONDS");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET DELAYED_DURABILITY = DISABLED");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("ALTER DATABASE [" + databaseName + "] SET  READ_WRITE");
        //    list.Add("GO");

        //    CreateScriptFile(list, path);
        //}

        #endregion Generate Create Database Vesta DB

        #region Generate Create Table Statistics

        //private static void GenerateCreateTableCounters(string databaseName, string owner, string path)
        //{
        //    List<string> list = new List<string>();

        //    list.Add("USE [" + databaseName + "]");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("SET ANSI_NULLS ON");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("SET QUOTED_IDENTIFIER ON");
        //    list.Add("GO");
        //    list.Add(" ");

        //    //list.Add("SET ANSI_PADDING ON");
        //    //list.Add("GO");
        //    //list.Add(" ");

        //    //list.Add("CREATE TABLE [" + owner + "].[Counters](");
        //    list.Add("CREATE TABLE [" + owner + "].[" + SQLManager.VestaStatisticsTableName + "](");
        //    list.Add("    [Scans] [int] NOT NULL,");
        //    list.Add("    [Saves] [int] NOT NULL,");
        //    list.Add("    [Retires] [int] NOT NULL,");
        //    list.Add("    [Tests] [int] NOT NULL,");
        //    list.Add("    [Pass] [int] NOT NULL,");
        //    list.Add("    [Fail] [int] NOT NULL,");

        //    list.Add("    [SFLTests] [int] NOT NULL,");
        //    list.Add("    [SFLPass] [int] NOT NULL,");
        //    list.Add("    [SFLFail] [int] NOT NULL,");
        //    list.Add("    [SPFTests] [int] NOT NULL,");
        //    list.Add("    [SPFPass] [int] NOT NULL,");
        //    list.Add("    [SPFFail] [int] NOT NULL,");
        //    list.Add("    [SLLTests] [int] NOT NULL,");
        //    list.Add("    [SLLPass] [int] NOT NULL,");
        //    list.Add("    [SLLFail] [int] NOT NULL,");

        //    list.Add("    [AFLTests] [int] NOT NULL,");
        //    list.Add("    [AFLPass] [int] NOT NULL,");
        //    list.Add("    [AFLFail] [int] NOT NULL,");
        //    list.Add("    [APFTests] [int] NOT NULL,");
        //    list.Add("    [APFPass] [int] NOT NULL,");
        //    list.Add("    [APFFail] [int] NOT NULL,");
        //    list.Add("    [ALLTests] [int] NOT NULL,");
        //    list.Add("    [ALLPass] [int] NOT NULL,");
        //    list.Add("    [ALLFail] [int] NOT NULL,");

        //    list.Add("    [BFLTests] [int] NOT NULL,");
        //    list.Add("    [BFLPass] [int] NOT NULL,");
        //    list.Add("    [BFLFail] [int] NOT NULL,");
        //    list.Add("    [BPFTests] [int] NOT NULL,");
        //    list.Add("    [BPFPass] [int] NOT NULL,");
        //    list.Add("    [BPFFail] [int] NOT NULL,");
        //    list.Add("    [BLLTests] [int] NOT NULL,");
        //    list.Add("    [BLLPass] [int] NOT NULL,");
        //    list.Add("    [BLLFail] [int] NOT NULL,");

        //    list.Add("    [CFLTests] [int] NOT NULL,");
        //    list.Add("    [CFLPass] [int] NOT NULL,");
        //    list.Add("    [CFLFail] [int] NOT NULL,");
        //    list.Add("    [CPFTests] [int] NOT NULL,");
        //    list.Add("    [CPFPass] [int] NOT NULL,");
        //    list.Add("    [CPFFail] [int] NOT NULL,");
        //    list.Add("    [CLLTests] [int] NOT NULL,");
        //    list.Add("    [CLLPass] [int] NOT NULL,");
        //    list.Add("    [CLLFail] [int] NOT NULL,");

        //    list.Add("    [MessagesSent] [int] NOT NULL,");
        //    list.Add("    [MessagesReceived] [int] NOT NULL,");
        //    list.Add("    [SendErrors] [int] NOT NULL,");
        //    list.Add("    [ReceiveErrors] [int] NOT NULL");

        //    list.Add(") ON [PRIMARY]");
        //    list.Add(" ");

        //    list.Add("GO");

        //    CreateScriptFile(list, path);
        //}

        #endregion Generate Create Table Statistics

        #region Generate Create Table MeterTestHistory

        //private static void GenerateCreateTableMeterTestHistory(string databaseName, string owner, string path)
        //{
        //    List<string> list = new List<string>();

        //    list.Add("USE [" + databaseName + "]");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("SET ANSI_NULLS ON");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("SET QUOTED_IDENTIFIER ON");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("SET ANSI_PADDING ON");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("CREATE TABLE [" + owner + "].[MeterTestHistory](");
        //    list.Add("    [ID] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,");
        //    list.Add("    [MeterID] [varchar](20) NOT NULL,");
        //    list.Add("    [TestName] [varchar](3) NOT NULL,");
        //    list.Add("    [TestDate] [datetime] NOT NULL,");
        //    list.Add("    [TestResult] [float] NOT NULL,");
        //    list.Add("    [TestType] [varchar](10) NOT NULL,");
        //    list.Add("    [Pass] [varchar](5) NULL,");
        //    list.Add("    [Form] [varchar](5) NULL,");
        //    list.Add("    [MeterTypeCode] [varchar](10) NULL,");
        //    list.Add("    [AVolts] [varchar](10) NULL,");
        //    list.Add("    [AVoltsPhaseAngle] [varchar](10) NULL,");
        //    list.Add("    [AAmps] [varchar](10) NULL,");
        //    list.Add("    [AAmpsPhaseAngle] [varchar](10) NULL,");
        //    list.Add("    [BVolts] [varchar](10) NULL,");
        //    list.Add("    [BVoltsPhaseAngle] [varchar](10) NULL,");
        //    list.Add("    [BAmps] [varchar](10) NULL,");
        //    list.Add("    [BAmpsPhaseAngle] [varchar](10) NULL,");
        //    list.Add("    [CVolts] [varchar](10) NULL,");
        //    list.Add("    [CVoltsPhaseAngle] [varchar](10) NULL,");
        //    list.Add("    [CAmps] [varchar](10) NULL,");
        //    list.Add("    [CAmpsPhaseAngle] [varchar](10) NULL,");
        //    list.Add("    [PhaseAngle] [varchar](10) NULL,");
        //    list.Add("    [Frequency] [varchar](10) NULL,");
        //    list.Add("    [KH] [varchar](10) NULL,");
        //    list.Add("    [UserID] [varchar](10) NULL,");
        //    list.Add("    [BoardID] [varchar](10) NULL,");
        //    list.Add("    [TestReasonCode] [varchar](5) NULL,");
        //    list.Add("    [Phase] [varchar](5) NULL,");
        //    list.Add("    [Direction] [varchar](5) NULL,");
        //    list.Add("    [LimitsLow] [float] NULL,");
        //    list.Add("    [LimitsHigh] [float] NULL,");
        //    list.Add("    [Duration] [int] NULL,");
        //    list.Add("    [Pulses] [int] NULL,");
        //    list.Add("    [VestaVersion] [varchar](20) NULL,");
        //    list.Add("    [FirmwareVersion] [varchar](20) NULL,");
        //    list.Add("    [PKWH] [int] NULL,");
        //    list.Add("    [PulseCount] [int] NULL,");
        //    list.Add(") ON [PRIMARY]");
        //    list.Add(" ");

        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("SET ANSI_NULLS OFF");
        //    list.Add("GO");

        //    CreateScriptFile(list, path);
        //}

        #endregion Generate Create Table MeterTestHistory

        #region Generate Table UserLog

        //private static void GenerateCreateTableUserLog(string databaseName, string owner, string path)
        //{
        //    List<string> list = new List<string>();

        //    list.Add("USE [" + databaseName + "]");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("SET ANSI_NULLS ON");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("SET QUOTED_IDENTIFIER ON");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("SET ANSI_PADDING ON");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("CREATE TABLE [" + owner + "].[UserLog](");
        //    list.Add("    [ID] [int] IDENTITY(1,1) NOT NULL,");
        //    list.Add("    [Timestamp] [datetime] NOT NULL,");
        //    list.Add("    [UserID] [varchar](10) NOT NULL,");
        //    list.Add("    [Action] [varchar](20) NOT NULL,");
        //    list.Add("    [Server] [varchar](30) NULL,");
        //    list.Add("    [DB] [varchar](50) NULL,");

        //    list.Add("PRIMARY KEY CLUSTERED");
        //    list.Add("(");
        //    list.Add("    [ID]ASC");
        //    list.Add(")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
        //    list.Add(") ON [PRIMARY]");
        //    list.Add(" ");

        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("SET ANSI_PADDING OFF");
        //    list.Add("GO");

        //    CreateScriptFile(list, path);
        //}

        #endregion Generate Table UserLog

        #region Generate Drop Table MeterTestCustom

        //private static void GenerateDropTableMeterTestCustom(string databaseName, string owner, string path)
        //{
        //    List<string> list = new List<string>();

        //    list.Add("USE [" + databaseName + "]");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("DROP TABLE [" + owner + "].[MeterTestCustom]");
        //    list.Add("GO");

        //    CreateScriptFile(list, path);
        //}

        #endregion Generate Drop Table MeterTestCustom

        #region Generate Create Table MeterTestCustom

        //private static void GenerateCreateTableMeterTestCustom(string databaseName, string owner, string path)
        //{
        //    List<string> list = new List<string>();

        //    list.Add("USE [" + databaseName + "]");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("SET ANSI_NULLS ON");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("SET QUOTED_IDENTIFIER ON");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("SET ANSI_PADDING ON");
        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("CREATE TABLE [" + owner + "].[MeterTestCustom](");
        //    list.Add("    [TestID] [int] IDENTITY(1,1) NOT NULL,");
        //    list.Add("    [MeterID] [varchar](20) NOT NULL,");
        //    list.Add("    [TestName] [varchar](3) NOT NULL,");
        //    list.Add("    [Type] [varchar](2) NULL,");
        //    list.Add("    [TestDate] [datetime] NOT NULL,");
        //    list.Add("    [TestResult] [float] NOT NULL,");
        //    list.Add("    [TestType] [varchar](10) NOT NULL,");
        //    list.Add("    [AVolts] [varchar](10) NULL,");
        //    list.Add("    [AVoltsPhaseAngle] [varchar](10) NULL,");
        //    list.Add("    [AAmps] [varchar](10) NULL,");
        //    list.Add("    [AAmpsPhaseAngle] [varchar](10) NULL,");
        //    list.Add("    [BVolts] [varchar](10) NULL,");
        //    list.Add("    [BVoltsPhaseAngle] [varchar](10) NULL,");
        //    list.Add("    [BAmps] [varchar](10) NULL,");
        //    list.Add("    [BAmpsPhaseAngle] [varchar](10) NULL,");
        //    list.Add("    [CVolts] [varchar](10) NULL,");
        //    list.Add("    [CVoltsPhaseAngle] [varchar](10) NULL,");
        //    list.Add("    [CAmps] [varchar](10) NULL,");
        //    list.Add("    [CAmpsPhaseAngle] [varchar](10) NULL,");
        //    list.Add("    [PhaseAngle] [varchar](10) NULL,");
        //    list.Add("    [Frequency] [varchar](10) NULL,");
        //    list.Add("    [KH] [varchar](10) NULL,");
        //    list.Add("    [Pulses] [int] NULL,");
        //    list.Add("    [TestTime] [int] NULL,");
        //    list.Add("    [PKWH] [int] NULL,");
        //    list.Add("    [PulseCount] [int] NULL,");
        //    list.Add("CONSTRAINT [PK_MeterTestCustom] PRIMARY KEY CLUSTERED ");
        //    list.Add("(");
        //    list.Add("    [TestID] ASC");
        //    list.Add(")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
        //    list.Add(") ON [PRIMARY]");
        //    list.Add(" ");

        //    list.Add("GO");
        //    list.Add(" ");

        //    list.Add("SET ANSI_NULLS OFF");
        //    list.Add("GO");

        //    CreateScriptFile(list, path);
        //}

        #endregion Generate Create Table MeterTestCustom

        #region Check For Database

        //private static int CheckForDatabase(string database, string connectionString, ref string errorMessage)
        //{
        //    // returns true if Vesta Database is present
        //    // returns false if not

        //    try
        //    {
        //        //string connectionString = @"Server=.\SQLExpress; Database=master; Integrated Security=yes; Uid=auth_windows;";

        //        DataTable dt = SQLManager.GetDatabases(connectionString);

        //        foreach (DataRow row in dt.Rows)
        //        {
        //            string str = row[0].ToString();

        //            if (Utilities.StringEquals(str, database))
        //                return 1;
        //        }

        //        return 0;
        //    }

        //    catch (SqlException e)
        //    {
        //        SQLManager.SQLExpressProblem = true;

        //        errorMessage = e.Message + Environment.NewLine + Environment.NewLine + e.Source + Environment.NewLine + Environment.NewLine + e.StackTrace;

        //        return -1;
        //    }

        //    catch (Exception e)
        //    {
        //        SQLManager.SQLExpressProblem = true;

        //        errorMessage = e.Message + Environment.NewLine + Environment.NewLine + e.Source + Environment.NewLine + Environment.NewLine + e.StackTrace;

        //        return -2;
        //    }
        //}

        #endregion Check For Database

        #region Database Manager

        //public static StatisticsSpecification DatabaseManager(FoldersTable folders)
        //{
        //    StatisticsSpecification stats = null;
        //    string errorMessage = string.Empty;
        //    //string connectionString = @"Server=.\SQLExpress; Database=master; Integrated Security=yes; Uid=auth_windows;";
        //    string connectionString = @"Server=.\SQLExpress; Database=master; UId=sa; Password=visionmetering;";

        //    // look up VestaDB
        //    int result = SQLManager.CheckForDatabase(Noun.VestaDB, connectionString, ref errorMessage);

        //    if (result < 0)
        //    {
        //        Utilities.ShowMessageBox(errorMessage, "Program Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //        return new StatisticsSpecification();
        //    }

        //    if (result == 0)
        //    {
        //        if (!Directory.Exists(folders.SQLScriptsFolder))
        //            Directory.CreateDirectory(folders.SQLScriptsFolder);

        //        string path = Path.Combine(folders.SQLScriptsFolder, "Create VestaDB Database.sql");

        //        GenerateCreateDatabaseVestaDB(Noun.VestaDB, "dbo", path);

        //        RunSqlScriptFile(path, connectionString);
        //    }

        //    SQLManager.VestaDatabase = true;


        //    // look up Counters table in VestaDB
        //    SQLManager.StatisticsTable = SQLManager.FindTable(SQLManager.VestaStatisticsTableName, VestaConnectionString);

        //    if (SQLManager.StatisticsTable)
        //        stats = StatisticsManager.Load("dbo", SQLManager.VestaConnectionString);
        //    else
        //    {
        //        if (!Directory.Exists(folders.SQLScriptsFolder))
        //            Directory.CreateDirectory(folders.SQLScriptsFolder);

        //        string fileName = "Create " + SQLManager.VestaStatisticsTableName + " Table.sql";
        //        string path = Path.Combine(folders.SQLScriptsFolder, fileName);

        //        GenerateCreateTableCounters(Noun.VestaDB, "dbo", path);

        //        RunSqlScriptFile(path, VestaConnectionString);

        //        SQLManager.StatisticsTable = true;

        //        stats = StatisticsManager.Load(folders);
        //        StatisticsManager.InsertStatisticsRecord(stats, "dbo", SQLManager.VestaConnectionString);

        //        StatisticsManager.Remove(folders);
        //    }

        //    return stats;
        //}

        #endregion Database Manager
    }

    #endregion SQLManager Class
}