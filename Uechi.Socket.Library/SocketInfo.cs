using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Management;
using System.Management.Instrumentation;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;

namespace Uechi.Socket.Library
{
    public class SocketInfo
    {

        public static class PerformanceInfo
        {
            [DllImport("psapi.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

            [StructLayout(LayoutKind.Sequential)]
            private struct PerformanceInformation
            {
                public int Size;
                public IntPtr CommitTotal;
                public IntPtr CommitLimit;
                public IntPtr CommitPeak;
                public IntPtr PhysicalTotal;
                public IntPtr PhysicalAvailable;
                public IntPtr SystemCache;
                public IntPtr KernelTotal;
                public IntPtr KernelPaged;
                public IntPtr KernelNonPaged;
                public IntPtr PageSize;
                public int HandlesCount;
                public int ProcessCount;
                public int ThreadCount;
            }

            private static Int64 GetPhysicalAvailableMemoryInMiB()
            {
                PerformanceInformation pi = new PerformanceInformation();
                if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
                {
                    return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
                }
                else
                {
                    return -1;
                }

            }

            private static Int64 GetTotalMemoryInMiB()
            {
                PerformanceInformation pi = new PerformanceInformation();
                if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
                {
                    return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
                }
                else
                {
                    return -1;
                }

            }

            public static string GetMEMStatistics()
            {
                string strReturn = null;
                try
                {
                    Int64 int64Total = SocketInfo.PerformanceInfo.GetTotalMemoryInMiB();
                    Int64 int64Livre = SocketInfo.PerformanceInfo.GetPhysicalAvailableMemoryInMiB();
                    string strMEMPercentual = String.Format("{0:00}", ((decimal)int64Livre / (decimal)int64Total) * 100);
                    strReturn = "Memory Usage: " + (int64Total - int64Livre).ToString() + "MB;Memory Total: " + int64Total.ToString() + ";" + strMEMPercentual + "%;100%";
                }
                catch
                {
                    strReturn = null;
                }
                return strReturn;
            }

        }

        public static class PerformanceWMI
        {

            public static string GetCPUStatistics()
            {
                string strReturn = null;
                string strCPU = "0";
                try
                {
                    ManagementObjectSearcher objMngObjSrc = new ManagementObjectSearcher("select PercentProcessorTime from Win32_PerfFormattedData_PerfOS_Processor where Name = '_Total'");
                    if (objMngObjSrc != null)
                    {
                        foreach (ManagementObject obj in objMngObjSrc.Get())
                        {
                            strCPU = obj["PercentProcessorTime"].ToString();
                        }
                        strReturn = "CPU Total Usage: " + strCPU + "%;CPU Total Idle: " + (100 - Convert.ToInt16(strCPU)).ToString() + "%";
                    }
                }
                catch
                {
                    strReturn = null;
                }
                return strReturn;
            }

            private static string DISK(int intOpt)
            {
                decimal decValueOne;
                decimal decValueTwo;
                long lngValueOne;
                long lngValueTwo;
                string strReturn = null;
                try
                {
                    string strQuery = "select FreeSpace,Size,Name from Win32_LogicalDisk where DriveType=3";

                    ManagementObjectSearcher objMngObjSrc = new ManagementObjectSearcher(strQuery);
                    if (objMngObjSrc != null)
                    {
                        foreach (ManagementObject obj in objMngObjSrc.Get())
                        {
                            if (intOpt == 1)
                            {
                                lngValueOne = SocketUtil.Tratar.ToLongDBNull(obj["Size"].ToString());
                                lngValueTwo = SocketUtil.Tratar.ToLongDBNull(obj["FreeSpace"].ToString());
                                strReturn = (lngValueOne - lngValueTwo).ToString();
                            }
                            if (intOpt == 2)
                            {
                                strReturn = obj["Size"].ToString();
                            }
                            if (intOpt == 3)
                            {
                                strReturn = obj["FreeSpace"].ToString();
                            }
                            if (intOpt == 4)
                            {
                                decValueOne = Convert.ToDecimal(obj["Size"].ToString()) - Convert.ToDecimal(obj["FreeSpace"].ToString());
                                decValueTwo = Convert.ToDecimal(obj["Size"].ToString());
                                strReturn = ((decValueOne / decValueTwo) * 100).ToString().Substring(0, 5);
                            }

                        }
                    }
                }
                catch
                {
                    strReturn = null;
                }
                return strReturn;
            }

            public static string GetDISKStatistics()
            {
                string strReturn = null;
                try
                {
                    strReturn = "Disk Usage:" + SocketUtil.Tratar.ToSizeBytes(SocketInfo.PerformanceWMI.DISK(1)) + ";Disk Total:" + SocketUtil.Tratar.ToSizeBytes(SocketInfo.PerformanceWMI.DISK(2)) + ";" + String.Format("{0:D2}", SocketInfo.PerformanceWMI.DISK(4)) + " %;100%;";
                }
                catch
                {
                    strReturn = null;
                }
                return strReturn;
            }

            public static string GetSYSStatistics()
            {
                string strReturn = null;
                try
                {
                    string strConnected = checkConnected();
                    string strOSPlataform = "Windows";
                    string strOSVersion = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>() select x.GetPropertyValue("Caption")).FirstOrDefault().ToString();
                    string strOSArchitecture = (from x in new ManagementObjectSearcher("select AddressWidth from Win32_Processor where DeviceID='CPU0'").Get().Cast<ManagementObject>() select x.GetPropertyValue("AddressWidth")).FirstOrDefault().ToString(); ;
                    string strOSRelease = Environment.OSVersion.ToString();
                    string strHostName = Environment.MachineName.ToString();
                    string strLocalIP = GetLocalIPAddress();
                    string strPublicIP = GetPublicIPAddress(2);
                    strReturn = "Internet:" + strConnected + ";Operating System Type : " + strOSPlataform + ";OS Version : " + strOSVersion + ";Architecture : " + strOSArchitecture + ";Kernel Release : " + strOSRelease + ";Hostname : " + strHostName + ";Internal IP : " + strLocalIP + ";External IP : " + strPublicIP + ";Name Servers : " + strHostName + ";Logged In users : ;Ram Usages : ; total used free shared buff/cache available;Mem: ;Swap Usages : ; total used free shared buff/cache available;Swap: ;Disk Usages : ;Load Average : ;System Uptime Days/(HH:MM) : ;";
                }
                catch
                {
                }
                return strReturn;
            }

            private static string checkConnected()
            {
                string strReturn = "Desconnected";
                string nameOrAddress = "www.google.com";
                bool pingable = false;
                Ping pinger = new Ping();
                try
                {
                    PingReply reply = pinger.Send(nameOrAddress);
                    pingable = reply.Status == IPStatus.Success;
                    if (pingable)
                    {
                        strReturn = "Connected";
                    }
                }
                catch (PingException)
                {
                    strReturn = "Error";
                }
                return strReturn;
            }

            private static string GetLocalIPAddress()
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                throw new Exception("127.0.0.1");
            }

            private static string GetPublicIPAddress(int intOpt)
            {
                string strReturn = "127.0.0.1";
                try
                {
                    if (intOpt == 1)
                    {
                        string externalIP;
                        externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                        externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")).Matches(externalIP)[0].ToString();
                        strReturn = externalIP;
                    }
                    else if (intOpt == 2)
                    {
                        string url = "http://freegeoip.net/xml/";
                        WebClient wc = new WebClient();
                        wc.Proxy = null;
                        MemoryStream ms = new MemoryStream(wc.DownloadData(url));
                        XmlTextReader rdr = new XmlTextReader(url);
                        XmlDocument doc = new XmlDocument();
                        ms.Position = 0;
                        doc.Load(ms);
                        ms.Dispose();
                        Dictionary<string, string> KeyValue;
                        KeyValue = new Dictionary<string, string>();
                        foreach (XmlElement el in doc.ChildNodes[0].ChildNodes)
                        {
                            KeyValue.Add(el.Name, el.InnerText);
                        }
                        strReturn = KeyValue.Where(x => x.Value != null).Select(x => x.Value).FirstOrDefault();
                    }
                }
                catch
                {
                    strReturn = "127.0.0.1";
                }
                return strReturn;
            }
        }

        public static class PerformanceNetwork
        {
            private static Int64 int64RecSnt;
            private static Int64 int64RecRcv;

            public static string GetNETStatistics()
            {
                Int64 int64TotSnt = 0;
                Int64 int64TotRcv = 0;

                Int64 int64DifSnt = 0;
                Int64 int64DifRcv = 0;

                int64RecSnt = 0;
                int64RecRcv = 0;

                string strReturn = "";
                try
                {
                    NetworkInterface[] nicArr = NetworkInterface.GetAllNetworkInterfaces();
                    NetworkInterface nic = nicArr[0];
                    foreach (NetworkInterface adapter in nicArr)
                    {
                        IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                        GatewayIPAddressInformationCollection addresses = adapterProperties.GatewayAddresses;
                        if (addresses.Count > 0)
                        {
                            nic = adapter;
                        }
                    }
                    IPv4InterfaceStatistics interfaceStats = nic.GetIPv4Statistics();

                    int64RecSnt = Int64.Parse(interfaceStats.BytesSent.ToString());
                    int64RecRcv = Int64.Parse(interfaceStats.BytesReceived.ToString());

                    System.Threading.Thread.Sleep(100);

                    interfaceStats = nic.GetIPv4Statistics();

                    int64TotSnt = Int64.Parse(interfaceStats.BytesSent.ToString());
                    int64TotRcv = Int64.Parse(interfaceStats.BytesReceived.ToString());

                    int64DifSnt = int64TotSnt - int64RecSnt;
                    int64DifRcv = int64TotRcv - int64RecRcv;

                    strReturn = "RXbytes Total: " + SocketUtil.Tratar.ToSizeBytes(int64TotRcv.ToString()) + "; RXbytes: " + SocketUtil.Tratar.ToSizeBytes(int64DifRcv.ToString()) + "; TXbytes Total: " + SocketUtil.Tratar.ToSizeBytes(int64TotSnt.ToString()) + "; TXbytes: " + SocketUtil.Tratar.ToSizeBytes(int64DifSnt.ToString()) + "";

                }
                catch (Exception)
                {
                    strReturn = null;
                }
                return strReturn;
            }

        }

    }
}
