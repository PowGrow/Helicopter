using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wings : MonoBehaviour
{
    [SerializeField] private List<GameObject> _wingsGameObjectList; //������ ���� �������� ������� ��������

    //��������� �������� ����������� ��������� IHelicopterPart, ���������� ������ ���� �������� ������������� ����������
    public List<GameObject> ObjectList
    {
        get { return _wingsGameObjectList; }
    }
}
