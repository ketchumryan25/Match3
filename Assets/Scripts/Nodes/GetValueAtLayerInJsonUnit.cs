using UnityEngine;
using Unity.VisualScripting;
using Newtonsoft.Json.Linq;
using System;

[UnitTitle("Get Value at Layer in JSON")]
[UnitCategory("Custom/JSON")]
public class GetValueAtLayerInJsonUnit : Unit
{

    public ValueInput jsonInput;
    public ValueInput keyPathInput;
    public ValueOutput valueOutput;

    protected override void Definition()
    {
        jsonInput = ValueInput<string>("JSON");
        keyPathInput = ValueInput<string>("Key Path");
        valueOutput = ValueOutput<string>("Value", GetValueAtPath);
    }

    private string GetValueAtPath(Flow flow)
    {
        string jsonString = flow.GetValue<string>(jsonInput);
        string path = flow.GetValue<string>(keyPathInput);

        //Debug.Log($"JSON Input: {jsonString}");
        //Debug.Log($"Key Path: {path}");

        if (string.IsNullOrEmpty(jsonString))
        {
            //Debug.LogWarning("JSON string is null or empty");
            return "";
        }

        try
        {
            var token = JToken.Parse(jsonString);
            var keys = path.Split('.');

            foreach (var key in keys)
            {
                if (token == null)
                {
                    //Debug.LogWarning($"Path segment '{key}' not found.");
                    return "";
                }
                token = token[key];
            }

            if (token == null)
            {
                //Debug.LogWarning("Key not found at specified path.");
                return "";
            }

            //Debug.Log($"Found value: {token}");
            return token.ToString();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error parsing JSON: {e.Message}");
            return "";
        }
    }
}