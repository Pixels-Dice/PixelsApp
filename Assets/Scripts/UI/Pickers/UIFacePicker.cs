﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFacePicker : MonoBehaviour
{
    [Header("Controls")]
    public Button backButton;
    public Button saveButton;
    public Text titleText;
    public RectTransform contentRoot;
    public Button allFaces;
    public Button noFace;

    [Header("Prefabs")]
    public UIFacePickerToken faceTokenPrefab;

    int previousFaceMask = -1;
    System.Action<bool, int> closeAction;

    // The list of controls we have created to display dice
    List<UIFacePickerToken> faces = new List<UIFacePickerToken>();

    public bool isShown => gameObject.activeSelf;

    public bool isDirty => saveButton.gameObject.activeSelf;

    /// <summary>
    /// Invoke the die picker
    /// </sumary>
    public void Show(string title, int previousFaceMask, System.Action<bool, int> closeAction)
    {
        if (isShown)
        {
            Debug.LogWarning("Previous Face picker still active");
            ForceHide();
        }

        for (int i = 0; i < 20; ++i)
        {
            // New pattern
            bool faceSelected = (previousFaceMask & (1 << i)) != 0;
            var newFaceUI = CreateFaceToken(i, faceSelected);
            faces.Add(newFaceUI);
        }

        gameObject.SetActive(true);
        this.previousFaceMask = previousFaceMask;
        titleText.text = title;
        saveButton.gameObject.SetActive(false);

        this.closeAction = closeAction;
    }

    UIFacePickerToken CreateFaceToken(int faceIndex, bool selected)
    {
        // Create the gameObject
        var ret = GameObject.Instantiate<UIFacePickerToken>(faceTokenPrefab, contentRoot.transform);

        // Initialize it
        ret.Setup(faceIndex, selected);
        ret.onValueChanged.AddListener(UpdateButtons);
        return ret;
    }

    /// <summary>
    /// If for some reason the app needs to close the dialog box, this will do it!
    /// Normally it closes itself when you tap ok or cancel
    /// </sumary>
    public void ForceHide()
    {
        DiscardAndBack();
    }

    void Awake()
    {
        backButton.onClick.AddListener(DiscardAndBack);
        saveButton.onClick.AddListener(SaveAndBack);
        allFaces.onClick.AddListener(ToggleAllFaces);
        noFace.onClick.AddListener(ToggleNoFace);
    }

    void Hide(bool result, int faceMask)
    {
        foreach (var uiface in faces)
        {
            DestroyFaceToken(uiface);
        }
        faces.Clear();

        gameObject.SetActive(false);
        closeAction?.Invoke(result, faceMask);
        closeAction = null;
    }

    void UpdateButtons(bool newToggle)
    {
        saveButton.gameObject.SetActive(ComputeNewFaceMask() != previousFaceMask);
    }

    void DiscardAndBack()
    {
        if (isDirty)
        {
            PixelsApp.Instance.ShowDialogBox(
                "Discard Changes",
                "You have unsaved changes, are you sure you want to discard them?",
                "Discard",
                "Cancel", discard =>
                {
                    if (discard)
                    {
                        Hide(false, previousFaceMask);
                    }
                });
        }
        else
        {
            Hide(false, previousFaceMask);
        }
    }

    void SaveAndBack()
    {
        Hide(true, ComputeNewFaceMask());
    }

    int ComputeNewFaceMask()
    {
        int newFaceMask = 0;
        for (int i = 0; i < 20; ++i)
        {
            if (faces[i].isOn)
            {
                newFaceMask |= (1 << i);
            }
        }
        return newFaceMask;
    }

    void DestroyFaceToken(UIFacePickerToken token)
    {
        GameObject.Destroy(token.gameObject);
    }

    void ToggleAllFaces()
    {
        foreach (var btn in faces)
        {
            btn.Toggle(true);
        }
        UpdateButtons(true);
    }

    void ToggleNoFace()
    {
        foreach (var btn in faces)
        {
            btn.Toggle(false);
        }
        UpdateButtons(false);
    }
}
