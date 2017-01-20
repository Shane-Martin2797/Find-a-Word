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
	}

	public bool diagonals = true;
	public bool backwards = true;

	public int sizeOfBoardX, sizeOfBoardY;

	public List<Word> words = new List<Word>();

	public void AddWord(string w)
	{
		words.Add(new Word(w));
	}
}