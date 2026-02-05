using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapPrefabs : MonoBehaviour
{
    public GameObject targetPrefab; // The prefab that will swap with the attached object

    private bool isTouchEnabled = false; // Flag to check if touch input is enabled

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && isTouchEnabled)
        {
            // Swap the positions of the two objects
            SwapPositions();
        }
    }

    // Function to swap the positions of the two objects
    private void SwapPositions()
    {
        // Check if the target prefab is nearby
        Vector3 targetPosition = targetPrefab.transform.position;
        float distance = Vector3.Distance(transform.position, targetPosition);

        // Only swap positions if the target prefab is within a certain distance
        if (distance < 6.0f)
        {
            // Swap the positions of the two objects
            GameObject temp = Instantiate(targetPrefab, transform.position, Quaternion.identity);
            Destroy(targetPrefab);

            targetPrefab = Instantiate(gameObject, targetPosition, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    // Function to toggle touch input
    public void ToggleTouchInput()
    {
        isTouchEnabled =!isTouchEnabled;
    }
}