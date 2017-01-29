using UnityEngine;
using System.Collections;

public class InputController : SingletonBehaviour<InputController>
{

	public UILineRenderer lineRenderer;

	//Grid Size: 750 * 750
	//Grid Pos: 0, 150 (Anchor: Middle)

	//Words Size: 725 * 300
	//Words Pos: 0, 112.5 (Anchor: Bottom)

	//Find Area of Click/Tap
	//Find if it is within the Grid Area
	//Find the Grid (X, Y) Square
	//Start Vector2 of MousePos

	//If Dragging
	//Snap to 45 Degrees (if Diagonal, else 90 degrees)
	//Only allow Backwards if level allows backwards

	//Highlight Letters in Line as Selected.
	//Play Selected Animation

	//On Stop Mouse Drag/On Second Tap
	//Select Entire Word
	//If it follows a word path, pop the letters, permanently highlight letters and cross it off the list
	//else fizzle letters and unhighlight path/letters

	//Selection Types:
	//Highlight
	//Line
	//Both

	Vector2 res;
	Vector2 screenSize;
	RectTransform playSpace;

	void Start()
	{
		Input.multiTouchEnabled = false;
		lineRenderer.CreatePath(1, 0);
		res = ButtonController.Instance.canvasScaler.referenceResolution;
		screenSize = new Vector2(Screen.width, Screen.height);
		playSpace = GameController.Instance.grid.GetComponent<RectTransform>();
	}


	Vector2 mouseStart;
	Vector2 mouseDelta;
	Vector2 mouseEnd;

	int cur = 0;

	Vector2 GetGrid(Vector2 pos)
	{
		if (playSpace == null)
		{
			playSpace = GameController.Instance.grid.GetComponent<RectTransform>();
		}

		if (playSpace == null)
		{
			return -Vector2.one;
		}

		//Fix this so they aren't using temp variables

		Vector2 size = new Vector2(playSpace.rect.width, playSpace.rect.height);
		Vector2 trueAnchoredPos = (res / 2) + playSpace.anchoredPosition;
		Vector2 playAreaMin = trueAnchoredPos - (size / 2);
		Vector2 playAreaMax = trueAnchoredPos + (size / 2);

		if (pos.x > playAreaMin.x
		    && pos.y > playAreaMin.y
		    && pos.x < playAreaMax.x
		    && pos.y < playAreaMax.y)
		{
			Vector2 cellSize = GameController.Instance.grid.cellSize + GameController.Instance.grid.spacing;

			Vector2 flippedPos = new Vector2(pos.x, res.y - pos.y);

			Vector2 localPos = new Vector2(flippedPos.x - playAreaMin.x, flippedPos.y - (res.y - playAreaMax.y));
			Debug.Log("local: " + localPos);

			Vector2 square = new Vector2(Mathf.FloorToInt(localPos.x / cellSize.x), Mathf.FloorToInt(localPos.y / cellSize.y));
			Debug.Log("Square: " + square);

			return square;
		}
		else
		{
			return -Vector2.one;
		}
	}
	// Update is called once per frame
	void Update()
	{
/*
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);

			switch (touch.phase)
			{
				case TouchPhase.Began:
					{
						Debug.Log(touch.position);
						break;
					}
				case TouchPhase.Moved:
					{
						Debug.Log(touch.position);
						break;
					}
				case TouchPhase.Ended:
					{
						Debug.Log(touch.position);
						break;
					}
			}
		}
*/

		{
			Vector2 pos = Vector2.Scale((new Vector2(Input.mousePosition.x / screenSize.x, Input.mousePosition.y / screenSize.y)), (res));

			//Touch.Began
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				//Keep a record fo where we started.
				mouseStart = pos;
				Vector2 gridStart = GetGrid(pos);
				lineRenderer.SetPath(cur, mouseStart, 1);
				cur++;
				//lineRenderer.CreatePath(1, 0);
			}
			//Touch.Ended
			else if (Input.GetKeyUp(KeyCode.Mouse0))
			{
				//Confirm if we highlighted a word's 'Path'
				mouseEnd = pos;
			}
			//Touch.Moved
			else if (Input.GetKey(KeyCode.Mouse0))
			{
				//Check the angle that we are at and add the 'Path' of all the letters we are going through
				mouseDelta = pos;
				Vector2 gridEnd = GetGrid(pos);
				lineRenderer.SetPath(cur, mouseDelta, 1);
			}

		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ButtonController.Instance.BackMenu();
		}


		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			Vector2 screenSize = new Vector2(Screen.width, Screen.height);
			Vector2 res = ButtonController.Instance.canvasScaler.referenceResolution;

			//IF within CROSSWORD SQUARES (FIELDS)
			if (GridSpawner.Instance.gridSpawned)
			{
				Vector2 trueMousePos = new Vector2(((mousePos.x / screenSize.x) * res.x) - GridSpawner.Instance.trans.localPosition.x,
					                       ((mousePos.y / screenSize.y) * res.y) - GridSpawner.Instance.trans.localPosition.y);

				Vector2 bottomLeft = ((res / 2) - (GridSpawner.Instance.gridSpace / 2));

				Vector2 trueGridMousePos = trueMousePos - bottomLeft;

				if (trueGridMousePos.x > 0 && trueGridMousePos.y > 0
				    && trueGridMousePos.x <= GridSpawner.Instance.gridSpace.x && trueGridMousePos.y <= GridSpawner.Instance.gridSpace.y)
				{
					int xSpace = Mathf.FloorToInt(trueGridMousePos.x / GridSpawner.Instance.boxSize.x);
					int ySpace = Mathf.FloorToInt(trueGridMousePos.y / GridSpawner.Instance.boxSize.y);

					FieldController.Instance.SetPosition(xSpace, ySpace, true);

				}

			}



			//If within Word Finder Fields
			{

			}
		}

	}
}