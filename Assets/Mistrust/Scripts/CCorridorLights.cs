using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CCorridorLights : MonoBehaviour
{
    [SerializeField] float m_LightArea = 3f;
    public Light m_Light = null;
    public List<CDynamicLight> m_Lights = new List<CDynamicLight>();
    
    float DefaultIntensity = 0f;
    float IntensityWeight = 0f;

    private void Start()
    {
        //주변 조명 개수 받아두고 그에 따른 조명 켜고 꺼짐의 정도
        DefaultIntensity = m_Light.intensity;
        IntensityWeight = DefaultIntensity / m_Lights.Count;
        m_Light.intensity = 0;
        foreach (var it in m_Lights)
            it.m_FuncToggleCB = CalcLightIntensity;
    }

    void CalcLightIntensity(bool _toggle) 
    {
        m_Light.intensity += _toggle ? IntensityWeight : -IntensityWeight;
    }




#if UNITY_EDITOR
    [CustomEditor(typeof(CCorridorLights))]
    [CanEditMultipleObjects]
    public class Editor_CDynamicLight : Editor
    {
        List<CCorridorLights> selects = new List<CCorridorLights>();
        private void OnEnable()
        {
            selects.Clear();
            foreach (var it in targets)
            {
                selects.Add(it as CCorridorLights);
            }

            foreach (var it in selects)
            {
                it.m_Light = it.GetComponentInChildren<Light>();
                it.m_Lights.Clear();
                it.m_Lights.AddRange(it.GetComponentsInChildren<CDynamicLight>());

                Vector3 pos = it.transform.position;
                it.m_Lights[0].transform.position = new Vector3(pos.x + it.m_LightArea, pos.y, pos.z + it.m_LightArea);
                it.m_Lights[1].transform.position = new Vector3(pos.x - it.m_LightArea, pos.y, pos.z + it.m_LightArea);
                it.m_Lights[2].transform.position = new Vector3(pos.x + it.m_LightArea, pos.y, pos.z - it.m_LightArea);
                it.m_Lights[3].transform.position = new Vector3(pos.x - it.m_LightArea, pos.y, pos.z - it.m_LightArea);
            }
        }
    }
#endif
}
