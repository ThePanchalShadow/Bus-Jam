using System.Collections.Generic;
using UnityEngine;
using System;
using Objects;

public class BusManager : MonoBehaviour
{
    #region Variables

    public List<Bus> spawnedBuses = new();
    [SerializeField] private Transform spawnPoint;
    public Transform defaultTarget;
    [SerializeField] private float offset = 2f; // Modify this offset as needed.

    public event Action NewBusArrived;

    #endregion

    #region Functions

    /// <summary>
    /// Assigns positions to all buses.
    /// </summary>
    public void FirstPositionAssign()
    {
        if (spawnedBuses.Count <= 0) return;
       
        // Assign the first bus to the spawn point
        spawnedBuses[0].transform.position = spawnPoint.position;

        // For each subsequent bus, distribute them along the x-axis with a fixed offset.
        for (var i = 1; i < spawnedBuses.Count; i++)
        {
            var bus = spawnedBuses[i];
            var newPosition = spawnPoint.position;
            newPosition.x += i * (bus.transform.localScale.x / 2f) + offset;
            bus.transform.position = newPosition;
        }
    }

    /// <summary>
    /// Moves each bus to the next bus' position and moves the first bus to the spawn point.
    /// </summary>
    private void AssignPositions()
    {
        if (spawnedBuses.Count > 0)
        {
            // Move each bus to the position of the next bus
            for (var i = 0; i < spawnedBuses.Count - 1; i++)
            {
                var currentBus = spawnedBuses[i];
                var nextBus = spawnedBuses[i + 1];

                // Move current bus to the next bus' position
                currentBus.BusMovingForward(nextBus.transform.position);
            }

            // Move the first bus to the spawn point
            spawnedBuses[^1].BusMovingForward(spawnPoint.position);

            // Invoke the NewBusArrived event after the positions have been updated.
            NewBusArrived?.Invoke();
        }
        else
        {
            // If no buses left, check win condition.
            GameManager.Instance.CheckWinCondition();
        }
    }

    /// <summary>
    /// Updates the position of a bus. Called when a bus goes off-screen.
    /// </summary>
    /// <param name="bus">The bus that will be moved off-screen.</param>
    public void UpdatePositions(Bus bus)
    {
        bus.BusGoingOutOfScreenAndDestroyAnimation();

        // If there are no buses left, check win condition
        if (spawnedBuses.Count <= 0)
        {
            GameManager.Instance.CheckWinCondition();
            Debug.Log($"All Bus are Gone {spawnedBuses.Count}");
        }
        else
        {
            AssignPositions();
        }
    }

    /// <summary>
    /// Removes a bus from the list and plays its out-of-screen animation.
    /// </summary>
    /// <param name="bus">The bus to be removed.</param>
    public void RemoveBus(Bus bus)
    {
        if (!spawnedBuses.Contains(bus)) return;
        spawnedBuses.Remove(bus);
    }

    /// <summary>
    /// Returns the first bus in the list.
    /// </summary>
    public Bus GetCurrentBus()
    {
        return spawnedBuses.Count > 0 ? spawnedBuses[0] : null;
    }

    #endregion
}
