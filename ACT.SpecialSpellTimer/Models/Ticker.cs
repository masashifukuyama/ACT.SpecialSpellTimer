﻿namespace ACT.SpecialSpellTimer.Models
{
    using System;
    using System.Text.RegularExpressions;
    using System.Timers;
    using System.Xml.Serialization;

    using ACT.SpecialSpellTimer.Sound;
    using ACT.SpecialSpellTimer.Utility;

    /// <summary>
    /// ワンポイントテロップ
    /// </summary>
    [Serializable]
    public class OnePointTelop : IDisposable
    {
        [XmlIgnore]
        private Timer delayedSoundTimer;

        public OnePointTelop()
        {
            this.guid = Guid.Empty;
            this.Title = string.Empty;
            this.Keyword = string.Empty;
            this.KeywordToHide = string.Empty;
            this.Message = string.Empty;
            this.MatchSound = string.Empty;
            this.MatchTextToSpeak = string.Empty;
            this.DelaySound = string.Empty;
            this.DelayTextToSpeak = string.Empty;
            this.BackgroundColor = string.Empty;
            this.FontFamily = string.Empty;
            this.FontColor = string.Empty;
            this.FontOutlineColor = string.Empty;
            this.MatchedLog = string.Empty;
            this.MessageReplaced = string.Empty;
            this.RegexPattern = string.Empty;
            this.RegexPatternToHide = string.Empty;
            this.JobFilter = string.Empty;
            this.ZoneFilter = string.Empty;
            this.TimersMustRunningForStart = new Guid[0];
            this.TimersMustStoppingForStart = new Guid[0];
            this.Font = new FontInfo();
            this.KeywordReplaced = string.Empty;
            this.KeywordToHideReplaced = string.Empty;

            // ディレイサウンドタイマをセットする
            this.delayedSoundTimer = new Timer
            {
                AutoReset = false,
                Enabled = false
            };

            this.delayedSoundTimer.Elapsed += this.DelayedSoundTimer_Elapsed;
        }

        public bool AddMessageEnabled { get; set; }

        public int BackgroundAlpha { get; set; }

        public string BackgroundColor { get; set; }

        public double Delay { get; set; }

        [XmlIgnore]
        public bool Delayed { get; set; }

        public string DelaySound { get; set; }

        public string DelayTextToSpeak { get; set; }

        public double DisplayTime { get; set; }

        public bool Enabled { get; set; }

        public FontInfo Font { get; set; }

        public string FontColor { get; set; }

        public string FontFamily { get; set; }

        public string FontOutlineColor { get; set; }

        public float FontSize { get; set; }

        public int FontStyle { get; set; }

        [XmlIgnore]
        public bool ForceHide { get; set; }

        public Guid guid { get; set; }

        public long ID { get; set; }

        public string JobFilter { get; set; }

        public string Keyword { get; set; }

        [XmlIgnore]
        public string KeywordReplaced { get; set; }

        public string KeywordToHide { get; set; }

        [XmlIgnore]
        public string KeywordToHideReplaced { get; set; }

        public double Left { get; set; }

        [XmlIgnore]
        public DateTime MatchDateTime { get; set; }

        [XmlIgnore]
        public string MatchedLog { get; set; }

        public string MatchSound { get; set; }

        public string MatchTextToSpeak { get; set; }

        public string Message { get; set; }

        [XmlIgnore]
        public string MessageReplaced { get; set; }

        public bool ProgressBarEnabled { get; set; }

        [XmlIgnore]
        public Regex Regex { get; set; }

        public bool RegexEnabled { get; set; }

        [XmlIgnore]
        public string RegexPattern { get; set; }

        [XmlIgnore]
        public string RegexPatternToHide { get; set; }

        [XmlIgnore]
        public Regex RegexToHide { get; set; }

        public Guid[] TimersMustRunningForStart { get; set; }

        public Guid[] TimersMustStoppingForStart { get; set; }

        public string Title { get; set; }

        public double Top { get; set; }

        public string ZoneFilter { get; set; }

        public void Dispose()
        {
            if (this.delayedSoundTimer != null)
            {
                this.delayedSoundTimer.Stop();
                this.delayedSoundTimer.Dispose();
                this.delayedSoundTimer = null;
            }

            GC.SuppressFinalize(true);
        }

        /// <summary>
        /// ディレイサウンドのタイマを開始する
        /// </summary>
        public void StartDelayedSoundTimer()
        {
            var timer = this.delayedSoundTimer;

            if (timer == null)
            {
                return;
            }

            if (timer.Enabled)
            {
                timer.Stop();
            }

            if (this.Delay <= 0 ||
                this.MatchDateTime <= DateTime.MinValue)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(this.DelaySound) &&
                string.IsNullOrWhiteSpace(this.DelayTextToSpeak))
            {
                return;
            }

            // タイマをセットする
            var timeToPlay = this.MatchDateTime.AddSeconds(this.Delay);
            var duration = (timeToPlay - DateTime.Now).TotalMilliseconds;

            if (duration > 0d)
            {
                // タイマスタート
                timer.Interval = duration;
                timer.Start();
            }
        }

        private void DelayedSoundTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Delayed = true;

            var regex = this.Regex;
            var wave = this.DelaySound;
            var speak = this.DelayTextToSpeak;

            SoundController.Default.Play(this.DelaySound);

            if (!string.IsNullOrWhiteSpace(this.DelayTextToSpeak))
            {
                if (regex == null ||
                    !speak.Contains("$"))
                {
                    SoundController.Default.Play(speak);
                    return;
                }

                var match = regex.Match(this.MatchedLog);
                speak = match.Result(speak);

                SoundController.Default.Play(speak);
            }
        }
    }
}