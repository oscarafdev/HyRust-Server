using System.Collections.Generic;

namespace Fougerite.PluginLoaders
{
    public interface IPluginLoader
    {
        string GetExtension();

        string GetSource(string path);

        string GetMainFilePath(string pluginname);

        string GetPluginDirectoryPath(string name);

        List<string> GetPluginNames();

        void LoadPlugin(string name);

        void LoadPlugins();

        void ReloadPlugin(string name);

        void ReloadPlugins();

        void UnloadPlugin(string name);

        void UnloadPlugins();
    }
}