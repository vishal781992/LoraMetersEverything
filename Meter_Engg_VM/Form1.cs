#define DebugTest

using System;
using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
//using Microsoft.WindowsAPICodePack.Dialogs;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Windows.Forms.VisualStyles;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
//using System.Threading.Tasks;
//using System.Net.NetworkInformation;
//using System.Security.Policy;
//using Microsoft.VisualBasic.ApplicationServices;
//using System.Diagnostics;
//using System.Reflection;
//using System.Windows.Forms;


namespace Meter_Engg_VM
{
    //dont make a class here as this is a form.

    public partial class Form1 : Form
    {
        const string ApplicationVersionnumber = "01.00.K";
        const string DiscriptionTextForChange =
            "Added manual DataBase Select.";
        
        #region Declaration
        #region Declaration of list
        List<string> DevUI = new List<string>();                    //list declarations, All
        List<string> EndsightDevUI = new List<string>();            //endsight
        List<string> AppKey = new List<string>();
        List<string> CustVer = new List<string>();
        List<string> FwVersion = new List<string>();
        List<string> MeterID = new List<string>();
        List<string> EndsightMeterID = new List<string>();          //endsight
        List<string> BatchID = new List<string>();
        List<string> filenameArray = new List<string>();
        List<string> TimeOfprogrammingModems = new List<string>();
        List<string> DateOfprogrammingModems = new List<string>();
        List<string> EndsightStatus = new List<string>();           //endsight
        List<int> strToIntDateOPM = new List<int>();
        List<int> strToIntTimeOPM = new List<int>();
        List<int> MeterID_int = new List<int>();
        List<string> globalCounterForEndsight = new List<string>();
        
        //List<string> AMRCheckMeterID = new List<string>();



        #endregion Declaration of list

        #region Declaration of Arrays
        string[] TempArrayForDisplayContent = new string[10];       //for displaying the table
        public string[] Duplicates = new string[700];
        string[] displayConfigArray = new string[1];
        string[] AlreadyPresentDevUI = new string[500];
        string[] tempAryforDuplicates = new string[30];
        string[] statusCode = new string[600];
        string[] tempEndMeterID = new string[600];
        string[] Ary_DataFromTextFile = new string[2000];

        #endregion Declaration of Arrays

        #region Declaration of other variables
        ListViewItem itm,itm2;
        struct StatusCodesFromChirpStackAPI { public int LoginStatus; public int AddingNewDeviceStatus; public int AddingApplicationStatus; public int SearchingExistingApplication; };
        StatusCodesFromChirpStackAPI SCFlag = new StatusCodesFromChirpStackAPI(); public ReturnList rL = new ReturnList();
        Program_SQL psql = new Program_SQL();
        public enum Profile { UserCTime=1,SystemCTime=2};
        #endregion Declaration of other variables

        #region Declaration of Global Strings
        
        string FileInputDir = @"\\Netserver3\data\Loraproduction_Engineering\";// @"C:\MetershopApplicationFolder\InputFile\";//@"C:\Output\";  //change the input here
        const string FileOutgoingDir = @"\\Netserver3\data\Loraproduction_Engineering\" + @"MetershopApplicationFolder\OutputFile\";// @"C:\temp\";
        const string FileOutputForErrorSheets_tab2 = @"\\Netserver3\data\Loraproduction_Engineering\Amrfile\";//"C:\2020_08_26\ErrorModemsfile.csv";
        const string pathOfLogFile = @"C:\Project\readmeCacheVM\CacheFile.csv";
        string dboMeterID = "meterid";  //used in the database access function.
        string dboBatchID = "Batch";
        string strFilename, strFilename1;
        public string dataincomingfromDataBase;
        int CounterForCorrectDataSet;//string path;
        int CounterForUpdatedMetersToDB;
        string Endsight_File1FullPath; string Endsight_File1Name = string.Empty;
        public string Database;
        public bool FlagForNextProcesses=true;
        bool incrementProcessFlag = false;
        #endregion Declaration of Global Strings

        #region Declaration for API
        HttpClient client = new HttpClient();               //api http client
        ChiprstackAPI chirpS = new ChiprstackAPI();         //chripstack class defination
        #endregion Declaration for API

        #region Constructors
        public Form1()
        {
            InitializeComponent();
        }

        #endregion Constructors
        #endregion Declaration

        #region Form_Load

        private void Form1_Load(object sender, EventArgs e)
        {
            
            richTextBox1.Text = "Welcome Back!\r\nVerify the IP address you are using for Chirpstack before you press Login!\r\nBig files take longer time to process than usual, wait before clicking on any  other button after you press FILE GENERATOR.\r\n"+ DiscriptionTextForChange;
            MessageBox.Show("Welcome Back!\r\nVersion "+ ApplicationVersionnumber + " " + DiscriptionTextForChange);
            //richTextBox1.AppendText("Version 01.00.F.Removed the endsight manual file select, now it automatically selects the Latest File from the collection.")
            //TextBox_Username.Text = "admin";
            DateTime YearOnly = DateTime.Now; string dateFormatYY_MM_ = YearOnly.ToString("yyyy_MM_");
            TextBox_UserDateInput.Text = dateFormatYY_MM_;


            this.buttonChirpStack.Enabled = true;
            bool demo = listView.CanSelect;
            TextBox_UserDateInput.Focus();

            //Tooltip init.
            MergeHelp.SetToolTip(Button_Merge, "Merge the Files for Set Date.");
            CustIDHelp.SetToolTip(TextBox_CustID, "Enter the Customer ID as a Filter.");
            ChrpSHelp.SetToolTip(buttonChirpStack, "Check everything before you Hit this button.");
            toolTip_UpdateAllMeters.SetToolTip(checkBox_UpdateAllMeters, "This will UPDATE all the meters despite of any condition.");


            InitialServerInput();
            Array.Clear(Ary_DataFromTextFile,0,Ary_DataFromTextFile.Length);
            groupBox_meterRange.Visible = false; richTextBox_Temp.Visible = false;
            //puts the last used Server Address to the ComboBox
        }

        #endregion Form_Load

        #region Tab1 Functions

        #region Buttons

        #region SelectFilePress
      public void SelectButtonReplacement()
        {
            this.progressBar_Universal.Step = 5;
            Database = comboBox_DB.Text;
            richTextBox1.AppendText("\r\nTalking to Database.");

            string dateFormatYY_MM_dd = GetDateTimeFromUser((int)Profile.UserCTime);
            try
            {
                #region trying the automated file transfer
                this.progressBar_Universal.PerformStep(); //progress bar

                strFilename = FileOutgoingDir + dateFormatYY_MM_dd + @"\merge" + dateFormatYY_MM_dd + ".csv"; //MessageBox.Show("Uploading the File from the Default location" + strFilename);

                if (strFilename != null)//textBox1.Text
                {
                    string checkCsv = ".csv";
                    string checkProductionFile = "Prod&Database";
                    bool boolProductionFileCheckResponse = strFilename.Contains(checkProductionFile); bool boolCsvCheckResponse = strFilename.Contains(checkCsv);
                    this.progressBar_Universal.PerformStep(); //progress bar
                    if (!boolProductionFileCheckResponse && boolCsvCheckResponse)
                    {
                        //FileProcessFunction();
                        FileParsingFunction(strFilename, 0);
                        this.progressBar_Universal.PerformStep();
                    }else { MessageBox.Show("Production File will not be processed"); }
                    /*
                        *this function does the file parsing
                        *FileParsingFunction(strFilename,0);
                    */
                }
                #endregion trying the automated file transfer
                richTextBox1.AppendText("\r\nTalking to Database--Complete");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.progressBar_Universal.Value = 0; //progress bar
            }

        }
        #endregion SelectFilePress

        #region DisplayContentPress
        public void DisplayButtonReplacement()
        {
            /*this function is only use to organize data to display it on the Listview1. no data is processed here. */
            richTextBox1.AppendText("\r\nDisplaying the Content above.");
            try
            {
                for (int count = 0; count < DevUI.Count; count++)
                {
                    TempArrayForDisplayContent[0] = count + ".";   //the columns are designed above
                    TempArrayForDisplayContent[1] = DevUI[count];
                    TempArrayForDisplayContent[2] = AppKey[count];
                    TempArrayForDisplayContent[3] = CustVer[count];
                    TempArrayForDisplayContent[4] = FwVersion[count];
                    TempArrayForDisplayContent[5] = MeterID[count];
                    TempArrayForDisplayContent[6] = Duplicates[count];
                    TempArrayForDisplayContent[7] = BatchID[count];  //added


                    itm = new ListViewItem(TempArrayForDisplayContent);
                    listView.Items.Add(itm);
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion DisplayContentPress

        #region ExportFilePress

        public void ExportButtonReplacement()
        {
            this.progressBar_Universal.Step = 5;
            string dateFormatYY_MM_dd = GetDateTimeFromUser((int)Profile.UserCTime);
            int FileCounter = 1;
            string ExportFileName = "Prod&Database_" + dateFormatYY_MM_dd + "_" + FileCounter + ".csv";  //filename can be updated here
            string path = FileOutgoingDir + dateFormatYY_MM_dd + @"\" + ExportFileName;                  //address + Filename
            
            while (File.Exists(path))
            {
                FileCounter++;
                ExportFileName = "Prod&Database_" + dateFormatYY_MM_dd + "_" + FileCounter + ".csv";
                path = FileOutgoingDir + dateFormatYY_MM_dd + @"\" + ExportFileName;
            }
            try//try for file directory search
            {
                if (!File.Exists(path))
                {
                    TempArrayForDisplayContent[1] = "DevEUI";
                    TempArrayForDisplayContent[2] = "AppKey";
                    TempArrayForDisplayContent[3] = "Cust.Ver";
                    TempArrayForDisplayContent[4] = "Firm.Ver";
                    TempArrayForDisplayContent[5] = "MeterID";
                    TempArrayForDisplayContent[6] = "Duplicates";
                    TempArrayForDisplayContent[7] = "BatchID";
                    displayConfigArray[0] = TempArrayForDisplayContent[1] + "," + TempArrayForDisplayContent[2] + "," + TempArrayForDisplayContent[3] + "," + TempArrayForDisplayContent[4] + "," + TempArrayForDisplayContent[5] + "," + TempArrayForDisplayContent[6] + "," + TempArrayForDisplayContent[7];
                    File.AppendAllLines(path, displayConfigArray);

                    for (int count = 0; count < DevUI.Count; count++)
                    {
                        this.progressBar_Universal.PerformStep();
                        TempArrayForDisplayContent[1] = DevUI[count];
                        TempArrayForDisplayContent[2] = AppKey[count];
                        TempArrayForDisplayContent[3] = CustVer[count];
                        TempArrayForDisplayContent[4] = FwVersion[count];
                        TempArrayForDisplayContent[5] = MeterID[count];
                        TempArrayForDisplayContent[6] = Duplicates[count];
                        TempArrayForDisplayContent[7] = BatchID[count];

                        displayConfigArray[0] = TempArrayForDisplayContent[1] + "," + TempArrayForDisplayContent[2] + "," + TempArrayForDisplayContent[3] + "," + TempArrayForDisplayContent[4] + "," + TempArrayForDisplayContent[5] + "," + TempArrayForDisplayContent[6] + "," + TempArrayForDisplayContent[7];
                        File.AppendAllLines(path, displayConfigArray);
                    }
                }
                richTextBox1.AppendText("\r\nFile exported to: " + path + "\r\n**text can be copied from here using Ctrl+C.");
                comboBoxServerSelect.Focus();
            }
            catch (Exception ex)    //catch for file directory search
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion ExportFilePress

        #region MergePress - this is the new modified start main button, triggers all sequencially.
        private void button4_Merge(object sender, EventArgs e)                      //this function helps to merge the csv files
        {
            ListClearFunction();
            this.progressBar_Universal.Step = 5;
            Database = comboBox_DB.Text;
            if(!Database.Contains("Select DataBase"))
            {
                string dateFormatYY_MM_dd = GetDateTimeFromUser((int)Profile.UserCTime);
                try
                {
                    //the FileInputDir Check the Textbox4 before it could store the default address string to strfilename1
                    richTextBox1.AppendText("Merge Button is pressed");
                    strFilename1 = FileInputDir;    //Defaulkt addr::  @"C:\MetershopApplicationFolder\InputFile\"
                    string Folder = strFilename1;   //textBox4_MergeFileinput.Text;
                    var files = new DirectoryInfo(Folder).GetFiles("*.csv");


                    foreach (FileInfo file in files)
                    {
                        if (file.Name.Contains(dateFormatYY_MM_dd))//file.LastWriteTime > lastupdated)
                        {
                            if (!filenameArray.Contains(file.Name))
                            {
                                filenameArray.Add(strFilename1 + file.Name);
                            }
                        }
                    }
                    int count = 0;

                    //the following function will help us to create the folder with date as a directory.
                    #region creating the directory in parent foilder with reference to the date
                    string OutputDir = FileOutgoingDir + dateFormatYY_MM_dd;
                    try
                    {
                        // Determine whether the directory exists.
                        if (Directory.Exists(OutputDir))
                        {
                            richTextBox1.Text = "The merge" + dateFormatYY_MM_dd + ".csv exists already.";
                            string message = "Do you want to Delete The File\r\nand Create a new File?";
                            string title = "Folder Delete";
                            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                            DialogResult result = MessageBox.Show(message, title, buttons);
                            if (result == DialogResult.Yes)
                            {
                                File.Delete(OutputDir + @"\merge" + dateFormatYY_MM_dd + ".csv");
                                richTextBox1.Text = "The directory is modified successfully at " + dateFormatYY_MM_dd + " \r\n**You can always select the text here\r\nand Hit Ctrl+C to copy.";
                            }
                            else
                            {
                                richTextBox1.Text = "the folder is not Deleted\r\nYou can always visit the " + FileOutgoingDir + "\\" + dateFormatYY_MM_dd + "\r\npath for view the Files.\r\n**You can always select the text here\r\nand Hit Ctrl+C to copy.";
                            }
                        }
                        else
                        {
                            DirectoryInfo di = Directory.CreateDirectory(OutputDir);
                            richTextBox1.Text = "The directory was created successfully at " + dateFormatYY_MM_dd + " \r\n**You can always select the text here\r\nand Hit Ctrl+C to copy.";

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    #endregion creating the directory in temp folder with reference to the date

                    #region function to append the whole set of similar data into 1 file before talking to database
                    while (count < filenameArray.Count)
                    {
                        string filex1 = File.ReadAllText(filenameArray[count]);
                        File.AppendAllText(FileOutgoingDir + dateFormatYY_MM_dd + @"\merge" + dateFormatYY_MM_dd + ".csv", filex1); count++;
                    }
                    #endregion function to append the whole set of similar data into 1 file before talking to database


                    string demo = File.ReadAllText(FileOutgoingDir + dateFormatYY_MM_dd + @"\merge" + dateFormatYY_MM_dd + ".csv");
                    if (demo.Contains("DevEUI,AppKey,CustVer,FwVer"))
                    {
                        //demo.Remove()
                    }
                }
                catch (Exception ex)
                {
                    richTextBox1.Text = "No file found for current Day,\r\nOnly " + dateFormatYY_MM_dd + " files will be considered for the Merge and Processing.\r\nAll the next processes are Halted.";
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    FlagForNextProcesses = false;  //changing the flag value, making the statement false.
                }
                if (FlagForNextProcesses)
                {
                    SelectButtonReplacement();
                    DisplayButtonReplacement();
                    ExportButtonReplacement();
                }
                FlagForNextProcesses = true; //resetting the value to its initial
                this.progressBar_Universal.Value = 100;
            }
            else
                MessageBox.Show("Select the Correct DataBase.");
        }
        #endregion MergePress

        #region SelectServerPress
        private void button8_ServerNameSelect(object sender, EventArgs e)
        {
            buttonLoginServer.BackColor = Color.Green;
            chirpS.ServerURL = comboBoxServerSelect.Text;
            if(!chirpS.ServerURL.Contains("Server"))
            {
                richTextBox1.Text = chirpS.ServerURL + " server is selected\r\nwith given user and Password\r\nPress CS Button to proceed";
                chirpS.emailAPI = TextBox_Username.Text;
                chirpS.passwordAPI = TextBox_Password.Text;
                richTextBox1.Text = chirpS.ServerURL + " server is selected\r\nwith given user and Password\r\nPress CS Button to proceed";
                this.progressBar_Universal.Value = 0;
                DateTime NowTime = DateTime.Now;
                if (File.Exists(@"C:\Program Files (x86)\Meter_Engg_VM\CacheFile.csv"))
                {
#if (DebugTest)
                        File.AppendAllText(pathOfLogFile, "\r\n**********************************************");
                        File.AppendAllText(pathOfLogFile, "\r\nTimeOfLog: " + NowTime);
                        File.AppendAllText(pathOfLogFile, "\r\nUSER_" + TextBox_Username.Text + "_");
                        File.AppendAllText(pathOfLogFile, "PASS_" + TextBox_Password.Text + "_");
                        File.AppendAllText(pathOfLogFile, "URL_" + comboBoxServerSelect.Text + "_\r\n");
#endif
                }
                else
                {
#if (DebugTest)
                    File.AppendAllText(pathOfLogFile, "\r\n**********************************************");
                    File.AppendAllText(pathOfLogFile, "\r\nTimeOfLog: " + NowTime);
                    File.AppendAllText(pathOfLogFile, "\r\nUSER_" + TextBox_Username.Text + "_");
                    File.AppendAllText(pathOfLogFile, "PASS_" + TextBox_Password.Text + "_");
                    File.AppendAllText(pathOfLogFile, "URL_" + comboBoxServerSelect.Text + "_\r\n");
#endif

                }
            }
            else
            {
                MessageBox.Show("Select Server", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
                        
        }
#endregion SelectServerPress

        #region HttpChirpStackPress

        /*
            * The button5_HttpApi when pressed takes Applciation Name Appln discription and date format with login credentials, and sedn those to the ChirpstackAPI.cs
            * in return to all the functions used below we in return receive a code which is either X0 or X1 were 0 is error and 1 is success.
            * if we receive error in any of the process the process is haulted and we receive the error message, not always the system error message but some readable comment on the error like "password incorect or login failed" meanwhile the progressBar1_CS will increment on all the necessary steps.
            */
        private async void button5_HttpApi(object sender, EventArgs e)              //its function talks to website or server to uplaod the meters //async
        {
            richTextBox1.Text = "Waiting for Chirpstack Response.\r\nDont press Any Button.\r\nIf this message is still here after 30 secs,\r\nthere is a problem with ChirpStack";   
            string dateFormatYY_MM_dd = GetDateTimeFromUser((int)Profile.UserCTime);
            string ApplicationName = TextBox_ApplicationName.Text;
            string ApplicationDiscp = TextBox_ApplicationDesp.Text;
            int NumberOfMetersUploadedCS = 0;
            this.progressBar_Universal.Value = 0;
            
            try
            {
                if (DevUI.Count != 0)
                {
                    SCFlag.LoginStatus = await chirpS.ChirPostLogin();                      //chirpStack Login
                                                                                            //ErrorLogin = 20, SuccessLogin = 21
                    if (SCFlag.LoginStatus == (int)ReturnList.SuccessLogin && ApplicationName != null)
                    {
                        TextBox_Username.BackColor = Color.Green; TextBox_Password.BackColor = Color.Green;
                        MessageBox.Show("Logged in on ChirpStack!");
                        richTextBox1.Clear();
                        try
                        {
                            SCFlag.SearchingExistingApplication = await chirpS.ChirpGetApplicationName(ApplicationName);
                            if (SCFlag.SearchingExistingApplication == (int)ReturnList.SuccessGettingApplication)
                            {
                                richTextBox1.Text = "The Application Name Already exists, Using Existing!"; this.progressBar_Universal.PerformStep();
                            }
                            else { SCFlag.AddingApplicationStatus = await chirpS.ChirpPostApplication(ApplicationName, ApplicationDiscp); this.progressBar_Universal.PerformStep(); }     //Chirpstack Application creation
                                                                                                                                                                                    //ErrorAddingApplication = 40, SuccessAddingApplication = 41
                            if (SCFlag.AddingApplicationStatus == (int)ReturnList.SuccessAddingApplication || SCFlag.SearchingExistingApplication == (int)ReturnList.SuccessGettingApplication)
                            {
                                richTextBox1.Text = "the Application ID is:: " + chirpS.ChirpPostApplID;
#if (DebugTest)
                                File.AppendAllText(pathOfLogFile, "\r\nApplicationID_" + chirpS.ChirpPostApplID + "_");
                                File.AppendAllText(pathOfLogFile, "\r\nApplicationName_" + TextBox_ApplicationName.Text + "_");
#endif
                                int TempText10 = int.Parse(TextBox_MeterMinRange.Text);
                                int TempText11 = int.Parse(TextBox_MeterMaxRange.Text);
                                for (int i = 0; i < DevUI.Count; i++)       //device upload routine
                                {
                                    this.progressBar_Universal.PerformStep();

                                    if (int.TryParse(MeterID[i], out int TempMeterID))
                                    {
                                        if (CustVer[i] == TextBox_CustID.Text && (TempText10 <= TempMeterID && TempMeterID <= TempText11)) //!MeterID[i].Contains("No data") &&    //custom customer version number requested to upload
                                        {
                                            //the above condition check if the meterIds are in bounds AND customer version is in bound AND if meter ID is Not blank.
                                            SCFlag.AddingNewDeviceStatus = await chirpS.ChirpPostNewDevice(DevUI[i], AppKey[i], CustVer[i], FwVersion[i], MeterID[i]);
                                            //ErrorAddingNewDevice = 10, SuccessAddingNewDevice = 11
                                            if (SCFlag.AddingNewDeviceStatus != (int)ReturnList.SuccessAddingNewDevice)
                                            {
                                                AlreadyPresentDevUI[i] = DevUI[i];
                                            }
                                            else { NumberOfMetersUploadedCS++; }
                                        }
                                    }
                                }
                                MessageBox.Show(NumberOfMetersUploadedCS + " Meters\r\nare uploaded to ChirpStack.");
#if (DebugTest)
                                File.AppendAllText(pathOfLogFile, "\r\nUpdatedTo_ChirpStack_" + NumberOfMetersUploadedCS + "_\r\n");
#endif
                                richTextBox1.AppendText("\r\nmeters Malfunctioned at:: ");
                                for (int counter = 0; counter < AlreadyPresentDevUI.Length; counter++)
                                {
                                    if (AlreadyPresentDevUI[counter] != null)
                                        richTextBox1.AppendText((counter+1) + ", ");
                                }
                                richTextBox1.AppendText("\r\nrefer File at:" + FileOutgoingDir + dateFormatYY_MM_dd);
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
#if (DebugTest)
                            File.AppendAllText(pathOfLogFile, "\r\n" + ex.Message);
#endif
                        }
                    }
                    else { richTextBox1.Text = "LOGIN Failed!\r\nTry again.\r\nYou may have missed AppliationName"; TextBox_Username.BackColor = Color.Red; TextBox_Password.BackColor = Color.Red; }
                }
            }
            catch(Exception)
            {
                MessageBox.Show("Check the Application Name and Discription and login details,\r\nand try again\r\nCheck the Server Connection.\r\nServer may be down!");richTextBox1.Clear();
#if (DebugTest)
                File.AppendAllText(pathOfLogFile, "\r\nCheck the Application Name and Discription and login details,\r\nand try again\r\nCheck the Server Connection.\r\nServer may be down!");
#endif
            }
            if(DevUI.Count == 0)
                MessageBox.Show("Seems Like The Input File is Empty.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            ListClearFunction();
#if (DebugTest)
            File.AppendAllText(pathOfLogFile, "\r\nChirpStackUpload Success at_"+ GetDateTimeFromUser(2));
#endif
        }
    #endregion HttpChirpStackPress

        #endregion Buttons

        #region Functions DuplicateFinder, FileProcess, Fileparse, timeTrimmer, GetDateTimeFromUser, SortingTheData, LatestFileSort

        class ErrorList
        {
            public string DefaultStatusCode = "DSC";
            public string DefaultMeterID_unmatched = "DMIU";

            //public const string 
        }
        public string LatestFileSort(string FileInputDir)
        {
            DateTime lastupdated = DateTime.MinValue;
            strFilename1 = FileInputDir;
            string Folder = strFilename1;

            var files = new DirectoryInfo(Folder).GetFiles("*.csv");

            foreach (FileInfo file in files)
            {
                if (file.LastWriteTime > lastupdated)
                {
                    lastupdated = file.LastWriteTime;
                    Endsight_File1FullPath = file.FullName;
                    Endsight_File1Name = file.Name;
                }
            }
            return Endsight_File1FullPath;
        }
        public void SortingTheData()
        {
            try
            {
                for (int reference = DevUI.Count - 1; reference > 0; reference--)
                {
                    //int numOfDuplicates = 1;
                    for (int comparingTo = DevUI.Count - 2; comparingTo >= 0; comparingTo--)
                    {

                        if (DevUI[reference] == DevUI[comparingTo])
                        {
                            if (strToIntDateOPM[reference] == strToIntDateOPM[comparingTo])
                            {
                                if (strToIntTimeOPM[reference] < strToIntTimeOPM[comparingTo])
                                {
                                    CustVer[reference] = CustVer[comparingTo];
                                    FwVersion[reference] = FwVersion[comparingTo];
                                }
                            }
                            else if (strToIntDateOPM[reference] < strToIntDateOPM[comparingTo])
                            {
                                CustVer[reference] = CustVer[comparingTo];
                                FwVersion[reference] = FwVersion[comparingTo];
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Duplicate Finded,\r\n(Index detected)\r\nDone\r\n");
                //MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void DuplicateFinder()                                   //this function helps find the duplicates in the file 
        {
            try
            {
                for (int reference = 0; reference < DevUI.Count - 1; reference++)//(int reference = DevUI.Count - 1; reference > 0; reference--)//
                {
                    int numOfDuplicates = 1;    //placing here makes it sure to reset for every element in the list to generate the number of duplicates Prog has deleted for user
                    for (int comparingTo = reference + 1; comparingTo < DevUI.Count; comparingTo++)  //(int comparingTo = reference + 1; comparingTo < DevUI.Count;comparingTo--)
                    {

                        if (DevUI[reference] == DevUI[comparingTo])
                        {
                            if(comparingTo==DevUI.Count-1) //this function helps to eliminate the last duplicate element without overflow
                            {

                                DevUI[reference] = DevUI[comparingTo];
                                AppKey[reference] = AppKey[comparingTo];
                                CustVer[reference] = CustVer[comparingTo];
                                FwVersion[reference] = FwVersion[comparingTo];
                                //MeterID[reference] = MeterID[comparingTo];

                                Duplicates[reference] = numOfDuplicates+ " Dupl. Deleted";
                                DevUI.RemoveAt(comparingTo);
                                AppKey.RemoveAt(comparingTo);
                                CustVer.RemoveAt(comparingTo);
                                FwVersion.RemoveAt(comparingTo);
                                //MeterID.RemoveAt(comparingTo);
                                ++numOfDuplicates;

                                break;
                            }
                            while (DevUI[reference] == DevUI[comparingTo])
                            {
                            
                                tempAryforDuplicates[0] = DevUI[comparingTo];
                                tempAryforDuplicates[1] = AppKey[comparingTo];
                                tempAryforDuplicates[2] = CustVer[comparingTo];
                                tempAryforDuplicates[3] = FwVersion[comparingTo];
                                //tempAryforDuplicates[4] = MeterID[comparingTo];

                                Duplicates[reference] = numOfDuplicates + " Dupl. Deleted";
                                DevUI.RemoveAt(comparingTo);
                                AppKey.RemoveAt(comparingTo);
                                CustVer.RemoveAt(comparingTo);
                                FwVersion.RemoveAt(comparingTo);
                                //MeterID.RemoveAt(comparingTo);
                                ++numOfDuplicates;
                            }
                            DevUI[reference] = tempAryforDuplicates[0];
                            AppKey[reference] = tempAryforDuplicates[1];
                            CustVer[reference] = tempAryforDuplicates[2];
                            FwVersion[reference] = tempAryforDuplicates[3];
                            //MeterID[reference] = tempAryforDuplicates[4];

                            //demoWatch_reference = reference;
                            //demoWatch_compareTo = comparingTo;

                        }
                    }
                }
            }
            catch(Exception)
            {
                MessageBox.Show("Duplicate Finded,\r\n(Index detected)\r\nDone\r\n");
                //MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void FileProcessFunction()                               //function helps to find the latest file in the folder directory
        {
            string Folder = strFilename;
            var files = new DirectoryInfo(Folder).GetFiles("*.*");
            //var files = strFilename;
            string latestfile = "";

            DateTime lastupdated = DateTime.MinValue;
            foreach(FileInfo file in files)
            {
                if(file.LastWriteTime > lastupdated)
                {
                    lastupdated = file.LastWriteTime;
                    latestfile = file.Name;
                }
            }

            strFilename = @"" + Folder +latestfile;
        }
        public void FileParsingFunction(string strFilename,int i)       //function is used to parse the data of the file and to include them into the respective lists mentioned in the declarations
        {
            listView.Columns.Add("DBn", 50, HorizontalAlignment.Center);
            listView.Columns.Add("DevEUI", 150, HorizontalAlignment.Center);
            listView.Columns.Add("AppKey", 100, HorizontalAlignment.Center);
            listView.Columns.Add("CustVer", 50, HorizontalAlignment.Center);
            listView.Columns.Add("FwVer", 50, HorizontalAlignment.Center);
            listView.Columns.Add("meterID", 100, HorizontalAlignment.Center);
            listView.Columns.Add("DuChck", 80, HorizontalAlignment.Center);
            listView.Columns.Add("BatchID", 80, HorizontalAlignment.Center);
            using (System.IO.File.OpenRead(strFilename))
            {

                if (File.Exists(strFilename))
                {
                    #region Adding Columns
                    
                    //dataGridView1.Columns[0].Name = "Release Date";
                    #endregion Adding Columns
                    #region Text Parsing function
                    using (TextFieldParser parser = new TextFieldParser(strFilename))
                    {
                        parser.TextFieldType = FieldType.Delimited;
                        parser.SetDelimiters(",");
                        //testfield = parser.ReadFields();

                        //int i = 0;
                        while (!parser.EndOfData)
                        {
                            #region Parsing the engg file
                            listView.Visible = true;
                            listView.View = View.Details;
                            string[] fields = parser.ReadFields();
                            #endregion Parsing the engg file

                            #region listing the Data
                            DevUI.Add(fields[0]);
                            AppKey.Add(fields[1]);
                            CustVer.Add(fields[2]);
                            FwVersion.Add(fields[3]);
                            TimeOfprogrammingModems.Add(fields[4]);
                            #endregion listing the Data

                            if (DevUI[i] == "DevEUI")
                            {
                                DevUI.RemoveAt(i);
                                AppKey.RemoveAt(i);
                                CustVer.RemoveAt(i);
                                FwVersion.RemoveAt(i);
                                TimeOfprogrammingModems.RemoveAt(i);
                            }
                            else { i++; }
                        }
                        #endregion Text Parsing function
                        #region Database connect and query for meterID
                        TimeTrimmerFunction();  //time extraction into int format..20200825 for date and 1220 for time
                        SortingTheData();
                        DuplicateFinder();      //duplicate finder in the engg database

                        for (int databaseNumber = 0; databaseNumber < DevUI.Count; databaseNumber++)
                        {
                            //psql.DevUI = this.DevUI;
                            richTextBox1.Text = "The data is being processed\r\nTalking To Database.";
                            dataincomingfromDataBase = psql.GrabADatabaseWithDevEUI(DevUI[databaseNumber],dboMeterID, Database);
                            MeterID.Add(dataincomingfromDataBase);
                            dataincomingfromDataBase = psql.GrabADatabaseWithDevEUI(DevUI[databaseNumber], dboBatchID, Database);
                            BatchID.Add(dataincomingfromDataBase);

                        }
                        #endregion Database connect and query for meterID

                        //DuplicateFinder(); //duplicate finder in the engg database

                        #region if else for MessageBox, checks if the file is empty
                        if (DevUI == null)
                        {

                            MessageBox.Show("The .csv file is empty\r\n try after some time.");
                        }
                        else
                        {
                            //richTextBox1.Text = "The data is being processed \r\n Press Display to view Info.";
                        }
                        #endregion if else for MessageBox, checks if the file is empty

                    }

                }

            }
        }
        private void TimeTrimmerFunction()
        {
            try
            {
                for (int counter = 0; counter < TimeOfprogrammingModems.Count; counter++)
                {
                    do
                    {
                        DateOfprogrammingModems.Add(TimeOfprogrammingModems[counter].Substring(0, 10));
                        DateOfprogrammingModems[counter] = DateOfprogrammingModems[counter].Replace("-", "");
                        TimeOfprogrammingModems[counter] = TimeOfprogrammingModems[counter].Substring(11, 5);
                        TimeOfprogrammingModems[counter] = TimeOfprogrammingModems[counter].Replace(":", "");
                        strToIntDateOPM.Add(int.Parse(DateOfprogrammingModems[counter])); //this are going to be used after this part
                        strToIntTimeOPM.Add(int.Parse(TimeOfprogrammingModems[counter]));
                    }
                    while (TimeOfprogrammingModems.IndexOf("-") != -1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            DateOfprogrammingModems.Clear(); TimeOfprogrammingModems.Clear(); //these are discarded from this point
        } 
        public void EndsightComparefunction(string path)  //tab 2 function
        {
            ErrorList ER = new ErrorList();
            this.progressBar_Universal.Value = 0;
            ColumnNominclature(path);//columns would be named here instead of after this function.

            for (int counter = 0; counter < EndsightDevUI.Count; counter++)
            {
                string EndsightDevEuiTemp = EndsightDevUI[counter].ToLowerInvariant();
                if (DevUI.Contains(EndsightDevEuiTemp)){    globalCounterForEndsight.Add(EndsightDevEuiTemp);   }
            }
            for (int reference = 0; reference < DevUI.Count; reference++)
            {
                this.progressBar_Universal.Step = 1;
                this.progressBar_Universal.PerformStep();
                for (int comparingTo = 0; comparingTo < EndsightDevUI.Count; comparingTo++)
                {
                    while (DevUI[reference] == EndsightDevUI[comparingTo].ToLowerInvariant())//devEui matching?
                    {
                        if (MeterID[reference] == EndsightMeterID[comparingTo])//meterID matching?
                        {
                            if (EndsightStatus[comparingTo].Contains("ok")|| EndsightStatus[comparingTo].Contains("OK"))//status code matching?
                            {
                                Tab2_FileAppend(reference, comparingTo, path);//why it is here? to append the correct data flaged OK into File to AMR check.
                                DateTime dateTime = DateTime.Now;//local time taken from the computer

                                if (string.IsNullOrEmpty(psql.GrabADatabaseWithMeterID(MeterID[reference], "AMRchkBy", Database)) || checkBox_UpdateAllMeters.Checked)      //check if the return is true or false  CountForreturnDbData <= 1
                                {
                                    //MeterId, date, initialofUser, AppKey, Database, FwVersion, TxPower, FreqChannels, AppEUI
                                    psql.PostDataToAMRCheck(MeterID[reference], dateTime, TextBox_Initials.Text,AppKey[reference], Database,FwVersion[reference],TextBox_TxPower.Text,TextBox_FreqChannels.Text,TextBox_AppEUI.Text);
                                    CounterForUpdatedMetersToDB++;
                                }
                                DevUI.RemoveAt(reference) ;MeterID.RemoveAt(reference); AppKey.RemoveAt(reference);FwVersion.RemoveAt(reference);
                                if (reference != 0) { reference--; }
                                CounterForCorrectDataSet++;
                            }
                            else { statusCode[reference] = EndsightStatus[comparingTo] + ":" + ER.DefaultStatusCode; }//commented break
                        }
                        else { tempEndMeterID[reference] = EndsightMeterID[comparingTo] + ":" + ER.DefaultMeterID_unmatched; }//commented break
                        break;
                    }
                }
            }
            this.progressBar_Universal.Value = 100;
        }
        public string GetDateTimeFromUser(int user)
        {
            string dateFormatYY_MM_dd;
            switch (user)
            {
                case (int)Profile.UserCTime:  //User time
                    dateFormatYY_MM_dd = TextBox_UserDateInput.Text;
                    break;

                case (int)Profile.SystemCTime:  //System Time
                    DateTime lastupdated = DateTime.Today;
                    dateFormatYY_MM_dd = lastupdated.ToString("yyyy_MM_dd");
                    break;
                default:
                    dateFormatYY_MM_dd = TextBox_UserDateInput.Text;
                    break;
            }
            return dateFormatYY_MM_dd;
        }
        void InitialServerInput()
        {
            try
            {
                Ary_DataFromTextFile = File.ReadAllLines(pathOfLogFile);
                DateTime FilelastWriteTime = File.GetLastWriteTime(pathOfLogFile); string FilelastWriteTime_dateOnly = FilelastWriteTime.ToString("dd");
                DateTime FileCreationDate = File.GetCreationTime(pathOfLogFile); string FileCreationDate_OnlyD = FileCreationDate.ToString("dd"); string FileCreationDate_MDY = FileCreationDate.ToString("MM/dd/yyyy");
                DateTime NowTime = DateTime.Now;string NowTime_OnlyD = NowTime.ToString("dd"); string NowTime_MDY = NowTime.ToString("MM/dd/yyyy");

                bool CompareDate=false;
                foreach (string TextLine in Ary_DataFromTextFile)
                {
                    if (incrementProcessFlag)
                    {
                        int LengthOfURL = TextLine.LastIndexOf('_') - TextLine.IndexOf("_URL_");
                        string URLContent = TextLine.Substring(TextLine.IndexOf("_URL_") + 5, LengthOfURL - 5);
                        comboBoxServerSelect.Text = URLContent;

                        int LengthOfUSER = TextLine.LastIndexOf('_') - TextLine.IndexOf("USER_");
                        string USERContent = TextLine.Substring(TextLine.IndexOf("USER_") + 5, LengthOfUSER - 5);
                        
                        USERContent = USERContent.Substring(0, USERContent.IndexOf('_'));
                        TextBox_Username.Text = USERContent;

                        incrementProcessFlag = false;
                    }
                    
                    if (TextLine.Contains("TimeOfLog:"))
                    {
                        string TimeOfLogFromFile = TextLine.Substring(10, 11);
                        try { CompareDate = 9 >= int.Parse(FilelastWriteTime_dateOnly); }
                        catch { }    
                        if (CompareDate)
                            FilelastWriteTime_dateOnly = FilelastWriteTime.ToString("MM/d/yyyy");
                        else
                            FilelastWriteTime_dateOnly = FilelastWriteTime.ToString("MM/dd/yyyy");

                        if(TimeOfLogFromFile.Contains(FilelastWriteTime_dateOnly)){ incrementProcessFlag = true; }
                        //if (TimeOfLogFromFile.Contains(FilelastWriteTime_dateOnly))
                        //{
                        //    incrementProcessFlag = true;
                        //}
                    }
                }
                if(string.Equals(NowTime_OnlyD,"01") || string.Equals(NowTime_OnlyD, "15") || string.Equals(NowTime_OnlyD, "19"))
                {
                    if(!string.Equals(FileCreationDate_MDY, NowTime_MDY))
                    {
                        File.Delete(pathOfLogFile);
                        
                        File.Create(pathOfLogFile);
                        richTextBox1.AppendText("\r\nNew monthly Log file is created, Access the File anytime here: "+ pathOfLogFile);
                        MessageBox.Show("\r\nNew monthly Log file is created, Access the File anytime here: " + pathOfLogFile);
                    }
                    
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error); File.Create(pathOfLogFile); ; }
        }
    #region ClearList Function
        public void ListClearFunction()
        {
            
            listView.Clear();//if it gives you hard time delete it
            DevUI.Clear();
            EndsightDevUI.Clear();
            AppKey.Clear();
            CustVer.Clear();
            FwVersion.Clear();
            MeterID.Clear();
            EndsightMeterID.Clear();
            BatchID.Clear();
            filenameArray.Clear();
            TimeOfprogrammingModems.Clear();
            DateOfprogrammingModems.Clear();
            EndsightStatus.Clear();
            strToIntDateOPM.Clear();
            strToIntTimeOPM.Clear();
            MeterID_int.Clear();
            globalCounterForEndsight.Clear();
            CounterForUpdatedMetersToDB = 0;
            CounterForCorrectDataSet = 0;

            //array clear
            Array.Clear(TempArrayForDisplayContent, 0, TempArrayForDisplayContent.Length);
            Array.Clear(Duplicates, 0, Duplicates.Length);
            Array.Clear(displayConfigArray, 0, displayConfigArray.Length);
            Array.Clear(AlreadyPresentDevUI, 0, AlreadyPresentDevUI.Length);
            Array.Clear(tempAryforDuplicates, 0, tempAryforDuplicates.Length);
            Array.Clear(statusCode, 0, statusCode.Length);
            Array.Clear(tempEndMeterID, 0, tempEndMeterID.Length);
            Array.Clear(Ary_DataFromTextFile, 0, Ary_DataFromTextFile.Length);

            this.progressBar_Universal.Value = 0;

            textBox_BrowsedFileName.Clear();

        }
    #endregion ClearList Function
    #endregion Functions DuplicateFinder, FileProcess, Fileparse, timeTrimmer

        #endregion Tab1 Functions

        #region Tab2 Functions              //Tab 2 functions only, Endsight function

        private void Tab2button2_browse2(object sender, EventArgs e) //production file
        {
             
            string filePath = string.Empty; var fileContent = string.Empty;
            string Endsight_filePath = @"C:\Reports\"; var Endsight_fileContent = string.Empty;

            ListClearFunction();

            Endsight_filePath = LatestFileSort(Endsight_filePath);
            richTextBox1.Text = "Endsight File: " + Endsight_File1Name;
            //textBox_BrowsedFileName.Text = FileInputDir;
            richTextBox1.AppendText("\r\nbrowse this Directory for Production files: "+FileInputDir+@"\Output-->Date."+ "\r\nName of the File: Prod&Database_<YY:MM:DD>_Counter.\r\nyou can Copy the path.");
            if (Endsight_filePath != null)
            {
                using (TextFieldParser parser = new TextFieldParser(Endsight_filePath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    while (!parser.EndOfData)
                    {
                        #region Parsing the engg file
                        listView.Visible = true;
                        listView.View = View.Details;
                        string[] fields = parser.ReadFields();
                        #endregion Parsing the engg file

                        EndsightMeterID.Add(fields[2]);
                        EndsightDevUI.Add(fields[3]);
                        EndsightStatus.Add(fields[4]);
                    }
                }
            }

            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.InitialDirectory = FileOutgoingDir;
                openFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog1.FileName;
                    richTextBox1.AppendText("\r\n"+ openFileDialog1.FileName);
                    textBox_BrowsedFileName.Text = filePath.Substring(filePath.LastIndexOf("\\")+1,(filePath.Length - filePath.LastIndexOf("\\"))-1);
                    if (filePath != string.Empty)
                    {
                        using (TextFieldParser parser = new TextFieldParser(filePath))
                        {
                            parser.TextFieldType = FieldType.Delimited;
                            parser.SetDelimiters(",");
                            while (!parser.EndOfData)
                            {
                                #region Parsing the engg file
                                listView.Visible = true;
                                listView.View = View.Details;
                                string[] fields = parser.ReadFields();
                                #endregion Parsing the engg file

                                MeterID.Add(fields[4]);
                                FwVersion.Add(fields[3]);
                                AppKey.Add(fields[1]);
                                DevUI.Add(fields[0]);

                            }
                        }
                    }
                }
            }
            if(filePath != string.Empty)
                richTextBox1.AppendText("\r\nProduction File is selected!");
            else
                richTextBox1.AppendText("\r\nNothing is Selected, Try Browsing again!\r\nThe Help is Here.");
            //richTextBox1.Text = "Production File is selected!";
        }

        private void Tab2button3_compareAndExport(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox_BrowsedFileName.Text))
            {
                Database = comboBox_DB.Text;
                //if (Database)
                //{
                //    DialogResult result = MessageBox.Show("You have selected R Vision DB, Still want to continue? Press Yes to Proceed.", "Database Attention!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                //    Database = result == DialogResult.Yes ? true : false;
                //    //if (result == DialogResult.Yes) { Database = true; }
                //}

                if (!string.IsNullOrEmpty(TextBox_Initials.Text) && TextBox_Initials.Text.Length > 1)
                {
                    Button_RefreshTab2.BackColor = Color.Red;
                    richTextBox1.Text = "Compare and Export Button pressed";
                    string date = GetDateTimeFromUser(2);//2 for systemtime
                    Directory.CreateDirectory(FileOutputForErrorSheets_tab2 + date);
                    int FileCounter = 1;
                    string ExportFileName = "ErrorSheet_" + date + "_" + FileCounter + ".csv";
                    string path = FileOutputForErrorSheets_tab2 + date + @"\" + ExportFileName;
                    while (File.Exists(path))
                    {
                        FileCounter++;
                        ExportFileName = "ErrorSheet_" + date + "_" + FileCounter + ".csv";
                        path = FileOutputForErrorSheets_tab2 + date + @"\" + ExportFileName;
                    }

                    EndsightComparefunction(path); //comparing the data here.

                    listView.Columns.Add("DBn", 50, HorizontalAlignment.Left);
                    listView.Columns.Add("DevEUI", 150, HorizontalAlignment.Left);
                    listView.Columns.Add("Prod.MeterID", 150, HorizontalAlignment.Left);
                    listView.Columns.Add("Ends.MeterID", 150, HorizontalAlignment.Left);
                    listView.Columns.Add("Status Code", 100, HorizontalAlignment.Left);
                    try     //try for file directory search
                    {
                        if (File.Exists(path))
                        {
                            for (int count = 1; count < DevUI.Count; count++)
                            {
                                this.progressBar_Universal.Step = 5;
                                this.progressBar_Universal.PerformStep();
                                TempArrayForDisplayContent[0] = "" + count;
                                TempArrayForDisplayContent[1] = DevUI[count];
                                TempArrayForDisplayContent[2] = MeterID[count];
                                TempArrayForDisplayContent[3] = tempEndMeterID[count];
                                TempArrayForDisplayContent[4] = statusCode[count];
                                if (TempArrayForDisplayContent[3] == null) { TempArrayForDisplayContent[3] = "ND_Endst"; }
                                if (TempArrayForDisplayContent[4] == null) { TempArrayForDisplayContent[4] = "Not OK_Mnul"; }

                                displayConfigArray[0] = TempArrayForDisplayContent[1] + "," + TempArrayForDisplayContent[2] + "," + TempArrayForDisplayContent[3] + "," + TempArrayForDisplayContent[4];
                                File.AppendAllLines(path, displayConfigArray);
                                displayConfigArray[0] = TempArrayForDisplayContent[1] + "\t" + TempArrayForDisplayContent[2] + "\t" + TempArrayForDisplayContent[3] + "\t" + TempArrayForDisplayContent[4];

                                itm2 = new ListViewItem(TempArrayForDisplayContent);
                                listView.Items.Add(itm2);
                            }
                            File.AppendAllText(path, "All Short terms:: ND-No Data, Endst-Endsight, Mnul-Manual Entry, DMIU- Default MeterID unmatched for matched DevEUI with Production records, DSC-Default Statuscode OK");
                        }
                        richTextBox1.Text = "the file is exported to\r\n" + path + "\r\n" + CounterForCorrectDataSet + "-------Datasets are Correct And can be used to AMR check.";
                        richTextBox1.AppendText("\r\n" + globalCounterForEndsight.Count + "-------globalCounterForEndsight.Debug use!");
                        richTextBox1.AppendText("\r\n" + CounterForUpdatedMetersToDB + " Meters Updated in AMR Check, Others might be already checked!");//Amr checkedon these meters
                        try { richTextBox1.AppendText("\r\n" + FwVersion[2] + " -------ModemFirmwareRev updated in DataBase."); }
                        catch { }
                        MessageBox.Show(CounterForUpdatedMetersToDB + "-------meters are updated to DB.");
                        if (CounterForCorrectDataSet != globalCounterForEndsight.Count) { richTextBox1.AppendText("\r\nCheck the Data once again\r\nas the DevEUI and meterID are not same."); }
                        this.progressBar_Universal.Value = 100;
                    }
                    catch (Exception ex)    //catch for file directory search
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else { MessageBox.Show("Enter the Initials.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
            }
            else { MessageBox.Show("Select the file First.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
        }

        public void ColumnNominclature(string path)
        {
            TempArrayForDisplayContent[1] = "DevEUI";
            TempArrayForDisplayContent[2] = "MeterID";
            TempArrayForDisplayContent[3] = "En MeterID";
            TempArrayForDisplayContent[4] = "Status Code";
            TempArrayForDisplayContent[5] = "ModemFwRev";
        //used for file export format
            displayConfigArray[0] = TempArrayForDisplayContent[1] + "," + TempArrayForDisplayContent[2] + "," + TempArrayForDisplayContent[3] + "," + TempArrayForDisplayContent[4]+","+ TempArrayForDisplayContent[5];
            File.AppendAllLines(path, displayConfigArray);
        //dont know the use of the following file.
            displayConfigArray[0] = TempArrayForDisplayContent[1] + "\t" + TempArrayForDisplayContent[2] + "\t" + TempArrayForDisplayContent[3] + "\t" + TempArrayForDisplayContent[4]+"\t" + TempArrayForDisplayContent[5];
        }
            
        public void Tab2_FileAppend(int reference,int compareTo,string path)
        {
            TempArrayForDisplayContent[1] = DevUI[reference];
            TempArrayForDisplayContent[2] = MeterID[reference];
            TempArrayForDisplayContent[3] = EndsightMeterID[compareTo];
            TempArrayForDisplayContent[4] = EndsightStatus[compareTo]+"_Endst";
            TempArrayForDisplayContent[5] = FwVersion[reference];
            if (TempArrayForDisplayContent[3] == null) { TempArrayForDisplayContent[3] = "ND_Endst"; }//EndsightMeterID_ Nd is No data
            if (TempArrayForDisplayContent[4] == null) { TempArrayForDisplayContent[4] = "OK_Mnul"; }//Status Code Mnul is Manual entry

            displayConfigArray[0] = TempArrayForDisplayContent[1] + "," + TempArrayForDisplayContent[2] + "," + TempArrayForDisplayContent[3] + "," + TempArrayForDisplayContent[4] + "," + TempArrayForDisplayContent[5];
            File.AppendAllLines(path, displayConfigArray);
        }

        #endregion Tab2 Functions

        #region Tab3 Functions

        private async void button2_Click(object sender, EventArgs e)
        {
            DevUI.RemoveAt(0);MeterID.RemoveAt(0);int counterForDown = 0; filenameArray.Clear(); richTextBox_Temp.Clear(); int MeterMin = 0, MeterMax = 0;
            progressBar_Universal.Minimum = 0; progressBar_Universal.Maximum = 100+DevUI.Count;
            progressBar_Universal.Value = 0; CounterForCorrectDataSet = 0; CounterForUpdatedMetersToDB = 0;

            if (!string.IsNullOrEmpty(textBox_FileSelectDelete.Text))
            {
                if (!string.IsNullOrEmpty(textBox_MeterRmin.Text) && !string.IsNullOrEmpty(textBox_MeterRmax.Text))
                {
                    MeterMin = int.Parse(textBox_MeterRmin.Text);
                    MeterMax = int.Parse(textBox_MeterRmax.Text);
                }
                ChiprstackAPI CS = new ChiprstackAPI();
                CS.passwordAPI = textBox_pass_Delete.Text;
                CS.emailAPI = textBox_user_Delete.Text;
                CS.ServerURL = textBox_addr_Delete.Text;

                await CS.ChirPostLogin(); progressBar_Universal.Value += 10;

                try
                {
                    foreach (string Dev in DevUI)
                    {
                        progressBar_Universal.Value += 1;
                        if (checkBox_meterRangeOption.Checked)
                        {
                            if (int.TryParse(MeterID[counterForDown], out int Temp_meterID))
                            {
                                if ((Temp_meterID >= MeterMin) && (Temp_meterID <= MeterMax))
                                {
                                    CounterForUpdatedMetersToDB++;
                                    bool result = await CS.ChirpDeleteMetersWithDevEUI(Dev);
                                    Thread.Sleep(200);
                                    if (!result)
                                    {
                                        filenameArray.Add(Dev + "---" + Temp_meterID + " .Error Deleting this Device.\r\n");
                                        richTextBox_Temp.AppendText(Dev + "---" + Temp_meterID + " .Error Deleting this Device.\r\n");
                                        CounterForCorrectDataSet++;//older name is used to avoid more declarations.

                                    }
                                    if(CounterForUpdatedMetersToDB==1)
                                    {
                                        richTextBox_Temp.AppendText("Login Success.\r\n");
                                    }
                                }
                            }

                        }
                        else
                        {
                            if (int.TryParse(MeterID[counterForDown], out int Temp_meterID))
                            {
                                CounterForUpdatedMetersToDB++;
                                bool result = await CS.ChirpDeleteMetersWithDevEUI(Dev);
                                if (!result)
                                {
                                    filenameArray.Add(Dev + "---" + Temp_meterID + " .Error Deleting this Device.\r\n");
                                    richTextBox_Temp.AppendText(Dev + "---" + Temp_meterID + " .Error Deleting this Device.\r\n");
                                }
                            }
                        }
                        counterForDown++;
                    }
                }
                catch (Exception exp)
                {
                    richTextBox_Temp.AppendText("Error Catch.\r\n" + exp);
                }
                richTextBox_Temp.AppendText("Process is completed.\r\nMeters qualified for range: " + CounterForUpdatedMetersToDB + "\r\nError occured in " + CounterForCorrectDataSet + " Meter(s).\r\nMeters Success: " + (CounterForUpdatedMetersToDB - CounterForCorrectDataSet));
                progressBar_Universal.Value = progressBar_Universal.Maximum;
            }
            else
                richTextBox1.Text = "Error selecting the File.";


        }

        private void Button_Browse_CS_Click(object sender, EventArgs e)
        {
            richTextBox_Temp.Clear(); richTextBox1.Clear();ListClearFunction();
            string filePath = string.Empty;
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.InitialDirectory = @"\\netserver3\data\Loraproduction_Engineering\MetershopApplicationFolder\OutputFile";
                openFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog1.FileName;
                    richTextBox1.AppendText("\r\n" + openFileDialog1.FileName);
                    textBox_FileSelectDelete.Text = filePath.Substring(filePath.LastIndexOf("\\") + 1, (filePath.Length - filePath.LastIndexOf("\\")) - 1);
                    if (filePath != string.Empty)
                    {
                        using (TextFieldParser parser = new TextFieldParser(filePath))
                        {
                            parser.TextFieldType = FieldType.Delimited;
                            parser.SetDelimiters(",");
                            while (!parser.EndOfData)
                            {
                                #region Parsing the engg file
                                listView.Visible = true;
                                listView.View = View.Details;
                                string[] fields = parser.ReadFields();
                                #endregion Parsing the engg file
                                if (!string.Equals(fields[6], "No data"))
                                {
                                    DevUI.Add(fields[0]);
                                    MeterID.Add(fields[4]);
                                    BatchID.Add(fields[6]);
                                }
                            }
                        }
                    }
                }
            }
            try
            {
                foreach (string MID in MeterID)
                {
                    if(!string.Equals(MID.ToUpper(), "METERID"))//Compare(
                    {
                        try
                        {
                            MeterID_int.Add(int.Parse(MID));
                        }
                        catch { }
                    }
                }
                MeterID_int.Sort();
                label_MinValue.Text = "min: " + MeterID_int[0]; label_maxValue.Text = "max: " + MeterID_int[MeterID_int.Count - 1];
                textBox_MeterRmin.Text = string.Empty+MeterID_int[0];
            }
            catch
            {
                label_MinValue.Text = "min: No Data"; label_maxValue.Text = "max: No Data";
            }

            int counterForDown = 0;
            foreach (string DEV in DevUI)
            {
                richTextBox_Temp.AppendText(DEV + "----" + MeterID[counterForDown] + "\r\n"); counterForDown++;
            }
            richTextBox_Temp.Visible = true;
        }

        #endregion tab3

        #region General Functions           //function which does not belong to any tab
        #region versionNumber
        private void version_text(object sender, EventArgs e)                       //label1: Version is typed here, Hardcoded for now
            {
                label1.Text = ApplicationVersionnumber;
            }
            #endregion versionNumber

        #region TextBoxforNotification
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Length > 0)
            {
                // Copy the formatted content to the clipboard
                // Clipboard.SetText(richTextBox1.Rtf, TextDataFormat.Rtf);
                //Graphics objGraphics;
                Clipboard.SetData(DataFormats.Rtf, richTextBox1.SelectedRtf);
                Clipboard.Clear();

            }
        }
        #endregion TextBoxforNotification

        #region Empty Functions

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            ListClearFunction();
            richTextBox1.Text = "Lists are all cleared up!";
            Button_RefreshTab2.BackColor = Color.Empty;
        }
        private void version(object sender, EventArgs e)
        {
            label1.Text = "version";
        }

        private void Tab2_Click(object sender, EventArgs e)
        {

        }

    private void TextBox_UserDateInput_MouseDown(object sender, MouseEventArgs e)
    {

    }

    private void TextBox_ApplicationDesp_MouseLeave(object sender, EventArgs e)
    {
        TextBox_CustID.Text = TextBox_ApplicationDesp.Text;
    }

    private void TextBox_MeterMaxRange_TextChanged(object sender, EventArgs e)
    {

    }

    private void TextBox_MeterMaxRange_TextChanged_1(object sender, EventArgs e)
    {

    }

        private void comboBoxServerSelect_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox_meterRangeOption_CheckedChanged(object sender, EventArgs e)
        {
            groupBox_meterRange.Visible = checkBox_meterRangeOption.Checked ? true : false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            richTextBox_Temp.Visible = checkBox_NotificationViewToggler.Checked ? true : false;
        }

        private void TextBox_UserDateInput_MouseEnter(object sender, EventArgs e)
        {
            richTextBox_Temp.Visible = false;
        }

        private void richTextBox_Temp_VisibleChanged(object sender, EventArgs e)
        {
            checkBox_NotificationViewToggler.Checked = richTextBox_Temp.Visible ? true : false;
        }

        private void comboBox_DB_DropDown(object sender, EventArgs e)
        {
            //RE1.DataBaseFinder(comboBox_DataBaseName.Text);
            //comboBox_DataBaseName.DataSource = RE1.FilenamesForSearch; comboBox_tab5_DBName.DataSource = RE1.FilenamesForSearch;
            //comboBox_DataBaseName.BackColor = Color.Red; comboBox_tab5_DBName.BackColor = Color.Red; labeltab5_1.Visible = false;
        }

        private void comboBox_DB_DropDownClosed(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(comboBox_DataBaseName.Text))
            //{
            //    comboBox_DataBaseName.BackColor = Color.LightGreen;
            //    comboBox_tab5_DBName.BackColor = Color.LightGreen;
            //}
            //else
            //{
            //    comboBox_DataBaseName.BackColor = Color.Red;
            //    comboBox_tab5_DBName.BackColor = Color.Red;
            //}
        }

        private void textBox4_MergeFileip(object sender, EventArgs e)
    {
        FileInputDir = TextBox__MergeFileinput.Text;
    }

    private void tabPage1_Click(object sender, EventArgs e)
    {
            
    }


    #endregion Empty Functions

        #region Commented program snippets
        //string dateFormatYY_MM_dd= textBox5.Text;
        ////Date and time, this can be use anywhere refering the current time and day
        //DateTime lastupdated = DateTime.Today;
        //if (dateFormatYY_MM_dd == null) { dateFormatYY_MM_dd = lastupdated.ToString("yyyy_MM_dd"); ; }
        ////dateFormatYY_MM_dd = lastupdated.ToString("yyyy_MM_dd");
        #endregion Commented program snippets

        #endregion General Functions
    }


}

