using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class PlayerData
{
	public int level = 0;
	public int latestLevel = 0;
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

	public Dictionary<string, bool> found = new Dictionary<string, bool>();
	public string board;
}

public static class SaveLoad
{
	private const string fileType = ".txt";
	private const string encasingFolders = "/Files/Saves/";
	private const string PlayerSaveName = "PlayerSave";
	private const string GameStateName = "GameState";


	public static string GetFilePath(string name)
	{
		#if UNITY_EDITOR
		return Application.dataPath + encasingFolders + name + fileType;
		#elif UNITY_ANDROID
		return Application.persistentDataPath + name + fileType;
		#elif UNITY_IPHONE
		return Application.persistentDataPath + "\" + name + fileType;
		#else
		return Application.dataPath + encasingFolders + name + fileType;
		#endif
	}

	public static void CreateFile(string path)
	{
		Debug.LogWarning("Failed to find data for Player: \n Creating file");
		File.Create(path);
	}

	public static void SavePlayer(PlayerData data)
	{
		if (!File.Exists(GetFilePath(PlayerSaveName)))
		{
			CreateFile(GetFilePath(PlayerSaveName));
		}

		FileStream file = new FileStream(GetFilePath(PlayerSaveName), FileMode.OpenOrCreate, FileAccess.ReadWrite);
        
		using (file)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(file, data);
		}

		file.Close();
	}


	public static PlayerData LoadPlayerData()
	{
		if (File.Exists(GetFilePath(PlayerSaveName)))
		{
			FileStream file = new FileStream(GetFilePath(PlayerSaveName), FileMode.Open, FileAccess.ReadWrite);
			BinaryFormatter bf = new BinaryFormatter();
			PlayerData data = (PlayerData)bf.Deserialize(file);
			file.Flush();
			file.Close();
			return data;
		}
		else
		{
			return new PlayerData();
		}
	}

	public static void SaveGameState(PauseData data)
	{
		if (!File.Exists(GetFilePath(GameStateName)))
		{
			CreateFile(GetFilePath(GameStateName));
		}

		FileStream file = new FileStream(GetFilePath(GameStateName), FileMode.OpenOrCreate, FileAccess.ReadWrite);
        
		using (file)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(file, data);
		}

		file.Close();
	}

	public static PauseData LoadLevel()
	{
		if (File.Exists(GetFilePath(GameStateName)))
		{
			FileStream file = new FileStream(GetFilePath(GameStateName), FileMode.Open, FileAccess.ReadWrite);
			BinaryFormatter bf = new BinaryFormatter();
			PauseData data = (PauseData)bf.Deserialize(file);
			file.Flush();
			file.Close();
			return data;
		}
		else
		{
			return new PauseData();
		}
	}
}