namespace Fougerite.PluginLoaders
{
    public interface ISingleton
    {
        bool CheckDependencies();

        void Initialize();
    }
}