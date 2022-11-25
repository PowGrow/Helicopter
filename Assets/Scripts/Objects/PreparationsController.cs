using UnityEngine;

public class PreparationsController : MonoBehaviour
{
    [SerializeField] private UIPreparation _preparationUI;
    private PlayerControls _playerControls; //��������� ������ input "PlayerControls"
    private RaycastHit2D _screenHit; //����� ����� �� ������
    private static IHelicopterPart _selectedHelicopterPart;
    private static GameObject _selectedObject; //��������� ������ ���� ������ ��������
    private static GameObject _selectedPreview; //��������� ������ ������ ������
    private static int _previousId = -1;
    private static PreparationsController _instance;

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
    public UIPreparation PreparationUI
    {
        get { return _preparationUI; }
    }
    
    private void OnScreenClick(Vector3 mousePosition) //���������� ��� ����� �� �����
    {
        //���� ��� ����� ���������� ���������, �� ���������� ����� ��������� ������ � "�����������" ����, ���� �� ���� � �������� ����� ������
        //��������� ��� � ���� "��������� ����� ��������"
        _screenHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero);

        if (_screenHit.collider != null)
        {
            _preparationUI.ClearSelection(SelectedObject, 1);
            _preparationUI.ClearSelection(SelectedPreview, 2);
            IsPartUnlocked();
            SelectedObject = _preparationUI.SelectObject(_screenHit.collider.gameObject, SelectedObject);
            _preparationUI.CreatePreviewButtons(SelectedHelicopterPart.Type);
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
        if(SelectedHelicopterPart.Price <= Managers.Configuration.Currency)
        {
            Managers.Configuration.Currency -= SelectedHelicopterPart.Price;
            Managers.Configuration.UnlockedObjects[SelectedHelicopterPart.Type][SelectedHelicopterPart.Id] = true;
            SelectedPreview.GetComponent<PreviewButton>().RemoveLock();
            _preparationUI.UnlockButton(false);
        }
    }
    private void EscapeButtonPressed() //��� ������� ������ Escape, ������� �� ������ �����������
    {
        IsPartUnlocked();
        if (SelectedObject == null)
        {
            _preparationUI.SwitchMainMenu();
            Utils.TimeScale();
        }
        _preparationUI.ClearSelection(SelectedObject, 1);
        SelectedObject = null;
    }
    public GameObject ChangePart(int partId) //������ ��������� ��������, ���� ����������
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
        UIPreparation.Instance.ClearSelection(selectedPart, 1);
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
