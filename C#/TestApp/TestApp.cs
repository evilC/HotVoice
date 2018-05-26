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

            //hv.AddChoiceList("udlr", new[] {"Up", "Down", "Left", "Right"});
            hv.AddChoiceList("Apps", "Notepad, Command Prompt");
            hv.SubscribeWordWithChoiceList("flaps", "Percent", new Action<string>((value) => {
                Console.WriteLine($"flaps: {value}");
            }));

            hv.SubscribeWordWithChoiceList("Launch", "Apps", new Action<string>((value) =>
            {
                Console.WriteLine($"Launch: *{value}*");
            }));

            hv.SubscribeWord("test", new Action(() =>
            {
                Console.WriteLine("Test");
            }));

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
