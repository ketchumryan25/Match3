using Unity.VisualScripting;
using Newtonsoft.Json; // Make sure Newtonsoft.Json is available in your project
using UnityEngine;
using System.Collections.Generic;

[UnitTitle("Add Nested JSON Object")]
[UnitCategory("Custom")]
public class AddNestedJsonObjectUnit : Unit
{
    // Input for base JSON data string
    [DoNotSerialize]
    public ValueInput baseJsonInput;

    // Input for new JSON data string to add
    [DoNotSerialize]
    public ValueInput newJsonInput;

    // Input for the nested object key name
    [DoNotSerialize]
    public ValueInput nestedKeyInput;

    // Output for the resulting JSON data string
    [DoNotSerialize]
    public ValueOutput resultJsonOutput;

    protected override void Definition()
    {
        // Define input ports
        baseJsonInput = ValueInput<string>("Base JSON");
        newJsonInput = ValueInput<string>("New JSON");
        nestedKeyInput = ValueInput<string>("Nested Key");

        // Define output port
        resultJsonOutput = ValueOutput<string>("Result JSON", (flow) =>
        {
            string baseJsonStr = flow.GetValue<string>(baseJsonInput);
            string newJsonStr = flow.GetValue<string>(newJsonInput);
            string nestedKey = flow.GetValue<string>(nestedKeyInput);

            // Deserialize base JSON into a dictionary
            var baseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(baseJsonStr);
            if (baseDict == null)
            {
                baseDict = new Dictionary<string, object>();
            }

            // Deserialize new JSON into a dictionary
            var newDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(newJsonStr);
            if (newDict == null)
            {
                newDict = new Dictionary<string, object>();
            }

            // Add the new JSON as a nested object under the specified key
            baseDict[nestedKey] = newDict;

            // Serialize back to JSON string
            string resultJson = JsonConvert.SerializeObject(baseDict);
            return resultJson;
        });
    }
}