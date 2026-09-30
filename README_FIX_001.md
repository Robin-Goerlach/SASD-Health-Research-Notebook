# Fix 001 – WPF Application namespace collision

## Problem

`dotnet build` failed in the WPF project with:

```text
App.xaml.cs(8,28): error CS0118: 'Application' is a namespace but is used like a type
```

## Cause

The solution has an application-layer namespace:

```text
Sasd.HealthNotebook.Application
```

In the WPF startup class, the code originally used:

```csharp
using System.Windows;
public partial class App : Application
```

The compiler resolved `Application` as part of the project namespace instead of the WPF type `System.Windows.Application`.

## Fix

Replace:

```text
src/Sasd.HealthNotebook.Wpf/App.xaml.cs
```

with the file included in this ZIP.

The corrected class explicitly inherits from:

```csharp
System.Windows.Application
```

## Commands after applying the fix

```powershell
dotnet clean
dotnet restore
dotnet build
```

## Commit suggestion

```text
fix: resolve WPF application namespace collision
```
