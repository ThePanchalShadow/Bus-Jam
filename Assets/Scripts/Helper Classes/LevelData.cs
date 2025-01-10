using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public struct LevelData
{
    public int peopleStandAmount;
    
    [FormerlySerializedAs("ofColumnsAmount")]
    [Space]   
    [Header("Grid Amount")]
    [Range(0,10)] public int gridColumnsAmount;
    [FormerlySerializedAs("ofRowsAmount")] [Range(0,10)] public int gridRowsAmount;
}
