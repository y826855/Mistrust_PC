using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CCSVDictionary : MonoBehaviour
{
    public SerializeDictionary<uint, ScriptableObject> m_AllScriptables
        = new SerializeDictionary<uint, ScriptableObject>();

    
    //public SerializeDictionary<Vector2Int, bool> m_Interactions =
    //    new SerializeDictionary<Vector2Int, bool>();


    //상호작용 저장용
    [System.Serializable]
    public class CInteraction 
    { public List<bool> m_Acts = new List<bool>(); }
    public SerializeDictionary<CEventCollection.EChapter, CInteraction> m_Interactions 
        = new SerializeDictionary<CEventCollection.EChapter, CInteraction>();


    public enum EDataID
    {
        NONE = -1,
    }

    //public uint m_EndOfSkill = 0;

    //타입에 따른 데이터 모두 반환함
    public List<T> GetAllDataOfType<T>() where T : ScriptableObject 
    {
        //uint startIdx = 0;
        //uint endIdx = 0;
        List<T> dataList = new List<T>();

        //if (typeof(T) == typeof(CCSV_Skill))
        //{
        //    startIdx = (int)EDataID.SKILL;
        //    endIdx = m_EndOfSkill;
        //}
        //else if (typeof(T) == typeof(object/*tpye*/))
        //{ }


        //for (uint i = startIdx; i < endIdx; i++)
        //    dataList.Add(m_AllScriptables[i] as T);

        return dataList;
    }
    
    //public T GetDataOfType<T>(uint _idx) where T : ScriptableObject
    public T GetDataOfType<T>(uint _idx) where T : CScriptable_CSVData<T>
    {
        T data = m_AllScriptables[_idx] as T;
        return data;
    }

    //ID를 받아 타입으로 변환
    public System.Type  IDtoType(uint _idx) 
    {
        //if (_idx < ((uint)EDataID.ENEMY)) return typeof(CUtility.CData_Skill);
        //else if (_idx < ((uint)EDataID.DROPTABLE)) return typeof(CUtility.CData_Enemy);
        //else if (_idx < ((uint)EDataID.ITEM)) return typeof(CUtility.CData_Drop);
        //else if (_idx < ((uint)EDataID.WEAPON)) return typeof(CUtility.CData_Item);
        //else return typeof(CUtility.CData_Weapon);

        return null;
    }

    //불러오기
    public void LoadDatas() 
    {
        //JsonUtility.FromJson
        string path = Application.dataPath + "/Data/InteractionData.txt";

        if (File.Exists(path) == true)
        {
            string data = File.ReadAllText(path);
            m_Interactions = JsonUtility.FromJson
                <SerializeDictionary<CEventCollection.EChapter, CInteraction>>(data);
            Debug.Log(data);
        }
    }

    //저장
    public void SaveDatas() 
    {
        string path = Application.dataPath + "/Data";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string data = JsonUtility.ToJson(m_Interactions);
        Debug.Log(data);
        File.WriteAllText(path + "/InteractionData.txt", data);
    }
}
