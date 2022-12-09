using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Guns : MonoBehaviour, IShooter
{
    [SerializeField] private List<GameObject> _gunsGameObjectList; //������ �������� ���� �����
    [SerializeField] private List<GunInfo> _gunInfoList;
    [SerializeField] private Buffs _buffs; //������ �� ��������� ������ ��������������� ����� ���������� ������� �� ����� ������

    private static Guns _instance;

    public static Guns Instance
    {
        get { return _instance;}
    }
    public List<GameObject> ObjectList
    {
        get { return _gunsGameObjectList; }
    }
    public List<GunInfo> GunInfoList
    {
        get { return _gunInfoList; }
    }

    public void Fire()
    {
        TryToShootBullet(this);
    }
    private void TryToShootBullet(Guns guns)    //�������� ����������, ���� �������, �� ������������� ������� ����������� �������� � ����, � ����������� �� ��������� �����
    {
        for (int currentGunIndex = 0; currentGunIndex < _gunsGameObjectList.Count; currentGunIndex++)
        {
            if (_gunsGameObjectList[currentGunIndex].activeSelf)
            {
                var bullet = _gunInfoList[currentGunIndex].GunTrigger.Click(_gunsGameObjectList[currentGunIndex].transform.position, _gunsGameObjectList[currentGunIndex].transform.rotation);
                if(bullet != null)
                {
                    var projectile = Managers.GameObjects.Projectiles.Last();
                    projectile.DamageMultiplier = _gunInfoList[currentGunIndex].DamageMultiplier;
                    projectile.DamageModificator = _buffs.PowerModificator;
                    projectile.SpeedModificator = _buffs.SpeedModificator;
                }
            }
        }
    }
    public void SetupGunMountPoints(int gunsSummCount, List<Vector2> wingsMountPoints)    //������������� ��� ����� �� ������ ����� � ����������� �� �� ����������
    {
        //������� ��������� ��� �����
        foreach(GameObject gun in _gunsGameObjectList)
        {
            gun.SetActive(false);
        }
        //����� ���������� ������ ������ � ������������� �� ������ ���������
        for(int i = 0; i < _gunsGameObjectList.Count; i++)
        {
            if (i < gunsSummCount)
            {
                _gunsGameObjectList[i].transform.localPosition = wingsMountPoints[i];
                _gunsGameObjectList[i].SetActive(true);
            }
        }
    }
    private void LoadGunInfo()    //��������� ���������� � ������ ��� ������
    {
        _gunInfoList.Clear();
        for(int gunIndex = 0; gunIndex < _gunsGameObjectList.Count; gunIndex++)
        {
            var gunInfo = _gunsGameObjectList[gunIndex].GetComponent<GunInfo>();
            if (gunInfo == null)
                _gunInfoList.Add(null);
            else
                _gunInfoList.Add(gunInfo);
        }
    }
    private void SetInstance()
    {
        if (_instance == null)
            _instance = this;
    }

    private void Awake()
    {
        SetInstance();
    }
    private void Start()
    {
        LoadGunInfo();
    }

}
