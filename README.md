# Roblox FastFlag Injector

A Windows desktop tool (WinForms, .NET 8) for editing Roblox's client-side
FastFlags via `ClientAppSettings.json`. This is a config file editor — it does
not modify game memory, inject into processes, or touch anticheat. Roblox
reads this JSON on launch and applies any flags it recognizes.

## What it does

- Scans `%LOCALAPPDATA%\Roblox\Versions\` for installed Roblox versions
  (Player and/or Studio)
- Lets you view, add, edit, and remove flags in a grid
- Supports Boolean / Integer / String flag values
- Includes a short list of publicly known performance/graphics flags as a
  starting point (you can add any flag name manually)
- Automatically backs up the existing `ClientAppSettings.json` (timestamped)
  before overwriting
- Can apply the same flag set to a single install or all detected installs
- Import a flag set from any `.json` file (a flat object of `"FlagName": value`
  pairs, same format as `ClientAppSettings.json` itself) — choose to merge
  with the current list or replace it entirely

## Ban risk — read this

Since **September 29, 2025**, Roblox introduced a server-side **Fast Flag
Allowlist**. Only flag names on that official list are actually applied by
the client — anything else you set locally is simply ignored. From Roblox's
own FAQ on the change:

> "Will I face consequences if I attempt to use Fast Flags that are not on
> the Fast Flag Allowlist? **No.** Fast Flags not on the Allowlist will
> simply be ignored. No further action will be taken against a user
> attempting to use a restricted Fast Flag."
> — https://devforum.roblox.com/t/allowlist-for-local-client-configuration-via-fast-flags/3966569

This tool only edits `ClientAppSettings.json` — the same file Roblox's own
client reads for local overrides. It does not inject code into the Roblox
process, patch the game binary, or use a third-party executor/bootstrapper.
That distinction matters: Roblox's automated enforcement has targeted
*modified clients* (DLL injection, memory patching, cheat executors), not
plain JSON config edits.

That said, no one can promise zero risk on a live platform whose policies
and enforcement can change at any time — this is a snapshot of the current,
official policy, not a permanent guarantee. To stay in the lowest-risk
lane:
- Only rely on flags that are actually on the current allowlist (the
  built-in presets are pulled from it, but re-check the devforum thread
  above since Roblox updates it without notice)
- Don't combine this with executors, bootstrappers, or anything that goes
  beyond editing this one config file
- Setting a non-allowlisted flag won't get you punished per the FAQ above —
  it'll just do nothing



- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download) (needed to build; not
  needed to just run the published .exe)

## Build & run

From this folder, in a terminal:

```powershell
dotnet build
dotnet run
```

## Publish as a standalone .exe

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The output .exe will be under
`bin\Release\net8.0-windows\win-x64\publish\RobloxFlagInjector.exe`.
You can copy just that file to run it on another Windows machine without
installing the .NET SDK there.

## Or open in Visual Studio

Double-click `RobloxFlagInjector.csproj` in Visual Studio 2022+ (with the
".NET desktop development" workload installed), then press F5.

## Notes

- Flag names are case-sensitive and must match exactly what Roblox's client
  looks for; unrecognized names are simply ignored by the client, they won't
  crash anything.
- Close Roblox completely before applying changes, and restart it afterward
  for the new flags to take effect.
- Modifying client flags is a widely used practice in the Roblox community
  for things like FPS caps and rendering backend selection, but it's still a
  modification of the official client. Use flags you understand, and be aware
  some flags (especially undocumented/internal ones) can cause instability.
- Backups are saved next to the original file as
  `ClientAppSettings.json.backup_YYYYMMDD_HHMMSS` — delete the current file
  and rename a backup back to restore.

## Project structure

- `Program.cs` — entry point
- `MainForm.cs` — the UI (built in code, no designer file)
- `RobloxLocator.cs` — finds Roblox install folders, handles backups
- `FlagFile.cs` — JSON load/save for `ClientAppSettings.json`
- `FlagPresets.cs` — small curated list of known flags for the picker

## Getting a downloadable .exe via GitHub (recommended for sharing)

This repo includes `.github/workflows/build.yml`, which builds a real
Windows `.exe` on GitHub's own Windows runners — no local Windows machine
needed on your end just to produce a release.

1. Push this project to a public GitHub repo
2. Tag a release: `git tag v1.0 && git push origin v1.0`
3. GitHub Actions builds it automatically and attaches
   `RobloxFlagInjector-win-x64.zip` (containing the self-contained
   `RobloxFlagInjector.exe`) to a Release page
4. Share that Release page link — anyone can download the zip, extract it,
   and run the exe with no .NET install required

You can also trigger a build manually anytime from the Actions tab
("Run workflow") without needing a new tag.
