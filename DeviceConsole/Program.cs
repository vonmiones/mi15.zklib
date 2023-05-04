using DeviceConsole.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using zklib;
using System.Windows.Forms;
using Ini.Net;
using System.IO;
using Microsoft.Win32;
using mi15libraries;
namespace DeviceConsole
{
    class Program
    {
        private static SystemConfig sysconfig = new SystemConfig();
        private static Parsers parser = new Parsers();
        private static LogClass log = new LogClass();
        private static string alias { get; set; }
        private static void Dc_EventCallback(object sender, DeviceEventArgs args)
        {
            ConsoleHelper.message(args.EventProgress, args.EventStatus);
            string file = DateTime.Today.ToString("yyyyMMdd")+".att";
            string _log = parser.tap(ip, args.EventProgress);
            if (_log != "invalid")
            {
                LogClass.WriteToFileAsync("attendance/" + file, _log);
            }
            if (File.Exists("device.ini") == true)
            {
                alias = deviceconfig.ReadString(ip.Trim().Replace(".", ""), "alias");
                if (_log != "invalid")
                {
                    LogClass.WriteToFileAsync("attendance/" + alias + "-" + file, _log);
                }
            }
        }
        private static string ip = "192.168.8.201";
        private static int port = 8000;
        private static string coms = "0";
        private const int pingIntervalMs = 3000;
        private static DeviceControl dc = new DeviceControl(ip, coms);
        private static IniFile config = new IniFile("config.ini");
        private static IniFile deviceconfig = new IniFile("device.ini");

        /**
        *   Parameters 
        *   - ip
        *   - port
        *   - alias
        *   - order
        **/
        static void Main(string[] args)
        {
            Application.ApplicationExit += Application_ApplicationExit;
            SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(PowerModeChangedHandler);
            if (!Directory.Exists(@"attendance"))
            {
                Directory.CreateDirectory(@"attendance");
            }
            if (!File.Exists("config.ini"))
            {
                config.WriteString("clock", "sync", "true");
                config.WriteString("clock", "interval", "8000");
                config.WriteString("clock", "timezone", "Asia/Manila");
                config.WriteString("device", "count", "1");
                config.WriteString("device", "enablelog", "true");
                config.WriteString("device", "count", "1");
                config.WriteString("log", "extension", ".log");
                config.WriteString("log", "format", "type-date"); // type | date | time | custom  | or combination
                config.WriteString("log", "path", "log");
                config.WriteString("log", "separtate", "true");
                config.WriteString("log", "type", "info,success,warning,error");
                config.WriteString("device", "count", "1");
                config.WriteString("server", "port", "8000");
            }
            else
            {
                sysconfig.clock = new ClockConfig()
                {
                    interval = config.ReadInteger("clock", "interval"),
                    sync = config.ReadString("clock", "interval"),
                    timezone = config.ReadString("clock", "timezone")
                };
                sysconfig.log = new LogConfig()
                {
                    extension = config.ReadString("log", "extension"),
                    format = config.ReadString("log", "format"),
                    path = config.ReadString("log", "path"),
                    separtate = config.ReadBoolean("log", "separtate"),
                    type = config.ReadString("log", "type")
                };
                sysconfig.server = new ServerConfig()
                {
                     port = config.ReadInteger("server", "port")
                };
                sysconfig.device = new DeviceConfig()
                {
                    count = config.ReadInteger("device", "count"),
                    enablelog = config.ReadBoolean("device", "enablelog")
                };
            }


            if (args.Length > 0)
            {
                ip = args[0].Trim();
                if (args.Length > 1)
                {
                    port = int.Parse(args[1].Trim());
                    coms = args[2];
                    
                    // ip
                    if (args[0].Trim() != "")
                    {
                        int webport = args[1].Trim() != "" ? args[1].Trim().ToInt() : 8000;
                        deviceconfig.WriteString(args[0].Trim().Replace(".",""), "ip", args[0].Trim().ToString());
                        deviceconfig.WriteString(args[0].Trim().Replace(".", ""), "webport", webport.ToString());
                    }
                    // port
                    if (args[1].Trim() != "")
                    {
                        port = deviceconfig.ReadInteger(args[0].Trim().Replace(".", ""), "webport");
                    }
                    // alias
                    if (args[2].Trim() != "")
                    {
                        deviceconfig.WriteString(args[0].Trim().Replace(".", ""), "alias", args[2].Trim());
                    }
                    // order
                    if (args[3].Trim() != "")
                    {
                        deviceconfig.WriteString(args[0].Trim().Replace(".", ""), "order", args[3].Trim());
                    }
                }
            }

            ConnectToDevice();
        }

        private static void PowerModeChangedHandler(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
            {

            }
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private static TcpServer server = new TcpServer(port);
        private static int requestThread = 0;

        private static void TCPServer()
        {
            server.OnReceive += (request) =>
            {

                // Get the request method and URL
                string[] requestLines = request.Split('\n');
                string[] requestTokens = requestLines[0].Split(' ');
                string method = requestTokens[0];
                string url = requestTokens[1];

                // Parse the query string parameters from the URL
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                int queryIndex = url.IndexOf('?');
                if (queryIndex >= 0 && queryIndex < url.Length - 1)
                {
                    string queryString = url.Substring(queryIndex + 1);
                    string[] keyValuePairs = queryString.Split('&');
                    foreach (string keyValuePair in keyValuePairs)
                    {
                        string[] tokens = keyValuePair.Split('=');
                        if (tokens.Length == 2)
                        {
                            string key = tokens[0];
                            string value = tokens[1];
                            parameters[key] = Uri.UnescapeDataString(value);
                        }
                    }

                    //ConsoleHelper.message($"Received request: {request}", "info");

                    string responseContent = "COMMAND: " + parameters["command"];
                    string file = "data";
                    string action = "api";
                    // Handle the request based on the parameters
                    if (method == "GET")
                    {
                        string paraMethod = parameters.ContainsKey("method") ? parameters["method"] : null;
                        string cmd = parameters.ContainsKey("command") ? parameters["command"] : null;
                        action = parameters.ContainsKey("action") ? parameters["action"] : null;
                        string userid = parameters.ContainsKey("userid") ? parameters["userid"] : null;
                        int findex = parameters.ContainsKey("findex") ? int.Parse(parameters["findex"]) : 7;
                        int iflag = parameters.ContainsKey("iflag") ? int.Parse(parameters["iflag"]) : 0;

                        switch (paraMethod.ToLower())
                        {
                            case "online":
                                switch (cmd)
                                {
                                    case "register":
                                        dc.RegisterOnline(userid, findex, iflag);
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case "system":
                                switch (cmd)
                                {

                                    case "reboot":
                                        dc.Reboot();
                                        break;
                                    case "state":
                                        switch (Thread.CurrentThread.ThreadState)
                                        {
                                            case ThreadState.Running:
                                                responseContent = "Running";
                                                break;
                                            case ThreadState.StopRequested:
                                                responseContent = "Stop Requested";
                                                break;
                                            case ThreadState.SuspendRequested:
                                                responseContent = "Suspend Requested";
                                                break;
                                            case ThreadState.Background:
                                                responseContent = "Running in the Background";
                                                break;
                                            case ThreadState.Unstarted:
                                                responseContent = "Unstarted";
                                                break;
                                            case ThreadState.Stopped:
                                                responseContent = "Stopped";
                                                break;
                                            case ThreadState.WaitSleepJoin:
                                                responseContent = "Wait Sleep Join";
                                                break;
                                            case ThreadState.Suspended:
                                                responseContent = "Suspended";
                                                break;
                                            case ThreadState.AbortRequested:
                                                responseContent = "Abort Requested";
                                                break;
                                            case ThreadState.Aborted:
                                                responseContent = "Aborted";
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case "get":
                                file = cmd;
                                switch (cmd)
                                {
                                    case "device-ip":
                                        responseContent = ip;
                                        break;
                                    case "deviceinfo":
                                        string infojson = JsonConvert.SerializeObject(dc.DeviceInfo());
                                        responseContent = infojson;
                                        break;
                                    case "employees":
                                        string employeesjson = JsonConvert.SerializeObject(dc.GetAllEmployees());
                                        responseContent = employeesjson;
                                        break;
                                    case "alluserinfo":
                                        string allemployeesjson = JsonConvert.SerializeObject(dc.GetAllUserInfo());
                                        responseContent = allemployeesjson;
                                        break;
                                    case "fptemplate":
                                        string fpjson = JsonConvert.SerializeObject(dc.GetAllFPTemplate());
                                        responseContent = fpjson;
                                        break;
                                    case "allattlog":
                                        string atljson = JsonConvert.SerializeObject(dc.GetAllAttendanceLog());
                                        responseContent = atljson;
                                        break;
                                    case "newattlog":
                                        string natljson = JsonConvert.SerializeObject(dc.GetNewAttendanceLog());
                                        responseContent = natljson;
                                        break;
                                    default:
                                        responseContent = "Please provide a command parameter";
                                        break;
                                }
                                break;
                                ConsoleHelper.message("Executing " + paraMethod.ToUpper() + ": " + file, "info");
                            default:
                                responseContent = "Please provide a method parameter";
                                break;
                        }

                    }
                    else
                    {
                        responseContent = "Unsupported method";
                    }

                    // process the request and generate a response

                    byte[] responseContentBytes = Encoding.UTF8.GetBytes(responseContent);

                    StringBuilder responseBuilder = new StringBuilder();
                    responseBuilder.AppendLine("HTTP/1.1 200 OK");
                    switch (action)
                    {
                        case "download":
                            responseBuilder.AppendLine($"Content-Disposition: attachment; filename = " + file + ".json");
                            alias = deviceconfig.ReadString(ip.Trim().Replace(".", ""), "alias");
                            LogClass.WriteToFileAsync("attendance/" + alias.ToLower() + "-" + file+".json", responseContent);
                            break;
                        default:
                            break;
                    }
                    responseBuilder.AppendLine($"Content-Type:  application/json; charset=UTF-8");
                    responseBuilder.AppendLine($"Content-Length: {responseContentBytes.Length}");
                    responseBuilder.AppendLine();
                    responseBuilder.Append(responseContent);

                    return responseBuilder.ToString();


                }
                return "";

            };

            server.OnSend += (sender, endPoint) =>
            {
                Console.WriteLine($"Sending respons to Client connected");
            };

            server.OnClientConnected += (sender, endPoint) =>
            {
                //Console.WriteLine($"Client connected: {endPoint}");
            };

            server.OnClientDisconnected += (sender, endPoint) =>
            {
                //Console.WriteLine($"Client disconnected: {endPoint}");
            };

            server.OnError += (sender, exception) =>
            {
                //Console.WriteLine($"Error: {exception}");
            };

            server.Start();

        }

        private static void Server_OnSend(object sender, string response)
        {
            throw new NotImplementedException();
        }

        private static void ConnectToDevice(string constatus = "Connecting to ")
        {
            while (dc.Connect() < 0)
            {
                ConsoleHelper.message(constatus + ip, "error");
            }
            ConsoleHelper.message(dc.broadcast, dc.broadcaststatus);
            dc.syncDateTime();
            dc.EventCallback += Dc_EventCallback;

            Thread tcpServerMonitor = new Thread(TCPServer);
            tcpServerMonitor.Start();
            tcpServerMonitor.Join();

            Thread networkCheckerThread = new Thread(NetworkChecker);
            networkCheckerThread.Start();
            networkCheckerThread.Join();

            Console.ReadLine();
            server.Stop();
            dc.disconnect();
        }
        static void NetworkChecker()
        {
            while (true)
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(ip);

                if (reply.Status == IPStatus.Success)
                {

                    Console.Title = "Ping to " + ip + " succeeded with roundtrip time " + reply.RoundtripTime + " ms";
                }
                else
                {
                    Console.Title = "Ping to " + ip + " failed with status " + reply.Status;
                    ConsoleHelper.message("Ping to " + ip + " failed with status " + reply.Status, "error");
                    dc.EventCallback -= Dc_EventCallback;
                    ConnectToDevice("Reconnecting to ");
                    //Environment.Exit(0);
                }

                Thread.Sleep(pingIntervalMs);
            }
        }
    }
}
