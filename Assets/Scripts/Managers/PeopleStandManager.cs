using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PeopleStandManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<Transform> spawnedStands = new();
    [SerializeField] private List<Transform> occupiedStands = new();
    [Range(0,8)] public int visibleStand = 0;

    [SerializeField, Range(0.1f, 5f)] private float spacing = 2.0f; // Adjustable spacing in the Inspector.
    #endregion

    #region Methods

    /// <summary>
    /// Assigns the first available stand to the appropriate list, positioning them along the X-axis with the spawn point at the center.
    /// </summary>
    public void FirstPositionAssign()
    {
        // Position stands along the X-axis with the spawnPoint as the center.
        PositionStandsAlongXAxis();
    }

    /// <summary>
    /// Checks for an available stand and updates the spawned and occupied lists.
    /// </summary>
    /// <param name="stand">The first available stand, if any.</param>
    /// <returns>True if a stand is available; otherwise, false.</returns>
    public bool CheckForAvailableStand(out Transform stand)
    {
        if (spawnedStands.Count > 0)
        {
            // Assign the first stand in the list.
            stand = spawnedStands[0];
            
            // Remove it from the spawned list and add it to the occupied list.
            spawnedStands.RemoveAt(0);
            occupiedStands.Add(stand);

            return true;
        }

        // No stands available.
        stand = null;
        return false;
    }

    /// <summary>
    /// Positions stands symmetrically along the X-axis with the spawnPoint as the center.
    /// </summary>
    private void PositionStandsAlongXAxis()
    {
        for (var i = 0; i < spawnedStands.Count; i++)
        {
                var stand = spawnedStands[i];
                stand.gameObject.SetActive(true);
                stand.SetParent(null);
                stand.position = spawnPoint.position + Vector3.right * spacing * i;
                Debug.Log(stand.position);
        }

        var center = spawnedStands[^1].position.x/2;
        Debug.Log(center);
        spawnPoint.position = new Vector3(center,0,0);
        foreach (var stand in spawnedStands.Where(stand => stand.gameObject.activeInHierarchy))
        {
            stand.SetParent(spawnPoint);
        }
        spawnPoint.position = Vector3.zero;
    }

    #endregion
}
