using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class GridManager : MonoBehaviour
{
    [FormerlySerializedAs("m_columns")] [FormerlySerializedAs("columns")] [Range(0,10)] [SerializeField] private int mColumns = 3;
    [FormerlySerializedAs("rows")] [Range(0,10)] [SerializeField] private int mRows = 3;     
    [SerializeField] private Transform gridPrefab;
    [SerializeField] private float offset = 1.1f;
    [SerializeField] private Transform spawnPoint;     

    [SerializeField] private List<Transform> allGrids = new(); 
    public List<Transform> visibleGrids = new();  

    // private void OnValidate()
    // {
    //     if (!Application.isPlaying && gridPrefab && spawnPoint)
    //     {
    //         _ = UpdateGrids(columns, rows);
    //     }
    //     else
    //     {
    //         Debug.LogWarning("GridPrefab or spawnPoint is missing.");
    //     }
    // }

    /// <summary>
    /// Updates the grid positions and visibility based on the row and column count.
    /// </summary>
    /// <param name="columns1"></param>
    /// <param name="rows1"></param>
    public Task UpdateGrids(int columns, int rows)
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
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Calculates the offset required to center the grid.
    /// </summary>
    /// <returns>A Vector3 offset for centering the grid.</returns>
    private Vector3 GetGridOffset()
    {
        var width = (mColumns - 1) * offset;
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

