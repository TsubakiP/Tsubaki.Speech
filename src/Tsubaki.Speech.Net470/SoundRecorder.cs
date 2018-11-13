// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D
namespace Tsubaki.Speech
{
    using NAudio.Wave;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    public class SoundRecorder
    {
        public sealed class StoppedEventArgs : EventArgs
        {
            public WaveStream WaveStream { get; }

            internal StoppedEventArgs(WaveStream stream)
                => this.WaveStream = stream;
        }

        private readonly List<byte> _raw;
        private readonly WaveInEvent _wave;

        private bool _started = false;

        protected event EventHandler RecordingStarted;

        protected event EventHandler<StoppedEventArgs> RecordingStopped;

        public SoundRecorder()
        {
            this._raw = new List<byte>();
            this._wave = new WaveInEvent
            {
                WaveFormat = new WaveFormat(16000, 1)
            };

            this._wave.DataAvailable += (sender, e) => this._raw.AddRange(e.Buffer);
            this.RecordingStarted = OnStart;
            this.RecordingStopped = OnStop;
        }

        public void Start()
        {
            if (this._started)
                return;
            this._started = true;
            this._raw.Clear();
            this._wave.StartRecording();
            this.RecordingStarted?.Invoke(this, EventArgs.Empty);
            Debug.WriteLine("Recording started");
        }

        public void Stop()
        {
            if (!this._started)
                return;
            this._started = false;
            this._wave.StopRecording();
            var array = this._raw.ToArray();
            var ms = new MemoryStream(array);
            var wavStream = new RawSourceWaveStream(ms, this._wave.WaveFormat);
            this.RecordingStopped?.Invoke(this, new StoppedEventArgs(wavStream));

            Debug.WriteLine("Recording stopped");
        }

        protected virtual void OnStart(object sender, EventArgs e)
        {
        }

        protected virtual void OnStop(object sender, StoppedEventArgs e)
        {
        }
    }
}