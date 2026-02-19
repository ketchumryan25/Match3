using Unity.VisualScripting;
using System.Text.RegularExpressions;

[UnitTitle("Extract Digits As Integer")]
[UnitCategory("Custom")]
public class ExtractDigitsAsInt : Unit
{
    // Input: string
    [DoNotSerialize]
    public ValueInput inputString;

    // Output: integer
    [DoNotSerialize]
    public ValueOutput outputInt;

    protected override void Definition()
    {
        // Define input port
        inputString = ValueInput<string>("String", "");
        // Define output port
        outputInt = ValueOutput<int>("DigitsAsInt", ProcessString);
    }

    private int ProcessString(Flow flow)
    {
        string str = flow.GetValue<string>(inputString);
        // Use Regex to find all digits
        MatchCollection matches = Regex.Matches(str, @"\d+");
        string digitsStr = "";

        foreach (Match match in matches)
        {
            digitsStr += match.Value;
        }

        // Parse the concatenated digits to int, default to 0 if empty
        if (int.TryParse(digitsStr, out int result))
        {
            return result;
        }
        return 0;
    }
}