[Setup]
AppName=Zaapix
AppVersion=1.0
DefaultDirName={userappdata}\Zaapix
DisableDirPage=yes
OutputDir=.
OutputBaseFilename=ZaapixInstaller
Compression=lzma
SolidCompression=yes
SetupIconFile=.\ico\icon.ico

[Files]
Source: "D:\DofusGroupFinder\ClientPublish\ZaapixClient\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs
Source: "dotnet-runtime.exe"; Flags: dontcopy

[Icons]
Name: "{group}\Zaapix"; Filename: "{app}\Zaapix.exe"
Name: "{group}\Uninstall Zaapix"; Filename: "{uninstallexe}"
Name: "{userdesktop}\Zaapix"; Filename: "{app}\Zaapix.exe"

[Run]
; Installe .NET Desktop Runtime 9 en silence si nécessaire
Filename: "{tmp}\dotnet-runtime.exe"; Parameters: "/install /quiet /norestart"; \
    Flags: runhidden waituntilterminated; StatusMsg: "Installation du Runtime .NET 9..."; Check: NeedsDotNet

; Lance Zaapix après l'installation (si pas en mode silencieux)
Filename: "{app}\Zaapix.exe"; Description: "Lancer Zaapix"; Flags: nowait postinstall skipifsilent

[Code]
function NeedsDotNet(): Boolean;
begin
  Result := True;
end;

procedure InitializeWizard();
begin
  if NeedsDotNet() then
    ExtractTemporaryFile('dotnet-runtime.exe');
end;
