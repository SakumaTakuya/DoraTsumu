using UnityEngine;

public class InitialSettings : ScriptableObject
{
    private const string Path = "Assets/New Initial Settings.asset";
    
    public EmitterSetting EmitterSetting;
    //public TsumuEraser TsumuEraser;

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/Initial Settings")]
    private static void Create()
    {
        var parent = CreateInstance<InitialSettings>();
        parent.EmitterSetting = CreateInstance<EmitterSetting>();
        //parent.TsumuEraser = CreateInstance<TsumuEraser>();
        
        UnityEditor.AssetDatabase.AddObjectToAsset(parent.EmitterSetting, Path);
        //UnityEditor.AssetDatabase.AddObjectToAsset(parent.TsumuEraser, Path);
        UnityEditor.AssetDatabase.CreateAsset(parent, Path);
        UnityEditor.AssetDatabase.ImportAsset(Path);
    }
#endif
}