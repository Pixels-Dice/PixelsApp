﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainNavigationButton : MonoBehaviour
{
    [Header("Controls")]
    public Button button;
    public Image buttonImage;

    [Header("Sprites")]
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    // Forward the click event
    public Button.ButtonClickedEvent onClick => button.onClick;

    public void SetCurrent(bool current)
    {
        buttonImage.sprite = current ? activeSprite : inactiveSprite;
    }
}
