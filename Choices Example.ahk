#SingleInstance force
#Persistent

; Define a list of apps
appsList := {DosBox: {ExeName: "cmd.exe"}, Notepad: {ExeName: "Notepad.exe"}}

; Load the HotVoice Library
#include Lib\HotVoice.ahk

; Create a new HotVoice class
hv := new HotVoice()

; Initialize HotVoice and tell it what ID Recognizer to use
hv.Initialize(0)

; Add a List of Apps
; We need to pass a comma-separated list of choices...
; ... so parse the appNames array and build the list
for app in appsList {
	c++
}
for appName, appObj in appsList {
	i++
	appNames .= appName
	if (i != c){
		appNames .= ","
	}
}
; Add the list of apps as a "Choice List"
hv.AddChoiceList("Apps", appNames)

; Add some commands to control the apps
hv.SubscribeWordWithChoiceList("Launch", "Apps", Func("ChoiceTest").Bind("Launch"))
hv.SubscribeWordWithChoiceList("Close", "Apps", Func("ChoiceTest").Bind("Close"))
hv.SubscribeWordWithChoiceList("Minimize", "Apps", Func("ChoiceTest").Bind("Minimize"))
hv.SubscribeWordWithChoiceList("Maximize", "Apps", Func("ChoiceTest").Bind("Maximize"))
hv.SubscribeWordWithChoiceList("Restore", "Apps", Func("ChoiceTest").Bind("Restore"))

return

ChoiceTest(verb, app){
	global appsList
	ToolTip % verb " was triggered @ " A_TickCount "`nChoice: *" app "*"
	if (verb = "launch"){
		Run % appsList[app].ExeName
	} else if (verb = "close"){
		WinClose, % "ahk_exe " appsList[app].ExeName
	} else if (verb = "Minimize"){
		WinMinimize, % "ahk_exe " appsList[app].ExeName
	} else if (verb = "Maximize"){
		WinMaximize, % "ahk_exe " appsList[app].ExeName
	} else if (verb = "Restore"){
		WinRestore, % "ahk_exe " appsList[app].ExeName
	}
}

^Esc::
	ExitApp