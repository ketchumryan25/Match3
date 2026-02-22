using UnityEngine;
using Unity.VisualScripting; // Make sure to include this for custom units
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[UnitCategory("Custom/JSON")]
[UnitTitle("Update JSON Entry to File")]
public class UpdateJsonEntry : Unit
{
    // Inputs
    private ValueInput jsonStringInput;
    private ValueInput filenameInput;
    private ValueInput newEntryKeyInput;

    // Control outputs
    private ControlOutput triggerOutput;

    protected override void Definition()
    {
        // Define inputs
        jsonStringInput = ValueInput<string>("JSON String");
        filenameInput = ValueInput<string>("Filename");
        newEntryKeyInput = ValueInput<string>("New Entry Key");

        // Define control output
        triggerOutput = ControlOutput("Trigger Output");

        // Define control input for execution
        ControlInput triggerControl = ControlInput("Trigger", (flow) =>
        {
            Execute(flow);
            return triggerOutput; // Continue flow after execution
        });

        // Connect control flow
        Succession(triggerControl, triggerOutput);
    }

    private void Execute(Flow flow)
    {
        string jsonStr = flow.GetValue<string>(jsonStringInput);
        string filename = flow.GetValue<string>(filenameInput);
        string newKey = flow.GetValue<string>(newEntryKeyInput);

        // Format JSON string with indentation
        string formattedJsonString = FormatJsonString(jsonStr);

        // Reformat string lists without indentation
        string reformattedJsonString = ReformatStringLists(formattedJsonString);

        // Load existing JSON file or create new
        string filePath = Path.Combine(defaultPath, filename);
        JObject jsonObj;

        if (File.Exists(filePath))
        {
            string existingContent = File.ReadAllText(filePath);
            jsonObj = JObject.Parse(existingContent);
        }
        else
        {
            jsonObj = new JObject();
        }

        // Parse the new JSON data
        JObject newEntry;
        try
        {
            newEntry = JObject.Parse(reformattedJsonString);
        }
        catch
        {
            Debug.LogError("Invalid JSON string input.");
            return;
        }

        // Check if the key exists; update if it does, add if it doesn't
        if (jsonObj.ContainsKey(newKey))
        {
            jsonObj[newKey] = newEntry; // Update existing entry
        }
        else
        {
            jsonObj.Add(newKey, newEntry); // Add new entry
        }

        // Save back to file
        File.WriteAllText(filePath, jsonObj.ToString(Formatting.Indented));
    }

    private string FormatJsonString(string json)
    {
        try
        {
            var parsedJson = JToken.Parse(json);
            return parsedJson.ToString(Formatting.Indented);
        }
        catch
        {
            Debug.LogError("Invalid JSON string input.");
            return json;
        }
    }

    private string ReformatStringLists(string json)
    {
        try
        {
            var token = JToken.Parse(json);
            ReformatLists(token);
            return token.ToString(Formatting.Indented);
        }
        catch
        {
            Debug.LogError("Error parsing JSON during reformatting.");
            return json;
        }
    }

    private void ReformatLists(JToken token)
    {
        if (token.Type == JTokenType.Object)
        {
            foreach (var property in ((JObject)token).Properties())
            {
                ReformatLists(property.Value);
            }
        }
        else if (token.Type == JTokenType.Array)
        {
            var array = (JArray)token;
            // Check if all elements are strings
            bool allStrings = true;
            foreach (var item in array)
            {
                if (item.Type != JTokenType.String)
                {
                    allStrings = false;
                    break;
                }
            }
            // No explicit action needed here; formatting is controlled during serialization
            if (allStrings)
            {
                // No action needed; array will serialize as a single line if formatting is set accordingly
            }
            else
            {
                foreach (var item in array)
                {
                    ReformatLists(item);
                }
            }
        }
    }

    private string defaultPath => Application.persistentDataPath;
}