using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(FieldController))]
public class GridSpawner : SingletonBehaviour<GridSpawner>
{

	public InputField gridInputField;
	public RectTransform trans;
	public bool gridSpawned = false;

	public GameObject wordObject;
	public GameObject fieldObject;

	public Vector2 gridSpace;

	public float gapRatio = (0.8f);

	public Vector2 boxSize;

	public override void OnSingletonAwake()
	{
		trans = GetComponent<RectTransform>();
	}

	void Start()
	{
		gridSpace = new Vector2(trans.rect.width, trans.rect.height);
	}

	public void SpawnGrid(int x, int y)
	{
		if (x < 0 || y < 0)
		{
			return;
		}

		wordObject.SetActive(true);
		//fieldObject.SetActive(true);

		FieldController.Instance.CreateField(x, y);

		boxSize = new Vector2(gridSpace.x / x, gridSpace.y / y);

		Vector2 trueBoxSize = new Vector2(boxSize.x * gapRatio, boxSize.y * gapRatio);

		//Vector2 gapSize = new Vector2(boxSize.x * (1 - gapRatio), boxSize.y * (1 - gapRatio));

		for (int j = 0; j < x; j++)
		{
			for (int i = 0; i < y; i++)
			{
				InputField obj = GameObject.Instantiate(gridInputField) as InputField;
				RectTransform objTrans = obj.GetComponent<RectTransform>();
				objTrans.SetParent(trans);
								

				objTrans.localPosition = new Vector2((i * boxSize.x) - (gridSpace.x / 2) + (boxSize.x / 2),
					(gridSpace.y / 2) - (j * boxSize.y) - (boxSize.y / 2));

				objTrans.localScale = Vector3.one;

				objTrans.sizeDelta = trueBoxSize;

				Text placeholderText = obj.placeholder.GetComponent<Text>();

				placeholderText.text = "A";

				obj.textComponent.fontSize = Mathf.RoundToInt((Mathf.Min(trueBoxSize.x, trueBoxSize.y)) / 1.875f);
				placeholderText.fontSize = Mathf.RoundToInt((Mathf.Min(trueBoxSize.x, trueBoxSize.y)) / 1.875f);

				FieldController.Instance.SetField(i, j, obj);

			}
		}

		WordsContentTracker.Instance.SpawnFields(WordsContentTracker.Instance.fieldVariables.defaultInputFields);
		gridSpawned = true;
	}


	public void DeleteGrid()
	{
		gridSpawned = false;
		FieldController.Instance.DeleteFields();
		WordsContentTracker.Instance.DeleteWords();
	}
}
