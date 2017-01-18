using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelSelector : MonoBehaviour
{

	public List<Button> levels = new List<Button>();


	void Start()
	{
		for (int i = 0; i < levels.Count; i++)
		{
			int tempVal = i;

			levels [i].onClick.AddListener(() =>
			{
				ButtonController.Instance.LoadLevel(tempVal);
			});

			levels [i].GetComponentInChildren<Text>(true).text = "Level " + (i + 1);
		}
	}

}
