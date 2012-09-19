
using System;
using System.Linq;
using System.Threading;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace Ecslent2D.Input.Kinect
{

    public class RecognitionResult
    {
        public string word;
        public float likelihood;

    }

    public class AudioListener
    {
        public AudioCell cell;
        public bool done = false;
        public RecognizerInfo ri;
        public KinectAudioSource source;
        public AudioListener(AudioCell c, KinectAudioSource audioSource)
        {
            ri = GetKinectRecognizer();
            cell = c;
            c.Initialize(ri.Culture);
            source = audioSource;

        }
        public void Listen()
        {

            source.EchoCancellationMode = EchoCancellationMode.None; // No AEC for this sample
            source.AutomaticGainControlEnabled = false; // Important to turn this off for speech recognition
            RecognizerInfo ri = GetKinectRecognizer();
            var sre = new SpeechRecognitionEngine(ri.Id);
            sre.SpeechRecognized += SreSpeechRecognized;
            sre.SpeechHypothesized += SreSpeechHypothesized;
            sre.SpeechRecognitionRejected += SreSpeechRecognitionRejected;
            System.IO.Stream s = source.Start();
            sre.SetInputToAudioStream(s, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            while (!done)
            {
                Grammar newGrammar = cell.GetNewGrammar();
                if (newGrammar != null)
                    sre.LoadGrammar(newGrammar);
                sre.Recognize(new TimeSpan(0, 0, 0, 0, 500));

            }
        }


        private RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }
        private void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("Could not detect: {0} , {1}", e.Result.Text, e.Result.Confidence);
        }

        private void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Console.WriteLine("Hypothosized: {0}, {1}", e.Result.Text, e.Result.Confidence);

        }

        private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine("Recognized: {0}, {1}", e.Result.Text, e.Result.Confidence);
            cell.SetRecognitionResult(new RecognitionResult { word = e.Result.Text, likelihood = e.Result.Confidence });
        }
    }

    public class AudioCell
    {
        Choices permanentVocabulary;
        Choices newVocabulary;
        RecognitionResult result;
        bool initialized = false;
        System.Globalization.CultureInfo culture;

        public AudioCell()
        {

        }
        public void Initialize(System.Globalization.CultureInfo cult)
        {
            culture = (System.Globalization.CultureInfo)cult.Clone();

            permanentVocabulary = new Choices("quit", "help");
            newVocabulary = null;
        }
        public Grammar GetNewGrammar()
        {
            if (Monitor.TryEnter(this))  // Enter synchronization block
            {
                Grammar g = null;

                try
                {
                    if (!initialized)
                    {
                        initialized = true;
                        GrammarBuilder gb = new GrammarBuilder() { Culture = culture };
                        gb.Append(permanentVocabulary);
                        g = new Grammar(gb);
                    }
                    else if (newVocabulary != null)
                    {
                        newVocabulary.Add(permanentVocabulary);

                        GrammarBuilder gb = new GrammarBuilder() { Culture = culture };
                        gb.Append(newVocabulary);
                        newVocabulary = null;
                        g = new Grammar(gb);
                    }

                }
                finally
                {
                    Monitor.Exit(this);
                }
                return g;
            }

            else
                return null;
        }

        public void SetNewVocabulary(Choices newG)
        {
            Monitor.Enter(this);
            try
            {
                newVocabulary = newG;
            }
            finally
            {
                Monitor.Exit(this);
            }

        }

        public RecognitionResult GetRecognitionResult()
        {
            if (Monitor.TryEnter(this))  // Enter synchronization block
            {

                RecognitionResult res = null;
                try
                {
                    if (result != null)
                        res = new RecognitionResult { word = result.word, likelihood = result.likelihood };
                    result = null;
                }

                finally
                {
                    Monitor.Exit(this);
                }
                return res;
            }

            else
                return null;
        }

        public void SetRecognitionResult(RecognitionResult newResult)
        {
            Monitor.Enter(this);
            try
            {
                result = newResult;
            }
            finally
            {
                Monitor.Exit(this);
            }

        }
    }
}
