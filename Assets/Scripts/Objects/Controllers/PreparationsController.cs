using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreparationsController : MonoBehaviour
{
    [SerializeField] private PreparationData _preparationData;
    private PlayerControls _playerControls; // Class input action "PlayerControls" instance
    private RaycastHit2D _screenHit; //Point of screen click
    private List<Button> PreviewButtonList;


    public Func<string, IHelicopterPart, List<Button>>      OnCreatePreviewButtons;
    public Action<bool>                                     OnUnlockingPreview;
    public Func<GameObject, GameObject, GameObject>         OnSelectObject;
    public Action<GameObject,IHelicopterPart,GameObject>    OnSelectPreviewButton;
    public Action                                           OnSwitchingMainMenu;
    public Action<bool>                                     OnPartWasUnlocked;
    public Action<bool>                                     OnSwitchingDescriptionContainer;
    public Action<GameObject, int>                          OnClearSelection;

    private void OnScreenClick(Vector3 mousePosition)
    {
        //���� ��� ����� ���������� ���������, �� ���������� ����� ��������� ������ � "�����������" ����, ���� �� ���� � �������� ����� ������
        //��������� ��� � ���� "��������� ����� ��������"
        //If on click we hit collied, then color early selected object in to "default" color, if it is not null and choose new object coloring it in to "selected" color;
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
    private void OnPreviewButtonClick(int index)
    {
        if (_preparationData.SelectedObjectPrevious != -1
            && !Managers.Configuration.UnlockedObjects[_preparationData.SelectedHelicopterPart.Type][_preparationData.SelectedHelicopterPart.Id]) //If when choosing new part of helicopter selected object was not buyed then select previous buyed part
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
    public void DeployButtonClick() //Called on "Deploy" button click, moving to next scene "Game"
    {
        Managers.Configuration.HelicopterData = Managers.Data.GetHelicopterData(Managers.GameObjects.GetObject("Player"));
        Time.timeScale = 1;
        Managers.Levels.GoToNext();
    }
    public void UnlockButtonClick() //Called on "Unlock" button click, unlocking new helicopter part
    {
        if(_preparationData.SelectedHelicopterPart.Price <= Managers.Configuration.Currency)
        {
            Managers.Configuration.Currency -= _preparationData.SelectedHelicopterPart.Price;
            Managers.Configuration.UnlockedObjects[_preparationData.SelectedHelicopterPart.Type][_preparationData.SelectedHelicopterPart.Id] = true;
            _preparationData.SelectedPreview.GetComponent<PreviewButton>().RemoveLock();
            OnUnlockingPreview.Invoke(true);//Pass true- if we want to hide "Unlock" button.
        }
    }
    private void EscapeButtonPressed()
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
    public void ResetSelectionToBoughtPart() //If when choosing new part of helicopter selected object was not buyed then select previous buyed part
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