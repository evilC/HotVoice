using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Speech.Recognition;

namespace TestApp
{
    class TestApp
    {
        static void Main(string[] args)
        {
            var hv = new HotVoice.HotVoice();
            hv.Initialize();
            
            // ----------------------- Volume Demo --------------------
            var volumeGrammar = hv.NewGrammar();
            volumeGrammar.AppendString("Volume");

            var percentPhrase = hv.NewGrammar();
            var percentChoices = hv.GetChoices("Percent");
            percentPhrase.AppendChoices(percentChoices);
            percentPhrase.AppendString("percent");

            var fractionPhrase = hv.NewGrammar();
            var fractionChoices = hv.NewChoices("quarter, half, three-quarters, full");
            fractionPhrase.AppendChoices(fractionChoices);

            volumeGrammar.AppendGrammars(percentPhrase, fractionPhrase);

            hv.LoadGrammar(volumeGrammar, "Volume", new Action<string, string[]>((name, values) =>
            {
                Console.WriteLine($"{name}: {string.Join(" ", values)}");
            }));

            // ---------------------- Call Contact Demo ----------------
            var contactGrammar = hv.NewGrammar();
            contactGrammar.AppendString("Call");

            var femaleChoices = hv.NewChoices("Anne, Mary");
            var femalePhrase = hv.NewGrammar();
            femalePhrase.AppendChoices(femaleChoices);
            femalePhrase.AppendString("on her");

            var maleChoices = hv.NewChoices("James, Sam");
            var malePhrase = hv.NewGrammar();
            malePhrase.AppendChoices(maleChoices);
            malePhrase.AppendString("on his");

            contactGrammar.AppendGrammars(malePhrase, femalePhrase);

            var phoneChoices = hv.NewChoices("cell, home, work");
            contactGrammar.AppendChoices(phoneChoices);

            contactGrammar.AppendString("phone");

            hv.LoadGrammar(contactGrammar, "CallContact", new Action<string, string[]>((name, values) =>
            {
                Console.WriteLine($"{name}: {string.Join(" ", values)}");
            }));

            //hv.SubscribeVolume(new Action<int>((value) => {
            //    Console.WriteLine("Volume: " + value);
            //}));

            hv.StartRecognizer();

            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
