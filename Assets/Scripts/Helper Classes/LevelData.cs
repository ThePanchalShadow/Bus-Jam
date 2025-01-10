using UnityEngine;

[System.Serializable]
public struct LevelData
{
    public int peopleStandAmount;
    
    [Space]   
    [Header("Grid Amount")]
    [Range(0,10)] public int ofColumnsAmount;
    [Range(0,10)] public int ofRowsAmount;
}
