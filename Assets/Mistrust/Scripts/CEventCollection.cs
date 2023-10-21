using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CEventCollection : MonoBehaviour
{
    public enum EChapter { CHAPTER01 = 0, CHAPTER02, CHAPTER03 };
    public GameObject ParentObj = null;
    public EChapter m_Chapter = EChapter.CHAPTER01;


    public void Start()
    {
        
    }

    public void GetJson() 
    {
        string data = JsonUtility.ToJson(CGameManager.Instance.m_Dictionary.m_Interactions);
        CGameManager.Instance.m_Dictionary.m_Interactions =
            JsonUtility.FromJson(data,
            typeof(SerializeDictionary<EChapter, CCSVDictionary.CInteraction>))
            as SerializeDictionary<EChapter, CCSVDictionary.CInteraction>;
    }

    //º¯°æ
    public void ChangeLocked(int _idx, bool _toggle) 
    {
        Debug.Log("CHAGED");
        CGameManager.Instance.m_Dictionary.
            m_Interactions[m_Chapter].m_Acts[_idx] = _toggle;
    }
}
