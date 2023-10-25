using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDoor : CInteractable
{

    bool isFront = false;   //������ ���� �յ� ?
    public bool IsDoorLeftHandle = false;
    public bool m_IsLocked = false; //����ֳ�?
    public bool m_bIsOpened = false; //���� �����־ �ݾƾ��ϳ�?
    public bool m_LockingEverytime = false; //���������� ���� �����ΰ�?

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

    //��ȣ�ۿ�
    public override void Interaction()
    {
        if (m_bCanWork == true) 
        {
            m_Outline.enabled = false;

            //if (m_IsLocked == true && m_Type == EInteractionType.DOOR_LOCKED)
            if (m_Lock.m_IsSolved == false && m_Type == EInteractionType.DOOR_LOCKED)
            {
                //TODO : ������ ��ȣ ������ ��..
                Debug.Log("���� ����־��");

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

    //�������
    public void UnlockDoor()
    {
        m_IsLocked = false;
    }


    Coroutine coDoorMove = null;

    //�÷��̾� ��ġ�� ���� ������
    void MoveDoor() 
    {
        Transform player = CGameManager.Instance.m_Player.transform;
        //float z = m_DoorHingi.InverseTransformPoint(player.position).z;
        float z = m_DoorHingi.InverseTransformPoint(player.position).x;

        Debug.Log(z);
        //front = -1
        float isfront = z > 0 ? -1f : 1f;
        //���� �����̹� �̸� ����
        if (IsDoorLeftHandle == true) isfront *= -1;
        //�ݱ�
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

    //������ �ִ�
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
