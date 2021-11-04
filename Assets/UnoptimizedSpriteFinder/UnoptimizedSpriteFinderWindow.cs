using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace UnoptimizedSpriteFinder
{
    public class UnoptimizedSpriteFinderWindow : EditorWindow
    {
        private Vector2 _scrollPosition = Vector2.zero;
        private string[] _spritePaths;
        private UnoptimizedSpriteFinderSettings _settings;

        [MenuItem("Window/Unoptimized Sprite Finder/Open Window")]
        public static void Open()
        {
            GetWindow<UnoptimizedSpriteFinderWindow>();
        }

        private void OnGUI()
        {
            _settings = (UnoptimizedSpriteFinderSettings)EditorGUILayout.ObjectField("Settings", _settings, typeof(UnoptimizedSpriteFinderSettings));

            if (_settings == null)
            {
                EditorGUILayout.HelpBox("You should assign a Settings first",MessageType.Error);
                return;
            }

            if (GUILayout.Button("Search for sprites"))
            {
                var spritePaths = GetSpritePaths();
                var atlases = GetAtlases();

                _spritePaths = FilterSpritePathsWith(spritePaths, atlases);
            }

            if(_spritePaths != null)
                ShowAssetsInformation();
        }

        private string[] FilterSpritePathsWith(string[] spritePaths, SpriteAtlas[] atlases) =>
            spritePaths
                .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
                .Where(SizeIsInvalid)
                .Where(x => !IsInsideAnAtlas(x, atlases))
                .Select(AssetDatabase.GetAssetPath)
                .ToArray();
        
        private void ShowAssetsInformation()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            foreach (var path in _spritePaths)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(path);
                if (GUILayout.Button("Select"))
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(path);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        private bool IsInsideAnAtlas(Sprite sprite, SpriteAtlas[] atlases)
        {
            var spritePath = AssetDatabase.GetAssetPath(sprite);
            foreach (var atlas in atlases)
            {
                var paths = atlas.GetPackables()
                    .Select(AssetDatabase.GetAssetPath);

                if (paths.Any(x => spritePath.Contains(x)))
                    return true;
            }

            return false;
        }
            

        private bool SizeIsInvalid(Sprite sprite) =>
            !IsAValidSize(sprite.texture.width)
            || !IsAValidSize(sprite.texture.height);

        private bool IsAValidSize(int value) => 
            _settings.exponentValidSizes.Contains(value) ||
            IsMultiplierOfAValidSize(value);

        private bool IsMultiplierOfAValidSize(int value) => 
            _settings.multiplierValidSizes.Any(x => value % x == 0);

        private string[] GetSpritePaths() =>
            AssetDatabase.FindAssets("t:Sprite")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(IsNotAnIgnoredPath)
                .Where(PathContainsNeededText)
                .Where(PathDoesNotContainUnNeededText)
                .ToArray();

        private bool PathDoesNotContainUnNeededText(string path) =>
            string.IsNullOrEmpty(_settings.pathShouldNotContain) 
            || !path.Contains(_settings.pathShouldNotContain);

        private bool PathContainsNeededText(string path) => 
            string.IsNullOrEmpty(_settings.pathHasToContain)
                || path.Contains(_settings.pathHasToContain);

        private bool IsNotAnIgnoredPath(string path) => 
            !_settings.ignorePaths.Any(path.StartsWith);

        private SpriteAtlas[] GetAtlases() =>
            AssetDatabase.FindAssets("t:spriteatlas")
                .Select(x => AssetDatabase.LoadAssetAtPath<SpriteAtlas>(AssetDatabase.GUIDToAssetPath(x)))
                .ToArray();
    }
}
