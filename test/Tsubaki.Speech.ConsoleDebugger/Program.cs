// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Tsubaki.Speech.ConsoleDebugger
{
    using System;

    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var vr = new VoiceRecognizer("secret.json");

            vr.Start();
            Console.ReadKey();

            vr.Stop();
            Console.ReadKey();
        }
    }
}