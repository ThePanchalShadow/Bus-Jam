using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Range(0,10)] [SerializeField] private int columns = 3;
    [Range(0,10)] [SerializeField] private int rows = 3;     
    [SerializeField] private Transform gridPrefab;
    [SerializeField] private float offset = 1.1f;
    [SerializeField] private Transform spawnPoint;     

    [SerializeField] private List<Transform> allGrids = new(); 
    public List<Transform> visibleGrids = new();  

    private void OnValidate()
    {
        if (!Application.isPlaying && gridPrefab && spawnPoint)
        {
            UpdateGrids();
        }
        else
        {
            Debug.LogWarning("GridPrefab or spawnPoint is missing.");
        }
    }

    /// <summary>
    /// Updates the grid positions and visibility based on the row and column count.
    /// </summary>
    private void UpdateGrids()
    {
        var amountOfGridsNeeded = columns * rows;

        UpdateGridsList(amountOfGridsNeeded);

        visibleGrids.Clear();

        var gridCenter = spawnPoint.position;

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                var index = i * columns + j;
                if (index >= allGrids.Count) continue;

                var grid = allGrids[index];
                grid.gameObject.SetActive(true);

                var gridPosition = new Vector3(j * offset, 0, i * offset);

                grid.position = gridCenter + gridPosition - GetGridOffset();

                grid.name = $"Grid ({i}, {j})";

                visibleGrids.Add(grid);
            }
        }

        DisableExtraGrids(amountOfGridsNeeded);
    }

    /// <summary>
    /// Calculates the offset required to center the grid.
    /// </summary>
    /// <returns>A Vector3 offset for centering the grid.</returns>
    private Vector3 GetGridOffset()
    {
        var width = (columns - 1) * offset;
        return new Vector3(width / 2, 0, 0);
    }

    /// <summary>
    /// Ensures the grid list contains enough grids for the specified amount.
    /// </summary>
    /// <param name="amountOfGridsNeeded">The number of grids required.</param>
    private void UpdateGridsList(int amountOfGridsNeeded)
    {
        while (allGrids.Count < amountOfGridsNeeded)
        {
            var newGrid = Instantiate(gridPrefab, spawnPoint);
            allGrids.Add(newGrid);
        }
    }

    /// <summary>
    /// Disables extra grids that are not required.
    /// </summary>
    /// <param name="amountOfGridsNeeded">The number of active grids needed.</param>
    private void DisableExtraGrids(int amountOfGridsNeeded)
    {
        for (var i = amountOfGridsNeeded; i < allGrids.Count; i++)
        {
            var extraGrid = allGrids[i];
            extraGrid.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Clears all grids and resets the lists.
    /// </summary>
    public void ClearGrids()
    {
        foreach (var grid in allGrids.Where(grid => grid))
        {
            DestroyImmediate(grid.gameObject);
        }

        allGrids.Clear();
        visibleGrids.Clear();
    }
}

