using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CSL
{
    public class CS463_HL_API
    {
        public const string ApiVersion = "1.0.0.2";

        private Uri httpUri;

        private string LoginName;
        private string LoginPassword;
        private string SessionId;
        private string ErrorMsg;
        private Int32 ErrorCode;
        private Int32 TimeWindow;
        private Int32 HttpTimeout;

        private int ApiLogLevel;

        private const string ERR_MSG_UNKNOWN_ERROR = "Unknown error";
        private const Int32 ERR_CODE_UNKNOWN_ERROR = -999;
        private const string ERR_MSG_UNKNOWN_RESPONSE = "Unknown response";
        private const Int32 ERR_CODE_UNKNOWN_RESPONSE = -100;

        private const Int32 ERR_CODE_NO_ERROR = 0;
        private const Int32 ERR_CODE_WEB_ERROR = -101;
        private const Int32 ERR_CODE_NOT_LOGIN = -201;


        #region Constructor
        public CS463_HL_API()
        {
            httpUri = null;
            LoginName = "";
            LoginPassword = "";
            SessionId = "";
            ErrorMsg = "";
            ErrorCode = 0;
            TimeWindow = 30;
            ApiLogLevel = LOG_LEVEL.Disabled;
            http_timeout = 30000;   //30s
        }
        #endregion

        #region Properties
        public Int32 http_timeout
        {
            set { HttpTimeout = value; }
            get { return HttpTimeout; }
        }

        public int api_log_level
        {
            get { return (ApiLogLevel); }
            set { ApiLogLevel = value; }
        }

        public Int32 time_window
        {
            get { return (TimeWindow); }
            set
            {
                if (value > 1)
                    TimeWindow = value;
            }
        }

        public string login_name
        {
            get { return (LoginName); }
            set { LoginName = value; }
        }

        public string login_password
        {
            get { return (LoginPassword); }
            set { LoginPassword = value; }
        }

        public string session_id
        {
            get { return (SessionId); }
            set { SessionId = value; }
        }

        public string error_msg
        {
            get { return (ErrorMsg); }
        }

        public Int32 error_code
        {
            get { return (ErrorCode); }
        }
        #endregion

        static public DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime st = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return st.AddSeconds(timestamp);
        }

        public string getURI()
        {
            return httpUri.AbsoluteUri;
        }

        public bool setURI(string uri)
        {
            try
            {
                httpUri = new Uri(uri);
                return true;
            }
            catch (UriFormatException uEx)
            {
                ErrorMsg = uEx.Message;
            }
            return false;
        }

        static public double getFreq(string str)
        {
            //Bit 0-10: main frequency
            //Bit 11-14: frequency offset
            //bit 15: reserved
            try
            {
                UInt16 f = UInt16.Parse(str);
                return 860 + (f & 0x7FF) * 0.05 + (f >> 11) * 0.005;
            }
            catch
            {
                return 0.0;
            }
        }

        private bool isCSLResponse(ref XmlDocument doc, string cmd)
        {
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;

            XmlElement element = doc.DocumentElement;
            if (element.Name != "CSL") return false;
            XmlNode node = doc.SelectSingleNode("CSL/Command");
            if (node == null) return false;
            if (node.InnerXml.Equals(cmd, StringComparison.OrdinalIgnoreCase) == false) return false;
            return true;
        }

        private bool parseErrorCode(ref XmlDocument doc)
        {
            //<CSL><Ack>Error: ... </Ack>
            XmlNode node = doc.SelectSingleNode("CSL/Ack");
            if (node != null)
            {
                if (node.InnerXml.StartsWith("Error:", StringComparison.OrdinalIgnoreCase))
                {
                    //error
                    string msg = node.InnerXml.Substring(7);
                    ErrorMsg = msg.Trim();
                    return true;
                }
            }

            //<CSL><Error ... />
            node = doc.SelectSingleNode("CSL/Error");
            if (node == null) return false;
            //<Error>
            foreach (XmlAttribute att in node.Attributes)
            {
                if (att.Name.Equals("msg", StringComparison.OrdinalIgnoreCase))
                {
                    ErrorMsg = att.InnerXml.Substring(6).Trim();
                }
                else if (att.Name.Equals("code", StringComparison.OrdinalIgnoreCase))
                {
                    Int32.TryParse(att.InnerXml, out ErrorCode);
                }
            }
            return true;
        }

        public bool isOnline()
        {
            string cmd = "isOnline";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    //<Ack>
                    if (node.InnerXml.StartsWith("OK:", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public System.Collections.ArrayList getCaptureTagsRawWithinDuration(string time)
        {
            System.Collections.ArrayList tags = new System.Collections.ArrayList();
            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command=getCaptureTagsRaw&mode=getTagWithinDuration&duration={1}&session_id={2}", httpUri.AbsoluteUri, time, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null)
                    return null;

                XmlDataDocument doc = new XmlDataDocument();

                doc.LoadXml(resp);

                XmlElement element = doc.DocumentElement;
                if (element.Name == "CSL")
                {
                    XmlNode node = doc.SelectSingleNode("CSL/Command");
                    if (node != null)
                    {
                        if (node.InnerXml.Equals("getcapturetagsraw", StringComparison.OrdinalIgnoreCase))
                        {
                            node = doc.SelectSingleNode("CSL/TagList/tagEPC");
                            if (node != null)
                            {
                                //<Ack>
                                TAG tag;
                                while (node != null)
                                {
                                    tag = new TAG();
                                    foreach (XmlAttribute att in node.Attributes)
                                    {
                                        if (att.Name.Equals("capturepoint_id", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.Antenna = att.InnerXml;
                                        }
                                        else if (att.Name.Equals("capturepoint_name", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.CapturePointId = att.InnerXml;
                                        }
                                        else if (att.Name.Equals("freq", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (att.InnerXml != null)
                                            {
                                                tag.Frequency = getFreq(att.InnerXml);
                                            }
                                        }
                                        else if (att.Name.Equals("index", StringComparison.OrdinalIgnoreCase))
                                        {
                                            string s = att.InnerXml;
                                            if (s.Length > 1)
                                            {
                                                tag.Index = s.Substring(1);
                                                tag.session = s.Substring(0, 1);
                                            }

                                        }
                                        else if (att.Name.Equals("rssi", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.RSSI = double.Parse(att.InnerXml);
                                        }
                                        else if (att.Name.Equals("tag_id", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.TagOrigId = att.InnerXml;
                                        }
                                        else if (att.Name.Equals("time", StringComparison.OrdinalIgnoreCase))
                                        {
                                            int.TryParse(att.InnerXml, out tag.Time);
                                        }
                                        else if (att.Name.Equals("event_id", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.EventId = att.InnerXml;
                                        }
                                        else if (att.Name.Equals("reader_ip", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.ServerIp = att.InnerXml;
                                        }
                                    }
                                    //tag.ApiTimeStampUTC = DateTime.UtcNow;
                                    tag.ApiTimeStampUTC = ConvertFromUnixTimestamp (tag.Time);
                                    tag.ServerIp = httpUri.Host;
                                    tags.Add(tag);
                                    node = node.NextSibling;
                                }
                                ErrorCode = ERR_CODE_NO_ERROR;
                                ErrorMsg = "";
                                return tags;
                            }
                            node = doc.SelectSingleNode("CSL/Error");
                            if (node != null)
                            {
                                //<Error>
                                foreach (XmlAttribute att in node.Attributes)
                                {
                                    if (att.Name.Equals("msg", StringComparison.OrdinalIgnoreCase))
                                    {
                                        ErrorMsg = att.InnerXml.Substring(6).Trim();
                                    }
                                    else if (att.Name.Equals("code", StringComparison.OrdinalIgnoreCase))
                                    {
                                        Int32.TryParse(att.InnerXml, out ErrorCode);
                                    }
                                }
                                return null;
                            }
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                ErrorMsg = wex.Message;
                return null;
            }
            ErrorMsg = "Invalid isOnline response.";
            return null;
        }

        public System.Collections.ArrayList getCaptureTagsRaw(string mode)
        {
            System.Collections.ArrayList tags = new System.Collections.ArrayList();
            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command=getCaptureTagsRaw&mode={1}&session_id={2}", httpUri.AbsoluteUri, mode, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null)
                    return null;

                XmlDataDocument doc = new XmlDataDocument();

                doc.LoadXml(resp);

                XmlElement element = doc.DocumentElement;
                if (element.Name == "CSL")
                {
                    XmlNode node = doc.SelectSingleNode("CSL/Command");
                    if (node != null)
                    {
                        if (node.InnerXml.Equals("getcapturetagsraw", StringComparison.OrdinalIgnoreCase))
                        {
                            node = doc.SelectSingleNode("CSL/TagList/tagEPC");
                            if (node != null)
                            {
                                //<Ack>
                                TAG tag;
                                while (node != null)
                                {
                                    tag = new TAG();
                                    foreach (XmlAttribute att in node.Attributes)
                                    {
                                        if (att.Name.Equals("capturepoint_id", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.Antenna = att.InnerXml;
                                        }
                                        else if (att.Name.Equals("capturepoint_name", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.CapturePointId = att.InnerXml;
                                        }
                                        else if (att.Name.Equals("freq", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (att.InnerXml != null)
                                            {
                                                tag.Frequency = getFreq(att.InnerXml);
                                            }
                                        }
                                        else if (att.Name.Equals("index", StringComparison.OrdinalIgnoreCase))
                                        {
                                            string s = att.InnerXml;
                                            if (s.Length > 1)
                                            {
                                                tag.Index = s.Substring(1);
                                                tag.session = s.Substring(0, 1);
                                            }

                                        }
                                        else if (att.Name.Equals("rssi", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.RSSI = double.Parse(att.InnerXml);
                                        }
                                        else if (att.Name.Equals("tag_id", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.TagOrigId = att.InnerXml;
                                        }
                                        else if (att.Name.Equals("time", StringComparison.OrdinalIgnoreCase))
                                        {
                                            int.TryParse(att.InnerXml, out tag.Time);
                                        }
                                        else if (att.Name.Equals("event_id", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.EventId = att.InnerXml;
                                        }
                                        else if (att.Name.Equals("reader_ip", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.ServerIp = att.InnerXml;
                                        }
                                    }
                                    //tag.ApiTimeStampUTC = DateTime.UtcNow;
                                    tag.ApiTimeStampUTC = ConvertFromUnixTimestamp(tag.Time);
                                    tag.ServerIp = httpUri.Host;
                                    tags.Add(tag);
                                    node = node.NextSibling;
                                }
                                ErrorCode = ERR_CODE_NO_ERROR;
                                ErrorMsg = "";
                                return tags;
                            }
                            node = doc.SelectSingleNode("CSL/Error");
                            if (node != null)
                            {
                                //<Error>
                                foreach (XmlAttribute att in node.Attributes)
                                {
                                    if (att.Name.Equals("msg", StringComparison.OrdinalIgnoreCase))
                                    {
                                        ErrorMsg = att.InnerXml.Substring(6).Trim();
                                    }
                                    else if (att.Name.Equals("code", StringComparison.OrdinalIgnoreCase))
                                    {
                                        Int32.TryParse(att.InnerXml, out ErrorCode);
                                    }
                                }
                                return null;
                            }
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                ErrorMsg = wex.Message;
                return null;
            }
            ErrorMsg = "Invalid isOnline response.";
            return null;
        }

        public System.Collections.ArrayList getCaptureTagsRawEPC()
        {
            System.Collections.ArrayList tags = new System.Collections.ArrayList();
            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command=getCaptureTagsRaw&mode=getEPC&session_id={1}", httpUri.AbsoluteUri, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null)
                    return null;

                XmlDataDocument doc = new XmlDataDocument();

                doc.LoadXml(resp);

                XmlElement element = doc.DocumentElement;
                if (element.Name == "CSL")
                {
                    XmlNode node = doc.SelectSingleNode("CSL/Command");
                    if (node != null)
                    {
                        if (node.InnerXml.Equals("getcapturetagsraw", StringComparison.OrdinalIgnoreCase))
                        {
                            node = doc.SelectSingleNode("CSL/TagList/tagEPC");
                            if (node != null)
                            {
                                //<Ack>
                                TAG tag;
                                while (node != null)
                                {
                                    tag = new TAG();
                                    foreach (XmlAttribute att in node.Attributes)
                                    {
                                        if (att.Name.Equals("capturepoint_id", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.Antenna = att.InnerXml;
                                        }
                                        else if (att.Name.Equals("capturepoint_name", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.CapturePointId = att.InnerXml;
                                        }
                                        else if (att.Name.Equals("freq", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (att.InnerXml != null)
                                            {
                                                tag.Frequency = getFreq(att.InnerXml);
                                            }
                                        }
                                        else if (att.Name.Equals("index", StringComparison.OrdinalIgnoreCase))
                                        {
                                            string s = att.InnerXml;
                                            if (s.Length > 1)
                                            {
                                                tag.Index = s.Substring(1);
                                                tag.session = s.Substring(0, 1);
                                            }

                                        }
                                        else if (att.Name.Equals("rssi", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.RSSI = double.Parse(att.InnerXml);
                                        }
                                        else if (att.Name.Equals("tag_id", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.TagOrigId = att.InnerXml;
                                        }
                                        else if (att.Name.Equals("time", StringComparison.OrdinalIgnoreCase))
                                        {
                                            int.TryParse(att.InnerXml, out tag.Time);
                                        }
                                        else if (att.Name.Equals("event_id", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.EventId = att.InnerXml;
                                        }
                                        else if (att.Name.Equals("reader_ip", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.ServerIp = att.InnerXml;
                                        }
                                    }
                                    //tag.ApiTimeStampUTC = DateTime.UtcNow;
                                    tag.ApiTimeStampUTC = ConvertFromUnixTimestamp(tag.Time);
                                    tag.ServerIp = httpUri.Host;
                                    tags.Add(tag);
                                    node = node.NextSibling;
                                }
                                ErrorCode = ERR_CODE_NO_ERROR;
                                ErrorMsg = "";
                                return tags;
                            }
                            node = doc.SelectSingleNode("CSL/Error");
                            if (node != null)
                            {
                                //<Error>
                                foreach (XmlAttribute att in node.Attributes)
                                {
                                    if (att.Name.Equals("msg", StringComparison.OrdinalIgnoreCase))
                                    {
                                        ErrorMsg = att.InnerXml.Substring(6).Trim();
                                    }
                                    else if (att.Name.Equals("code", StringComparison.OrdinalIgnoreCase))
                                    {
                                        Int32.TryParse(att.InnerXml, out ErrorCode);
                                    }
                                }
                                return null;
                            }
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                ErrorMsg = wex.Message;
                return null;
            }
            ErrorMsg = "Invalid isOnline response.";
            return null;
        }

        public System.Collections.ArrayList getCaptureTagsRawAllBanks()
        {
            System.Collections.ArrayList tags = new System.Collections.ArrayList();
            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command=getCaptureTagsRaw&mode=getAllBanks&session_id={1}", httpUri.AbsoluteUri, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null)
                    return null;

                XmlDataDocument doc = new XmlDataDocument();

                doc.LoadXml(resp);

                XmlElement element = doc.DocumentElement;
                if (element.Name == "CSL")
                {
                    XmlNode node = doc.SelectSingleNode("CSL/Command");
                    if (node != null)
                    {
                        if (node.InnerXml.Equals("getcapturetagsraw", StringComparison.OrdinalIgnoreCase))
                        {
                            node = doc.SelectSingleNode("CSL/TagList/tagEPC");
                            if (node != null)
                            {
                                //<Ack>
                                TAG tag;
                                while (node != null)
                                {
                                    tag = new TAG();
                                    foreach (XmlAttribute att in node.Attributes)
                                    {
                                        if (att.Name.Equals("capturepoint_id", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.Antenna = att.InnerXml;
                                        }
                                        else if (att.Name.Equals("capturepoint_name", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.CapturePointId = att.InnerXml;
                                        }
                                        else if (att.Name.Equals("freq", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (att.InnerXml != null)
                                            {
                                                tag.Frequency = getFreq(att.InnerXml);
                                            }
                                        }
                                        else if (att.Name.Equals("index", StringComparison.OrdinalIgnoreCase))
                                        {
                                            string s = att.InnerXml;
                                            if (s.Length > 1)
                                            {
                                                tag.Index = s.Substring(1);
                                                tag.session = s.Substring(0, 1);
                                            }

                                        }
                                        else if (att.Name.Equals("rssi", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.RSSI = double.Parse(att.InnerXml);
                                        }
                                        else if (att.Name.Equals("tag_id", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.TagOrigId = att.InnerXml;
                                        }
                                        else if (att.Name.Equals("time", StringComparison.OrdinalIgnoreCase))
                                        {
                                            int.TryParse(att.InnerXml, out tag.Time);
                                        }
                                        else if (att.Name.Equals("event_id", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.EventId = att.InnerXml;
                                        }
                                        else if (att.Name.Equals("reader_ip", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.ServerIp = att.InnerXml;
                                        }
                                    }
                                    //tag.ApiTimeStampUTC = DateTime.UtcNow;
                                    tag.ApiTimeStampUTC = ConvertFromUnixTimestamp(tag.Time);
                                    tag.ServerIp = httpUri.Host;
                                    tags.Add(tag);
                                    node = node.NextSibling;
                                }
                                ErrorCode = ERR_CODE_NO_ERROR;
                                ErrorMsg = "";
                                return tags;
                            }
                            node = doc.SelectSingleNode("CSL/Error");
                            if (node != null)
                            {
                                //<Error>
                                foreach (XmlAttribute att in node.Attributes)
                                {
                                    if (att.Name.Equals("msg", StringComparison.OrdinalIgnoreCase))
                                    {
                                        ErrorMsg = att.InnerXml.Substring(6).Trim();
                                    }
                                    else if (att.Name.Equals("code", StringComparison.OrdinalIgnoreCase))
                                    {
                                        Int32.TryParse(att.InnerXml, out ErrorCode);
                                    }
                                }
                                return null;
                            }
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                ErrorMsg = wex.Message;
                return null;
            }
            ErrorMsg = "Invalid isOnline response.";
            return null;
        }

        public System.Collections.ArrayList getTagDataAllBanks(string mb, ushort timeout)
        {
            System.Collections.ArrayList tags = new System.Collections.ArrayList();
            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command=getCaptureTagsRaw&mode=getAllBanks&session_id={1}", httpUri.AbsoluteUri, SessionId));
                sbReq.Append(String.Format("&bankSelected={0}&timeOut={1}", mb, timeout));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null)
                    return null;

                XmlDataDocument doc = new XmlDataDocument();

                doc.LoadXml(resp);

                XmlElement element = doc.DocumentElement;
                if (element.Name == "CSL")
                {
                    XmlNode node = doc.SelectSingleNode("CSL/Command");
                    if (node != null)
                    {
                        if (node.InnerXml.Equals("getcapturetagsraw", StringComparison.OrdinalIgnoreCase))
                        {
                            node = doc.SelectSingleNode("CSL/tag");
                            if (node != null)
                            {
                                //<Ack>
                                TAG tag;
                                while (node != null)
                                {
                                    tag = new TAG();
                                    foreach (XmlAttribute att in node.Attributes)
                                    {
                                        if (att.Name.Equals("tag_id", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.TagOrigId = att.InnerXml.Trim();
                                        }
                                        else if (att.Name.Equals("index", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.Index = att.InnerXml.Trim();
                                        }
                                        else if (att.Name.Equals("bank0", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.Bank0 = att.InnerXml.Trim();
                                        }
                                        else if (att.Name.Equals("bank2", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.Bank2 = att.InnerXml.Trim();
                                        }
                                        else if (att.Name.Equals("bank3", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.Bank3 = att.InnerXml.Trim();
                                        }
                                        else if (att.Name.Equals("pc", StringComparison.OrdinalIgnoreCase))
                                        {
                                            tag.PC = att.InnerXml.Trim();
                                        }
                                    }
                                    //tag.ApiTimeStampUTC = DateTime.UtcNow;
                                    tag.ApiTimeStampUTC = ConvertFromUnixTimestamp(tag.Time);
                                    tags.Add(tag);
                                    node = node.NextSibling;
                                }
                                ErrorCode = ERR_CODE_NO_ERROR;
                                ErrorMsg = "";
                                return tags;
                            }
                            node = doc.SelectSingleNode("CSL/Error");
                            if (node != null)
                            {
                                //<Error>
                                foreach (XmlAttribute att in node.Attributes)
                                {
                                    if (att.Name.Equals("msg", StringComparison.OrdinalIgnoreCase))
                                    {
                                        ErrorMsg = att.InnerXml.Substring(6).Trim();
                                    }
                                    else if (att.Name.Equals("code", StringComparison.OrdinalIgnoreCase))
                                    {
                                        Int32.TryParse(att.InnerXml, out ErrorCode);
                                    }
                                }
                                return null;
                            }
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                ErrorMsg = wex.Message;
                return null;
            }
            ErrorMsg = "Invalid isOnline response.";
            return null;
        }

        private string sendHTTPRequest(string request)
        {
            saveToLogVerbose(request);

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(request);
                req.Timeout = http_timeout;    //30 seconds

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                Stream respStream = resp.GetResponseStream();

                StringBuilder sb = new StringBuilder();
                byte[] buf = new byte[8192];
                string tempString = null;
                int count = 0;

                do
                {
                    count = respStream.Read(buf, 0, buf.Length);

                    if (count != 0)
                    {
                        // translate from bytes to ASCII text
                        tempString = Encoding.ASCII.GetString(buf, 0, count);

                        // continue building the string
                        sb.Append(tempString);
                    }
                }
                while (count > 0); // any more data to read?

                saveToLogVerbose(sb.ToString());
                return sb.ToString();
            }
            catch (WebException wEx)
            {
                ErrorCode = ERR_CODE_WEB_ERROR;
                ErrorMsg = wEx.Message;
                return null;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return null;
            }
        }

        public bool getAccessMode()
        {
            string cmd = "getAccessMode";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);

                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Access");
                if (node != null)
                {
                    string value = node.InnerXml;
                    XmlAttributeCollection atts = node.Attributes;
                    foreach (XmlAttribute att in atts)
                    {
                        if (att.Name.Equals("mode", StringComparison.OrdinalIgnoreCase))
                        {
                            Int32 mode;
                            if (Int32.TryParse(att.Value, out mode))
                            {
                                if (mode == 1)
                                {
                                    ErrorCode = ERR_CODE_NO_ERROR;
                                    ErrorMsg = "";
                                    return true;
                                }
                            }
                        }
                    }
                    return false;
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool setAccessMode(bool mode)
        {
            string cmd = "setAccessMode";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            string mode_str = "low";
            if (mode == API_MODE.High)
            {
                mode_str = "high";
            }

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}&mode={3}", httpUri.AbsoluteUri, cmd, SessionId, mode_str));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);

                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool setAccessMode_WS(bool mode)
        {
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            string mode_str = (mode == API_MODE.High) ? "1.+High+Level+HTTP+API+Mode" : "2. Low Level Mach1 API Mode";

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}setAccessMode?oper=setAccessMode&access_mode={1}", httpUri.AbsoluteUri, mode_str));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                if (resp.Contains("Successful") == true) return true;
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool setReaderID(READER_ID reader_id)
        {
            string cmd = "setReaderID";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}&reader_id={3}&desc={4}", httpUri.AbsoluteUri, cmd, SessionId, reader_id.id, reader_id.desc));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool getReaderID(out READER_ID rdr)
        {
            string cmd = "getReaderID";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            rdr.id = "";
            rdr.desc = "";

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Reader");
                if (node != null)
                {
                    XmlAttributeCollection atts = node.Attributes;
                    if (atts == null) return false;
                    node = atts.GetNamedItem("desc");
                    if (node == null) return false;
                    rdr.desc = node.Value;
                    node = atts.GetNamedItem("reader_id");
                    if (node == null) return false;
                    rdr.id = node.Value;

                    ErrorCode = ERR_CODE_NO_ERROR;
                    ErrorMsg = "";
                    return true;
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public System.Collections.ArrayList getOperProfile()
        {
            string cmd = "getOperProfile";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            System.Collections.ArrayList list = new System.Collections.ArrayList();
            string cp1_name = null;
            string cp2_name = null;
            string cp3_name = null;
            string cp4_name = null;

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return null;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return null;

                XmlNode node = doc.SelectSingleNode("CSL/CapturePointList/capturepoint");
                if (node != null)
                {
                    while (node != null)
                    {
                        XmlAttributeCollection atts = node.Attributes;
                        if (atts.GetNamedItem("id").Value.Equals("Antenna1"))
                        {
                            cp1_name = atts.GetNamedItem("name").Value;
                        }
                        else if (atts.GetNamedItem("id").Value.Equals("Antenna2"))
                        {
                            cp2_name = atts.GetNamedItem("name").Value;
                        }
                        else if (atts.GetNamedItem("id").Value.Equals("Antenna3"))
                        {
                            cp3_name = atts.GetNamedItem("name").Value;
                        }
                        else if (atts.GetNamedItem("id").Value.Equals("Antenna4"))
                        {
                            cp4_name = atts.GetNamedItem("name").Value;
                        }
                        node = node.NextSibling;
                    }
                }
                node = doc.SelectSingleNode("CSL/ProfileList/profile");
                if (node != null)
                {
                    while (node != null)
                    {
                        OPERATION_PROFILE op = new OPERATION_PROFILE();
                        op.ant1_enable = false;
                        op.ant2_enable = false;
                        op.ant3_enable = false;
                        op.ant4_enable = false;
                        op.profile_enable = false;

                        XmlAttributeCollection atts = node.Attributes;
                        op.profile_id = atts.GetNamedItem("profile_id").Value;
                        op.capture_mode = atts.GetNamedItem("captureMode").Value;
                        op.modulation_profile = atts.GetNamedItem("modulationProfile").Value;
                        Int32.TryParse(atts.GetNamedItem("populationEst").Value, out op.population);
                        if (atts.GetNamedItem("enable").Value.Equals("true", StringComparison.OrdinalIgnoreCase))
                            op.profile_enable = true;
                        Int32.TryParse(atts.GetNamedItem("sessionNo").Value, out op.session_no);
                        op.transmit_power = atts.GetNamedItem("transmitPower").Value;
                        Int32.TryParse(atts.GetNamedItem("duplicateEliminationTime").Value, out op.window_time);
                        op.ant1_name = cp1_name;
                        op.ant2_name = cp2_name;
                        op.ant3_name = cp3_name;
                        op.ant4_name = cp4_name;
                        string ports = atts.GetNamedItem("antennaPort").Value;
                        if (ports.Contains("1"))
                            op.ant1_enable = true;
                        if (ports.Contains("2"))
                            op.ant2_enable = true;
                        if (ports.Contains("3"))
                            op.ant3_enable = true;
                        if (ports.Contains("4"))
                            op.ant4_enable = true;
                        op.trigger = atts.GetNamedItem("triggerMethod").Value;

                        list.Add(op);
                        node = node.NextSibling;
                    }

                    ErrorCode = ERR_CODE_NO_ERROR;
                    ErrorMsg = "";

                    return list;
                }

                parseErrorCode(ref doc);
                return null;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return null;
            }
        }

        public bool setOperProfile(OPERATION_PROFILE profile)
        {
            string cmd = "setOperProfile";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();
                string enable = "false";
                if (profile.profile_enable)
                    enable = "true";
                string antennaPort = "";
                if (profile.ant1_enable)
                    antennaPort += "1";
                if (profile.ant2_enable)
                    antennaPort += "2";
                if (profile.ant3_enable)
                    antennaPort += "3";
                if (profile.ant4_enable)
                    antennaPort += "4";

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&profile_id={0}&captureMode={1}&duplicateEliminationTime={2}", profile.profile_id, profile.capture_mode, profile.window_time));
                sbReq.Append(String.Format("&modulationProfile={0}&populationEst={1}&sessionNo={2}", profile.modulation_profile, profile.population, profile.session_no));
                sbReq.Append(String.Format("&transmitPower={0}&antennaPort={1}&enable={2}", profile.transmit_power, antennaPort, enable));
                sbReq.Append(String.Format("&triggerMethod={0}", profile.trigger));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }

                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool setOperProfile_TxPowers(OPERATION_PROFILE profile)
        {
            string cmd = "setOperProfile";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();
                string enable = "false";
                if (profile.profile_enable)
                    enable = "true";
                string antennaPort = "";
                if (profile.ant1_enable)
                    antennaPort += "1";
                if (profile.ant2_enable)
                    antennaPort += "2";
                if (profile.ant3_enable)
                    antennaPort += "3";
                if (profile.ant4_enable)
                    antennaPort += "4";

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&profile_id={0}&captureMode={1}&duplicateEliminationTime={2}", profile.profile_id, profile.capture_mode, profile.window_time));
                sbReq.Append(String.Format("&modulationProfile={0}&populationEst={1}&sessionNo={2}", profile.modulation_profile, profile.population, profile.session_no));
                sbReq.Append(String.Format("&antennaPort={0}&enable={1}", antennaPort, enable));
                sbReq.Append(String.Format("&triggerMethod={0}", profile.trigger));
                sbReq.Append(String.Format("&transmitPower1={0}&&transmitPower2={1}&transmitPower3={2}&transmitPower4={3}", profile.ant1_power, profile.ant2_power, profile.ant3_power, profile.ant4_power));

                sbReq.Append(String.Format("&antennaPortScheme={0}", profile.antennaPortScheme));
                if (profile.memoryBank.Equals("None") == false)
                {
                    sbReq.Append(String.Format("&tagModel={0}&memoryBank={1}", profile.tagModel, profile.memoryBank));
                }

                if (profile.RxSensitivity == (int)OPERATION_PROFILE.RxSensitivityOption.MaximumSensitivity)
                {
                    sbReq.Append("&RxSensitivity=0");
                }
                else
                {
                    sbReq.Append(String.Format("&RxSensitivity=1&RxSensitivityValue1={0}&RxSensitivityValue2={1}&RxSensitivityValue3={2}&RxSensitivityValue4={3}",
                        profile.RxSensitivityValue1, profile.RxSensitivityValue2, profile.RxSensitivityValue3, profile.RxSensitivityValue4));
                }

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }

                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public string[] getCapturePointName()
        {
            string cmd = "getCapturePointName";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            string[] name = new string[4];

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return name;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return name;

                XmlNode node = doc.SelectSingleNode("CSL/capturepoint");
                if (node != null)
                {
                    while (node != null)
                    {
                        XmlAttributeCollection atts = node.Attributes;
                        if (atts.GetNamedItem("id").Value.Equals("Antenna1"))
                        {
                            name[0] = atts.GetNamedItem("name").Value;
                        }
                        else if (atts.GetNamedItem("id").Value.Equals("Antenna2"))
                        {
                            name[1] = atts.GetNamedItem("name").Value;
                        }
                        else if (atts.GetNamedItem("id").Value.Equals("Antenna3"))
                        {
                            name[2] = atts.GetNamedItem("name").Value;
                        }
                        else if (atts.GetNamedItem("id").Value.Equals("Antenna4"))
                        {
                            name[3] = atts.GetNamedItem("name").Value;
                        }
                        node = node.NextSibling;
                    }
                    ErrorCode = ERR_CODE_NO_ERROR;
                    ErrorMsg = "";

                    return name;
                }

                parseErrorCode(ref doc);
                return name;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return name;
            }
        }

        public bool setCapturePointName(string ant, string name)
        {
            string cmd = "setCapturePointName";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&capturepoint_id={0}&name={1}", ant, name));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }

                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public System.Collections.ArrayList listServer()
        {
            string cmd = "listServer";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            System.Collections.ArrayList list = new System.Collections.ArrayList();

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return null;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return null;

                XmlNode node = doc.SelectSingleNode("CSL/ServerList/Server");
                if (node != null)
                {
                    while (node != null)
                    {
                        SERVER_INFO si = new SERVER_INFO();
                        si.enable = false;

                        XmlAttributeCollection atts = node.Attributes;
                        si.id = atts.GetNamedItem("server_id").Value;
                        si.desc = atts.GetNamedItem("desc").Value;
                        si.ip = atts.GetNamedItem("server_ip").Value;
                        si.server_port = atts.GetNamedItem("server_port").Value;
                        si.reader_port = atts.GetNamedItem("reader_port").Value;
                        si.mode = atts.GetNamedItem("mode").Value;
                        if (atts.GetNamedItem("enable").Value.Equals("true", StringComparison.OrdinalIgnoreCase))
                            si.enable = true;

                        list.Add(si);
                        node = node.NextSibling;
                    }
                    ErrorCode = ERR_CODE_NO_ERROR;
                    ErrorMsg = "";

                    return list;
                }

                parseErrorCode(ref doc);
                return null;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return null;
            }
        }

        public bool setServerID(SERVER_INFO svr)
        {
            string cmd = "setServerID";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                string enable = "false";
                if (svr.enable)
                    enable = "true";

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&server_id={0}&desc={1}", svr.id, svr.desc));
                sbReq.Append(String.Format("&server_ip={0}&server_port={1}&enable={2}", svr.ip, svr.server_port, enable));
                sbReq.Append(String.Format("&reader_ip={0}&mode={1}", svr.reader_port, svr.mode));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool modServerID(SERVER_INFO svr)
        {
            string cmd = "modServerID";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                string enable = "false";
                if (svr.enable)
                    enable = "true";

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&server_id={0}&desc={1}", svr.id, svr.desc));
                sbReq.Append(String.Format("&server_ip={0}&server_port={1}&enable={2}", svr.ip, svr.server_port, enable));
                sbReq.Append(String.Format("&reader_ip={0}&mode={1}", svr.reader_port, svr.mode));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool delServerID(string id)
        {
            string cmd = "delServerID";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&server_id={0}", id));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        // Tag Group
        public bool importTagGroupCSV(string id, string data)
        {
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                ASCIIEncoding enc = new ASCIIEncoding();
                byte[] postData = enc.GetBytes(String.Format("session_id={0}&tagGroupFilename={1}.csv&tagGroupContent={2}", session_id, id, data));

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(String.Format("{0}importTagGroupCSV", httpUri.AbsoluteUri));
                req.Timeout = http_timeout;    //30 seconds
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = postData.Length;

                Stream newStream = req.GetRequestStream();
                newStream.Write(postData, 0, postData.Length);
                newStream.Close();

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                Stream respStream = resp.GetResponseStream();

                StringBuilder sb = new StringBuilder();
                byte[] buf = new byte[8192];
                string tempString = null;
                int count = 0;

                do
                {
                    count = respStream.Read(buf, 0, buf.Length);
                    if (count != 0)
                    {
                        tempString = Encoding.ASCII.GetString(buf, 0, count);
                        sb.Append(tempString);
                    }
                }
                while (count > 0);

                if (sb.Length > 0)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(sb.ToString());
                    if (isCSLResponse(ref doc, "importTagGroupCSV") == false) return false;

                    XmlNode node = doc.SelectSingleNode("CSL/Ack");
                    if (node != null)
                    {
                        if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                        {
                            ErrorCode = ERR_CODE_NO_ERROR;
                            ErrorMsg = "";
                            return true;
                        }
                    }
                    parseErrorCode(ref doc);
                }
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool delTagGroup(string id)
        {
            string cmd = "delTagGroup";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&group_id={0}", id));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public System.Collections.ArrayList listTagGroup()
        {
            string cmd = "listTagGroup";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            System.Collections.ArrayList list = new System.Collections.ArrayList();

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return null;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return null;

                XmlNode node = doc.SelectSingleNode("CSL/TagGroupList/tagGroup");
                if (node != null)
                {
                    while (node != null)
                    {
                        XmlAttributeCollection atts = node.Attributes;
                        list.Add(atts.GetNamedItem("group_id").Value);
                        node = node.NextSibling;
                    }
                    ErrorCode = ERR_CODE_NO_ERROR;
                    ErrorMsg = "";

                    return list;
                }

                parseErrorCode(ref doc);
                return null;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return null;
            }
        }

        public bool AddTagGroupID(string id)
        {
            string cmd = "newTagGroup";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&group_id={0}", id));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool AddTagGroupMember(string id, string epcid)
        {
            string cmd = "addTagGroupMember";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&group_id={0}&tag_id={1}", id, epcid));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool DelTagGroupMember(string id, string epcid)
        {
            string cmd = "delTagGroupMember";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&group_id={0}&tag_id={1}", id, epcid));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool DelAllMembers(string id)
        {
            string cmd = "delTagGroupMemberAll";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&group_id={0}", id));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool ImportTagGroupMembers(string id, List<string> epcidlist)
        {
            string cmd = "importTagGroup";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();
                string epcidliststr = "";

                for (int i = 0; i < epcidlist.Count; i++)
                {
                    if (i > 0)
                        epcidliststr += ",";
                    epcidliststr += epcidlist[i];
                }

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&group_id={0}&tagGroupContent={1}", id, epcidliststr));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        #region TriggeringLogic
        public System.Collections.ArrayList listTriggeringLogic()
        {
            System.Collections.ArrayList list = new System.Collections.ArrayList();

            string cmd = "listTriggeringLogic";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return null;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return null;

                XmlNode node = doc.SelectSingleNode("CSL/TriggeringLogic/logic");
                if (node != null)
                {
                    while (node != null)
                    {
                        TRIGGER_INFO info = new TRIGGER_INFO();

                        XmlAttributeCollection atts = node.Attributes;
                        info.id = atts.GetNamedItem("logic_id").Value;
                        info.desc = atts.GetNamedItem("desc").Value;
                        info.mode = atts.GetNamedItem("mode").Value;
                        info.capture_point = atts.GetNamedItem("capture_point").Value;
                        info.logic = atts.GetNamedItem("logic").Value;

                        list.Add(info);
                        node = node.NextSibling;
                    }
                    ErrorCode = ERR_CODE_NO_ERROR;
                    ErrorMsg = "";

                    return list;
                }
                parseErrorCode(ref doc);
                return null;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return null;
            }
        }

        public bool addTriggeringLogic(TRIGGER_INFO info)
        {
            string cmd = "addTriggeringLogic";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&logic_id={0}&desc={1}&mode={2}", info.id, info.desc, info.mode));
                if (info.capture_point != "")
                    sbReq.Append(String.Format("&capture_point={0}", info.capture_point));
                if (info.logic != "")
                    sbReq.Append(String.Format("&logic={0}", info.logic));


                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool modTriggeringLogic(TRIGGER_INFO info)
        {
            string cmd = "modTriggeringLogic";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&logic_id={0}&desc={1}&mode={2}", info.id, info.desc, info.mode));
                if (info.capture_point != "")
                    sbReq.Append(String.Format("&capture_point={0}", info.capture_point));
                if (info.logic != "")
                    sbReq.Append(String.Format("&logic={0}", info.logic));


                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool delTriggeringLogic(string id)
        {
            string cmd = "delTriggeringLogic";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&logic_id={0}", id));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }
        #endregion

        #region ResultantAction
        public System.Collections.ArrayList listResultantAction()
        {
            string cmd = "listResultantAction";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            System.Collections.ArrayList list = new System.Collections.ArrayList();

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return null;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return null;

                XmlNode node = doc.SelectSingleNode("CSL/ResultantActionList/resultantaction");
                if (node != null)
                {
                    while (node != null)
                    {
                        RESULTANT_ACTION_INFO info = new RESULTANT_ACTION_INFO();

                        XmlAttributeCollection atts = node.Attributes;
                        info.action_id = atts.GetNamedItem("action_id").Value;
                        info.desc = atts.GetNamedItem("desc").Value;
                        info.action_mode = atts.GetNamedItem("action_mode").Value;
                        try
                        {
                            info.server_id = atts.GetNamedItem("server_id").Value;
                        }
                        catch { }
                        try
                        {
                            info.report_id = atts.GetNamedItem("report_id").Value;
                        }
                        catch { }
                        try
                        {
                            info.batchEndNtf = atts.GetNamedItem("batchEndNtf").Value.Equals("true") ? true : false;
                        }
                        catch { }
                        try
                        {
                            UInt32.TryParse(atts.GetNamedItem("close_socket").Value, out info.close_socket);
                        }
                        catch { }
                        try
                        {
                            info.console = atts.GetNamedItem("console").Value.Equals("true") ? true : false;
                        }
                        catch { }
                        try
                        {
                            info.duplicateEliminationFlag = atts.GetNamedItem("duplicateEliminationFlag").Value.Equals("true") ? true : false;
                        }
                        catch { }
                        try
                        {
                            UInt32.TryParse(atts.GetNamedItem("duration").Value, out info.duration);
                        }
                        catch { }
                        try
                        {
                            UInt32.TryParse(atts.GetNamedItem("post_action_delay").Value, out info.post_action_delay);
                        }
                        catch { }
                        try
                        {
                            UInt32.TryParse(atts.GetNamedItem("pre_action_wait").Value, out info.pre_action_wait);
                        }
                        catch { }
                        try
                        {
                            info.pulse_logic = atts.GetNamedItem("pulse_logic").Value;
                        }
                        catch { }
                        try
                        {
                            info.pulse_mode = atts.GetNamedItem("pulse_mode").Value;
                        }
                        catch { }
                        try
                        {
                            UInt32.TryParse(atts.GetNamedItem("pulse_width").Value, out info.pulse_width);
                        }
                        catch { }
                        try
                        {
                            info.report_id = atts.GetNamedItem("report_id").Value;
                        }
                        catch { }
                        try
                        {
                            info.serialPort1 = atts.GetNamedItem("serialPort1").Value.Equals("true") ? true : false;
                        }
                        catch { }
                        try
                        {
                            info.server_id = atts.GetNamedItem("server_id").Value;
                        }
                        catch { }
                        try
                        {
                            info.tagGroup = atts.GetNamedItem("tagGroup").Value;
                        }
                        catch { }
                        try
                        {
                            info.transport = atts.GetNamedItem("transport").Value;
                        }
                        catch { }

                        list.Add(info);
                        node = node.NextSibling;
                    }
                    ErrorCode = ERR_CODE_NO_ERROR;
                    ErrorMsg = "";

                    return list;
                }
                parseErrorCode(ref doc);
                return null;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return null;
            }
        }

        public bool addResultantAction(RESULTANT_ACTION_INFO info)
        {
            string cmd = "addResultantAction";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();


                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&action_id={0}&desc={1}&action_mode={2}", info.action_id, info.desc, info.action_mode));
                sbReq.Append(String.Format("&server_id={0}&report_id={1}", info.server_id, info.report_id));
                sbReq.Append(String.Format("&data_format_id=CS461 TCP Format")); // for CS463

                if (info.action_mode == "Read IATA Tag to Server")
                {
                    sbReq.Append(String.Format("&backgroundTagFiltering={0}&previousTagFiltering={1}&noOfPreviousTag={2}", info.backgroundTagFiltering, info.previousTagFiltering, info.noOfPreviousTag));
                    sbReq.Append(String.Format("&withoutDateFieldFiltering={0}&userDefinedTagListFilter={1}", info.withoutDateFieldFiltering, info.userDefinedTagListFilter));
                }

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool modResultantAction(RESULTANT_ACTION_INFO info)
        {
            string cmd = "modResultantAction";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&action_id={0}&desc={1}&action_mode={2}", info.action_id, info.desc, info.action_mode));
                sbReq.Append(String.Format("&server_id={0}&report_id={1}", info.server_id, info.report_id));
                sbReq.Append(String.Format("&data_format_id=CS461 TCP Format")); // for CS463

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool delResultantAction(string id)
        {
            string cmd = "delResultantAction";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&action_id={0}", id));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }
        #endregion

        #region Event Handling
        public System.Collections.ArrayList listEvent()
        {
            string cmd = "listEvent";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            System.Collections.ArrayList list = new System.Collections.ArrayList();

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return null;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return null;

                XmlNode node = doc.SelectSingleNode("CSL/EventList/event");
                if (node != null)
                {
                    while (node != null)
                    {
                        EVENT_INFO info = new EVENT_INFO();
                        info.enable = false;
                        info.event_log = false;

                        XmlAttributeCollection atts = node.Attributes;
                        info.event_id = atts.GetNamedItem("event_id").Value;
                        info.desc = atts.GetNamedItem("desc").Value;
                        info.operProfile_id = atts.GetNamedItem("operProfile_id").Value;
                        info.enable = (atts.GetNamedItem("enable").Value.Equals("true", StringComparison.OrdinalIgnoreCase)) ? true : false;
                        info.event_log = (atts.GetNamedItem("event_log").Value.Equals("true", StringComparison.OrdinalIgnoreCase)) ? true : false;
                        info.inventoryEnablingTrigger = atts.GetNamedItem("inventoryEnablingTrigger").Value;
                        info.inventoryDisablingTrigger = atts.GetNamedItem("inventoryDisablingTrigger").Value;
                        info.inventoryEnablingAction = atts.GetNamedItem("inventoryEnablingAction").Value;
                        info.inventoryDisablingAction = atts.GetNamedItem("inventoryDisablingAction").Value;
                        info.exclusivity = atts.GetNamedItem("exclusivity").Value;
                        info.resultant_action = atts.GetNamedItem("resultant_action").Value;
                        info.triggering_logic = atts.GetNamedItem("triggering_logic").Value;

                        list.Add(info);
                        node = node.NextSibling;
                    }
                    ErrorCode = ERR_CODE_NO_ERROR;
                    ErrorMsg = "";

                    return list;
                }
                parseErrorCode(ref doc);
                return null;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return null;
            }
        }

        public bool addEvent(EVENT_INFO info)
        {
            string cmd = "addEvent";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();
                string eventEnable = (info.enable == true) ? "true" : "false";
                string eventLog = (info.event_log == true) ? "true" : "false";

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&event_id={0}&desc={1}&triggering_logic={2}&operProfile_id={3}", info.event_id, info.desc, info.triggering_logic, info.operProfile_id));
                sbReq.Append(String.Format("&resultant_action={0}&event_log={1}&enable={2}", info.resultant_action, eventLog, eventEnable));
                sbReq.Append(String.Format("&inventoryEnablingTrigger={0}&inventoryDisablingTrigger={1}", info.inventoryEnablingTrigger, info.inventoryDisablingTrigger));
                sbReq.Append(String.Format("&inventoryEnablingAction={0}&inventoryDisablingAction={1}&exclusivity={2}", info.inventoryEnablingAction, info.inventoryDisablingAction, info.exclusivity));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool modEvent(EVENT_INFO info)
        {
            string cmd = "modEvent";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();
                string eventEnable = (info.enable == true) ? "true" : "false";
                string eventLog = (info.event_log == true) ? "true" : "false";

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&event_id={0}&desc={1}&triggering_logic={2}&operProfile_id={3}", info.event_id, info.desc, info.triggering_logic, info.operProfile_id));
                sbReq.Append(String.Format("&resultant_action={0}&event_log={1}&enable={2}", info.resultant_action, eventLog, eventEnable));
                sbReq.Append(String.Format("&inventoryEnablingTrigger={0}&inventoryDisablingTrigger={1}", info.inventoryEnablingTrigger, info.inventoryDisablingTrigger));
                sbReq.Append(String.Format("&inventoryEnablingAction={0}&inventoryDisablingAction={1}&exclusivity={2}", info.inventoryEnablingAction, info.inventoryDisablingAction, info.exclusivity));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool delEvent(string id)
        {
            string cmd = "delEvent";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&event_id={0}", id));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);

                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool enableEvent(string id, bool enable)
        {
            string cmd = "enableEvent";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&event_id={0}&enable={1}", id, (enable) ? "true" : "false"));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);

                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool disableAllEvents()
        {
            string cmd = "disableAllEvents";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);

                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        #endregion

        #region User Management
        public System.Collections.ArrayList listUsers()
        {
            System.Collections.ArrayList list = new System.Collections.ArrayList();

            string cmd = "listUsers";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null)
                    return null;

                XmlDataDocument doc = new XmlDataDocument();

                doc.LoadXml(resp);

                XmlElement element = doc.DocumentElement;
                if (element.Name == "CSL")
                {
                    XmlNode node = doc.SelectSingleNode("CSL/Command");
                    if (node != null)
                    {
                        if (node.InnerXml.Equals("listUsers", StringComparison.OrdinalIgnoreCase))
                        {
                            node = doc.SelectSingleNode("CSL/Account");
                            if (node != null)
                            {
                                while (node != null)
                                {
                                    USER_INFO info = new USER_INFO();

                                    XmlAttributeCollection atts = node.Attributes;
                                    info.username = atts.GetNamedItem("username").Value;
                                    //info.password = atts.GetNamedItem("password").Value;
                                    info.desc = atts.GetNamedItem("desc").Value;
                                    info.level = int.Parse(atts.GetNamedItem("level").Value);

                                    list.Add(info);
                                    node = node.NextSibling;
                                }
                                ErrorCode = ERR_CODE_NO_ERROR;
                                ErrorMsg = "";

                                return list;
                            }

                            node = doc.SelectSingleNode("CSL/Error");
                            if (node != null)
                            {
                                //<Error>
                                foreach (XmlAttribute att in node.Attributes)
                                {
                                    if (att.Name.Equals("msg", StringComparison.OrdinalIgnoreCase))
                                    {
                                        ErrorMsg = att.InnerXml.Substring(6).Trim();
                                    }
                                    else if (att.Name.Equals("code", StringComparison.OrdinalIgnoreCase))
                                    {
                                        Int32.TryParse(att.InnerXml, out ErrorCode);
                                    }
                                }
                                return null;
                            }
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                ErrorMsg = wex.Message;
                return null;
            }
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;
            return null;
        }

        public bool addUser(USER_INFO info)
        {
            string cmd = "addUser";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&username={0}&password={1}&desc={2}&level={3}", info.username, info.password, info.desc, info.level));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool delUser(string name)
        {
            string cmd = "delUser";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&username={0}", name));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool setUserPassword(USER_INFO user)
        {
            string cmd = "setUserPassword";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&username={0}&password={1}", user.username, user.password));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }
        #endregion

        #region login Logout
        public bool login()
        {
            string cmd = "login";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&username={2}&password={3}", httpUri.AbsoluteUri, cmd, LoginName, LoginPassword));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK:", StringComparison.OrdinalIgnoreCase))
                    {
                        //ok
                        char[] separators = new char[] { ' ', '=' };
                        string[] str = node.InnerXml.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        if (str[2].Length == 8)
                        {
                            SessionId = str[2];
                            ErrorCode = ERR_CODE_NO_ERROR;
                            ErrorMsg = "";
                            return true;
                        }
                        ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                        ErrorMsg = "Invalid session id.";
                        return false;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch (WebException wex)
            {
                ErrorCode = ERR_CODE_WEB_ERROR;
                ErrorMsg = wex.Message;
                return false;
            }
        }

        public bool logout()
        {
            string cmd = "logout";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);

                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");

                if (node != null)
                {
                    //<Ack>
                    if (node.InnerXml.StartsWith("OK:", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool forceLogout()
        {
            string cmd = "forceLogout";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&username={2}&password={3}", httpUri.AbsoluteUri, cmd, LoginName, LoginPassword));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);

                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");

                if (node != null)
                {
                    //<Ack>
                    if (node.InnerXml.StartsWith("OK:", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool setAutoLogoutTime(int time)
        {
            string cmd = "setAutoLogoutTime";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&time={0}", time));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);

                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");

                if (node != null)
                {
                    //<Ack>
                    if (node.InnerXml.StartsWith("OK:", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }
        #endregion

        #region Information
        public READER_STATUS getReaderStatus()
        {
            string cmd = "getReaderStatus";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            READER_STATUS status = new READER_STATUS();
            int year, month, day, hour, minute, second;

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&username={2}&password={3}", httpUri.AbsoluteUri, cmd, LoginName, LoginPassword));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return null;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return null;

                XmlNode node = doc.SelectSingleNode("CSL/Model");
                if (node != null)
                {
                    XmlAttributeCollection atts = node.Attributes;
                    status.Model = atts["name"].InnerXml;
                    status.Protocol_Supported = atts["protocol"].InnerXml;
                }
                else
                {
                    parseErrorCode(ref doc);
                    return null;
                }

                node = doc.SelectSingleNode("CSL/Reader");
                if (node != null)
                {
                    XmlAttributeCollection atts = node.Attributes;
                    status.Description = atts["desc"].InnerXml;
                    status.ID = atts["reader_id"].InnerXml;
                }

                node = doc.SelectSingleNode("CSL/ReaderVersion");
                if (node != null)
                {
                    XmlAttributeCollection atts = node.Attributes;
                    status.DSP_Version = atts["dsp"].InnerXml;
                    status.EdgeServer_Version = atts["edgeServer"].InnerXml;
                    status.FPGA_Version = atts["fpga"].InnerXml;
                    status.LibMapi_Version = atts["libMapi"].InnerXml;
                    status.Middleware_Version = atts["middleware"].InnerXml;
                    status.ModemController_Version = atts["modemController"].InnerXml;
                    status.Firmware_Version = atts["version"].InnerXml;
                }

                node = doc.SelectSingleNode("CSL/Timezone");
                if (node != null)
                {
                    XmlAttributeCollection atts = node.Attributes;
                    status.DaylightSaving = atts["daylight_saving"].InnerXml;
                    status.TimeZone = atts["tz"].InnerXml;
                }

                node = doc.SelectSingleNode("CSL/Logout");
                if (node != null)
                {
                    XmlAttributeCollection atts = node.Attributes;
                    status.Session_Timeout = atts["time"].InnerXml;
                }

                node = doc.SelectSingleNode("CSL/UserStatus");
                if (node != null)
                {
                    XmlAttributeCollection atts = node.Attributes;
                    status.Client_IP = atts["client_ip"].InnerXml;
                    status.Host_IP = atts["host_ip"].InnerXml;
                    status.User_Level = atts["level"].InnerXml;
                    status.Login_Status = atts["login_status"].InnerXml;
                    status.Session_Id = atts["session_id"].InnerXml;
                    status.Username = atts["username"].InnerXml;
                }

                node = doc.SelectSingleNode("CSL/AccessMode");
                if (node != null)
                {
                    XmlAttributeCollection atts = node.Attributes;
                    status.Access_Mode = atts["mode"].InnerXml;
                    status.Access_Mode_Description = atts["name"].InnerXml;
                }

                node = doc.SelectSingleNode("CSL/OperationProfile");
                if (node != null)
                {
                    XmlAttributeCollection atts = node.Attributes;
                    status.Antenna_Ports = atts["antennaPort"].InnerXml;
                    status.Capture_Mode = atts["captureMode"].InnerXml;
                    status.Profile_ID = atts["profile_id"].InnerXml;
                    status.Trigger_Methed = atts["triggerMethod"].InnerXml;
                }

                status.Active_Event = new System.Collections.ArrayList();
                node = doc.SelectSingleNode("CSL/ActiveEventList/Event");
                while (node != null)
                {
                    EVENT_INFO eventInfo = new EVENT_INFO();
                    XmlAttributeCollection atts = node.Attributes;
                    eventInfo.event_id = atts["event_id"].InnerXml;
                    eventInfo.desc = atts["desc"].InnerXml;
                    status.Active_Event.Add(eventInfo);
                    node = node.NextSibling;
                }

                node = doc.SelectSingleNode("CSL/CurrentLocalTime");
                if (node != null)
                {
                    XmlAttributeCollection atts = node.Attributes;
                    year = int.Parse(atts["year"].InnerXml);
                    month = int.Parse(atts["month"].InnerXml);
                    day = int.Parse(atts["day"].InnerXml);
                    hour = int.Parse(atts["hour"].InnerXml);
                    minute = int.Parse(atts["minute"].InnerXml);
                    second = int.Parse(atts["second"].InnerXml);
                    status.Local_Time = new DateTime(year, month, day, hour, minute, second);
                }

                node = doc.SelectSingleNode("CSL/CurrentUTCTime");
                if (node != null)
                {
                    XmlAttributeCollection atts = node.Attributes;
                    year = int.Parse(atts["year"].InnerXml);
                    month = int.Parse(atts["month"].InnerXml);
                    day = int.Parse(atts["day"].InnerXml);
                    hour = int.Parse(atts["hour"].InnerXml);
                    minute = int.Parse(atts["minute"].InnerXml);
                    second = int.Parse(atts["second"].InnerXml);
                    status.UTC_Time = new DateTime(year, month, day, hour, minute, second);
                }

                ErrorCode = ERR_CODE_NO_ERROR;
                ErrorMsg = "";
                return status;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return null;
            }
        }

        public HEALT_INFO healtCheck()
        {
            string cmd = "healthCheck";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            HEALT_INFO info = new HEALT_INFO();

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&username={2}&password={3}", httpUri.AbsoluteUri, cmd, LoginName, LoginPassword));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return null;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return null;

                XmlNode node = doc.SelectSingleNode("CSL/result");
                if (node != null)
                {
                    XmlAttributeCollection atts = node.Attributes;
                    info.Check_Time = atts["checkTime"].InnerXml;
                    info.EdgeServer = atts["edgeServer"].InnerXml;
                    info.Free_RAM = atts["freeRAM"].InnerXml;
                    info.ModemController = atts["modemController"].InnerXml;
                    info.RFID_App = atts["rfidApp"].InnerXml;
                    info.Up_Time = atts["upTime"].InnerXml;
                }
                else
                {
                    parseErrorCode(ref doc);
                    return null;
                }


                ErrorCode = ERR_CODE_NO_ERROR;
                ErrorMsg = "";
                return info;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return null;
            }
        }
        #endregion

        public void saveToLogVerbose(string msg)
        {
            if (ApiLogLevel >= LOG_LEVEL.Verbose)
            {
                saveToLog(msg);
            }
        }

        public void saveToLogInfo(string msg)
        {
            if (ApiLogLevel >= LOG_LEVEL.Info)
            {
                saveToLog(msg);
            }
        }

        private void saveToLog(string msg)
        {
            lock (this)
            {
                StreamWriter sw = new StreamWriter(Application.StartupPath + "\\api.log", true);
                sw.WriteLine(String.Format("{0}|{1}\n", DateTime.Now.ToLocalTime(), msg));
                sw.Close();
            }
        }

        public bool startInventory()
        {
            string cmd = "startInventory";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}&mode=pollingTrigger", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool startInventory(string mode, string model, string bank)
        {
            string cmd = "startInventory";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}&mode={3}", httpUri.AbsoluteUri, cmd, SessionId, mode));
                if (model.Length > 0)
                    sbReq.Append(String.Format("&tag_model={0}", model));
                if (bank.Length > 0)
                    sbReq.Append(String.Format("&bankSelected={0}", bank));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool stopInventory()
        {
            string cmd = "startInventory";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}&mode=readStop", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool purgeAllTags()
        {
            string cmd = "startInventory";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}&mode=purgeAllTags", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool connect()
        {
            if (isOnline() == true) return true;

            //Not login yet, or cannot connect to server
            if (ErrorCode == ERR_CODE_WEB_ERROR) return false;

            if (login() == true) return true;

            //Cannot login, may have other people using reader
            forceLogout();

            if (login() == false) return false;

            //check is the reader in high level api mode.
            if (getAccessMode() == API_MODE.High) return true;

            if (setAccessMode(API_MODE.High) == true) return true;

            if (setAccessMode_WS(API_MODE.High) == true) return true;   //work around for 2.2.0 SP18 RC43

            return false;
        }

        public bool setIOPort(int io, int value)
        {
            if (runIO_output(io, value, "run") == false) return false;
            //            Thread.Sleep(100);
            //            if (runIO_output(io, value, "check") == false) return false;
            return true;
        }

        public bool runIO_output(int io, int data, string mode)
        {
            string cmd = "runIO_output";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                if (mode == "run")
                {
                    sbReq.Append(String.Format("&mode=run&port={0}&oper_logic={1}", io, data));
                }
                else
                {
                    sbReq.Append(String.Format("&mode=check", mode, io, data));
                }

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public GPI_INFO directIO_input()
        {
            string cmd = "directIOInput";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&mode=syncGet", httpUri.AbsoluteUri, cmd));
                sbReq.Append(String.Format("&username={0}&password={1}", login_name, login_password));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return null;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return null;

                XmlNode node = doc.SelectSingleNode("CSL/Input");
                if (node != null)
                {
                    GPI_INFO gpi = new GPI_INFO();

                    XmlAttributeCollection atts = node.Attributes;
                    string portList = atts.GetNamedItem("port_list").Value;
                    string logicList = atts.GetNamedItem("input_logic_list").Value;

                    int pos = portList.IndexOf('1');
                    gpi.GPI1 = (logicList[pos] == '1') ? true : false;

                    pos = portList.IndexOf('2');
                    gpi.GPI2 = (logicList[pos] == '1') ? true : false;

                    pos = portList.IndexOf('3');
                    gpi.GPI3 = (logicList[pos] == '1') ? true : false;

                    pos = portList.IndexOf('4');
                    gpi.GPI4 = (logicList[pos] == '1') ? true : false;

                    return gpi;
                }

                parseErrorCode(ref doc);
                return null;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return null;
            }
        }

        public bool directIO_output(int io, int data)
        {
            string cmd = "directIOOutput";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}", httpUri.AbsoluteUri, cmd));
                sbReq.Append(String.Format("&mode=run&port={0}&oper_logic={1}", io, data));
                sbReq.Append(String.Format("&username={0}&password={1}", login_name, login_password));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool runIO_output8bit(byte gpo)
        {
            string cmd = "runIO_output8bits";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}&logic={3:X2}", httpUri.AbsoluteUri, cmd, SessionId, gpo));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool directIO_output8bits(byte gpo)
        {
            string cmd = "directIOOutput8bits";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}&logic={3:X2}", httpUri.AbsoluteUri, cmd, SessionId, gpo));
                sbReq.Append(String.Format("&username={0}&password={1}", login_name, login_password));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    string value = node.InnerXml;
                    if (value.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public int LogLevel(string str)
        {
            if (str == "Info") return LOG_LEVEL.Info;
            if (str == "Verbose") return LOG_LEVEL.Verbose;
            return LOG_LEVEL.Disabled;
        }

        public System.Collections.ArrayList syncModifyAccessPassword(WRITETAG_INFO wti)
        {
            string cmd = "writeTag";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            System.Collections.ArrayList list = new System.Collections.ArrayList();

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&mode=syncModifyAccessPassword&password={0}&unlockPassword={1}&accessLock={2}", wti.password, wti.unlockPassword, wti.accessLock));
                sbReq.Append(String.Format("&filterOp1={0}&filter1={1}&filterLogic={2}", wti.filter_op_1, wti.filter1, wti.filter_logic));
                sbReq.Append(String.Format("&filterOp2={0}&filter2={1}", wti.filter_op_2, wti.filter2));
                sbReq.Append(String.Format("&writeFixedNo={0}&timeOut=5000", wti.writeFixedNo));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return null;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return null;

                XmlNode node = doc.SelectSingleNode("CSL/EpcValue");
                if (node != null)
                {
                    while (node != null)
                    {
                        WRITETAG_STATUS wts = new WRITETAG_STATUS();

                        XmlAttributeCollection atts = node.Attributes;
                        wts.after = atts.GetNamedItem("after").Value;
                        wts.before = atts.GetNamedItem("before").Value;
                        wts.status = atts.GetNamedItem("status").Value;

                        list.Add(wts);
                        node = node.NextSibling;
                    }
                    ErrorCode = ERR_CODE_NO_ERROR;
                    ErrorMsg = "";

                    return list;
                }

                parseErrorCode(ref doc);
                return null;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return null;
            }
        }

        public System.Collections.ArrayList syncModifyKillPassword(WRITETAG_INFO wti)
        {
            string cmd = "writeTag";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            System.Collections.ArrayList list = new System.Collections.ArrayList();

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&mode=syncModifyKillPassword&killPassword={0}&unlockPassword={1}", wti.killPassword, wti.unlockPassword));
                sbReq.Append(String.Format("&filterOp1={0}&filter1={1}&filterLogic={2}", wti.filter_op_1, wti.filter1, wti.filter_logic));
                sbReq.Append(String.Format("&filterOp2={0}&filter2={1}", wti.filter_op_2, wti.filter2));
                sbReq.Append(String.Format("&writeFixedNo={0}&timeOut=5000", wti.writeFixedNo));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return null;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return null;

                XmlNode node = doc.SelectSingleNode("CSL/EpcValue");
                if (node != null)
                {
                    while (node != null)
                    {
                        WRITETAG_STATUS wts = new WRITETAG_STATUS();

                        XmlAttributeCollection atts = node.Attributes;
                        wts.after = atts.GetNamedItem("after").Value;
                        wts.before = atts.GetNamedItem("before").Value;
                        wts.status = atts.GetNamedItem("status").Value;

                        list.Add(wts);
                        node = node.NextSibling;
                    }
                    ErrorCode = ERR_CODE_NO_ERROR;
                    ErrorMsg = "";

                    return list;
                }

                parseErrorCode(ref doc);
                return null;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return null;
            }
        }

        public System.Collections.ArrayList syncKillTag(WRITETAG_INFO wti)
        {
            string cmd = "writeTag";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            System.Collections.ArrayList list = new System.Collections.ArrayList();

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&mode=syncKillTag&killPassword={0}", wti.killPassword));
                sbReq.Append(String.Format("&filterOp1={0}&filter1={1}&filterLogic={2}", wti.filter_op_1, wti.filter1, wti.filter_logic));
                sbReq.Append(String.Format("&filterOp2={0}&filter2={1}", wti.filter_op_2, wti.filter2));
                sbReq.Append(String.Format("&killFixedNo={0}&timeOut={1}", wti.killFixedNo, wti.timeOut));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return null;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return null;

                XmlNode node = doc.SelectSingleNode("CSL/EpcValue");
                if (node != null)
                {
                    while (node != null)
                    {
                        WRITETAG_STATUS wts = new WRITETAG_STATUS();

                        XmlAttributeCollection atts = node.Attributes;
                        wts.after = atts.GetNamedItem("after").Value;
                        wts.before = atts.GetNamedItem("before").Value;
                        wts.status = atts.GetNamedItem("status").Value;

                        list.Add(wts);
                        node = node.NextSibling;
                    }
                    ErrorCode = ERR_CODE_NO_ERROR;
                    ErrorMsg = "";

                    return list;
                }

                parseErrorCode(ref doc);
                return null;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return null;
            }
        }

        public System.Collections.ArrayList syncWriteTags(WRITETAG_INFO wti)
        {
            string cmd = "writeTag";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            System.Collections.ArrayList list = new System.Collections.ArrayList();

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&filterOp1={0}&filter1={1}&filterLogic={2}", wti.filter_op_1, wti.filter1, wti.filter_logic));
                sbReq.Append(String.Format("&filterOp2={0}&filter2={1}", wti.filter_op_2, wti.filter2));
                sbReq.Append(String.Format("&writeFixedNo={0}&autoInc={1}", wti.writeFixedNo, wti.auto_inc));
                sbReq.Append(String.Format("&mode={0}&timeOut={1}", wti.mode, wti.timeOut));

                if (wti.passwordEnable.Equals("true"))
                {
                    //Need password
                    sbReq.Append(String.Format("&unlockPassword={0}&passwordEnable=true", wti.unlockPassword));
                }

                if (wti.bank1_enable.Equals("true") == true)
                {
                    sbReq.Append(String.Format("&epc={0}&bank1_enable=true", wti.epc));
                    if (wti.passwordEnable.Equals("true"))
                    {
                        sbReq.Append(String.Format("bank1Lock={0}&bank1Unlock={1}", wti.bank1Lock, wti.bank1Unlock));
                    }
                }
                else if (wti.bank3_enable.Equals("true") == true)
                {
                    sbReq.Append(String.Format("&bank3_hex={0}&bank3_enable=true&tag_model={1}", wti.user_memory, wti.tag_model));
                    if (wti.passwordEnable.Equals("true"))
                    {
                        sbReq.Append(String.Format("bank3Lock={0}&bank3Unlock={1}", wti.bank3Lock, wti.bank3Unlock));
                    }
                    //work around for a bug that must have EPC field.
                    sbReq.Append(String.Format("&epc=000000000000000000000001"));
                }

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return null;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return null;

                XmlNode node = doc.SelectSingleNode("CSL/EpcValue");
                if (node != null)
                {
                    while (node != null)
                    {
                        WRITETAG_STATUS wts = new WRITETAG_STATUS();

                        XmlAttributeCollection atts = node.Attributes;
                        wts.after = atts.GetNamedItem("after").Value;
                        wts.before = atts.GetNamedItem("before").Value;
                        wts.status = atts.GetNamedItem("status").Value;

                        list.Add(wts);
                        node = node.NextSibling;
                    }
                    ErrorCode = ERR_CODE_NO_ERROR;
                    ErrorMsg = "";

                    return list;
                }

                parseErrorCode(ref doc);
                return null;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return null;
            }
        }

        public bool killTags(WRITETAG_INFO wti)
        {
            string cmd = "writeTag";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}&mode=killTagFixedNumber", httpUri.AbsoluteUri, cmd, SessionId));

                sbReq.Append(String.Format("&filterOp1={0}&filter1={1}&filterLogic={2}", wti.filter_op_1, wti.filter1, wti.filter_logic));
                sbReq.Append(String.Format("&filterOp2={0}&filter2={1}&timeOut={2}", wti.filter_op_2, wti.filter2, wti.timeOut));
                sbReq.Append(String.Format("&killFixedNo={0}&killPassword={1}&useActiveProfile={2}", wti.killFixedNo, wti.killPassword, wti.useActiveProfile));

                if (wti.useActiveProfile.Equals("false"))
                {
                    sbReq.Append(String.Format("&antennaPort={0}modulationProfile={1}&mode={2}", wti.antennas, wti.modulation_profile, wti.mode));
                    sbReq.Append(String.Format("&transmitPower={0}&populationEst={1}&sessionNo={2}", wti.transmit_power, wti.population, wti.session_no));
                }

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool killTagsStop()
        {
            string cmd = "startInventory";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                //Stop kill tag using either
                //- command = startInventory, mode=killStop or,
                //- command = writeTag, mode=killStop
                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}&mode=killStop", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool writeTags(WRITETAG_INFO wti)
        {
            string cmd = "writeTag";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));

                sbReq.Append(String.Format("&filterOp1={0}&filter1={1}&filterLogic={2}", wti.filter_op_1, wti.filter1, wti.filter_logic));
                sbReq.Append(String.Format("&filterOp2={0}&filter2={1}", wti.filter_op_2, wti.filter2));
                sbReq.Append(String.Format("&writeFixedNo={0}&useActiveProfile={1}", wti.writeFixedNo, wti.useActiveProfile));

                if (wti.useActiveProfile.Equals("false"))
                {
                    sbReq.Append(String.Format("&antennaPort={0}modulationProfile={1}&mode={2}", wti.antennas, wti.modulation_profile, wti.mode));
                    sbReq.Append(String.Format("&transmitPower={0}&populationEst={1}&sessionNo={2}", wti.transmit_power, wti.population, wti.session_no));
                    sbReq.Append(String.Format("&epc={0}&autoInc={1}", wti.epc, wti.auto_inc));
                }

                if (wti.mode.Equals("writeAllBanksFilterManyHexBased") || wti.mode.Equals("writeAllBanksFilterFixedNumberHexBased"))
                {
                    sbReq.Append(String.Format("&tag_model={0}&bank3_hex={1}&bank1_enable={2}&bank3_enable={3}", wti.tag_model, wti.user_memory, wti.bank1_enable, wti.bank3_enable));
                    sbReq.Append(String.Format("&bank1Lock={0}&bank1Unlock={1}", wti.bank1Lock, wti.bank1Unlock));
                    sbReq.Append(String.Format("&bank3Lock={0}&bank3Unlock={1}", wti.bank3Lock, wti.bank3Unlock));
                }

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public System.Collections.ArrayList writeTagsStatus()
        {
            string cmd = "writeTag";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            System.Collections.ArrayList list = new System.Collections.ArrayList();

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}&mode=verifyEpc", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return null;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                //if (isCSLResponse(ref doc, cmd) == false) return null;
                if (isCSLResponse(ref doc, "verifyEpc") == false) return null;

                XmlNode node = doc.SelectSingleNode("CSL/EpcValue");
                if (node != null)
                {
                    while (node != null)
                    {
                        WRITETAG_STATUS wts = new WRITETAG_STATUS();

                        XmlAttributeCollection atts = node.Attributes;
                        wts.after = atts.GetNamedItem("after").Value;
                        wts.before = atts.GetNamedItem("before").Value;
                        wts.status = atts.GetNamedItem("status").Value;

                        list.Add(wts);
                        node = node.NextSibling;
                    }
                    ErrorCode = ERR_CODE_NO_ERROR;
                    ErrorMsg = "";

                    return list;
                }

                parseErrorCode(ref doc);
                return null;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return null;
            }
        }

        public bool writeTagsStop()
        {
            string cmd = "writeTag";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}&mode=stop", httpUri.AbsoluteUri, cmd, SessionId));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

        public bool setDate(DateTime utc)
        {
            string cmd = "setDateTime";
            ErrorCode = ERR_CODE_UNKNOWN_RESPONSE;
            ErrorMsg = ERR_MSG_UNKNOWN_RESPONSE;    //default error message

            try
            {
                StringBuilder sbReq = new StringBuilder();

                sbReq.Append(String.Format("{0}API?command={1}&session_id={2}", httpUri.AbsoluteUri, cmd, SessionId));
                sbReq.Append(String.Format("&Year={0}&Month={1}&Day={2}", utc.Year, utc.Month, utc.Day));
                sbReq.Append(String.Format("&Hour={0}&Minute={1}&Second={2}", utc.Hour, utc.Minute, utc.Second));

                string resp = sendHTTPRequest(sbReq.ToString());
                if (resp == null) return false;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                if (isCSLResponse(ref doc, cmd) == false) return false;

                XmlNode node = doc.SelectSingleNode("CSL/Ack");
                if (node != null)
                {
                    if (node.InnerXml.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        ErrorCode = ERR_CODE_NO_ERROR;
                        ErrorMsg = "";
                        return true;
                    }
                }
                parseErrorCode(ref doc);
                return false;
            }
            catch
            {
                ErrorCode = ERR_CODE_UNKNOWN_ERROR;
                ErrorMsg = ERR_MSG_UNKNOWN_ERROR;
                return false;
            }
        }

    }

    #region BatchEndEventArgs
    public delegate void BatchEndEventHandler(object sender, BatchEndEventArgs e);

    public class BatchEndEventArgs : System.EventArgs
    {
        private string timestamp;
        public string TimeStamp
        {
            get { return timestamp; }
        }

        public BatchEndEventArgs(string time)
        {
            timestamp = time;
        }
    }
    #endregion

    #region DataEndEventArgs
    public delegate void DataEndEventHandler(object sender, DataEndEventArgs e);

    public class DataEndEventArgs : System.EventArgs
    {
        private string timestamp;
        public string TimeStamp
        {
            get { return timestamp; }
        }

        public DataEndEventArgs(string time)
        {
            timestamp = time;
        }
    }
    #endregion

    #region AntennaEventArgs
    public delegate void AntennaEventHandler(object sender, AntennaEventArgs e);

    public class AntennaEventArgs : System.EventArgs
    {
        private string antPorts;
        public string Ports
        {
            get { return antPorts; }
        }

        private string timestamp;
        public string TimeStamp
        {
            get { return timestamp; }
        }

        public AntennaEventArgs(string antPort, string time)
        {
            antPorts = antPort;
            timestamp = time;
        }
    }
    #endregion

    #region BufferOverflowEventArgs
    public delegate void BufferOverflowEventHandler(object sender, BufferOverflowEventArgs e);

    public class BufferOverflowEventArgs : System.EventArgs
    {
        private string timestamp;
        public string TimeStamp
        {
            get { return timestamp; }
        }

        public BufferOverflowEventArgs(string time)
        {
            timestamp = time;
        }
    }
    #endregion

    #region TagReceiveEventArgs
    public delegate void TagReceiveEventHandler(object sender, TagReceiveEventArgs e);

    public class TagReceiveEventArgs : System.EventArgs
    {
        private object newTags;
        public object rxTag
        {
            get { return newTags; }
        }

        public TagReceiveEventArgs(object intag)
        {
            newTags = intag;
        }
    }
    #endregion

    #region TagListEventArgs
    public delegate void TagListEventHandler(object sender, TagListEventArgs e);

    public class TagListEventArgs : System.EventArgs
    {
        private System.Collections.ArrayList tagsList = new System.Collections.ArrayList();
        public System.Collections.ArrayList TagsList
        {
            get { return tagsList; }
        }

        public TagListEventArgs(System.Collections.ArrayList list)
        {
            tagsList = list;
        }
    }
    #endregion

    #region InventoryEventArgs
    public delegate void InventoryEventHandler(object sender, InventoryEventArgs e);

    public class InventoryEventArgs : System.EventArgs
    {
        object info;

        public object InventoryNtf
        {
            get { return info; }
        }

        public InventoryEventArgs(INVENTORY_NTF_INFO o)
        {
            info = (object)o;
        }
    }
    #endregion

    #region GpiEventArgs
    public delegate void GpiEventHandler(object sender, GpiEventArgs e);

    public class GpiEventArgs : System.EventArgs
    {
        object info;

        public object GpiNtf
        {
            get { return info; }
        }

        public GpiEventArgs(GPI_NTF_INFO o)
        {
            info = (object)o;
        }
    }
    #endregion

    #region Trusted Server
    // State object for reading client data asynchronously
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
        // Stores received tags for batch alert mode
        public System.Collections.ArrayList list = new System.Collections.ArrayList();
    }

    public class TrustedServer
    {
        private int ApiLogLevel = LOG_LEVEL.Disabled;
        private bool Listening = false;
        private int port = 9090;
        private Socket listenerSocket = null;
        private Thread serverThread = null;
        private System.Collections.ArrayList clientSockets = new System.Collections.ArrayList();

        // Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public int tcp_port
        {
            get { return (port); }
            set { port = value; }
        }

        public int api_log_level
        {
            get { return (ApiLogLevel); }
            set { ApiLogLevel = value; }
        }

        public bool isListening
        {
            get { return (Listening); }
        }

        public TrustedServer()
        {
        }

        public TrustedServer(int n)
        {
            port = n;
        }

        public void Start()
        {
            if (Listening == true) return;
            Listening = true;

            serverThread = new Thread(new ThreadStart(StartListening));
            serverThread.IsBackground = true;
            serverThread.Start();
        }

        private void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(new IPEndPoint(IPAddress.Any, port));
                listener.Listen(100);
                listenerSocket = listener;

                while (Listening == true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    saveToLogInfo("Wait for client...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }
            }
            catch
            {
                //
                Listening = false;
            }
        }

        public void Stop()
        {
            if (Listening == false) return;

            Listening = false;

            listenerSocket.Close();
            listenerSocket = null;

            Monitor.Enter(clientSockets);
            foreach (Socket s in clientSockets)
            {
                s.Close();
                saveToLogInfo(String.Format("stop client connection. {0} active client.", clientSockets.Count));
            }
            clientSockets.Clear();
            Monitor.Exit(clientSockets);

            serverThread.Abort();
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket client = (Socket)ar.AsyncState;
            try
            {
                Socket handler = client.EndAccept(ar);

                if (Listening == true)
                {

                    // Create the state object.
                    StateObject state = new StateObject();
                    state.workSocket = handler;
                    Monitor.Enter(clientSockets);
                    clientSockets.Add(handler);
                    Monitor.Exit(clientSockets);

                    IPEndPoint ip = (IPEndPoint)handler.RemoteEndPoint;
                    saveToLogInfo(String.Format("Connected from {0}:{1}. {2} active clients", ip.Address.ToString(), ip.Port.ToString(), clientSockets.Count));

                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                }
                else
                {
                    if (client != null)
                        client.Close();
                    saveToLogInfo("End Connection");
                }
            }
            catch
            {
                if (client != null)
                    client.Close();
                saveToLogInfo("Accept Connection Error");
            }
        }

        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            try
            {
                // Read data from the client socket. 
                int bytesRead = handler.EndReceive(ar);
                if (Listening == true)
                {
                    if (bytesRead > 0)
                    {
                        // There  might be more data, so store the data received so far.
                        state.sb.Append(Encoding.ASCII.GetString(
                            state.buffer, 0, bytesRead));

                        // Check for end-of-file tag. If it is not there, read 
                        // more data.
                        content = state.sb.ToString();
                        saveToLogVerbose(content);
                        state.sb = new StringBuilder();
                        state.sb.Append(parseNtf(content, state));

                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback(ReadCallback), state);
                    }
                    else
                    {
                        if (state.workSocket != null)
                        {
                            state.workSocket.Shutdown(SocketShutdown.Both);
                            state.workSocket.Close();
                            Monitor.Enter(clientSockets);
                            clientSockets.Remove(state.workSocket);
                            Monitor.Exit(clientSockets);
                        }
                        state.workSocket = null;
                        saveToLogInfo(String.Format("No data read, remote disconnect?, {0} active clients", clientSockets.Count));
                    }
                }
                else
                {
                    if (state.workSocket != null)
                    {
                        state.workSocket.Shutdown(SocketShutdown.Both);
                        state.workSocket.Close();
                        Monitor.Enter(clientSockets);
                        clientSockets.Remove(state.workSocket);
                        Monitor.Exit(clientSockets);
                    }
                    state.workSocket = null;
                    saveToLogInfo(String.Format("Disconnect client. {0} active clients", clientSockets.Count));
                }
            }
            catch
            {
                if (state.workSocket != null)
                {
                    state.workSocket.Close();
                    Monitor.Enter(clientSockets);
                    clientSockets.Remove(state.workSocket);
                    Monitor.Exit(clientSockets);
                }
                state.workSocket = null;
                saveToLogInfo(String.Format("Receive data Error. {0} active clients", clientSockets.Count));
            }
        }

        public string parseNtf(string str, StateObject state)
        {
            int i = 0;
            bool completeStr = str.EndsWith("\n");
            string[] records = str.Split(new char[] { '\n' });

            for (i = 0; i < records.Length - 1; i++)
            {
                parseRecords(records[i], state);
            }

            if (completeStr == true)
            {
                parseRecords(records[i], state);
                return "";
            }
            else
            {
                return records[i];
            }
        }

        public void parseRecords(string str, StateObject state)
        {
            #region evtNtf
            if (str.StartsWith("cmd=evtNtf"))
            {
                //cmd=evtNtf&batchEnd=yes
                if (str.Contains("batchEnd=yes"))
                {
                    saveToLogVerbose("BatchEnd Found");
                    OnTagListEvent(new TagListEventArgs(state.list));
                    state.list.Clear();
                    OnBatchEnd(new BatchEndEventArgs(DateTime.Now.ToString()));
                    return;
                }

                //cmd=evtNtf&msg=GPI&port=1&state=1&time=1073250762
                if (str.Contains("msg=GPI"))
                {
                    GPI_NTF_INFO info = new GPI_NTF_INFO();
                    string[] Invkeys = str.Split('&');
                    foreach (string s in Invkeys)
                    {
                        if (s.StartsWith("port=", StringComparison.OrdinalIgnoreCase))
                        {
                            info.port = s.Substring(5);
                        }
                        else if (s.StartsWith("state=", StringComparison.OrdinalIgnoreCase))
                        {
                            info.state = s.Substring(6);
                        }
                        else if (s.StartsWith("time=", StringComparison.OrdinalIgnoreCase))
                        {
                            info.time = s.Substring(5);
                        }
                    }
                    saveToLogVerbose(String.Format("GPI Port{0}={1}", info.port, info.state));
                    OnGpi(new GpiEventArgs(info));
                    return;
                }

                //cmd=evtNtf&evt_id=TagfoundAction&msg=invenDisabled&trig=irNotfound&time=1175329956
                if (str.Contains("msg="))
                {
                    INVENTORY_NTF_INFO info = new INVENTORY_NTF_INFO();
                    string[] Invkeys = str.Split('&');
                    foreach (string s in Invkeys)
                    {
                        if (s.StartsWith("evt_id=", StringComparison.OrdinalIgnoreCase))
                        {
                            info.id = s.Substring(7);
                        }
                        else if (s.StartsWith("msg=", StringComparison.OrdinalIgnoreCase))
                        {
                            info.msg = s.Substring(4);
                        }
                        else if (s.StartsWith("trig=", StringComparison.OrdinalIgnoreCase))
                        {
                            info.trigger = s.Substring(5);
                        }
                        else if (s.StartsWith("time=", StringComparison.OrdinalIgnoreCase))
                        {
                            info.time = s.Substring(5);
                        }
                    }
                    saveToLogVerbose(String.Format("{0} {1} Found", info.id, info.msg));
                    OnInventory(new InventoryEventArgs(info));
                    return;
                }

                //cmd=evtNtf&evt_id=DemoEvent&src_ip=192.168.26.211&ant=Antenna1&cp_id=Capture Point 1
                //&idx=B85&tag_id=14040000000000000000005D&rssi=-45&time=1219912627&cnt=1&bank1=
                //&freq=1265&PC=3000&usec=180016
                TAG intag = new TAG();
                string[] keys = str.Split('&');
                foreach (string s in keys)
                {
                    //System.Diagnostics.Debug.Print(s);

                    if (s.StartsWith("evt_id=", StringComparison.OrdinalIgnoreCase))
                    {
                        intag.EventId = s.Substring(7);
                    }
                    else if (s.StartsWith("src_ip=", StringComparison.OrdinalIgnoreCase))
                    {
                        intag.ServerIp = s.Substring(7);
                    }
                    else if (s.StartsWith("ant=", StringComparison.OrdinalIgnoreCase))
                    {
                        intag.Antenna = s.Substring(4);
                    }
                    else if (s.StartsWith("cp_id=", StringComparison.OrdinalIgnoreCase))
                    {
                        intag.CapturePointId = s.Substring(6);
                    }
                    else if (s.StartsWith("idx=", StringComparison.OrdinalIgnoreCase))
                    {
                        if (s.Length > 5)
                        {
                            intag.Index = s.Substring(5);
                            intag.session = s.Substring(4, 1);
                        }
                    }
                    else if (s.StartsWith("freq=", StringComparison.OrdinalIgnoreCase))
                    {
                        UInt16 f;
                        try
                        {
                            // CS461
                            //UInt16.TryParse(s.Substring(5), out f);
                            //intag.Frequency = 860 + (f & 0x7FF) * 0.05 + (f >> 11) * 0.005;
                            double.TryParse(s.Substring(5), out intag.Frequency);
                        }
                        catch
                        {
                        }
                    }
                    else if (s.StartsWith("tag_id=", StringComparison.OrdinalIgnoreCase))
                    {
                        intag.TagOrigId = s.Substring(7);
                    }
                    else if (s.StartsWith("PC=", StringComparison.OrdinalIgnoreCase))
                    {
                        intag.PC = s.Substring(3, 4);
                    }
                    else if (s.StartsWith("rssi=", StringComparison.OrdinalIgnoreCase))
                    {
                        double.TryParse(s.Substring(5), out intag.RSSI);
                    }
                    else if (s.StartsWith("time=", StringComparison.OrdinalIgnoreCase))
                    {
                        string time = s.Substring(5);
                        try
                        {
                            Int32.TryParse(time, out intag.Time);
                        }
                        catch
                        {
                        }
                    }
                    else if (s.StartsWith("usec=", StringComparison.OrdinalIgnoreCase))
                    {
                        string usec = s.Substring(5);
                        try
                        {
                            Int32.TryParse(usec, out intag.USec);
                        }
                        catch
                        {
                        }
                    }
                    else if (s.StartsWith("cnt=", StringComparison.OrdinalIgnoreCase))
                    {
                        Int32.TryParse(s.Substring(4), out intag.count);
                    }
                    else if (s.StartsWith("bank0=", StringComparison.OrdinalIgnoreCase))
                    {
                        intag.Bank0 = s.Substring(6);
                    }
                    else if (s.StartsWith("bank2=", StringComparison.OrdinalIgnoreCase))
                    {
                        intag.Bank2 = s.Substring(6);
                    }
                    else if (s.StartsWith("bank3=", StringComparison.OrdinalIgnoreCase))
                    {
                        intag.Bank3 = s.Substring(6);
                    }
                    else if (s.StartsWith("pc=", StringComparison.OrdinalIgnoreCase))
                    {
                        intag.PC = s.Substring(3);
                    }

                }
                //intag.ApiTimeStampUTC = DateTime.UtcNow;
                intag.ApiTimeStampUTC = CS463_HL_API.ConvertFromUnixTimestamp(intag.Time);
                OnTagReceiveEvent(new TagReceiveEventArgs(intag));
                if (state.list.Count <= 10000)
                    state.list.Add(intag);      //add new tag to list if number of tag in list is not more than 10000
            }
            #endregion
            #region blog
            else if (str.StartsWith("cmd=blog"))
            {
                //cmd=blogNtf&dataEnd=yes
                if (str.Contains("dataEnd=yes"))
                {
                    saveToLogVerbose("DataEnd Found");
                    OnDataEnd(new DataEndEventArgs(DateTime.Now.ToString()));
                    return;
                }
                //cmd=blogNtf&evt_id=DemoEvent&src_ip=192.168.25.249&ant=Antenna1&cp_id=Capture Point 1&idx=C31&tag_id=000000000000000000000006&rssi=-49&time=1173076720
            }
            #endregion
            #region errNtf
            else if (str.StartsWith("cmd=errNtf"))
            {
                //cmd=errNtf&status=buffer overflow&time=1173076720
                string status = null;
                string time = null;

                string[] keys = str.Split('&');
                foreach (string s in keys)
                {
                    if (s.StartsWith("status=", StringComparison.OrdinalIgnoreCase))
                    {
                        if (s.Length > 7)
                            status = s.Substring(7);
                    }
                    else if (s.StartsWith("time=", StringComparison.OrdinalIgnoreCase))
                    {
                        if (s.Length > 5)
                            time = s.Substring(5);
                    }
                }
                if (status.Equals("buffer overflow", StringComparison.OrdinalIgnoreCase))
                {
                    saveToLogVerbose("BufferOverflow Found");
                    OnBufferOverflowEvent(new BufferOverflowEventArgs(time));
                    return;
                }
            }
            #endregion
            #region antNtf
            else if (str.StartsWith("cmd=antNtf"))
            {
                //cmd=antNtf&mismatch=1234&time=1173076720
                string port = null;
                string time = null;

                string[] keys = str.Split('&');
                foreach (string s in keys)
                {
                    if (s.StartsWith("mismatch=", StringComparison.OrdinalIgnoreCase))
                    {
                        if (s.Length > 9)
                            port = s.Substring(9);
                    }
                    else if (s.StartsWith("time=", StringComparison.OrdinalIgnoreCase))
                    {
                        if (s.Length > 5)
                            time = s.Substring(5);
                    }
                }
                saveToLogVerbose("AntennaNtf Found");
                OnAntennaEvent(new AntennaEventArgs(port, time));
            }
            #endregion
        }

        public void saveToLogVerbose(string msg)
        {
            if (ApiLogLevel >= LOG_LEVEL.Verbose)
            {
                saveToLog(msg);
            }
        }

        public void saveToLogInfo(string msg)
        {
            if (ApiLogLevel >= LOG_LEVEL.Info)
            {
                saveToLog(msg);
            }
        }

        private void saveToLog(string msg)
        {
            lock (this)
            {
                StreamWriter sw = new StreamWriter(System.Environment.CurrentDirectory + "\\tcp.log", true);
                sw.WriteLine(String.Format("{0}|{1}\n", DateTime.Now.ToLocalTime(), msg));
                sw.Close();
            }
        }

        #region events
        public event BatchEndEventHandler BatchEnd;

        protected virtual void OnBatchEnd(BatchEndEventArgs e)
        {
            if (BatchEnd != null) BatchEnd(this, e);
        }

        public event DataEndEventHandler DataEnd;

        protected virtual void OnDataEnd(DataEndEventArgs e)
        {
            if (DataEnd != null) DataEnd(this, e);
        }

        public event AntennaEventHandler AntennaEvent;

        protected virtual void OnAntennaEvent(AntennaEventArgs e)
        {
            if (AntennaEvent != null) AntennaEvent(this, e);
        }

        public event BufferOverflowEventHandler BufferOverflowEvent;

        protected virtual void OnBufferOverflowEvent(BufferOverflowEventArgs e)
        {
            if (BufferOverflowEvent != null) BufferOverflowEvent(this, e);
        }

        public event TagReceiveEventHandler TagReceiveEvent;

        protected virtual void OnTagReceiveEvent(TagReceiveEventArgs e)
        {
            if (TagReceiveEvent != null) TagReceiveEvent(this, e);
        }

        public event TagListEventHandler TagListEvent;

        protected virtual void OnTagListEvent(TagListEventArgs e)
        {
            if (TagListEvent != null) TagListEvent(this, e);
        }

        public event InventoryEventHandler InventoryEvent;

        protected virtual void OnInventory(InventoryEventArgs e)
        {
            if (InventoryEvent != null) InventoryEvent(this, e);
        }

        public event GpiEventHandler GpiEvent;

        protected virtual void OnGpi(GpiEventArgs e)
        {
            if (GpiEvent != null) GpiEvent(this, e);
        }
        #endregion
    }
    #endregion

    #region datatypes
    public struct TAG
    {
        public string CapturePointId;
        public double Frequency;
        public string Index;
        public double RSSI;
        public string TagOrigId;    //EPC
        public Int32 Time;          //Timestamp (reader time) at 1st seen; hours from 1970 0:0:0
        public Int32 USec;          //Timestamp (reader time) at 1st seen; micro-second 
        public string ServerIp;
        public string EventId;
        public DateTime ApiTimeStampUTC;    //Timestamp (Server) receive this notification
        public string session;
        public string Antenna;
        public Int32 count;
        public string Bank0;
        public string Bank2;
        public string Bank3;
        public string PC;           //PC bits
    }

    //public struct TAG_MULTI_BANKS
    //{
    //    public string CapturePointId;
    //    public double Frequency;
    //    public string Index;
    //    public double RSSI;
    //    public string EPC;
    //    public string TID;
    //    public string UserMemory;
    //    public Int32 Time;
    //    public string ServerIp;
    //    public string EventId;
    //    public DateTime ApiTimeStampUTC;
    //    public string session;
    //    public string PC;
    //}

    public struct READER_ID
    {
        public string id;
        public string desc;
    }

    public class OPERATION_PROFILE
    {
        public string profile_id;
        public string modulation_profile;
        public Int32 population;
        public Int32 session_no;
        public string capture_mode;
        public Int32 window_time;
        public string ant1_name;
        public string ant2_name;
        public string ant3_name;
        public string ant4_name;
        public bool ant1_enable;
        public bool ant2_enable;
        public bool ant3_enable;
        public bool ant4_enable;
        public bool profile_enable;
        public string trigger;
        public string ant1_power;
        public string ant2_power;
        public string ant3_power;
        public string ant4_power;
        public string transmit_power;
        public Int32 estTagTime;
        public bool automaticSearch;
        public string fixedFrequency;
        public bool LBT;
        public string tagModel;
        public string memoryBank;           // Bank0, Bank2, Bank3
        public string antennaPortScheme;    //true - Separated, false - Combined
        public Int32 RxSensitivity = 0;
        public string RxSensitivityValue1;
        public string RxSensitivityValue2;
        public string RxSensitivityValue3;
        public string RxSensitivityValue4;

        public enum RxSensitivityOption
        {
            MaximumSensitivity = 0,
            VariableSensitivity
        }

        public enum TRIGGER_METHOD
        {
            Autonomous_Time_Trigger = 0,
            Polling_Trigger_By_Client,
        };

        public readonly static string[] TRIGGER_METHOD_STRING = new string[] { 
                        "Autonomous Time Trigger",
                        "Polling Trigger By Client"
        };

        public enum MODULATION_PROFILE
        {
            Profile0_High_Speed = 0, Profile1_Hybrid, Profile2_Dense_Reader_M4, Profile3_Dense_Reader_M8
        }

        public readonly static string[] MODULATION_PROFILE_STRING = new string[] {
            "Profile0", "Profile1", "Profile2", "Profile3"
        };
    }

    public struct SERVER_INFO
    {
        public string id;
        public string desc;
        public string ip;
        public string reader_port;
        public string server_port;
        public string mode;
        public bool enable;
    }

    public struct TAG_GROUP_INFO
    {
        public string id;
        public string desc;
        public string property;
    }

    public struct TAG_FILTER_INFO
    {
        public string id;
        public string mode;
        public string param;
    }

    public struct TRIGGER_INFO
    {
        public string id;
        public string desc;
        public string mode;
        public string capture_point;
        public string logic;
        public string PC;
    }

    public class RESULTANT_ACTION_INFO
    {
        //common
        public string action_id = "DemoAction";
        public string desc = "Demo Action";
        public string action_mode = "Do Nothing (Only Show on Screen)";

        //Output Port
        public string pulse_logic = string.Empty;
        public string pulse_mode = string.Empty;
        public UInt32 duration = 0;
        public UInt32 dutycycle = 0;
        public UInt32 post_action_delay = 0;
        public UInt32 pre_action_wait = 0;
        public UInt32 pulse_width = 0;

        //Trusted Server
        public bool batchEndNtf = true;
        public bool console = false;
        public bool duplicateEliminationFlag = true;
        public bool serialPort1 = false;
        public string report_id = "Default Report";
        public string server_id = "Demo Server";
        public string transport = "tcp";
        public UInt32 close_socket = 0;
        public string tagGroup = string.Empty;

        //IATA
        public string backgroundTagFiltering = "false";
        public string noOfPreviousTag = "10";
        public string previousTagFiltering = "false";
        public string userDefinedTagListFilter = "false";
        public string withoutDateFieldFiltering = "false";
    }

    public class EVENT_INFO
    {
        public bool enable = false;
        public bool event_log = false;
        public string desc = string.Empty;
        public string event_id = "Demo Event";
        public string exclusivity = string.Empty;                   //"Non-exclusive", "Exclusive"
        public string inventoryDisablingAction = string.Empty;      //"NONE", Output Port Action
        public string inventoryDisablingTrigger = string.Empty;     //"Never Stop", Input Port Trigger, Timeout Trigger
        public string inventoryEnablingAction = string.Empty;       //"NONE", Output Port Action
        public string inventoryEnablingTrigger = string.Empty;      //"Always On", Input Port Trigger
        public string operProfile_id = "Default Profile";           //Must be "Default Profile"
        public string resultant_action = "Demo Action";             //ID of actions. A1 THEN A2
        public string triggering_logic = "Demo Trigger";            //ID of trigger
    }

    public class API_MODE
    {
        public const bool High = true;
        public const bool Low = false;
    }

    public class IO_PORT
    {
        public const int Port1 = 1;
        public const int Port2 = 2;
        public const int Port3 = 3;
        public const int Port4 = 4;
    }

    public class IO_LOGIC
    {
        public const int High = 1;
        public const int Low = 0;
    }

    public class LOG_LEVEL
    {
        public const int Disabled = 0;
        public const int Info = 10;
        public const int Verbose = 99;
    }

    public class READER_STATUS
    {
        public string Model;
        public string Protocol_Supported;
        public string Description;
        public string ID;
        public string DSP_Version;
        public string EdgeServer_Version;
        public string FPGA_Version;
        public string LibMapi_Version;
        public string Middleware_Version;
        public string ModemController_Version;
        public string Firmware_Version;
        public string TimeZone;
        public string DaylightSaving;
        public string Session_Timeout;
        public string Client_IP;
        public string Host_IP;
        public string User_Level;
        public string Login_Status;
        public string Session_Id;
        public string Username;
        public string Access_Mode;
        public string Access_Mode_Description;
        public string Antenna_Ports;
        public string Capture_Mode;
        public string Profile_ID;
        public string Trigger_Methed;
        public System.Collections.ArrayList Active_Event;
        public DateTime Local_Time;
        public DateTime UTC_Time;
    }

    public class HEALT_INFO
    {
        public string Check_Time;
        public string EdgeServer;
        public string Free_RAM;
        public string ModemController;
        public string RFID_App;
        public string Up_Time;
    }

    public class WRITETAG_INFO
    {
        public string epc = "";                  //Hex. digit, length = 24. e.g "UUUUBBBBCCCCDDDDEEEEFFFF" where "U" is no change
        public string user_memory = "";          //Hex. digit, length = depends on tag IC. e.g. "AAAABBBB"
        public string mode = "";                 //"writeEpcFilterOnce", "writeEpcFilterMany", "writeAllBanksFilterManyHexBased"
        public string modulation_profile = "";   //String. "Profile0", "Profile1", "Profile2" or "Profile3"
        public Int32 population = 1;             //Decimal digit, range: 1 to 65535
        public Int32 session_no = 1;             //Decimal digit, range: 1 to 3
        public string transmit_power = "30.00";  //Decimal digit, range: 15.00 to 30.00 with step size 0.25.
        public string antennas = "";             //"1", "2", "3", "4" combination e.g. "13" will use antenna 1 & 3
        public string filter_op_1 = "";          //"EQUAL", "NOT EQUAL" or "NONE"
        public string filter_op_2 = "";          //"EQUAL", "NOT EQUAL"
        public string filter1 = "";              //Hex. digit, length = 24. e.g. "AAAAXXXXBBBBXXXXCCCCXXXX" where "X" is don't care
        public string filter2 = "";              //Hex. digit, length = 24. e.g. "AAAAXXXXBBBBXXXXCCCCXXXX" where "X" is don't care
        public string filter_logic = "NONE";     //Logic operation between filter 1 and 2. "AND" or "NONE"
        public string auto_inc = "false";        //True, False
        public string tag_model = "";            //"NXP", "Monza", "Monaco", "Monza ID"
        public string bank1_enable = "";         //True, False
        public string bank3_enable = "";         //True, False
        public Int32 writeFixedNo = 1;           //No. of tag to be written
        public Int32 killFixedNo = 1;            //No. of tag to be killed
        public string bank1Lock = "false";       //Lock memory bank 1
        public string bank1Unlock = "false";     //Unlock memory bank 1
        public string bank3Lock = "false";       //Lock memory bank 3
        public string bank3Unlock = "false";     //Unlock memory bank 3
        public string password = "";             //New Access/Kill password
        public string unlockPassword = "";       //Access password
        public string accessLock = "";           //Lock memory bank 0
        public string killPassword = "";         //Kill password
        public string timeOut = "5000";          //Timeout period in msec for synchronous write operations
        public string passwordEnable = "false";  //Using password
        public string useActiveProfile = "false"; //Use current operation profile

    }

    public class WRITETAG_STATUS
    {
        public string before;
        public string after;
        public string status;
    }

    public class USER_INFO
    {
        public string username = "";
        public string password = "";
        public string desc = "";
        public int level = 0;

        public readonly static string[] description = new string[] { 
                        "",
                        "Business process user - Report generation only",
                        "Business process user - Configuration & maintenance",
                        "System level user - Read only access of system configuration",
                        "System level user - Configuration &  maintenance",
                        "",
                        "",
                        "",
                        "",
                        "Root user"
                };

        public enum LEVELS
        {
            Business_process_user_Report_generation_only = 1,
            Business_process_user_Configuration_and_maintenance = 2,
            System_level_user_Read_only_access_of_system_configuration = 3,
            System_level_user_Configuration_and_maintenance = 4,
            Root_user = 9
        }

        #region properities
        public string level_description
        {
            get { return description[level]; }
        }
        #endregion

        public USER_INFO()
        {
        }

        public USER_INFO(string name, string pass, string des, LEVELS lvl)
        {
            username = name;
            password = pass;
            desc = des;
            level = (int)lvl;
        }
    }

    public class INVENTORY_NTF_INFO
    {
        public string id = "";
        public string msg = "";
        public string trigger = "";
        public string time = "";
    }

    public class GPI_NTF_INFO
    {
        public string port = "";
        public string state = "";
        public string time = "";
    }

    public class GPI_INFO
    {
        public bool GPI1 = false;
        public bool GPI2 = false;
        public bool GPI3 = false;
        public bool GPI4 = false;
    }

    public class EPC
    {
        ulong lower = 0;
        uint upper = 0;
        bool isBCD = false;
        Regex rxBCD = new Regex(@"^\d{24}$");   //BCD must be 24 decimal digits

        #region Properties
        public string Text
        {
            set
            {
                string sEpc = value;
                isBCD = rxBCD.IsMatch(value);
                upper = uint.Parse(sEpc.Substring(0, 8), System.Globalization.NumberStyles.HexNumber);
                lower = ulong.Parse(sEpc.Substring(8, 16), System.Globalization.NumberStyles.HexNumber);
            }

            get
            {
                return ToHexString();
            }
        }
        #endregion

        public string Inc()
        {
            lower++;
            if (lower == 0)
                upper++;

            return ToHexString();
        }

        public string Dec()
        {
            if (lower == 0)
                upper--;
            lower--;

            return ToHexString();
        }

        public string BCD_Inc()
        {
            byte[] BCD = ToByteArray();
            uint i = 0;
            bool inc = true;

            if (isBCD == false)
            {
                Exception e = new Exception("Not a valid 24 digits BCD number");
                throw e;
            }

            for (i = 0; (i < 24) && inc; i++)
            {
                if (BCD[i] != 9)
                {
                    BCD[i]++;
                    inc = false;
                }
                else
                {
                    BCD[i] = 0;
                }
            }

            return ToHexString(BCD);
        }


        private string ToHexString()
        {
            return String.Format("{0:X8}{1:X16}", upper, lower);
        }

        private byte[] ToByteArray()
        {
            byte[] BCD = new byte[24];
            uint i = 0;
            ulong l = lower;
            uint u = upper;

            for (i = 0; i < 16; i++)
            {
                BCD[i] = (byte)(l & 0x0f);
                l >>= 4;
            }

            for (; i < 24; i++)
            {
                BCD[i] = (byte)(u & 0x0f);
                u >>= 4;
            }

            return BCD;
        }

        private string ToHexString(byte[] b)
        {
            int i = 0;

            upper = b[23];
            for (i = 22; i > 15; i--)
            {
                upper <<= 4;
                upper += b[i];
            }

            lower = b[i--];
            for (; i >= 0; i--)
            {
                lower <<= 4;
                lower += b[i];
            }

            return ToHexString();
        }
    }

    #endregion

    public class ShowWaitCursor : IDisposable
    {
        public ShowWaitCursor()
        {
            Cursor.Current = Cursors.WaitCursor;
        }
        public void Dispose()
        {
            Cursor.Current = Cursors.Default;
        }
    }

}

