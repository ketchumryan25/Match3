using UnityEngine;
using Unity.VisualScripting;

[UnitCategory("Math/Number")]
[UnitTitle("Aspect Ratio")]
public class AspectRatioNode : Unit
{
    [DoNotSerialize]
    public ValueInput width;

    [DoNotSerialize]
    public ValueInput height;

    [DoNotSerialize]
    public ValueOutput aspectRatio;

    protected override void Definition()
    {
        width = ValueInput<float>(nameof(width), 1f);
        height = ValueInput<float>(nameof(height), 1f);
        
        // Ensure height is not zero
        aspectRatio = ValueOutput<float>("aspectRatio", (flow) =>
        {
            float w = flow.GetValue<float>(width);
            float h = flow.GetValue<float>(height);
            return h != 0 ? w / h : 0;
        });

        // Optionally, define dependencies
        Requirement(width, aspectRatio);
        Requirement(height, aspectRatio);
    }
}   