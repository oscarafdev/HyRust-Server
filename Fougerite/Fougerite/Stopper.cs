using System;
using System.Diagnostics;
using Fougerite.PluginLoaders;

namespace Fougerite
{
    public class Stopper : CountedInstance, IDisposable
    {
        private readonly string Type;
        private readonly string Method;
        private readonly long WarnTimeMS;
        private readonly Stopwatch stopper;

        public Stopper(string type, string method, float warnSecs = 0.1f)
        {
            Type = type;
            Method = method;
            WarnTimeMS = (long)(warnSecs * 1000);
            stopper = Stopwatch.StartNew();
        }

        void IDisposable.Dispose()
        {

            if (stopper.ElapsedMilliseconds > WarnTimeMS) {
                Logger.LogWarning(string.Format("[{0}.{1}] Took: {2}s ({3}ms)",
                    Type,
                    Method,
                    stopper.Elapsed.Seconds,
                    stopper.ElapsedMilliseconds
                ));
            }
        }
    }
}