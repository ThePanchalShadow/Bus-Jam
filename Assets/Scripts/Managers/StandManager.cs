using System.Collections.Generic;
using System.Linq;
using Objects;
using UnityEngine;

public class StandManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform spawnPoint;
    public List<Transform> spawnedStands = new();
    public List<Transform> occupiedStands = new();
    public List<CustomerAI> standingPeople = new();

    [SerializeField, Range(0.1f, 5f)] private float spacing = 2.0f; // Adjustable spacing in the Inspector.

    #endregion

    #region Methods

    /// <summary>
    /// Positions stands symmetrically along the X-axis with the spawnPoint as the center.
    /// </summary>
    public void FirstPositionAssign()
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

    /// <summary>
    /// Checks for an available stand and updates the spawned and occupied lists.
    /// </summary>
    /// <param name="stand">The first available stand, if any.</param>
    /// <returns>True if a stand is available; otherwise, false.</returns>
    public bool CheckForAvailableStand(CustomerAI customer, out Transform stand)
    {
        if (spawnedStands.Count > 0)
        {
            // Assign the first stand in the list.
            stand = spawnedStands[0];
            
            // Remove it from the spawned list and add it to the occupied list.
            spawnedStands.RemoveAt(0);
            occupiedStands.Add(stand);

            standingPeople.Add(customer);
            
            return true;
        }

        // No stands available.
        stand = null;
        return false;
    }
    #endregion

    public void ClearStands()
    {
        spawnedStands.Clear();
        occupiedStands.Clear();
    }
}
