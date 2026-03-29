using UnityEngine;
using Unity.VisualScripting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

[UnitTitle("Update Key at Layer in JSON")]
[UnitCategory("Custom/JSON")]
public class UpdateKeyAtLayerInJsonUnit : Unit
{

    // Inputs
    [DoNotSerialize]
    public ValueInput jsonInput; // JSON array or object

    [DoNotSerialize]
    public ValueInput keyPathInput; // Path to key, e.g., "layer1.layer2.key"

    [DoNotSerialize]
    public ValueInput newValueInput; // New value

    // Output
    [DoNotSerialize]
    public ValueOutput jsonOutput;

    protected override void Definition()
    {
        jsonInput = ValueInput<string>("JSON");
        keyPathInput = ValueInput<string>("Key Path");
        newValueInput = ValueInput<string>("New Value");

        jsonOutput = ValueOutput<string>("Updated JSON", GetUpdatedJson);
    }

    private string GetUpdatedJson(Flow flow)
    {
        string jsonString = flow.GetValue<string>(jsonInput);
        string keyPath = flow.GetValue<string>(keyPathInput);
        string newValue = flow.GetValue<string>(newValueInput);

        if (string.IsNullOrEmpty(jsonString))
        {
            Debug.LogError("JSON input is null or empty");
            return "";
        }

        if (string.IsNullOrEmpty(keyPath))
        {
            Debug.LogError("Key path is null or empty");
            return jsonString;
        }

        try
        {
            var token = JToken.Parse(jsonString);

            // Determine if JSON is an array or object
            if (token.Type == JTokenType.Array)
            {
                var array = (JArray)token;
                foreach (var item in array)
                {
                    UpdateValueAtPath(item, keyPath, newValue);
                }
            }
            else
            {
                UpdateValueAtPath(token, keyPath, newValue);
            }

            return token.ToString();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error processing JSON: {e.Message}");
            return jsonString;
        }
    }

    private void UpdateValueAtPath(JToken token, string path, string newVal)
    {
        var keys = path.Split('.');

        JToken current = token;
        for (int i = 0; i < keys.Length; i++)
        {
            if (current == null)
            {
                return; // Path does not exist
            }

            if (i == keys.Length - 1)
            {
                // Last key, update value
                if (current.Type == JTokenType.Object)
                {
                    var obj = (JObject)current;
                    if (obj.ContainsKey(keys[i]))
                    {
                        obj[keys[i]] = JToken.FromObject(newVal);
                    }
                }
            }
            else
            {
                current = current[keys[i]];
            }
        }
    }
}