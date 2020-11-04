using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using TrendNET.WMS.Device.App;
using TrendNET.WMS.Device.Components;

using HandHeldProducts.Embedded.Decoding;
using HandHeldProducts.Embedded.Hardware;

namespace TrendNET.WMS.Device.App.ScannerAPI {
    class HoneywellReader : GenericReader {
        private DecodeAssembly oDecodeAssembly = new DecodeAssembly();
        
        private int SCAN_KEY = Convert.ToInt32 (WMSDeviceConfig.GetString ("ScanKeyCode", "193"));
        // 193 = Honeywell D6500
        // 0x2A = 42 = ?
        // 0x86 = 134 = ?

        private ReaderResults _lastResults = new ReaderResults();
        private static Object _syncLock = new Object();

        private bool _isScanButtonPressed = false;


        public override bool HandleKeyDown(KeyEventArgs e) {
            //Log.Write(new LogEntry("HoneywellReader: HandleKeyDown"));
            if (e.KeyCode == (Keys)SCAN_KEY && !_isScanButtonPressed)
            {
                //Log.Write(new LogEntry("HoneywellReader: HandleKeyDown -- SCAN key pressed"));
                _isScanButtonPressed = true;
                // Submit a read.
                oDecodeAssembly.ScanBarcode();
                e.Handled = true;

                NotifyStatus(GetState());

                return true;
            }
            else
            {
                //Log.Write(new LogEntry("HoneywellReader: HandleKeyDown -- key=" + e.KeyCode.ToString()));
            }

            return false;
        }

        public override bool HandleKeyUp(KeyEventArgs e) {
            //Log.Write(new LogEntry("HoneywellReader: HandleKeyUp"));
            if (e.KeyCode == (Keys)SCAN_KEY)
            {
                //Log.Write(new LogEntry("HoneywellReader: HandleKeyUp -- SCAN key released"));
                _isScanButtonPressed = false;
                oDecodeAssembly.CancelScanBarcode();
                e.Handled = true;

                NotifyStatus(GetState());

                return true;
            }
            else
            {
                //Log.Write(new LogEntry("HoneywellReader: HandleKeyUp -- key=" + e.KeyCode.ToString ()));
            }

            return false;
        }

        public override ReaderState GetState() {
            //Log.Write(new LogEntry("HoneywellReader: GetState"));
            ReaderState state = new ReaderState();

            if (_isScanButtonPressed) {
                state.ScannerState = ScannerState.Waiting;
            } else {
                state.ScannerState = ScannerState.Ready;
            }

            return state;
        }

        public override ReaderResults GetNextResult() {
            //Log.Write(new LogEntry("HoneywellReader: GetNextResult"));
            ReaderResults res = new ReaderResults() {
                Status = ScannerStatus.Canceled,
                Data = ""
            };

            if (_lastResults == null)
                return res;

            return _lastResults;
        }

        /// <summary>
        /// Initialize the reader.
        /// </summary>
        public override bool EnableScanner() {
            //Log.Write(new LogEntry("HoneywellReader: EnableScanner"));
            // If the reader is already initialized then fail the initialization.
            lock (_syncLock) {
                try {
                    //Log.Write(new LogEntry("HoneywellReader: EnableScanner API Revision = " + oDecodeAssembly.ApiRevision));
                    //Log.Write(new LogEntry("HoneywellReader: EnableScanner Decoder Revision = " + oDecodeAssembly.DecoderRevision));

                    oDecodeAssembly.EnableSymbology(SymbologyConfigurator.Symbologies.All, true);
                    oDecodeAssembly.DecodeMode = DecodeAssembly.DecodeModes.Standard;
                    /*
                    oDecodeAssembly.EnableSymbology(SymbologyConfigurator.Symbologies.Code128, true);
                    oDecodeAssembly.EnableSymbology(SymbologyConfigurator.Symbologies.Code39, true);
                    oDecodeAssembly.EnableSymbology(SymbologyConfigurator.Symbologies.EAN13, true);
                    oDecodeAssembly.EnableSymbology(SymbologyConfigurator.Symbologies.EAN8, true);
                    oDecodeAssembly.EnableSymbology(SymbologyConfigurator.Symbologies.MSI, true);
                    oDecodeAssembly.EnableSymbology(SymbologyConfigurator.Symbologies.UPCA, true);
                    oDecodeAssembly.EnableSymbology(SymbologyConfigurator.Symbologies.UPCE0, true);
                    */
                    _isScanButtonPressed = false;

                    //--- Add a Handler for the Decode Event ---
                    oDecodeAssembly.DecodeEvent += new DecodeAssembly.DecodeEventHandler(oDecodeAssembly_DecodeEvent);
                } catch (Exception ex) {
                    Services.Services.ReportException(ex);
                    //Log.Write(new LogEntry("Error enabling scanner: " + ex.ToString()));

                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Stop reading and disable/close the reader.
        /// </summary>
        public override bool DisableScanner() {
            //Log.Write(new LogEntry("HoneywellReader: DisableScanner"));
            // If we have a reader
            if (oDecodeAssembly != null) {
                try {
                    oDecodeAssembly.CancelScanBarcode();

                    return true;
                } catch (Exception ex) {
                    Services.Services.ReportException(ex);
                    //Log.Write(new LogEntry("Error disabling scanner: " + ex.ToString()));
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Start a read on the reader.
        /// </summary>
        public override void StartRead(bool toggleSoftTrigger) {
            //Log.Write(new LogEntry("HoneywellReader: StartRead"));
        }

        /// <summary>
        /// Stop all reads on the reader.
        /// </summary>
        public override void StopRead() {
            //Log.Write(new LogEntry("HoneywellReader: StopRead"));
        }

        private void oDecodeAssembly_DecodeEvent(object sender, DecodeAssembly.DecodeEventArgs e) {
            //Log.Write(new LogEntry("HoneywellReader: oDecodeAssembly_DecodeEvent"));
            try {
                _lastResults = new ReaderResults();

                //--- Was the Decode Attempt Successful? ---
                if (e.ResultCode == DecodeAssembly.ResultCodes.Success) {
                    //Log.Write(new LogEntry("HoneywellReader: oDecodeAssembly_DecodeEvent -> Success"));
                    _lastResults.Status = ScannerStatus.Success;
                    _lastResults.Data = e.Message;

                    //Helpers.Log.Write(string.Format("Read done:\nmessage: {0}, codeID: {1}, aimID: {2}, aimMod: {3}, length: {4}, result: {5}",e.Message, e.CodeId, e.AimId, e.AimModifier, e.Length.ToString(), e.ResultCode.ToString()));

                    //--- Play an SDK Provided Audible Sound ---
                    oDecodeAssembly.Sound.Play(Sound.SoundTypes.Success, true);
                } else {
                    //Log.Write(new LogEntry("HoneywellReader: oDecodeAssembly_DecodeEvent -> Failed"));
                    if (e.DecodeException != null) {
                        //--- Async Decode Exception ---
                        switch (e.DecodeException.ResultCode) {
                            case DecodeAssembly.ResultCodes.Cancel:            // Async Decode was Canceled
                                //Log.Write(new LogEntry("HoneywellReader: oDecodeAssembly_DecodeEvent -> Failed -> Cancel"));
                                return;
                            case DecodeAssembly.ResultCodes.NoDecode:          // Scan Timeout
                                //Log.Write(new LogEntry("HoneywellReader: oDecodeAssembly_DecodeEvent -> Failed -> NoDecode"));
                                break;
                            default:
                                //Log.Write(new LogEntry("HoneywellReader: oDecodeAssembly_DecodeEvent -> Failed -> Exception: " + e.DecodeException.Message));
                                break;
                        }
                    } else {
                        //--- Generic Async Exception ---
                        //Log.Write(new LogEntry("Generic async exception"));
                             
                        //MessageBox.Show(e.Exception.Message);

                        _lastResults.Status = ScannerStatus.DeviceFailure;
                    }
                }

                NotifyStatus(GetState());
                NotifyRead(_lastResults);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
