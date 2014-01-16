using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace NHotkey.Wpf
{
    public class WpfHotkeyManager : HotkeyManager
    {
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
            DependencyProperty.RegisterAttached("RegisterGlobalHotkey", typeof(bool), typeof(WpfHotkeyManager), new PropertyMetadata(false));

        private readonly Window _window;

        public WpfHotkeyManager(Window window)
        {
            _window = window;
            _window.SourceInitialized += WindowSourceInitialized;
            _window.Closed += WindowClosed;
        }

        public void Add(string name, Key key, ModifierKeys modifiers, EventHandler<HotkeyEventArgs> handler)
        {
            Add(name, key, modifiers, false, handler);
        }

        public void Add(string name, Key key, ModifierKeys modifiers, bool noRepeat, EventHandler<HotkeyEventArgs> handler)
        {
            var flags = GetFlags(modifiers, noRepeat);
            var vk = (uint)KeyInterop.VirtualKeyFromKey(key);
            Add(name, vk, flags, handler);
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

        private HwndSource _source;
        
        void WindowSourceInitialized(object sender, EventArgs e)
        {
            _source = (HwndSource) PresentationSource.FromVisual(_window);
            if (_source == null)
                throw new InvalidOperationException("Source is not initialized");
            _source.AddHook(HandleMessage);
            RegisterInputBindings();
            Register(_source.Handle);
        }

        void WindowClosed(object sender, EventArgs e)
        {
            _source.RemoveHook(HandleMessage);
            Unregister();
            UnregisterInputBindings();
        }

        private void RegisterInputBindings()
        {
            var converter = new KeyGestureConverter();
            foreach (var binding in _window.InputBindings.OfType<KeyBinding>())
            {
                if (!GetRegisterGlobalHotkey(binding))
                    return;
                var gesture = (KeyGesture)binding.Gesture;
                string name = gesture.DisplayString;
                if (string.IsNullOrEmpty(name))
                    name = converter.ConvertToString(gesture);
                Add(name, gesture.Key, gesture.Modifiers, null);
            }
        }

        private void UnregisterInputBindings()
        {
            var converter = new KeyGestureConverter();
            foreach (var binding in _window.InputBindings.OfType<KeyBinding>())
            {
                if (!GetRegisterGlobalHotkey(binding))
                    return;
                var gesture = (KeyGesture)binding.Gesture;
                string name = gesture.DisplayString;
                if (string.IsNullOrEmpty(name))
                    name = converter.ConvertToString(gesture);
                Remove(name);
            }
        }

        private IntPtr HandleMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            Hotkey hotkey;
            var result = HandleHotkeyMessage(hwnd, msg, wparam, lparam, ref handled, out hotkey);
            if (handled)
                return result;

            if (hotkey != null)
            {
                ExecuteBoundCommand(hotkey);
            }
            return result;
        }

        private void ExecuteBoundCommand(Hotkey hotkey)
        {
            var key = KeyInterop.KeyFromVirtualKey((int) hotkey.VirtualKey);
            var modifiers = GetModifiers(hotkey.Flags);
            foreach (var binding in _window.InputBindings.OfType<KeyBinding>())
            {
                if (binding.Key == key && binding.Modifiers == modifiers)
                {
                    ExecuteCommand(binding);
                }
            }
        }

        private static void ExecuteCommand(InputBinding binding)
        {
            var command = binding.Command;
            var parameter = binding.CommandParameter;
            var target = binding.CommandTarget;
            
            if (command == null)
                return;
            
            var routedCommand = command as RoutedCommand;
            if (routedCommand != null)
            {
                if (routedCommand.CanExecute(parameter, target))
                    routedCommand.Execute(parameter, target);
            }
            else
            {
                if (command.CanExecute(parameter))
                    command.Execute(parameter);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _window.SourceInitialized -= WindowSourceInitialized;
            _window.Closed -= WindowClosed;
        }
    }
}
