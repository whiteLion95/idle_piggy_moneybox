
using Mamboo.Analytics.Adjust.Internal;

namespace Mamboo.Internal.Analytics
{
    internal class AdjustAnalyticsProvider : IAnalyticsProvider
    {
        public void Initialize()
        {
            AdjustWrapper.Initialize();
        }
    }
}