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

	}

	// Use this for initialization
	void Start()
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
}
