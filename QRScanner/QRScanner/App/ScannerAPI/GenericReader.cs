using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


using QRscanner.App.Components;
using static Java.Util.ResourceBundle;

namespace QRscanner.App.ScannerAPI {
    public enum ScannerStatus { Success = 1, ReadTimeout = 2, DeviceFailure = 3, ReaderIncompatible = 4, Canceled = 5 }
    public enum ScannerState { Waiting = 1, Idle = 2, Ready = 3 }

    public class ReaderState {
        public ScannerState ScannerState { get; set; }
    }

    public class ReaderResults {
        public String Data { get; set; }
        public ScannerStatus Status { get; set; }
    }

    public class GenericReader {
        //private Control control;
        public delegate void ReadDelegate(ReaderResults rs);
        public delegate void StateDelegate(ReaderState rs);

        private event ReadDelegate _readNotifyHandler;
        private event StateDelegate _statusNotifyHandler;

        public void SetEvent(ReadDelegate readEvent) {
            //Log.Write(new LogEntry("GenericReader: SetEvent"));
            this._readNotifyHandler += readEvent;
        }

        public void ClearEvent(ReadDelegate readEvent) {
            //Log.Write(new LogEntry("GenericReader: ClearEvent"));
            this._readNotifyHandler -= readEvent;
        }

        public void SetStatusEvent(StateDelegate statusEvent) {
            //Log.Write(new LogEntry("GenericReader: SetStatusEvent"));
            this._statusNotifyHandler += statusEvent;
        }

        public void ClearStatusEvent(StateDelegate statusEvent) {
            //Log.Write(new LogEntry("GenericReader: ClearStatusEvent"));
            this._statusNotifyHandler -= statusEvent;
        }

        protected void NotifyStatus(ReaderState rs) {
            //Log.Write(new LogEntry("GenericReader: NotifyStatus"));
            if (_statusNotifyHandler != null)
                _statusNotifyHandler(rs);
        }

        protected void NotifyRead(ReaderResults rs) {
            //Log.Write(new LogEntry("GenericReader: NotifyRead"));
            if (_readNotifyHandler != null)
                _readNotifyHandler(rs);
        }

        public void SetForControl(Control c) {
            //Log.Write(new LogEntry("GenericReader: SetForControl"));
            //control = c;
            //control.BackColor = Color.Aqua;
            c.BackColor = Color.Aqua;

            var p = (c.Parent as WMSForm);
            p.EnableScanner();
        }

        public virtual bool HandleKeyDown(KeyEventArgs e) {
            //Log.Write(new LogEntry("GenericReader: HandleKeyDown"));
            return false;
        }

        public virtual bool HandleKeyUp(KeyEventArgs e) {
            //Log.Write(new LogEntry("GenericReader: HandleKeyUp"));
            return false;
        }

        public virtual ReaderResults GetNextResult() {
            //Log.Write(new LogEntry("GenericReader: GetNextResult"));
            return null;
        }

        public virtual ReaderState GetState() {
            //Log.Write(new LogEntry("GenericReader: GetState"));
            return null;
        }


        public virtual bool EnableScanner() {
            //Log.Write(new LogEntry("GenericReader: EnableScanner"));
            return false;
        }

        public virtual bool DisableScanner() {
            //Log.Write(new LogEntry("GenericReader: DisableScanner"));
            return false;
        }

        public virtual void StartRead(bool toggleSoftTrigger) {
            //Log.Write(new LogEntry("GenericReader: StartRead"));
        }

        public virtual void StopRead() {
            //Log.Write(new LogEntry("GenericReader: StopRead"));
        }
    }

    public static class ScannerFactory {
        private static GenericReader _genericReader = null;

        private static GenericReader Instantiate() {
            //Log.Write(new LogEntry("GenericReader: ScannerFactory.Instantiate"));

            String driverType = WMSDeviceConfig.GetString("ScannerType", "");
            //Log.Write(new LogEntry("GenericReader: ScannerFactory.Instantiate DriverType=" + driverType));

            switch (driverType.Trim().ToUpper()) {
                case "SYMBOL" :
                    return (GenericReader)(new SymbolReader());
                case "HONEYWELL" :
                    return (GenericReader)(new HoneywellReader());
            }

            return null;
        }

        public static GenericReader ReaderInstance {
            get {
                //Log.Write(new LogEntry("GenericReader: ScannerFactory.ReaderInstance.Get"));

                if (_genericReader == null)
                    _genericReader = Instantiate();

                return _genericReader;
            }
        }
    }
}
