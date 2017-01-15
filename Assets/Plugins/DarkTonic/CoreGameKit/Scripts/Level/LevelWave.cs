/*! \cond PRIVATE */
using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class LevelWave {
        // ReSharper disable InconsistentNaming
        public LevelSettings.WaveType waveType = LevelSettings.WaveType.Timed;
        public LevelSettings.SkipWaveMode skipWaveType = LevelSettings.SkipWaveMode.None;
        public WorldVariableCollection skipWavePassCriteria = new WorldVariableCollection();
        public bool pauseGlobalWavesWhenCompleted;
        public string waveName = "UNNAMED";
        public string waveClass = "None";
        public LevelWaveMusicSettings musicSettings = new LevelWaveMusicSettings();
        public int WaveDuration = 5;
        public bool endEarlyIfAllDestroyed;
        public bool waveBeatBonusesEnabled;
        public bool useCompletionEvents;
        public LevelSettings.WaveSpawnerUseMode spawnerUseMode = LevelSettings.WaveSpawnerUseMode.AllAbove;
        public int spawnersToUseMin = 1;
        public int spawnersToUseMax = 1;
        public bool isDummyWave;

        public List<CGKCustomEventToFire> completionCustomEvents = new List<CGKCustomEventToFire>();

        public WorldVariableCollection waveDefeatVariableModifiers = new WorldVariableCollection();
        public bool isExpanded = true;
        public int sequencedWaveNumber = 0;
        public int randomWaveNumber = 0; // assigned and only used for random sorting.
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Call this method to clone the wave (used by clone icon in Inspector)
        /// </summary>
        /// <returns>A deep copy of the wave.</returns>
        public LevelWave Clone() {
            var clone = new LevelWave {
                waveType = waveType,
                skipWaveType = skipWaveType,
                skipWavePassCriteria = skipWavePassCriteria,
                waveName = waveName,
                musicSettings = musicSettings,
                WaveDuration = WaveDuration,
                endEarlyIfAllDestroyed = endEarlyIfAllDestroyed,
                waveBeatBonusesEnabled = waveBeatBonusesEnabled,
                useCompletionEvents = useCompletionEvents,
                completionCustomEvents = completionCustomEvents,
                waveDefeatVariableModifiers = waveDefeatVariableModifiers,
                isExpanded = isExpanded
            };


            return clone;
        }
    }
}
/*! \endcond */