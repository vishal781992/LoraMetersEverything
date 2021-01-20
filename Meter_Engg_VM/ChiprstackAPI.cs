using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Threading.Tasks;
//using System.Windows.Forms;

namespace Meter_Engg_VM
{
    public enum ReturnList
    {
        ErrorAddingNewDevice = 10,
        SuccessAddingNewDevice = 11,

        ErrorLogin = 20,
        SuccessLogin = 21,

        ErrorGettingDevUI = 30,
        SuccessGettingDevUI = 31,

        ErrorAddingApplication = 40,
        SuccessAddingApplication = 41,

        ErrorAppendingAppKey = 50,
        SuccessAppendingAppKey = 51,

        ErrorGettingApplication = 60,
        SuccessGettingApplication = 61,
        ErrorGettingApplicationNoResponse = 62,

        ErrorFlag = 70,
        SuccessFlag = 71,
    }

    public class ChiprstackAPI : Form
    {
        #region Declarations
        public string ServerURL = "http://117.100.100.20:8080/api/";//65
        public string emailAPI;//can be modified by textbox 6
        public string passwordAPI;//can be modified by textbox 7
        public string ChirpPostApplID;
        public string ChirpPostDeviceKeyResponse;
        public string ChirpGetApplID;
        public string CustomerIDFilter;
        int counterMeterID = 1;

        private const string ServiceProfileID = "f9f80426-52c6-4dfe-9b58-1c376d6e0720";//new one
        private const string DeviceProfileID = "a8fdb64e-7493-49c3-8ee9-f7ed20ef267c";
        private string JwtToken;
        #endregion Declarations

        #region ChirpLoginFunction
        public async Task<int> ChirPostLogin()                                                        //Login process
        {

            using (var clientLogin = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), ServerURL + "internal/login"))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");//Accept

                    request.Content = new StringContent("{" + "\n" + "\"email\"" + ":" + "\"" + emailAPI + "\"" + "," + "\n" + "\"password\":" + "\"" + passwordAPI + "\"" + "\n" + "}");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    var response = await clientLogin.SendAsync(request);//httpClient
                    if (response.IsSuccessStatusCode)
                    {
                        JwtToken = await response.Content.ReadAsStringAsync();
                        int countOfToken = JwtToken.Count() - 10;
                        JwtToken = JwtToken.Substring(8, countOfToken);

                        return (int)ReturnList.SuccessLogin;
                    }
                    else { return (int)ReturnList.ErrorLogin; }
                }

            }
        }
        #endregion ChirpLoginFunction

        #region ChirpGetApplicationFunction

        public async Task<int> ChirpGetApplicationName(string ApplicationName)           //get the Applciation return if exists
        {
            using (var clientgetApplExisting = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), ServerURL + "applications?limit=10&search=" + ApplicationName))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("Grpc-Metadata-Authorization", "Bearer " + JwtToken);

                    var response = await clientgetApplExisting.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        ChirpGetApplID = await response.Content.ReadAsStringAsync();
                        ChirpPostApplID = DataExtractionFunction(ChirpGetApplID, "id", ",");  //extracts the application ID
                        if (ChirpPostApplID.Contains("Error"))
                        {
                            ChirpPostApplID = "5000";
                            return (int)ReturnList.ErrorGettingApplication;//no ID found
                        }
                        return (int)ReturnList.SuccessGettingApplication;//old ID found
                    }
                    else { return (int)ReturnList.ErrorGettingApplication; }//no ID found
                }
            }
        }

        #endregion ChirpGetApplicationFunction

        #region ChirpPostApplicationFunction
        public async Task<int> ChirpPostApplication(string ApplicationName, string ApplicationDiscp)   //adding application, randomizing the application ID
        {
            using (var clientPostNewAppl = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), ServerURL + "applications"))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("Grpc-Metadata-Authorization", "Bearer " + JwtToken);
                    request.Content = new StringContent("{  \n" + "\"application\": {  \n     \"description\":" + "\"" + ApplicationDiscp + "\",  \n     \"id\": \"5000\",  \n     \"name\":" + "\"" + ApplicationName + "\",  \n     \"organizationID\": \"1\",  \n     \"payloadCodec\": \"\",  \n     \"payloadDecoderScript\": \"\",  \n     \"payloadEncoderScript\": \"\",  \n     \"serviceProfileID\": \"" + ServiceProfileID + "\"  \n   }  \n }");


                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    var response = await clientPostNewAppl.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        ChirpPostApplID = await response.Content.ReadAsStringAsync();
                        ChirpPostApplID = DataExtractionFunction(ChirpPostApplID, "id", ",");
                        //int count = ChirpPostApplID.Count() - 2;
                        //ChirpPostApplID = ChirpPostApplID.Substring(7, 2);

                        return (int)ReturnList.SuccessAddingApplication;
                    }
                    else { ChirpPostApplID = response.StatusCode.ToString(); return (int)ReturnList.ErrorAddingApplication; }
                }
            }
        }
        #endregion ChirpPostApplicationFunction

        #region ChirpGetMeterByDevUIFunction
        public async void ChirpGetMeterByDevUI(string DevUI)
        {
            using (var clientGetInfoDevUI = new HttpClient())
            {
                //devui to All the details of the meter already uploaded(below)
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), ServerURL + "devices/" + DevUI))       //testing devUI: 0e504b3735508237
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("Grpc-Metadata-Authorization", "Bearer " + JwtToken);

                    var response = await clientGetInfoDevUI.SendAsync(request);//httpClient
                    if (response.IsSuccessStatusCode)
                    {
                        string demoResponse1 = await response.Content.ReadAsStringAsync();
                    }
                }
            }
        }
        #endregion ChirpGetMeterByDevUIFunction

        #region ChirpPostNewDeviceFunction
        public async Task<int> ChirpPostNewDevice(string DevUI, string AppKey, string CustVer, string FwVersion, string MeterID) //adding device to server
        {

            string ChirpPostNewDeviceResponse;
            using (var clientPostNewDevice = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), ServerURL + "devices"))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("Grpc-Metadata-Authorization", "Bearer " + JwtToken);
                    request.Content = new StringContent("{" + "\n" + "\"device\":" + "{" + "\n" + "\"applicationID\":" + "\"" + ChirpPostApplID + "\"" + ",  \n" + "\"description\":" + "\"" + CustVer + "\"" + ",  \n" + "\"devEUI\":" + "\"" + DevUI + "\"" + ",  \n" + "\"deviceProfileID\":" + "\"" + DeviceProfileID + "\"" + ",  \n" + "\"isDisabled\": false,  \n" + "\"name\":" + "\"" + MeterID + "\"" + ",  \n" + "\"referenceAltitude\": 0,  \n" + "\"skipFCntCheck\": true, \n" + "\"tags\": {},  \n" + "\"variables\": {}  \n   }  \n }");


                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    var response = await clientPostNewDevice.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        ChirpPostNewDeviceResponse = await response.Content.ReadAsStringAsync(); counterMeterID++;
                        int responseTemp = await ChirpPostDeviceKey(DevUI, AppKey);
                        if (responseTemp == (int)ReturnList.SuccessAppendingAppKey)
                        {
                            return (int)ReturnList.SuccessAddingNewDevice;
                        }
                        else { return (int)ReturnList.ErrorAddingNewDevice; }

                    }
                    else { ChirpPostNewDeviceResponse = response.StatusCode.ToString(); return (int)ReturnList.ErrorAddingNewDevice; }
                }
            }

        }
        #endregion ChirpPostNewDeviceFunction

        #region ChirpPostDeviceKeyFunction
        public async Task<int> ChirpPostDeviceKey(string DevUI, string AppKey)
        {
            using (var clientPostDeviceKey = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), ServerURL + "devices/" + DevUI + "/keys"))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("Grpc-Metadata-Authorization", "Bearer " + JwtToken);

                    request.Content = new StringContent("{  \n   \"deviceKeys\": {  \n     \"appKey\": \"" + AppKey + "\",  \n     \"devEUI\": \"" + DevUI + "\",  \n     \"genAppKey\": \"00000000000000000000000000000000\",  \n   \"nwkKey\":\"" + AppKey + "\"  \n   }  \n }");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    var response = await clientPostDeviceKey.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        return (int)ReturnList.SuccessAppendingAppKey;
                    }
                    else { return (int)ReturnList.ErrorAppendingAppKey; }
                }
            }
        }
        #endregion ChirpPostDeviceKeyFunction

        #region DataprocessingFunction
        public string DataExtractionFunction(string strReference, string strNeeded, string dataSeperator)
        {
            string retString, name;
            if (strReference.Contains(strNeeded))
            {
                name = strReference.Substring(strReference.IndexOf("\"" + strNeeded + "\":"));
                try { retString = name.Substring(name.IndexOf("\"" + strNeeded + "\":"), name.IndexOf(dataSeperator)); }
                catch { retString = name.Substring(name.IndexOf("\"" + strNeeded + "\":"), name.IndexOf("}")); }

                retString = retString.Substring(retString.IndexOf(":"));
                retString = retString.Replace(":", "");
                retString = retString.Replace("\"", "");
                return retString;
            }
            else { return "Error"; }

        }
        #endregion DataprocessingFunction

        #region DeleteMetersWithDevEUI

        public async Task<bool>  ChirpDeleteMetersWithDevEUI(string DevEUI)
        {

            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("DELETE"), "http://117.100.100.20:8080/api/devices/"+DevEUI))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("Grpc-Metadata-Authorization", "Bearer " + JwtToken);

                    var response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else { return false; }
                }
            }
        }
        #endregion DeletemeterswithdevEUI

        //private void InitializeComponent()
        //{
        //    this.SuspendLayout();
        //    // 
        //    // ChiprstackAPI
        //    // 
        //    this.ClientSize = new System.Drawing.Size(592, 417);
        //    this.Name = "ChiprstackAPI";
        //    this.ResumeLayout(false);

        //}
    }//chirpstack close

}   //meter_enggg close

/*
 *  TimeOfprogrammingModems[counter] = TimeOfprogrammingModems[counter].Substring(TimeOfprogrammingModems.IndexOf(""), TimeOfprogrammingModems.IndexOf());
                    DateOfprogrammingModems.Add(TimeOfprogrammingModems[counter].Substring(0, 10));// TimeOfprogrammingModems[counter].Substring(1,5);//Remove(0, 13);
                    //DateOfprogrammingModems[counter] = DateOfprogrammingModems[counter].trim
                    TimeOfprogrammingModems[counter] = TimeOfprogrammingModems[counter].Substring(11,5);//.Remove(2, 1);
                    //TimeOfprogrammingModems[counter] = TimeOfprogrammingModems[counter].Remove(4, 1);
                    int.TryParse(TimeOfprogrammingModems[counter], out int Timedemo);//  TimeOfprogrammingModems[counter];
                    TimeOfprogramming.Add(Timedemo);
*/
