using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCreator : MonoBehaviour
{
    // This method creates and loads a new scene
    public void CreateNewScene()
    {
        // Generate a unique scene name based on time or any logic
        string newSceneName = "Scene_" + System.DateTime.Now.ToString("yyyyMMddHHmmss");

        // Create a new empty scene
        Scene newScene = SceneManager.CreateScene(newSceneName);

        // Optionally, set this scene as active
        SceneManager.SetActiveScene(newScene);

        Debug.Log("Created and loaded new scene: " + newSceneName);
    }
}