﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDieLargeBatteryView : MonoBehaviour
{
    [Header("Controls")]
    public Image batteryImage;
    public Text batteryLevelText;
    public Text batteryNiceText;

    [Header("Properties")]
    public Sprite[] batteryLevelImages;
    public float[] batteryLevels;
    public Sprite notAvailableImage;

    public void SetLevel(float? level)
    {
        if (level.HasValue)
        {
            // Find the first keyframe
            int index = 0;
            while (index < batteryLevels.Length && batteryLevels[index] > level.Value) {
                index++;
            }

            var sprite = batteryLevelImages[index];
            batteryImage.sprite = sprite;
            batteryLevelText.text = level.Value.ToString("P0");
            if (Mathf.RoundToInt(level.Value * 100.0f) == 69)
            {
                batteryNiceText.text = "Nice.";
            }
            else
            {
                batteryNiceText.text = "Battery";
            }
        }
        else
        {
            int index = batteryLevels.Length-1;
            var sprite = batteryLevelImages[index];
            batteryImage.sprite = sprite;
            batteryLevelText.text = "Unknown";
            batteryNiceText.text = "Battery";
        }
    }
}
