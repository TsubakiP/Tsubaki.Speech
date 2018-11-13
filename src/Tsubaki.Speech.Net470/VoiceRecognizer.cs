// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Tsubaki.Speech
{
    using Google.Cloud.Speech.V1;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;

    public sealed class VoiceRecognizer : SoundRecorder
    {
        private const string GOOGLE_APPLICATION_CREDENTIALS = "GOOGLE_APPLICATION_CREDENTIALS";
        private readonly RecognitionConfig _config;
        private SpeechClient _speech;

        /// <summary>
        /// </summary>
        public event Action<IReadOnlyList<SpeechRecognitionAlternative>> Responsed;

        /// <summary>
        /// </summary>
        /// <param name="secret"></param>
        public VoiceRecognizer(string secret)
        {
            this._config = new RecognitionConfig
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 16000,
                LanguageCode = Thread.CurrentThread.CurrentCulture.Name,
            };

            this.Init(secret);
        }

        protected override void OnStart(object sender, EventArgs e)
        {
            if (this._speech is null)
            {
                return;
            }
        }

        protected override void OnStop(object sender, StoppedEventArgs e)
        {
            if (this._speech is null)
            {
                return;
            }
            var wavStream = RecognitionAudio.FromStream(e.WaveStream);
            var results = this._speech.Recognize(this._config, wavStream).Results;
            var alternatives = results.SelectMany(r => r.Alternatives);

            Debug.WriteLine("\n========== Results ==========");
            foreach (var alternative in alternatives)
            {
                Debug.WriteLine(alternative.Transcript + $"[{alternative.Confidence}]");
            }
            Debug.WriteLine("=============================\n");

            this.Responsed?.Invoke(alternatives.ToList());
        }

        private void Init(string secret)
        {
            const EnvironmentVariableTarget Target = EnvironmentVariableTarget.User;
            try
            {
                var file = new FileInfo(secret);
                if (!file.Exists)
                    throw new FileNotFoundException("Can't found client-secret file");
                Environment.SetEnvironmentVariable(GOOGLE_APPLICATION_CREDENTIALS, file.FullName, Target);
                this._speech = SpeechClient.Create();
            }
            catch
            {
                Environment.SetEnvironmentVariable(GOOGLE_APPLICATION_CREDENTIALS, null, Target);
                throw;
            }
        }
    }
}