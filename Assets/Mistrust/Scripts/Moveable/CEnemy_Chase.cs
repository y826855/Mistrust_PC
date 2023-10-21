using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using UnityEditor;


public class CEnemy_Chase : CStateMachine
{

    Coroutine coMoveState = null;
    public EEnemyMove currMoveState = EEnemyMove.IDLE;

    public EEnemyMove m_MoveState
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
                case EEnemyMove.IDLE:   Idle(); break;
                case EEnemyMove.CHASE:  Chase(); break;
                case EEnemyMove.PATROL: Patrol(); break;
                case EEnemyMove.ACTION: break;
                default: break;
            }
        }
    }

    public Animator m_Animator = null;

    public Transform m_PatrolLocations = null;
    public NavMeshAgent m_Agent = null;
    public LayerMask m_FindLayer;
    public float m_AttackDist = 1.5f;
    public float m_PatrolRange = 3f;

    [SerializeField] float viewAngle = 0;  // 시야 각도 (130도)
    [SerializeField] float viewDistance = 0; // 시야 거리 (10미터)
    [SerializeField] float defaultSpeed = 0f;
    [SerializeField] float patrolSpeed = 0f;

    [SerializeField] List<Vector3> m_DefaultPatrolPos = new List<Vector3>();
    [SerializeField] List<Vector3> m_PatrolPos = new List<Vector3>();
    

    public void GetHint(Vector3 _hintPos) 
    {
        
    }

    void Start()
    {
        var inputedState = m_MoveState;
        m_MoveState = EEnemyMove.ACTION;
        m_MoveState = inputedState;

        defaultSpeed = m_Agent.speed;
        m_Agent.stoppingDistance = m_AttackDist;
    }

    //대기
    public void Idle() 
    {
        coMoveState = StartCoroutine(CoIdle());
    }
    IEnumerator CoIdle() 
    {
        yield return CUtility.m_WFS_1;
        m_MoveState = EEnemyMove.PATROL;
        //m_MoveState = EEnemyMove.CHASE;

    }
    //////////////////////////////////////////////////

    //////////추적
    public void Chase() 
    { coMoveState = StartCoroutine(CoChase()); }

    //플레이어 쫒아감. 일정시간 멈춰있으면 상태 변경
    IEnumerator CoChase()
    {
        var player = CGameManager.Instance.m_Player;

        float stopTime = 2f;
        bool isInRange = false;

        m_Agent.speed = defaultSpeed;

        while (stopTime >= 0f) 
        {
            float distSqr = (this.transform.position - player.transform.position).sqrMagnitude;

            //공격 사거리 들어왔으면?
            if (distSqr < m_AttackDist * m_AttackDist) 
            { Attack(); isInRange = true;  break; }

            //m_Agent.Move(player.transform.position);
            m_Agent.destination = player.transform.position;

            //속도에 따른 애님
            float moveWeight = m_Agent.velocity.sqrMagnitude / (defaultSpeed * defaultSpeed);
            m_Animator.SetFloat("MoveSpeed", Mathf.Clamp(moveWeight, 0.1f, 1f));

            //멈춤 상태에서의 시간 //
            if (m_Agent.path.status == NavMeshPathStatus.PathPartial)
                stopTime -= Time.deltaTime;

            yield return null;
        }

        yield return null;
        if (isInRange == false)
        { 
            m_MoveState = EEnemyMove.IDLE; 
            Debug.Log("Target Lost 돌아가서 다시 순찰"); 
        }
    }
    //////////////////////////////////////////////////
    //정찰
    public void Patrol() 
    { coMoveState = StartCoroutine(CoPatrol()); }
    IEnumerator CoPatrol() 
    {
        m_Agent.speed = patrolSpeed;
        var player = CGameManager.Instance.m_Player;


        if (m_DefaultPatrolPos.Count > 0) 
        {
            int currPatrolIdx = CheckNearestPatrolPoint();
            m_Agent.SetDestination(m_DefaultPatrolPos[currPatrolIdx]);

            while (true)
            {//남은거리 얼마 안남았을때 다음 순찰지로 이동
                if (m_PatrolRange > m_Agent.remainingDistance)
                {
                    if (++currPatrolIdx == m_DefaultPatrolPos.Count) 
                        currPatrolIdx = 0;
                    m_Agent.SetDestination(m_DefaultPatrolPos[currPatrolIdx]);

                    //속도에 따른 애님
                    float moveWeight = m_Agent.velocity.sqrMagnitude / (defaultSpeed * defaultSpeed);
                    m_Animator.SetFloat("MoveSpeed", Mathf.Clamp(moveWeight, 0.1f, 1f));
                }

                //시야 안에 플레이어 있나 체크
                if (TargetInView(player.transform.position) == true) 
                { 
                    m_MoveState = EEnemyMove.CHASE;
                    break;
                }
                yield return null;
            }
        }
    }

    //가장 가까운 순찰지점 찾음
    int CheckNearestPatrolPoint() 
    {
        float near = (this.transform.position - m_DefaultPatrolPos[0]).sqrMagnitude;
        int nearIdx = 0;
        for (int i = 1; i < m_DefaultPatrolPos.Count; i++) 
        {
            float dist = (this.transform.position - m_DefaultPatrolPos[i]).sqrMagnitude;
            if (dist < near) { near = dist; nearIdx = i; }
            //Debug.Log(dist);
        }

        Debug.Log(nearIdx);
        Debug.Log(near);
        return nearIdx;
    }
    //////////////////////////////////////////////////

    //공격
    public void Attack()
    {
        Debug.Log("꽁껶!!");
        m_MoveState = EEnemyMove.ACTION;
        coMoveState = StartCoroutine(CoAttack());
    }
    IEnumerator CoAttack() 
    {//공격 딜레이 이후 추적
        yield return CUtility.m_WFS_2;
        m_MoveState = EEnemyMove.CHASE;
    }
    //////////////////////////////////////////////////

    private Vector3 BoundaryAngle(float angle)
    {
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));
    }


    //시야 안 플레이어 체크
    //https://ansohxxn.github.io/unity%20lesson%203/ch7-3/
    private bool TargetInView(Vector3 _playerPos)
    {
        Vector3 dist = _playerPos - this.transform.position;
        //시야 사거리 안으로 왔나?
        if (dist.sqrMagnitude < viewDistance * viewDistance)
        {
            Vector3 direction = (_playerPos - this.transform.position).normalized;
            float angle = Vector3.Angle(direction, this.transform.forward);

            RaycastHit hit;
            if (angle < viewAngle * 0.5f)
            {
                if (Physics.Raycast(transform.position + transform.up, direction, out hit, viewDistance, m_FindLayer))
                {
                    //_target = hit.transform;
                    return true;
                }
            }
        }

        return false;
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        float radius = 0.5f;
        foreach (var it in m_DefaultPatrolPos) 
        { Gizmos.DrawSphere(it, radius); }


        Vector3 leftBoundary = BoundaryAngle(-viewAngle * 0.5f);  // z 축 기준으로 시야 각도의 절반 각도만큼 왼쪽으로 회전한 방향 (시야각의 왼쪽 경계선)
        Vector3 rightBoundary = BoundaryAngle(viewAngle * 0.5f);  // z 축 기준으로 시야 각도의 절반 각도만큼 오른쪽으로 회전한 방향 (시야각의 오른쪽 경계선)

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, viewDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, this.transform.position + leftBoundary * viewDistance);
        Gizmos.DrawLine(this.transform.position, this.transform.position + rightBoundary * viewDistance);

    }


#if UNITY_EDITOR
    [CustomEditor(typeof(CEnemy_Chase))]
    public class Editor_CEnemy_Chase : Editor 
    {
        CEnemy_Chase selected = null;
        private void OnEnable()
        {
            selected = target as CEnemy_Chase;

            EditorUtility.SetDirty(selected);
            if (selected.m_PatrolLocations == null) return;
            selected.m_DefaultPatrolPos.Clear();
            foreach (Transform it in selected.m_PatrolLocations)
            { selected.m_DefaultPatrolPos.Add(it.transform.position); }
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif



}
