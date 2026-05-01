; Inno Setup launcher for deckify.
;
; Bundles the full ClickOnce publish/ folder into the installer EXE. On run:
;   1) cleans up stale ClickOnce subscriptions, old user-store cert entries,
;      and any prior bundle directory from earlier installer iterations
;   2) extracts the bundled payload to %LocalAppData%\deckify-installer-bundle
;      (persistent location — see [Files] comment for why not {tmp})
;   3) launches the local setup.exe (ClickOnce bootstrapper)
;   4) user clicks "Install" on the standard ClickOnce trust prompt
;      ("Publisher cannot be verified" — yellow warning icon)
;
; No certificate import is performed: the self-signed cert can't reach trusted
; chain status from a non-elevated context (Machine\Root needs UAC; user-store
; Root import shows a scary unsuppressable Windows warning dialog). The
; ClickOnce trust prompt is the Microsoft-supported flow for unverified
; publishers and works without admin.
;
; Auto-updates do NOT work via this installer. VSTOInstaller registers
; HKCU\Software\Microsoft\Office\PowerPoint\Addins\Deckify\Manifest with a
; file:// URL pointing at the bundle directory, so ClickOnce has no remote
; channel to poll. Updates are user-driven: the in-app About panel's
; "Update" button opens the latest installer.exe download URL in the browser
; and the user re-runs it.
;
; Compiled in CI via:
;   ISCC.exe /Q /O<outdir> /DAppVersion=0.1.x /DPublishDir=...\publish tools\deckify-installer.iss

#ifndef AppVersion
  #define AppVersion "0.0.0"
#endif

#ifndef PublishDir
  #define PublishDir "publish"
#endif

[Setup]
; Stable GUID — keeps installer identity consistent across versions even
; though we don't actually register an uninstall entry (see Uninstallable=no).
AppId={{A8F3B2D1-6E4C-4B7A-9F8D-D3CC1FE15E6E}
AppName=deckify
AppVersion={#AppVersion}
AppPublisher=Tillmann Schatz
AppPublisherURL=https://github.com/tillmannschatz/deckify

; We don't register an {app} install dir — the bundled ClickOnce payload
; goes to %LocalAppData%\deckify-installer-bundle (see [Files]).
CreateAppDir=no
DisableDirPage=yes
DisableProgramGroupPage=yes
DisableReadyPage=yes
DisableWelcomePage=yes
DisableFinishedPage=yes

; ClickOnce manages uninstall via Apps & Features.
Uninstallable=no

OutputBaseFilename=deckify-installer
OutputDir=.
PrivilegesRequired=lowest
WizardStyle=modern

[Files]
; Bundle the full ClickOnce publish/ output. Extract to a PERSISTENT location
; in %LocalAppData% rather than {tmp}: setup.exe forks VSTOInstaller.exe as a
; subprocess and returns BEFORE VSTOInstaller finishes copying files into the
; ClickOnce cache. If we used {tmp}, Inno would auto-clean it on exit and
; VSTOInstaller would fail with "Could not find file ...\Deckify.vsto".
; The bundle (~3 MB) stays on disk after install — harmless, refreshed on
; subsequent installer runs (see CleanupOldBundle).
Source: "{#PublishDir}\*"; DestDir: "{localappdata}\deckify-installer-bundle"; Flags: recursesubdirs createallsubdirs

[Code]
procedure CleanupOldClickOnceSubscription();
var
  RC: Integer;
begin
  // Soft cleanup: rundll32 dfshim.dll,CleanOnlineAppCache wipes ALL
  // ClickOnce apps of the current user. Acceptable here — deckify is the
  // only relevant ClickOnce app for this user base, and stale subscriptions
  // from approaches 1-5 otherwise trigger "another version installed".
  // ResultCode ignored (0 if cleared, 1 if nothing to clear — both fine).
  Exec(ExpandConstant('{sys}\rundll32.exe'),
       'dfshim.dll,CleanOnlineAppCache',
       '', SW_HIDE, ewWaitUntilTerminated, RC);
end;

procedure CleanupOldUserCert();
var
  RC: Integer;
begin
  // Best-effort: remove old user-store cert entries from approach 5.
  // No dialog, no admin, ResultCode ignored.
  Exec(ExpandConstant('{sys}\certutil.exe'),
       '-user -delstore TrustedPublisher "Tillmann Schatz"',
       '', SW_HIDE, ewWaitUntilTerminated, RC);
end;

procedure CleanupOldBundle();
begin
  // Wipe any prior extracted bundle so [Files] starts clean. DelTree args:
  // (Path, IsDir, DeleteFiles, DeleteSubdirsAlso). No-op if path is missing.
  DelTree(ExpandConstant('{localappdata}\deckify-installer-bundle'),
          True, True, True);
end;

procedure RunBundledSetup();
var
  SetupExe: string;
  RC: Integer;
begin
  SetupExe := ExpandConstant('{localappdata}\deckify-installer-bundle\setup.exe');
  if not FileExists(SetupExe) then begin
    MsgBox('Bundled setup.exe nicht gefunden: ' + SetupExe,
           mbError, MB_OK);
    Abort();
  end;

  // Launch the ClickOnce bootstrapper. It shows the standard
  // "Application Install - Security Warning" trust prompt with the
  // "Publisher cannot be verified" warning. The user clicks Install once;
  // subsequent auto-updates from gh-pages are silent (same cert thumbprint).
  Exec(SetupExe, '', '', SW_SHOW, ewWaitUntilTerminated, RC);
  if RC <> 0 then begin
    Log(Format('setup.exe exited with code %d', [RC]));
    // Inline the Format call on a single logical line; ISCC's pre-pass
    // treats a "[X]" token at the start of a line as a section header
    // even inside [Code], so we keep arg lists off line-starts.
    MsgBox('ClickOnce-Installation fehlgeschlagen (Code ' + IntToStr(RC) + ').' + #13#10 + 'Versuche die manuelle Installation gemaess Anleitung auf' + #13#10 + 'https://tillmannschatz.github.io/deckify/', mbError, MB_OK);
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  case CurStep of
    ssInstall:
      begin
        // Before [Files] extracts the bundle:
        // - wipe stale ClickOnce subscriptions
        // - drop old user-store cert entries from earlier installer attempts
        // - delete any prior bundle dir so the new files extract cleanly
        CleanupOldClickOnceSubscription();
        CleanupOldUserCert();
        CleanupOldBundle();
      end;
    ssPostInstall:
      // [Files] has copied the payload to %LocalAppData%\deckify-installer-bundle.
      // Launch setup.exe — user interacts with the ClickOnce trust prompt.
      // VSTOInstaller registers HKCU\...\Manifest = file:// (the bundle path),
      // and we deliberately don't try to overwrite it. We did try, but
      // VSTOInstaller is forked async and writes the file:// URL AFTER our
      // overwrite, so the user ends up with the wrong URL anyway. Living with
      // file:// in HKCU means ClickOnce auto-updates don't fire — the in-app
      // "Update" button on the About panel handles updates by sending the user
      // back to the latest installer.exe.
      RunBundledSetup();
  end;
end;
