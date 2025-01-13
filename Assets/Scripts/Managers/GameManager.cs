using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using System;
using Objects;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    #region Variables

    [Header("------Managers' references------")]
    public BusManager busManager;
    public CustomerManager customerManager;
    public GridManager gridManager;
    public StandManager standManager;
    
    [Header("------Prefabs------")]
    [SerializeField] private List<CustomerAI> customerPrefabsByColor;
    [SerializeField] private List<Bus> busPrefabsByColor;
    [SerializeField] private Transform standPrefab;
    
    [Header("------Level Data------")]
    [SerializeField] private LevelData levelData;

    [Header("------ DO NOT MODIFY------")]
    [SerializeField] private int busAmountToSpawn;
    [SerializeField] private int gateAmountToSpawn;
    [SerializeField] private int standAmountToSpawn;

    public event Action OnGameOver;
    public event Action OnGameWin;

    [SerializeField] private bool isGameCompleted = false;

    #endregion

    #region Functions

    /// <summary>
    /// Clears the current scene, destroying any existing game objects and reinitializing the level.
    /// </summary>
    [ContextMenu("Start New Level")]
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
    private void InitializeLevel()
    {
        var totalGrids = levelData.gridColumnsAmount * levelData.gridRowsAmount;

        standAmountToSpawn = levelData.peopleStandAmount;
        busAmountToSpawn = totalGrids / 3;
        gateAmountToSpawn = levelData.gateAmountToSpawn;
        
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
        for (var i = 0; i < busAmountToSpawn; i++)
        {
            var busPrefab = busPrefabsByColor[UnityEngine.Random.Range(0, busPrefabsByColor.Count)];
            var bus = Instantiate(busPrefab);
            busManager.spawnedBuses.Add(bus);
        }

        busManager.FirstPositionAssign();
        CreateStands();
    }

    private void CreateStands()
    {
        for (var i = 0; i < standAmountToSpawn; i++)
        {
            var stand = Instantiate(standPrefab);
            standManager.spawnedStands.Add(stand);
        }
        
        standManager.FirstPositionAssign();
        CreateCustomers();
    }

    /// <summary>
    /// Creates customers and assigns them to gates and the CustomerManager.
    /// </summary>
    private void CreateCustomers()
    {
        var tempCustomerList = new List<CustomerAI>();

        // For each bus, instantiate 3 customers of the same color.
        foreach (var bus in busManager.spawnedBuses)
        {
            for (var i = 0; i < 3; i++)
            {
                var customerPrefab = customerPrefabsByColor.Find(x => x.colorType == bus.colorType);
                var customer = Instantiate(customerPrefab).GetComponent<CustomerAI>();
                tempCustomerList.Add(customer);
            }
        }

        // Create gates and add them to the spawned gates list
        for (var i = 0; i < gateAmountToSpawn; i++)
        {
            var gate = Instantiate(gridManager.visibleGrids[UnityEngine.Random.Range(0, gridManager.visibleGrids.Count)].GetCustomerGate());
            customerManager.spawnedGates.Add(gate);
        }

        // Assign customers to gates
        foreach (var gate in customerManager.spawnedGates)
        {
            var randomCustomerCount = UnityEngine.Random.Range(1, tempCustomerList.Count);
            var assignedCustomers = tempCustomerList.Take(randomCustomerCount).ToList();
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

        var allStandsFilled = standManager.occupiedStands.Count >= levelData.peopleStandAmount;

        if (!allStandsFilled)
        {
            OnGameOver?.Invoke();
            isGameCompleted = true;
            return true;
        }

        if (standManager.standingPeople.Any(customerAI => customerAI.colorType == busManager.GetCurrentBus().colorType))
        {
            return false;
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
        var busesEmpty = busManager.spawnedBuses.Count == 0;
        var customersEmpty = customerManager.spawnedCustomers.Count == 0;
        var allCustomerAnimationsComplete = customerManager.AreAllCustomerAnimationsComplete();
//TODO set a way to check if all gates are empty
// bool allGatesEmpty = customerManager.spawnedGates.All(gate => gate.GetComponent<Gate>().HasNoCustomers());

        if (busesEmpty && customersEmpty && allCustomerAnimationsComplete /*&& allGatesEmpty*/)
        {
            OnGameWin?.Invoke();
        }
    }

    #endregion
}
