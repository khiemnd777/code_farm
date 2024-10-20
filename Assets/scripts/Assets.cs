using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Assets : MonoBehaviour
{
    public List<AssetItem> items;

    // Start is called before the first frame update
    void Start()
    {
        UpdateMachineListStyles();
    }

    void UpdateMachineListStyles()
    {
        if (items != null && items.Count > 0)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (i % 2 == 0)
                {
                    ColorUtility.TryParseHtmlString("#FFF", out Color color);
                    items[i].image.color = color;
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#D1D1D1", out Color color);
                    items[i].image.color = color;
                }
            }
        }
    }
}
