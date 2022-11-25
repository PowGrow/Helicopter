using TMPro;
using UnityEngine;

//����� ���������� � �������� DescriptionTextObject, ���������� � ���� ����� �������� ���������� ���������� �� JSON
public class UIPartsDescription : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI             _descriptionComponent;
    [SerializeField] private TextMeshProUGUI            _priceComponent;

    public void SetDescriptionText(string description)
    {
        //������������� �������� ������ � ����������� �� ��������� ����� �������� � ��� ��������������
        _descriptionComponent.text = description;
    }

    public void ClearDescriptionText()
    {
        //������� ��������
        _descriptionComponent.text = string.Empty;
    }

    public void SetPriceText(string price)
    {
        _priceComponent.text = price;
    }
}
