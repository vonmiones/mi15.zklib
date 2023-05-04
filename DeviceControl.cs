using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zklib
{
    public class DeviceEventArgs : EventArgs
    {
        public string EventProgress { get; }
        public string EventStatus { get; }
        public string EventTypes { get; }
        public string EventCode { get; }

        public DeviceEventArgs(string Event, string Status, string Eventtype, string Code)
        {
            EventProgress = Event;
            EventStatus = Status;
            EventTypes = Eventtype;
            EventCode = Code;
        }
    }
    public class DeviceInfoClass
    {
        public string omessage { get; set; }
        public string sFirmver { get; set; }
        public string sMac { get; set; }
        public string sPlatform { get; set; }
        public string sSN { get; set; }
        public string sProductTime { get; set; }
        public string sDeviceName { get; set; }
        public int iFPAlg { get; set; }
        public int iFaceAlg { get; set; }
        public string sProducter { get; set; }
    }
    public class DeviceControl
    {
        public delegate void ActivityEventHandler(object sender, DeviceEventArgs args);
        public event ActivityEventHandler EventCallback;
        public void DoActivity(string progress, string status, string type, string code)
        {
            OnDeviceResponse(new DeviceEventArgs(progress, status, type, code));
        }
        protected virtual void OnDeviceResponse(DeviceEventArgs args)
        {
            broadcast = args.EventProgress;
            broadcaststatus = args.EventStatus;

            EventCallback?.Invoke(this, args);
        }

        public delegate void MessageEvent(string progress, string status = null, string type = null);
        public delegate void ProgressCallback(int progress, string text = null, int istatus = 0, string status = null, string ip = null);
        public string broadcast { get; set; }
        public string broadcaststatus { get; set; }

        private SDKHelper SDK = new SDKHelper();
        private string _ip, _port, _coms;
        public DeviceControl(string ip = "192.168.1.201", string coms = "0")
        {
            _ip = ip;
            _port = "4370";
            _coms = coms;
            SDKHelper.eMessage += SDKHelper_eMessage;
        }
        public int Connect()
        {
            return SDK.ConnectTCP(broadcast, _ip, _port, _coms);
        }
        public void syncDateTime()
        {
            SDK.sta_SetDeviceTime(DateTime.Now.ToLocalTime().AddSeconds(1).ToLongTimeString());
        }
        private void SDKHelper_eMessage(string progress, string status = null, string type = null)
        {
            DoActivity(progress, status, null, null);
        }
        public List<SDKHelper.Employee> GetAllEmployees()
        {
            return SDK.sta_getEmployees();
        }
        public List<SDKHelper.BioTemplate> GetAllFPTemplate()
        {
            ProgressBar pb = new ProgressBar();
            return SDK.sta_BatchGetBioTemplates(pb, 1);
        }
        public List<SDKHelper.UserInfo> GetAllUserInfo()
        {
            return SDK.sta_GetAllUserFPInfo();
        }
        public void Reboot()
        {
            SDK.Restart();
        }
        public void RegisterOnline(string id, int index, int iflag)
        {
            SDK.RegisterUser(id, index, iflag);
            //SDK.sta_OnlineEnroll(id, index, iflag);
        }


        public List<SDKHelper.AttendanceLog> GetAllAttendanceLog()
        {
            List<SDKHelper.AttendanceLog> dt = new List<SDKHelper.AttendanceLog>();
            SDK.sta_readAttLog(dt);
            return dt;
        }
        public DataTable GetNewAttendanceLog()
        {
            DataTable dt = new DataTable();
            SDK.sta_ReadNewAttLog(dt);
            return dt;
        }
        public DeviceInfoClass DeviceInfo()
        {
            string omessage = "";
            string sFirmver = "";
            string sMac = "";
            string sPlatform = "";
            string sSN = "";
            string sProductTime = "";
            string sDeviceName = "";
            int iFPAlg = 0;
            int iFaceAlg = 0;
            string sProducter = "";
            SDK.sta_GetDeviceInfo(omessage, out sFirmver, out sMac, out sPlatform, out sSN, out sProductTime, out sDeviceName, out iFPAlg, out iFaceAlg, out sProducter);
            DeviceInfoClass di = new DeviceInfoClass() {
                omessage = omessage,
                sFirmver = sFirmver,
                sMac = sMac,
                sPlatform = sPlatform,
                sSN = sSN,
                sProductTime = sProductTime,
                sDeviceName = sDeviceName,
                iFPAlg = iFPAlg,
                iFaceAlg = iFaceAlg,
                sProducter = sProducter
            };

            return di;
        }

        public void disconnect()
        {
            SDK.Disconnect();
        }


    }
}
