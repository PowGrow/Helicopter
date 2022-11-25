using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] Button _continueButton;
    [SerializeField] TextMeshProUGUI _saveGameLabel;
    public void OnNewGameButtonClick()
    {
        if(Managers.Levels.SceneId() != 1)
            Utils.TimeScale();
        Managers.Configuration.DefaultConfiguration();
        Managers.Levels.GoToNext();
    }

    public void OnContinueButtonClick() //��������� ����
    {
        Utils.TimeScale();
        Managers.Data.LoadGameState();
        Managers.Levels.GoToNext();
    }

    private void CheckSave()//��������� ������� ���������� � ��� ��� �������� ��������� ������ ���������� ����.
    {
        if (!Managers.Data.IsThereASave())
            _continueButton.interactable = false;
    }

    public void OnMainMenuButtonClick()//������������ � ������� ����
    {
        Utils.TimeScale();
        Managers.Levels.GoTo(1);
    }
    
    public void OnSaveGameButtonClick()//��������� ����
    {
        Managers.Data.SaveGameState();
        Utils.TimeScale();
        UIPreparation.Instance.ShowSavedGameLabel();
        this.gameObject.SetActive(false);
    }

    public void OnExitButtonClick()//������� �� ����
    {
        Application.Quit();
    }

    private void Awake()
    {
        if(Managers.Levels.SceneId() == 1)
            CheckSave();   
    }

}
