# HotVoice

Use your voice as a Hotkey in AutoHotkey!  

## Using HotVoice in your scripts
### Inital Setup
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
![](https://i.imgur.com/XI0sqC8.png) 

"Recognizers" are basically Language Packs. Ordering seems pretty arbitrary, but the "Lightweight" one seems present on all machines and does not seem to work. Luckily, it seems that any other language pack that is installed will be `ID 0`, so for now I just hard-code the Demo to use ID 0. You can tweak it in the code though. 
The `Mic Volume` slider should move when you speak.  
HotVoice uses the "Default Recording Device" that is configured in Windows.  
6. See the `Simple Example.ahk` for the simplest possible script. More documentation will be forthcoming, sorry...

# Developers
This **ONLY APPLIES** if you want to work with the C# code that powers HotVoice.  
If you are writing AHK scripts using HotVoice, this does not apply to you.  
### Initial Setup
1. Install the Speech SDK.  
2. When you open the SLN, you may need to fix reference to `Microsoft.Speech`  
It can be found in `C:\Program Files\Microsoft SDKs\Speech\v11.0\Assembly` 
