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
	public int language = 0;
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
	private static string fileType = ".txt";
	private static string encasingFolders = "/Files/Saves/";
	private static string PlayerSaveName = "PlayerSave";
	private static string GameStateName = "GameState";


	public static string GetFilePath(string name)
	{
		#if UNITY_EDITOR
		return Application.dataPath + encasingFolders + name + fileType;
		#elif UNITY_ANDROID
		return Application.persistentDataPath + name + fileType;
		#elif UNITY_IPHONE
		return Application.persistentDataPath + "/" + name + fileType;
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


	#if UNITY_EDITOR

	private static string AmericanDictionary = "Dictionarys/American";
	private static string AustralianDictionary = "Dictionarys/Australian";
	private static string BritishDictionary = "Dictionarys/British";

	public static Dictionary<int, List<string>> ImportDictionary(Language lan)
	{
		Dictionary<int, List<string>> dic = new Dictionary<int, List<string>>();

		string path = "";

		switch (lan)
		{
			case Language.Australian:
				{
					path = GetFilePath(AustralianDictionary);
					break;
				}
			case Language.American:
				{
					path = GetFilePath(AustralianDictionary);
					break;
				}
			case Language.British:
				{
					path = GetFilePath(AustralianDictionary);
					break;
				}
			default:
				{
					path = GetFilePath(AustralianDictionary);
					break;
				}
		}


		if (!File.Exists(path))
		{
			return null;
		}

		StreamReader file = new StreamReader(path);
		string f = file.ReadToEnd();

		string[] lines = f.Split('\n');

		for (int i = 0; i < lines.Length; i++)
		{
			if (lines [i].Length > 15)
			{
				continue;
			}
			if (lines [i].Contains("'"))
			{
				continue;
			}

			lines [i] = lines [i].ToLower();

			if (!dic.ContainsKey(lines [i].Length))
			{
				dic.Add(lines [i].Length, new List<string>());
			}
			dic [lines [i].Length].Add(lines [i]);
		}

		return dic;
	}

	#endif

}