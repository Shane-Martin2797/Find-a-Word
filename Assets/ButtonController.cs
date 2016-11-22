using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonController : SingletonBehaviour<ButtonController>
{
	public enum TypeOfSpawn
	{
		Create,
		Solve
	}

	public Canvas canvas;
	public CanvasScaler canvasScaler;

	public GameObject title;
	public GameObject setGridSizeObject;

	public InputField XSize;
	public InputField YSize;

	int x;
	int y;

	TypeOfSpawn t;

	public override void OnSingletonAwake()
	{
		canvas = GetComponent<Canvas>();
		canvasScaler = GetComponent<CanvasScaler>();
	}

	public void OpenGrid(int s)
	{
		t = (TypeOfSpawn)s;

		title.SetActive(false);
		setGridSizeObject.SetActive(true);
	}


	public void SolveCrossword()
	{
		CrosswordSolver.Instance.Solve(FieldController.Instance.fields, WordsContentTracker.Instance.words);
	}

	public void SpawnGrid()
	{
		if (!int.TryParse(XSize.text, out x))
		{
			return;
		}
		if (!int.TryParse(YSize.text, out y))
		{
			return;
		}

		switch (t)
		{
			case TypeOfSpawn.Create:
				{
					GridSpawner.Instance.SpawnGrid(x, y);
				}
				break;
			case TypeOfSpawn.Solve:
				{
					GridSpawner.Instance.SpawnGrid(x, y);
				}
				break;
		}
		setGridSizeObject.SetActive(false);
	}
}
