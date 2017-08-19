using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections.ObjectModel;

namespace UKLib.Debug
{
    public struct BeepOptions
    {
        public int time, freq;
    }

    public class BeepThreadClass
    {
        Thread th;
        bool running = false;

        Queue<BeepOptions> BeepQueue = new Queue<BeepOptions>();

        public void Add(int time, int freq)
        {
            BeepOptions b;
            b.time = time;
            b.freq = freq;
            lock (BeepQueue)
            {
                if (BeepQueue.Count < 10)
                {
                    BeepQueue.Enqueue(b);
                }
            }
        }

        public void Add()
        {
            // The default beep.
            Add(200, 800);
        }

        public BeepThreadClass()
        {
            running = true;
            th = new Thread(new ThreadStart(DoBeep));
            th.Name = "BeepThread";
            th.Start();
        }

        public void Stop()
        {
            th.Abort();
            running = false;
        }

        void DoBeep()
        {
            while (running)
            {
                if (BeepQueue.Count > 0)
                {
                    BeepOptions b = BeepQueue.Dequeue();
                    Console.Beep(b.freq, b.time);
                }
                try
                {
                    Thread.Sleep(1);
                }
                catch
                {
                    running = false;
                }
            }
        }
    }
}
