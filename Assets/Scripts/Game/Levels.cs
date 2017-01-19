using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Levels))]
public class LevelsCustomInspector : Editor
{
	int min = 0;
	int max = 0;
	bool updated = false;
	int oldMax = 0;

	public override void OnInspectorGUI()
	{
		Levels levelsScript = (Levels)target;

		if (levelsScript.selector != null)
		{
			if (!updated)
			{
				max = levelsScript.selector.levels.Count;
				oldMax = max;
				updated = true;
			}

			if (oldMax != levelsScript.selector.levels.Count)
			{
				updated = false;
			}

			min = EditorGUILayout.IntSlider("Starting Level: ", min, 0, max);
			max = EditorGUILayout.IntSlider("Ending Level: ", max, min, levelsScript.selector.levels.Count);

			if (GUILayout.Button("Populate Levels "))
			{
				Debug.Log("Populating From Level: " + min + " To Level: " + max);

				levelsScript.PopulateLevels(min, max);


				UnityEditor.PrefabUtility.ReplacePrefab(levelsScript.gameObject, UnityEditor.PrefabUtility.GetPrefabParent(levelsScript.gameObject));
			}
		}
		DrawDefaultInspector();
	}
}

#endif

public class Levels : SingletonBehaviour<Levels>
{
	public Level[] levels;

	private List<string> strings = new List<string> { "but", "edd", "new" };

	public LevelSelector selector;

	void Start()
	{
		if (selector == null)
		{
			selector = FindObjectOfType<LevelSelector>();
		}
	}

	public void PopulateLevels(int min, int max)
	{
		if (selector == null)
		{
			selector = FindObjectOfType<LevelSelector>();
		}

		if (selector != null)
		{
			Level[] clone = levels;
			levels = new Level[selector.levels.Count];

			if (clone != null)
			{
				for (int i = 0; i < min; i++)
				{
					if (clone.Length >= i)
					{
						break;
					}
					levels [i] = clone [i];
				}

				for (int i = max; i < selector.levels.Count; i++)
				{
					if (clone.Length >= i)
					{
						break;
					}
					levels [i] = clone [i];
				}
			}


			for (int i = min; i < max; i++)
			{
				List<string> stringInput = new List<string>();

				if (Random.value > 0.5f)
				{
					stringInput.Add(strings [0]);
				}
				if (Random.value > 0.5f)
				{
					stringInput.Add(strings [1]);
				}
				if (Random.value > 0.5f)
				{
					stringInput.Add(strings [2]);
				}

				levels [i] = (new Level(5, 5, stringInput));
			}

		}
	}
}