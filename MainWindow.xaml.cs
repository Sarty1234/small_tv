using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Threading;
using SharpHook.Data;
using SharpHook;
using System.Threading.Tasks;
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
            string keySeparator = " + ";
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
                for (int i = MenuKeys.Count - 1; i >= 0; i--)
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
                for (int i = TVKeys.Count - 1; i >= 0; i--)
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
                for (int i = PanicKeys.Count - 1; i >= 0; i--)
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


        private void SelectWindowButton_Click(object sender, RoutedEventArgs e)
        {

        }








        int b = 0;
        public void MenuFunction()
        {
            b += 1;
            MenuKeysLabel.Content = b;

            //MessageBox.Show("hi");
        }


        public void TVFunction()
        {

        }


        public void PanicFunction()
        {

        }
    }
}