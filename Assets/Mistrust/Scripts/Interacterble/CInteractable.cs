using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CInteractable : MonoBehaviour
{
    [Header("---------------Interactable---------------")]
    public bool m_bCanWork = true;
    public Transform m_Handle = null;
    [SerializeField] protected Outline m_Outline = null;
    [SerializeField] protected bool m_bIsEnter = false;
    [SerializeField] float outlineThick = 5f;
    [SerializeField] float outlineDist = 5f;


    public enum EInteractionType { NONE = 0, DOOR_OPEN, DOOR_LOCKED, ROOTING, TOUCHING }
    [Header("---------------STATE---------------")]
    public EInteractionType m_Type = EInteractionType.NONE;

    public virtual void Interaction() 
    {
        if (m_bCanWork == true) { }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("IN PLAYER");
        m_Outline.enabled = true;

        if (m_bCanWork == true)
        CGameManager.Instance.m_Player.m_NearInterObj = this;
        coOutlineShow = StartCoroutine(CoOutlineShow());

        m_bIsEnter = true;
    }

    private void OnTriggerExit(Collider other)
    {
        m_Outline.enabled = false;

        if (CGameManager.Instance.m_Player.m_NearInterObj == this)
            CGameManager.Instance.m_Player.m_NearInterObj = null;

        if (coOutlineShow != null) StopCoroutine(coOutlineShow);
        coOutlineShow = null;
        m_bIsEnter = false;
    }

    Coroutine coOutlineShow = null;
    IEnumerator CoOutlineShow() 
    {
        var target = CGameManager.Instance.m_Player.transform;
        while (true) 
        {
            //float dist = Vector3.Distance(target.position, this.transform.position);

            float dist = (target.position - this.transform.position).sqrMagnitude;
            //if (dist < 2f) dist = 0;
            m_Outline.OutlineWidth = Mathf.Lerp(5, 0, dist / outlineDist);
            yield return null;
        }
    }


    private void OnEnable()
    {
        m_Outline.enabled = false;
        m_Outline.OutlineWidth = 0;
        coOutlineShow = null;
    }

    private void OnDisable()
    {
        m_Outline.enabled = false;
        m_Outline.OutlineWidth = 0;
        coOutlineShow = null;
    }

}
