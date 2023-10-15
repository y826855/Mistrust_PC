using UnityEngine;
using UnityEngine.AI;

// Use physics raycast hit from mouse click to set agent destination
[RequireComponent(typeof(NavMeshAgent))]
public class C_ClickToMove_Custom : MonoBehaviour
{
    NavMeshAgent m_Agent;
    RaycastHit m_HitInfo = new RaycastHit();


    //public NavMeshSurface m_Surface = null;
    public Transform m_Cursor = null;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        //m_Surface.BuildNavMesh();
    }


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
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(m_Cursor.position, 0.5f);
    }
}
