using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public int rows = 5; // Number of rows
    public int columns = 5; // Number of columns
    public float cellSize = 1f; // Size of each cell
    public GameObject cellPrefab; // Prefab for the cell
    public Vector3 startPosition = Vector3.zero; // Starting position for grid generation

    void Start()
    {

    }

    public void GenerateGrid()
    {
        if (cellPrefab == null)
        {
            Debug.LogError("Please assign a cell prefab.");
            return;
        }

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Shift rows along the Y-axis
                Vector3 position = startPosition + new Vector3(col * cellSize, -row * cellSize, 0);
                Instantiate(cellPrefab, position, Quaternion.identity, transform);
            }
        }
    }
}