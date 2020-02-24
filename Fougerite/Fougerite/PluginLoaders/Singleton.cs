using System;

namespace Fougerite.PluginLoaders
{
    public abstract class Singleton<T> : CountedInstance where T : ISingleton
    {
        private static T Instance;

        public static T GetInstance()
        {
            return Singleton<T>.Instance;
        }

        static Singleton()
        {
            Singleton<T>.Instance = Activator.CreateInstance<T>();
            if (Singleton<T>.Instance.CheckDependencies())
            {
                Singleton<T>.Instance.Initialize();
            }
            else
            {
                Logger.LogWarning(Instance.GetType() + " is disabled in the Fougerite.cfg, and will not load any plugins.");
            }

        }
    }
}