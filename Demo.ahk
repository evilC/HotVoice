﻿#SingleInstance force
#Persistent

; Load the HotVoice Library
#include Lib\HotVoice.ahk

; Create a new HotVoice class
hv := new HotVoice()
recognizers := hv.GetRecognizerList()

Gui, Add, Text, xm w600 Center, Available Recognizers
Gui, Add, ListView, xm w600 r5 hwndhRecognizerId -Multi, ID|Name|Code|Language
Loop % recognizers.Length(){
	rec := recognizers[A_index]
	if (rec.TwoLetterISOLanguageName == "iv")
		continue ; Invariant language culture does not seem to be supported
	LV_Add(, rec.Id, rec.Name, rec.TwoLetterISOLanguageName, rec.LanguageDisplayName)
}
LV_ModifyCol(1, 30)
LV_ModifyCol(2, 350)
LV_ModifyCol(3, 40)
LV_ModifyCol(4, 155)
LV_Modify(1, "Select")
Gui, Add, Button, Center w600 gLoadRecognizer, Load selected recognizer`n(Languages supported by this demo: en, fr)
Gui, Add, Text, xm w600 Center, Available Commands
Gui, Add, ListView, xm w600 r10 hwndhAvailableCommands, Name|Grammar
Gui, Add, Text, xm Center w600, Mic Volume
Gui, Add, Slider, xm w600 hwndhSlider
Gui, Add, Text, xm Center w600, Output
Gui, Add, Edit, hwndhOutput w600 r5
LV_ModifyCol(1, 125)
Gui, Show, , HotVoice Demo
return

LoadRecognizer:
; Initialize HotVoice and tell it what ID Recognizer to use
Gui, ListView, % hRecognizerId
if (LV_GetCount() == 0){
	UpdateOutput("No supported languages found")
	return
}
recognizer := GetCurrentRecognizer()
Gui, ListView, % hAvailableCommands
LV_Delete()

if (recognizer.TwoLetterISOLanguageName == "en"){
	; ==== ENGLISH ====
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
	hv.LoadGrammar(volumeGrammar, "Volume", Func("LogWords"))
	; Use custom text in listview, else it is too long
	LV_Add(, "Volume", "Volume [[quarter,half,three-quarters,full],[<Number> percent]]")

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

	LV_Add(, "CallContact", hv.LoadGrammar(contactGrammar, "CallContact", Func("LogWords")))
} else if (recognizer.TwoLetterISOLanguageName == "fr"){
	; ==== FRENCH ====
	hv.Initialize(recognizer.Id)
	
	; -------- Volume Command ------------
	volumeGrammar := hv.NewGrammar()
	volumeGrammar.AppendString("Volume")

	percentPhrase := hv.NewGrammar()
	percentChoices := hv.GetChoices("Percent")
	percentPhrase.AppendChoices(percentChoices)
	percentPhrase.AppendString("Pour-cent")

	volumeGrammar.AppendGrammars(percentPhrase)
	hv.LoadGrammar(volumeGrammar, "FrenchVolume", Func("LogWords"))
	LV_Add(, "Volume", "Volume [<Nombre>] pour cent")
	
	; -------- French Greeting Command ------------
	contactGrammar := hv.NewGrammar()
	contactGrammar.AppendString("Bonjour")

	maleGrammar := hv.NewGrammar()
	maleChoices := hv.NewChoices("Claude, Jaques")
	maleGrammar.AppendChoices(maleChoices)

	contactGrammar.AppendGrammars(maleGrammar)

	LV_Add(, "Greeting (French)", hv.LoadGrammar(contactGrammar, "FrenchGreeting", Func("LogWords")))
} else if (recognizer.TwoLetterISOLanguageName == "ru"){
	; ==== RUSSIAN ====
	hv.Initialize(recognizer.Id)
	; -------- Volume Command ------------
	volumeGrammar := hv.NewGrammar()
	volumeGrammar.AppendString("Громкость")

	percent1Phrase := hv.NewGrammar()
	percent1Choices := hv.GetChoices("Percent")
	percent1Phrase.AppendChoices(percent1Choices)
	;in real life language it sounds more like "процентоф", but it also work with grammatically correct "процентов"
	percent1Phrase.AppendString("процентов")

	percent2Phrase := hv.NewGrammar()
	percent2Choices := hv.GetChoices("Percent")
	percent2Phrase.AppendChoices(percent2Choices)
	percent2Phrase.AppendString("процента")

	percent3Phrase := hv.NewGrammar()
	percent3Choices := hv.GetChoices("Percent")
	percent3Phrase.AppendChoices(percent3Choices)
	percent3Phrase.AppendString("процент")

	fractionPhrase := hv.NewGrammar()
	fractionChoices := hv.NewChoices("четверть, половина, три-четверти, полная")
	fractionPhrase.AppendChoices(fractionChoices)

	volumeGrammar.AppendGrammars(percent1Phrase, percent2Phrase, percent3Phrase, fractionPhrase)

	hv.LoadGrammar(volumeGrammar, "RussianVolume", Func("LogWords"))
	; Use custom text in listview, else it is too long
	LV_Add(, "Volume", "Громкость [[четверть,половина,три-четверти,полная],[<число> процентов,процента,процент]]")

	; -------- Call Contact Command -------------
	contactGrammar := hv.NewGrammar()
	contactGrammar.AppendString("Позвонить")

	femaleGrammar := hv.NewGrammar()
	;grammatically correct form "Кристине, Маше" but it sounds in language more like "Кристине, Маши"
	femaleChoices := hv.NewChoices("Кристине, Маши")
	femaleGrammar.AppendChoices(femaleChoices)
	femaleGrammar.AppendString("на-ее")

	maleGrammar := hv.NewGrammar()
	maleChoices := hv.NewChoices("Олегу, Роме")
	maleGrammar.AppendChoices(maleChoices)
	;grammatically correct form "на-его" but it sounds in language more like "на-ево"
	maleGrammar.AppendString("на-ево")

	contactGrammar.AppendGrammars(maleGrammar, femaleGrammar)

	phoneChoices := hv.NewChoices("сотовый, домашний, рабочий")
	contactGrammar.AppendChoices(phoneChoices)
	contactGrammar.AppendString("телефон")

	LV_Add(, "CallContact", hv.LoadGrammar(contactGrammar, "RussianCallContact", Func("LogWords")))	
} else {
	UpdateOutput("Language " recognizer.TwoLetterISOLanguageName " is not supported by this demo")
	return
}

; Monitor the volume
hv.SubscribeVolume(Func("OnMicVolumeChange"))
LogRecognizerLoad(recognizer)
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

; Log out recognized words
LogWords(grammarName, words){
	UpdateOutput(grammarName ": " Join(words))
}

; Join array of words into sentence
Join(arr){
	for i, w in arr {
		str .= w " "
	}
	return str
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