# .NET notes: what's installed, why, and moving apps to .NET 10

Written 2026-10-07 after a software audit of this machine. Plain-language reference;
the hands-on version is the lesson in
`learning/dotnet-winforms-to-net10/LESSON.md`.

## Old .NET vs new .NET

- **.NET Framework** (latest and last: 4.8) is built into Windows. Frozen: it is still
  patched through Windows but gets no new features. There is no 4.9.
- **.NET** (6, 8, 9, 10...) is installed separately and several versions sit side by
  side. An app built for one major version only runs on that major version
  (6 runs on 6.x, not on 8 or 10) unless it explicitly allows rolling forward.
- A window or error showing `4.8.9xxx` is a build number of .NET Framework, not a
  higher version.

## What is installed here and why

| Installed | Needed by | Verdict |
|---|---|---|
| .NET 6 (runtime + Windows Desktop) | **Sims 4 Studio** (`S4Studio.runtimeconfig.json` asks for 6.0.0, no roll-forward) | **Keep.** It is end-of-life, so watch for a Sims 4 Studio update that moves to a newer .NET |
| .NET 9 | Pinned earlier on purpose (Winhance targets 9) | Keep |
| .NET 8 | Most likely pulled in by Visual Studio workloads (not traced) | Leave |
| .NET 10 SDK + runtimes | Current. Used for new work | Keep |
| PowerShell 7, Proton VPN, ShareX | Carry their own copy of .NET (self-contained) | Use none of the shared runtimes |

How to see what an app needs: open its `<name>.runtimeconfig.json`.
`"frameworks"` means it uses the shared runtime installed on the machine.
`"includedFrameworks"` means it brings its own.

## The MAUI / Android / iOS / Emscripten entries

About 45 entries in Programs and Features (MAUI SDK and templates, Android/iOS/Apple
manifests, Emscripten and Mono toolchain manifests) came with the .NET SDK. They total
roughly 0.4 MB of manifests. `dotnet workload list` is empty, so no workload is
actually installed.

Decision (2026-10-07): **leave them.** A plain uninstall (`msiexec /x ... /qn`) reports
success and logs "removal completed" but removes nothing, because a WiX dependency check
stops it when other installed products depend on the package. Forcing it
(`IGNOREDEPENDENCIES=ALL`) would leave the SDK installer with missing pieces.

## Moving an existing app to .NET 10: what usually happens

The lesson tested a small Windows Forms app. What it showed:

1. Change `net48` to `net10.0-windows` in the `.csproj`.
2. The startup lines can be replaced with `ApplicationConfiguration.Initialize()`.
3. `App.config` is still read the same way, but the copy next to the program is renamed
   from `.exe.config` to `.dll.config`.
4. On a **desktop** (Windows Forms / WPF) app, `System.Configuration` comes with the
   runtime. Adding the NuGet package triggers warning `NU1510`. A console app or a
   library would need the package.

## What can be upgraded on this machine

Only code **you own**. Everything installed here is someone else's (Sims 4 Studio,
ShareX, RimSort...). Your own projects are Python and JavaScript, which aren't .NET. So
for now the lesson app is the practice target. A real candidate would be any small
Windows tool you write or find with source code.

## MAUI and WinUI: questions to revisit later

- **WinUI 3**: Windows only, modern look, built on the Windows App SDK.
- **.NET MAUI**: one codebase for Windows, Android, iOS and macOS. Needs
  `dotnet workload install maui`.
- Both are a rewrite of the screens, not just a project move. Moving to .NET 10 comes
  first and is usually all a Windows desktop app needs.
- To explore: when is it worth leaving Windows Forms or WPF at all?
