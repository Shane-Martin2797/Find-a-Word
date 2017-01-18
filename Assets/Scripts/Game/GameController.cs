using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : SingletonBehaviour<GameController>
{

	public PlayerData playerSave;

	void OnEnable()
	{
		playerSave = SaveLoad.Load();
	}

	void OnDisable()
	{
		SaveLoad.Save(playerSave);
	}


	public void LoadLevel(int level)
	{
		playerSave.level = level;
		if (playerSave.latestLevel < level)
		{
			playerSave.latestLevel = level;
		}
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
}
