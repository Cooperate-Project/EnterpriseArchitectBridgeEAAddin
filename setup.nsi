CRCCheck On
!include "MUI.nsh"
!include "target\project.nsh"

!define MUI_ABORTWARNING
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "LICENSE"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "English"

Name "${PROJECT_NAME} ${PROJECT_VERSION}"
InstallDir "$PROGRAMFILES\${PROJECT_NAME}"

Section "Main" SEC01
  SetOutPath "$INSTDIR"
  file /r "${PROJECT_BUILD_DIR}\libraries\*"  
SectionEnd

Section "Registration" SEC02
  ; determine path of regasm.exe
  ClearErrors
  ReadRegStr $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client" "InstallPath"
  IfErrors ERROR_FRAMEWORK NO_ERROR_FRAMEWORK
  ERROR_FRAMEWORK:
    Abort "Could not determine path of .NET framework version 4."
  NO_ERROR_FRAMEWORK:
  StrCpy $1 "RegAsm.exe"
  StrCpy $1 $0$1
  StrCpy $2 "\EnterpriseArchitectAutoRefresh.dll"
  StrCpy $2 $INSTDIR$2

  ; register assembly
  ClearErrors
  ExecWait '"$1" /codebase "$2"'
  IfErrors ERROR_REGISTER NO_ERROR_REGISTER
  ERROR_REGISTER:
    Abort "Could not register addin in .NET framework."
  NO_ERROR_REGISTER:

  ; register addin
  ClearErrors
  WriteRegStr HKCU "Software\Sparx Systems\EAAddins\EnterpriseArchitectAutoRefresh" "" "EnterpriseArchitectAutoRefresh.AutoRefreshAddin"
  IfErrors ERROR_REGISTRY NO_ERROR_REGISTRY
  ERROR_REGISTRY:
    Abort "Could not register addin in Enterprise Architect."
  NO_ERROR_REGISTRY:
SectionEnd

Section -Post
  SetOutPath "$INSTDIR"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PROJECT_NAME}" "DisplayName" "${PROJECT_NAME} (remove only)"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PROJECT_NAME}" "UninstallString" "$INSTDIR\Uninstall.exe"
  WriteUninstaller "$INSTDIR\Uninstall.exe"
SectionEnd

Section Uninstall
  ; unregister library
  DeleteRegKey HKCU "Software\Sparx Systems\EAAddins\EnterpriseArchitectAutoRefresh"
  ClearErrors
  ReadRegStr $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client" "InstallPath"
  IfErrors ERROR_FRAMEWORK NO_ERROR_FRAMEWORK
  ERROR_FRAMEWORK:
  MessageBox MB_OK "Could not determine path of .NET framework version 4."
  Goto FILE_DELETION
  NO_ERROR_FRAMEWORK:
  StrCpy $1 "RegAsm.exe"
  StrCpy $1 $0$1
  StrCpy $2 "\EnterpriseArchitectAutoRefresh.dll"
  StrCpy $2 $INSTDIR$2
  ClearErrors
  ExecWait '"$1" /codebase /unregister "$2"'
  IfErrors ERROR_UNREGISTER NO_ERROR_UNREGISTER
  ERROR_UNREGISTER:
  MessageBox MB_OK  "Could not unregister addin in .NET framework."
  Goto FILE_DELETION
  NO_ERROR_UNREGISTER:

  ; delete files 
  FILE_DELETION:
  RMDir /r "$INSTDIR\*.*"    
  RMDir "$INSTDIR"

  ; unregister uninstaller
  DeleteRegKey HKEY_LOCAL_MACHINE "SOFTWARE\${PROJECT_NAME}"
  DeleteRegKey HKEY_LOCAL_MACHINE "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PROJECT_NAME}"  

SectionEnd


