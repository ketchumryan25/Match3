using UnityEngine;
using Unity.VisualScripting;

[UnitCategory("GameObject")]
[UnitTitle("Get Child Index")]
public class GetChildIndexUnit : Unit
{
    // Input: The child GameObject
    [DoNotSerialize]
    public ValueInput childObject;

    // Output: Index of the child within its parent
    [DoNotSerialize]
    public ValueOutput index;

    protected override void Definition()
    {
        childObject = ValueInput<GameObject>("Child Object");
        index = ValueOutput<int>("Index", GetChildIndex);
        Requirement(childObject, index);
    }

    private int GetChildIndex(Flow flow)
    {
        GameObject child = flow.GetValue<GameObject>(childObject);
        if (child == null)
            return -1;

        Transform parentTransform = child.transform.parent;
        if (parentTransform == null)
            return -1;

        for (int i = 0; i < parentTransform.childCount; i++)
        {
            if (parentTransform.GetChild(i) == child.transform)
                return i;
        }
        return -1; // Not found among parent's children
    }
}