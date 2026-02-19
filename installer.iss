; Health Optimizer Installer Script
; Created with Inno Setup

#define MyAppName "Health Optimizer"
#define MyAppVersion "1.1.2"
#define MyAppPublisher "Colt Straven"
#define MyAppURL "https://github.com/ColtStraven/health-optimizer"
#define MyAppExeName "HealthOptimizer.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
AppId={{8F7A9B2C-4E5D-4A3B-9C2D-1E5F7A9B2C4E}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
OutputDir=installers
OutputBaseFilename=HealthOptimizer-Setup-v{#MyAppVersion}
SetupIconFile=Assets\app-icon.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "publish\win-x64\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Messages]
WelcomeLabel2=This will install [name/ver] on your computer.%n%nHealth Optimizer helps you track nutrition, workouts, blood pressure, and body measurements with advanced correlation analysis to optimize your health.

[Code]
procedure CurPageChanged(CurPageID: Integer);
begin
  if CurPageID = wpFinished then
  begin
    WizardForm.FinishedLabel.Caption := 
      'Health Optimizer has been successfully installed!' + #13#10 + #13#10 +
      'Start tracking your health data today to discover your optimal nutrition ranges.' + #13#10 + #13#10 +
      'Your data is stored locally and never leaves your computer.';
  end;
end;

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent