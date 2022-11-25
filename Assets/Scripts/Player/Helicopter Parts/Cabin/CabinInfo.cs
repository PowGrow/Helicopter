using System;
using System.Collections.Generic;
using UnityEngine;

public class CabinInfo : MonoBehaviour, IHelicopterPart
{
    [SerializeField] private Cabin  _cabin;//������� ������ ScriptableObject, ������ ���� ��������� ��� ��������� ���� ����������
    [SerializeField] private int    _id;//ID ������
    [SerializeField] private float  _cabinHealth = 0f; //���� �������� ������ ��������
    [SerializeField] private float  _cabinArmor = 0f; //���� ����� ������ ��������
    [SerializeField] private Sprite _sprite;//������ ������
    [SerializeField] private int    _price;             //���� ���������
    [SerializeField] private string _description;    //�������� �������

    private Health _playerHealth;

    //��������� �������� ��� ��������� �����
    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }
    public float CabinHealth
    {
        get { return _cabinHealth; }
        set { 
                _cabinHealth = value;
                _playerHealth.MaxHealth = value;
            }
    }
    public float CabinArmor
    {
        get { return _cabinArmor; }
        set { _cabinArmor = value; }
    }
    public Sprite Sprite
    {
        get { return _sprite; }
        set { _sprite = value; }
    }
    public int Price
    {
        get { return _price; }
        set { _price = value; }
    }
    public string Description
    {
        get { return _cabin.Description; }
        set { _description = value; }
    }
    public string Type
    {
        get { return _cabin.GetType().ToString(); }
    }
    public List<GameObject> ObjectList
    {
        get { return this.transform.parent.GetComponent<Cabins>().ObjectList; }
    }

    public GameObject partGameObject
    {
        get { return this.gameObject; }
    }

    //�������� �������� �������� � ����� � ����������� �� ������������� ������ ��������
    private void SetCabinInfoFromContainer() 
    {
        Utils.SetObjectInfo(this, _cabin);
    }

    private void Awake()
    {
        _playerHealth = transform.parent.parent.GetComponent<Health>();
    }

    private void OnEnable()
    {
        SetCabinInfoFromContainer();
    }
}
