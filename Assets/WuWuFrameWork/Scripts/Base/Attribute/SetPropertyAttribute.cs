using UnityEngine;

public class SetPropertyAttribute : PropertyAttribute
{
    private string m_Name = string.Empty;

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
}
