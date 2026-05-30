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
        private static readonly List<Key> PressedKeys = new List<Key>();
        private static SimpleGlobalHook _hook;

        public MainWindow()
        {
            InitializeComponent();


            UpdateKeyLabels();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _hook = new SimpleGlobalHook();

            _hook.KeyPressed += OnKeyPressed;
            _hook.KeyReleased += OnKeyReleased;


            await Task.Run(() => _hook.Run());
        }



        private void OnKeyPressed(object sender, KeyboardHookEventArgs e)
        {
            lock (PressedKeys) {
                Key pressedKey = KeyInterop.KeyFromVirtualKey((int)e.Data.KeyCode);
                if ((int)e.Data.RawCode == 160)
                {
                    pressedKey = Key.LeftShift;
                } 
                else if ((int)e.Data.RawCode == 162)
                {
                    pressedKey = Key.LeftCtrl;
                }
                else if ((int)e.Data.RawCode == 164)
                {
                    pressedKey = Key.LeftAlt;
                }
                PressedKeys.Add(pressedKey);





                // Check in keys alligning
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
                Key releasedKey = KeyInterop.KeyFromVirtualKey((int)e.Data.KeyCode);
                PressedKeys.Remove(releasedKey);
            }
        }









        // Params
        Key HotkeyResetFinishKey => Config.Instance.HotkeyResetFinishKey;
        List<Key> MenuKeys = Config.Instance.MenuKeys;
        List<Key> TVKeys = Config.Instance.TVKeys;
        List<Key> PanicKeys = Config.Instance.PanicKeys;



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

        private void ReloadPressedKeysButton_Click(object sender, RoutedEventArgs e)
        {
            PressedKeys.Clear();
        }

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
        public static int? GetFocusedProcessId()
        {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == IntPtr.Zero)
            {
                return null; // No window is currently focused (e.g., switching contexts)
            }

            GetWindowThreadProcessId(hwnd, out uint processId);
            return (int)processId;
        }

        /// <summary>
        /// Optional: Gets the full Process details of the focused window.
        /// </summary>
        public static Process GetFocusedProcess()
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



        





        public void MenuFunction()
        {
            if (this.WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
                this.Activate();
                // this.Focus();
            } 
            else
            {
                WindowState = WindowState.Minimized;
            }
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

        // Standard Win32 ShowWindow constants
        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_RESTORE = 9;


        Process tvProcess = null;
        private bool HideWindow = true;
        public void TVFunction()
        {
            if (tvProcess == null) return;
            if (HideWindow)
            {
                ShowWindow(tvProcess.MainWindowHandle, SW_HIDE);
                WindowState = WindowState.Minimized;
                NtSuspendProcess(tvProcess.Handle);
            } else
            {
                NtResumeProcess(tvProcess.Handle);
                ShowWindow(tvProcess.MainWindowHandle, SW_RESTORE);
            }

            HideWindow = !HideWindow;
        }


        public void PanicFunction()
        {
            if (tvProcess != null)
            {
                ShowWindow(tvProcess.MainWindowHandle, SW_HIDE);
                tvProcess.Kill();
            }
            
            WindowState = WindowState.Minimized;
            Application.Current.Shutdown();
        }
    }
}