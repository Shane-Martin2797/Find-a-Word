using UnityEngine;
using System.Collections;

public class InputController : SingletonBehaviour<InputController>
{
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


	void Start()
	{
		Input.multiTouchEnabled = false;
	}


	Vector2 mouseStart;
	Vector2 mouseDelta;
	Vector2 mouseEnd;

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
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				mouseStart = Input.mousePosition;
			}
			else if (Input.GetKeyUp(KeyCode.Mouse0))
			{
				mouseEnd = Input.mousePosition;
			}
			else if (Input.GetKey(KeyCode.Mouse0))
			{
				mouseDelta = Input.mousePosition;
			}

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