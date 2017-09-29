using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Props
{
    public enum ConnectionStatus
    {
        Error = -2,
        Disconnected = -1,
        Connecting,
        Connected
    };

    public class CServer
    {
        public string g_Map;
        private const string apiUrl = "response_server.php";
        public ConnectionStatus connectionStatus = ConnectionStatus.Disconnected;
        public CServer(){}

        public void InitalizeConnection()
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("api_method", "establishConnection");
            nvc.Add("api_data", "Connect");
            string info = HTTP.Post(apiUrl, nvc);

            API_Response _response = JsonConvert.DeserializeObject<API_Response>(info);
            if (_response.IsError)
            {
                connectionStatus = ConnectionStatus.Error;
                Program.form.SetErrorText(_response.ErrorMessage);
            }
            else if (_response.ResponseData != "SUCCESS")
            {
                connectionStatus = ConnectionStatus.Error;
            }
            else if (!_response.IsError && _response.ResponseData == "SUCCESS")
            {
                connectionStatus = ConnectionStatus.Connected;
            }
        }

        public void DeleteProp(Prop p)
        {
            NameValueCollection nvc = new NameValueCollection();

            PropReq preq = new PropReq();
            preq.Model = p.Model;
            preq.Map = g_Map;
            preq.DoorID = p.DoorID;

            nvc.Add("api_method", "deleteProp");
            nvc.Add("api_data", JsonConvert.SerializeObject(preq));
            string info = HTTP.Post(apiUrl, nvc);
            //MessageBox.Show(info);
            //Clipboard.SetText(info);
        }

        public List<Prop> GetPropsByMap(string map)
        {
            NameValueCollection nvc = new NameValueCollection();
            Form form = Program.form;

            g_Map = map;

            nvc.Add("api_method", "getPropsByMap");
            nvc.Add("api_data", "{\"map\":\""+map+"\"}");
            string info = HTTP.Post(apiUrl, nvc);

            try
            {
                var propLoad = JsonConvert.DeserializeObject<List<Prop>>(info);
                Program.form.SetErrorText("");
                return propLoad;
            }
            catch(JsonException e)
            {
                Program.form.SetErrorText("Error: Invalid Map Specified");
                return new List<Prop>();
            }
        }
    }

    public class PropReq
    {
        public string Model { get; set; }
        public string Map { get; set; }
        public string DoorID { get; set; }
    }

    public class Prop
    {
        public string DoorID { get; set; }
        public string Model { get; set; }
    }

    public class API_Response
    {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string ResponseData { get; set; }
    }

    public static class HTTP
    {
        public static String Post(string uri, NameValueCollection pairs)
        {
            byte[] response = null;
            using (WebClient client = new WebClient())
            {
                try
                {
                    response = client.UploadValues(uri, pairs);
                }
                catch (WebException e)
                {
                    API_Response resp = new API_Response();
                    resp.IsError = true; resp.ErrorMessage = e.Message;
                    return JsonConvert.SerializeObject(resp);
                }
            }

            return Encoding.Default.GetString(response);
        }
    }
}
