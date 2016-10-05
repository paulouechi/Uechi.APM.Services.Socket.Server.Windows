using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Uechi.Socket.Library
{
    public class SocketUtil
    {
        public class Tratar
        {
            private static string strRetorno;
            private static int intRetorno;
            private static short shoRetorno;
            private static Int16 int16Retorno;
            private static Int32 int32Retorno;
            private static Int64 int64Retorno;
            private static long lngRetorno;
            private static bool booRetorno;
            private static Boolean BooRetorno;
            private static DateTime dttRetorno;
            private static float fltRetorno;
            private static byte bytRetorno;
            private static object objRetorno;

            static readonly string[] strArrySuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            public static string ToSizeBytes(string strColuna)
            {
                strRetorno = "";
                Int64 int64Retorno = 0;
                try
                {
                    int64Retorno = Convert.ToInt64(strColuna);
                }
                catch (Exception)
                {
                    int64Retorno = 0;
                }
                try
                {
                    if (int64Retorno == 0) { return "0.0 bytes"; }
                    int mag = (int)Math.Log(int64Retorno, 1024);
                    decimal adjustedSize = (decimal)int64Retorno / (1L << (mag * 10));
                    strRetorno = string.Format("{0:n1} {1}", adjustedSize, strArrySuffixes[mag]);
                }
                catch 
                {
                    strRetorno = "0";
                }
                return strRetorno;
            }

            public static bool ToBooleanDBNull(string strColuna)
            {
                booRetorno = false;
                try
                {
                    if (strColuna != null || strColuna != "" || strColuna != string.Empty)
                    {
                        booRetorno = Convert.ToBoolean(strColuna);
                    }
                }
                catch
                {
                }
                return booRetorno;
            }

            public static short ToInt16DBNull(string strColuna)
            {
                shoRetorno = 0;
                try
                {
                    if (strColuna != null || strColuna != "" || strColuna != string.Empty)
                    {
                        shoRetorno = Convert.ToInt16(strColuna);
                    }
                }
                catch
                {
                }
                return shoRetorno;
            }

            public static Int32 ToInt32DBNull(string strColuna)
            {
                int32Retorno = 0;
                try
                {
                    if (strColuna != null || strColuna != "" || strColuna != string.Empty)
                    {
                        int32Retorno = Convert.ToInt32(strColuna);
                    }
                }
                catch
                {
                }
                return int32Retorno;
            }

            public static long ToInt64DBNull(string strColuna)
            {
                int64Retorno = 0;
                try
                {
                    if (strColuna != null || strColuna != "" || strColuna != string.Empty)
                    {
                        int64Retorno = Convert.ToInt64(strColuna);
                    }
                }
                catch
                {
                }
                return int64Retorno;
            }

            public static long ToLongDBNull(string strColuna)
            {
                lngRetorno = 0;
                try
                {
                    if (strColuna != null || strColuna != "" || strColuna != string.Empty)
                    {
                        lngRetorno = Convert.ToInt64(strColuna);
                    }
                }
                catch
                {
                }
                return lngRetorno;
            }

            public static DateTime ToDateTimeDBNull(string strColuna)
            {
                dttRetorno = DateTime.MinValue;
                try
                {
                    if (strColuna != null || strColuna != "" || strColuna != string.Empty)
                    {
                        dttRetorno = Convert.ToDateTime(strColuna);
                    }
                }
                catch
                {
                }
                return dttRetorno;
            }

            public static float ToFloatDBNull(string strColuna)
            {
                fltRetorno = 0;
                try
                {
                    if (strColuna != null || strColuna != "" || strColuna != string.Empty)
                    {
                        fltRetorno = Convert.ToInt16(strColuna);
                    }
                }
                catch
                {
                }
                return fltRetorno;
            }

            public static string ToStringDBNull(string strColuna)
            {
                strRetorno = "";
                try
                {
                    if (strColuna != null || strColuna != "" || strColuna != string.Empty)
                    {
                        strRetorno = strColuna.ToString();
                    }
                }
                catch
                {
                }
                return strRetorno;
            }

            public static byte ToByteDBNull(string strColuna)
            {
                bytRetorno = Convert.ToByte(0);
                try
                {
                    if (strColuna != null || strColuna != "" || strColuna != string.Empty)
                    {
                        bytRetorno = Convert.ToByte(strColuna);
                    }
                }
                catch
                {
                }
                return bytRetorno;
            }

            public static object ToObjectDBNull(string strColuna)
            {
                objRetorno = "";
                try
                {
                    if (strColuna != null || strColuna != "" || strColuna != string.Empty)
                    {
                        objRetorno = strColuna;
                    }
                }
                catch
                {
                }
                return objRetorno;
            }

            public static string ToCharDBNull(string strColuna)
            {
                strRetorno = "";
                try
                {
                    if (strColuna != null || strColuna != "" || strColuna != string.Empty)
                    {
                        strRetorno = strColuna.ToString();
                    }
                }
                catch
                {
                }
                return strRetorno;
            }

        }

        public class Parameters {

            public static string GetAppKey(string strKeyName)
            {
                string strReturn;
                try
                {
                    strReturn = ConfigurationManager.AppSettings[strKeyName].ToString();
                }
                catch
                {
                    strReturn = null;
                }
                return strReturn;
            }

            public static string GetAppConnection(string strKeyConnection)
            {
                string strReturn;
                try
                {
                    strReturn = ConfigurationManager.ConnectionStrings[strKeyConnection].ToString();
                }
                catch
                {
                    strReturn = null;
                }
                return strReturn;
                
            }

        
        }

        public class Show {

            public static void Mensagens(string strMsg, Boolean booExibir)
            {
                if (booExibir) {
                    Console.WriteLine("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "] - " + strMsg);
                }
            }
        
        }
    }
}
