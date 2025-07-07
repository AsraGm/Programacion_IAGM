using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPS : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI FPSText;

    void Update()
    {
        float fps = 1.0f / Time.deltaTime;
        FPSText.text = Mathf.RoundToInt(fps).ToString() + " FPS";
    }
}
