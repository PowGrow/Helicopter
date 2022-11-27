using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreparationsController : MonoBehaviour
{
    private PlayerControls _playerControls; //��������� ������ input "PlayerControls"
    private RaycastHit2D _screenHit; //����� ����� �� ������
    private static IHelicopterPart _selectedHelicopterPart;
    private static GameObject _selectedObject; //��������� ������ ���� ������ ��������
    private static GameObject _selectedPreview; //��������� ������ ������ ������
    private static int _previousId = -1;
    private static PreparationsController _instance;
    private List<Button> PreviewButtonList;
    public Action<GameObject, int>                      OnClearSelection;
    public Func<string,List<Button>>                    OnCreatePreviewButtons;
    public Action<bool>                                 OnUnlockingPreview;
    public Func<GameObject, GameObject, GameObject>     OnSelectObject;
    public Action<GameObject>                           OnSelectPreviewButton;
    public Action                                       OnSwitchingMainMenu;


    private GameObject SelectedObject
    {
        get { return _selectedObject; }
        set
        {
            if(value == null)
                _selectedHelicopterPart = null;
            else
                _selectedHelicopterPart = value.GetComponent<IHelicopterPart>();
            _selectedObject = value;
        }
    }

    public static PreparationsController Instance
    {
        get { return _instance; }
    }
    public static IHelicopterPart SelectedHelicopterPart
    {
        get { return _selectedHelicopterPart; }
    }
    public static GameObject SelectedPreview
    {
        get { return _selectedPreview; }
        set { _selectedPreview = value; }
    }
    public static int SelectedObjectPrevious
    {
        get { return _previousId; }
        set { _previousId = value; }
    }
    
    private void OnScreenClick(Vector3 mousePosition) //���������� ��� ����� �� �����
    {
        //���� ��� ����� ���������� ���������, �� ���������� ����� ��������� ������ � "�����������" ����, ���� �� ���� � �������� ����� ������
        //��������� ��� � ���� "��������� ����� ��������"
        _screenHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero);

        if (_screenHit.collider != null)
        {
            OnClearSelection?.Invoke(SelectedObject, 1);
            OnClearSelection.Invoke(SelectedPreview, 2);
            IsPartUnlocked();
            SelectedObject = OnSelectObject.Invoke(_screenHit.collider.gameObject, SelectedObject);
            PreviewButtonList = OnCreatePreviewButtons.Invoke(SelectedHelicopterPart.Type);
            for(int index = 0; index < PreviewButtonList.Count; index++)
            {
                var buttonIndex = index;
                PreviewButtonList[index].onClick.AddListener(() => OnPreviewButtonClick(buttonIndex));
            }
        }

    }
    private void OnPreviewButtonClick(int index) //����������� ��� ������ ����� �� ������ ��������, ��� �� �������� ������� �����
    {
        if (SelectedObjectPrevious != -1
            && !Managers.Configuration.UnlockedObjects[SelectedHelicopterPart.Type][SelectedHelicopterPart.Id]) //���� ��� ������ ������ ������� ������� �� ��� ������ �� ���������� ������ ������������ �� ����������
            SelectPart(SelectedObjectPrevious);
        SelectPart(index);

        void SelectPart(int partIndex)
        {
            var newSelectedObject = ChangePart(partIndex);
            if (newSelectedObject != null)
                SelectedObject = OnSelectObject.Invoke(newSelectedObject, SelectedObject);
            OnSelectPreviewButton.Invoke(PreviewButtonList[partIndex].gameObject);
        }
    }

    private void DeployButtonClick() //���������� ��� ����� �� ������ "Deploy" ����������� ������� �� ��������� ����� "Game"
    {
        Managers.Configuration.HelicopterData = Managers.Data.GetHelicopterData(Managers.GameObjects.GetObject("Player"));
        Time.timeScale = 1;
        Managers.Levels.GoToNext();
    }
    private void UnlockButtonClick() //���������� ��� ����� �� ������ unlock, ��� ������� ����� ����� ��������
    {
        if(SelectedHelicopterPart.Price <= Managers.Configuration.Currency)
        {
            Managers.Configuration.Currency -= SelectedHelicopterPart.Price;
            Managers.Configuration.UnlockedObjects[SelectedHelicopterPart.Type][SelectedHelicopterPart.Id] = true;
            SelectedPreview.GetComponent<PreviewButton>().RemoveLock();
            OnUnlockingPreview.Invoke(false);
        }
    }
    private void EscapeButtonPressed() //��� ������� ������ Escape, ������� �� ������ �����������
    {
        IsPartUnlocked();
        if (SelectedObject == null)
        {
            OnSwitchingMainMenu.Invoke();
            Utils.TimeScale();
        }
        OnClearSelection.Invoke(SelectedObject, 1);
        SelectedObject = null;
    }
    public GameObject ChangePart(int partId) //������ ��������� ��������, ���� ����������, UPD: ������������ ������ �� ������ ������ ������������
    {
        var objectId = _selectedHelicopterPart.Id;
        if (partId != objectId)
        {
            SelectedObjectPrevious = objectId;
            switch (SelectedHelicopterPart.Type)
            {
                case "Gun":
                    SwitchGun(partId);
                    break;
                default:
                    return SwitchObject(partId);
            }
        }
        return null;
    }
    public GameObject TryToChangePart(IHelicopterPart partToSelect, int partId)
    {
        SelectedObject = partToSelect.partGameObject;
        ChangePart(partId);
        return SelectedObject;
    }
    private void SwitchGun(int id)
    {
        Gun newGun = Resources.Load<Gun>($"ScriptableObjects/{SelectedObject.tag}/part_{id}");
        SelectedObject.GetComponent<GunInfo>().Gun = newGun;
    }
    private GameObject SwitchObject(int id)
    {
        IHelicopterPart helicopterPart = _selectedHelicopterPart;
        SelectedObject.SetActive(false);
        SelectedObject = helicopterPart.ObjectList[id].gameObject;
        SelectedObject.SetActive(true);
        return SelectedObject;
    }
    public void IsPartUnlocked() //����� ������������ ����� ���������� �� ���������� ����������������, � ������ ���� ����� �������� �� ��� ������
    {
        if (SelectedObject != null)
        {
            if (!Managers.Configuration.UnlockedObjects[SelectedHelicopterPart.Type][SelectedHelicopterPart.Id])
                ChangePart(SelectedObjectPrevious);
        }
    }
    public void ClearSelection(GameObject selectedPart)
    {
        OnClearSelection.Invoke(selectedPart, 1);
    }
    private void SetInstance()
    {
        if (_instance == null)
            _instance = this;
    }
    private void Awake()
    {
        SetInstance();
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
