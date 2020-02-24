using System.Collections.Generic;

namespace Fougerite.Events
{
    using System;
    using System.Timers;

    public class TimedEvent
    {
        private Dictionary<string, object> _args;
        private string _name;
        private System.Timers.Timer _timer;
        private long lastTick;
        private int _elapsedCount;

        public delegate void TimedEventFireDelegate(TimedEvent te);
        public event TimedEventFireDelegate OnFire;
        
        public TimedEvent(string name, double interval, bool autoreset = false)
        {
            this._name = name;
            this._timer = new Timer();
            this._timer.Interval = interval;
            this._timer.Elapsed += new ElapsedEventHandler(this._timer_Elapsed);
            this._elapsedCount = 0;
            this._timer.AutoReset = autoreset;
        }

        public TimedEvent(string name, double interval, bool autoreset, Dictionary<string, object> args)
            : this(name, interval)
        {
            this._timer.AutoReset = autoreset;
            _args = args;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (this.OnFire != null)
                {
                    this.OnFire(this);
                }
                this.lastTick = DateTime.UtcNow.Ticks;
                this._elapsedCount += 1;
            }
            catch (Exception ex)
            {
                Logger.LogError("Error occured at timer: " + this.Name + " Error: " + ex.ToString());
                this.Stop();
                Logger.LogDebug("Trying to restart timer.");
                this.Start();
                Logger.LogDebug("Restarted!");
            }
        }

        public void Start()
        {
            this._timer.Start();
            this.lastTick = DateTime.UtcNow.Ticks;
        }

        public void Stop()
        {
            this._timer.Stop();
        }

        public void Kill()
        {
            Stop();
            this._timer.Dispose();
        }

        public bool AutoReset
        {
            get { return this._timer.AutoReset; }
            set { this._timer.AutoReset = value; }
        }

        public Dictionary<string, object> Args
        {
            get
            {
                return this._args;
            }
            set
            {
                this._args = value;
            }
        }

        
        public double Interval
        {
            get
            {
                return this._timer.Interval;
            }
            set
            {
                this._timer.Interval = value;
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        public double TimeLeft
        {
            get
            {
                return (this.Interval - ((DateTime.UtcNow.Ticks - this.lastTick) / 0x2710L));
            }
        }
        
        public int ElapsedCount 
        {
            get
            {
                return this._elapsedCount;
            }
        }
    }
}