using TMPro;
using UnityEngine;

public class FieldTick : MonoBehaviour
{
    [SerializeField]
    TMP_Text _tooltipText;

    [System.NonSerialized]
    public int tick;

    void Start()
    {
        _tooltipText.text = $"{tick}";
    }
}
