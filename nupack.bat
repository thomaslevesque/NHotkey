@echo off
set packageDir=NuGetPackages
nuget pack -OutputDirectory %packageDir% %packageDir%\NHotkey.nuspec
nuget pack -OutputDirectory %packageDir% %packageDir%\NHotkey.Wpf.nuspec
nuget pack -OutputDirectory %packageDir% %packageDir%\NHotkey.WindowsForms.nuspec
