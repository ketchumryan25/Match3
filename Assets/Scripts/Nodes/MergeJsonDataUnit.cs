using Unity.VisualScripting;
using Newtonsoft.Json; // Ensure Newtonsoft.Json is available in your project
using System.Collections.Generic;
using UnityEngine;

[UnitTitle("Merge JSON Data")]
[UnitCategory("Custom")]
public class MergeJsonDataUnit : Unit
{
    // Input: First JSON data string
    [DoNotSerialize]
    public ValueInput jsonInput1;

    // Input: Second JSON data string
    [DoNotSerialize]
    public ValueInput jsonInput2;

    // Output: Merged JSON data string
    [DoNotSerialize]
    public ValueOutput mergedJsonOutput;

    protected override void Definition()
    {
        // Define input ports
        jsonInput1 = ValueInput<string>("JSON 1");
        jsonInput2 = ValueInput<string>("JSON 2");

        // Define output port
        mergedJsonOutput = ValueOutput<string>("Merged JSON", (flow) =>
        {
            string jsonStr1 = flow.GetValue<string>(jsonInput1);
            string jsonStr2 = flow.GetValue<string>(jsonInput2);

            // Deserialize both JSON strings into dictionaries
            var dict1 = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonStr1);
            if (dict1 == null)
            {
                dict1 = new Dictionary<string, object>();
            }
            var dict2 = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonStr2);
            if (dict2 == null)
            {
                dict2 = new Dictionary<string, object>();
            }

            // Merge dict2 into dict1 (dict2 overwrites dict1 if keys collide)
            foreach (var kvp in dict2)
            {
                dict1[kvp.Key] = kvp.Value;
            }

            // Serialize the merged dictionary back to a JSON string
            string mergedJson = JsonConvert.SerializeObject(dict1);
            return mergedJson;
        });
    }
}