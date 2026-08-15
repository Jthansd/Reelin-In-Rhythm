using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectiveDatabase", menuName = "Scriptable Objects/ObjectiveDatabase")]
public class ObjectiveDatabase : ScriptableObject
{
    [SerializeField] private List<Objective> objectiveList;

    public List<Objective> Objectives => objectiveList;
}
