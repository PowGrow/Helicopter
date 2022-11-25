using System.Collections.Generic;
using UnityEngine;

public class Cabins : MonoBehaviour
{
    [SerializeField] private List<GameObject> _cabinGameObjectList; //������ ���� �������� ����� ��������

    //��������� �������� ����������� ��������� IHelicopterPart, ���������� ������ ���� �������� ������������� ����������
    public List<GameObject> ObjectList
    {
        get{ return _cabinGameObjectList; }
    }
}
