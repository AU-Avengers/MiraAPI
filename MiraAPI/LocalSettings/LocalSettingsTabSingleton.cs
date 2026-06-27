using System.Linq;

namespace MiraAPI.LocalSettings;

/// <summary>
/// Singleton for local settings tabs.
/// </summary>
/// <typeparam name="T">The settings tab type.</typeparam>
public static class LocalSettingsTabSingleton<T> where T : LocalSettingsTab
{
    private static T _instance;

    /// <summary>
    /// Gets the instance of the setting tab.
    /// </summary>
#pragma warning disable CA1000
    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;

            if (LocalSettingsManager.TypeToTab.TryGetValue(typeof(T), out var tab))
            {
                _instance = (T)(object)tab;
                return _instance;
            }

            return null;
        }
    }
#pragma warning restore CA1000
}
