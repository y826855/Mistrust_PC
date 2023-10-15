using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(CScriptableSingletone<>))]
public class CScriptableEditor : Editor
{
    [MenuItem("Assets/Set ScriptableSingletone")]
    public static void OpenInspector()
    {
        //Selection.activeObject = test.Instance;
        //Selection.activeObject = CCSVLoader.Instance;
        Selection.activeObject = CGameManager.Instance;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUI.changed == true)
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
    }
}

#endif