using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using System;
using Objects;
using UnityEngine.Serialization;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    #region Variables

    public BusManager busManager;
    public CustomerManager customerManager;
    public GridManager gridManager;
    public StandManager standManager;
    [SerializeField] private List<GameObject> customerPrefabsByColor;
    [SerializeField] private List<GameObject> busPrefabsByColor;
    [SerializeField] private Transform gatePrefab;
    [SerializeField] private LevelData levelData;

    [SerializeField] private int busAmountToSpawn;
    [SerializeField] private int gateAmountToSpawn;
    [SerializeField] private int gridAmountToSpawn;

    [SerializeField] private int minGridColumns = 0;
    [SerializeField] private int maxGridColumns = 10;
    [SerializeField] private int minGridRows = 0;
    [SerializeField] private int maxGridRows = 10;

    public event Action OnGameOver;
    public event Action OnGameWin;

    [SerializeField] private bool isGameCompleted = false;

    #endregion

    #region Functions

    /// <summary>
    /// Clears the current scene, destroying any existing game objects and reinitializing the level.
    /// </summary>
    public void ClearScene()
    {
        if (busManager)
        {
            foreach (var bus in busManager.spawnedBuses)
            {
                Destroy(bus.gameObject);
            }
        }

        if (customerManager)
        {
            foreach (var customerAi in customerManager.spawnedCustomers)
            {
                Destroy(customerAi.gameObject);
            }
        }

        if (gridManager)
        {
            gridManager.ClearGrids();
        }

        if (standManager)
        {
            // Assuming there's a method to clear stands
            standManager.ClearStands();
        }

        // Destroy the gates
        if (customerManager.spawnedGates.Count > 0)
        {
            foreach (var gate in customerManager.spawnedGates)
            {
                Destroy(gate);
            }
        }

        InitializeLevel();
    }

    /// <summary>
    /// Initializes the level by calculating the total number of grids and determining the number of buses and gates to spawn.
    /// </summary>
    public void InitializeLevel()
    {
        int totalGrids = levelData.gridColumnsAmount * levelData.gridRowsAmount;

        busAmountToSpawn = totalGrids / 3;
        gateAmountToSpawn = 0; // Gates are set to zero in this logic
        gridAmountToSpawn = totalGrids;

        CreateGrid();
    }

    /// <summary>
    /// Creates the grid and updates the GridManager.
    /// </summary>
    private async void CreateGrid()
    {
        await gridManager.UpdateGrids(levelData.gridColumnsAmount, levelData.gridRowsAmount);
        CreateBus();
    }

    /// <summary>
    /// Creates the buses, assigns them to the BusManager, and assigns their positions.
    /// </summary>
    private void CreateBus()
    {
        for (int i = 0; i < busAmountToSpawn; i++)
        {
            GameObject busPrefab = busPrefabsByColor[UnityEngine.Random.Range(0, busPrefabsByColor.Count)];
            Bus bus = Instantiate(busPrefab).GetComponent<Bus>();
            busManager.spawnedBuses.Add(bus);
        }

        busManager.FirstPositionAssign();
        CreateCustomer();
    }

    /// <summary>
    /// Creates customers and assigns them to gates and the CustomerManager.
    /// </summary>
    private void CreateCustomer()
    {
        List<CustomerAI> tempCustomerList = new List<CustomerAI>();

        // For each bus, instantiate 3 customers of the same color.
        foreach (Bus bus in busManager.spawnedBuses)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject customerPrefab = customerPrefabsByColor[UnityEngine.Random.Range(0, customerPrefabsByColor.Count)];
                CustomerAI customer = Instantiate(customerPrefab).GetComponent<CustomerAI>();
                tempCustomerList.Add(customer);
            }
        }

        // Create gates and add them to the spawned gates list
        for (int i = 0; i < gateAmountToSpawn; i++)
        {
            Transform gate = Instantiate(gatePrefab);
            customerManager.spawnedGates.Add(gate);
        }

        // Assign customers to gates
        foreach (var gate in customerManager.spawnedGates)
        {
            int randomCustomerCount = UnityEngine.Random.Range(1, tempCustomerList.Count);
            List<CustomerAI> assignedCustomers = tempCustomerList.Take(randomCustomerCount).ToList();
            tempCustomerList.RemoveAll(customer => assignedCustomers.Contains(customer));

            foreach (var customer in assignedCustomers)
            {
                customer.transform.position = gate.transform.position;  // Assign customer to gate's position
                customer.gameObject.SetActive(false);  // Disable customer until they are assigned to a bus
            }
        }

        // Remaining customers go to the CustomerManager
        foreach (var customer in tempCustomerList)
        {
            customerManager.spawnedCustomers.Add(customer);
        }

        customerManager.FirstPositionAssign();
    }

    /// <summary>
    /// Checks if the game over condition is met.
    /// </summary>
    public bool CheckGameOver()
    {
        if(isGameCompleted) return false; // TODO: check this one since I am not sure if i should return false or true

        bool allStandsFilled = standManager.occupiedStands.Count >= levelData.peopleStandAmount;

        if (!allStandsFilled)
        {
            OnGameOver?.Invoke();
            isGameCompleted = true;
            return true;
        }

        foreach (var customerAI in standManager.standingPeople)
        {
            // Assuming there's a way to check if a bus color matches the current customer color
            if (customerAI.colorType == busManager.GetCurrentBus().color)
            {
                return false;
            }
        }

        OnGameOver?.Invoke();
        isGameCompleted = true;
        return true;
    }

    /// <summary>
    /// Checks if the win condition is met.
    /// </summary>
    public void CheckWinCondition()
    {
        if(isGameCompleted) return;
        bool busesEmpty = busManager.spawnedBuses.Count == 0;
        bool customersEmpty = customerManager.spawnedCustomers.Count == 0;
        bool allCustomerAnimationsComplete = customerManager.AreAllCustomerAnimationsComplete();
//TODO set a way to check if all gates are empty
// bool allGatesEmpty = customerManager.spawnedGates.All(gate => gate.GetComponent<Gate>().HasNoCustomers());

        if (busesEmpty && customersEmpty && allCustomerAnimationsComplete /*&& allGatesEmpty*/)
        {
            OnGameWin?.Invoke();
        }
    }

    #endregion
}
