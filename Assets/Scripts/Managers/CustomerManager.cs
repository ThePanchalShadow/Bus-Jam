using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Objects;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerManager : MonoBehaviour
{
    #region Variables

    public List<CustomerAI> spawnedCustomers = new();
    public List<Transform> spawnedGates = new();
    #endregion

    #region Functions

    /// <summary>
    /// Assigns customers to visible grids randomly and updates their positions and paths.
    /// </summary>
    public void FirstPositionAssign()
    {
        // Get reference to visible grids from the GridManager
        var tempGrids = new List<Transform>(GameManager.Instance.gridManager.visibleGrids);

        foreach (var customer in spawnedCustomers)
        {
            if (tempGrids.Count == 0)
            {
                Debug.LogWarning("Not enough grids available for all customers.");
                break;
            }

            // Randomly assign a grid to the customer
            int randomIndex = Random.Range(0, tempGrids.Count);
            Transform grid = tempGrids[randomIndex];
            tempGrids.RemoveAt(randomIndex);

            // Update customer's position
            customer.transform.position = grid.position;
        }

        // Update paths for all customers
        UpdateCustomersPath();
    }

    /// <summary>
    /// Updates paths for all customers.
    /// </summary>
    public void UpdateCustomersPath()
    {
        _ = UpdateCustomersPathAsync();
    }

    /// <summary>
    /// Asynchronously updates paths for all customers.
    /// </summary>
    private async Task UpdateCustomersPathAsync()
    {
        foreach (var customer in spawnedCustomers)
        {
            await customer.UpdateReachability();
        }
    }

    /// <summary>
    /// Removes a customer from the list and disables it.
    /// </summary>
    /// <param name="customer">The customer to remove.</param>
    public void RemoveCustomer(CustomerAI customer)
    {
        if (!spawnedCustomers.Contains(customer)) return;
        spawnedCustomers.Remove(customer);
        DisableCustomer(customer);
        UpdateCustomersPath();
    }

    /// <summary>
    /// Disables a customer by turning off its obstacle and AI components.
    /// </summary>
    /// <param name="customer">The customer to disable.</param>
    private static void DisableCustomer(CustomerAI customer)
    {
        customer.ActivateObstacle(false);

        if (customer.TryGetComponent(out AIPath aiPath)) aiPath.enabled = false;

        if (customer.TryGetComponent(out Seeker seeker)) seeker.enabled = false;
    }

    /// <summary>
    /// Adds a customer to the spawned customers list.
    /// </summary>
    /// <param name="customer">The customer to add.</param>
    public void AddSpawnedCustomer(CustomerAI customer)
    {
        if (!spawnedCustomers.Contains(customer)) spawnedCustomers.Add(customer);
    }

    #endregion

    public bool AreAllCustomerAnimationsComplete()
    {
        return spawnedCustomers.All(customer => customer.AllAnimationsCompleted);
    }
}
