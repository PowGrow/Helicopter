using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPreparation : MonoBehaviour
{
    [SerializeField] private UIDescriptionContainer         _uiDescriptionContainer;//������ ���������� �������� �������
    [SerializeField] private GameObject         _mainMenuObject;//������ �������� ����
    [SerializeField] private GameObject         _deployButton;//������ ������ ������
    [SerializeField] private GameObject         _previewPrefab; //������ ������ ������ ����������� ��������
    [SerializeField] private Transform          _partsPreviewButtonContainer; //������ �� ������������ ��������� ���� ������ ������ ����������� ��������
    [SerializeField] private Color              _chosenObjectColor;//���� ���������� �������
    [SerializeField] private Color              _defaultPreviewColor; //����������� ���� ������� preview ��������
    [SerializeField] private Color              _defaultObjectColor; //����������� ���� �������� ��������
    [SerializeField] private UIPartsDescription _partsDescription;//�������� ���������� �������
    [SerializeField] private TextMeshProUGUI    _gameSavedLabel;//����� "���� ���������"

    private float                   _previewOffset; //����� �� ����������� ��� ������ ������ ������������ ��������� ��������
    private static UIPreparation    _instance; //���� ������ �� ��������� ������ UIPreparation
  
    public static UIPreparation Instance //��������� �������� ���������� ������ �� ��������� ������ UIPreparation
    {
        get { return _instance; }
    }
    public Transform PartsPreviewButtonParent
    {
        get { return _partsPreviewButtonContainer; }
    }
    public Color ChosenObjectColor
    {
        get { return _chosenObjectColor; }
    }
    public Color DefaultPreviewColor
    {
        get { return _defaultPreviewColor; }
    }
    public Color DefaultObjectColor
    {
        get { return _defaultObjectColor; }
    }
    
    private void ClearPreviewButtons()  //������� ��� ��������� ����� ������ ������������� �����������
    {
        if(PartsPreviewButtonParent.childCount > 0)
        {
            foreach (RectTransform child in PartsPreviewButtonParent)
            {
                Destroy(child.gameObject);
            }
        }
    }
    private PreviewButton CreatePreviewButton(GameObject buttonPrefab, float offset, int index, UnityEngine.Object part) //������ ������ �������������, ������������� �� ������ ������� � ������������ ������� � ����������� �� � ������
    {
        var previewButtonObject = Instantiate(buttonPrefab, PartsPreviewButtonParent, false); //��������������
        previewButtonObject.transform.localPosition = new Vector2((previewButtonObject.GetComponent<RectTransform>().sizeDelta.x + offset) * index, 0); //����� ������� �� ��������� �� �������
        var previewButton = previewButtonObject.GetComponent<PreviewButton>();
        previewButton.ItemPreviewImageSprite = part.GetType().GetField("Sprite").GetValue(part) as Sprite; //������������� ������ � ���
        previewButton.ItemPreviewName = part.GetType().GetField("Id").GetValue(part).ToString();
        return previewButton;
    }
    public void CreatePreviewButtons(string objectTag)
    {
        var selectedObjectSprite = PreparationsController.SelectedHelicopterPart.Sprite;
        var buttonCount = Utils.ObjectCount(objectTag);

        for(int buttonIndex = 0; buttonIndex < buttonCount; buttonIndex++)
        {
            var part = Resources.Load($"ScriptableObjects/{objectTag}/part_{buttonIndex}", Type.GetType(objectTag));
            var previewButton = CreatePreviewButton(_previewPrefab,_previewOffset,buttonIndex, part);
            if (Managers.Configuration.UnlockedObjects[objectTag][buttonIndex] == false)
                previewButton.IsLocked = true;
            if (previewButton.ItemPreviewImageSprite.name == selectedObjectSprite.name)
                SelectPreviewButton(previewButton.gameObject);
        }
    }
    private void SelectDraw(GameObject objectToSelect, GameObject previouslySelectedObject, int rendererType)//���������� ��������� ��������� ����� ����� ���������� �������, rendererType 1: SpriteRenderer, 2:Image
    {
        switch (rendererType)
        {
            case 1:
                if (previouslySelectedObject != null)
                    previouslySelectedObject.GetComponent<SpriteRenderer>().color = DefaultObjectColor;
                if (objectToSelect != null)
                    objectToSelect.GetComponent<SpriteRenderer>().color = ChosenObjectColor;
                break;
            case 2:
                if (previouslySelectedObject != null)
                    previouslySelectedObject.GetComponent<Image>().color = DefaultPreviewColor;
                if (objectToSelect != null)
                    objectToSelect.GetComponent<Image>().color = ChosenObjectColor;
                break;
        }
    }
    public GameObject SelectObject(GameObject objectToSelect,GameObject previouslySelectedObject)
    {
        SelectDraw(objectToSelect, previouslySelectedObject, 1); //��������� �������� ����� ������
        return objectToSelect;
    }
    public void SelectPreviewButton(GameObject previewObject)
    {
        SelectDraw(previewObject, PreparationsController.SelectedPreview, 2);
        PreparationsController.SelectedPreview = previewObject;
        var selectedPart = PreparationsController.SelectedHelicopterPart;
        _partsDescription.SetDescriptionText(selectedPart.Description);
        _partsDescription.SetPriceText(selectedPart.Price.ToString());
        SwitchDescriptionContainer(true);
        if (Managers.Configuration.UnlockedObjects[selectedPart.Type][selectedPart.Id])
            _uiDescriptionContainer.UnlockButton = false;
        else
            _uiDescriptionContainer.UnlockButton = true;
    }
    public void ClearSelection(GameObject selectedObject, int rendererType)
    {
        SelectDraw(null, selectedObject, rendererType);
        ClearPreviewButtons();
        SwitchDescriptionContainer(false);
    }
    public void SwitchDescriptionContainer(bool state)
    {
        _uiDescriptionContainer.GameObject.SetActive(state);
        _deployButton.SetActive(!state);
    }
    public void UnlockButton(bool state)
    {
        _uiDescriptionContainer.UnlockButton = state;
    }
    public void SwitchMainMenu()
    {
        _mainMenuObject.SetActive(!_mainMenuObject.activeSelf);
    }
    public void ShowSavedGameLabel()
    {
        _gameSavedLabel.gameObject.SetActive(true);
        StartCoroutine(HideSaveGameLabel(2));
    }
    private IEnumerator HideSaveGameLabel(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        _gameSavedLabel.gameObject.SetActive(false);
        _gameSavedLabel.fontSize = 1;
    }

    private void Awake()
    {
        _previewOffset = (float)Screen.width / (float)Screen.height; //������������� �������� ������� ������ � ����������� ������� ������
        //��������� ������ �� ���� ��������� ������ UIPreparation
        if (_instance == null)
            _instance = this;
    }
}
