# HotVoice

Use your voice as a Hotkey in AutoHotkey!  

## Using HotVoice in your scripts
### Initial Setup
1. Install the [Microsoft Speech Platform Runtime](https://www.microsoft.com/en-us/download/details.aspx?id=27225)  
You will need either x86 or x64, depending on what version of AHK you run. No harm in installing both, just to be sure.  
2. Install at least one [Language Pack](https://www.microsoft.com/en-us/download/details.aspx?id=27224)  
eg `MSSpeech_SR_en-US_TELE.msi`  
3. Download a release of HotVoice from the [Releases Page](https://github.com/evilC/HotVoice/releases) and unzip it to a folder of your choice (This shall be referred to as the "HotVoice folder" from now on).    
**DO NOT** use the "Clone or Download" button on the main GitHub page, this is for developers.  
4. Ensure the DLLs that are in the HotVoice Folder are not blocked.  
There are various ways to do this, but I find the simplest is to run the powershell command `Get-ChildItem -Path '.' -Recurse | Unblock-File` in the Hotvoice Folder.  
5. Run the Demo script and make sure it works for you.  
It should look something like this:  
![](https://i.imgur.com/TLzzvTF.png) 

"Recognizers" are basically Language Packs. Ordering seems pretty arbitrary, but the "Lightweight" one seems present on all machines and does not seem to work. Luckily, it seems that any other language pack that is installed will be `ID 0`, so for now I just hard-code the Demo to use ID 0. You can tweak it in the code though. 
The `Mic Volume` slider should move when you speak.  
HotVoice uses the "Default Recording Device" that is configured in Windows.  
6. See the `Simple Example.ahk` for the simplest possible script. More documentation will be forthcoming, sorry...

### Using the Library
#### Grammar and Choices objects
HotVoice uses these two types of object to build up commands.  
Throughout this documentation, the following syntax will be used to denote a phrase with optional components:  
`Launch [Notepad, Word]`  
In this instance it can mean "Launch Notepad" or "Launch Word".  
##### Choices Objects
Choices objects represent a series of optional words. As in the above example, `[Notepad, Word]` is a series of Choices.  
##### Grammar Objects
Grammar objects are the primary building blocks of HotVoice. They can hold either single words, or choices objects, or even other Grammar objects.
#### Initializing HotVoice
A HotVoice script must do the following:  
1. Load the HotVoice Library
```
; Load the HotVoice Library
#include Lib\HotVoice.ahk
```
2. Create a new HotVoice class
```
hv := new HotVoice()
```
3. Add at least one Grammar
```
; Create a new Grammar
testGrammar := hv.NewGrammar()

; Add the word "Test" to it
testGrammar.AppendString("Test")

; Load the Grammar
hv.LoadGrammar(testGrammar, "Test", Func("MyFunc"))
```
4. Start the Recognizer
```
hv.StartRecognizer()
```


# Developers
This **ONLY APPLIES** if you want to work with the C# code that powers HotVoice.  
If you are writing AHK scripts using HotVoice, this does not apply to you.  
### Initial Setup
1. Install the [Microsoft Speech Platform SDK 11](https://msdn.microsoft.com/en-us/library/hh362873(v=office.14).aspx#Anchor_2).  
2. When you open the SLN, you may need to fix the reference to `Microsoft.Speech`  
It can be found in `C:\Program Files\Microsoft SDKs\Speech\v11.0\Assembly` 

[Speech API Reference on MSDN](https://msdn.microsoft.com/en-us/library/hh378380(v=office.14).aspx)
