using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace NHotkey.Wpf
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "This is a singleton; disposing it would break it")]
    public class HotkeyManager : HotkeyManagerBase
    {
        #region Singleton implementation

        public static HotkeyManager Current { get { return LazyInitializer.Instance; } }

        private static class LazyInitializer
        {
            static LazyInitializer() { }
            public static readonly HotkeyManager Instance = new HotkeyManager();
        }

        #endregion

        #region Attached property for KeyBindings

        [AttachedPropertyBrowsableForType(typeof(KeyBinding))]
        public static bool GetRegisterGlobalHotkey(KeyBinding binding)
        {
            return (bool)binding.GetValue(RegisterGlobalHotkeyProperty);
        }

        public static void SetRegisterGlobalHotkey(KeyBinding binding, bool value)
        {
            binding.SetValue(RegisterGlobalHotkeyProperty, value);
        }

        public static readonly DependencyProperty RegisterGlobalHotkeyProperty =
            DependencyProperty.RegisterAttached(
                "RegisterGlobalHotkey",
                typeof(bool),
                typeof(HotkeyManager),
                new PropertyMetadata(
                    false,
                    RegisterGlobalHotkeyPropertyChanged));

        private static void RegisterGlobalHotkeyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var keyBinding = d as KeyBinding;
            if (keyBinding == null)
                return;

            bool oldValue = (bool) e.OldValue;
            bool newValue = (bool) e.NewValue;

            if (DesignerProperties.GetIsInDesignMode(d))
                return;

            if (oldValue && !newValue)
            {
                Current.RemoveKeyBinding(keyBinding);
            }
            else if (newValue && !oldValue)
            {
                Current.AddKeyBinding(keyBinding);
            }
        }

        #endregion

        #region HotkeyAlreadyRegistered event

        public static event EventHandler<HotkeyAlreadyRegisteredEventArgs> HotkeyAlreadyRegistered;

        private static void OnHotkeyAlreadyRegistered(string name)
        {
            var handler = HotkeyAlreadyRegistered;
            if (handler != null)
                handler(null, new HotkeyAlreadyRegisteredEventArgs(name));
        }

        #endregion

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly HwndSource _source;
        private readonly WeakReferenceCollection<KeyBinding> _keyBindings;

        private HotkeyManager()
        {
            _keyBindings = new WeakReferenceCollection<KeyBinding>();

            var parameters = new HwndSourceParameters("Hotkey sink")
                             {
                                 HwndSourceHook = HandleMessage,
                                 ParentWindow = HwndMessage
                             };
            _source = new HwndSource(parameters);
            SetHwnd(_source.Handle);
        }

        public void AddOrReplace(string name, Key key, ModifierKeys modifiers, EventHandler<HotkeyEventArgs> handler)
        {
            AddOrReplace(name, key, modifiers, false, handler);
        }

        public void AddOrReplace(string name, Key key, ModifierKeys modifiers, bool noRepeat, EventHandler<HotkeyEventArgs> handler)
        {
            var flags = GetFlags(modifiers, noRepeat);
            var vk = (uint)KeyInterop.VirtualKeyFromKey(key);
            AddOrReplace(name, vk, flags, handler);
        }

        private static HotkeyFlags GetFlags(ModifierKeys modifiers, bool noRepeat)
        {
            var flags = HotkeyFlags.None;
            if (modifiers.HasFlag(ModifierKeys.Shift))
                flags |= HotkeyFlags.Shift;
            if (modifiers.HasFlag(ModifierKeys.Control))
                flags |= HotkeyFlags.Control;
            if (modifiers.HasFlag(ModifierKeys.Alt))
                flags |= HotkeyFlags.Alt;
            if (modifiers.HasFlag(ModifierKeys.Windows))
                flags |= HotkeyFlags.Windows;
            if (noRepeat)
                flags |= HotkeyFlags.NoRepeat;
            return flags;
        }

        private static ModifierKeys GetModifiers(HotkeyFlags flags)
        {
            var modifiers = ModifierKeys.None;
            if (flags.HasFlag(HotkeyFlags.Shift))
                modifiers |= ModifierKeys.Shift;
            if (flags.HasFlag(HotkeyFlags.Control))
                modifiers |= ModifierKeys.Control;
            if (flags.HasFlag(HotkeyFlags.Alt))
                modifiers |= ModifierKeys.Alt;
            if (flags.HasFlag(HotkeyFlags.Windows))
                modifiers |= ModifierKeys.Windows;
            return modifiers;
        }

        private void AddKeyBinding(KeyBinding keyBinding)
        {
            var gesture = (KeyGesture)keyBinding.Gesture;
            string name = GetNameForKeyBinding(gesture);
            try
            {
                AddOrReplace(name, gesture.Key, gesture.Modifiers, null);
                _keyBindings.Add(keyBinding);
            }
            catch (HotkeyAlreadyRegisteredException)
            {
                OnHotkeyAlreadyRegistered(name);
            }
        }

        private void RemoveKeyBinding(KeyBinding keyBinding)
        {
            var gesture = (KeyGesture)keyBinding.Gesture;
            string name = GetNameForKeyBinding(gesture);
            Remove(name);
            _keyBindings.Remove(keyBinding);
        }

        private readonly KeyGestureConverter _gestureConverter = new KeyGestureConverter();
        private string GetNameForKeyBinding(KeyGesture gesture)
        {
            string name = gesture.DisplayString;
            if (string.IsNullOrEmpty(name))
                name = _gestureConverter.ConvertToString(gesture);
            return name;
        }

        private IntPtr HandleMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            Hotkey hotkey;
            var result = HandleHotkeyMessage(hwnd, msg, wparam, lparam, ref handled, out hotkey);
            if (handled)
                return result;

            if (hotkey != null)
                handled = ExecuteBoundCommand(hotkey);

            return result;
        }

        private bool ExecuteBoundCommand(Hotkey hotkey)
        {
            var key = KeyInterop.KeyFromVirtualKey((int)hotkey.VirtualKey);
            var modifiers = GetModifiers(hotkey.Flags);
            bool handled = false;
            foreach (var binding in _keyBindings)
            {
                if (binding.Key == key && binding.Modifiers == modifiers)
                {
                    handled |= ExecuteCommand(binding);
                }
            }
            return handled;
        }

        private static bool ExecuteCommand(InputBinding binding)
        {
            var command = binding.Command;
            var parameter = binding.CommandParameter;
            var target = binding.CommandTarget;

            if (command == null)
                return false;

            var routedCommand = command as RoutedCommand;
            if (routedCommand != null)
            {
                if (routedCommand.CanExecute(parameter, target))
                {
                    routedCommand.Execute(parameter, target);
                    return true;
                }
            }
            else
            {
                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                    return true;
                }
            }
            return false;
        }
    }
}
