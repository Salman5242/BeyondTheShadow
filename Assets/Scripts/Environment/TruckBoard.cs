using Infrastructure;
using Infrastructure.Services;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TruckBoard : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI[] _objectivesDescriptionTXT;
    [SerializeField]
    private CanvasGroup[] _objectivesCanvasGroup;

    private const float CompletedTasksAplpha = 0.6f;

    private bool[] _isTaskCompletedSeted = new bool[3];
    private GameObjectivesService _gameObjectivesService;

    void Start()
    {
        _gameObjectivesService = AllServices.Container.Single<GameObjectivesService>();
        SetDescriptionText();
        _gameObjectivesService.OnObjectiveCompleted += SetDoneObjective;
    }

    private void OnDestroy()
    {
        _gameObjectivesService.OnObjectiveCompleted -= SetDoneObjective;
    }
    private void SetDescriptionText()
    {
        for(int i = 0; i< _objectivesDescriptionTXT.Length; i++)
        {
            _objectivesDescriptionTXT[i].text = (i+2) + ") " + _gameObjectivesService.CurrObjectives[i].ObjectiveDescription;
        }
    }

    private void SetDoneObjective()
    {
        for(int i = 0; i< _objectivesDescriptionTXT.Length; i++)
        {
            if (_gameObjectivesService.CurrObjectives[i].IsDone && !_isTaskCompletedSeted[i])
                SetTaskCompleted(i);
        }
    }

    private void SetTaskCompleted(int taskNum)
    {
        _objectivesCanvasGroup[taskNum].alpha = CompletedTasksAplpha;
        _objectivesDescriptionTXT[taskNum].fontStyle = FontStyles.Strikethrough;

        _isTaskCompletedSeted[taskNum] = true;
    }
}
