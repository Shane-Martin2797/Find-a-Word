using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameController : SingletonBehaviour<GameController>
{
	public PlayerData playerSave;
	bool paused = false;

	public char[,] board;

	void OnEnable()
	{
		playerSave = SaveLoad.Load();
	}

	void OnDisable()
	{
		SaveLoad.Save(playerSave);
	}


	void OnApplicationFocus(bool hasFocus)
	{
		paused = !hasFocus;
		if (paused)
		{
			PauseApplication();
		}
		else
		{
			UnPauseApplication();
		}
	}

	void OnApplicationPause(bool pauseState)
	{
		paused = pauseState;
		if (paused)
		{
			PauseApplication();
		}
		else
		{
			UnPauseApplication();
		}
	}

	private PauseData pauseData;

	void PauseApplication()
	{
		DateTime now = DateTime.Now;

		pauseData.second = now.Second;
		pauseData.minute = now.Minute;
		pauseData.hour = now.Hour;

		pauseData.day = now.Day;
		pauseData.month = now.Month;
		pauseData.year = now.Year;

		pauseData.level = playerSave.level;
		pauseData.found = currentLevel.GetLevelData().found;
		pauseData.board = LevelToString();

	}

	public string LevelToString()
	{

	}

	void UnPauseApplication()
	{
	}

	public void LoadLevel(int level)
	{
		playerSave.level = level;
		if (playerSave.latestLevel < level)
		{
			playerSave.latestLevel = level;
		}

		CreateLevel(level);
	}

	public void CompleteLevel(bool win)
	{
		if (win)
		{
			LoadLevel(playerSave.level + 1);
		}
		else
		{
			LoadLevel(playerSave.level);
		}
	}

	Level currentLevel;

	public void CreateLevel(int levelNumber)
	{
		//Level data = LevelEditor.DataToLevel(LevelEditor.LoadLevel(levelNumber));

		//Create a local version of the data so we can edit word paths and keep a copy of it.
		if (currentLevel == null)
		{
			currentLevel = new Level();
		}

		Reset(currentLevel);

		//Copy(currentLevel, data);

		currentLevel.sizeOfBoardX = 5;
		currentLevel.sizeOfBoardY = 5;

		currentLevel.AddWord(("but"));
		currentLevel.AddWord(("edd"));

		LevelEditor.SaveLevel(levelNumber, currentLevel.GetLevelData());

		board = new char[currentLevel.sizeOfBoardX, currentLevel.sizeOfBoardY];
		BuildBoard();
	}

	void BuildBoard()
	{
		StartCoroutine(PopulateWords);
	}

	IEnumerator PopulateWords()
	{
		Debug.Log("POPULATING WORDS");

		List<Vector2>[] paths = new List<Vector2>[currentLevel.words.Count];

		for (int i = 0; i < currentLevel.words.Count; i++)
		{
			paths [i] = FindSuitablePath(currentLevel.words [i]);
		}

		for (int i = 0; i < paths.Length; i++)
		{
			for (int j = 0; j < paths [j].Count; j++)
			{
				board [paths [i] [j].x, paths [i] [j].y] = currentLevel.words [i].wordString [j];
			}
		}

		Debug.Log("DONE POPULATING WORDS");

		StartCoroutine(PopulateLetters());
	}

	IEnumerator PopulateLetters()
	{
		Debug.Log("POPULATING LETTERS");

		Debug.Log("DONE POPULATING LETTERS");

		StartCoroutine(ScrambleExtraLetters());
	}

	IEnumerator ScrambleExtraLetters()
	{
		Debug.Log("POPULATING LETTERS");
		Debug.Log("DONE POPULATING LETTERS");

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
			if (dirToCheck == 0)
			{
				allStartingPoints.Remove(allStartingPoints [i]);
				i--;
				continue;
			}

			bool continueSearching = true;
			bool foundPossiblePath = false;
			int dir = 0;

			//For every direction to check
			for (int j = 0; j < dirToCheck.Count; j++)
			{
				foundPossiblePath = false;
				continueSearching = true;
				dir = j;

				// Start at 2 since we have the first 2 letters
				for (int l = 2; l < word.Length; l++)
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
					for (int p = 0; p < word.Length - 1; p++)
					{
						allPossiblePaths [i].Add(
							new Vector2(Mathf.RoundToInt(allStartingPoints [i].x) + (Mathf.RoundToInt(dirToCheck [dir].x) * (p + 1)),
								Mathf.RoundToInt(allStartingPoints [i].y) + (Mathf.RoundToInt(dirToCheck [dir].y) * (p + 1))));
					}
				}
			}
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



				if (SearchTile(xx, yy, word [1]))
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
