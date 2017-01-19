using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Word
{
	public string wordString;
	public List<Vector2> path = new List<Vector2>();
	public bool found = false;

	public Word()
	{
	}

	public Word(string s)
	{
		wordString = s;
	}

	public Word(string s, List<Vector2> p)
	{
		wordString = s;
		path = p;
	}

}

public class CrosswordSolver : SingletonBehaviour<CrosswordSolver>
{

	char[,] crossword;
	List<string> wordFinder = new List<string>();
	public List<Word> foundWords = new List<Word>();

	public bool diagonals = true;
	public bool backwards = true;

	public void Solve(InputField[,] grid = null, List<InputField> wordsToFind = null)
	{
		if (grid == null)
		{
			if (FieldController.Instance == null)
			{
				return;
			}
			grid = FieldController.Instance.fields;
		}

		if (wordsToFind == null)
		{
			if (FieldController.Instance == null)
			{
				return;
			}
			wordsToFind = WordsContentTracker.Instance.words;
		}

		foundWords.Clear();

		SetCrossword(grid);

		SetWordFinder(wordsToFind);

		SolveCrossword();
	}

	void SetCrossword(InputField[,] grid)
	{
		crossword = new char[grid.GetLength(0), grid.GetLength(1)];

		for (int i = 0; i < grid.GetLength(0); i++)
		{
			for (int j = 0; j < grid.GetLength(1); j++)
			{
				if (grid [i, j].text == "" || grid [i, j].text == " ")
				{
					Debug.LogWarning("Grid Position: (" + i + ", " + j + ") is missing a value");
				}
				else
				{
					crossword [i, j] = grid [i, j].text.ToLower() [0];
				}
			}
		}
	}

	void SetWordFinder(List<InputField> word)
	{

		for (int i = 0; i < word.Count; i++)
		{
			if (word [i] == null || word [i].text == null || word [i].text == "" || word [i].text == " ")
			{
				continue;
			}
			wordFinder.Add(word [i].text.ToLower());
		}
		
	}

	void SolveCrossword()
	{

		List<Vector2> path = new List<Vector2>();

		bool foundWord = false;

		for (int i = 0; i < wordFinder.Count; i++)
		{
			string wordToFind = wordFinder [i];
			foundWord = false;

			for (int x = 0; x < crossword.GetLength(0); x++)
			{

				if (foundWord)
				{
					break;
				}

				for (int y = 0; y < crossword.GetLength(1); y++)
				{
					if (crossword [x, y] == '\0' || crossword [x, y] == ' ')
					{
						continue;
					}

					if (SearchTile(x, y, wordToFind [0]))
					{
						path.Clear();
						Debug.Log("Found Letter " + wordToFind [0]);
						path.Add(new Vector2(x, y));

						if (SearchForWord(x, y, wordToFind, path))
						{
							foundWords.Add(new Word(wordToFind, path));
							foundWord = true;
						}
					}

					if (foundWord)
					{
						break;
					}
				}
			}
		}
	}

	bool SearchTile(int x, int y, char letter)
	{
		if (x < 0 || y < 0 || x > crossword.GetLength(0) - 1 || y > crossword.GetLength(1) - 1)
		{
			return false;
		}

		if (crossword [x, y] == '\0' || crossword [x, y] == ' ')
		{
			return false;
		}

		if (crossword [x, y] == letter)
		{
			return true;
		}
		return false;
	}

	bool SearchForWord(int x, int y, string word, List<Vector2> path)
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

				if (!backwards && (i == -1 || j == 1))
				{
					continue;
				}

				if (!diagonals && ((i == -1 && j == -1) || (i == -1 && j == 1) || (i == 1 && j == -1) || (i == 1 && j == 1)))
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


		if (directionsToCheck.Count == 0)
		{
			return false;
		}


		//Continue in that direction checking if the letters match.
		//Run a for loop for however many letters are left
		//check if it matches along the path

		bool continueSearching = true;
		bool foundWord = false;
		int dir = 0;

		//For every direction to check
		for (int j = 0; j < directionsToCheck.Count; j++)
		{
			dir = j;

			// Start at 2 since we have the first 2 letters
			for (int i = 2; i < word.Length; i++)
			{

				int xx = Mathf.RoundToInt(path [0].x) + (Mathf.RoundToInt(directionsToCheck [j].x) * (i));
				int yy = Mathf.RoundToInt(path [0].y) + (Mathf.RoundToInt(directionsToCheck [j].y) * (i));

				if (!SearchTile(xx, yy, word [(i)]))
				{
					continueSearching = false;
				}
				else
				{
					Debug.Log("Found Letter " + word [i]);
					if (i == word.Length - 1)
					{
						foundWord = true;
					}
				}

				if (!continueSearching)
				{
					break;
				}
			}

			if (foundWord)
			{
				break;
			}

			if (!continueSearching)
			{
				continueSearching = true;
				continue;
			}
		}

		for (int i = 0; i < word.Length - 1; i++)
		{
			path.Add(new Vector2(Mathf.RoundToInt(path [0].x) + (Mathf.RoundToInt(directionsToCheck [dir].x) * (i + 1)),
				Mathf.RoundToInt(path [0].y) + (Mathf.RoundToInt(directionsToCheck [dir].y) * (i + 1))));
		}

		if (foundWord)
		{
			return true;
		}


		return false;
	}

}
