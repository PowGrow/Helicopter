using System.Collections.Generic;
using UnityEngine;

public class WingsInfo : MonoBehaviour, IHelicopterPart
{
    [SerializeField] private Wing       _wing; //������� ������ ScriptableObject, ������ ���� ��������� ��� ��������� ���� ����������
    [SerializeField] private int        _Id; //ID �������
    [SerializeField] private List<Vector2>  _mountPoints; //���� ������ ����� ��������� �����
    [SerializeField] private Sprite     _sprite;//������ �����
    [SerializeField] private int        _price;//���� ���������
    [SerializeField] private string     _description;//�������� �����

    //��������� �������� ��� ��������� �����
    public int Id
    {
        get { return _Id; }
        set { _Id = value; }
    }
    public List<Vector2> MountPoints
    {
        get { return _mountPoints; }
        set { _mountPoints = value; }
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
        get { return _wing.Description; }
        set { _description = value; }
    }
    public string Type
    {
        get { return _wing.GetType().ToString(); }
    }
    public int MountPointsCount
    {
        get { return MountPoints.Count; }
    }
    public List<GameObject> ObjectList
    {
        get { return this.transform.parent.GetComponent<Wings>().ObjectList; }
    }

    public GameObject partGameObject
    {
        get { return this.gameObject; }
    }

    private void SetWingsInfoFromContainer()
    {
        Utils.SetObjectInfo(this, _wing);
        Guns.Instance.SetupGunMountPoints(MountPointsCount, MountPoints);
    }

    private void OnEnable()
    {
        if (Guns.Instance != null)
            SetWingsInfoFromContainer();
    }
}
