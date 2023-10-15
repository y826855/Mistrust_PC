using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class CScriptableSingletone<T> : ScriptableObject where T : ScriptableObject
{
    //부모 폴더 위치
    //static public string m_FolderPath = "Assets/Resources/";
    static public string m_FolderPath = CUtility.m_FolderPath;

    //존재할 폴더
    static public string m_AssetFolder = "Singletone";
    //static public string m_AssetFolder = "Scriptable";

    static private T _instance = null;

    static public System.Action func;
    abstract public void BeforeLoadInstance();

    static public T Instance
    {
        get
        {
            if (_instance != null) return _instance;


            //_instance = AssetDatabase.LoadAssetAtPath<T>(string.Format("[0][1][2]", m_FolderPath, m_AssetFolder, m_AssetFileName));

            //파일 존재하는지 먼저 불러보기
            _instance = Resources.Load<T>(string.Format("{0}/{1}", m_AssetFolder, typeof(T).Name));
            //Debug.Log(string.Format("{0}/{1}", m_AssetFolder, typeof(T).Name));

#if UNITY_EDITOR
            string directory = string.Format("{0}{1}/", m_FolderPath, m_AssetFolder);

            //경로에 없다면
            if (_instance == null)
            {
                //폴더 경로 확인. 없다면 폴더 생성
                if (AssetDatabase.IsValidFolder(directory) == false)
                {
                    //AssetDatabase.CreateFolder(m_FolderPath, m_AssetFolder);
                    System.IO.Directory.CreateDirectory(directory);
                }

                //경로에 다시 접근
                //_instance = AssetDatabase.LoadAssetAtPath<T>(directory);
                _instance = AssetDatabase.LoadAssetAtPath<T>(string.Format("{0}{1}.asset", directory, typeof(T).Name));

                //다시 접근해도 없다면
                if (_instance == null)
                {//객체 생성
                    _instance = CreateInstance<T>();
                    AssetDatabase.CreateAsset(_instance, string.Format("{0}{1}.asset", directory, typeof(T).Name));
                    Debug.Log(directory + typeof(T).Name);
                    Debug.Log("obj created");
                }
            }
#endif
            //인스턴스 처음 호출할때 AWAKE 실행
            (_instance as CScriptableSingletone<T>).BeforeLoadInstance();
            return _instance;
        }
    }
}
