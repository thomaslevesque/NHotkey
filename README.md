NHotkey
=======

Easily handle shortcut keys even when your app doesn't have focus!

### Windows Forms usage

Add a reference to `NHotkey.dll` and `NHotkey.WindowsForms.dll`.

In your `Form`, declare a field of type `HotkeyManager`:

```csharp
    private readonly HotkeyManager _hotkeyManager;
```

In the constructor of your `Form`, initialize `_hotkeyManager` to a new instance of
`HotkeyManager`, passing a reference to the form to the constructor:

```csharp
    _hotkeyManager = new HotkeyManager(this);
```

Add some hotkeys:

```csharp
    _hotkeyManager.Add("Increment", Keys.Control | Keys.Alt | Keys.Add, OnIncrement);
    _hotkeyManager.Add("Decrement", Keys.Control | Keys.Alt | Keys.Subtract, OnDecrement);
```

- the first parameter is an application-defined name for the hotkey; it can be anything you like,
as long as it's unique;
- the second parameter is the combination of keys for which you want to register a hotkey;
- the last parameter is a delegate of type `EventHandler<HotkeyEventArgs>` that will be called
when this hotkey is pressed. For instance:

```csharp
    private void OnIncrement(object sender, HotkeyEventArgs e)
    {
        Value++;
        e.Handled = true;
    }

    private void OnDecrement(object sender, HotkeyEventArgs e)
    {
        Value--;
        e.Handled = true;
    }
```

If you want to handle several hotkeys with the same handler, you can check the `Name`
property of the `HotkeyEventArgs`:

```csharp
    private void OnIncrementOrDecrement(object sender, HotkeyEventArgs e)
    {
        switch (e.Name)
        {
            case "Increment":
                Value++;
                break;
            case "DEcrement":
                Value--;
                break;
        }
        e.Handled = true;
    }
```

### WPF usage

The approach for WPF is very similar to the one for Windows Forms, with a few minor differences.
The WPF version also supports `InputBindings`.

Add a reference to `NHotkey.dll` and `NHotkey.Wpf.dll`.

In your `Window`, declare a field of type `HotkeyManager`:

```csharp
    private readonly HotkeyManager _hotkeyManager;
```

In the constructor of your `Window`, initialize `_hotkeyManager` to a new instance of
`HotkeyManager`, passing a reference to the window to the constructor:

```csharp
    _hotkeyManager = new HotkeyManager(this);
```

Add some hotkeys:

```csharp
    _hotkeyManager.Add("Increment", Key.Add, ModifierKeys.Control | ModifierKeys.Alt, OnIncrement);
    _hotkeyManager.Add("Decrement", Key.Subtract, ModifierKeys.Control | ModifierKeys.Alt, OnDecrement);
```

- the first parameter is an application-defined name for the hotkey; it can be anything you like,
as long as it's unique;
- the second and third parameters are the key and modifiers for which you want to register a hotkey;
- the last parameter is a delegate of type `EventHandler<HotkeyEventArgs>` that will be called
when this hotkey is pressed.

To support applications that use the MVVM pattern, you can also specify hotkeys in XAML using
`InputBindings`. Just declare `KeyBindings` as usual, and set the `HotkeyManager.RegisterGlobalHotkey`
attached property to `true`:

```xml
    ...
    <Window.InputBindings>
        <KeyBinding Gesture="Ctrl+Alt+Add" Command="{Binding IncrementCommand}"
                    HotkeyManager.RegisterGlobalHotkey="True" />
        <KeyBinding Gesture="Ctrl+Alt+Subtract" Command="{Binding DecrementCommand}"
                    HotkeyManager.RegisterGlobalHotkey="True" />
    </Window.InputBindings>
    ...
```

**Remarks**

- if you use the XAML approach, you still need to create a `HotkeyManager` for the window, or the `KeyBindings` won't
work when your app doesn't have focus.
- `KeyBindings` that are added *after* the window is shown won't be taken into account.