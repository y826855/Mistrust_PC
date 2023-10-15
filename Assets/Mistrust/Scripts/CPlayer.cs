using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayer : MonoBehaviour
{
    public CInteractable m_NearInterObj = null;

    private void Awake()
    {
        CGameManager.Instance.m_Player = this;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            Interaction();
    }

    public void Interaction() 
    {
        if(m_NearInterObj != null) m_NearInterObj.Interaction();
    }



}
