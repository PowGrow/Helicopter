using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreparationsController : MonoBehaviour
{
    [SerializeField] private PreparationData _preparationData;
    private PlayerControls _playerControls; //��������� ������ input action "PlayerControls"
    private RaycastHit2D _screenHit; //����� ����� �� ������
    private List<Button> PreviewButtonList;


    public Func<string, IHelicopterPart, List<Button>>      OnCreatePreviewButtons;
    public Action<bool>                                     OnUnlockingPreview;
    public Func<GameObject, GameObject, GameObject>         OnSelectObject;
    public Action<GameObject,IHelicopterPart,GameObject>    OnSelectPreviewButton;
    public Action                                           OnSwitchingMainMenu;
    public Action<bool>                                     OnPartWasUnlocked;
    public Action<bool>                                     OnSwitchingDescriptionContainer;
    public Action<GameObject, int>                          OnClearSelection;

    private void OnScreenClick(Vector3 mousePosition) //���������� ��� ����� �� �����
    {
        //���� ��� ����� ���������� ���������, �� ���������� ����� ��������� ������ � "�����������" ����, ���� �� ���� � �������� ����� ������
        //��������� ��� � ���� "��������� ����� ��������"
        _screenHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero);

        if (_screenHit.collider != null)
        {
            OnClearSelection?.Invoke(_preparationData.SelectedObject, 1);
            OnClearSelection.Invoke(_preparationData.SelectedPreview, 2);
            OnSwitchingDescriptionContainer?.Invoke(true);
            OnUnlockingPreview?.Invoke(true);
            ResetSelectionToBoughtPart();
            _preparationData.SelectedObject = OnSelectObject.Invoke(_screenHit.collider.gameObject, _preparationData.SelectedObject);
            PreviewButtonList = OnCreatePreviewButtons.Invoke(_preparationData.SelectedHelicopterPart.Type, _preparationData.SelectedHelicopterPart);
            for(int index = 0; index < PreviewButtonList.Count; index++)
            {
                var buttonIndex = index;
                PreviewButtonList[index].onClick.AddListener(() => OnPreviewButtonClick(buttonIndex));
                if (Convert.ToInt32(PreviewButtonList[index].gameObject.name) == _preparationData.SelectedHelicopterPart.Id)
                {
                    OnSelectPreviewButton.Invoke(PreviewButtonList[index].gameObject, _preparationData.SelectedHelicopterPart, _preparationData.SelectedPreview);
                    _preparationData.SelectedPreview = PreviewButtonList[index].gameObject;
                }
            }
        }
    }
    private void OnPreviewButtonClick(int index) //����������� ��� ������ ����� �� ������ ��������, ��� �� �������� ������� �����
    {
        if (_preparationData.SelectedObjectPrevious != -1
            && !Managers.Configuration.UnlockedObjects[_preparationData.SelectedHelicopterPart.Type][_preparationData.SelectedHelicopterPart.Id]) //���� ��� ������ ������ ������� ������� �� ��� ������ �� ���������� ������ ������������ �� ����������
            SelectPart(_preparationData.SelectedObjectPrevious);
        SelectPart(index);

        void SelectPart(int partIndex)
        {
            var newSelectedObject = _preparationData.ChangePart(partIndex);
            if (newSelectedObject != null)
                _preparationData.SelectedObject = OnSelectObject.Invoke(newSelectedObject, _preparationData.SelectedObject);
            OnSelectPreviewButton.Invoke(PreviewButtonList[partIndex].gameObject, _preparationData.SelectedHelicopterPart, _preparationData.SelectedPreview);
            _preparationData.SelectedPreview = PreviewButtonList[partIndex].gameObject;
            OnSwitchingDescriptionContainer.Invoke(true);
            OnPartWasUnlocked.Invoke(Managers.Configuration.UnlockedObjects[_preparationData.SelectedHelicopterPart.Type][_preparationData.SelectedHelicopterPart.Id]);
        }
    }
    public void DeployButtonClick() //���������� ��� ����� �� ������ "Deploy" ����������� ������� �� ��������� ����� "Game"
    {
        Managers.Configuration.HelicopterData = Managers.Data.GetHelicopterData(Managers.GameObjects.GetObject("Player"));
        Time.timeScale = 1;
        Managers.Levels.GoToNext();
    }
    public void UnlockButtonClick() //���������� ��� ����� �� ������ unlock, ��� ������� ����� ����� ��������
    {
        if(_preparationData.SelectedHelicopterPart.Price <= Managers.Configuration.Currency)
        {
            Managers.Configuration.Currency -= _preparationData.SelectedHelicopterPart.Price;
            Managers.Configuration.UnlockedObjects[_preparationData.SelectedHelicopterPart.Type][_preparationData.SelectedHelicopterPart.Id] = true;
            _preparationData.SelectedPreview.GetComponent<PreviewButton>().RemoveLock();
            OnUnlockingPreview.Invoke(true);//������� true- ���� ����� ������ ������ Unlock
        }
    }
    private void EscapeButtonPressed() //��� ������� ������ Escape, ������� �� ������ �����������
    {
        ResetSelectionToBoughtPart();
        if (_preparationData.SelectedObject == null)
        {
            OnSwitchingMainMenu.Invoke();
            Utils.TimeScale();
        }
        OnClearSelection.Invoke(_preparationData.SelectedObject, 1);
        OnSwitchingDescriptionContainer.Invoke(false);
        _preparationData.SelectedObject = null;
    }
    public void ResetSelectionToBoughtPart() //����� ������������ ����� ���������� �� ���������� ����������������, � ������ ���� ����� �������� �� ��� ������
    {
        if (_preparationData.SelectedObject != null)
        {
            if (!Managers.Configuration.UnlockedObjects[_preparationData.SelectedHelicopterPart.Type][_preparationData.SelectedHelicopterPart.Id])
                _preparationData.ChangePart(_preparationData.SelectedObjectPrevious);
        }
    }
    private void Awake()
    {
        _playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        _playerControls.Player.Mouse.performed += callbackContext => OnScreenClick(Input.mousePosition);
        _playerControls.GUI.Escape.performed += callbackContext => EscapeButtonPressed();
        _playerControls.Enable();
}
    private void OnDisable()
    {
        _playerControls.Disable();
    }
}