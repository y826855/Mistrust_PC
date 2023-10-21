using UnityEngine;
using UnityEngine.AI;

// Use physics raycast hit from mouse click to set agent destination
[RequireComponent(typeof(NavMeshAgent))]
public class C_ClickToMove_Custom : MonoBehaviour
{
    [SerializeField] NavMeshAgent m_Agent;
    RaycastHit m_HitInfo = new RaycastHit();


    //public NavMeshSurface m_Surface = null;
    public Transform m_Cursor = null;
    public Transform m_Forword = null;
    [SerializeField] Animator m_Animator = null;
    private Vector3 prevDir = Vector3.zero;
    [SerializeField] bool canTurn = true;
    float defaultSpeed = 8.75f;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        defaultSpeed = m_Agent.speed * m_Agent.speed;
        prevDir = m_Agent.velocity.normalized;
        Vector3 dest = m_Agent.destination;
        //m_Surface.BuildNavMesh();
    }


    Vector3 dest = Vector3.zero;

    //move
    void Update()
    {
        //if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
        if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftShift))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
            {
                m_Agent.destination = m_HitInfo.point;
                m_Cursor.position = m_HitInfo.point;

                Vector3 dest = m_Agent.destination;
                dest.y = m_Agent.transform.position.y;
            }
        }


        if (m_Agent.remainingDistance > m_Agent.stoppingDistance)
        {
            //prevDir = m_Agent.velocity.normalized;
            //float dot = Vector3.Dot(prevDir, m_Agent.desiredVelocity.normalized);
            //if (dot < 0.9f)
            //{
            //    // 아크 코사인을 사용하여 각도를 라디안 단위로 구함
            //    float angleInRadians = Mathf.Acos(dot);
            //    // 라디안을 도(degree)로 변환 (원하는 경우)
            //    float angleInDegrees = angleInRadians * Mathf.Rad2Deg;
            //    Debug.Log(dot);
            //}

            float moveWeight = m_Agent.velocity.sqrMagnitude / defaultSpeed;
            m_Animator.SetFloat("MoveSpeed", moveWeight);

            Vector3 currVel = m_Agent.velocity.normalized;
            Vector3 destVel = m_Agent.desiredVelocity.normalized;

            if (canTurn == true
                && m_Agent.velocity != Vector3.zero
                && m_Agent.desiredVelocity != Vector3.zero
                && currVel != destVel
                )
            {
                float angle = Vector3.Cross(currVel, destVel).y;
                //Debug.Log(angle);
                if (angle > 0.3f || angle < -0.3f) 
                {
                    if (angle > 0) { Debug.Log("좌회전중"); }
                    else Debug.Log("우회전중");
                    m_Animator.SetTrigger("MoveTurn");
                    canTurn = false;
                    //m_Agent.speed = 3.5f;
                }
                
            }
        }
    }

    public void EndMoveTurn() 
    {
        canTurn = true;
        m_Agent.speed = 8.75f;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(m_Cursor.position, 0.5f);

        //Vector3 currPos = this.transform.position;

        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(currPos, currPos + m_Agent.desiredVelocity.normalized);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(currPos, currPos + m_Agent.velocity.normalized);

        //Vector3 forword = Vector3.forward;
        //Vector3 left = (Vector3.right).normalized;
        //Vector3 right = (Vector3.forward - Vector3.right *2f).normalized;
        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(Vector3.zero, Vector3.forward);
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(Vector3.zero, left);
        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(Vector3.zero, right);


        //var r = Vector3.Dot(right, Vector3.forward);
        //var l = Vector3.Dot(left, Vector3.forward);
        //var rc = Vector3.Cross(right, Vector3.forward);
        //var lc = Vector3.Cross(left, Vector3.forward);

        //Debug.Log("L : " + l.ToString() + " LC : " + lc.ToString());
        //Debug.Log("R : " + r.ToString() + " RC : " + rc.ToString());

    }
}
