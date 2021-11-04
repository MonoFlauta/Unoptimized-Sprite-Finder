using System;
using UnityEditor;
using UnityEngine;

namespace UnoptimizedSpriteFinder
{
    public class UnoptimizedSpriteFinderSettings : ScriptableObject
    {
        [MenuItem("Window/Unoptimized Sprite Finder/Create Settings")]
        public static void CreateMyAsset()
        {
            var asset = CreateInstance<UnoptimizedSpriteFinderSettings>();

            AssetDatabase.CreateAsset(asset, "Assets/UnoptimizedSpriteFinderData/UnoptimizedSpriteFinderSettings.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        public string[] ignorePaths = Array.Empty<string>();
        public string pathHasToContain;
        public string pathShouldNotContain;
        public int[] multiplierValidSizes = { 4 };
        public int[] exponentValidSizes = {1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096};
    }
}