#SingleInstance force

; Load the HotVoice Library
#include Lib\HotVoice.ahk

; Create a new HotVoice class
hv := new HotVoice()

; Initialize HotVoice and tell it what ID Recognizer to use
hv.Initialize(0)

; Add a word, and tell it which function to call when that word is spoken
hv.SubscribeWord("Test", Func("MyFunc"))

return

MyFunc(){
	ToolTip % "HotWord was triggered @ " A_TickCount
}
