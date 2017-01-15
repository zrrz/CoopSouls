using System;

/*! \cond PRIVATE */
// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class CgkCustomEvent {
        public string EventName;
        public string ProspectiveName;
		public bool IsEditing;
		// ReSharper disable InconsistentNaming
        public bool eventExpanded = true;
        public LevelSettings.EventReceiveMode eventRcvMode = LevelSettings.EventReceiveMode.Always;
        public LevelSettings.EventReceiveFilter eventRcvFilterMode = LevelSettings.EventReceiveFilter.All;
        public int filterModeQty = 1;
        public KillerFloat distanceThreshold = new KillerFloat(10, 0.1f, float.MaxValue);
        public int frameLastFired = -1;
        // ReSharper restore InconsistentNaming

        public CgkCustomEvent(string eventName) {
            EventName = eventName;
            ProspectiveName = eventName;
        }
    }
}
/*! \endcond */