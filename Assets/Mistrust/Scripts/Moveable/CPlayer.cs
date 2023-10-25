using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;

public class CPlayer : CStateMachine
{
    public CInteractable m_NearInterObj = null;
    CInteractable currInterObj = null;

    public Transform m_Model = null;
    [SerializeField] Transform m_Cursor = null;
    public UnityEngine.AI.NavMeshAgent m_Agent;
    [SerializeField] Animator m_Animator = null;


    [Header("-------------------MOVE-------------------")]
    [SerializeField] float defaultSpeed = 8.75f;
    [SerializeField] float minSpeed = 2.5f;
    [SerializeField] float maxSpeedDest = 10f;
    [SerializeField] LayerMask m_CheckLandMask;
    [SerializeField] LayerMask m_InteractionMask;
    [Header(("-------------------RIG-------------------"))]
    [SerializeField] Rig m_Rig_Move = null;
    [SerializeField] Rig m_Rig_Interaction = null;
    [SerializeField] Transform m_MoveWeightTarget = null;
    [SerializeField] Transform m_InterationWeightTarget = null;



    [Header(("-------------------STATE-------------------"))]
    Coroutine coMoveState = null;
    public EPlayerMove currMoveState = EPlayerMove.IDLE;
    float speedSlow = 1f;
    [SerializeField] bool canAct = true;

    public EPlayerMove m_MoveState
    {
        get
        { return currMoveState; }
        set
        {

            if (currMoveState == value) return;
            else if (coMoveState != null)
            {
                StopCoroutine(coMoveState);
                coMoveState = null;
            }

            currMoveState = value;
            //값이 바뀌면 함수 실행됨
            switch (currMoveState)
            {
                case EPlayerMove.IDLE: Idle(); break;
                case EPlayerMove.MOVE: Move(); break;
                case EPlayerMove.ACTION: break;
                default: break;
            }
        }
    }

    private void Start()
    {
        Debug.Log("RIGGINS");
        Idle();
    }

    //////////////////////////////////////////////////
    public void Idle()
    {
        coMoveState = StartCoroutine(CoIdle());
    }
    IEnumerator CoIdle()
    {
        while (true)
        {
            if (CheckMouseInput() == true)
            {
                m_MoveState = EPlayerMove.MOVE;
                break;
            }
            yield return null;
        }
    }
    //////////////////////////////////////////////////
    public void Move()
    {
        coMoveState = StartCoroutine(CoMove());
    }
    IEnumerator CoMove()
    {
        float stopTime = 1f;
        Vector3 locPos = m_MoveWeightTarget.localPosition;
        //일정 시간 입력 없으면 idle로
        while (stopTime > 0)
        {
            bool isInput = CheckMouseInput();

            //
            if (m_Agent.remainingDistance > m_Agent.stoppingDistance)
            {
                float moveWeight = m_Agent.velocity.sqrMagnitude / (defaultSpeed * defaultSpeed);
                m_Animator.SetFloat("MoveSpeed", Mathf.Clamp(moveWeight, 0.1f, 1f));

                Vector3 currVel = m_Agent.velocity.normalized;
                Vector3 destVel = m_Agent.desiredVelocity.normalized;

                float angle = Vector3.Cross(currVel, destVel).y;
                locPos.x = angle;
                m_MoveWeightTarget.localPosition = Vector3.Lerp(
                    m_MoveWeightTarget.localPosition, locPos, Time.deltaTime);

                //if (canTurn == true
                //    && m_Agent.velocity != Vector3.zero
                //    && m_Agent.desiredVelocity != Vector3.zero
                //    && currVel != destVel
                //    )
                //{
                //    //회전시 좌 우 체크
                //    float angle = Vector3.Cross(currVel, destVel).y;
                //    //Debug.Log(angle);
                //    if (angle > 0.3f || angle < -0.3f)
                //    {
                //        if (angle > 0) { Debug.Log("우회전"); }
                //        else Debug.Log("좌회전");
                //    }
                //}
                stopTime = 1f;
            }
            else if (isInput == false)
            {
                m_Animator.SetFloat("MoveSpeed", 0);
                stopTime -= Time.deltaTime;
            }

            yield return null;
        }

        m_Animator.SetFloat("MoveSpeed", 0);
        m_MoveState = EPlayerMove.IDLE;
    }


    //마우스 입력 받음
    RaycastHit m_HitInfo = new RaycastHit();
    public bool CheckMouseInput()
    {

        if (Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftShift))
        {
            //레이케스트 하여 마우스 포인터로 움직임
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo, 100f, m_CheckLandMask))
            {
                m_Agent.destination = m_HitInfo.point;
                Vector3 dest = m_Agent.destination;
                dest.y = m_Agent.transform.position.y;
                m_Cursor.position = m_HitInfo.point;

                float dist = m_Agent.remainingDistance;
                if (dist <= 0) dist = 0.1f;
                float weight = dist / maxSpeedDest;

                //거리 비례 속도변경
                m_Agent.speed = Mathf.Lerp(minSpeed, defaultSpeed, weight * speedSlow);

                //상호작용중 움직임 받으면 종료함
                if (canAct == false)
                {
                    m_Animator.SetTrigger("CancelAction");
                    
                    string packit = "none";
                    packit = string.Format("{0}+{1}",
                        (int)CUtility.ESendToMobile.CLOSE_APP, packit);
                    CGameManager.Instance.m_Network.SendToMobile(packit);

                    if (coInteractionIK != null) { StopCoroutine(coInteractionIK); }
                    coInteractionIK = StartCoroutine(CoInteractionIK(false, 0.1f));
                    canAct = true;
                }

                return true;
            }
        }
        return false;
    }


    private void Awake()
    {
        CGameManager.Instance.m_Player = this;
    }

    //상시 마우스 입력 상호작용
    public void Update()
    {

        if (canAct == true && Input.GetMouseButtonDown(0)) 
        {
            m_Animator.ResetTrigger("CancelAction");

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            Physics.Raycast(ray, out hit, 100f, m_InteractionMask);
            Debug.Log(hit.collider?.gameObject.name);

            if (m_NearInterObj != null &&
                Physics.Raycast(ray, out hit, 100f,m_InteractionMask)) 
            {
                currInterObj = m_NearInterObj;
                Interaction();
            }
            
        }
        //    Interaction();
    }



    //상호작용
    public void Interaction()
    {
        if (m_NearInterObj != null)
        {
            switch (m_NearInterObj.m_Type)
            {
                case CInteractable.EInteractionType.DOOR_OPEN:
                    m_Agent.destination = m_NearInterObj.m_Handle.position;
                    coInteractionIK = StartCoroutine(CoInteractionIK(true, 1f));
                    m_Animator.SetTrigger("DoorOpen");
                    canAct = false;
                    break;

                case CInteractable.EInteractionType.DOOR_LOCKED:
                    m_Agent.destination = m_NearInterObj.m_Handle.position;
                    coInteractionIK = StartCoroutine(CoInteractionIK(true, 1f));
                    m_Animator.SetTrigger("DoorClosed");
                    canAct = false;
                    break;

                case CInteractable.EInteractionType.NONE:
                    m_NearInterObj.Interaction();
                    break;
            }
        }
        //
    }
    void CheckIKWorking() 
    {
        if (coInteractionIK != null) StopCoroutine(coInteractionIK);
        coInteractionIK = null;
    }

    //상호작용 오브젝트 IK
    Coroutine coInteractionIK = null;
    IEnumerator CoInteractionIK(bool _toggle, float _duration)
    {
        //Debug.Log("IK start");
        if (m_NearInterObj != null)
        {
            Vector3 _targetPos = m_NearInterObj.m_Handle.position;
            float t = 1f;

            //StartCoroutine(LookAt(_targetPos, 0.2f));
            //if (_toggle == true) yield return StartCoroutine(LookAt(_targetPos, 0.2f));
            while (t > 0)
            {
                t -= Time.deltaTime / _duration;

                //Debug.Log("IK MOVED");

                if (_toggle == true)
                {
                    m_InterationWeightTarget.position = _targetPos;
                    m_Rig_Interaction.weight = 1f - t;
                    LookAt(_targetPos, t);
                }
                else m_Rig_Interaction.weight = t;

                yield return null;
            }
        }
    }

    //지정된 방향 쳐다보기
    IEnumerator CoLookAt(Vector3 _target, float _duration) 
    {
        float t = 1f;
        while (t > 0)
        {
            t -= Time.deltaTime / _duration;
            LookAt(_target, t);
            yield return null;
        }
    }

    void LookAt(Vector3 _target, float _weight) 
    {
        _target.y = this.transform.position.y;
        Vector3 look = _target - this.transform.position;
        this.transform.forward =
            Vector3.Lerp(this.transform.forward, look, 1f - _weight);
    }


    //애니메이션 상호작용 끝나면 호출됨
    public void Anim_Interaction() 
    {
        Debug.Log("interaction done");

        if (m_NearInterObj != null && canAct == false)
        {
            m_NearInterObj.Interaction();
            canAct = true;
        }

        //ik 위치 되돌리기
        if (coInteractionIK != null) 
        {
            StopCoroutine(coInteractionIK); 
        }
        coInteractionIK = StartCoroutine(CoInteractionIK(false, 0.1f));
    }

    public void Anim_Interaction_Phone()
    {
        //canAct = true;
        Debug.Log("PHONE ");
        if (m_NearInterObj != null && canAct == false)
        { m_NearInterObj.Interaction(); }
        coInteractionIK = StartCoroutine(CoInteractionIK(false, 0.1f));
    }


    public void ReciveData(string _data) 
    {
        Debug.Log(_data);

        if(bool.Parse(_data) == true)
            m_NearInterObj.Interaction_ActionDone();

        m_Animator.SetTrigger("CancelAction");
    }

}
