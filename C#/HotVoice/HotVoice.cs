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
    //private readonly Dictionary<string, dynamic> _wordCallbacks = new Dictionary<string, dynamic>(StringComparer.OrdinalIgnoreCase);
    //private readonly Dictionary<string, dynamic> _parameterCallbacks = new Dictionary<string, dynamic>(StringComparer.OrdinalIgnoreCase);
    private dynamic _volumeCallback;
    private int _volumeLevel = 0;
    private bool _recognizerRunning;
    private readonly List<RecognizerInfo> _recognizers;
    private readonly  Dictionary<string, Choices> _choiceVarDictionary = new Dictionary<string, Choices>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, GrammarBuilder> _grammarBuilderDictionary = new Dictionary<string, GrammarBuilder>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, dynamic> _grammarCallbacks = new Dictionary<string, dynamic>(StringComparer.OrdinalIgnoreCase);

    public HotVoice()
    {
        _recognizers = SpeechRecognitionEngine.InstalledRecognizers().ToList();
        var percentileChoices = new Choices();
        for (var i = 0; i <= 100; i++)
            percentileChoices.Add(i.ToString());
        _choiceVarDictionary.Add("Percent", percentileChoices);
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

    #region Grammar Vars & Choice Vars
    public GrammarBuilder GrammarVarGet(string name)
    {
        if (!_grammarBuilderDictionary.ContainsKey(name))
        {
            _grammarBuilderDictionary.Add(name, new GrammarBuilder());
        }

        return _grammarBuilderDictionary[name];
    }

    public string GrammarVarLoad(string grammarName, dynamic callback)
    {
        var gb = GrammarVarGet(grammarName);
        var g = new Grammar(gb);
        _grammarCallbacks.Add(grammarName, callback);
        g.Name = grammarName;
        _recognizer.LoadGrammar(g);
        return gb.DebugShowPhrases;
    }

    public void GrammarVarAddString(string name, string str)
    {
        GrammarVarGet(name).Append(str);
    }

    public void GrammarVarAddGrammarVars(string grammarName, string grammarVars)
    {
        var choices = GrammarVarListToChoices(grammarVars);
        GrammarVarGet(grammarName).Append(choices);
    }

    public void GrammarVarAddChoiceVar(string grammarName, string choiceVar)
    {
        GrammarVarGet(grammarName).Append(ChoiceVarGet(choiceVar));
    }

    public GrammarBuilder[] GrammarVarListToGrammarBuilderArray(string grammarBuilderVarList)
    {
        var grammarVars = StringToArray(grammarBuilderVarList);
        var grammarBuilderArray = new GrammarBuilder[grammarVars.Length];
        for (var i = 0; i < grammarVars.Length; i++)
        {
            grammarBuilderArray[i] = GrammarVarGet(grammarVars[i]);
        }

        return grammarBuilderArray;
    }

    public Choices GrammarVarListToChoices(string grammarBuilderListStr)
    {
        //var names = StringToArray(grammarBuilderVarList);
        return new Choices(GrammarVarListToGrammarBuilderArray(grammarBuilderListStr));
    }

    public void ChoiceVarAdd(string name, string choiceString)
    {
        _choiceVarDictionary.Add(name, new Choices(StringToArray(choiceString)));
    }

    private Choices ChoiceVarGet(string name)
    {
        if (!_choiceVarDictionary.ContainsKey(name))
        {
            throw new Exception($"Could not find Choice Var {name}");
        }

        return _choiceVarDictionary[name];
    }

    #endregion

    #region Recognizers

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

    #endregion

    // I don't know how to pass an array of strings from AHK to C#, so for now, just use comma-separated strings
    public string[] StringToArray(string choiceString) => choiceString.Split(',').Select(p => p.Trim()).ToArray();

    #region Subscriptions

    //public bool SubscribeWordWithChoiceList(string text, string choiceList, dynamic callback)
    //{
    //    try
    //    {
    //        if (_parameterCallbacks.ContainsKey(text))
    //        {
    //            return false;
    //        }
    //        _parameterCallbacks[text] = callback;

    //        var gb = new GrammarBuilder(text);

    //        gb.Append(ChoiceVarGet(choiceList));
            

    //        var g = new Grammar(gb) {Priority = 127};

    //        _recognizer.LoadGrammar(g);
    //        _grammarCallbacks[text] = callback;
    //        return true;
    //    }
    //    catch
    //    {
    //        return false;
    //    }
    //}

    //public bool SubscribeWord(string text, dynamic callback)
    //{
    //    try
    //    {
    //        if (_wordCallbacks.ContainsKey(text))
    //        {
    //            return false;
    //        }
    //        _wordCallbacks[text] = callback;

    //        var g = new Grammar(new GrammarBuilder(text));
    //        g.Name = text;
    //        _recognizer.LoadGrammar(g);
    //        _grammarCallbacks[text] = callback;
    //        return true;
    //    }
    //    catch
    //    {
    //        return false;
    //    }
    //}

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
    #endregion

    public void StartRecognizer()
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
        var name = e.Result.Grammar.Name;
        var words = new string[e.Result.Words.Count];
        for (var i = 0; i < e.Result.Words.Count; i++)
        {
            words[i] = e.Result.Words[i].Text;
        }

        _grammarCallbacks[name](words);
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

