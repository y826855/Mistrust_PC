using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CStateMachine : MonoBehaviour
{
    public enum EEnemyMove { 
        IDLE = 0, 
        CHASE, PATROL, 
        ACTION,//�������� ���� �ൿ �Ҷ� ����
        STURN, DIE 
    };
    public enum EPlayerMove 
    { 
        IDLE = 0, 
        INTERACTION, MOVE, 
        ACTION, //�������� ���� �ൿ �Ҷ� ����
        STURN, DIE 
    };



}
