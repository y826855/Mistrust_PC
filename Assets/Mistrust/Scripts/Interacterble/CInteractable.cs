using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CInteractable : MonoBehaviour
{
    public bool m_bCanWork = true;


    public virtual void Interaction() 
    {
        if (m_bCanWork == true) { }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("IN PLAYER");

        if(m_bCanWork == true)
        CGameManager.Instance.m_Player.m_NearInterObj = this;
    }

    private void OnTriggerExit(Collider other)
    {
        if (CGameManager.Instance.m_Player.m_NearInterObj == this)
            CGameManager.Instance.m_Player.m_NearInterObj = null;
    }

}
