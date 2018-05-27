#SingleInstance force
#Persistent

; Load the HotVoice Library
#include Lib\HotVoice.ahk

Gui, Add, Text, xm w600 Center, Available Commands
Gui, Add, ListView, xm w600 r10, Name|Grammar

; Create a new HotVoice class
hv := new HotVoice()

; Initialize HotVoice and tell it what ID Recognizer to use
hv.Initialize(0)

; -------- Volume Command ------------
hv.GrammarVarAddChoiceVar("percentPhrase", "Percent")
hv.GrammarVarAddString("percentPhrase", "percent")

hv.ChoiceVarAdd("fractionChoices", "quarter, half, three-quarters, full")
hv.GrammarVarAddChoiceVar("fractionPhrase", "fractionChoices")

hv.GrammarVarAddString("volumeCommand", "Volume")
hv.GrammarVarAddGrammarVars("volumeCommand", "fractionPhrase, percentPhrase")

LV_Add(, "Volume", hv.GrammarVarLoad("volumeCommand", Func("Volume")))
LV_ModifyCol(1, 80)

; -------- Call Contact Command -------------
hv.ChoiceVarAdd("females", "Anne, Mary")
hv.GrammarVarAddChoiceVar("callFemales", "females")
hv.GrammarVarAddString("callFemales", "on her")

hv.ChoiceVarAdd("males", "James, Sam")
hv.GrammarVarAddChoiceVar("callMales", "males")
hv.GrammarVarAddString("callMales", "on his")

;// Create a Choices object that contains a set of alternative phone types.
hv.ChoiceVarAdd("phoneTypes", "cell, home, work")


;// Construct the phrase.
hv.GrammarVarAddString("CallGrammar", "Call")
hv.GrammarVarAddGrammarVars("CallGrammar", "callFemales, callMales")
hv.GrammarVarAddChoiceVar("CallGrammar", "phoneTypes")
hv.GrammarVarAddString("CallGrammar", "phone")

LV_Add(, "CallContact", hv.GrammarVarLoad("CallGrammar", Func("CallContact")))
Gui, Show, , Grammar Example

hv.StartRecognizer()

return

Volume(value){
	static fractionToPercent := {"quarter": 25, "half": 50, "three-quarters": 75, "full": 100}
	words := BuildArray(value)
	if (words[3] = "percent"){
		vol := words[2]
	} else {
		vol := fractionToPercent[words[2]]
	}
	Tooltip % "Volume: " words[2] " " words[3] "`nSETTING VOLUME TO " vol
	SoundSet, % vol
}

CallContact(value){
	words := BuildArray(value)
	max := words.Length()
	Loop % max {
		wordStr .= words[A_Index]
		if (A_Index != max){
			wordStr .= " "
		}
	}
	ToolTip % "CALL CONTACT: " wordStr
}

BuildArray(arr){
	max := arr.MaxIndex()
	ret := []
	Loop % max + 1 {
		ret.Push(arr[A_Index - 1])
	}
	return ret
}

^Esc::
	ExitApp