using Unity.VisualScripting;
using System.IO;
using UnityEngine;

[UnitTitle("Check JSON File")]
[UnitCategory("Custom")]
public class CheckJsonFileUnit : Unit
{
    // Input control port to trigger the check
    [DoNotSerialize]
    public ControlInput triggerInput;

    // Output control port after check completes
    [DoNotSerialize]
    public ControlOutput triggerOutput;

    // Input port for filename
    [DoNotSerialize]
    public ValueInput filenameInput;

    // Output port for existence of JSON
    [DoNotSerialize]
    public ValueOutput existsOutput;

    // Output port for the file path
    [DoNotSerialize]
    public ValueOutput jsonPathOutput;

    // Output port indicating if a new JSON was created
    [DoNotSerialize]
    public ValueOutput createdOutput;

    protected override void Definition()
    {
        // Define input control port
        triggerInput = ControlInput("Trigger", (flow) =>
        {
            string filename = flow.GetValue<string>(filenameInput);
            string fullPath = Path.Combine(Application.persistentDataPath, filename + ".json");
            bool wasCreated = false;

            if (!File.Exists(fullPath))
            {
                // Create an empty JSON if it doesn't exist
                try
                {
                    File.WriteAllText(fullPath, "{}");
                    wasCreated = true;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to write JSON: {e.Message}");
                }
            }

            // Set output ports with data
            flow.SetValue(existsOutput, File.Exists(fullPath));
            flow.SetValue(jsonPathOutput, fullPath);
            flow.SetValue(createdOutput, wasCreated);

            // Continue flow
            return triggerOutput;
        });

        // Define output control port
        triggerOutput = ControlOutput("Done");

        // Define input port for filename
        filenameInput = ValueInput<string>("Filename");

        // Define output port for existence of JSON
        existsOutput = ValueOutput<bool>("Exists", (flow) =>
        {
            string filename = flow.GetValue<string>(filenameInput);
            string fullPath = Path.Combine(Application.persistentDataPath, filename + ".json");
            return File.Exists(fullPath);
        });

        // Define output port for the file path
        jsonPathOutput = ValueOutput<string>("Path", (flow) =>
        {
            string filename = flow.GetValue<string>(filenameInput);
            return Path.Combine(Application.persistentDataPath, filename + ".json");
        });

        // Define output port for whether a JSON was created
        createdOutput = ValueOutput<bool>("Created", (flow) =>
        {
            string filename = flow.GetValue<string>(filenameInput);
            string fullPath = Path.Combine(Application.persistentDataPath, filename + ".json");
            return File.Exists(fullPath) ? false : true; // True if created now, false if already existed
        });

        // Connect control flow
        Succession(triggerInput, triggerOutput);
    }
}