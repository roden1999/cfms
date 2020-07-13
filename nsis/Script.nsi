
;helper defines
!define PRODUCT_NAME "ZKTS File Manager"
!define PRODUCT_VERSION ""
!define PRODUCT_PUBLISHER "ZKTeco Solutions"
!define PRODUCT_OWNER "ZKTeco Solutions"
!define PRODUCT_INTERNAL "HRSI5"
!define PRODUCT_WEB_SITE "http://payroll.mysolutions.ph"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\ZKTeco Solutions"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKCU"

; MUI 1.67 compatible ------
!include "MUI.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"
; Welcome page
!insertmacro MUI_PAGE_WELCOME
; Directory page
!insertmacro MUI_PAGE_DIRECTORY
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
;!define MUI_FINISHPAGE_RUN "$INSTDIR\TAAdmin.exe"
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "English"

; MUI end ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "HRIS5.exe"
InstallDir "C:\inetpub\wwwroot\HRIS5"
InstallDirRegKey HKCU "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show

Section "Basic registry functions" Basic
	
	WriteRegStr  HKCU "SOFTWARE\${PRODUCT_OWNER}\${PRODUCT_INTERNAL}" "InstallPath" "$INSTDIR"
				
SectionEnd

Section "NETCORE" SEC01

  CreateDirectory "$INSTDIR\"
  CreateDirectory "$INSTDIR\App_Data"
  CreateDirectory "$INSTDIR\App_Data\attachment"
  CreateDirectory "$INSTDIR\App_Data\imports"
  CreateDirectory "$INSTDIR\App_Data\photos"
  CreateDirectory "$INSTDIR\App_Data\ATTPhotos"
    
  SetOutPath "$INSTDIR"

  SetOverwrite off
  File "settings.json"
  File "web.config" # config file we don't want to overwrite
  
  SetOverwrite on
  File /r /x web.config /x settings.json "..\serve\bin\Debug\netcoreapp3.0\publish\*.*" # 
  
    
SectionEnd

Section "NODE" SEC02

  CreateDirectory "$INSTDIR\wwwroot"
  SetOutPath "$INSTDIR\wwwroot"
  File /r /x apihost.js "..\admin\Build\*.*" 
  File apihost.js
  
SectionEnd


Section -AdditionalIcons
  ;WriteIniStr "$INSTDIR\MySolutions.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  ;CreateShortCut "$SMPROGRAMS\MySolutions Time And Attendance Enterprise Client 2.8\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd


Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?" IDYES +2
  Abort
FunctionEnd

Section Uninstall
  
  RMDir "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd