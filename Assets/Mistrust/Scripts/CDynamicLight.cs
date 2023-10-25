using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider))]
public class CDynamicLight : MonoBehaviour
{
    [SerializeField] ParticleSystem m_Particle = null;
    [SerializeField] bool toggleLight = true;
    [SerializeField] float m_RecoverDelay = 1f;
    public System.Action<bool> m_FuncToggleCB = null;
    

    private void Start()
    {
        ToggleLight(toggleLight);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (coDelayTurnOn != null) StopCoroutine(coDelayTurnOn);
            coDelayTurnOn = null;
            ToggleLight(false); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (coDelayTurnOn == null && other.tag == "Enemy")
            coDelayTurnOn = StartCoroutine(CoDelayTurnOn());
    }

    Coroutine coDelayTurnOn = null;

    IEnumerator CoDelayTurnOn() 
    {
        yield return CUtility.GetSecD5To2D5(m_RecoverDelay);
        ToggleLight(true);
    }


    //TODO: 꺼지는 연출 켜지는 연출 필요할듯

    public void ToggleLight(bool _toggle) 
    {
        toggleLight = _toggle;

        m_Particle.Clear();

        if (_toggle == true) m_Particle.Play();
        else m_Particle.Stop();

        m_FuncToggleCB(toggleLight);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CDynamicLight))]
    [CanEditMultipleObjects]
    public class Editor_CDynamicLight : Editor 
    {
        List<CDynamicLight> selects = new List<CDynamicLight>();
        private void OnEnable()
        {
            selects.Clear();
            foreach (var it in targets) 
            { 
                selects.Add(it as CDynamicLight); 
            }

            foreach (var it in selects) 
            {
                it.m_Particle = it.GetComponent<ParticleSystem>();
            }
        }
    }
#endif

}
