using UnityEngine;
using System;
using Unity.VisualScripting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

[UnitTitle("Count JSON Root Entries")]
[UnitCategory("Custom/JSON")]
public class CountJsonRootEntriesUnit : Unit
{
    // Input port for JSON string
    [DoNotSerialize]
    public ValueInput jsonInput;

    // Output port for the count
    [DoNotSerialize]
    public ValueOutput countOutput;

    protected override void Definition()
    {
        // Define input port
        jsonInput = ValueInput<string>("JSON");

        // Define output port
        countOutput = ValueOutput<int>("Count", GetRootEntryCount);
    }

    // Cached value for the count
    private int rootEntryCount;

    private int GetRootEntryCount(Flow flow)
    {
        string jsonString = flow.GetValue<string>(jsonInput);
        if (string.IsNullOrEmpty(jsonString))
        {
            Debug.LogError("Input JSON string is null or empty");
            return 0;
        }

        try
        {
            var jsonObject = JObject.Parse(jsonString);
            rootEntryCount = jsonObject.Count;
            return rootEntryCount;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error parsing JSON: {e.Message}");
            return 0;
        }
    }
}