using UnityEngine;
using UnityEditor;

public class GridGenerator : MonoBehaviour
{
    public int columns = 3;
    public int rows = 3;
    public float padding = 0.5f;
    public Sprite cellSprite;
    public GameObject cellPrefab; // Optional: if you want a custom prefab, otherwise we'll generate a simple sprite object

    // Method to generate the grid
    public void GenerateGrid()
    {
        // Clear previous children
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GameObject cell;
                if (cellPrefab != null)
                {
                    cell = Instantiate(cellPrefab, transform);
                }
                else
                {
                    cell = new GameObject($"Cell_{x}_{y}", typeof(SpriteRenderer));
                    cell.transform.parent = this.transform;
                }

                // Set position
                cell.transform.localPosition = new Vector3(
                    x + x * padding,
                    y + y * padding,
                    0
                );

                // Set sprite
                SpriteRenderer sr = cell.GetComponent<SpriteRenderer>();
                if (sr != null && cellSprite != null)
                {
                    sr.sprite = cellSprite;
                }
            }
        }
    }
}