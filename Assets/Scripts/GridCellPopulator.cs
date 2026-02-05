using UnityEngine;
using System.Collections.Generic;

public class GridCellPopulator : MonoBehaviour
{
    public GridGenerator gridGenerator; // Reference to the grid generator script
    public List<GameObject> prefabOptions; // List of prefabs to randomly assign
    public GameObject targetParent; // Optional parent for the placed prefabs

    private int totalCells; // Total number of grid cells
    private Dictionary<Vector3, GameObject> placedPrefabsDict = new Dictionary<Vector3, GameObject>(); // Store placed prefabs by position

    public void Populate()
    {
        if (gridGenerator == null)
        {
            Debug.LogError("Please assign the GridGenerator reference.");
            return;
        }

        // Generate the grid
        //gridGenerator.GenerateGrid();

        // Get all generated cells
        List<GameObject> cells = new List<GameObject>();
        foreach (Transform child in gridGenerator.transform)
        {
            cells.Add(child.gameObject);
        }

        totalCells = cells.Count;
        List<GameObject> remainingCells = new List<GameObject>(cells);
        ShuffleList(remainingCells);

        // Place prefabs on all cells
        foreach (GameObject cell in remainingCells)
        {
            GameObject prefabToPlace = null;
            int attempts = 0;
            const int maxAttempts = 10;

            do
            {
                prefabToPlace = prefabOptions[Random.Range(0, prefabOptions.Count)];
                attempts++;
            }
            while (violatesThreeInARow(cell.transform.position, prefabToPlace) && attempts < maxAttempts);

            if (prefabToPlace != null)
            {
                Instantiate(prefabToPlace, cell.transform.position, Quaternion.identity, targetParent != null ? targetParent.transform : null);
                // Store placed prefab with position as key
                placedPrefabsDict[cell.transform.position] = prefabToPlace;
            }
        }
    }

    // Shuffle list helper
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randIndex = Random.Range(i, list.Count);
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }

    // Check if placing the prefab at this position causes 3 in a row
    bool violatesThreeInARow(Vector3 position, GameObject prefab)
    {
        int countHorizontal = 1; // Count the current position
        int countVertical = 1;

        // Check horizontally
        countHorizontal += CountSamePrefabInDirection(position, prefab, Vector3.left);
        countHorizontal += CountSamePrefabInDirection(position, prefab, Vector3.right);

        // Check vertically
        countVertical += CountSamePrefabInDirection(position, prefab, Vector3.up);
        countVertical += CountSamePrefabInDirection(position, prefab, Vector3.down);

        // If either line would reach 3 or more, reject placement
        if (countHorizontal >= 3 || countVertical >= 3)
            return true;

        return false;
    }

    int CountSamePrefabInDirection(Vector3 startPos, GameObject prefab, Vector3 direction)
    {
        int count = 0;
        Vector3 checkPos = startPos + direction * gridGenerator.cellSize;

        for (int i = 0; i < 2; i++) // only check up to 2 neighbors in each direction
        {
            if (placedPrefabsDict.TryGetValue(checkPos, out GameObject neighborPrefab))
            {
                if (neighborPrefab.name == prefab.name)
                {
                    count++;
                    checkPos += direction * gridGenerator.cellSize;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
        return count;
    }
}