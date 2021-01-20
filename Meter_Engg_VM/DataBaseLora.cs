using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace Meter_Engg_VM
{
    public static class Utilities
    {
        #region Check For Null String

        public static string CheckForNullString(string s)
        {
            string str = (s == null) ? string.Empty : s;
            return str;
        }

        #endregion Check For Null String
    }


    public class Program_SQL
    {
        public int number = 0;
        string MessageFromDatabase;

        #region Properties

        //public List<string> DevUI { get; set; }

        #endregion Properties

        
        private const string dbUsername = "power";              //make the database changes her and they will be reflected in all function
        private const string dbPassword = "power";
        private const string dbNetworkServerName = @"Netserver3;";
        //public string dbNameofDatabase;
        

        public string ConnectionStringBuilder(string Database)
        {
            string dbNameOfDatabase;
            //Form F1 = new Form();
            //dbNameOfDatabase = DatabaseChange ? "ReturnsVision" : "LoraVision";
            dbNameOfDatabase = Database;
            string connectionstring = "Server=" + dbNetworkServerName + " Database=" + dbNameOfDatabase + "; User=" + dbUsername + "; Password=" + dbPassword + ";";
            return connectionstring;
        }
        


        #region GrabADatabasewithdevEUI
        public string GrabADatabaseWithDevEUI(string devui,string columnToAccessFromDB,string Database)
        {
            
            string query = "select dbo.Meter."+columnToAccessFromDB+" from dbo.Meter where dbo.Meter.DevEUI =" + "'" + devui + "'"; //Batch, MeterID
            DataTable dt = SQLManager.ExecuteQuery(query, ConnectionStringBuilder(Database));
            if (dt.Rows.Count <= 0)
                return "No data";
            foreach (DataRow dr in dt.Rows){MessageFromDatabase = Utilities.CheckForNullString(SQLManager.CheckForNull<string>(dr[columnToAccessFromDB]));}
            
            return MessageFromDatabase;
        }
        #endregion GrabADatabasewithdevEUI



        #region GrabADatabaseWithMeterID
        public string GrabADatabaseWithMeterID(string MeterID, string columnToAccessFromDB, string Database) // meterId to check the MAr already exists or not
        {
            
            string query = "select dbo.Meter." + columnToAccessFromDB + " from dbo.Meter where dbo.Meter.MeterID =" + "'" + MeterID + "'"; //Batch, MeterID

            DataTable dt = SQLManager.ExecuteQuery(query, ConnectionStringBuilder(Database));

            //if (dt.Rows.Count <= 0)
            //    return "No data";

            foreach (DataRow dr in dt.Rows)
            {
                MessageFromDatabase = Utilities.CheckForNullString(SQLManager.CheckForNull<string>(dr[columnToAccessFromDB]));
            }
            return MessageFromDatabase;
        }
        #endregion GrabADatabaseWithMeterID



        #region PostDataToAMRCheck

        public string PostDataToAMRCheck(string MeterId, DateTime date, string initialofUser, string AppKey, string Database, string FwVersion, string TxPower, string FreqChannels, string AppEUI)//for AMR check
        {
            try
            {
                string query = "UPDATE dbo.Meter set AMRchkBy = " + "'" + initialofUser + "'" + ",AMRchkDate = " + "'" + date + "'" + ",AppKey = " + "'" + AppKey + "'" + ",AppEUI = " + "'" + AppEUI + "'" + ",FreqChannels = " + "'" + FreqChannels + "'" + ",TxPower = " + "'" + TxPower + "'" + ",ModemFirmwareRev = " + "'" + FwVersion + "'" + "where MeterID =" + "'" + MeterId + "'";
                DataTable dt = SQLManager.ExecuteQuery(query, ConnectionStringBuilder(Database));

                foreach (DataRow dr in dt.Rows)
                {
                    MessageFromDatabase = Utilities.CheckForNullString(SQLManager.CheckForNull<string>(dr[MeterId]));
                }
                return MessageFromDatabase;
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + Environment.NewLine +
                    e.StackTrace + Environment.NewLine + Environment.NewLine +
                    e.Source,
                    "Program Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
            }
        }


        //public string PostDataToAMRCheck(string MeterId, DateTime date, string initialofUser,string AppKey, bool DatabaseChange, string FwVersion,string TxPower,string FreqChannels,string AppEUI)//for AMR check
        //{
        //    string query = "UPDATE dbo.Meter set AMRchkBy = " + "'" + initialofUser + "'" + ",AMRchkDate = " + "'" + date + "'" +",AppKey = "+ "'" + AppKey + "'" +",AppEUI = "+ "'" + AppEUI + "'" +",FreqChannels = "+ "'" + FreqChannels + "'" +",TxPower = "+ "'" + TxPower + "'"+",ModemFirmwareRev = "+ "'" + FwVersion + "'" +"where MeterID =" + "'" + MeterId + "'";
        //    DataTable dt = SQLManager.ExecuteQuery(query, ConnectionStringBuilder(DatabaseChange));

        //    //UPDATE dbo.Meter
        //    //set AMRchkBy = 'SC',AMRchkDate = ''
        //    //where MeterID ='12345678'

        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        MessageFromDatabase = Utilities.CheckForNullString(SQLManager.CheckForNull<string>(dr[MeterId]));
        //    }
        //    return MessageFromDatabase;
        //}
        #endregion PostDataToAMRCheck
    }
}


/*
* 
*       //string DemoString = DevUI[number];
        //string connectionstring = "Server=" + @".\SQLExpress;" + " Database=SampleDB; User=sa; Password=visionmetering;";
        // string connectionstring = "Server=" + @"Netserver3;" + " Database=LoraVision; User=power; Password=power;";
        //string query = "select * from dbo.Meter";


        //string connectionstring = "Server=" + @"Netserver3;" + " Database=LoraVision; User=power; Password=power;";
        //public string GrabADatabase(string devui)
        //{
        //    string meterID = "MeterID";
        //    string query = @"select dbo.meter.meterid, dbo.meter.DevEUI from dbo.Meter where dbo.meter.DevEUI =" + "'" + devui + "'";

        //    DataTable dt = SQLManager.ExecuteQuery(query, connectionstring);


        //    if (dt.Rows.Count <= 0)
        //        return "No data";

        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        meterID = Utilities.CheckForNullString(SQLManager.CheckForNull<string>(dr["MeterID"]));
        //    }
        //    return meterID;
        //}

        //public static string dbNameOfDatabase = "ReturnsVision";
       
        //public static string dbNameofDatabase = "LoraVision";

        //string connectionstring = "Server=" + dbNetworkServerName + " Database="+ dbNameofDatabase + "; User="+ dbUsername + "; Password="+ dbPassword + ";";



*/