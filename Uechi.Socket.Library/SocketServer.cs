using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Uechi.Socket.Library
{
    public class SocketServer
    {
        private string strKey;
        private Int32 int32Port;
        private TcpClient objTcpClient;
        private NetworkStream objNetStream;

        public void Iniciar(Boolean booLog)
        {
            int intOpt;
            string strParameter;
            string strReturn;
            try
            {
                int32Port = SocketUtil.Tratar.ToInt32DBNull(SocketUtil.Parameters.GetAppKey("porta"));
                strKey = SocketUtil.Parameters.GetAppKey("chave");
                SocketUtil.Show.Mensagens("Uechi.Server.Socket inicializando o serviço.", booLog);
                TcpListener objTcpListner = new TcpListener(IPAddress.Any, int32Port);
                string IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).Select(ip => ip).FirstOrDefault().ToString();
                objTcpListner.Start();
                SocketUtil.Show.Mensagens("Uechi.Server.Socket inicializado em " + IP, booLog);
                while (true)
                {
                    SocketUtil.Show.Mensagens("Uechi.Server.Socket aguardando conexão...", booLog);
                    TcpClient clientSocket = objTcpListner.AcceptTcpClient();
                    SocketUtil.Show.Mensagens("Uechi.Server.Socket conexão recebida. ", booLog);
                    while (true)
                    {
                        strParameter = Receber(clientSocket);
                        SocketUtil.Show.Mensagens("Uechi.Server.Socket Parametro recebido: " + strParameter, booLog);
                        intOpt = options(strParameter);
                        strReturn = parameters(intOpt, strParameter);
                        if (!Enviar(clientSocket, strReturn))
                        {
                            SocketUtil.Show.Mensagens("Uechi.Server.Socket erro ao enviar parametro resposta.", booLog);
                        }
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                SocketUtil.Show.Mensagens("Erro: " + e.Message.ToString(), booLog);
            }
            SocketUtil.Show.Mensagens("Serviço finalizado...", booLog);
        }

        private string Receber(TcpClient objRecTcpClient)
        {
            string strReturn = null;
            objTcpClient = objRecTcpClient;
            objNetStream = objTcpClient.GetStream();
            try
            {
                int intSize = objTcpClient.SendBufferSize; // objNetStream.ReadByte();
                byte[] bytBuff = new byte[intSize];
                objNetStream.Read(bytBuff, 0, intSize);
                strReturn = Encoding.ASCII.GetString(bytBuff);
                objNetStream.Flush();
                strReturn = strReturn.Replace("\n", "").Replace("\r", "").Replace("\0", "");
            }
            catch
            {
                strReturn = null;
            }
            return strReturn;
        }

        private Boolean Enviar(TcpClient objEnvTcpClient, string strMensagem)
        {
            Boolean booEnv = true;
            objTcpClient = objEnvTcpClient;
            objNetStream = objTcpClient.GetStream();
            try
            {
                strMensagem = strMensagem + "\n";
                byte[] bytBuff = Encoding.UTF8.GetBytes(strMensagem);
                byte intSize = (byte)strMensagem.Length;
                objNetStream.WriteByte(intSize);
                objNetStream.Write(bytBuff, 0, bytBuff.Length);
                objNetStream.Flush();
            }
            catch (IOException)
            {
                booEnv = false;
                objTcpClient.Close();
                Thread.CurrentThread.Abort();
            }
            return booEnv;
        }

        private int options(String strParameter)
        {
            int intRet = 0;
            String strSubParameter;
            try
            {
                if (strParameter != null && strParameter.Length > 0)
                {
                    strSubParameter = strParameter.Substring(32, 1);
                    intRet = SocketUtil.Tratar.ToInt16DBNull(strSubParameter);
                }
            }
            catch (Exception e)
            {
                intRet = 0;
            }
            return intRet;
        }

        private Boolean validate(String strParameter)
        {
            Boolean booVal = false;
            String strChave;
            String strValida;
            try
            {
                strKey = SocketUtil.Parameters.GetAppKey("chave");
                if (strParameter != null && strParameter.Length > 0)
                {
                    strChave = strParameter.Substring(0, 32);
                    strValida = strKey;
                    if (strValida.Length > 0)
                    {
                        if (strChave.ToLower().Equals(strValida.ToLower()))
                        {
                            booVal = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                booVal = false;
            }
            return booVal;
        }

        private String parameters(int intOpt, String strParameter)
        {
            String strRet = null;
            try
            {
                if (strParameter != null && strParameter.Length > 0)
                {
                    if (validate(strParameter))
                    {
                        strRet = command(intOpt);
                    }
                }
            }
            catch (Exception e)
            {
                strRet = null;
            }
            return strRet;
        }

        private String command(int intOpt)
        {
            Process objProcess = new System.Diagnostics.Process();
            string strOut = null;
            try
            {
                if (intOpt == 1)
                {
                    // CPU
                    strOut = SocketInfo.PerformanceWMI.GetCPUStatistics();
                }
                else if (intOpt == 2)
                {
                    // MEM
                    strOut = SocketInfo.PerformanceInfo.GetMEMStatistics();
                }
                else if (intOpt == 3)
                {
                    // DISK
                    strOut = SocketInfo.PerformanceWMI.GetDISKStatistics();
                }
                else if (intOpt == 4)
                {
                    // NETWORK
                    strOut = SocketInfo.PerformanceNetwork.GetNETStatistics();
                }
                else if (intOpt == 9)
                {
                    // All
                    strOut = SocketInfo.PerformanceWMI.GetSYSStatistics();
                }
            }
            catch (Exception e)
            {
                strOut = e.Message.ToString();
            }
            return strOut;
        }

    }
}
