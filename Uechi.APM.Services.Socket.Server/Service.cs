using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using Uechi.Socket.Library;

namespace Uechi.APM.Services.Socket.Server
{
    public partial class Service : ServiceBase
    {
        protected string strOutMsg;

        Timer objTimer = new Timer();
        Boolean booLog = SocketUtil.Tratar.ToBooleanDBNull(SocketUtil.Parameters.GetAppKey("log"));


        public Service()
        {
            InitializeComponent();
            try
            {
                SocketUtil.Show.Mensagens("Inicialização do Serviço " + SocketUtil.Parameters.GetAppKey("sistema") + ".", booLog);
                objTimer.Interval = Convert.ToInt64(SocketUtil.Tratar.ToInt64DBNull(SocketUtil.Parameters.GetAppKey("intervalo")));
            }
            catch (Exception ex)
            {
                SocketUtil.Show.Mensagens("Erro na inicialização do serviço " + SocketUtil.Parameters.GetAppKey("sistema") + ": " + ex.Message.ToString(), booLog);
            }
            finally
            {
            }
        }

        protected void objTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                objTimer.Enabled = false;
                SocketServer objSock = new SocketServer();
                objSock.Iniciar(booLog);
                objTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                SocketUtil.Show.Mensagens("Falha na inicialização do serviço " + SocketUtil.Parameters.GetAppKey("sistema") + ": " + ex.Message.ToString(), booLog);
            }
        }

        protected override void OnStart(string[] args)
        {
            objTimer.Elapsed += new ElapsedEventHandler(objTimer_Elapsed);
            objTimer.Enabled = true;
        }

        protected override void OnPause()
        {
            objTimer.Enabled = false;
            SocketUtil.Show.Mensagens("Serviço " + SocketUtil.Parameters.GetAppKey("sistema") + " finalizado.", booLog);
        }

        protected override void OnContinue()
        {
            objTimer.Enabled = true;
            SocketUtil.Show.Mensagens("Serviço " + SocketUtil.Parameters.GetAppKey("sistema") + " finalizado.", booLog);
        }

        protected override void OnStop()
        {
            objTimer.Enabled = false;
            SocketUtil.Show.Mensagens("Serviço " + SocketUtil.Parameters.GetAppKey("sistema") + " finalizado.", booLog);
        }
    }
}
