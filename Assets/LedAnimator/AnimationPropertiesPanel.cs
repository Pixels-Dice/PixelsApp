﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AnimationPropertiesPanel : SingletonMonoBehaviour<AnimationPropertiesPanel>
{
	[SerializeField]
	InputField _nameInput = null;
	[SerializeField]
	Dropdown _roleDropdown = null;
	[SerializeField]
	Toggle _roleToggle = null;
	[SerializeField]
	Transform[] _toDisableOnShow = null;

	ApplyCallback _doneCb;
	System.Action _removeCb;
	System.Action _duplicateCb;

	public delegate void ApplyCallback(string name, string role);

	public void Show(string name, string role, bool hasRole, string[] roles, ApplyCallback applyCallback, System.Action removeCallback, System.Action duplicateCallback)
	{
        _nameInput.text = name;
		_roleDropdown.options = roles.Select(str => new Dropdown.OptionData(str)).ToList();
		_roleDropdown.value = System.Array.IndexOf(roles, role);
		_roleToggle.isOn = hasRole;
		_doneCb = applyCallback;
		_removeCb = removeCallback;
		_duplicateCb = duplicateCallback;
		gameObject.SetActive(true);
        _nameInput.Select();
	}

	public void Apply()
	{
        string name = _nameInput.text;
        if (string.IsNullOrWhiteSpace(name))
        {
			name = string.Empty;
        }
		string role = _roleDropdown.interactable ? _roleDropdown.options[_roleDropdown.value].text : null;
		Close();
        _doneCb(name, role);
	}

	public void Close()
	{
		gameObject.SetActive(false);
	}

	public void AssignRole(bool yes)
	{
		_roleDropdown.interactable = yes;
	}

	public void DuplicateAnimation()
	{
		Close();
		_duplicateCb();
	}

	public void RemoveAnimation()
	{
		Close();
		_removeCb();
	}

	void OnEnable()
	{
		foreach (var trans in _toDisableOnShow)
		{
			trans.gameObject.SetActive(false);
		}
	}
}
