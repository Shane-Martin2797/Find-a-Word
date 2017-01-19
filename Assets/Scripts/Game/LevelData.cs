using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Level
{

	public Level()
	{
	}

	public Level(int sizeX, int sizeY, List<string> strings)
	{

		sizeOfBoardX = sizeX;
		sizeOfBoardY = sizeY;
		for (int i = 0; i < strings.Count; i++)
		{
			words.Add(new Word(strings [i]));
		}
		GetLevelData();
	}

	public bool diagonals = true;
	public bool backwards = true;

	public int sizeOfBoardX, sizeOfBoardY;

	public List<Word> words = new List<Word>();

	private LevelData data = new LevelData();

	public LevelData GetLevelData()
	{
		data.sizeOfBoardX = sizeOfBoardX;
		data.sizeOfBoardY = sizeOfBoardY;

		data.words.Clear();
		data.found.Clear();
		for (int i = 0; i < words.Count; i++)
		{
			data.words.Add(words [i].wordString);
			data.found.Add(words [i].found);
		}

		return data;
	}

	public void AddWord(string w)
	{
		words.Add(new Word(w));
	}
}

[System.Serializable]
public class LevelData
{
	public bool diagonals = true;
	public bool backwards = true;

	public int sizeOfBoardX;
	public int sizeOfBoardY;
	public List<string> words = new List<string>();
	public List<bool> found = new List<bool>();
}

[System.Serializable]
public class PauseData
{
	public int level;

	public int second;
	public int minute;
	public int hour;

	public int day;
	public int month;
	public int year;

	public List<bool> found = new List<bool>();
	public string board;

	public bool diagonals = true;
	public bool backwards = true;
}