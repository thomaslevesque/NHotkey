using System.Windows.Markup;

// Mapping a custom namespace to the standard WPF namespace is usually something to avoid,
// however in this case we're only importing one type (HotkeyManager), and it's unlikely to
// collide with another type in a future version of WPF. So in this case, we do it for the
// sake of simplicity, so that the user doesn't need to map the namespace manually.
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "NHotkey.Wpf")]
