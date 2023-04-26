using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BazzaGibbs.GameSceneManager
{
    public class GameSceneManager : MonoBehaviour {
        // Only use for checking singleton
        private static GameSceneManager s_Instance;

        private static GameSceneManager Instance {
            get {
                if (s_Instance == null) {
                    s_Instance = new GameObject().AddComponent<GameSceneManager>();
                    s_Instance.wasInstantiatedByProperty = true;
                }

                return s_Instance;
            }
        }

        [SerializeField] private GameLevel m_StartLevel;

        // One and only one level can be loaded at a time.
        [NonSerialized] public GameLevel currentLevel;
        // Core scenes are not dependent on any level or aux scenes, and do not need to be unloaded when changing level.
        [NonSerialized] public HashSet<GameCoreScene> coreScenes = new();
        // Auxiliary scenes can be dependent on a level or core scene. They are all unloaded when the level is changed.
        [NonSerialized] public HashSet<GameAuxiliaryScene> auxiliaryScenes = new();

        private bool wasInstantiatedByProperty = false;
        
        private void Awake() {
            if (s_Instance != null && wasInstantiatedByProperty == false) {
                Debug.LogError("[GameSceneManager] Multiple GameSceneManagers. Please make sure only one exists.", this);
                DestroyImmediate(gameObject);
                return;
            }

            if (m_StartLevel != null) {
                SetLevel(m_StartLevel);
            }
        }

        public static void SetLevel(GameLevel level) {
            // Set current active scene to entry point.
            if (SceneManager.GetActiveScene() != Instance.gameObject.scene) {
                SceneManager.SetActiveScene(Instance.gameObject.scene);
            }

            // Unload aux scenes before changing level
            foreach (GameAuxiliaryScene auxScene in Instance.auxiliaryScenes) {
                auxScene.Unload();
            }
            Instance.auxiliaryScenes.Clear();

            if (Instance.currentLevel != null) {
                // todo: Await
                Instance.currentLevel.Unload();
            }

            Instance.currentLevel = level;
            // todo: Await
            level.Load();
            // Level will set itself active, we don't have a reference to the actual Scene object

        }

        public static void LoadAuxScene(GameAuxiliaryScene auxScene) {
            if (Instance.auxiliaryScenes.Contains(auxScene)) return;
            Instance.auxiliaryScenes.Add(auxScene);
            auxScene.Load();
        }

        public static void UnloadAuxScene(GameAuxiliaryScene auxScene) {
            if(Instance.auxiliaryScenes.TryGetValue(auxScene, out GameAuxiliaryScene loadedAuxScene)) {
                loadedAuxScene.Unload();
                Instance.auxiliaryScenes.Remove(loadedAuxScene);
            }
        }

        public static void LoadCoreScene(GameCoreScene coreScene) {
            if (Instance.coreScenes.Contains(coreScene)) return;
            Instance.coreScenes.Add(coreScene);
            coreScene.Load();
        }

        public static void UnloadCoreScene(GameCoreScene coreScene) {
            if(Instance.coreScenes.TryGetValue(coreScene, out GameCoreScene loadedCoreScene)) {
                loadedCoreScene.Unload();
                Instance.coreScenes.Remove(loadedCoreScene);
            }
        }

    }
}