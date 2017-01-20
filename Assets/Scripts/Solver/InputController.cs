using UnityEngine;
using System.Collections;

public class InputController : SingletonBehaviour<InputController>
{

	
	// Update is called once per frame
	void Update()
	{
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