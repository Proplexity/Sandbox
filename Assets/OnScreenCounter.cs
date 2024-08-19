using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OnScreenCounter : MonoBehaviour
{
    public TextMeshProUGUI counterText;

    public int Counter { get; set; } = 0;

    private void Update()
    {
        counterText.SetText($"Boxes Collected = {Counter}");
    }
}
