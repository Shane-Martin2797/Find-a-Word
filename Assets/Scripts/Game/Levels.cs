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

		if (GUILayout.Button("Read String"))
		{
			levelsScript.ReadLevelCSV();
		}

		if (levelsScript.levelsString != null && levelsScript.selector != null)
		{
			if (!updated)
			{
				max = levelsScript.levelsString.Length;
				oldMax = max;
				updated = true;
			}

			if (oldMax != levelsScript.levelsString.Length)
			{
				updated = false;
			}

			min = EditorGUILayout.IntSlider("Starting Level: ", min, 0, max);
			max = EditorGUILayout.IntSlider("Ending Level: ", max, min, levelsScript.levelsString.Length);

			if (GUILayout.Button("Populate Levels "))
			{
				Debug.Log("Populating From Level: " + min + " To Level: " + max);

				levelsScript.PopulateLevels(Language.American, min, max);
				levelsScript.PopulateLevels(Language.Australian, min, max);
				levelsScript.PopulateLevels(Language.British, min, max);

				levelsScript.selector.PopulateButtons(levelsScript.levelsString.Length);

				UnityEditor.PrefabUtility.ReplacePrefab(levelsScript.gameObject, UnityEditor.PrefabUtility.GetPrefabParent(levelsScript.gameObject));
			}
		}
		DrawDefaultInspector();
	}
}
#endif


public class Levels : SingletonBehaviour<Levels>
{

	#if UNITY_EDITOR

	public TextAsset levelsDataCSV;
	public string[][] levelsString;
	public int startingCSVNumberOfLetters = 1;

	public void ReadLevelCSV()
	{
		string s = levelsDataCSV.text;

		string[] lines = s.Split('\n');

		levelsString = new string[lines.Length][];

		for (int i = 0; i < lines.Length; i++)
		{
			levelsString [i] = lines [i].Split(',');
		}
	}

	public int GetNumberOfWords(int level, int letters)
	{
		if(letters < startingCSVNumberOfLetters)
		{
			return -1;
		}
	
		int res;
		if (int.TryParse(levelsString [level] [(letters - startingCSVNumberOfLetters)], out res))
		{
			return res;
		}
		return -1;
	}

	#endif


	public Level[] americanLevels;
	public Level[] australianLevels;
	public Level[] britishLevels;

	private List<string> strings = new List<string> { "but", "edd", "new" };

	public LevelSelector selector;

	public Level[] GetLevel (Language lan)
	{
		switch (lan) {
		case Language.Australian:
			{
				return australianLevels;
			}
		case Language.American:
			{
				return americanLevels;
			}
		case Language.British:
			{
				return britishLevels;
			}
		default:
			{
				return australianLevels;
			}
		}
	}

	public void SetLevel (Language lan, Level[] level)
	{
		switch (lan) {
		case Language.Australian:
			{
				australianLevels = level;
				break;
			}
		case Language.American:
			{
				americanLevels = level;
				break;
			}
		case Language.British:
			{
				britishLevels = level;
				break;
			}
		default:
			{
				australianLevels = level;
				break;
			}
		}
	}



	void Start ()
	{
		if (selector == null) {
			selector = FindObjectOfType<LevelSelector> ();
		}
	}

	public Dictionary<int, List<string>> AustralianDictionary;
	public Dictionary<int, List<string>> AmericanDictionary;
	public Dictionary<int, List<string>> BritishDictionary;

	Dictionary<int, List<string>> GetDictionary (Language lan, bool import = false)
	{
		switch (lan) {
		case Language.Australian:
			{
				if (AustralianDictionary == null || import) {
					AustralianDictionary = SaveLoad.ImportDictionary (lan);
				}
				return AustralianDictionary;
			}
		case Language.American:
			{
				if (AmericanDictionary == null || import) {
					AmericanDictionary = SaveLoad.ImportDictionary (lan);
				}
				return AmericanDictionary;
			}
		case Language.British:
			{
				if (BritishDictionary == null || import) {
					BritishDictionary = SaveLoad.ImportDictionary (lan);
				}
				return BritishDictionary;
			}
		default:
			{
				if (AustralianDictionary == null || import) {
					AustralianDictionary = SaveLoad.ImportDictionary (lan);
				}
				return AustralianDictionary;
			}
		}
	}

	string GetRandomWord (List<string> wordList, List<string> wordsToAvoid = null)
	{
		if (wordList == null || wordList.Count == 0) {
			return "BUTTS";
		}
		string randomWord = wordList [UnityEngine.Random.Range (0, wordList.Count)];

		if (wordsToAvoid != null) {
			int rC = 0;
			while (wordsToAvoid.Contains(randomWord)) {
				if (rC >= 1000) {

					if (rC - 1000 > wordList.Count) {
						return "BUTTS";
					}
					randomWord = wordList [rC - 1000];
				} else {
					randomWord = wordList [UnityEngine.Random.Range (0, wordList.Count)];
				}
				rC++;
			}
		}

		return randomWord;
	}

	List<string> GetWords (Dictionary<int, List<string>> dic, int lev)
	{
		List<string> s = new List<string> ();

		for (int i = 0; i < levelsString [lev].Length; i++) {
			for (int j = 0; j < GetNumberOfWords(lev, i); j++) {
				string newWord = (GetRandomWord (dic [i], s));
				s.Add (newWord);
			}
		}
		return s;
	}

	public void PopulateLevels (Language lan, int min, int max)
	{
		Dictionary<int, List<string>> dic = GetDictionary (lan);

		if (selector == null) {
			selector = FindObjectOfType<LevelSelector> ();
		}

		if (selector != null) {
			Level[] levels = GetLevel (lan);

			Level[] clone = levels;

			levels = new Level[levelsString.Length];

			if (clone != null) {
				for (int i = 0; i < min; i++) {
					if (clone.Length >= i) {
						break;
					}
					levels [i] = clone [i];
				}

				for (int i = max; i < levelsString.Length; i++) {
					if (clone.Length >= i) {
						break;
					}
					levels [i] = clone [i];
				}
			}


			for (int i = min; i < max; i++) {
				List<string> stringInput = GetWords (dic, i);

				int x = 0;
				int y = 0;

				int length = 0;
				int largestWordLength = 0;

				for (int l = 0; l < stringInput.Count; l++) {
					length += stringInput [l].Length;
					if (stringInput [l].Length > largestWordLength) {
						largestWordLength = stringInput [l].Length;
					}
				}

				x = Mathf.Max (Mathf.RoundToInt (((float)length / 2) * ((float)length / 2)), largestWordLength);
				y = x;

				levels [i] = (new Level (x, y, stringInput));
				SetLevel (lan, levels);
			}
		}
	}
}