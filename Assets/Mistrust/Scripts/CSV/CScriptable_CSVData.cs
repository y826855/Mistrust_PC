using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class CScriptable_CSVData<T> : ScriptableObject where T : ScriptableObject
{
    //public Sprite m_Icon = null;

#if UNITY_EDITOR
    //
    static public T CreateInst(string path, string name)
    {
        //파일 존재하는지 먼저 불러보기
        //T get = Resources.Load<T>(string.Format("{0}/{1}", path, name));
        T get = AssetDatabase.LoadAssetAtPath<T>(string.Format("{0}{1}.asset", path, name));


        if (get == null)
        {
            //폴더 경로 확인. 없다면 폴더 생성
            if (AssetDatabase.IsValidFolder(path) == false)
            {
                System.IO.Directory.CreateDirectory(path);
            }

            //경로에 다시 접근
            get = AssetDatabase.LoadAssetAtPath<T>(string.Format("{0}{1}.asset", path, name));

            //다시 접근해도 없다면
            if (get == null)
            {//객체 생성
                get = CreateInstance<T>();
                Debug.Log(string.Format("Create Asset : {0}{1}.asset", path, name));
                AssetDatabase.CreateAsset(get, string.Format("{0}{1}.asset", path, name));
            }
        }

        return get;
    }

    //텍스쳐 불러오기
    static public Sprite LoadTexture(string path, string name)
    {
        //png
        var texture = AssetDatabase.LoadAssetAtPath<Sprite>
                (string.Format("{0}SP_{1}.png", path, name));
        //jpg
        if (texture == null)
            texture = AssetDatabase.LoadAssetAtPath<Sprite>
            (string.Format("{0}SP_{1}.jpg", path, name));

        else return texture;

        


        /*TODO : 다른 확장자도 추가해보자*/


        return texture;
    }

    //프리펩 불러오기
    static public GameObject LoadPref(string _path, string _name)
    {
        var pref = AssetDatabase.LoadAssetAtPath<GameObject>
            (string.Format("{0}Pref_{1}.prefab", _path, _name));
        return pref;
    }
#endif
}
