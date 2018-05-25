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
            var str = hv.GetRecognizers();
            hv.Initialize();
            hv.SubscribeWord("five", new Action(() => {
                Console.WriteLine("Five");
            }));

            hv.SubscribeWord("one", new Action(() => {
                Console.WriteLine("One");
            }));

            hv.SubscribeVolume(new Action<int>((value) => {
                Console.WriteLine("Volume: " + value);
            }));

            while (true)
            {
                Console.ReadLine();
            }
        }

    }
}
