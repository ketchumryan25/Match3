using UnityEngine;
using Unity.VisualScripting;
using System.IO;
using System;

[UnitTitle("Write JSON to File")]
[UnitCategory("Custom/File")]
public class WriteJsonToFileUnit : Unit
{

    // Control output after writing
    [DoNotSerialize]
    public ControlOutput output;

    // Inputs
    [DoNotSerialize]
    public ValueInput jsonInput;

    [DoNotSerialize]
    public ValueInput filenameInput; // optional

    protected override void Definition()
    {
        jsonInput = ValueInput<string>("JSON");
        filenameInput = ValueInput<string>("Filename", "savedData.json");

        output = ControlOutput("Done");

        ControlInput trigger = ControlInput("Trigger", (flow) => 
        {
            WriteJsonFile(flow);
            return output;
        });

        Succession(trigger, output);
    }

    private void WriteJsonFile(Flow flow)
    {
        string jsonData = flow.GetValue<string>(jsonInput);
        string filename = flow.GetValue<string>(filenameInput);

        string path = Path.Combine(Application.persistentDataPath, filename);

        try
        {
            File.WriteAllText(path, jsonData);
            Debug.Log($"JSON data written to: {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write JSON to file: {e.Message}");
        }
    }
}