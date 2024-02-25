using System.Collections.Generic;

namespace Mamboo.Internal.Analytics
{
    internal static class AnalyticsManager
    {
        private static readonly List<IAnalyticsProvider> _analyticsProviders = new List<IAnalyticsProvider>()
        {
            new AdjustAnalyticsProvider(), new GameAnalyticsProvider(), new FacebookAnalyticsProvider()
        };

        internal static GameGenre GameGenre { private set; get; }

        internal static void Initialize(MambooSettings mambooSettings)
        {
            _analyticsProviders.ForEach(provider => provider.Initialize());

            GameGenre = mambooSettings.gameGenre;
        }
    }
}