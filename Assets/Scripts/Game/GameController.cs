using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public enum Language
{
	American,
	Australian,
	British
}

public class GameController : SingletonBehaviour<GameController>
{
	public PlayerData playerSave;
	bool paused = false;
	public GridLayoutGroup grid;
	private float gapRatio = 0.8f;

	public GameObject letter;

	public void SpawnGrid(int x, int y)
	{
		if (x < 0 || y < 0)
		{
			return;
		}
		RectTransform trans = grid.GetComponent<RectTransform>();

		Vector2 spaceSize = new Vector2(trans.rect.width / x, trans.rect.height / y);

		Vector2 trueBoxSize = spaceSize * gapRatio;
		Vector2 spacing = spaceSize * (1 - gapRatio);

		grid.cellSize = trueBoxSize;
		grid.spacing = spacing;
		grid.padding.left = Mathf.RoundToInt(spacing.x / 2);
		grid.padding.top = Mathf.RoundToInt(spacing.y / 2);


		for (int j = 0; j < x; j++)
		{
			for (int i = 0; i < y; i++)
			{
				GameObject letter = GameObject.Instantiate(GetLetter(x, y));

				if (letter == null)
				{
					Debug.Log("No letter for grid position: (" + x + ", " + y + ").");
					continue;
				}

				RectTransform letterTrans = letter.GetComponent<RectTransform>();

				if (letterTrans == null)
				{
					letterTrans = letter.AddComponent<RectTransform>();
				}

				letterTrans.SetParent(trans);

				//TODO: Since I am Going to Use Images/Prefabs, Scale will Change (not sizeDelta)
				letterTrans.localScale = Vector3.one;
				letterTrans.sizeDelta = trueBoxSize;
			}
		}

		//TODO: Cause words to spawn filled out (as uneditable objects)
		WordsContentTracker.Instance.SpawnFields(WordsContentTracker.Instance.fieldVariables.defaultInputFields);
	}

	public GameObject GetLetter(int x, int y)
	{

		//	Get board [x, y], convert it into a string, then a enum/int/object to object, then return that object.

		return letter;
	}


	public char[,] board;

	void OnEnable()
	{
		playerSave = SaveLoad.LoadPlayerData();
	}

	void OnDisable()
	{
		SaveLoad.SavePlayer(playerSave);
	}


	void OnApplicationFocus(bool hasFocus)
	{
		paused = !hasFocus;
		if (paused)
		{
			PauseApplication();
		}
	}

	void OnApplicationPause(bool pauseState)
	{
		paused = pauseState;
		if (paused)
		{
			PauseApplication();
		}
	}

	private PauseData pauseData = new PauseData();

	void PauseApplication()
	{
		if (currentLevel != null)
		{
			DateTime now = DateTime.Now;

			pauseData.second = now.Second;
			pauseData.minute = now.Minute;
			pauseData.hour = now.Hour;

			pauseData.day = now.Day;
			pauseData.month = now.Month;
			pauseData.year = now.Year;

			pauseData.level = playerSave.level;

			if (pauseData.found == null)
			{
				pauseData.found = new Dictionary<string, bool>();
			}
			else
			{
				pauseData.found.Clear();
			}

			for (int i = 0; i < currentLevel.words.Count; i++)
			{
				pauseData.found [currentLevel.words [i].wordString] = currentLevel.words [i].found;
			}

			pauseData.board = BoardToString();
		}
	}

	public string BoardToString()
	{
		string b = "";
		if (board == null)
		{
			return b;
		}

		for (int i = 0; i < board.GetLength(0); i++)
		{
			for (int j = 0; j < board.GetLength(1); j++)
			{
				b += board [i, j];
			}
		}
		return b;
	}

	void UnPauseApplication()
	{
	}

	public void LoadLevel(int level)
	{
		/*
		if (playerSave.level == level)
		{
			if (Reload())
			{
				CreateLevel(level, pauseData);
				return;
			}
		}
		*/

		playerSave.level = level;

		if (playerSave.latestLevel < level)
		{
			playerSave.latestLevel = level;
		}

		CreateLevel(level);
	}

	public bool Reload()
	{
		pauseData = SaveLoad.LoadLevel();

		DateTime currentTime = DateTime.Now;
		DateTime timeOfPause = new DateTime();

		timeOfPause.AddSeconds(pauseData.second);
		timeOfPause.AddMinutes(pauseData.minute);
		timeOfPause.AddHours(pauseData.hour);

		timeOfPause.AddDays(pauseData.day);
		timeOfPause.AddMonths(pauseData.month);
		timeOfPause.AddYears(pauseData.year);

		TimeSpan difference = currentTime.Subtract(timeOfPause);
		Debug.Log(difference);

		if (difference.Days >= 1)
		{
			return false;
		}

		return true;
	}


	public void CompleteLevel(bool win)
	{
		DeloadLevel();
		if (win)
		{
			LoadLevel(playerSave.level + 1);
		}
		else
		{
			LoadLevel(playerSave.level);
		}
	}

	void DeloadLevel()
	{
		currentLevel = null;
	}

	Level currentLevel;

	public void CreateLevel(int levelNumber)
	{
		if (currentLevel == null)
		{
			currentLevel = new Level();
		}
		else
		{
			Reset(currentLevel);
		}

		Copy(currentLevel, Levels.Instance.GetLevel((Language)playerSave.language) [levelNumber]);

		board = new char[currentLevel.sizeOfBoardX, currentLevel.sizeOfBoardY];
		BuildBoard();
	}

	public void CreateLevel(int levelNumber, PauseData data)
	{
		if (currentLevel == null)
		{
			currentLevel = new Level();
		}
		else
		{
			Reset(currentLevel);
		}

		Copy(currentLevel, Levels.Instance.GetLevel((Language)playerSave.language) [levelNumber]);

		for (int i = 0; i < currentLevel.words.Count; i++)
		{
			if (data.found [currentLevel.words [i].wordString] == null)
			{
				Debug.Log("Word: " + currentLevel.words [i].wordString + " not Loaded");
				continue;
			}
			currentLevel.words [i].found = data.found [currentLevel.words [i].wordString];
		}

		board = new char[currentLevel.sizeOfBoardX, currentLevel.sizeOfBoardY];
		BuildBoard(data.board);
	}


	void BuildBoard()
	{
		StartCoroutine(PopulateWords());
	}

	void BuildBoard(string b)
	{
		for (int i = 0; i < board.GetLength(0); i++)
		{
			for (int j = 0; j < board.GetLength(1); j++)
			{
				board [i, j] = b [(i * board.GetLength(1) + j)];
			}
		}
		StartCoroutine(CheckBoard());
	}


	IEnumerator PopulateWords()
	{
		Debug.Log("POPULATING WORDS");

		List<Vector2>[] paths = new List<Vector2>[currentLevel.words.Count];

		for (int i = 0; i < currentLevel.words.Count; i++)
		{
			paths [i] = FindSuitablePath(currentLevel.words [i].wordString);
		}

		yield return new WaitForEndOfFrame();

		for (int i = 0; i < paths.Length; i++)
		{
			currentLevel.words [i].path = paths [i];
			for (int j = 0; j < paths [i].Count; j++)
			{
				board [Mathf.RoundToInt(paths [i] [j].x), Mathf.RoundToInt(paths [i] [j].y)] = currentLevel.words [i].wordString [j];
			}
		}

		Debug.Log("DONE POPULATING WORDS");

		StartCoroutine(PopulateLetters());
	}

	IEnumerator PopulateLetters()
	{
		Debug.Log("POPULATING LETTERS");

		for (int i = 0; i < board.GetLength(0); i++)
		{
			for (int j = 0; j < board.GetLength(1); j++)
			{
				if (SearchTile(i, j, '\0', true))
				{
					board [i, j] = PickRandomLetter();
				}
			}
		}

		yield return new WaitForEndOfFrame();
		Debug.Log("DONE POPULATING LETTERS");

		StartCoroutine(CheckBoard());
	}

	char PickRandomLetter()
	{
		string letter = "e";
		return letter [0];
	}

	IEnumerator CheckBoard()
	{
		Debug.Log("POPULATING LETTERS");
		yield return new WaitForEndOfFrame();
		Debug.Log("DONE POPULATING LETTERS");

		StartCoroutine(CreateBoard());
	}

	IEnumerator CreateBoard()
	{
		Debug.Log("CREATING BOARD");


		GridSpawner.Instance.SpawnGrid(board.GetLength(0), board.GetLength(1));
		SpawnGrid(board.GetLength(0), board.GetLength(1));

		for (int i = 0; i < board.GetLength(0); i++)
		{
			for (int j = 0; j < board.GetLength(1); j++)
			{
				FieldController.Instance.fields [i, j].text = board [i, j].ToString();
			}
		}

		yield return new WaitForEndOfFrame();
		Debug.Log("DONE CREATING BOARD");
	}


	List<Vector2> FindSuitablePath(string word)
	{
		List<List<Vector2>> allPossiblePaths = new List<List<Vector2>>();

		List<Vector2> allStartingPoints = new List<Vector2>();

		for (int x = (0); x < (board.GetLength(0)); x++)
		{
			for (int y = (0); y < (board.GetLength(1)); y++)
			{
				if (SearchTile(x, y, word [0], true))
				{
					allStartingPoints.Add(new Vector2(x, y));
				}
			}
		}

		for (int i = 0; i < allStartingPoints.Count; i++)
		{
			List<Vector2> dirToCheck = FindDirectionsToCheck(Mathf.RoundToInt(allStartingPoints [i].x), Mathf.RoundToInt(allStartingPoints [i].y), word);
			if (dirToCheck.Count == 0)
			{
				allStartingPoints.Remove(allStartingPoints [i]);
				i--;
				continue;
			}

			bool continueSearching = true;
			bool foundPossiblePath = false;

			//For every direction to check
			for (int j = 0; j < dirToCheck.Count; j++)
			{
				foundPossiblePath = false;
				continueSearching = true;

				// Start at 2 since we have the first 2 letters
				for (int l = 1; l < word.Length; l++)
				{

					int xx = Mathf.RoundToInt(allStartingPoints [i].x) + (Mathf.RoundToInt(dirToCheck [j].x) * (l));
					int yy = Mathf.RoundToInt(allStartingPoints [i].y) + (Mathf.RoundToInt(dirToCheck [j].y) * (l));

					if (!SearchTile(xx, yy, word [l], true))
					{
						continueSearching = false;
					}
					else
					{
						if (l == word.Length - 1)
						{
							foundPossiblePath = true;
						}
					}

					if (!continueSearching)
					{
						break;
					}
				}

				if (foundPossiblePath)
				{
					allPossiblePaths.Add(new List<Vector2>());
					for (int p = 0; p < (word.Length); p++)
					{

						allPossiblePaths [(allPossiblePaths.Count - 1)].Add(
							new Vector2(
								Mathf.RoundToInt(allStartingPoints [i].x) + (Mathf.RoundToInt(dirToCheck [j].x) * (p)),
								Mathf.RoundToInt(allStartingPoints [i].y) + (Mathf.RoundToInt(dirToCheck [j].y) * (p))));
					}
				}
			}
		}

		if (allPossiblePaths.Count == 0)
		{
			return new List<Vector2>();
		}

		return allPossiblePaths [UnityEngine.Random.Range(0, allPossiblePaths.Count)];
	}

	bool SearchTile(int x, int y, char letter, bool inculdeBlanks = false)
	{
		if (x < 0 || y < 0 || x > board.GetLength(0) - 1 || y > board.GetLength(1) - 1)
		{
			return false;
		}

		if (board [x, y] == '\0' || board [x, y] == ' ')
		{
			return inculdeBlanks;
		}

		if (board [x, y] == letter)
		{
			return true;
		}
		return false;
	}

	List<Vector2> FindDirectionsToCheck(int x, int y, string word)
	{

		//Find Second Letter

		List<Vector2> directionsToCheck = new List<Vector2>();

		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (i == 0 && j == 0)
				{
					continue;
				}

				if (!currentLevel.backwards && (i == -1 || j == 1))
				{
					continue;
				}

				if (!currentLevel.diagonals && ((i == -1 && j == -1) || (i == -1 && j == 1) || (i == 1 && j == -1) || (i == 1 && j == 1)))
				{
					continue;
				}

				int xx = x + i;
				int yy = y + j;



				if (SearchTile(xx, yy, word [1], true))
				{
					directionsToCheck.Add(new Vector2(i, j));
				}
			}
		}

		return directionsToCheck;
	}



	public static void Reset(Level data)
	{
		data.sizeOfBoardX = 0;
		data.sizeOfBoardY = 0;
		data.words.Clear();
	}

	public static void Copy(Level dataTo, Level dataFrom)
	{
		if (dataTo == null || dataFrom == null)
		{
			return;
		}

		dataTo.sizeOfBoardX = dataFrom.sizeOfBoardX;
		dataTo.sizeOfBoardY = dataFrom.sizeOfBoardY;

		for (int i = 0; i < dataFrom.words.Count; i++)
		{
			dataTo.words.Add(dataFrom.words [i]);
		}
	}
}
