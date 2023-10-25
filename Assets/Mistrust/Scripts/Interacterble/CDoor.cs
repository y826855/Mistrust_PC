using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDoor : CInteractable
{

    bool isFront = false;   //열리는 방향 앞뒤 ?
    public bool IsDoorLeftHandle = false;
    public bool m_IsLocked = false; //잠겨있나?
    public bool m_bIsOpened = false; //문이 열려있어서 닫아야하나?
    public bool m_LockingEverytime = false; //닫힐때마다 문이 잠길것인가?

    [Header("------------------------")]
    [SerializeField] Vector3 openAngle = new Vector3(0, 140f, 0);
    public Transform m_DoorHingi = null;
    [SerializeField] Collider m_ColDoor = null;
    [SerializeField] NavMeshSourceTag m_NavMeshSourceTag = null;
    public float m_OpenDuration = 1f;

    [Header("------------------------")]
    public CUtility.CLock m_Lock = new CUtility.CLock();
    public int m_Idx = -1;

    public void Start()
    {
        if (m_Type == EInteractionType.NONE)
            MoveDoor();
    }

    //상호작용
    public override void Interaction()
    {
        if (m_bCanWork == true) 
        {
            m_Outline.enabled = false;

            //if (m_IsLocked == true && m_Type == EInteractionType.DOOR_LOCKED)
            if (m_Lock.m_IsSolved == false && m_Type == EInteractionType.DOOR_LOCKED)
            {
                //TODO : 폰으로 신호 보낼때 흠..
                Debug.Log("문이 잠겨있어요");

//#if UNITY_ANDROID
                string packit = JsonUtility.ToJson(m_Lock);
                packit = string.Format("{0}+{1}", 
                    (int)CUtility.ESendToMobile.DOOR_LOCK, packit);
                CGameManager.Instance.m_Network.SendToMobile(packit);
//#endif
            }
            else { MoveDoor(); }
        }
    }

    public override void Interaction_ActionDone()
    {
        MoveDoor();
        m_Lock.m_IsSolved = true;
    }

    //잠금해제
    public void UnlockDoor()
    {
        m_IsLocked = false;
    }


    Coroutine coDoorMove = null;

    //플레이어 위치에 따른 문열기
    void MoveDoor() 
    {
        Transform player = CGameManager.Instance.m_Player.transform;
        //float z = m_DoorHingi.InverseTransformPoint(player.position).z;
        float z = m_DoorHingi.InverseTransformPoint(player.position).x;

        Debug.Log(z);
        //front = -1
        float isfront = z > 0 ? -1f : 1f;
        //왼쪽 손잡이문 이면 반전
        if (IsDoorLeftHandle == true) isfront *= -1;
        //닫기
        if (m_bIsOpened == true)
        {
            isfront = 0;
            if(m_LockingEverytime == false) m_Type = EInteractionType.DOOR_OPEN;
            else m_Type = EInteractionType.DOOR_LOCKED;
        }
        else m_Type = EInteractionType.NONE;

        if (coDoorMove == null)
        coDoorMove = StartCoroutine(CoDoorMove(openAngle * isfront));
    }

    //문열기 애님
    IEnumerator CoDoorMove(Vector3 _eulerAngle) 
    {
        m_bCanWork = false;
        m_bIsOpened = !m_bIsOpened;

        m_ColDoor.enabled = false;
        m_NavMeshSourceTag.enabled = false;

        var rot = m_DoorHingi.transform.localRotation;
        float t = 0;
        while (t < 1f) 
        {
            t += Time.deltaTime / m_OpenDuration;
            m_DoorHingi.transform.localRotation =
                Quaternion.Lerp(rot, Quaternion.Euler(_eulerAngle), t);
            yield return null;    
        }
        m_DoorHingi.transform.localRotation = Quaternion.Euler(_eulerAngle);

        yield return CUtility.m_WFS_DOT1;
        m_ColDoor.enabled = true;
        m_NavMeshSourceTag.enabled = true;
        m_bCanWork = true;
        coDoorMove = null;

        if (m_bIsEnter == true) m_Outline.enabled = true;
    }




}
