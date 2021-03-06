/*! \cond PRIVATE */
using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class WaveSpecifics {
        // ReSharper disable InconsistentNaming
        public bool isExpanded = true;
        public bool enableWave = true;
        public bool visualizeWave = true;
        public int SpawnLevelNumber;
        public int SpawnWaveNumber;
        public KillerInt MinToSpwn = new KillerInt(1, 0, int.MaxValue);
        public KillerInt MaxToSpwn = new KillerInt(2, 0, int.MaxValue);
        public KillerFloat WaveDelaySec = new KillerFloat(0f, 0f, float.MaxValue);
        public KillerFloat TimeToSpawnEntireWave = new KillerFloat(3f, 0f, float.MaxValue);
        public Transform prefabToSpawn;
        public string prefabToSpawnCategoryName;
        public SpawnOrigin spawnSource = SpawnOrigin.Specific;
        public int prefabPoolIndex;
        public string prefabPoolName = "";
        public bool repeatWaveUntilNew;
        public int waveCompletePercentage = 100;

        public RepeatWaveMode curWaveRepeatMode = RepeatWaveMode.NumberOfRepetitions;
        public TimedRepeatWaveMode curTimedRepeatWaveMode = TimedRepeatWaveMode.EliminationStyle;
        public KillerFloat repeatPauseMinimum = new KillerFloat(0f, 0f, float.MaxValue);
        public KillerFloat repeatPauseMaximum = new KillerFloat(0f, 0f, float.MaxValue);
        public KillerInt repeatItemInc = new KillerInt(0, -100, 100);
        public KillerFloat repeatTimeInc = new KillerFloat(0f, float.MinValue, float.MaxValue);
        public KillerInt repeatItemLmt = new KillerInt(100, 0, 1000);
        public KillerFloat repeatTimeLmt = new KillerFloat(100f, .1f, float.MaxValue);

        public KillerInt repetitionsToDo = new KillerInt(2, 2, int.MaxValue);
        public WorldVariableCollection repeatPassCriteria = new WorldVariableCollection();

        public bool waveRepeatBonusesEnabled;
        public WorldVariableCollection waveRepeatVariableModifiers = new WorldVariableCollection();
        public bool waveRepeatFireEvents;
        public List<CGKCustomEventToFire> waveRepeatCustomEvents = new List<CGKCustomEventToFire>();

        public bool positionExpanded = true;
        public PositionMode positionXmode = PositionMode.SpawnerPosition;
        public PositionMode positionYmode = PositionMode.SpawnerPosition;
        public PositionMode positionZmode = PositionMode.SpawnerPosition;
        public KillerFloat customPosX = new KillerFloat(0f, float.MinValue, float.MaxValue);
        public KillerFloat customPosY = new KillerFloat(0f, float.MinValue, float.MaxValue);
        public KillerFloat customPosZ = new KillerFloat(0f, float.MinValue, float.MaxValue);
        public Transform otherObjectX;
        public Transform otherObjectY;
        public Transform otherObjectZ;

        public RotationMode curRotationMode = RotationMode.UsePrefabRotation;
        public Vector3 customRotation = Vector3.zero;

        public bool enableLimits;
        public KillerFloat doNotSpawnIfMbrCloserThan = new KillerFloat(5f, 0.1f, float.MaxValue);
        public KillerFloat doNotSpawnRandomDist = new KillerFloat(0f, .1f, float.MaxValue);

        public bool enableRandomizations;
        public bool randomXRotation;
        public bool randomYRotation;
        public bool randomZRotation;
        public KillerFloat randomDistX = new KillerFloat(0f, 0f, TriggeredSpawner.MaxDistance);
        public KillerFloat randomDistY = new KillerFloat(0f, 0f, TriggeredSpawner.MaxDistance);
        public KillerFloat randomDistZ = new KillerFloat(0f, 0f, TriggeredSpawner.MaxDistance);

        public KillerFloat randomXRotMin = new KillerFloat(0f, 0f, 360f);
        public KillerFloat randomXRotMax = new KillerFloat(360f, 0f, 360f);
        public KillerFloat randomYRotMin = new KillerFloat(0f, 0f, 360f);
        public KillerFloat randomYRotMax = new KillerFloat(360f, 0f, 360f);
        public KillerFloat randomZRotMin = new KillerFloat(0f, 0f, 360f);
        public KillerFloat randomZRotMax = new KillerFloat(360f, 0f, 360f);

        public bool enableIncrements;
        public bool enableKeepCenter;
        public KillerFloat incrementPositionX = new KillerFloat(0f, float.MinValue, float.MaxValue);
        public KillerFloat incrementPositionY = new KillerFloat(0f, float.MinValue, float.MaxValue);
        public KillerFloat incrementPositionZ = new KillerFloat(0f, float.MinValue, float.MaxValue);
        public KillerFloat incrementRotX = new KillerFloat(0f, -180f, 180f);
        public KillerFloat incrementRotY = new KillerFloat(0f, -180f, 180f);
        public KillerFloat incrementRotZ = new KillerFloat(0f, -180f, 180f);

        public WaveOffsetChoiceMode offsetChoiceMode = WaveOffsetChoiceMode.RandomlyChosen;
        public List<Vector3> waveOffsetList = new List<Vector3>();

        public bool enablePostSpawnNudge;
        public KillerFloat postSpawnNudgeFwd = new KillerFloat(0f, float.MinValue, float.MaxValue);
        public KillerFloat postSpawnNudgeRgt = new KillerFloat(0f, float.MinValue, float.MaxValue);
        public KillerFloat postSpawnNudgeDwn = new KillerFloat(0f, float.MinValue, float.MaxValue);

        private int waveOffsetIndex;
        // ReSharper restore InconsistentNaming

        public enum WaveOffsetChoiceMode {
            RandomlyChosen,
            UseInOrder
        }

        public enum RepeatWaveMode {
            Endless,
            NumberOfRepetitions,
            UntilWorldVariableAbove,
            UntilWorldVariableBelow
        }

        public enum TimedRepeatWaveMode {
            EliminationStyle,
            StrictTimeStyle
        }

        public enum SpawnOrigin {
            Specific,
            PrefabPool
        }

        public enum PositionMode {
            SpawnerPosition,
            CustomPosition,
            OtherObjectPosition
        }

        public enum SpawnerRotationMode {
            KeepRotation,
            LookAtCustomEventOrigin
        }

        public enum RotationMode {
            UsePrefabRotation,
            UseSpawnerRotation,
            CustomRotation,
            LookAtCustomEventOrigin
        }

        public bool IsValid {
            get {
                if (!enableWave) {
                    return false;
                }

                if (repeatPauseMinimum.Value > repeatPauseMaximum.Value) {
                    return false;
                }

                if (MinToSpwn.Value > MaxToSpwn.Value) {
                    return false;
                }

                return true;
            }
        }

        public Vector3 WaveOffset {
            get {
                if (waveOffsetList.Count == 0) {
                    waveOffsetList.Add(Vector3.zero);
                    return Vector3.zero;
                }

                var index = 0;

                switch (offsetChoiceMode) {
                    case WaveOffsetChoiceMode.RandomlyChosen:
                        index = UnityEngine.Random.Range(0, waveOffsetList.Count);
                        break;
                    case WaveOffsetChoiceMode.UseInOrder:
                        index = waveOffsetIndex;
                        waveOffsetIndex++;
                        if (waveOffsetIndex >= waveOffsetList.Count) {
                            waveOffsetIndex = 0;
                        }
                        break;
                }

                return waveOffsetList[index];
            }
        }

        public WaveSpecifics Clone() {
            var clone = new WaveSpecifics {
                isExpanded = isExpanded,
                enableWave = enableWave,
                visualizeWave = visualizeWave,
                SpawnLevelNumber = SpawnLevelNumber,
                SpawnWaveNumber = SpawnWaveNumber,
                MinToSpwn = MinToSpwn,
                MaxToSpwn = MaxToSpwn,
                WaveDelaySec = WaveDelaySec,
                TimeToSpawnEntireWave = TimeToSpawnEntireWave,
                prefabToSpawn = prefabToSpawn,
                spawnSource = spawnSource,
                prefabPoolIndex = prefabPoolIndex,
                prefabPoolName = prefabPoolName,
                repeatWaveUntilNew = repeatWaveUntilNew,
                waveCompletePercentage = waveCompletePercentage,

                curWaveRepeatMode = curWaveRepeatMode,
                curTimedRepeatWaveMode = curTimedRepeatWaveMode,
                repeatPauseMinimum = repeatPauseMinimum,
                repeatPauseMaximum = repeatPauseMaximum,
                repeatItemInc = repeatItemInc,
                repeatTimeInc = repeatTimeInc,
                repeatItemLmt = repeatItemLmt,
                repeatTimeLmt = repeatTimeLmt,
                repetitionsToDo = repetitionsToDo,
                repeatPassCriteria = repeatPassCriteria,

                waveRepeatBonusesEnabled = waveRepeatBonusesEnabled,
                waveRepeatVariableModifiers = waveRepeatVariableModifiers,
                waveRepeatFireEvents = waveRepeatFireEvents,
                waveRepeatCustomEvents = waveRepeatCustomEvents,

                positionExpanded = positionExpanded,
                positionXmode = positionXmode,
                positionYmode = positionYmode,
                positionZmode = positionZmode,
                customPosX = customPosX,
                customPosY = customPosY,
                customPosZ = customPosZ,

                curRotationMode = curRotationMode,
                customRotation = customRotation,

                enableLimits = enableLimits,
                doNotSpawnIfMbrCloserThan = doNotSpawnIfMbrCloserThan,
                doNotSpawnRandomDist = doNotSpawnRandomDist,

                enableRandomizations = enableRandomizations,
                randomXRotation = randomXRotation,
                randomYRotation = randomYRotation,
                randomZRotation = randomZRotation,
                randomDistX = randomDistX,
                randomDistY = randomDistY,
                randomDistZ = randomDistZ,

                randomXRotMin = randomXRotMin,
                randomXRotMax = randomXRotMax,
                randomYRotMin = randomYRotMin,
                randomYRotMax = randomYRotMax,
                randomZRotMin = randomZRotMin,
                randomZRotMax = randomZRotMax,

                enableIncrements = enableIncrements,
                enableKeepCenter = enableKeepCenter,
                incrementPositionX = incrementPositionX,
                incrementPositionY = incrementPositionY,
                incrementPositionZ = incrementPositionZ,
                incrementRotX = incrementRotX,
                incrementRotY = incrementRotY,
                incrementRotZ = incrementRotZ,

                waveOffsetList = waveOffsetList,

                enablePostSpawnNudge = enablePostSpawnNudge,
                postSpawnNudgeFwd = postSpawnNudgeFwd,
                postSpawnNudgeRgt = postSpawnNudgeRgt,
                postSpawnNudgeDwn = postSpawnNudgeDwn
            };

            return clone;
        }
    }
}
/*! \endcond */
