using System;
using System.Collections.Generic;
using DarkTonic.CoreGameKit;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using RelationsInspector.Backend.CoreGameKit;

[CustomEditor(typeof(LevelSettings))]
// ReSharper disable once CheckNamespace
public class LevelSettingsInspector : Editor {
    private LevelSettings _settings;
    private bool _isDirty;

    // ReSharper disable once FunctionComplexityOverflow
    public override void OnInspectorGUI() {
        EditorGUI.indentLevel = 0;

        _settings = (LevelSettings)target;

        var isInProjectView = DTInspectorUtility.IsPrefabInProjectView(_settings);

        WorldVariableTracker.ClearInGamePlayerStats();

        DTInspectorUtility.DrawTexture(CoreGameKitInspectorResources.LogoTexture);

        _isDirty = false;

        if (isInProjectView) {
            DTInspectorUtility.ShowRedErrorBox("You have selected the LevelWaveSettings prefab in Project View.");
            DTInspectorUtility.ShowRedErrorBox("Do not drag this prefab into the Scene. It will be linked to this prefab if you do. Click the button below to create a LevelWaveSettings prefab in the Scene.");

            EditorGUILayout.Separator();

            GUI.contentColor = DTInspectorUtility.BrightButtonColor;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("Create LevelWaveSettings Prefab", EditorStyles.toolbarButton, GUILayout.Width(180))) {
                CreateLevelSettingsPrefab();
            }
            EditorGUILayout.EndHorizontal();
            GUI.contentColor = Color.white;
            return;
        }

        var allStats = KillerVariablesHelper.AllStatNames;

        var playerStatsHolder = _settings.transform.FindChild(LevelSettings.WorldVariablesContainerTransName);
        if (playerStatsHolder == null) {
            Debug.LogError("You have no child prefab of LevelSettings called '" + LevelSettings.WorldVariablesContainerTransName + "'. " + LevelSettings.RevertLevelSettingsAlert);
            DTInspectorUtility.ShowRedErrorBox("Please check the console. You have a breaking error.");
            return;
        }

        EditorGUI.indentLevel = 0;

        DTInspectorUtility.StartGroupHeader();

        var newUseWaves = EditorGUILayout.BeginToggleGroup(" Use Global Waves", _settings.useWaves);
        if (newUseWaves != _settings.useWaves) {
            if (Application.isPlaying) {
                DTInspectorUtility.ShowAlert("Cannot change this setting at runtime.");
            } else {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Use Global Waves");
                _settings.useWaves = newUseWaves;
            }
        }
        DTInspectorUtility.EndGroupHeader();

        if (_settings.useWaves) {
            EditorGUI.indentLevel = 0;

            DTInspectorUtility.StartGroupHeader(1);
            var newUseMusic = GUILayout.Toggle(_settings.useMusicSettings, " Use Music Settings");
            if (newUseMusic != _settings.useMusicSettings) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Use Music Settings");
                _settings.useMusicSettings = newUseMusic;
            }
            EditorGUILayout.EndVertical();

            if (_settings.useMusicSettings) {
                EditorGUI.indentLevel = 0;

                var newGoMusic = (LevelSettings.WaveMusicMode)EditorGUILayout.EnumPopup("G.O. Music Mode", _settings.gameOverMusicSettings.WaveMusicMode);
                if (newGoMusic != _settings.gameOverMusicSettings.WaveMusicMode) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change G.O. Music Mode");
                    _settings.gameOverMusicSettings.WaveMusicMode = newGoMusic;
                }
                if (_settings.gameOverMusicSettings.WaveMusicMode == LevelSettings.WaveMusicMode.PlayNew) {
                    var newWaveMusic = (AudioClip)EditorGUILayout.ObjectField("G.O. Music", _settings.gameOverMusicSettings.WaveMusic, typeof(AudioClip), true);
                    if (newWaveMusic != _settings.gameOverMusicSettings.WaveMusic) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "assign G.O. Music");
                        _settings.gameOverMusicSettings.WaveMusic = newWaveMusic;
                    }
                }
                if (_settings.gameOverMusicSettings.WaveMusicMode != LevelSettings.WaveMusicMode.Silence) {
                    var newMusicVol = EditorGUILayout.Slider("G.O. Music Volume", _settings.gameOverMusicSettings.WaveMusicVolume, 0f, 1f);
                    if (newMusicVol != _settings.gameOverMusicSettings.WaveMusicVolume) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change G.O. Music Volume");
                        _settings.gameOverMusicSettings.WaveMusicVolume = newMusicVol;
                    }
                } else {
                    var newFadeTime = EditorGUILayout.Slider("Silence Fade Time", _settings.gameOverMusicSettings.FadeTime, 0f, 15f);
                    if (newFadeTime != _settings.gameOverMusicSettings.FadeTime) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Silence Fade Time");
                        _settings.gameOverMusicSettings.FadeTime = newFadeTime;
                    }
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUI.indentLevel = 0;

            DTInspectorUtility.AddSpaceForNonU5();
            DTInspectorUtility.StartGroupHeader(1);
            var newEnableWarp = GUILayout.Toggle(_settings.enableWaveWarp, " Custom Start Wave?");
            if (newEnableWarp != _settings.enableWaveWarp) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Custom Start Wave?");
                _settings.enableWaveWarp = newEnableWarp;
            }
            EditorGUILayout.EndVertical();

            if (_settings.enableWaveWarp) {
                EditorGUI.indentLevel = 0;

                KillerVariablesHelper.DisplayKillerInt(ref _isDirty, _settings.startLevelNumber, "Custom Start Level#", _settings);
                KillerVariablesHelper.DisplayKillerInt(ref _isDirty, _settings.startWaveNumber, "Custom Start Wave#", _settings);
            }
            EditorGUILayout.EndVertical();
            DTInspectorUtility.ResetColors();

            var newDisableSyncro = EditorGUILayout.Toggle("Syncro Spawners Off", _settings.disableSyncroSpawners);
            if (newDisableSyncro != _settings.disableSyncroSpawners) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Syncro Spawners Off");
                _settings.disableSyncroSpawners = newDisableSyncro;
            }

            var newStart = EditorGUILayout.Toggle("Auto Start Waves", _settings.startFirstWaveImmediately);
            if (newStart != _settings.startFirstWaveImmediately) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Auto Start Waves");
                _settings.startFirstWaveImmediately = newStart;
            }

            var newDestroy = (LevelSettings.WaveRestartBehavior)EditorGUILayout.EnumPopup("Wave Restart Mode", _settings.waveRestartMode);
            if (newDestroy != _settings.waveRestartMode) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Wave Restart Mode");
                _settings.waveRestartMode = newDestroy;
            }
        }

        EditorGUILayout.EndToggleGroup();

        DTInspectorUtility.AddSpaceForNonU5();

        DTInspectorUtility.StartGroupHeader();
        var newUse = EditorGUILayout.BeginToggleGroup(" Use Initialization Options", _settings.initializationSettingsExpanded);
        if (newUse != _settings.initializationSettingsExpanded) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Use Initialization Options");
            _settings.initializationSettingsExpanded = newUse;
        }

        if (_settings.initializationSettingsExpanded) {
            DTInspectorUtility.BeginGroupedControls();
            DTInspectorUtility.ShowColorWarningBox("When LevelSettings has finished initializing, fire the Custom Events below");

            EditorGUILayout.BeginHorizontal();
            GUI.contentColor = DTInspectorUtility.AddButtonColor;
            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("Add", "Click to add a Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50))) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Add Initialization Custom Event");
                _settings.initializationCustomEvents.Add(new CGKCustomEventToFire());
            }
            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50))) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Remove Initialization Custom Event");
                _settings.initializationCustomEvents.RemoveAt(_settings.initializationCustomEvents.Count - 1);
            }
            GUI.contentColor = Color.white;

            EditorGUILayout.EndHorizontal();

            if (_settings.initializationCustomEvents.Count == 0) {
                DTInspectorUtility.ShowColorWarningBox("You have no Custom Events selected to fire.");
            }

            DTInspectorUtility.VerticalSpace(2);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _settings.initializationCustomEvents.Count; i++) {
                var anEvent = _settings.initializationCustomEvents[i].CustomEventName;

                anEvent = DTInspectorUtility.SelectCustomEventForVariable(ref _isDirty, anEvent, _settings, "Custom Event");

                if (anEvent == _settings.initializationCustomEvents[i].CustomEventName) {
                    continue;
                }

                _settings.initializationCustomEvents[i].CustomEventName = anEvent;
            }

            DTInspectorUtility.EndGroupedControls();
        }
        EditorGUILayout.EndToggleGroup();
        DTInspectorUtility.EndGroupHeader();

        EditorGUI.indentLevel = 0;

        if (!Application.isPlaying) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Wave Visualizations", GUILayout.Width(120));
			GUI.contentColor = DTInspectorUtility.BrightButtonColor;
			GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Refresh All", "Refresh all wave visualizations"), EditorStyles.toolbarButton, GUILayout.Width(70))) {
                var syncros = FindObjectsOfType(typeof(WaveSyncroPrefabSpawner));
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var j = 0; j < syncros.Length; j++) {
                    var aSpawner = (WaveSyncroPrefabSpawner)syncros[j];
                    aSpawner.gameObject.DestroyChildrenImmediateWithMarker();

                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var w = 0; w < aSpawner.waveSpecs.Count; w++) {
                        var aWave = aSpawner.waveSpecs[w];
                        if (!aWave.visualizeWave) {
                            continue;
                        }

                        aSpawner.SpawnWaveVisual(aWave);
                        break;
                    }
                }

                var trigSpawners = FindObjectsOfType(typeof(TriggeredSpawner));
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var t = 0; t < trigSpawners.Length; t++) {
                    var trigSpawner = (TriggeredSpawner)trigSpawners[t];

                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var w = 0; w < trigSpawner.AllWaves.Count; w++) {
                        var wave = trigSpawner.AllWaves[w];
                        if (!wave.enableWave || !wave.visualizeWave) {
                            continue;
                        }

                        trigSpawner.gameObject.DestroyChildrenImmediateWithMarker();
                        trigSpawner.SpawnWaveVisual(trigSpawner.AllWaves[w]);
                        break;
                    }
                }
            }

            GUILayout.Space(6);
            if (GUILayout.Button(new GUIContent("Hide All", "Hide all wave visualizations"), EditorStyles.toolbarButton, GUILayout.Width(70))) {
                var visuals = FindObjectsOfType(typeof(VisualizationMarker));
                var totalItems = visuals.Length;
                var i = 0;
                while (visuals.Length > 0 && i < totalItems) {
                    DestroyImmediate(((VisualizationMarker)visuals[i]).gameObject);
                    i++;
                }
            }

            GUILayout.Space(6);
            if (GUILayout.Button(new GUIContent("Disable All", "Disable all wave visualizations"), EditorStyles.toolbarButton, GUILayout.Width(70))) {
                var visuals = FindObjectsOfType(typeof(VisualizationMarker));
                var totalItems = visuals.Length;
                var i = 0;

                while (visuals.Length > 0 && i < totalItems) {
                    DestroyImmediate(((VisualizationMarker)visuals[i]).gameObject);
                    i++;
                }

                var syncros = FindObjectsOfType(typeof(WaveSyncroPrefabSpawner));

                for (var j = 0; j < syncros.Length; j++) {
                    var aSpawner = (WaveSyncroPrefabSpawner)syncros[j];
                    var hasChanged = false;

                    for (var w = 0; w < aSpawner.waveSpecs.Count; w++) {
                        var aWave = aSpawner.waveSpecs[w];
                        if (!aWave.enableWave || !aWave.visualizeWave) {
                            continue;
                        }

                        aWave.visualizeWave = false;
                        hasChanged = true;
                    }

                    if (hasChanged) {
                        EditorUtility.SetDirty(aSpawner);
                    }
                }

                var trigSpawners = FindObjectsOfType(typeof(TriggeredSpawner));
                // ReSharper disable ForCanBeConvertedToForeach
                for (var j = 0; j < trigSpawners.Length; j++) {
                    var aSpawner = (TriggeredSpawner)trigSpawners[j];

                    var isChanged = false;
                    for (var z = 0; z < aSpawner.AllWaves.Count; z++) {
                        var wave = aSpawner.AllWaves[z];
                        if (!wave.visualizeWave) {
                            continue;
                        }

                        wave.visualizeWave = false;
                        isChanged = true;
                    }

                    if (isChanged) {
                        EditorUtility.SetDirty(aSpawner);
                    }
                }
            }

            GUI.contentColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        var newPersist = EditorGUILayout.Toggle("Persist Between Scenes", _settings.persistBetweenScenes);
        if (newPersist != _settings.persistBetweenScenes) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Persist Between Scenes");
            _settings.persistBetweenScenes = newPersist;
        }

        var newLogging = EditorGUILayout.Toggle("Log Messages", _settings.isLoggingOn);
        if (newLogging != _settings.isLoggingOn) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Log Messages");
            _settings.isLoggingOn = newLogging;
        }

        var hadNoListener = _settings.listener == null;
        var newListener = (LevelSettingsListener)EditorGUILayout.ObjectField("Listener", _settings.listener, typeof(LevelSettingsListener), true);
        if (newListener != _settings.listener) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "assign Listener");
            _settings.listener = newListener;
            if (hadNoListener && _settings.listener != null) {
                _settings.listener.sourceTransName = _settings.transform.name;
            }
        }

		GUI.contentColor = DTInspectorUtility.BrightButtonColor;
		if (GUILayout.Button("Collapse All Sections", EditorStyles.toolbarButton, GUILayout.Width(140))) {
			UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Collapse All Sections");
			_settings.killerPoolingExpanded = false;
			_settings.createPrefabPoolsExpanded = false;
			_settings.spawnersExpanded = false;
			_settings.gameStatsExpanded = false;
			_settings.showLevelSettings = false;
			_settings.showCustomEvents = false;
		}
		GUI.contentColor = Color.white;

        if (Application.isPlaying && PoolBoss.IsServer) {
            DTInspectorUtility.StartGroupHeader(1, false);
            EditorGUILayout.LabelField("Game Status Panel", EditorStyles.boldLabel);
            //EditorGUILayout.EndVertical();
            if (LevelSettings.IsGameOver) {
                GUI.backgroundColor = Color.red;
                DTInspectorUtility.ShowRedErrorBox("Game Status: GAME OVER");
            } else {
                GUI.backgroundColor = Color.green;
                DTInspectorUtility.ShowLargeBarAlertBox("Game Status: NOT OVER");
            }

            if (_settings.useWaves) {
                if (LevelSettings.WavesArePaused) {
                    GUI.backgroundColor = Color.red;

                    DTInspectorUtility.ShowRedErrorBox("Wave Status: Paused");
                } else {
                    GUI.backgroundColor = Color.green;
                    EditorGUILayout.BeginHorizontal();
					var level = LevelSettings.CurrentLevel + 1;
					var wave = LevelSettings.CurrentLevelWave;

					DTInspectorUtility.ShowLargeBarAlertBox("Playing Level: [" + level + "] Wave: [" + wave + "]");
                    EditorGUILayout.EndHorizontal();
                }
            }

            GUI.backgroundColor = Color.green;
            EditorGUILayout.BeginHorizontal();

            if (_settings.useWaves) {
                if (LevelSettings.WavesArePaused) {
                    if (GUILayout.Button("Unpause", EditorStyles.miniButton, GUILayout.Width(70))) {
                        LevelSettings.UnpauseWave();
                    }
                } else if (!LevelSettings.IsGameOver) {
                    if (GUILayout.Button("Pause", EditorStyles.miniButton, GUILayout.Width(70))) {
                        LevelSettings.PauseWave();
                    }
                }
            }

            var hasNextWave = LevelSettings.HasNextWave;

            if (!LevelSettings.WavesArePaused && hasNextWave && !LevelSettings.IsGameOver) {
                GUILayout.Space(4);

                if (GUILayout.Button("Next Wave", EditorStyles.miniButton, GUILayout.Width(70))) {
                    LevelSettings.EndWave();
                }
            }
			if (LevelSettings.IsGameOver) {
				GUILayout.Space(4);
				if (GUILayout.Button("Continue game", EditorStyles.miniButton, GUILayout.Width(90))) {
					LevelSettings.ContinueGame();        
				}

				GUILayout.Space(4);
				if (GUILayout.Button("Restart game", EditorStyles.miniButton, GUILayout.Width(80))) {
					LevelSettings.RestartGame();        
				}
			}

            EditorGUILayout.EndHorizontal();

            DTInspectorUtility.AddSpaceForNonU5();

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndVertical();
        }

        DTInspectorUtility.VerticalSpace(4);

        // Pool Boss section

        var state = _settings.killerPoolingExpanded;
        var text = "Pool Boss";

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (!state) {
            GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
        } else {
            GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
        }

        GUILayout.BeginHorizontal();

        text = "<b><size=11>" + text + "</size></b>";

		if (state) {
            text = "\u25BC " + text;
        } else {
            text = "\u25BA " + text;
        }
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
            state = !state;
        }

        GUILayout.Space(2f);


        EditorGUI.indentLevel = 0;
        if (state != _settings.killerPoolingExpanded) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Pool Boss");
            _settings.killerPoolingExpanded = state;
        }
        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;

        var poolingHolder = _settings.transform.FindChild(LevelSettings.KillerPoolingContainerTransName);
        if (poolingHolder == null) {
            Debug.LogError("You have no child prefab of LevelSettings called '" + LevelSettings.KillerPoolingContainerTransName + "'. " + LevelSettings.RevertLevelSettingsAlert);
            return;
        }
        if (_settings.killerPoolingExpanded) {
            DTInspectorUtility.BeginGroupedControls();
            var kp = poolingHolder.GetComponent<PoolBoss>();
            if (kp == null) {
                Debug.LogError("You have no PoolBoss script on your " + LevelSettings.KillerPoolingContainerTransName + " subprefab. " + LevelSettings.RevertLevelSettingsAlert);
                return;
            }

            DTInspectorUtility.ShowColorWarningBox(string.Format("You have {0} Pool Item(s) set up. Click the button below to configure Pooling.", kp.poolItems.Count));

            EditorGUILayout.BeginHorizontal();
            GUI.contentColor = DTInspectorUtility.BrightButtonColor;
            GUILayout.Space(10);
            if (GUILayout.Button("Configure Pooling", EditorStyles.toolbarButton, GUILayout.Width(120))) {
                Selection.activeGameObject = poolingHolder.gameObject;
            }
            GUI.contentColor = Color.white;

            EditorGUILayout.EndHorizontal();
            DTInspectorUtility.EndGroupedControls();
        }
        // end Pool Boss section

        // create Prefab Pools section
        EditorGUI.indentLevel = 0;
        DTInspectorUtility.VerticalSpace(2);

        state = _settings.createPrefabPoolsExpanded;
        text = "Prefab Pools";

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (!state) {
            GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
        } else {
            GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
        }

        GUILayout.BeginHorizontal();

        text = "<b><size=11>" + text + "</size></b>";

		if (state) {
            text = "\u25BC " + text;
        } else {
            text = "\u25BA " + text;
        }
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
            state = !state;
        }

        GUILayout.Space(2f);

        if (state != _settings.createPrefabPoolsExpanded) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Prefab Pools");
            _settings.createPrefabPoolsExpanded = state;
        }

        EditorGUILayout.EndHorizontal();

        if (_settings.createPrefabPoolsExpanded) {
            DTInspectorUtility.BeginGroupedControls();
            // BUTTONS...
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
            EditorGUI.indentLevel = 0;

            // Add expand/collapse buttons if there are items in the list

            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
            // A little space between button groups
            GUILayout.Space(6);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;

            DTInspectorUtility.StartGroupHeader();
            EditorGUI.indentLevel = 1;
            var newExp = DTInspectorUtility.Foldout(_settings.newPrefabPoolExpanded, "Create New Prefab Pools");
            if (newExp != _settings.newPrefabPoolExpanded) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand Create New Prefab Pools");
                _settings.newPrefabPoolExpanded = newExp;
            }
            EditorGUILayout.EndVertical();

            if (_settings.newPrefabPoolExpanded) {
                EditorGUI.indentLevel = 0;
                var newPoolName = EditorGUILayout.TextField("New Pool Name", _settings.newPrefabPoolName);
                if (newPoolName != _settings.newPrefabPoolName) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change New Pool Name");
                    _settings.newPrefabPoolName = newPoolName;
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUI.contentColor = DTInspectorUtility.AddButtonColor;
                if (GUILayout.Button("Create Prefab Pool", EditorStyles.toolbarButton, GUILayout.MaxWidth(110))) {
                    CreatePrefabPool();
                }
                GUI.contentColor = Color.white;
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            DTInspectorUtility.VerticalSpace(2);

            var pools = LevelSettings.GetAllPrefabPools;
            if (pools.Count == 0) {
                DTInspectorUtility.ShowColorWarningBox("You currently have no Prefab Pools.");
            }

            foreach (var pool in pools) {
                DTInspectorUtility.StartGroupHeader(1, false);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(pool.name);
                GUILayout.FlexibleSpace();

                var buttonPressed = DTInspectorUtility.AddControlButtons("Prefab Pool");
                if (buttonPressed == DTInspectorUtility.FunctionButtons.Edit) {
                    Selection.activeGameObject = pool.gameObject;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            if (pools.Count > 0) {
                DTInspectorUtility.VerticalSpace(2);
            }

            DTInspectorUtility.EndGroupedControls();
        }
        GUI.color = Color.white;
        // end create prefab pools section

        // create spawners section
        EditorGUI.indentLevel = 0;

        DTInspectorUtility.VerticalSpace(2);
        state = _settings.spawnersExpanded;
        text = "Syncro Spawners";

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (!state) {
            GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
        } else {
            GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
        }

        GUILayout.BeginHorizontal();

        text = "<b><size=11>" + text + "</size></b>";

		if (state) {
            text = "\u25BC " + text;
        } else {
            text = "\u25BA " + text;
        }
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
            state = !state;
        }

        GUILayout.Space(2f);
        EditorGUILayout.EndHorizontal();


        if (state != _settings.spawnersExpanded) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Syncro Spawners");
            _settings.spawnersExpanded = state;
        }

        if (_settings.spawnersExpanded) {
            DTInspectorUtility.BeginGroupedControls();
            // BUTTONS...
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
            EditorGUI.indentLevel = 0;

            // Add expand/collapse buttons if there are items in the list

            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
            // A little space between button groups
            GUILayout.Space(6);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
            // end create spawners section
            GUI.color = Color.white;

            var spawners = LevelSettings.GetAllSpawners;

            DTInspectorUtility.StartGroupHeader();
            EditorGUI.indentLevel = 1;
            var newExp = DTInspectorUtility.Foldout(_settings.createSpawnerExpanded, "Create New");
            if (newExp != _settings.createSpawnerExpanded) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand _settings.createSpawnerExpanded");
                _settings.createSpawnerExpanded = newExp;
            }
            EditorGUILayout.EndVertical();

            if (_settings.createSpawnerExpanded) {
                EditorGUI.indentLevel = 0;
                var newName = EditorGUILayout.TextField("New Spawner Name", _settings.newSpawnerName);
                if (newName != _settings.newSpawnerName) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change New Spawner Name");
                    _settings.newSpawnerName = newName;
                }

                var newType =
                    (LevelSettings.SpawnerType)EditorGUILayout.EnumPopup("New Spawner Color", _settings.newSpawnerType);
                if (newType != _settings.newSpawnerType) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change New Spawner Color");
                    _settings.newSpawnerType = newType;
                }

                EditorGUILayout.BeginHorizontal(EditorStyles.boldLabel);
                GUILayout.Space(10);
                GUI.contentColor = DTInspectorUtility.AddButtonColor;
                if (GUILayout.Button("Create Spawner", EditorStyles.toolbarButton, GUILayout.MaxWidth(110))) {
                    CreateSpawner();
                }
                GUI.contentColor = Color.white;
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            DTInspectorUtility.VerticalSpace(2);

            if (spawners.Count == 0) {
                DTInspectorUtility.ShowColorWarningBox("You currently have no Syncro Spawners.");
            }

            GUI.backgroundColor = DTInspectorUtility.BrightButtonColor;
            foreach (var spawner in spawners) {
                DTInspectorUtility.StartGroupHeader(1, false);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(spawner.name);
                GUILayout.FlexibleSpace();
                var buttonPressed = DTInspectorUtility.AddControlButtons("Spawner");
                if (buttonPressed == DTInspectorUtility.FunctionButtons.Edit) {
                    Selection.activeGameObject = spawner.gameObject;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            GUI.backgroundColor = Color.white;

            DTInspectorUtility.EndGroupedControls();
        }

        GUI.color = Color.white;


        // Player stats
        EditorGUI.indentLevel = 0;
        DTInspectorUtility.VerticalSpace(2);

        state = _settings.gameStatsExpanded;
        text = "World Variables";

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (!state) {
            GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
        } else {
            GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
        }

        GUILayout.BeginHorizontal();

        text = "<b><size=11>" + text + "</size></b>";

		if (state) {
            text = "\u25BC " + text;
        } else {
            text = "\u25BA " + text;
        }
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
            state = !state;
        }

        GUILayout.Space(2f);

        if (state != _settings.gameStatsExpanded) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle World Variables");
            _settings.gameStatsExpanded = state;
        }

        EditorGUILayout.EndHorizontal();

        if (_settings.gameStatsExpanded) {
            DTInspectorUtility.BeginGroupedControls();
            // BUTTONS...
            GUI.color = Color.white;

            var variables = LevelSettings.GetAllWorldVariables;
            if (variables.Count == 0) {
                DTInspectorUtility.ShowColorWarningBox("You currently have no World Variables.");
            }

            foreach (var worldVar in variables) {
                DTInspectorUtility.StartGroupHeader(1, false);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(worldVar.name);

                GUILayout.FlexibleSpace();

                var variable = worldVar.GetComponent<WorldVariable>();
                GUI.contentColor = DTInspectorUtility.BrightTextColor;
                GUILayout.Label(WorldVariableTracker.GetVariableTypeFriendlyString(variable.varType));
                GUI.contentColor = Color.white;

                var buttonPressed = DTInspectorUtility.AddControlButtons("World Variable");
                if (buttonPressed == DTInspectorUtility.FunctionButtons.Edit) {
                    Selection.activeGameObject = worldVar.gameObject;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            GUI.backgroundColor = Color.white;

            DTInspectorUtility.VerticalSpace(3);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUI.contentColor = DTInspectorUtility.BrightButtonColor;
            if (GUILayout.Button("World Variable Panel", EditorStyles.toolbarButton, GUILayout.MaxWidth(130))) {
                Selection.objects = new Object[] {
					playerStatsHolder.gameObject
				};
                return;
            }
            GUI.contentColor = Color.white;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            DTInspectorUtility.EndGroupedControls();
        }
        // end Player  stats
        GUI.color = Color.white;

        _settings._frames++;
        _isDirty = true;

        // level waves
        DTInspectorUtility.VerticalSpace(2);
        state = _settings.showLevelSettings;
        text = "Levels & Waves";

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (!state) {
            GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
        } else {
            GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
        }

        GUILayout.BeginHorizontal();

        text = "<b><size=11>" + text + "</size></b>";

		if (state) {
            text = "\u25BC " + text;
        } else {
            text = "\u25BA " + text;
        }
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
            state = !state;
        }

        GUILayout.Space(2f);

        if (state != _settings.showLevelSettings) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Level Waves");
            _settings.showLevelSettings = state;
        }
        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;

        if (_settings.showLevelSettings) {
            if (_settings.useWaves) {
                DTInspectorUtility.BeginGroupedControls();
                EditorGUI.indentLevel = 0;  // Space will handle this for the header

				DTInspectorUtility.StartGroupHeader(0);
				var newShow = GUILayout.Toggle(_settings.showCustomWaveClasses, " Show Custom Wave Classes");
				if (newShow != _settings.showCustomWaveClasses) {
					UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Show Custom Wave Classes");
					_settings.showCustomWaveClasses = newShow;
				}
				
				EditorGUILayout.EndVertical();
				if (_settings.showCustomWaveClasses) {
                    if (_settings.customWaveClasses.Count == 0) {
                        DTInspectorUtility.ShowLargeBarAlertBox("You have no Custom Wave Classes set up.");
                    }

                    int? classToDelete = null;

                    for (var i = 0; i < _settings.customWaveClasses.Count; i++)  {
                        var waveClass = _settings.customWaveClasses[i];

                        DTInspectorUtility.StartGroupHeader(1, false);
                        EditorGUILayout.BeginHorizontal();

                        var newName = EditorGUILayout.TextField("Wave Class", waveClass);
                        if (newName != waveClass) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Rename Custom Wave Class");
                            _settings.customWaveClasses[i] = newName;
                        }

                        var oldBG = GUI.backgroundColor;
                        GUI.backgroundColor = DTInspectorUtility.DeleteButtonColor;
                        if (GUILayout.Button(new GUIContent("Delete", "Click to delete Custom Wave Class"), EditorStyles.miniButton, GUILayout.MaxWidth(45))) {
                            classToDelete = i;
                        }
                        GUI.backgroundColor = oldBG;
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                    }

                    if (classToDelete.HasValue) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Delete Custom Wave Class");
                        _settings.customWaveClasses.RemoveAt(classToDelete.Value);
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUI.backgroundColor = Color.white;
                    GUI.contentColor = DTInspectorUtility.AddButtonColor;
                    GUILayout.Space(10);
                    if (GUILayout.Button(new GUIContent("Add", "New Custom Wave Class"), EditorStyles.toolbarButton, GUILayout.Width(32))) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Add New Custom Wave Class");
                        _settings.customWaveClasses.Add("New Wave Class (rename)");
                    }
                    GUI.contentColor = Color.white;

                    EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndVertical();

                if (_settings.LevelTimes.Count > 0) {
                    var newRepeat = (LevelSettings.LevelLoopMode)EditorGUILayout.EnumPopup("Last Level Completed", _settings.repeatLevelMode);
                    if (newRepeat != _settings.repeatLevelMode) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Last Level Completed");
                        _settings.repeatLevelMode = newRepeat;
                    }
                }

				EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Level Wave Settings");

                // BUTTONS...
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));

                // Add expand/collapse buttons if there are items in the list
                if (_settings.LevelTimes.Count > 0) {
                    GUI.contentColor = DTInspectorUtility.BrightButtonColor;
                    const string collapseIcon = "Collapse";
                    var content = new GUIContent(collapseIcon, "Click to collapse all");
                    var masterCollapse = GUILayout.Button(content, EditorStyles.toolbarButton);

                    const string expandIcon = "Expand";
                    content = new GUIContent(expandIcon, "Click to expand all");
                    var masterExpand = GUILayout.Button(content, EditorStyles.toolbarButton);
                    if (masterExpand) {
                        ExpandCollapseAll(true);
                    }
                    if (masterCollapse) {
                        ExpandCollapseAll(false);
                    }
                    GUI.contentColor = Color.white;
                } else {
                    GUILayout.FlexibleSpace();
                }

                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(50));

                var addText = string.Format("Click to add level{0}.", _settings.LevelTimes.Count > 0 ? " at the end" : "");

                // Main Add button
                GUI.contentColor = DTInspectorUtility.AddButtonColor;
                if (GUILayout.Button(new GUIContent("Add", addText), EditorStyles.toolbarButton)) {
                    _isDirty = true;
                    CreateNewLevelAfter();
                }
                GUI.contentColor = Color.white;

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndHorizontal();

                // ReSharper disable TooWideLocalVariableScope
                // ReSharper disable RedundantAssignment
                var levelButtonPressed = DTInspectorUtility.FunctionButtons.None;
                var waveButtonPressed = DTInspectorUtility.FunctionButtons.None;
                // ReSharper restore RedundantAssignment
                // ReSharper restore TooWideLocalVariableScope

                EditorGUI.indentLevel = 0;

                if (_settings.LevelTimes.Count == 0) {
                    DTInspectorUtility.ShowColorWarningBox("You have no Levels set up.");
                }

                var levelToDelete = -1;
                var levelToInsertAt = -1;
                var waveToInsertAt = -1;
                var waveToDelete = -1;
                int? waveToCopy = null;

                for (var l = 0; l < _settings.LevelTimes.Count; l++) {
                    EditorGUI.indentLevel = 0;
                    var levelSetting = _settings.LevelTimes[l];

                    DTInspectorUtility.StartGroupHeader();
                    EditorGUILayout.BeginHorizontal();
                    // Display foldout with current state
                    EditorGUI.indentLevel = 1;
					var levelDisplayName = string.Format("Level {0} Waves & Settings", (l + 1));
					if (!levelSetting.isExpanded && !string.IsNullOrEmpty(levelSetting.levelName)) {
						levelDisplayName += " (" + levelSetting.levelName + ")";
					}

					state = DTInspectorUtility.Foldout(levelSetting.isExpanded, levelDisplayName);
                    if (state != levelSetting.isExpanded) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand Level Waves & Settings");
                        levelSetting.isExpanded = state;
                    }

					GUILayout.FlexibleSpace();

					levelButtonPressed = DTInspectorUtility.AddFoldOutListItemButtons(l, _settings.LevelTimes.Count, "level", true, "Click to show all prefabs spawned in this Level", false);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();

                    EditorGUI.indentLevel = 0;

                    if (levelSetting.isExpanded) {
						var newName = EditorGUILayout.TextField("Level Name", levelSetting.levelName);
						if (newName != levelSetting.levelName) {
							UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Level Name");
							levelSetting.levelName = newName;
						}

						var newOrder = (LevelSettings.WaveOrder)EditorGUILayout.EnumPopup("Wave Sequence", levelSetting.waveOrder);
                        if (newOrder != levelSetting.waveOrder) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Wave Sequence");
                            levelSetting.waveOrder = newOrder;
                        }

                        for (var w = 0; w < levelSetting.WaveSettings.Count; w++) {
                            var showVisualize = false;

                            var waveSetting = levelSetting.WaveSettings[w];

                            DTInspectorUtility.StartGroupHeader(1);
                            EditorGUILayout.BeginHorizontal();
                            EditorGUI.indentLevel = 1;
                            
							var waveDisplayName = "Wave " + (w + 1);
							if (!waveSetting.isExpanded && !string.IsNullOrEmpty(waveSetting.waveName)) {
								waveDisplayName += " (" + waveSetting.waveName + ")";
							}
							// Display foldout with current state
                            var innerExpanded = DTInspectorUtility.Foldout(waveSetting.isExpanded, waveDisplayName);
                            if (innerExpanded != waveSetting.isExpanded) {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand Wave");
                                waveSetting.isExpanded = innerExpanded;
                            }

                            if (GUILayout.Button(new GUIContent("Visualize", "Visualize Waves of All Spawners"),
                                EditorStyles.toolbarButton, GUILayout.Width(64))) {
                                showVisualize = true;
                            }

							waveButtonPressed = DTInspectorUtility.AddFoldOutListItemButtons(w, levelSetting.WaveSettings.Count, "wave", true, "Click to show all prefabs spawned in this Wave", true, false, true);

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();

                            if (waveSetting.isExpanded) {
                                EditorGUI.indentLevel = 0;
                                if (waveSetting.skipWaveType == LevelSettings.SkipWaveMode.Always) {
                                    DTInspectorUtility.ShowColorWarningBox("This wave is set to be skipped.");
                                }

                                if (string.IsNullOrEmpty(waveSetting.waveName)) {
                                    waveSetting.waveName = "UNNAMED";
                                }

                                if (_settings.showCustomWaveClasses) {
                                    if (_settings.customWaveClasses.Count == 0) {
                                        DTInspectorUtility.ShowLargeBarAlertBox("Set up some Custom Wave Classes above first.");
                                    } else { 
                                        var waveClassIndex = _settings.customWaveClasses.IndexOf(waveSetting.waveClass);
                                        if (waveClassIndex < 0) {
                                            waveClassIndex = 0;
                                            _isDirty = true;
                                            waveSetting.waveClass = _settings.customWaveClasses[0];
                                        }

                                        var newClassIndex = EditorGUILayout.Popup("Custom Wave Class", waveClassIndex, _settings.customWaveClasses.ToArray());
                                        if (newClassIndex != waveClassIndex) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Custom Wave Class");
                                            waveSetting.waveClass = _settings.customWaveClasses[newClassIndex];
                                        }
                                    }
                                }

                                var newWaveName = EditorGUILayout.TextField("Wave Name", waveSetting.waveName);
                                if (newWaveName != waveSetting.waveName) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Wave Name");
                                    waveSetting.waveName = newWaveName;
                                }

                                var newWaveType = (LevelSettings.WaveType)EditorGUILayout.EnumPopup("Wave Type", waveSetting.waveType);
                                if (newWaveType != waveSetting.waveType) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Wave Type");
                                    waveSetting.waveType = newWaveType;
                                }

                                if (waveSetting.waveType == LevelSettings.WaveType.Timed) {
                                    var newEnd = EditorGUILayout.Toggle("End When All Destroyed", waveSetting.endEarlyIfAllDestroyed);
                                    if (newEnd != waveSetting.endEarlyIfAllDestroyed) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle End Early When All Destroyed");
                                        waveSetting.endEarlyIfAllDestroyed = newEnd;
                                    }

                                    var newDuration = EditorGUILayout.IntSlider("Duration (sec)", waveSetting.WaveDuration, 1, 2000);
                                    if (newDuration != waveSetting.WaveDuration) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Duration");
                                        waveSetting.WaveDuration = newDuration;
                                    }
                                }

                                switch (waveSetting.skipWaveType) {
                                    case LevelSettings.SkipWaveMode.IfWorldVariableValueAbove:
                                    case LevelSettings.SkipWaveMode.IfWorldVariableValueBelow:
                                        EditorGUILayout.Separator();
                                        break;
                                }

                                var newSkipType = (LevelSettings.SkipWaveMode)EditorGUILayout.EnumPopup("Skip Wave Type", waveSetting.skipWaveType);
                                if (newSkipType != waveSetting.skipWaveType) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Skip Wave Type");
                                    waveSetting.skipWaveType = newSkipType;
                                }

                                switch (waveSetting.skipWaveType) {
                                    case LevelSettings.SkipWaveMode.IfWorldVariableValueAbove:
                                    case LevelSettings.SkipWaveMode.IfWorldVariableValueBelow:
                                        var missingStatNames = new List<string>();
                                        missingStatNames.AddRange(allStats);
                                        missingStatNames.RemoveAll(delegate(string obj) {
                                            return waveSetting.skipWavePassCriteria.HasKey(obj);
                                        });

                                        var newStat = EditorGUILayout.Popup("Add Skip Wave Limit", 0, missingStatNames.ToArray());
                                        if (newStat != 0) {
                                            AddWaveSkipLimit(missingStatNames[newStat], waveSetting);
                                        }

                                        if (waveSetting.skipWavePassCriteria.statMods.Count == 0) {
                                            DTInspectorUtility.ShowRedErrorBox("You have no Skip Wave Limits. Wave will never be skipped.");
                                        } else {
                                            EditorGUILayout.Separator();

                                            int? indexToDelete = null;

                                            for (var i = 0; i < waveSetting.skipWavePassCriteria.statMods.Count; i++) {
                                                var modifier = waveSetting.skipWavePassCriteria.statMods[i];

                                                var buttonPressed = DTInspectorUtility.FunctionButtons.None;

                                                switch (modifier._varTypeToUse) {
                                                    case WorldVariableTracker.VariableType._integer:
                                                        buttonPressed = KillerVariablesHelper.DisplayKillerInt(ref _isDirty, modifier._modValueIntAmt, modifier._statName, _settings, true, true);
                                                        break;
                                                    case WorldVariableTracker.VariableType._float:
                                                        buttonPressed = KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, modifier._modValueFloatAmt, modifier._statName, _settings, true, true);
                                                        break;
                                                    default:
                                                        Debug.LogError("Add code for varType: " + modifier._varTypeToUse.ToString());
                                                        break;
                                                }

                                                KillerVariablesHelper.ShowErrorIfMissingVariable(modifier._statName);

                                                if (buttonPressed == DTInspectorUtility.FunctionButtons.Remove) {
                                                    indexToDelete = i;
                                                }
                                            }

                                            DTInspectorUtility.ShowColorWarningBox("Limits are inclusive: i.e. 'Above' means >=");
                                            if (indexToDelete.HasValue) {
                                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "remove Skip Wave Limit");
                                                waveSetting.skipWavePassCriteria.DeleteByIndex(indexToDelete.Value);
                                            }

                                            EditorGUILayout.Separator();
                                        }

                                        break;
                                }

                                if (_settings.useMusicSettings) {
                                    if (l > 0 || w > 0) {
                                        var newMusicMode = (LevelSettings.WaveMusicMode)EditorGUILayout.EnumPopup("Music Mode", waveSetting.musicSettings.WaveMusicMode);
                                        if (newMusicMode != waveSetting.musicSettings.WaveMusicMode) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Music Mode");
                                            waveSetting.musicSettings.WaveMusicMode = newMusicMode;
                                        }
                                    }

                                    if (waveSetting.musicSettings.WaveMusicMode == LevelSettings.WaveMusicMode.PlayNew) {
                                        var newWavMusic = (AudioClip)EditorGUILayout.ObjectField("Music", waveSetting.musicSettings.WaveMusic, typeof(AudioClip), true);
                                        if (newWavMusic != waveSetting.musicSettings.WaveMusic) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Wave Music");
                                            waveSetting.musicSettings.WaveMusic = newWavMusic;
                                        }
                                    }
                                    if (waveSetting.musicSettings.WaveMusicMode != LevelSettings.WaveMusicMode.Silence) {
                                        var newVol = EditorGUILayout.Slider("Music Volume", waveSetting.musicSettings.WaveMusicVolume, 0f, 1f);
                                        if (newVol != waveSetting.musicSettings.WaveMusicVolume) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Music Volume");
                                            waveSetting.musicSettings.WaveMusicVolume = newVol;
                                        }
                                    } else {
                                        var newFadeTime = EditorGUILayout.Slider("Silence Fade Time", waveSetting.musicSettings.FadeTime, 0f, 15f);
                                        if (newFadeTime != waveSetting.musicSettings.FadeTime) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Silence Fade Time");
                                            waveSetting.musicSettings.FadeTime = newFadeTime;
                                        }
                                    }
                                }

                                if (!Application.isPlaying) {
                                    DTInspectorUtility.VerticalSpace(2);
                                    var spawnersUsed = FindMatchingSpawners(l, w);

                                    if (spawnersUsed.Count == 0) {
                                        DTInspectorUtility.ShowLargeBarAlertBox("You have no Spawners set up for this Wave.");
                                    } else {
                                        GUI.contentColor = DTInspectorUtility.BrightTextColor;
                                        GUILayout.Label("Spawners set up for this wave: " + spawnersUsed.Count, EditorStyles.boldLabel);
                                        GUI.contentColor = Color.white;
                                    }

                                    foreach (var spawner in spawnersUsed) {
                                        DTInspectorUtility.StartGroupHeader(0, false);
                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.Label(spawner.name);
                                        GUILayout.FlexibleSpace();

                                        var buttonPressed = DTInspectorUtility.AddControlButtons("World Variable");
                                        if (buttonPressed == DTInspectorUtility.FunctionButtons.Edit) {
                                            Selection.activeGameObject = spawner.gameObject;
                                        }

                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.EndVertical();
                                    }

                                    if (spawnersUsed.Count > 1) {
                                        var newUsing = (LevelSettings.WaveSpawnerUseMode)EditorGUILayout.EnumPopup("Spawners To Use", waveSetting.spawnerUseMode);
                                        if (newUsing != waveSetting.spawnerUseMode) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Spawners To Use");
                                            waveSetting.spawnerUseMode = newUsing;
                                        }

                                        if (waveSetting.spawnerUseMode == LevelSettings.WaveSpawnerUseMode.RandomSubset) {
                                            var newMin = EditorGUILayout.IntSlider("Use Spawners Min", waveSetting.spawnersToUseMin, 1, spawnersUsed.Count);
                                            if (newMin != waveSetting.spawnersToUseMin) {
                                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Use Spawners Min");
                                                waveSetting.spawnersToUseMin = newMin;
                                            }

                                            var newMax = EditorGUILayout.IntSlider("Use Spawners Max", waveSetting.spawnersToUseMax, 1, spawnersUsed.Count);
                                            if (newMax != waveSetting.spawnersToUseMax) {
                                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Use Spawners Max");
                                                waveSetting.spawnersToUseMax = newMax;
                                            }

                                            if (waveSetting.spawnersToUseMin > waveSetting.spawnersToUseMax) {
                                                _isDirty = true;
                                                waveSetting.spawnersToUseMax = waveSetting.spawnersToUseMin;
                                            }

                                            if (waveSetting.spawnersToUseMax < waveSetting.spawnersToUseMin) {
                                                _isDirty = true;
                                                waveSetting.spawnersToUseMin = waveSetting.spawnersToUseMax;
                                            }
                                        }
                                    }
                                }

                                DTInspectorUtility.VerticalSpace(2);
                                EditorGUILayout.LabelField("Wave Completed Options", EditorStyles.boldLabel);

                                var newPause = EditorGUILayout.Toggle("Pause Global Waves", waveSetting.pauseGlobalWavesWhenCompleted);
                                if (newPause != waveSetting.pauseGlobalWavesWhenCompleted) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Pause Global Waves");
                                    waveSetting.pauseGlobalWavesWhenCompleted = newPause;
                                }

                                DTInspectorUtility.StartGroupHeader(0, false);
                                // beat level variable modifiers
                                var newBonusesEnabled = EditorGUILayout.BeginToggleGroup(" Wave Completion Bonus", waveSetting.waveBeatBonusesEnabled);
                                if (newBonusesEnabled != waveSetting.waveBeatBonusesEnabled) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Wave Completion Bonus");
                                    waveSetting.waveBeatBonusesEnabled = newBonusesEnabled;
                                }
                                EditorGUILayout.EndVertical();

                                if (waveSetting.waveBeatBonusesEnabled) {
                                    var missingBonusStatNames = new List<string>();
                                    missingBonusStatNames.AddRange(allStats);
                                    missingBonusStatNames.RemoveAll(delegate(string obj) {
                                        return
                                            waveSetting
                                                .waveDefeatVariableModifiers
                                                .HasKey(obj);
                                    });

                                    var newBonusStat = EditorGUILayout.Popup("Add Variable Modifer", 0,
                                        missingBonusStatNames.ToArray());
                                    if (newBonusStat != 0) {
                                        AddBonusStatModifier(missingBonusStatNames[newBonusStat], waveSetting);
                                    }

                                    if (waveSetting.waveDefeatVariableModifiers.statMods.Count == 0) {
                                        if (waveSetting.waveBeatBonusesEnabled) {
                                            DTInspectorUtility.ShowColorWarningBox(
                                                "You currently are using no modifiers for this wave.");
                                        }
                                    } else {
                                        EditorGUILayout.Separator();

                                        int? indexToDelete = null;

                                        for (var i = 0; i < waveSetting.waveDefeatVariableModifiers.statMods.Count; i++) {
                                            var modifier = waveSetting.waveDefeatVariableModifiers.statMods[i];

                                            var buttonPressed = DTInspectorUtility.FunctionButtons.None;
                                            switch (modifier._varTypeToUse) {
                                                case WorldVariableTracker.VariableType._integer:
                                                    buttonPressed = KillerVariablesHelper.DisplayKillerInt(
                                                        ref _isDirty, modifier._modValueIntAmt, modifier._statName,
                                                        _settings, true, true);
                                                    break;
                                                case WorldVariableTracker.VariableType._float:
                                                    buttonPressed =
                                                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty,
                                                            modifier._modValueFloatAmt, modifier._statName, _settings,
                                                            true, true);
                                                    break;
                                                default:
                                                    Debug.LogError("Add code for varType: " +
                                                                   modifier._varTypeToUse.ToString());
                                                    break;
                                            }

                                            KillerVariablesHelper.ShowErrorIfMissingVariable(modifier._statName);

                                            if (buttonPressed == DTInspectorUtility.FunctionButtons.Remove) {
                                                indexToDelete = i;
                                            }
                                        }

                                        if (indexToDelete.HasValue) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings,
                                                "delete Variable Modifier");
                                            waveSetting.waveDefeatVariableModifiers.DeleteByIndex(indexToDelete.Value);
                                        }
                                    }
                                }
                                EditorGUILayout.EndToggleGroup();

                                DTInspectorUtility.VerticalSpace(2);
                                DTInspectorUtility.StartGroupHeader(0, false);
                                // beat level Custom Events to fire
                                var newExp = EditorGUILayout.BeginToggleGroup(" Wave Completion Custom Events", waveSetting.useCompletionEvents);
                                if (newExp != waveSetting.useCompletionEvents) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Wave Completion Completion Custom Events");
                                    waveSetting.useCompletionEvents = newExp;
                                }
                                EditorGUILayout.EndVertical();

                                if (waveSetting.useCompletionEvents) {
                                    DTInspectorUtility.BeginGroupedControls();
                                    DTInspectorUtility.ShowColorWarningBox("When wave completed, fire the Custom Events below");
                                    EditorGUILayout.BeginHorizontal();
                                    GUI.contentColor = DTInspectorUtility.AddButtonColor;
                                    GUILayout.Space(10);
                                    if (GUILayout.Button(new GUIContent("Add", "Click to add a Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50))) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Add Wave Completion Custom Event");
                                        waveSetting.completionCustomEvents.Add(new CGKCustomEventToFire());
                                    }
                                    GUILayout.Space(10);
                                    if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50))) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Remove Last Wave Completion Custom Event");
                                        waveSetting.completionCustomEvents.RemoveAt(waveSetting.completionCustomEvents.Count - 1);
                                    }
                                    GUI.contentColor = Color.white;

                                    EditorGUILayout.EndHorizontal();

                                    if (waveSetting.completionCustomEvents.Count == 0) {
                                        DTInspectorUtility.ShowColorWarningBox("You have no Custom Events selected to fire.");
                                    }

                                    if (waveSetting.completionCustomEvents.Count > 0) {
                                        DTInspectorUtility.VerticalSpace(2);
                                    }

                                    // ReSharper disable once ForCanBeConvertedToForeach
                                    for (var i = 0; i < waveSetting.completionCustomEvents.Count; i++) {
                                        var anEvent = waveSetting.completionCustomEvents[i].CustomEventName;

                                        anEvent = DTInspectorUtility.SelectCustomEventForVariable(ref _isDirty, anEvent, _settings, "Custom Event");

                                        if (anEvent == waveSetting.completionCustomEvents[i].CustomEventName) {
                                            continue;
                                        }

                                        waveSetting.completionCustomEvents[i].CustomEventName = anEvent;
                                    }
                                    DTInspectorUtility.EndGroupedControls();
                                }
                                EditorGUILayout.EndToggleGroup();

                            }

                            if (showVisualize) {
                                var allSpawners = LevelSettings.GetAllSpawners;
                                // ReSharper disable once ForCanBeConvertedToForeach
                                for (var i = 0; i < allSpawners.Count; i++) {
                                    var aSpawner = allSpawners[i];
                                    aSpawner.gameObject.DestroyChildrenImmediateWithMarker();

                                    var spawn = aSpawner.GetComponent<WaveSyncroPrefabSpawner>();
                                    // ReSharper disable ForCanBeConvertedToForeach
                                    for (var wave = 0; wave < spawn.waveSpecs.Count; wave++) {
                                        // ReSharper restore ForCanBeConvertedToForeach
                                        spawn.waveSpecs[wave].visualizeWave = false;
                                    }
                                }

                                var spawnersUsed = FindMatchingSpawners(l, w);
                                foreach (var spawner in spawnersUsed) {
                                    // ReSharper disable once ForCanBeConvertedToForeach
                                    for (var lw = 0; lw < spawner.waveSpecs.Count; lw++) {
                                        var aWave = spawner.waveSpecs[lw];
                                        // ReSharper disable once InvertIf
                                        if (aWave.SpawnLevelNumber == l && aWave.SpawnWaveNumber == w) {
                                            aWave.visualizeWave = true;
                                            //Debug.Log(spawner.name + " : " + l + " : " + w);
                                            spawner.SpawnWaveVisual(aWave);
                                        }
                                    }
                                }
                            }

                            switch (waveButtonPressed) {
                                case DTInspectorUtility.FunctionButtons.Remove:
                                    if (levelSetting.WaveSettings.Count <= 1) {
                                        DTInspectorUtility.ShowAlert("You cannot delete the only Wave in a Level. Delete the Level if you like.");
                                    } else {
                                        waveToDelete = w;
                                    }

                                    _isDirty = true;
                                    break;
                                case DTInspectorUtility.FunctionButtons.Add:
                                    waveToInsertAt = w;
                                    _isDirty = true;
                                    break;
                                case DTInspectorUtility.FunctionButtons.Copy:
                                    waveToCopy = w;
                                    break;
								case DTInspectorUtility.FunctionButtons.ShowRelations:
									DTInspectorUtility.ShowLevelAndWaveSpawnedPrefabs(l + 1, w + 1);
									break;
                            }

                            EditorGUILayout.EndVertical();
                            DTInspectorUtility.AddSpaceForNonU5();
                        }

                        if (waveToDelete >= 0) {
                            if (DTInspectorUtility.ConfirmDialog("Delete wave? This cannot be undone.")) {
                                DeleteWave(levelSetting, waveToDelete, l);
                                _isDirty = true;
                            }
                        }
                        if (waveToInsertAt > -1) {
                            InsertWaveAfter(levelSetting, waveToInsertAt, l);
                            _isDirty = true;
                        }
                        if (waveToCopy.HasValue) {
                            CloneWave(levelSetting, waveToCopy.Value, l);
                            _isDirty = true;
                        }
                    }

                    switch (levelButtonPressed) {
                        case DTInspectorUtility.FunctionButtons.Remove:
                            if (DTInspectorUtility.ConfirmDialog("Delete level? This cannot be undone.")) {
                                levelToDelete = l;
                                _isDirty = true;
                            }
                            break;
                        case DTInspectorUtility.FunctionButtons.Add:
                            _isDirty = true;
                            levelToInsertAt = l;
                            break;
						case DTInspectorUtility.FunctionButtons.ShowRelations: 
							DTInspectorUtility.ShowLevelAndWaveSpawnedPrefabs(l + 1, 0);
							break;
                    }

                    EditorGUILayout.EndVertical();

                    if (!levelSetting.isExpanded) {
                        continue;
                    }

                    DTInspectorUtility.VerticalSpace(0);
                    DTInspectorUtility.AddSpaceForNonU5(3);
                }

                if (levelToDelete > -1) {
                    DeleteLevel(levelToDelete);
                }

                if (levelToInsertAt > -1) {
                    CreateNewLevelAfter(levelToInsertAt);
                }

                DTInspectorUtility.EndGroupedControls();
            } else {
                DTInspectorUtility.BeginGroupedControls();
                EditorGUILayout.LabelField(" Level Wave Settings (DISABLED)");
                DTInspectorUtility.EndGroupedControls();
            }
        }

        // level waves
        EditorGUI.indentLevel = 0;
        DTInspectorUtility.VerticalSpace(2);

        state = _settings.showCustomEvents;
        text = "Custom Events";

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (!state) {
            GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
        } else {
            GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
        }

        GUILayout.BeginHorizontal();

        text = "<b><size=11>" + text + "</size></b>";

		if (state) {
            text = "\u25BC " + text;
        } else {
            text = "\u25BA " + text;
        }
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
            state = !state;
        }

        GUILayout.Space(2f);

        if (state != _settings.showCustomEvents) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Custom Events");
            _settings.showCustomEvents = state;
        }
        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;

        if (_settings.showCustomEvents) {
            DTInspectorUtility.BeginGroupedControls();
            var newEvent = EditorGUILayout.TextField("New Event Name", _settings.newEventName);
            if (newEvent != _settings.newEventName) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change New Event Name");
                _settings.newEventName = newEvent;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUI.contentColor = DTInspectorUtility.AddButtonColor;
            if (GUILayout.Button("Create New Event", EditorStyles.toolbarButton, GUILayout.Width(100))) {
                CreateCustomEvent(_settings.newEventName);
            }

            GUILayout.Space(10);

            var hasExpanded = false;
            foreach (var t in _settings.customEvents) {
                if (!t.eventExpanded) {
                    continue;
                }
                hasExpanded = true;
                break;
            }

            if (_settings.customEvents.Count > 0) {
                var buttonText = hasExpanded ? "Collapse All" : "Expand All";

                if (GUILayout.Button(buttonText, EditorStyles.toolbarButton, GUILayout.Width(100))) {
                    ExpandCollapseCustomEvents(!hasExpanded);
                }
                GUILayout.Space(10);
                if (GUILayout.Button("Sort Alpha", EditorStyles.toolbarButton, GUILayout.Width(100))) {
                    SortCustomEvents();
                }
            }

            GUI.contentColor = Color.white;
            EditorGUILayout.EndHorizontal();

            if (_settings.customEvents.Count == 0) {
                DTInspectorUtility.ShowColorWarningBox("You currently have no custom events.");
            }

            EditorGUILayout.Separator();

            int? customEventToDelete = null;
            int? eventToRename = null;
			int? eventToVisualize = null;
			int? eventToEdit = null;
			int? eventToHide = null;

            for (var i = 0; i < _settings.customEvents.Count; i++) {
                DTInspectorUtility.StartGroupHeader();
                EditorGUI.indentLevel = 1;
                var anEvent = _settings.customEvents[i];

                EditorGUILayout.BeginHorizontal();
				var eName = anEvent.EventName;
				if (anEvent.IsEditing) {
					eName = "Rename:";
				}

				var exp = DTInspectorUtility.Foldout(anEvent.eventExpanded, eName);
                if (exp != anEvent.eventExpanded) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand Custom Event");
                    anEvent.eventExpanded = exp;
                }
				GUILayout.FlexibleSpace();
				if (Application.isPlaying) {
                    var receivers = LevelSettings.ReceiversForEvent(anEvent.EventName);

                    GUI.contentColor = DTInspectorUtility.BrightButtonColor;
                    if (receivers.Count > 0) {
                        if (GUILayout.Button("Select", EditorStyles.toolbarButton, GUILayout.Width(50))) {
                            var matches = new List<GameObject>(receivers.Count);

                            foreach (var t in receivers) {
                                matches.Add(t.gameObject);
                            }
                            Selection.objects = matches.ToArray();
                        }
                    }

                    if (GUILayout.Button("Fire!", EditorStyles.toolbarButton, GUILayout.Width(50))) {
                        LevelSettings.FireCustomEvent(anEvent.EventName, _settings.transform.position);
                    }

                    GUI.contentColor = DTInspectorUtility.BrightTextColor;
                    GUILayout.Label(string.Format("Receivers: {0}", receivers.Count));
                    GUI.contentColor = Color.white;
                } else {
                    if (anEvent.IsEditing) {
						var oldColor = GUI.backgroundColor;
						GUI.backgroundColor = DTInspectorUtility.BrightTextColor;
						var newName = GUILayout.TextField(anEvent.ProspectiveName, GUILayout.Width(130));
	                    if (newName != anEvent.ProspectiveName) {
	                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Proposed Event Name");
	                        anEvent.ProspectiveName = newName;
	                    }
						GUI.backgroundColor = oldColor;
					}

					var buttonPressed = DTInspectorUtility.AddCustomEventIcons(anEvent.IsEditing, true);

                    switch (buttonPressed) {
                        case DTInspectorUtility.FunctionButtons.Remove:
                            customEventToDelete = i;
                            break;
                        case DTInspectorUtility.FunctionButtons.Rename:
                            eventToRename = i;
                            break;
						case DTInspectorUtility.FunctionButtons.Cancel:
							anEvent.IsEditing = false;
							_isDirty = true;
							break;
						case DTInspectorUtility.FunctionButtons.Visualize:
							eventToVisualize = i;
							break;
						case DTInspectorUtility.FunctionButtons.Hide:
							eventToHide = i;
							break;
						case DTInspectorUtility.FunctionButtons.Edit:
							eventToEdit = i;
							break;
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                if (!anEvent.eventExpanded) {
                    EditorGUILayout.EndVertical();
                    DTInspectorUtility.AddSpaceForNonU5();
                    continue;
                }
                EditorGUI.indentLevel = 0;
                var rcvMode = (LevelSettings.EventReceiveMode)EditorGUILayout.EnumPopup("Send To Receivers", anEvent.eventRcvMode);
                if (rcvMode != anEvent.eventRcvMode) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Send To Receivers");
                    anEvent.eventRcvMode = rcvMode;
                }

                if (rcvMode == LevelSettings.EventReceiveMode.WhenDistanceLessThan || rcvMode == LevelSettings.EventReceiveMode.WhenDistanceMoreThan) {
                    KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, anEvent.distanceThreshold, "Distance Threshold", _settings);
                }

                if (rcvMode != LevelSettings.EventReceiveMode.Never) {
                    var rcvFilter = (LevelSettings.EventReceiveFilter)EditorGUILayout.EnumPopup("Valid Receivers", anEvent.eventRcvFilterMode);
                    if (rcvFilter != anEvent.eventRcvFilterMode) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Valid Receivers");
                        anEvent.eventRcvFilterMode = rcvFilter;
                    }
                }

                switch (anEvent.eventRcvFilterMode) {
                    case LevelSettings.EventReceiveFilter.Closest:
                    case LevelSettings.EventReceiveFilter.Random:
                        var newQty = EditorGUILayout.IntField("Valid Qty", anEvent.filterModeQty);
                        if (newQty != anEvent.filterModeQty) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Valid Qty");
                            anEvent.filterModeQty = Math.Max(1, newQty);
                        }
                        break;
                }

                EditorGUILayout.EndVertical();
                DTInspectorUtility.AddSpaceForNonU5();
            }

            if (customEventToDelete.HasValue) {
                _settings.customEvents.RemoveAt(customEventToDelete.Value);
            }
            if (eventToRename.HasValue) {
                RenameEvent(_settings.customEvents[eventToRename.Value]);
            }
			if (eventToVisualize.HasValue) {
				VisualizeEvent(_settings.customEvents[eventToVisualize.Value]);
			}
			if (eventToEdit.HasValue) {
				_settings.customEvents[eventToEdit.Value].IsEditing = true;
				_isDirty = true;
			}
			if (eventToHide.HasValue) {
				HideEvent(_settings.customEvents[eventToHide.Value]);
			}

            DTInspectorUtility.EndGroupedControls();
        }

        if (GUI.changed || _isDirty) {
            EditorUtility.SetDirty(target);	// or it won't save the data!!
        }

        //DrawDefaultInspector();
    }

    private void ExpandCollapseAll(bool isExpand) {
        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand / collapse all Level Wave Settings");

        foreach (var level in _settings.LevelTimes) {
            level.isExpanded = isExpand;
            foreach (var wave in level.WaveSettings) {
                wave.isExpanded = isExpand;
            }
        }
    }

    private void CreateSpawner() {
        var newSpawnerName = _settings.newSpawnerName;

        if (string.IsNullOrEmpty(newSpawnerName)) {
            DTInspectorUtility.ShowAlert("You must enter a name for your new Spawner.");
            return;
        }

        Transform spawnerTrans = null;

        switch (_settings.newSpawnerType) {
            case LevelSettings.SpawnerType.Green:
                spawnerTrans = _settings.GreenSpawnerTrans;
                break;
            case LevelSettings.SpawnerType.Red:
                spawnerTrans = _settings.RedSpawnerTrans;
                break;
        }

        var spawnPos = _settings.transform.position;
        spawnPos.x += Random.Range(-10, 10);
        spawnPos.z += Random.Range(-10, 10);

        // ReSharper disable once PossibleNullReferenceException
        var newSpawner = Instantiate(spawnerTrans.gameObject, spawnPos, Quaternion.identity) as GameObject;
        // ReSharper disable once PossibleNullReferenceException
        UndoHelper.CreateObjectForUndo(newSpawner.gameObject, "create Spawner");
        newSpawner.name = newSpawnerName;

        var spawnersHolder = _settings.transform.FindChild(LevelSettings.SpawnerContainerTransName);
        if (spawnersHolder == null) {
            DTInspectorUtility.ShowAlert(LevelSettings.NoSpawnContainerAlert);

            DestroyImmediate(newSpawner);

            return;
        }

        newSpawner.transform.parent = spawnersHolder.transform;
    }

    private void CreatePrefabPool() {
        var newPrefabPoolName = _settings.newPrefabPoolName;

        if (string.IsNullOrEmpty(newPrefabPoolName)) {
            DTInspectorUtility.ShowAlert("You must enter a name for your new Prefab Pool.");
            return;
        }

        var spawnPos = _settings.transform.position;

        var newPool = Instantiate(_settings.PrefabPoolTrans.gameObject, spawnPos, Quaternion.identity) as GameObject;
        // ReSharper disable once PossibleNullReferenceException
        newPool.name = newPrefabPoolName;

        var poolsHolder = _settings.transform.FindChild(LevelSettings.PrefabPoolsContainerTransName);
        if (poolsHolder == null) {
            DTInspectorUtility.ShowAlert(LevelSettings.NoPrefabPoolsContainerAlert);

            DestroyImmediate(newPool);
            return;
        }

        var dupe = poolsHolder.FindChild(newPrefabPoolName);
        if (dupe != null) {
            DTInspectorUtility.ShowAlert("You already have a Prefab Pool named '" + newPrefabPoolName + "', please choose another name.");

            DestroyImmediate(newPool);
            return;
        }

        UndoHelper.CreateObjectForUndo(newPool.gameObject, "create Prefab Pool");
        newPool.transform.parent = poolsHolder.transform;
    }

    private static void InsertWaveAfter(LevelSpecifics spec, int waveToInsertAt, int level) {
        var spawners = LevelSettings.GetAllSpawners;

        var newWave = new LevelWave();

        waveToInsertAt++;
        spec.WaveSettings.Insert(waveToInsertAt, newWave);

        foreach (var spawner in spawners) {
            var spawnerScript = spawner.GetComponent<WaveSyncroPrefabSpawner>();
            spawnerScript.InsertWave(waveToInsertAt, level);
        }
    }

    private static void CloneWave(LevelSpecifics spec, int waveToInsertAt, int level) {
        var spawners = LevelSettings.GetAllSpawners;

        var newWave = spec.WaveSettings[waveToInsertAt].Clone();

        waveToInsertAt++;
        spec.WaveSettings.Insert(waveToInsertAt, newWave);

        foreach (var spawner in spawners) {
            var spawnerScript = spawner.GetComponent<WaveSyncroPrefabSpawner>();
            spawnerScript.InsertWave(waveToInsertAt, level);
        }
    }

    private void DeleteLevel(int levelToDelete) {
        var spawners = LevelSettings.GetAllSpawners;

        _settings.LevelTimes.RemoveAt(levelToDelete);

        foreach (var spawner in spawners) {
            var spawnerScript = spawner.GetComponent<WaveSyncroPrefabSpawner>();
            spawnerScript.DeleteLevel(levelToDelete);
        }
    }

    private void CreateNewLevelAfter(int? index = null) {
        var spawners = LevelSettings.GetAllSpawners;

        var newLevel = new LevelSpecifics();
        var newWave = new LevelWave();
        newLevel.WaveSettings.Add(newWave);

        int newLevelIndex;

        if (index == null) {
            newLevelIndex = _settings.LevelTimes.Count;
        } else {
            newLevelIndex = index.Value + 1;
        }

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Add Level");

        _settings.LevelTimes.Insert(newLevelIndex, newLevel);

        foreach (var spawner in spawners) {
            var spawnerScript = spawner.GetComponent<WaveSyncroPrefabSpawner>();
            spawnerScript.InsertLevel(newLevelIndex);
        }
    }

    private static void DeleteWave(LevelSpecifics spec, int waveToDelete, int levelNumber) {
        var spawners = LevelSettings.GetAllSpawners;

        var spawnerScripts = new List<WaveSyncroPrefabSpawner>();
        foreach (var s in spawners) {
            spawnerScripts.Add(s.GetComponent<WaveSyncroPrefabSpawner>());
        }

        spec.WaveSettings.RemoveAt(waveToDelete);

        foreach (var script in spawnerScripts) {
            script.DeleteWave(levelNumber, waveToDelete);
        }
    }

    private void AddWaveSkipLimit(string modifierName, LevelWave spec) {
        if (spec.skipWavePassCriteria.HasKey(modifierName)) {
            DTInspectorUtility.ShowAlert("This wave already has a Skip Wave Limit for World Variable: " + modifierName + ". Please modify the existing one instead.");
            return;
        }

        var myVar = WorldVariableTracker.GetWorldVariableScript(modifierName);

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Skip Wave Limit");

        spec.skipWavePassCriteria.statMods.Add(new WorldVariableModifier(modifierName, myVar.varType));
    }

    private static List<WaveSyncroPrefabSpawner> FindMatchingSpawners(int level, int wave) {
        var spawners = LevelSettings.GetAllSpawners;

        var matchingSpawners = new List<WaveSyncroPrefabSpawner>();

        foreach (var spawner in spawners) {
            var spawnerScript = spawner.GetComponent<WaveSyncroPrefabSpawner>();
            var matchingWave = spawnerScript.FindWave(level, wave);
            if (matchingWave == null) {
                continue;
            }

            matchingSpawners.Add(spawnerScript);
        }

        return matchingSpawners;
    }

    private void AddBonusStatModifier(string modifierName, LevelWave waveSpec) {
        if (waveSpec.waveDefeatVariableModifiers.HasKey(modifierName)) {
            DTInspectorUtility.ShowAlert("This Wave already has a modifier for World Variable: " + modifierName + ". Please modify that instead.");
            return;
        }

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Wave Completion Bonus modifier");

        var vType = WorldVariableTracker.GetWorldVariableScript(modifierName);

        waveSpec.waveDefeatVariableModifiers.statMods.Add(new WorldVariableModifier(modifierName, vType.varType));
    }

    private void CreateLevelSettingsPrefab() {
        // ReSharper disable once RedundantCast
        var go = Instantiate(_settings.gameObject) as GameObject;
        // ReSharper disable once PossibleNullReferenceException
        go.name = "LevelWaveSettings";
        go.transform.position = Vector3.zero;
    }

    private void CreateCustomEvent(string newEventName) {
        if (_settings.customEvents.FindAll(delegate(CgkCustomEvent obj) {
            return obj.EventName == newEventName;
        }).Count > 0) {
            DTInspectorUtility.ShowAlert("You already have a custom event named '" + newEventName + "'. Please choose a different name.");
            return;
        }

        _settings.customEvents.Add(new CgkCustomEvent(newEventName));
    }

	private void VisualizeEvent(CgkCustomEvent cEvent) {
		var trigSpawners = FindObjectsOfType(typeof(TriggeredSpawner));

		// ReSharper disable once ForCanBeConvertedToForeach
		for (var t = 0; t < trigSpawners.Length; t++) {
			var trigSpawner = (TriggeredSpawner)trigSpawners[t];
		
			var matchingWave = trigSpawner.userDefinedEventWaves.Find(delegate(TriggeredWaveSpecifics obj) {
				return obj.customEventName == cEvent.EventName;
			});

			if (matchingWave == null || !matchingWave.enableWave) {
				continue;
			}

			var isChanged = false;

			// ReSharper disable once ForCanBeConvertedToForeach
			for (var w = 0; w < trigSpawner.AllWaves.Count; w++) {
				var wave = trigSpawner.AllWaves[w];
				if (!wave.enableWave || !wave.visualizeWave || wave == matchingWave) {
					continue;
				}

				isChanged = true;
				wave.visualizeWave = false;
				break;
			}

			if (!matchingWave.visualizeWave) {
				isChanged = true;
				matchingWave.visualizeWave = true;
			}

			trigSpawner.gameObject.DestroyChildrenImmediateWithMarker();
			trigSpawner.SpawnWaveVisual(matchingWave);

			if (isChanged) {
				EditorUtility.SetDirty(trigSpawner);
			}
		}
	}

	private void HideEvent(CgkCustomEvent cEvent) {
		var trigSpawners = FindObjectsOfType(typeof(TriggeredSpawner));
		
		// ReSharper disable once ForCanBeConvertedToForeach
		for (var t = 0; t < trigSpawners.Length; t++) {
			var trigSpawner = (TriggeredSpawner)trigSpawners[t];
			
			var matchingWave = trigSpawner.userDefinedEventWaves.Find(delegate(TriggeredWaveSpecifics obj) {
				return obj.customEventName == cEvent.EventName && obj.enableWave && obj.visualizeWave;
			});

			if (matchingWave == null) {
				continue;
			}
			
			matchingWave.visualizeWave = false;
			trigSpawner.gameObject.DestroyChildrenImmediateWithMarker();

			EditorUtility.SetDirty(trigSpawner);
		}
	}

    private void RenameEvent(CgkCustomEvent cEvent) {
        var match = _settings.customEvents.FindAll(delegate(CgkCustomEvent obj) {
            return obj.EventName == cEvent.ProspectiveName;
        });

        if (match.Count > 0) {
            DTInspectorUtility.ShowAlert("You already have a custom event named '" + cEvent.ProspectiveName + "'. Please choose a different name.");
            return;
        }
		 
		cEvent.IsEditing = false;

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "rename Custom Event");
        cEvent.EventName = cEvent.ProspectiveName;
    }

    private void ExpandCollapseCustomEvents(bool shouldExpand) {
        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Expand / Collapse All Custom Events");

        foreach (var t in _settings.customEvents) {
            t.eventExpanded = shouldExpand;
        }
    }

    private void SortCustomEvents() {
        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Sort Custom Events Alpha");

        _settings.customEvents.Sort(delegate(CgkCustomEvent x, CgkCustomEvent y) {
            return x.EventName.CompareTo(y.EventName);
        });
    }
}
