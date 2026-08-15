using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivesUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent; // the "Content" object from the Scroll View
    [SerializeField] private GameObject objectiveEntryPrefab;
    [SerializeField] private ObjectiveManager objectiveManager;
    //[SerializeField] private List<Objective> allObjectives; // same list QuestManager tracks, or expose a getter from QuestManager instead
    [SerializeField] private ObjectiveDatabase allObjectives;

    private void OnEnable()
    {
        objectiveManager.OnObjectiveCompleted += HandleObjectiveCompleted;
        Refresh();
    }

    private void OnDisable()
    {
        objectiveManager.OnObjectiveCompleted -= HandleObjectiveCompleted;
    }


    public void Refresh()
    {
        foreach(Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach(var objective in allObjectives.Objectives)
        {
            GameObject objectiveEntry = Instantiate(objectiveEntryPrefab, contentParent);
            ObjectiveEntryUI entryUI = objectiveEntry.GetComponent<ObjectiveEntryUI>();
            entryUI.Bind(objective, objectiveManager.IsCompleted(objective.objectiveId));
        }

    }


    private void HandleObjectiveCompleted(Objective objective)
    {
        Refresh();
    }



}
