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
            //���� �ٲ�� �Լ� �����
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

    [SerializeField] float viewAngle = 0;  // �þ� ���� (130��)
    [SerializeField] float viewDistance = 0; // �þ� �Ÿ� (10����)
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

    //���
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

    //////////����
    public void Chase() 
    { coMoveState = StartCoroutine(CoChase()); }

    //�÷��̾� �i�ư�. �����ð� ���������� ���� ����
    IEnumerator CoChase()
    {
        var player = CGameManager.Instance.m_Player;

        float stopTime = 2f;
        bool isInRange = false;

        m_Agent.speed = defaultSpeed;

        while (stopTime >= 0f) 
        {
            float distSqr = (this.transform.position - player.transform.position).sqrMagnitude;

            //���� ��Ÿ� ��������?
            if (distSqr < m_AttackDist * m_AttackDist) 
            { Attack(); isInRange = true;  break; }

            //m_Agent.Move(player.transform.position);
            m_Agent.destination = player.transform.position;

            //�ӵ��� ���� �ִ�
            float moveWeight = m_Agent.velocity.sqrMagnitude / (defaultSpeed * defaultSpeed);
            m_Animator.SetFloat("MoveSpeed", Mathf.Clamp(moveWeight, 0.1f, 1f));

            //���� ���¿����� �ð� //
            if (m_Agent.path.status == NavMeshPathStatus.PathPartial)
                stopTime -= Time.deltaTime;

            yield return null;
        }

        yield return null;
        if (isInRange == false)
        { 
            m_MoveState = EEnemyMove.IDLE; 
            Debug.Log("Target Lost ���ư��� �ٽ� ����"); 
        }
    }
    //////////////////////////////////////////////////
    //����
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
            {//�����Ÿ� �� �ȳ������� ���� �������� �̵�
                if (m_PatrolRange > m_Agent.remainingDistance)
                {
                    if (++currPatrolIdx == m_DefaultPatrolPos.Count) 
                        currPatrolIdx = 0;
                    m_Agent.SetDestination(m_DefaultPatrolPos[currPatrolIdx]);

                    //�ӵ��� ���� �ִ�
                    float moveWeight = m_Agent.velocity.sqrMagnitude / (defaultSpeed * defaultSpeed);
                    m_Animator.SetFloat("MoveSpeed", Mathf.Clamp(moveWeight, 0.1f, 1f));
                }

                //�þ� �ȿ� �÷��̾� �ֳ� üũ
                if (TargetInView(player.transform.position) == true) 
                { 
                    m_MoveState = EEnemyMove.CHASE;
                    break;
                }
                yield return null;
            }
        }
    }

    //���� ����� �������� ã��
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

    //����
    public void Attack()
    {
        Debug.Log("�ǄM!!");
        m_MoveState = EEnemyMove.ACTION;
        coMoveState = StartCoroutine(CoAttack());
    }
    IEnumerator CoAttack() 
    {//���� ������ ���� ����
        yield return CUtility.m_WFS_2;
        m_MoveState = EEnemyMove.CHASE;
    }
    //////////////////////////////////////////////////

    private Vector3 BoundaryAngle(float angle)
    {
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));
    }


    //�þ� �� �÷��̾� üũ
    //https://ansohxxn.github.io/unity%20lesson%203/ch7-3/
    private bool TargetInView(Vector3 _playerPos)
    {
        Vector3 dist = _playerPos - this.transform.position;
        //�þ� ��Ÿ� ������ �Գ�?
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


        Vector3 leftBoundary = BoundaryAngle(-viewAngle * 0.5f);  // z �� �������� �þ� ������ ���� ������ŭ �������� ȸ���� ���� (�þ߰��� ���� ��輱)
        Vector3 rightBoundary = BoundaryAngle(viewAngle * 0.5f);  // z �� �������� �þ� ������ ���� ������ŭ ���������� ȸ���� ���� (�þ߰��� ������ ��輱)

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
