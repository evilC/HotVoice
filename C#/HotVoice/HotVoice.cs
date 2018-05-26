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
    private readonly Dictionary<string, dynamic> _parameterCallbacks = new Dictionary<string, dynamic>(StringComparer.OrdinalIgnoreCase);
    private dynamic _volumeCallback;
    private int _volumeLevel = 0;
    private bool _recognizerRunning;
    private readonly List<RecognizerInfo> _recognizers;
    private readonly  Dictionary<string, Choices> _choiceDictionary = new Dictionary<string, Choices>(StringComparer.OrdinalIgnoreCase);

    public HotVoice()
    {
        _recognizers = SpeechRecognitionEngine.InstalledRecognizers().ToList();
        var percentileChoices = new Choices();
        for (var i = 0; i <= 100; i++)
            percentileChoices.Add(i.ToString());
        _choiceDictionary.Add("Percent", percentileChoices);
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
        _recognizer.SpeechRecognized += Recognizer_SpeechRecognized;

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
        var dict = new Dictionary<string, string>();
        foreach (var t in _recognizers)
        {
            dict.Add(t.Id, t.Name);
        }

        return dict;
    }

    public string Test(string[] blah)
    {
        return "OK";
    }

//    public void AddChoiceList(string name, string[] choiceArray)
    public void AddChoiceList(string name, string choiceString)
    {
        var parts = choiceString.Split(',').Select(p => p.Trim()).ToArray();

        _choiceDictionary.Add(name, new Choices(parts));
    }

    private Choices GetChoiceList(string name)
    {
        if (!_choiceDictionary.ContainsKey(name))
        {
            throw new Exception($"Could not find Choice List {name}");
        }

        return _choiceDictionary[name];
    }

    public bool SubscribeWordWithChoiceList(string text, string choiceList, dynamic callback)
    {
        try
        {
            if (_parameterCallbacks.ContainsKey(text))
            {
                return false;
            }
            _parameterCallbacks[text] = callback;

            var gb = new GrammarBuilder(text);

            gb.Append(GetChoiceList(choiceList));
            

            var g = new Grammar(gb) {Priority = 127};

            _recognizer.LoadGrammar(g);

            StartRecognizer();
            return true;
        }
        catch
        {
            return false;
        }
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

            var g = new Grammar(new GrammarBuilder(text));
            _recognizer.LoadGrammar(g);

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
            _recognizer.AudioLevelUpdated += Recognizer_AudioLevelUpdated;
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
    private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
    {
        var word = e.Result.Words[0].Text;
        if (e.Result.Words.Count > 1)
        {
            var p = "";
            var max = e.Result.Words.Count;
            for (var i = 1; i < max; i++)
            {
                p += e.Result.Words[i].Text;
                if (i < max - 1)
                {
                    p += " ";
                }
            }
            _parameterCallbacks[word](p);
        }
        else
        {
            _wordCallbacks[word]();
        }
    }

    // Write the audio level to the console when the AudioLevelUpdated event is raised.
    private void Recognizer_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
    {
        if (e.AudioLevel != _volumeLevel)
        {
            _volumeLevel = e.AudioLevel;
            _volumeCallback(_volumeLevel);
        }
    }

}

