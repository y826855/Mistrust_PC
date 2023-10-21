using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCSVDictionary : MonoBehaviour
{
    public SerializeDictionary<uint, ScriptableObject> m_AllScriptables
        = new SerializeDictionary<uint, ScriptableObject>();

    
    //public SerializeDictionary<Vector2Int, bool> m_Interactions =
    //    new SerializeDictionary<Vector2Int, bool>();
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

    //Ÿ�Կ� ���� ������ ��� ��ȯ��
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

    //ID�� �޾� Ÿ������ ��ȯ
    public System.Type  IDtoType(uint _idx) 
    {
        //if (_idx < ((uint)EDataID.ENEMY)) return typeof(CUtility.CData_Skill);
        //else if (_idx < ((uint)EDataID.DROPTABLE)) return typeof(CUtility.CData_Enemy);
        //else if (_idx < ((uint)EDataID.ITEM)) return typeof(CUtility.CData_Drop);
        //else if (_idx < ((uint)EDataID.WEAPON)) return typeof(CUtility.CData_Item);
        //else return typeof(CUtility.CData_Weapon);

        return null;
    }
}
