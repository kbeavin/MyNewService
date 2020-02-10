﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MyNewService
{
    public partial class MyNewService : ServiceBase
    {
        private int eventId = 1;

        #region Constructors

        //public MyNewService() 
        //{
        //    InitializeComponent();
        //    eventLog1 = new System.Diagnostics.EventLog();
        //    if (!System.Diagnostics.EventLog.SourceExists("MySource"))
        //    {
        //        System.Diagnostics.EventLog.CreateEventSource(
        //            "MySource", "MyNewLog");
        //    }
        //    eventLog1.Source = "MySource";
        //    eventLog1.Log = "MyNewLog";
        //}

        public MyNewService(string[] args)
        {
            InitializeComponent();

            // Default args values
            string eventSourceName = "MySource";
            string logName = "MyNewLog";

            // Set args values
            if (args.Length > 0)
            {
                eventSourceName = args[0];
            }

            if (args.Length > 1)
            {
                logName = args[1];
            }

            // Instantiate Event Log
            eventLog1 = new EventLog();

            // If doesn't exist create it
            if (!EventLog.SourceExists(eventSourceName))
            {
                EventLog.CreateEventSource(eventSourceName, logName);
            }

            // Set the values of our Event Log
            eventLog1.Source = eventSourceName;
            eventLog1.Log = logName;
        }

        #endregion

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("In OnStart.");

            #region Set Start Pending Status

            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            #endregion

            #region Start Timer

            // Set up a timer that triggers every minute.
            Timer timer = new Timer();
            timer.Interval = 60000; // 60 seconds
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();

            #endregion

            #region Update State to Running

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            #endregion
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("In OnStop.");

            #region Set Stop Pending Status

            // Update the service state to Stop Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            #endregion

            #region Update State to Stopped

            // Update the service state to Stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            #endregion
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.
            eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
        }

        #region Other Overrideable Methods 

        /*============================================*/
        /* You can override the OnPause, OnContinue,  */
        /* and OnShutdown methods to define additional*/
        /* processing for your component.             */
        /*============================================*/
        protected override void OnContinue()
        {
            eventLog1.WriteEntry("In OnContinue.");
        }

        protected override void OnPause()
        {
            eventLog1.WriteEntry("In OnPause.");
        }

        protected override void OnShutdown()
        {
            eventLog1.WriteEntry("In OnShutdown.");
        }

        #endregion

        #region Setting the Service Status

        /*============================================*/
        /* You can implement the SERVICE_START_PENDING*/
        /* status settings and others by adding code  */
        /* that calls the Windows SetServiceStatus    */
        /* function.                                  */
        /*============================================*/
        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

        #endregion
    }
}
