using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CStateMachine : MonoBehaviour
{
    public enum EEnemyMove { 
        IDLE = 0, 
        CHASE, PATROL, 
        ACTION,//지정되지 않은 행동 할때 쓰자
        STURN, DIE 
    };
    public enum EPlayerMove 
    { 
        IDLE = 0, 
        INTERACTION, MOVE, 
        ACTION, //지정되지 않은 행동 할때 쓰자
        STURN, DIE 
    };



}
