﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

namespace BazzaGibbs.GameSceneManager {
     
    [CreateAssetMenu(menuName = "Game Scene Manager/Play Mode Launch Settings", fileName = "PlayModeLaunchSettings", order = 22)]
    public class PlayModeLaunchSettings : ScriptableObject {
        public bool enabled = true;
        #if UNITY_EDITOR
        public SceneAsset playModeScene;
        #endif
        private void Awake() {
            SetPlayModeScene();
        }
        
        private void OnValidate() {
            SetPlayModeScene();
        }

        private void SetPlayModeScene() {
#if UNITY_EDITOR
            if (enabled && playModeScene != null) {
                EditorSceneManager.playModeStartScene = playModeScene;
            }
            else {
                EditorSceneManager.playModeStartScene = null;
            }
#endif
        }
    }
}