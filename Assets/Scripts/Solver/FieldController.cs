using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FieldController : SingletonBehaviour<FieldController>
{

	public int i;
	public int j;

	public InputField[,] fields;

	public void CreateField(int x, int y)
	{
		fields = new InputField[x, y];
	}

	public void SetField(int x, int y, InputField field)
	{
		fields [x, y] = field;
		field.onValueChanged.AddListener(delegate
		{
			OnFieldChange(field);
		});
	}

	void OnFieldChange(InputField field)
	{
		if (field.text != "")
		{
			NextField();
		}
	}

	public void NextField()
	{
		i++;
		if (i > fields.GetLength(0) - 1)
		{
			i = 0;
			j++;
			if (j > fields.GetLength(1) - 1)
			{
				j = 0;
			}
		}

		if (i < 0)
		{
			i = 0;
		}
		if (j < 0)
		{
			j = 0;
		}

		fields [i, j].Select();
	}

	public void SetPosition(int x, int y, bool flipY = false)
	{
		i = x;
		if (flipY)
		{
			j = (fields.GetLength(1) - 1) - y;
		}
		else
		{
			j = y;
		}
	}

	public void DeleteFields()
	{
		for (int i = 0; i < fields.GetLength(0); i++)
		{
			for (int j = 0; j < fields.GetLength(1); j++)
			{
				GameObject obj = fields [i, j].gameObject;
				fields [i, j] = null;
				GameObject.Destroy(obj);
			}
		}
		fields = new InputField[0, 0];
	}
}
