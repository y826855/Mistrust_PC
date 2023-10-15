using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;


public class CGameManager : CScriptableSingletone<CGameManager>
{
    //public CPlayerInput m_PlayerInput = null;


    //adding on Runtime
    public CPlayer m_Player = null;
    public C_PC_Hosting m_Network = null;

    //adding on Editor
    [Header("---scriptableDatas---")]
    public CCSVDictionary m_Dictionary = null;
    




    float gameSpeed = 1;

    public float m_GameSpeed 
    {
        get { return Time.deltaTime* gameSpeed; }
        set { gameSpeed = value; }
    }
    public float m_OriginGameSpeed 
    {
        get { return gameSpeed; }
    }
    

    public override void BeforeLoadInstance()
    {

    }



    //TODO : 게임 종료시 저장 여기서 하자
    public void Save() 
    {
        Debug.Log("QUIT");

    }

    




    ////////////////////////////////

    public void OnClick_EndGame()
    {
        //SceneManager.LoadSceneAsync("Scene_Lobby");
    }

    public void OnClick_StartGame() 
    {
        //SceneManager.LoadSceneAsync("Scene_Snake");
    }

}
