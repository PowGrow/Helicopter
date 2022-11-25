using System;
using UnityEngine;
using UnityEngine.UI;

public class PreviewButton : MonoBehaviour
{
    [SerializeField] private Image _previewButtonImage;
    [SerializeField] private GameObject _itemPreview;
    [SerializeField] private GameObject _lock;

    private UIPreparation _preparationUI;

    public Sprite ItemPreviewImageSprite
    {
        get { return _itemPreview.GetComponent<Image>().sprite; }
        set 
        {
            _itemPreview.GetComponent<Image>().sprite = value;
        }
    }
    public string ItemPreviewName
    {
        get { return this.transform.name; }
        set { this.transform.name = value; }
    }
    public Image PreviewButtonImage
    {
        get { return _previewButtonImage; }
    }
    public bool IsLocked
    {
        get { return _lock.activeSelf; }
        set { _lock.SetActive(value); }
    }
    private int Id
    {
        get { return Convert.ToInt32(this.ItemPreviewName); }
    }

    public void OnPreviewButtonClick() //����������� ��� ������ ����� �� ������ ��������, ��� �� �������� ������� �����
    {
        if (PreparationsController.SelectedObjectPrevious != -1
            && !Managers.Configuration.UnlockedObjects[PreparationsController.SelectedHelicopterPart.Type][PreparationsController.SelectedHelicopterPart.Id]) //���� ��� ������ ������ ������� ������� �� ��� ������ �� ���������� ������ ������������ �� ����������
            Select(PreparationsController.SelectedObjectPrevious);
        Select(Id);
    }

    public void Select(int id)
    {
        GameObject selectedObject = PreparationsController.Instance.ChangePart(id); //� ����������� �� ���� ���������� ����� ������� ��������� ������ ������ �����
        if (selectedObject != null) _preparationUI.SelectObject(selectedObject, null); //���������� ���������� ��������� ������� � ����������� ����, � ����-��������� � ���� "���������� ����������"
        _preparationUI.SelectPreviewButton(this.gameObject);
    }

    public void RemoveLock()
    {
        if(IsLocked)
            _lock.SetActive(false);
    }

    private void Start()
    {
        _preparationUI = UIPreparation.Instance;
    }
}
