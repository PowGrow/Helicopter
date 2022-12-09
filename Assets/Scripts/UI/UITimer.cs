using System;
using TMPro;
using UnityEngine;
//����� ��������� ���������� � ���������� ������� ����� ������
public class UITimer : MonoBehaviour
{
    [SerializeField] private GameObject _container;
    private Buffs _buffs;

    private TextMeshProUGUI _timerLabel;

    private void Awake()
    {
        _buffs = Managers.GameObjects.GetObject("Player").GetComponent<Buffs>();
        _timerLabel = this.GetComponent<TextMeshProUGUI>();
    }

    private void FixedUpdate()
    {
        //���� ����������� �������, �� � ����������� �� ���� ��� ���������� ��������� ������� ���������� � ������� �����
        if(_buffs.DamageModificator > 1)
        {
            switch(_container.name)
            {
                case ("Damage label"):
                    _timerLabel.text = Math.Round((decimal)_buffs.DamageTimer, 0).ToString();
                    break;
                case ("Speed label"):
                    _timerLabel.text = Math.Round((decimal)_buffs.SpeedTimer, 0).ToString();
                    break;
                default:
                    _timerLabel.text = " ";
                    break;
            }
        }
    }
}
