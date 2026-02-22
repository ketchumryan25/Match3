using Unity.VisualScripting;
using Newtonsoft.Json; // Ensure Newtonsoft.Json is imported
using System.Collections.Generic;
using UnityEngine;

[UnitTitle("Update JSON Key Value")]
[UnitCategory("Custom")]
public class UpdateJsonKeyValueUnit : Unit
{
    // Input: JSON data string
    [DoNotSerialize]
    public ValueInput jsonInput;

    // Input: Key to update
    [DoNotSerialize]
    public ValueInput keyInput;

    // Input: New value for the key
    [DoNotSerialize]
    public ValueInput valueInput;

    // Output: Updated JSON string
    [DoNotSerialize]
    public ValueOutput updatedJsonOutput;

    protected override void Definition()
    {
        // Define input ports
        jsonInput = ValueInput<string>("JSON");
        keyInput = ValueInput<string>("Key");
        valueInput = ValueInput<string>("Value");

        // Define output port
        updatedJsonOutput = ValueOutput<string>("Updated JSON", (flow) =>
        {
            string jsonStr = flow.GetValue<string>(jsonInput);
            string key = flow.GetValue<string>(keyInput);
            string newValue = flow.GetValue<string>(valueInput);

            // Deserialize JSON into dictionary
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonStr);
            if (dict == null)
            {
                dict = new Dictionary<string, object>();
            }

            // Update or add the key with the new value
            dict[key] = newValue;

            // Serialize back to JSON string
            string updatedJson = JsonConvert.SerializeObject(dict);
            return updatedJson;
        });
    }
}