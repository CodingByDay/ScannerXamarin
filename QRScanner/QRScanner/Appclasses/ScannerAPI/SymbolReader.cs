using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;




namespace TrendNET.WMS.Device.Appclasses.ScannerAPI {
    class SymbolReader : GenericReader {
        private Barcode.Barcode _reader = new Barcode.Barcode();

        private static Object _syncLock = new Object();

        /// <summary>
        /// Initialize the reader.
        /// </summary>
        public override bool EnableScanner() {
            //Log.Write(new LogEntry("SymbolReader: EnableScanner"));
            lock (_syncLock) {
                //Log.Write(new LogEntry("  scanner status: " + _reader.EnableScanner));
                if (_reader.EnableScanner == false) {
                    //Log.Write(new LogEntry("  enabling scanner start"));
                    _reader.DecoderParameters.CODABAR = Barcode.DisabledEnabled.Enabled;
                    _reader.DecoderParameters.CODE128 = Barcode.DisabledEnabled.Enabled;
                    _reader.DecoderParameters.CODE39 = Barcode.DisabledEnabled.Enabled;
                    _reader.DecoderParameters.CODE39Params.Code32Prefix = false;
                    _reader.DecoderParameters.CODE39Params.Concatenation = false;
                    _reader.DecoderParameters.CODE39Params.ConvertToCode32 = false;
                    _reader.DecoderParameters.CODE39Params.FullAscii = false;
                    _reader.DecoderParameters.CODE39Params.Redundancy = false;
                    _reader.DecoderParameters.CODE39Params.ReportCheckDigit = false;
                    _reader.DecoderParameters.CODE39Params.VerifyCheckDigit = false;
                    _reader.DecoderParameters.D2OF5 = Barcode.DisabledEnabled.Disabled;
                    _reader.DecoderParameters.EAN13 = Barcode.DisabledEnabled.Enabled;
                    _reader.DecoderParameters.EAN8 = Barcode.DisabledEnabled.Enabled;
                    _reader.DecoderParameters.I2OF5 = Barcode.DisabledEnabled.Enabled;
                    _reader.DecoderParameters.MSI = Barcode.DisabledEnabled.Enabled;
                    _reader.DecoderParameters.UPCA = Barcode.DisabledEnabled.Enabled;
                    _reader.DecoderParameters.UPCE0 = Barcode.DisabledEnabled.Enabled;
                    _reader.ScanParameters.BeepFrequency = 2670;
                    _reader.ScanParameters.BeepTime = 200;
                    _reader.ScanParameters.CodeIdType = Barcode.CodeIdTypes.None;
                    _reader.ScanParameters.LedTime = 3000;
                    _reader.ScanParameters.ScanType = Barcode.ScanTypes.Foreground;
                    _reader.ScanParameters.WaveFile = "";

                    _reader.EnableScanner = true;

                    _reader.OnRead += new Barcode.Barcode.ScannerReadEventHandler(_reader_OnRead);
                    _reader.OnStatus += new Barcode.Barcode.ScannerStatusEventHandler(_reader_OnStatus);

                    //Log.Write(new LogEntry("  enabling scanner end"));
                }
            }
            //Log.Write(new LogEntry("EnableScanner end"));

            return true;
        }

        void _reader_OnStatus(object sender, Symbol.Barcode.BarcodeStatus barcodeStatus) {
            //Log.Write(new LogEntry("SymbolReader: _reader_OnStatus"));
            ReaderState state = new ReaderState();

            switch (barcodeStatus.State) {
                case Symbol.Barcode.States.WAITING:
                    state.ScannerState = ScannerState.Waiting;
                    break;
                case Symbol.Barcode.States.IDLE:
                    state.ScannerState = ScannerState.Idle;
                    break;
                case Symbol.Barcode.States.READY:
                    state.ScannerState = ScannerState.Ready;
                    break;
            }

            NotifyStatus(state);
        }

        void _reader_OnRead(object sender, Symbol.Barcode.ReaderData readerData) {
            //Log.Write(new LogEntry("SymbolReader: _reader_OnRead"));
            ReaderResults res = new ReaderResults();
            res.Data = readerData.Text;

            switch (readerData.Result) {
                case Symbol.Results.SUCCESS:
                    res.Status = ScannerStatus.Success;
                    break;
                case Symbol.Results.E_SCN_READTIMEOUT:
                    res.Status = ScannerStatus.ReadTimeout;
                    break;
                case Symbol.Results.CANCELED:
                    res.Status = ScannerStatus.Canceled;
                    break;
                case Symbol.Results.E_SCN_DEVICEFAILURE:
                    res.Status = ScannerStatus.DeviceFailure;
                    break;
                case Symbol.Results.E_SCN_READINCOMPATIBLE:
                    res.Status = ScannerStatus.ReaderIncompatible;
                    break;
            }

            NotifyRead(res);
        }

        /// <summary>
        /// Stop reading and disable/close the reader.
        /// </summary>
        public override bool DisableScanner() {
            //Log.Write(new LogEntry("SymbolReader: DisableScanner"));
            lock (_syncLock) {
                //Log.Write(new LogEntry("  scanner status: " + _reader.EnableScanner));
                if (_reader.EnableScanner == true) {
                    //Log.Write(new LogEntry("  disabling scanner"));
                    _reader.EnableScanner = false;

                    //Log.Write(new LogEntry("  disabling scanner end"));
                }
            }

            //Log.Write(new LogEntry("DisableScanner end"));

            return true;
        }

        /// <summary>
        /// Start a read on the reader.
        /// </summary>
        public override void StartRead(bool toggleSoftTrigger) {
            //Log.Write(new LogEntry("SymbolReader: StartRead"));
        }

        /// <summary>
        /// Stop all reads on the reader.
        /// </summary>
        public override void StopRead() {
            //Log.Write(new LogEntry("SymbolReader: StopRead"));
        }

        public override ReaderState GetState() {
            //Log.Write(new LogEntry("SymbolReader: GetState"));
            ReaderState state = new ReaderState();
            
            Symbol.Barcode.BarcodeStatus statusData = _reader.Reader.GetNextStatus();

            switch (statusData.State) {
                case Symbol.Barcode.States.WAITING:
                    state.ScannerState = ScannerState.Waiting;
                    break;
                case Symbol.Barcode.States.IDLE:
                    state.ScannerState = ScannerState.Idle;
                    break;
                case Symbol.Barcode.States.READY:
                    state.ScannerState = ScannerState.Ready;
                    break;
            }

            return state;
        }

        public override bool HandleKeyDown(KeyEventArgs e) {
            //Log.Write(new LogEntry("SymbolReader: HandleKeyDown"));
            return false;
        }


        public override bool HandleKeyUp(KeyEventArgs e) {
            //Log.Write(new LogEntry("SymbolReader: HandleKeyUp"));
            return false;
        }

        public override ReaderResults GetNextResult() {
            //Log.Write(new LogEntry("SymbolReader: GetNextResult"));
            ReaderResults res = new ReaderResults();

            Symbol.Barcode.ReaderData readerData = _reader.Reader.GetNextReaderData();
            res.Data = readerData.Text;

            switch (readerData.Result) {
                case Symbol.Results.SUCCESS:
                    res.Status = ScannerStatus.Success;
                    break;
                case Symbol.Results.E_SCN_READTIMEOUT:
                    res.Status = ScannerStatus.ReadTimeout;
                    break;
                case Symbol.Results.CANCELED:
                    res.Status = ScannerStatus.Canceled;
                    break;
                case Symbol.Results.E_SCN_DEVICEFAILURE:
                    res.Status = ScannerStatus.DeviceFailure;
                    break;
                case Symbol.Results.E_SCN_READINCOMPATIBLE:
                    res.Status = ScannerStatus.ReaderIncompatible;
                    break;
            }

            return res;
        }
    }
}
