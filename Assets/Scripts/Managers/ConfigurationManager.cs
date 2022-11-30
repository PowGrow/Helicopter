using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConfigurationManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    private int _currency; //���� �������� ���������� ������ ������
    private Dictionary<string, List<bool>> _unlockedObjects = new Dictionary<string, List<bool>>(3); //3 - ���������� ������� ��������: Cabin,Gun,Wing
    private List<KeyValuePair<string, int>> _helicopterData = new List<KeyValuePair<string, int>>(); //������ ������������� ������ �������� (IHelicopterPart.Type,IHelicopterPart.Id)

    public Func<GameObject> OnSettingHelicopterGameObject;
    public Action<GameObject, int> OnClearSelection;
    public Action OnCurrencyChanged;

    public GameObject HelicopterObject
    {
        get 
        {
            return OnSettingHelicopterGameObject.Invoke();
        }
    }
    public Dictionary<string, List<bool>> UnlockedObjects
    {
        get { return _unlockedObjects; }
        set { _unlockedObjects = value; }
    }
    public List<KeyValuePair<string, int>> HelicopterData
    {
        get { return _helicopterData; }
        set { _helicopterData = value; }
    }
    public int Currency
    {
        get { return _currency; }
        set
        {
            _currency = value;
            var sceneId = Managers.Levels.SceneId();
            if (sceneId != 0 && sceneId != 1)
                OnCurrencyChanged?.Invoke();
        }
    }

    public void Startup()
    {
        Debug.Log("Configuration manager starting...");
        Initialize();
        status = ManagerStatus.Started;
    }
    private void Initialize()
    {
        Currency = 200; // HESOYAM like
        DefaultConfiguration();
    }
    public void DefaultConfiguration() //������������� ������ ������������ ��� ������ ����� ����.
    {
        Currency = 0;
        _helicopterData.Clear();
        _unlockedObjects.Clear();
        _unlockedObjects.Add("Cabin", new List<bool>(Enumerable.Repeat(false,4).ToList()));
        _unlockedObjects.Add("Gun", new List<bool>(Enumerable.Repeat(false, 11).ToList()));
        _unlockedObjects.Add("Wing", new List<bool>(Enumerable.Repeat(false, 4).ToList()));
        foreach(var item in _unlockedObjects)
            item.Value[0] = true;
    }
    public void LoadHelicopterData()
    {
        if(_helicopterData.Count != 0)
        {
            PreparationData _preparationData = new PreparationData();
            var helicopterParts = HelicopterObject.GetComponentsInChildren<IHelicopterPart>().Reverse().ToList();
            _preparationData.TryToChangePart(helicopterParts[0], _helicopterData[0].Value);
            _preparationData.TryToChangePart(helicopterParts[1], _helicopterData[1].Value);
            helicopterParts = HelicopterObject.GetComponentsInChildren<IHelicopterPart>().ToList();
            _helicopterData.Reverse();
            GameObject lastSelectedGameObject = null;
            for (int partIndex = 0; partIndex < helicopterParts.Count; partIndex++)
            {
                if (helicopterParts[partIndex].Type == "Gun")
                    lastSelectedGameObject = _preparationData.TryToChangePart(helicopterParts[partIndex], _helicopterData[partIndex].Value);
            }
        }
    }
}