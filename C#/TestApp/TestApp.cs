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
            var volumeGrammar = hv.Factory.NewGrammar();
            volumeGrammar.AppendString("Volume");

            var percentPhrase = hv.Factory.NewGrammar();
            var percentChoices = hv.GetChoices("Percent");
            percentPhrase.AppendChoices(percentChoices);
            percentPhrase.AppendString("percent");

            var fractionPhrase = hv.Factory.NewGrammar();
            var fractionChoices = hv.Factory.NewChoices("quarter, half, three-quarters, full");
            fractionPhrase.AppendChoices(fractionChoices);

            volumeGrammar.AppendGrammars(percentPhrase, fractionPhrase);

            hv.LoadGrammar(volumeGrammar, "Volume", new Action<string, string[]>((name, values) =>
            {
                Console.WriteLine($"{name}: {string.Join(" ", values)}");
            }));

            // ---------------------- Call Contact Demo ----------------
            var contactGrammar = hv.Factory.NewGrammar();
            contactGrammar.AppendString("Call");

            var femaleChoices = hv.Factory.NewChoices("Anne, Mary");
            var femalePhrase = hv.Factory.NewGrammar();
            femalePhrase.AppendChoices(femaleChoices);
            femalePhrase.AppendString("on her");

            var maleChoices = hv.Factory.NewChoices("James, Sam");
            var malePhrase = hv.Factory.NewGrammar();
            malePhrase.AppendChoices(maleChoices);
            malePhrase.AppendString("on his");

            contactGrammar.AppendGrammars(malePhrase, femalePhrase);

            var phoneChoices = hv.Factory.NewChoices("cell, home, work");
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
