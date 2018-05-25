using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Speech.Recognition;

// Get Started with Speech Recognition (Microsoft.Speech):
// https://msdn.microsoft.com/en-us/library/hh378426(v=office.14).aspx

// Install stuff from here:
// https://msdn.microsoft.com/en-us/library/hh362873(v=office.14).aspx#Anchor_2

// Plus a recognizer:
// https://www.microsoft.com/en-us/download/details.aspx?id=27224

public class HotVoice
{
    private SpeechRecognitionEngine _recognizer;
    private readonly Dictionary<string, dynamic> _wordCallbacks = new Dictionary<string, dynamic>(StringComparer.OrdinalIgnoreCase);
    private dynamic _volumeCallback;
    private int _volumeLevel = 0;
    private bool _recognizerRunning;
    private readonly List<RecognizerInfo> _recognizers;

    public HotVoice()
    {
        _recognizers = SpeechRecognitionEngine.InstalledRecognizers().ToList();
    }

    public string OkCheck()
    {
        return "OK";
    }

    public void Initialize(int recognizerId = 0)
    {
        AssertRecognizerExists(recognizerId);

        _recognizer = new SpeechRecognitionEngine(_recognizers[recognizerId].Id);

        // Add a handler for the speech recognized event.
        _recognizer.SpeechRecognized += recognizer_SpeechRecognized;

        // Configure the input to the speech recognizer.
        _recognizer.SetInputToDefaultAudioDevice();
    }

    public int GetRecognizerCount()
    {
        return _recognizers.Count;
    }

    public void AssertRecognizerExists(int recognizerId)
    {
        if (_recognizers.Count() < recognizerId)
        {
            throw new Exception($"Recognizer ID {recognizerId} does not exist");
        }
    }

    public string GetRecognizerName(int recognizerId)
    {
        AssertRecognizerExists(recognizerId);
        return _recognizers[recognizerId].Name;
    }

    public Dictionary<string, string> GetRecognizers()
    {
        var str = string.Empty;
        var dict = new Dictionary<string, string>();
        foreach (var t in _recognizers)
        {
            dict.Add(t.Id, t.Name);
        }

        return dict;
    }

    public bool SubscribeWord(string text, dynamic callback)
    {
        try
        {
            if (_wordCallbacks.ContainsKey(text))
            {
                return false;
            }
            _wordCallbacks[text] = callback;

            var servicesGrammar = new Grammar(new GrammarBuilder(new Choices(new string[] { text })));
            _recognizer.LoadGrammarAsync(servicesGrammar);

            StartRecognizer();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool SubscribeVolume(dynamic callback)
    {
        try
        {
            _volumeCallback = callback;
            _recognizer.AudioLevelUpdated += new EventHandler<AudioLevelUpdatedEventArgs>(sre_AudioLevelUpdated);
            StartRecognizer();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void StartRecognizer()
    {
        if (!_recognizerRunning)
        {
            // Start asynchronous, continuous speech recognition.
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            _recognizerRunning = true;
        }
    }
           
    // Handle the SpeechRecognized event.
    private void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
    {
        _wordCallbacks[e.Result.Text]();
    }

    // Write the audio level to the console when the AudioLevelUpdated event is raised.
    private void sre_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
    {
        if (e.AudioLevel != _volumeLevel)
        {
            _volumeLevel = e.AudioLevel;
            _volumeCallback(_volumeLevel);
        }
    }

}

