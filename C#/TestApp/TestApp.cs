using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    class TestApp
    {
        static void Main(string[] args)
        {
            var hv = new HotVoice();
            hv.Initialize();

            hv.GrammarVarAddChoiceVar("percentPhrase", "Percent");
            hv.GrammarVarAddString("percentPhrase", "percent");

            hv.ChoiceVarAdd("fractionChoices", "quarter, half, three-quarters, full");
            hv.GrammarVarAddChoiceVar("fractionPhrase", "fractionChoices");

            hv.GrammarVarAddString("flapsCommand", "flaps");
            hv.GrammarVarAddGrammarVars("flapsCommand", "percentPhrase, fractionPhrase");
            
            hv.GrammarVarLoad("flapsCommand", new Action<string[]>((values) => {
                Console.WriteLine($"Flaps: {values[1]}");
            }));

            // Contact Dialler demo
            hv.ChoiceVarAdd("females", "Anne, Mary");
            hv.GrammarVarAddChoiceVar("callFemales", "females");
            hv.GrammarVarAddString("callFemales", "on her");

            hv.ChoiceVarAdd("males", "James, Sam");
            hv.GrammarVarAddChoiceVar("callMales", "males");
            hv.GrammarVarAddString("callMales", "on his");

            // Create a Choices object that contains a set of alternative phone types.
            hv.ChoiceVarAdd("phoneTypes", "cell, home, work");


            // Construct the phrase.
            hv.GrammarVarAddString("CallGrammar", "Call");
            hv.GrammarVarAddGrammarVars("CallGrammar", "callFemales, callMales");
            hv.GrammarVarAddChoiceVar("CallGrammar", "phoneTypes");
            hv.GrammarVarAddString("CallGrammar", "phone");

            hv.GrammarVarLoad("CallGrammar", new Action<string[]>((values) => {
                Console.WriteLine($"Grammar: {values[0]}");
            }));



            hv.StartRecognizer();
            //hv.SubscribeWordWithChoiceList("flaps", "Percent", new Action<string[]>((values) => {
            //    Console.WriteLine($"flaps: {values[0]}, {values[1]}");
            //}));


            //hv.ChoiceVarAdd("udlr", new[] {"Up", "Down", "Left", "Right"});
            //hv.ChoiceVarAdd("Apps", "Notepad, Command Prompt");
            //hv.SubscribeWordWithChoiceList("flaps", "Percent", new Action<string>((value) => {
            //    Console.WriteLine($"flaps: {value}");
            //}));

            //hv.SubscribeWordWithChoiceList("Launch", "Apps", new Action<string>((value) =>
            //{
            //    Console.WriteLine($"Launch: *{value}*");
            //}));

            //hv.SubscribeWord("test", new Action(() =>
            //{
            //    Console.WriteLine("Test");
            //}));

            //hv.SubscribeWord("one", new Action(() => {
            //    Console.WriteLine("One");
            //}));

            //hv.SubscribeVolume(new Action<int>((value) => {
            //    Console.WriteLine("Volume: " + value);
            //}));

            while (true)
            {
                Console.ReadLine();
            }
        }

    }
}
