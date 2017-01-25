using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UILineRenderer : MonoBehaviour
{
	public Canvas canvas;
	public Transform parent;

	public List<Vector2[]> paths = new List<Vector2[]>();

	private List<Image[]> fillObjects = new List<Image[]>();

	public Color startColour;
	public Color endColour;

	private Color oldStartColour;
	private Color oldEndColour;

	public Sprite fill;


	public int GetPathsCount()
	{
		return paths.Count;
	}

	public int GetPathPointsLength(int path)
	{
		return paths [path].Length;
	}


	public float startThickness;
	public float endThickness;

	private float oldStartThickness;
	private float oldEndThickness;

	bool update = true;

	void Start()
	{
		if (canvas == null)
		{
			canvas = FindObjectOfType<Canvas>();
		}

		CreatePath(0, 5);
		SetPath(0, new Vector2(0, 0));
		SetPath(1, new Vector2(750, 0));
		SetPath(2, new Vector2(750, 1334));
		SetPath(3, new Vector2(0, 1334));
		SetPath(4, new Vector2(0, 0));
	}

	void Update()
	{
		if (canvas == null)
		{
			Debug.LogWarning("No Canvas Found");
			canvas = FindObjectOfType<Canvas>();
			return;
		}

		if (!update)
		{
			update = (startThickness != oldStartThickness) || (endThickness != oldEndThickness) || (startColour != oldStartColour) || (endColour != oldEndColour);
		}

		if (update)
		{
			RenderLines();
		}

		oldStartThickness = startThickness;
		oldEndThickness = endThickness;

		oldStartColour = startColour;
		oldEndColour = endColour;
	}

	void RenderLines()
	{


		for (int i = 0; i < fillObjects.Count; i++)
		{
			AddObjects(i);
			RemoveObjects(i);
		}

		for (int i = 0; i < paths.Count; i++)
		{
			for (int j = 0; j < paths [i].Length - 1; j++)
			{
				if (paths [i].Length < 2)
				{
					continue;
				}

				Image imageObject = GetImageObject(i, j);

				if (imageObject == null)
				{
					continue;
				}

				RectTransform trans = imageObject.GetComponent<RectTransform>();

				trans.anchoredPosition = ((paths [i] [j] + paths [i] [j + 1]) / 2);

				Vector2 diff = paths [i] [j + 1] - paths [i] [j];

				Vector3 rot = new Vector3(0, 0, (Mathf.Atan2(diff.normalized.y, diff.normalized.x) * Mathf.Rad2Deg));

				trans.eulerAngles = rot;

				float divisor = ((float)j / ((float)paths [i].Length - 2));

				trans.sizeDelta = new Vector2(Mathf.Abs(diff.magnitude), Mathf.Lerp(startThickness, endThickness, divisor));
				imageObject.color = Color.Lerp(startColour, endColour, divisor);
			}
		}

		update = false;
	}

	void AddObjects(int path)
	{
		//TODO: FIX
		if (path >= paths.Count || path >= fillObjects.Count)
		{
			return;
		}

		if (paths [path].Length <= fillObjects [path].Length)
		{
			return;
		}

		Image[] oldObjects = fillObjects [path];
		fillObjects [path] = new Image[paths [path].Length];

		for (int j = 0; j < oldObjects.Length; j++)
		{
			if (oldObjects [j] != null)
			{
				fillObjects [path] [j] = oldObjects [j];
			}
		}


		for (int i = 0; i < fillObjects [path].Length; i++)
		{
			if (fillObjects [path] [i] == null)
			{
				fillObjects [path] [i] = CreateNewImageObject();
				fillObjects [path] [i].gameObject.name = "Path " + path + " Set: " + i;
			}
		}
	}

	void RemoveObjects(int path)
	{
		if (path >= paths.Count || path >= fillObjects.Count)
		{
			return;
		}

		if (paths [path].Length >= fillObjects [path].Length)
		{
			return;
		}

		int sIndex = 0;

		if ((path < paths.Count) || paths [path].Length > 0)
		{
			sIndex = paths [path].Length;
		}

		Debug.Log(sIndex);

		for (int i = sIndex; i < fillObjects [path].Length; i++)
		{
			if (fillObjects [path] [i] != null)
			{
				Destroy(fillObjects [path] [i].gameObject);
			}
		}

		if (sIndex == 0)
		{
			paths.RemoveAt(path);
		}
		else
		{

			Image[] oldObjects = fillObjects [path];

			fillObjects [path] = new Image[sIndex];

			for (int i = 0; i < Mathf.RoundToInt(Mathf.Min(fillObjects [path].Length, oldObjects.Length)); i++)
			{
				fillObjects [path] [i] = oldObjects [i];
			}
		}

	}

	public Image GetImageObject(int path, int index)
	{
		if ((fillObjects.Count - 1) < path)
		{
			Debug.Log("Fill Objects Count: " + fillObjects.Count + " while Path " + path);
			return null;
		}

		if ((fillObjects [path].Length - 1) < index)
		{
			Debug.Log("Fill Objects + " + path + " Length: " + fillObjects [path].Length + " while Index " + index);
			return null;
		}

		if (fillObjects [path] [index] == null)
		{
			fillObjects [path] [index] = CreateNewImageObject();
			fillObjects [path] [index].gameObject.name = "Path " + path + " Set: " + index;
		}

		return fillObjects [path] [index];
	}

	Image CreateNewImageObject()
	{
		GameObject obj = new GameObject();

		if (parent != null)
		{
			obj.transform.SetParent(parent);
		}
		else
		{
			obj.transform.SetParent(canvas.transform);
		}
		RectTransform trans = obj.GetComponent<RectTransform>();

		if (trans == null)
		{
			trans = obj.AddComponent<RectTransform>();
		}
		trans.localScale = Vector3.one;
		trans.anchorMin = Vector2.zero;
		trans.anchorMax = Vector2.zero;

		Image objImage = obj.AddComponent<Image>();
		if (fill != null)
		{
			objImage.sprite = fill;
		}
		return objImage;
	}

	public void SetPath(int index, Vector2 position, int pathCount = 0)
	{
		for (int i = paths.Count; i <= pathCount; i++)
		{
			paths.Add(new Vector2[0]);
		}

		for (int i = fillObjects.Count; i <= pathCount; i++)
		{
			fillObjects.Add(new Image[0]);
		}

		if (paths [pathCount].Length <= index)
		{
			Vector2[] oldPath = paths [pathCount];
			paths [pathCount] = new Vector2[(index + 1)];
			for (int i = 0; i < oldPath.Length; i++)
			{
				paths [pathCount] [i] = oldPath [i];
			}
		}

		if (fillObjects [pathCount].Length <= index)
		{
			Image[] oldObjects = fillObjects [pathCount];
			fillObjects [pathCount] = new Image[(index + 1)];
			for (int i = 0; i < oldObjects.Length; i++)
			{
				fillObjects [pathCount] [i] = oldObjects [i];
			}
		}

		paths [pathCount] [index] = position;
		update = true;
	}

	public void SetPath(Vector2[] positions, int pathCount = 0)
	{
		for (int i = paths.Count; i <= pathCount; i++)
		{
			paths.Add(new Vector2[0]);
		}

		for (int i = fillObjects.Count; i <= pathCount; i++)
		{
			fillObjects.Add(new Image[0]);
		}


		paths [pathCount] = positions;
		update = true;
	}

	public void CreatePath(int size)
	{
		paths.Add(new Vector2[size]);
		fillObjects.Add(new Image[size]);
	}

	public void CreatePath(int pathCount, int size)
	{
		for (int i = paths.Count; i <= pathCount; i++)
		{
			paths.Add(new Vector2[0]);
		}


		for (int i = fillObjects.Count; i <= pathCount; i++)
		{
			fillObjects.Add(new Image[0]);
		}

		paths [pathCount] = new Vector2[size];

		//Create Path needs to check if there is already a path there and remove any objects already created.
		//RemoveObjects(pathCount);
		RemoveObjects(pathCount);

		fillObjects [pathCount] = new Image[size];
	}
}
