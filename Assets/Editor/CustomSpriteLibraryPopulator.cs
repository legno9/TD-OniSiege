using UnityEngine;
using UnityEditor;
using UnityEngine.U2D.Animation;
using System.Collections.Generic;
using System.Linq;

public class CustomSpriteLibraryPopulator : EditorWindow
{
    private SpriteLibraryAsset _parentLibraryAsset;
    private Texture2D _sourceTexture;
    private string _spriteLibName = "MySpriteLib";
    private SpriteLibraryAsset _newSpriteLibrarySourceAsset;

    [MenuItem("Tools/Sprite Library Auto Filler")]
    public static void ShowWindow()
    {
        GetWindow<CustomSpriteLibraryPopulator>("Sprite Library Auto Filler");
    }

    private void OnGUI()
    {
        GUILayout.Label("Parent to create a new Sprite Library", EditorStyles.boldLabel);

        _parentLibraryAsset = (SpriteLibraryAsset)EditorGUILayout.ObjectField(
            "Sprite Library Asset",
            _parentLibraryAsset,
            typeof(SpriteLibraryAsset),
            false
        );

        _sourceTexture = (Texture2D)EditorGUILayout.ObjectField(
            "Source Texture Sheet",
            _sourceTexture,
            typeof(Texture2D),
            false
        );
        
        _spriteLibName = EditorGUILayout.TextField("New Sprite Library Name", _spriteLibName);

        if (GUILayout.Button("New Sprite Library"))
        {
            CreateLib();
        }
    }
    
    private void CreateLib()
    {
        if (!_parentLibraryAsset || !_sourceTexture)
        {
            Debug.LogError("Sprite Library Asset and Source Texture must be assigned.");
        }
        
        string texturePath = AssetDatabase.GetAssetPath(_sourceTexture);
        Sprite[] spritesInTexture = AssetDatabase.LoadAllAssetsAtPath(texturePath)
            .OfType<Sprite>()
            .ToArray();

        if (spritesInTexture.Length == 0)
        {
            Debug.LogError("Sprite sheet does not contain any sprites.");
        }
        
        Dictionary<string, Sprite> spriteMap = new();
        foreach (var sprite in spritesInTexture)
        {
            int lastUnderscoreIndex = sprite.name.LastIndexOf('_');
            
            if (lastUnderscoreIndex == -1) continue;
            string suffix = sprite.name[lastUnderscoreIndex..];

            spriteMap.TryAdd(suffix, sprite);
        }
        
        string finalName = _spriteLibName + ".asset";
       
        _newSpriteLibrarySourceAsset = CreateInstance<SpriteLibraryAsset>();
        _newSpriteLibrarySourceAsset.name = finalName;
        
        IEnumerable<string> categories = _parentLibraryAsset.GetCategoryNames();
        
        foreach (var category in categories)
        {
            var labels = _parentLibraryAsset.GetCategoryLabelNames(category);
            foreach (var label in labels)
            {
                int lastUnderscoreIndex = label.LastIndexOf('_');
                
                if (lastUnderscoreIndex == -1) continue;
                string suffix = label[lastUnderscoreIndex..];

                if (!spriteMap.TryGetValue(suffix, out Sprite matchingSprite)) continue;

                _newSpriteLibrarySourceAsset.AddCategoryLabel(matchingSprite, category, label);
            }
        }
        
        AssetDatabase.CreateAsset(_newSpriteLibrarySourceAsset, "Assets/" + finalName);
    }
}