using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;
using Whisper.Utils;

namespace Whisper.Samples
{
    /// <summary>
    /// Record audio clip from microphone and make a transcription.
    /// </summary>
    public class MyMicrophone : MonoBehaviour
    {
        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;
        public bool streamSegments = true;
        public bool printLanguage = true;

        public delegate void OnHolaDetected();
        public OnHolaDetected onHolaDetected;
        public delegate void OnAdiosDetected();
        public OnAdiosDetected onAdiosDetected;

        private string _buffer;

        private Regex holaRegex = new Regex(@"\bhola\b", RegexOptions.IgnoreCase);
        private Regex adiosRegex = new Regex(@"\badiós\b", RegexOptions.IgnoreCase);


        private void Awake()
        {
            whisper.OnNewSegment += OnNewSegment;

            microphoneRecord.OnRecordStop += OnRecordStop;
        }

        private void OnVadChanged(bool vadStop)
        {
            microphoneRecord.vadStop = vadStop;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Space) && !microphoneRecord.IsRecording)
            {
                microphoneRecord.StartRecord();
            } else if (Input.GetKey(KeyCode.S) && microphoneRecord.IsRecording)
            {
                microphoneRecord.StopRecord();
            }
        }
        
        private async void OnRecordStop(AudioChunk recordedAudio)
        {
            _buffer = "";

            var sw = new Stopwatch();
            sw.Start();
            
            var res = await whisper.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
            if (res == null) 
                return;

            var finalText = res.Result.ToLower();
            if (holaRegex.IsMatch(finalText))
            {
                onHolaDetected?.Invoke();
            }
            if (adiosRegex.IsMatch(finalText))
            {
                onAdiosDetected?.Invoke();
            }

            var time = sw.ElapsedMilliseconds;
            var rate = recordedAudio.Length / (time * 0.001f);
            UnityEngine.Debug.Log($"Time: {time} ms\nRate: {rate:F1}x");

            var text = res.Result;
            if (printLanguage)
                text += $"\n\nLanguage: {res.Language}";
            
            UnityEngine.Debug.Log($"Transcription result:\n{text}"); 
        }
        
        private void OnTranslateChanged(bool translate)
        {
            whisper.translateToEnglish = translate;
        }
        
        private void OnNewSegment(WhisperSegment segment)
        {
            if (!streamSegments)
                return;

            _buffer += segment.Text;
        }
    }
}