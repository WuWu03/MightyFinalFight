// Copyright (c) 2014 Luminary LLC
// Licensed under The MIT License (See LICENSE for full text)
using UnityEngine;

public class SetPropertyAttribute : PropertyAttribute
{
	public string name 
	{
        get 
		{
			return m_Name;
		}
	}

	public bool isDirty { get; set; }

	public SetPropertyAttribute(string name)
	{
		m_Name = name;
	}

	private string m_Name = string.Empty;
}
