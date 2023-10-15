using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDoor : CInteractable
{
    bool isFront = false;
    public bool m_IsLocked = false;
    public bool m_bIsOpened = false;
    [SerializeField] Vector3 openAngle = new Vector3(0, 140f, 0);

    public Transform m_DoorHingi = null;
    public float m_OpenSpeed = 3f;

    [Header("------------------------")]
    public CUtility.CLock m_Lock = new CUtility.CLock();
    public string password = "135";



    public void Start()
    {
        m_Lock.m_IsOpened = false;
        m_Lock.m_Password = password;
        if (m_Lock != null) m_IsLocked = true;
    }

    //상호작용
    public override void Interaction()
    {
        if (m_bCanWork == true) 
        {
            if (m_IsLocked == true) 
            {
                //TODO : 폰으로 신호 보낼때 흠..
                string packit = JsonUtility.ToJson(m_Lock);
                packit = string.Format("{0}+{1}", "Lock", packit);
                CGameManager.Instance.m_Network.SendToMobile(packit);
            }
            else { MoveDoor(); }
        }
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
        float z = this.transform.InverseTransformPoint(player.position).z;
        Debug.Log(z);

        //front = 1
        float isfront = z > 0 ? 1f : -1f;

        if (m_bIsOpened == true) isfront = 0; //닫기
        if(coDoorMove == null)
        coDoorMove = StartCoroutine(CoDoorMove(openAngle * isfront));
    }

    //문열기 애님
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
