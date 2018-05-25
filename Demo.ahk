#SingleInstance force

; Load CLR library that allows us to load C# DLLs
#include Lib\HotVoice.ahk
recognizerID := 0	; Set Recognizer ID here if you need to
words := ["Guns", "Bombs", "Flaps"]

Gui, Add, Text, xm Center w400, % "Found Recognizers (Using ID " recognizerID ")"
Gui, Add, ListView, xm w400, ID|Name
Gui, Add, Text, Center w400, Mic Volume
Gui, Add, Slider, xm w400 hwndhSlider
Gui, Add, Text, xm Center w400, Available Words
Gui, Add, Text, xm w400 hwndhWords
Gui, Add, Text, xm Center w400, Output
Gui, Add, Edit, hwndhOutput w400 r5
Gui, Show, , HotVoice Demo

hv := new HotVoice()
recognizers := hv.GetRecognizerList()

Loop % recognizers.Length(){
	rec := recognizers[A_index]
	LV_Add(, rec.Id, rec.Name)
}

; Start the engine!!
hv.Initialize(recognizerID)

max := words.Length()
Loop % max {
	word := words[A_Index]
	hv.SubscribeWord(word, Func("UpdateOutput").Bind(word))
	l .= word
	if (A_Index != max)
		l .= ", "
}
GuiControl, , % hWords, % l

; Monitor the volume
hv.SubscribeVolume(Func("VolumeChanged"))

return

VolumeChanged(state){
	global hSlider
	GuiControl, , % hSlider, % state
}

UpdateOutput(text){
	global hOutput
	static WM_VSCROLL = 0x115
	static SB_BOTTOM = 7
	Gui, +HwndhGui
	; Get old text
	GuiControlGet, t, , % hOutput
	t .= text " @ " A_Now "`n"
	GuiControl, , % houtput, % t
	; Scroll box to end
	PostMessage, WM_VSCROLL, SB_BOTTOM, 0, Edit1, ahk_id %hGui%
}


^Esc::
GuiClose:
	ExitApp