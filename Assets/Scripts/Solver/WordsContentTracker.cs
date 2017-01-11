using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class InputFieldValues
{
	public Vector2 size;
	public int textSize = 40;
	public int characterLimit = 0;
	public int defaultInputFields = 5;
	public InputField inputField;

	public string defaultText = "Enter Word to Find";
}

public class WordsContentTracker : SingletonBehaviour<WordsContentTracker>
{
	public InputFieldValues fieldVariables = new InputFieldValues();

	public List<InputField> words = new List<InputField>();

	public GameObject contentParent;

	public ScrollRect scroll;
	public GridLayoutGroup grid;



	/*
				FieldComponent obj = GameObject.Instantiate(gridInputField) as FieldComponent;
				RectTransform objTrans = obj.GetComponent<RectTransform>();
				objTrans.SetParent(trans);
								

				objTrans.localPosition = new Vector2((i * boxSize.x) - (gridSpace.x / 2) + (boxSize.x / 2),
					(gridSpace.y / 2) - (j * boxSize.y) - (boxSize.y / 2));

				objTrans.localScale = Vector3.one;

				objTrans.sizeDelta = trueBoxSize;

				FieldController.Instance.SetField(i, j, obj);

*/

	public override void OnSingletonAwake()
	{
		scroll = GetComponent<ScrollRect>();
	}

	void Start()
	{
		if (grid != null)
		{
			grid.cellSize = fieldVariables.size;
		}
	}

	public void SpawnFields(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			AddInputField();
		}
	}

	public void AddInputField()
	{
		InputField fieldObj = GameObject.Instantiate(fieldVariables.inputField) as InputField;

		RectTransform rectTrans = fieldObj.GetComponent<RectTransform>();


		rectTrans.SetParent(contentParent.transform);

		rectTrans.localScale = Vector3.one;

		rectTrans.sizeDelta = fieldVariables.size;

		rectTrans.localPosition = new Vector2(0, (words.Count * -fieldVariables.size.y) - (fieldVariables.size.y * 0.2f));

		fieldObj.characterLimit = fieldVariables.characterLimit;

		fieldObj.placeholder.GetComponent<Text>().text = fieldVariables.defaultText;

		fieldObj.onEndEdit.AddListener(delegate
		{
			EndEdit(fieldObj);
		});

		words.Add(fieldObj);
	}


	public void EndEdit(InputField inputField)
	{
		if (inputField.text == "")
		{
			return;
		}

		if (words [words.Count - 1] == inputField)
		{
			AddInputField();
		}

		int index = words.IndexOf(inputField);
		index++;
		SelectField(index);
	}

	void SelectField(int i)
	{
		words [i].Select();
	}

	public void DeleteWords()
	{
		for (int i = 0; i < words.Count; i++)
		{
			GameObject obj = words [i].gameObject;
			words [i] = null;
			Destroy(obj);
		}

		words.Clear();

	}
}
