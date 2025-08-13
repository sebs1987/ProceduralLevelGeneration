using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropVariationGenerator : MonoBehaviour
{
    [ContextMenu("Generate Variation")]
    internal void GenerateVariation()
    {
        PropSelectionXor[] xorSelection = GetComponents<PropSelectionXor>();
        PropVariationOr[] propVariationOrs = GetComponents<PropVariationOr>();

        foreach (PropSelectionXor selection in xorSelection)
        {
            selection.GenerateVaration();
        }

        foreach (PropVariationOr variation in propVariationOrs)
        {
            variation.GenerateVariation();
        }
    }
}
