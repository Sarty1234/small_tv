using SharpHook;
using SharpHook.Data;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace smallTV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // *************************************************** //
        //                                                     //
        //               Loading config params                 //
        //    *It creates links to values stored in config*    //
        //                                                     //
        // *************************************************** //

        Key HotkeyResetFinishKey => Config.Instance.HotkeyResetFinishKey;
        List<Key> MenuKeys = Config.Instance.MenuKeys;
        List<Key> TVKeys = Config.Instance.TVKeys;
        List<Key> PanicKeys = Config.Instance.PanicKeys;



        // *************************************************** //
        //                                                     //
        //                 Main Window Events                  //
        //                                                     //
        // *************************************************** //
        public MainWindow()
        {
            InitializeComponent();
            UpdateKeyLabels();
        }


        // Adding listeners for key events
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SimpleGlobalHook _hook = new SimpleGlobalHook();

            _hook.KeyPressed += OnKeyPressed;
            _hook.KeyReleased += OnKeyReleased;


            await Task.Run(() => _hook.Run());
        }


        // Function responsible for user creating hotkeys
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (
                (MenuKeys.Count == 0 && e.Key != HotkeyResetFinishKey)
                ||
                (MenuKeys[0] != HotkeyResetFinishKey && !MenuKeys.Contains(e.Key))
                )
            {
                MenuKeys.Insert(0, e.Key);
                Config.Instance.Save();
                e.Handled = true;
                UpdateKeyLabels();
            }
            else
            if (
                (TVKeys.Count == 0 && e.Key != HotkeyResetFinishKey)
                ||
                (TVKeys[0] != HotkeyResetFinishKey && !TVKeys.Contains(e.Key))
                )
            {
                TVKeys.Insert(0, e.Key);
                Config.Instance.Save();
                e.Handled = true;
                UpdateKeyLabels();
            }
            else
            if (
                (PanicKeys.Count == 0 && e.Key != HotkeyResetFinishKey)
                ||
                (PanicKeys[0] != HotkeyResetFinishKey && !PanicKeys.Contains(e.Key))
                )
            {
                PanicKeys.Insert(0, e.Key);
                Config.Instance.Save();
                e.Handled = true;
                UpdateKeyLabels();
            }
        }



        // *************************************************** //
        //                                                     //
        //          Handling window key press events           //
        //                                                     //
        // *************************************************** //

        private const int _SecondsBeforeRefresh = 1;
        private DateTime _lastKeyUpdate = DateTime.MinValue;
        private static readonly List<Key> PressedKeys = new List<Key>();
        private void OnKeyPressed(object sender, KeyboardHookEventArgs e)
        {
            // locking so action can't be done simultiniously on multiple threads
            lock (PressedKeys) {

                // To avoid bugs clearing pressed keys after some time of inactivity
                if ((DateTime.Now - _lastKeyUpdate).TotalSeconds >= _SecondsBeforeRefresh)
                {
                    PressedKeys.Clear();
                }
                _lastKeyUpdate = DateTime.Now;


                // Parsing key value from event data and handling special keys
                Key pressedKey = GetKeyFromVirtualCode((int)e.Data.RawCode);
                PressedKeys.Add(pressedKey);


                // Invoking corresponding action if key sequince is met
                if (PressedKeys != null)
                {
                    if (MenuKeys != null && MenuKeys.Contains(HotkeyResetFinishKey) 
                        && PressedKeys.SequenceEqual(MenuKeys[1..]))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MenuFunction();
                        });
                    }


                    if (TVKeys != null && TVKeys.Contains(HotkeyResetFinishKey)
                        && PressedKeys.SequenceEqual(TVKeys[1..]))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            TVFunction();
                        });
                    }


                    if (PanicKeys != null && PanicKeys.Contains(HotkeyResetFinishKey)
                        && PressedKeys.SequenceEqual(PanicKeys[1..]))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            PanicFunction();
                        });
                    }
                }
            }
        }


        private void OnKeyReleased(object sender, KeyboardHookEventArgs e)
        {
            lock (PressedKeys)
            {
                Key releasedKey = GetKeyFromVirtualCode((int)e.Data.RawCode);
                PressedKeys.Remove(releasedKey);
            }
        }


        /// <summary>
        /// Gets Key type value from virtual key int code
        /// (also handles special keys)
        /// </summary>
        private static Key GetKeyFromVirtualCode(int keyCode)
        {
            Key pressedKey = KeyInterop.KeyFromVirtualKey(keyCode);
            if (keyCode == 160)
            {
                pressedKey = Key.LeftShift;
            }
            else if (keyCode == 162)
            {
                pressedKey = Key.LeftCtrl;
            }
            else if (keyCode == 164)
            {
                pressedKey = Key.LeftAlt;
            }

            return pressedKey;
        }



        // *************************************************** //
        //                                                     //
        //                   Handling UI keys                  //
        //                                                     //
        // *************************************************** //

        private void ResetMenuKeysButton_Click(object sender, RoutedEventArgs e)
        {
            MenuKeys.Clear();
            UpdateKeyLabels();
            this.Focus();
        }

        private void ResetTVKeysButton_Click(object sender, RoutedEventArgs e)
        {
            TVKeys.Clear();
            UpdateKeyLabels();
            this.Focus();
        }

        private void ResetPanicKeysButton_Click(object sender, RoutedEventArgs e)
        {
            PanicKeys.Clear();
            UpdateKeyLabels();
            this.Focus();
        }

        private void SelectWindowButton_Click(object sender, RoutedEventArgs e)
        {
            Console.Beep(1000, 100);
            Thread.Sleep(5000);
            Console.Beep(1000, 100);

            tvProcess = GetFocusedProcess();
            SelectedWindowLabel.Content = tvProcess.ProcessName;
            this.Activate();
            this.Focus();
        }


        /// <summary>
        /// Updates hotkey labels text
        /// </summary>
        private void UpdateKeyLabels()
        {
            string keySeparator = " -> ";
            int keySeparatorLength = keySeparator.Length;
            string awaitingNewKeyText = "Enter new hotkey";
            string howToStopText = $" (Press {HotkeyResetFinishKey.ToString()} to stop)";


            string keysString = "";

            if (MenuKeys.Count() == 0)
            {
                MenuKeysLabel.Content = awaitingNewKeyText;
            }
            else
            {
                for (int i = 0; i < MenuKeys.Count; i++)
                {
                    Key k = MenuKeys[i];
                    if (k == HotkeyResetFinishKey)
                        continue;
                    keysString += k.ToString() + keySeparator;
                }

                keysString = keysString.Remove(keysString.Length - keySeparatorLength);

                if (MenuKeys[0] != HotkeyResetFinishKey) 
                    keysString += howToStopText;

                MenuKeysLabel.Content = keysString;
            }
            keysString = "";


            if (TVKeys.Count() == 0)
            {
                TVKeysLabel.Content = awaitingNewKeyText;
            }
            else
            {
                for (int i = 0; i < TVKeys.Count; i++)
                {
                    Key k = TVKeys[i];
                    if (k == HotkeyResetFinishKey)
                        continue;
                    keysString += k.ToString() + keySeparator;
                }

                keysString = keysString.Remove(keysString.Length - keySeparatorLength);

                if (TVKeys[0] != HotkeyResetFinishKey)
                    keysString += howToStopText;

                TVKeysLabel.Content = keysString;
            }
            keysString = "";


            if (PanicKeys.Count() == 0)
            {
                PanicKeysLabel.Content = awaitingNewKeyText;
            }
            else
            {
                for (int i = 0; i < PanicKeys.Count; i++)
                {
                    Key k = PanicKeys[i];
                    if (k == HotkeyResetFinishKey)
                        continue;
                    keysString += k.ToString() + keySeparator;
                }

                keysString = keysString.Remove(keysString.Length - keySeparatorLength);

                if (PanicKeys[0] != HotkeyResetFinishKey)
                    keysString += howToStopText;

                PanicKeysLabel.Content = keysString;
            }
            keysString = "";
        }


        /// <summary>
        /// Gets the full Process details of the focused window.
        /// </summary>
        private static Process GetFocusedProcess()
        {
            int? pid = GetFocusedProcessId();
            if (pid.HasValue)
            {
                try
                {
                    return Process.GetProcessById(pid.Value);
                }
                catch (ArgumentException)
                {
                    // The process might have closed right after we got the PID
                    return null;
                }
            }
            return null;
        }


        // 1. Get the handle (HWND) of the currently focused window
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        // 2. Get the Process ID that created that window handle
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        /// <summary>
        /// Gets the Process ID of the currently focused window.
        /// </summary>
        /// <returns>The Process ID, or null if it couldn't be retrieved.</returns>
        private static int? GetFocusedProcessId()
        {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == IntPtr.Zero)
            {
                return null; // No window is currently focused (e.g., switching contexts)
            }

            GetWindowThreadProcessId(hwnd, out uint processId);
            return (int)processId;
        }



        // *************************************************** //
        //                                                     //
        //                   Action functions                  //
        //                                                     //
        // *************************************************** //

        // Standard Win32 ShowWindow constants
        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_RESTORE = 9;

        private bool _hideMenu = true;
        public void MenuFunction()
        {
            // using this to fully hide app icon from taskbar
            var helper = new System.Windows.Interop.WindowInteropHelper(this);
            IntPtr hwnd = helper.Handle;


            if (_hideMenu)
            {
                WindowState = WindowState.Minimized;
                ShowWindow(hwnd, SW_HIDE);
            } 
            else
            {
                ShowWindow(hwnd, SW_SHOWNORMAL);
                this.Activate();
                this.Focus();
            }

            _hideMenu = !_hideMenu;
        }


        Process tvProcess = null;
        private bool _hideWindow = true;
        public void TVFunction()
        {
            if (tvProcess == null) return;
            if (_hideWindow)
            {
                ShowWindow(tvProcess.MainWindowHandle, SW_HIDE);
                NtSuspendProcess(tvProcess.Handle);
            } else
            {
                NtResumeProcess(tvProcess.Handle);
                ShowWindow(tvProcess.MainWindowHandle, SW_RESTORE);
            }

            _hideWindow = !_hideWindow;
        }


        public void PanicFunction()
        {
            if (tvProcess != null)
            {
                ShowWindow(tvProcess.MainWindowHandle, SW_HIDE);
                tvProcess.Kill();
            }
            
            Application.Current.Shutdown();
        }


        // 1. Get the handle (HWND) of the currently focused window
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);


        // 2. Freeze process
        [DllImport("ntdll.dll")]
        private static extern uint NtSuspendProcess(IntPtr processHandle);


        // 3. Resume process
        [DllImport("ntdll.dll")]
        private static extern uint NtResumeProcess(IntPtr processHandle);
    }
}