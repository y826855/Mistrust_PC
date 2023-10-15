using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDoor : CInteractable
{
    bool isFront = false;
    public bool m_IsLock = false;
    public bool m_bIsOpened = false;
    [SerializeField] Vector3 openAngle = new Vector3(0, 140f, 0);

    public Transform m_DoorHingi = null;
    public float m_OpenSpeed = 3f;

    public override void Interaction()
    {
        if (m_bCanWork == true) 
        {
            if (m_IsLock == true) { }
            else { MoveDoor(); }
        }
    }

    Coroutine coDoorMove = null;

    void MoveDoor() 
    {
        Transform player = CGameManager.Instance.m_Player.transform;
        float z = this.transform.InverseTransformPoint(player.position).z;
        Debug.Log(z);

        //front = 1
        float isfront = z > 0 ? 1f : -1f;

        if (m_bIsOpened == true) isfront = 0; //´Ý±â
        if(coDoorMove == null)
        coDoorMove = StartCoroutine(CoDoorMove(openAngle * isfront));
    }

    IEnumerator CoDoorMove(Vector3 _eulerAngle) 
    {
        m_bCanWork = false;
        m_bIsOpened = !m_bIsOpened;

        float t = 0;
        while (t < 1f) 
        {
            t += Time.deltaTime / m_OpenSpeed;
            m_DoorHingi.transform.localRotation = 
                Quaternion.Lerp(m_DoorHingi.transform.localRotation,
                Quaternion.Euler(_eulerAngle),t);
            yield return null;    
        }

        m_DoorHingi.transform.localRotation = Quaternion.Euler(_eulerAngle);
        m_bCanWork = true;
        coDoorMove = null;
    }
    



}
