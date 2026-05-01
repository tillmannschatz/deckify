# Deployment Guide

How to release **deckify** — the VSTO PowerPoint Add-in.

## Repo split

Two repos cooperate:

| Repo | Role |
|---|---|
| [`tillmannschatz/PPTXML`](https://github.com/tillmannschatz/PPTXML) | Source code, branches, PRs, CI workflow |
| [`tillmannschatz/deckify`](https://github.com/tillmannschatz/deckify) | Distribution: GitHub Pages (ClickOnce) + Releases (asset mirror) |

Pages serves [`https://tillmannschatz.github.io/deckify/`](https://tillmannschatz.github.io/deckify/) from the `gh-pages` branch in `deckify`. ClickOnce auto-update reads this URL.

## Releasing — recommended path

Run [`/publish`](.claude/commands/publish.md) from a Claude Code session in this repo:

```
/publish patch   # 0.1.7 → 0.1.8
/publish minor   # 0.1.7 → 0.2.0
/publish major   # 0.1.7 → 1.0.0
/publish 0.5.0   # explicit
```

The slash command:
1. Reads the latest tag from `tillmannschatz/deckify` releases.
2. Computes the new version, asks for confirmation.
3. Patches `<ApplicationVersion>` in [src/Deckify/Deckify.csproj](src/Deckify/Deckify.csproj), commits, pushes to `development`.
4. Triggers [.github/workflows/publish.yml](.github/workflows/publish.yml) via `gh workflow run`.
5. Watches the run and reports success/failure.

The workflow on `windows-latest`:
1. Checks out `development` from PPTXML.
2. Restores the PFX cert from secret `PFX_BASE64`.
3. Restores NuGet packages, runs `dotnet test`.
4. `msbuild /t:Publish` with the new version → `publish/` folder.
5. Packs `publish/*` into `deckify-<version>.7z` (via 7-Zip).
6. Pushes `publish/` to `tillmannschatz/deckify` `gh-pages` branch (cross-repo, uses `DECKIFY_PAT`).
7. Creates GitHub Release `v<version>` in `tillmannschatz/deckify` with the 7z, `setup.exe`, `Deckify.vsto` as assets.
8. Cleans up the PFX file.

## One-time setup (already done — listed for reference)

In [`tillmannschatz/PPTXML`](https://github.com/tillmannschatz/PPTXML/settings/secrets/actions):

- Secret `PFX_BASE64` — base64-encoded contents of `src/Deckify/effensify.pfx` (the gitignored code-signing cert).
  ```powershell
  [Convert]::ToBase64String([IO.File]::ReadAllBytes("src\Deckify\effensify.pfx")) | Set-Clipboard
  ```
- Secret `DECKIFY_PAT` — Personal Access Token (classic) with scopes `repo` + `workflow`. Generated at <https://github.com/settings/tokens>. Used by the workflow to push to deckify's `gh-pages` and create releases there.

In `tillmannschatz/PPTXML` Settings → Actions → General:
- Workflow permissions: **Read repository contents and packages permissions** (default is fine; cross-repo writes go through `DECKIFY_PAT`).

In [`tillmannschatz/deckify`](https://github.com/tillmannschatz/deckify/settings/pages):
- Pages: enabled, source `gh-pages` branch, root path. Verified: `https://tillmannschatz.github.io/deckify/`.

## End-user install

**Recommended:** download `deckify-installer.exe` from the [latest release](https://github.com/tillmannschatz/deckify/releases/latest) and double-click. No admin rights required. The installer bundles the full ClickOnce payload, so the initial install works offline. When the standard ClickOnce trust prompt ("Application Install — Security Warning, Publisher cannot be verified") appears, click **Install**. Subsequent auto-updates from gh-pages run silently (same cert thumbprint = no further prompt).

**Alternative:** ClickOnce direct from <https://tillmannschatz.github.io/deckify/setup.exe>. Same trust prompt, but downloads the payload from Pages instead of from the bundled installer.

After install, the add-in registers under `HKCU\Software\Microsoft\Office\PowerPoint\Addins\Deckify` and checks `https://tillmannschatz.github.io/deckify/` every 7 days for updates (configurable in csproj `<UpdateInterval>`).

### Manual install (corporate / power-user)

If `deckify-installer.exe` is blocked by domain policy or AV, or you want to suppress the "Publisher cannot be verified" warning, do it via PowerShell. Still no admin needed:

```powershell
# Step 1 (optional): import the public cert into your user-store TrustedPublisher.
# Removes the yellow warning icon on the ClickOnce trust prompt.
$cer = "$env:TEMP\deckify-publisher.cer"
Invoke-WebRequest "https://tillmannschatz.github.io/deckify/deckify-publisher.cer" -OutFile $cer
Import-Certificate -FilePath $cer -CertStoreLocation Cert:\CurrentUser\TrustedPublisher

# Step 2: launch the ClickOnce bootstrapper.
Start-Process "https://tillmannschatz.github.io/deckify/setup.exe"
```

Updates after install come from gh-pages automatically.

## Manual fallback (legacy VS Publish Wizard)

Still works if you need to publish without CI — useful for a hot-fix when CI is broken.

1. Open `Deckify.sln` in Visual Studio 2022 (Office/SharePoint workload).
2. Right-click **Deckify** project → **Publish…**.
3. Wizard:
   - Publish location: a local folder, e.g. `C:\temp\deckify-publish\`.
   - Installation Folder URL: `https://tillmannschatz.github.io/deckify/`.
   - Prerequisites: .NET 4.8.1 + VSTO Runtime (auto-selected).
4. Output: `setup.exe`, `Deckify.vsto`, `Application Files/Deckify_<version>/`.
5. Mirror to `gh-pages` branch in deckify:
   ```bash
   git clone https://github.com/tillmannschatz/deckify.git -b gh-pages ../deckify-pages
   robocopy C:\temp\deckify-publish\ ..\deckify-pages\ /MIR /XD .git
   cd ../deckify-pages
   git add -A
   git commit -m "publish v<version>"
   git push
   ```
6. Optionally: `gh release create v<version> ...` in deckify with the assets.
