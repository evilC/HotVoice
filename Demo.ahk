#SingleInstance force
#Persistent

; Load the HotVoice Library
#include Lib\HotVoice.ahk

; Create a new HotVoice class
hv := new HotVoice()
recognizers := hv.GetRecognizerList()

Gui, Add, Text, xm w600 Center, Available Recognizers
Gui, Add, ListView, xm w600 r5 hwndhRecognizerId -Multi, ID|Name|Code
Loop % recognizers.Length(){
	rec := recognizers[A_index]
	LV_Add(, rec.Id, rec.Name, rec.TwoLetterISOLanguageName)
}
LV_ModifyCol(1, 50)
LV_ModifyCol(2, 450)
LV_ModifyCol(3, 75)
LV_Modify(1, "Select")
Gui, Add, Button, Center w600 gLoadRecognizer, Load Recognizer
Gui, Add, Text, xm w600 Center, Available Commands
Gui, Add, ListView, xm w600 r10 hwndhAvailableCommands, Name|Grammar
Gui, Add, Text, xm Center w600, Mic Volume
Gui, Add, Slider, xm w600 hwndhSlider
Gui, Add, Text, xm Center w600, Output
Gui, Add, Edit, hwndhOutput w600 r5
LV_ModifyCol(1, 80)
Gui, Show, , HotVoice Demo
return

LoadRecognizer:
; Initialize HotVoice and tell it what ID Recognizer to use
Gui, ListView, % hRecognizerId
recognizer := GetCurrentRecognizer()
Gui, ListView, % hAvailableCommands
LV_Delete()

if (recognizer.TwoLetterISOLanguageName == "en"){
	LogRecognizerLoad(recognizer)
	hv.Initialize(recognizer.Id)
	; -------- Volume Command ------------
	volumeGrammar := hv.NewGrammar()
	volumeGrammar.AppendString("Volume")

	percentPhrase := hv.NewGrammar()
	percentChoices := hv.GetChoices("Percent")
	percentPhrase.AppendChoices(percentChoices)
	percentPhrase.AppendString("percent")

	fractionPhrase := hv.NewGrammar()
	fractionChoices := hv.NewChoices("quarter, half, three-quarters, full")
	fractionPhrase.AppendChoices(fractionChoices)

	volumeGrammar.AppendGrammars(fractionPhrase, percentPhrase)

	LV_Add(, "Volume", hv.LoadGrammar(volumeGrammar, "Volume", Func("Volume")))

	; -------- Call Contact Command -------------
	contactGrammar := hv.NewGrammar()
	contactGrammar.AppendString("Call")

	femaleGrammar := hv.NewGrammar()
	femaleChoices := hv.NewChoices("Anne, Mary")
	femaleGrammar.AppendChoices(femaleChoices)
	femaleGrammar.AppendString("on-her")

	maleGrammar := hv.NewGrammar()
	maleChoices := hv.NewChoices("James, Sam")
	maleGrammar.AppendChoices(maleChoices)
	maleGrammar.AppendString("on-his")

	contactGrammar.AppendGrammars(maleGrammar, femaleGrammar)

	phoneChoices := hv.NewChoices("cell, home, work")
	contactGrammar.AppendChoices(phoneChoices)
	contactGrammar.AppendString("phone")

	LV_Add(, "CallContact", hv.LoadGrammar(contactGrammar, "CallContact", Func("CallContact")))
} else if (recognizer.TwoLetterISOLanguageName == "fr"){
	LogRecognizerLoad(recognizer)
	hv.Initialize(recognizer.Id)
	frenchGrammar := hv.NewGrammar()
	frenchGrammar.AppendString("Bonjour")
	LV_Add(, "French Test", hv.LoadGrammar(frenchGrammar, "Bonjour", Func("French")))
} else if (recognizer.TwoLetterISOLanguageName == "de"){
	LogRecognizerLoad(recognizer)
	hv.Initialize(recognizer.Id)
} else {
	UpdateOutput("Language " recognizer.TwoLetterISOLanguageName " is not supported by this demo")
	return
}

; Monitor the volume
hv.SubscribeVolume(Func("OnMicVolumeChange"))
hv.StartRecognizer()

return

LogRecognizerLoad(recognizer){
	UpdateOutput("Loading Recognizer ID:" recognizer.Id ", Code: " recognizer.TwoLetterISOLanguageName ", Name: " recognizer.LanguageDisplayName)
}

GetCurrentRecognizer(){
	global recognizers, hRecognizerId
	Gui, ListView, % hRecognizerId
	return recognizers[LV_GetNext()]
}

OnMicVolumeChange(state){
	global hSlider
	GuiControl, , % hSlider, % state
}

French(grammarName, words){
	UpdateOutput("Bonjour")
}

Volume(grammarName, words){
	static fractionToPercent := {"quarter": 25, "half": 50, "three-quarters": 75, "full": 100}
	if (words[3] = "percent"){
		vol := words[2]
	} else {
		vol := fractionToPercent[words[2]]
	}
	UpdateOutput(grammarName ": " words[2] " " words[3] " -- SETTING VOLUME TO " vol)
	SoundSet, % vol
}

CallContact(grammarName, words){
	max := words.Length()
	Loop % max {
		wordStr .= words[A_Index]
		if (A_Index != max){
			wordStr .= " "
		}
	}
	UpdateOutput(grammarName ": " wordStr)
}

UpdateOutput(text){
	global hOutput
	static WM_VSCROLL = 0x115
	static SB_BOTTOM = 7
	Gui, +HwndhGui
	; Get old text
	GuiControlGet, t, , % hOutput
	;~ t .= text " @ " A_Now "`n"
	t .= text "`n"
	GuiControl, , % houtput, % t
	; Scroll box to end
	PostMessage, WM_VSCROLL, SB_BOTTOM, 0, Edit1, ahk_id %hGui%
}

^Esc::
GuiClose:
	ExitApp