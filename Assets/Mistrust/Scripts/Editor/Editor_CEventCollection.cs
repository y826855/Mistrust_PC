using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


#if UNITY_EDITOR
[CustomEditor(typeof(CEventCollection))]

public class Editor_CEventCollection : Editor
{
    CEventCollection selected = null;

    private void OnEnable()
    {
        selected = target as CEventCollection;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtility.SetDirty(selected);
        if (GUILayout.Button("GET ALL LOCKS IN SCENE") == true)
        {
            //var doors = selected.ParentObj.GetComponentsInChildren<CDoor>();
            //selected.m_Locks.Clear();
            //foreach (var it in doors)
            //{ selected.m_Locks.Add(it.m_Lock.m_IsOpened); }

            var doors = selected.ParentObj.GetComponentsInChildren<CDoor>();
            var dictionary = CGameManager.Instance.m_Dictionary;
            if (dictionary.m_Interactions.ContainsKey(selected.m_Chapter) == false)
                dictionary.m_Interactions.Add(selected.m_Chapter, new CCSVDictionary.CInteraction());

            int idx = 0;
            var list = dictionary.m_Interactions[selected.m_Chapter];
            list.m_Acts.Clear();
            foreach (var it in doors) 
            {
                if (it.m_Lock.m_Password == "")
                { it.m_Idx = -1; continue; }
                //dictionary.m_Interactions[key] = it.m_Lock.m_IsOpened;
                list.m_Acts.Add(it.m_Lock.m_IsSolved);
                idx++;
            }
            Debug.Log("INTERACTIONS TO DICTIONARY : " + selected.m_Chapter.ToString());
        }

        //CGameManager
    }
}
#endif