using UnityEngine;
using Unity.VisualScripting;
using Newtonsoft.Json.Linq; // Ensure Newtonsoft.Json is included in your project

[UnitTitle("Parse JSON Nested Property")]
[UnitCategory("Data")]
public class ParseJsonNestedPropertyUnit : Unit
{
    // Input: JSON string
    [DoNotSerialize]
    public ValueInput jsonStringInput;

    // Input: Key path for nested property (e.g., "player.health" or "data.items[0].name")
    [DoNotSerialize]
    public ValueInput keyPathInput;

    // Output: JSON string of the nested object/property
    [DoNotSerialize]
    public ValueOutput outputJson;

    protected override void Definition()
    {
        jsonStringInput = ValueInput<string>("JSON String");
        keyPathInput = ValueInput<string>("Key Path");
        outputJson = ValueOutput<string>("Nested JSON", GetNestedJson);
    }

    private string GetNestedJson(Flow flow)
    {
        string jsonString = flow.GetValue<string>(jsonStringInput);
        string keyPath = flow.GetValue<string>(keyPathInput);

        if (string.IsNullOrEmpty(jsonString) || string.IsNullOrEmpty(keyPath))
            return null;

        try
        {
            var jsonObject = JObject.Parse(jsonString);
            JToken token = jsonObject.SelectToken(keyPath);
            if (token != null)
            {
                // Return the nested object/property as JSON string
                return token.ToString();
            }
            else
            {
                return null; // Key path not found
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON parsing error: {e.Message}");
            return null;
        }
    }
}