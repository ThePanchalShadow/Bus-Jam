using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public struct LevelData
{
    public int peopleStandAmount;
    public int gateAmountToSpawn;
    [Space]   
    [Header("Grid Amount")]
    [Range(0,10)] public int gridColumnsAmount;
    [Range(0,10)] public int gridRowsAmount;
}
