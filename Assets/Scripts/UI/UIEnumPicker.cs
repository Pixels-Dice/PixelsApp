﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIEnumPicker : MonoBehaviour
{
    [Header("Controls")]
    public Button backButton;
    public Text titleText;
    public RectTransform contentRoot;

    [Header("Parameters")]
    public UIEnumPickerToken enumPickerTokenPrefab;

    System.Enum currentValue;
    System.Enum newValue;
    System.Action<bool, System.Enum> closeAction;

    List<UIEnumPickerToken> tokens = new List<UIEnumPickerToken>();

    public bool isShown => gameObject.activeSelf;

    /// <summary>
    /// Invoke the die picker
    /// </sumary>
    public void Show(string title, System.Enum previousValue, System.Action<bool, System.Enum> closeAction, int min, int max)
    {
        if (isShown)
        {
            Debug.LogWarning("Previous Die picker still active");
            ForceHide();
        }

        gameObject.SetActive(true);
        currentValue = previousValue;
        titleText.text = title;
        this.closeAction = closeAction;

        var enumType = previousValue.GetType();
        List<string> enumValueNames;
        System.Array enumValues;

        int count = max - min + 1;
        enumValues = System.Array.CreateInstance(typeof(object), count);
        System.Array.Copy(System.Enum.GetValues(enumType), min, enumValues, 0, count);
        enumValueNames = System.Enum.GetNames(enumType).ToList().GetRange(min, count);

        for (int i = 0; i < enumValues.Length; ++i)
        {
            var value = (System.Enum)enumValues.GetValue(i);
            var token = CreateEnumToken(enumValueNames[i], value);
            tokens.Add(token);
            token.SetSelected(value.Equals(currentValue));
        }
    }

    /// <summary>
    /// If for some reason the app needs to close the dialog box, this will do it!
    /// Normally it closes itself when you tap ok or cancel
    /// </sumary>
    public void ForceHide()
    {
        Hide(false, currentValue);
    }

    void Awake()
    {
        backButton.onClick.AddListener(DiscardAndBack);
    }

    void Hide(bool result, System.Enum value)
    {
        foreach (var ui in tokens)
        {
            DestroyEnumToken(ui);
        }
        tokens.Clear();

        gameObject.SetActive(false);
        var closeActionCopy = closeAction;
        closeAction = null;
        closeActionCopy?.Invoke(result, value);
    }

    void DiscardAndBack()
    {
        Hide(false, currentValue);
    }

    UIEnumPickerToken CreateEnumToken(string enumString, System.Enum enumValue)
    {
        // Create the gameObject
        var ret = GameObject.Instantiate<UIEnumPickerToken>(enumPickerTokenPrefab, Vector3.zero, Quaternion.identity, contentRoot);

        // Initialize it
        ret.Setup(enumString, enumValue);
        ret.onEnumSelected += SelectEnum;

        return ret;
    }

    void DestroyEnumToken(UIEnumPickerToken token)
    {
        GameObject.Destroy(token.gameObject);
    }

    void SelectEnum(string enumString, System.Enum enumValue)
    {
        Hide(true, enumValue);
    }
}
