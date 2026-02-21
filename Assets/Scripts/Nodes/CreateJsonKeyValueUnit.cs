using Unity.VisualScripting;
using Newtonsoft.Json; // Make sure Newtonsoft.Json is available in your project
using UnityEngine;

[UnitTitle("Create JSON Key-Value")]
[UnitCategory("Custom")]
public class CreateJsonKeyValueUnit : Unit
{
    // Input for key name
    [DoNotSerialize]
    public ValueInput keyNameInput;

    // Input for value string
    [DoNotSerialize]
    public ValueInput valueStringInput;

    // Output JSON string
    [DoNotSerialize]
    public ValueOutput jsonStringOutput;

    protected override void Definition()
    {
        // Define input ports
        keyNameInput = ValueInput<string>("Key");
        valueStringInput = ValueInput<string>("Value");

        // Define output port
        jsonStringOutput = ValueOutput<string>("JSON String", (flow) =>
        {
            string key = flow.GetValue<string>(keyNameInput);
            string value = flow.GetValue<string>(valueStringInput);

            // Create an anonymous object with the key-value pair
            var jsonObject = new System.Collections.Generic.Dictionary<string, string>
            {
                { key, value }
            };

            // Convert to JSON string
            string jsonString = JsonConvert.SerializeObject(jsonObject);
            return jsonString;
        });
    }
}