# Lesson: move a small Windows Forms app from .NET Framework 4.8 to .NET 10

**Time:** about 30-45 minutes. **You'll end up with:** the same little app running on
the modern .NET, and a clear picture of what actually changes when you "upgrade".

This is a side track. It is separate from the userscript lessons in `../README.md`.

## The idea in plain language

Windows ships with an old, frozen version of .NET called **.NET Framework** (the last
one is 4.8). It is built into Windows and gets no new features. The modern version is
just called **.NET** (you have 10 installed). Apps written for the old one don't
automatically run on the new one. "Upgrading" means changing a few lines so the app
targets the new one and fixing whatever no longer matches.

The good news: for a small Windows Forms app, that is a **small** job. The screen code
(buttons, labels, forms) mostly stays exactly the same.

The app, **Click Counter**, is deliberately tiny but touches the three things that
usually need attention during a move:

1. **The project file** (`.csproj`), which says which .NET it targets.
2. **The startup code** (`Program.cs`), which changed shape in newer .NET.
3. **Configuration** (`App.config`), a classic .NET Framework feature.

It also shows a line on screen saying which .NET it is running on, so you can *see*
the change instead of taking it on trust.

## What was checked (and how)

Every command below was run on this machine (Windows 11, .NET SDK 10.0.401) before it
was written down. The app was built and launched on both versions and the window's
text was read back:

| Build | Window said |
|---|---|
| .NET Framework 4.8 | `Running on: .NET Framework 4.8.9345.0` (the number after 4.8 is a build number, there is no "4.9") |
| .NET 10 | `Running on: .NET 10.0.12` |

Both showed the greeting from `App.config` and counted clicks correctly. Things I did
**not** test are marked *(not tested)*.

## Before you start

Check these in PowerShell:

```powershell
dotnet --list-sdks                  # should list a 10.0.x line
dotnet --list-runtimes | Select-String WindowsDesktop   # should list a 10.0.x line
Test-Path 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8'   # True
```

The last one is the ".NET Framework 4.8 targeting pack". It lets the new `dotnet`
command build the *old* version of the app. If it says `False`, install the
".NET Framework 4.8 Developer Pack" from Microsoft and try again.

## Step 0: make your own copy

Never edit `starter/`. It stays clean so you can start over any time.

```powershell
cd C:\tools\traylespud\learning\dotnet-winforms-to-net10
Copy-Item starter work -Recurse
```

All the steps below happen inside `work/`.

## Step 1: build and run the OLD app

```powershell
dotnet build work
work\bin\Debug\net48\ClickCounter.exe
```

You should see a small window. Note three things:

- The first line says `(greeting not loaded yet)`. That is the TODO in Step 2.
- The second line says `Running on: .NET Framework 4.8...`.
- The button counts clicks.

Close the window. Look at the folder `work\bin\Debug\net48\`. Notice
`ClickCounter.exe.config`. That is your `App.config`, copied next to the program.

## Step 2: your task, finish `GetGreeting()` (still on .NET Framework)

Open `work\MainForm.cs` and find the `TODO` comment inside `GetGreeting()`.

Make `GetGreeting()` return the `Greeting` setting from `App.config`. The classic way is
one line:

```csharp
ConfigurationManager.AppSettings["Greeting"]
```

The decision that is yours: **what should happen when the setting is missing?**
`AppSettings["Greeting"]` returns `null` if the key isn't there (typo, deleted line,
file not copied). Options include a friendly default, an obvious "CONFIG MISSING"
text, or throwing an error so the problem can't hide. Pick one and be ready to say why.

Rebuild and run (`dotnet build work`, then run the exe). The first line should now show
`Hello from the config file!`. Then try deleting the `Greeting` line from `App.config`,
rebuild, and check your fallback works. Put the line back afterwards.

## Step 3: the move to .NET 10

Do these in order, and rebuild at the end. Each one is small.

### 3a. Change the target

Open `work\ClickCounter.csproj`. Change

```xml
<TargetFramework>net48</TargetFramework>
```

to

```xml
<TargetFramework>net10.0-windows</TargetFramework>
```

`-windows` matters: Windows Forms only exists on Windows, so the project says so.

### 3b. Delete the System.Configuration reference

Delete the whole block:

```xml
<ItemGroup>
  <!-- .NET Framework ships System.Configuration "in the box". .NET 10 does not. -->
  <Reference Include="System.Configuration" />
</ItemGroup>
```

You might expect to replace it with the NuGet package
`System.Configuration.ConfigurationManager`, because on many .NET project types you must.
**Not here.** A Windows Forms or WPF app on .NET 10 already gets that library from the
Windows Desktop runtime (checked: it is inside
`C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\10.0.12\`). If you add the
package anyway, the build works but prints warning `NU1510` telling you it is
unnecessary. A console app or a class library would need the package.

> The comment in the starter file says ".NET 10 does not" ship it. For a plain .NET
> runtime that is true, and for the *desktop* runtime it is not. Good example of why you
> test instead of assuming.

### 3c. Update the startup code

Open `work\Program.cs`. Replace these two lines

```csharp
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
```

with one:

```csharp
ApplicationConfiguration.Initialize();
```

Keep `Application.Run(new MainForm());`. The new line does the setup work from
settings in your `.csproj` (visual styles, text rendering, default font and high-DPI
mode). You haven't set any, so you get the .NET 10 defaults. *(The old two lines would
still compile on .NET 10, so this step is a modernisation rather than a fix.)*

### 3d. Build and run

```powershell
dotnet build work
work\bin\Debug\net10.0-windows\ClickCounter.exe
```

The second line should now read `Running on: .NET 10.0.x`. The greeting and the click
counter should behave exactly as before.

Look at the output folder again: `work\bin\Debug\net10.0-windows\`. The config file is now
called `ClickCounter.dll.config` (it was `ClickCounter.exe.config`). Same file, new
name. That is a classic gotcha if anything else in your setup looks for the old name.

## Step 4: what changed and why

| Thing | .NET Framework 4.8 | .NET 10 |
|---|---|---|
| Where the runtime lives | Built into Windows | Installed separately, side by side (that's why you see 6, 8, 9 and 10 in your installed list) |
| Project says | `net48` | `net10.0-windows` |
| Startup code | 2 setup lines + `Run` | `ApplicationConfiguration.Initialize()` + `Run` |
| Config file next to the program | `ClickCounter.exe.config` | `ClickCounter.dll.config` |
| Reading config | `System.Configuration` reference | Same code, library comes with the desktop runtime |
| Build output folder | `bin\Debug\net48` | `bin\Debug\net10.0-windows` |
| Language version | C# 7.3 by default | A much newer C# by default |

Also worth a look: run the old and new builds side by side. The default font and how
sharp the text is on a high-resolution screen usually differ between the two. *(Not
measured here, so check it yourself.)*

## Step 5 (optional): small modernisations *(not tested)*

Once it runs, you can add to the `.csproj` `<PropertyGroup>`:

```xml
<ImplicitUsings>enable</ImplicitUsings>
<Nullable>enable</Nullable>
```

The compiler will then suggest fixes (like handling `null` from the config lookup, which
connects straight back to your Step 2 decision). Do this one change at a time and
rebuild between.

## Step 6 (optional): hand it to someone *(not tested)*

```powershell
dotnet publish work -c Release
```

By default that makes a version that needs the **.NET 10 Desktop Runtime** on the other
computer. Look up the `--self-contained` option if you want one that carries its own
runtime (bigger, no install needed).

## What about WPF? *(not built in this lesson)*

The move is the same shape. Instead of `<UseWindowsForms>true</UseWindowsForms>` the
project has `<UseWPF>true</UseWPF>`, and the screen is described in XAML files. The
`net48` to `net10.0-windows` change and the "does it still build" loop are the same.

## Where MAUI and WinUI fit *(next lessons, not covered here)*

Moving to .NET 10 is **step one** and is usually all you need. The user interface
technologies are a separate, bigger decision made afterwards:

- **WinUI 3** is the modern, Windows-only UI.
- **.NET MAUI** is for one app that runs on Windows, Android, iOS and macOS.

Both mean rewriting the screens, not just moving the project. On your machine
`dotnet workload list` is empty, so MAUI would need `dotnet workload install maui`
first.

## Check yourself

1. What does the `-windows` part of `net10.0-windows` do?
2. Why did the config file get a different name after the move?
3. The starter's comment said .NET 10 doesn't ship `System.Configuration`. What did you
   find, and what would change if this were a console app instead?
4. If `Greeting` were missing, what does your `GetGreeting()` do, and why that choice?
5. Why can the old two startup lines still compile on .NET 10 even though the new one
   exists?

## If something goes wrong

- **"Cannot find reference assemblies for .NETFramework v4.8"** when building Step 1: the
  4.8 targeting pack is missing (see *Before you start*).
- **Start over:** delete `work/` and redo Step 0.
- **A window opens and closes instantly:** run the exe from PowerShell so you can see
  any error text, and check that `ClickCounter.dll.config` exists next to it.
