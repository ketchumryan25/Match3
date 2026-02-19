using UnityEngine;
using Unity.VisualScripting;
using System.IO;

// Custom node to find and read a JSON file based on a filename input
[UnitTitle("Find and Read JSON File")]
[UnitCategory("Custom")]
public class FindAndReadJsonNode : Unit
{
    // Input port for filename or relative path
    [DoNotSerialize]
    public ValueInput filename;

    // Optional base directory (can be set in inspector or code)
    [DoNotSerialize]
    public ValueInput baseDirectory;

    // Output port for the JSON content
    [DoNotSerialize]
    public ValueOutput jsonData;

    protected override void Definition()
    {
        // Define input ports
        filename = ValueInput<string>("File Name", "yourfile.json");
        baseDirectory = ValueInput<string>("Base Directory", Application.dataPath); // default to Assets folder

        // Define output port
        jsonData = ValueOutput<string>("JSON Data", GetJsonContent);
    }

    private string GetJsonContent(Flow flow)
    {
        string fileName = flow.GetValue<string>(filename);
        string baseDir = flow.GetValue<string>(baseDirectory);

        // Combine base directory and filename
        string fullPath = Path.Combine(baseDir, fileName);

        if (File.Exists(fullPath))
        {
            try
            {
                string jsonContent = File.ReadAllText(fullPath);
                return jsonContent;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error reading JSON file: {e.Message}");
                return null;
            }
        }
        else
        {
            Debug.LogError($"File not found at path: {fullPath}");
            return null;
        }
    }
}