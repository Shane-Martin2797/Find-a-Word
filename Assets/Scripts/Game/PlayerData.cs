using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class PlayerData
{
	public int level = 0;
}

public static class SaveLoad
{
	private const string fileType = ".txt";
	private const string encasingFolders = "/Files/Saves/";


	public static string GetPlayerSaveFilePath()
	{
		#if UNITY_EDITOR
		return Application.dataPath + encasingFolders + "PlayerSave" + fileType;
		#elif UNITY_ANDROID
		return Application.persistentDataPath + "PlayerSave" + fileType;
		#elif UNITY_IPHONE
		return Application.persistentDataPath + "\" + "PlayerSave" + fileType;
		#else
		return Application.dataPath + encasingFolders + "PlayerSave" + fileType;
		#endif
	}


	public static void CreateFile(PlayerData data)
	{
		Debug.LogWarning("Failed to find data for Player: \n Creating file");
		File.Create(GetPlayerSaveFilePath());
	}

	public static void Save(PlayerData data)
	{
		if (!File.Exists(GetPlayerSaveFilePath()))
		{
			CreateFile(data);
		}

		FileStream file = new FileStream(GetPlayerSaveFilePath(), FileMode.OpenOrCreate, FileAccess.ReadWrite);
        
		using (file)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(file, data);
		}

		file.Close();
	}


	public static PlayerData Load()
	{
		if (File.Exists(GetPlayerSaveFilePath()))
		{
			FileStream file = new FileStream(GetPlayerSaveFilePath(), FileMode.Open, FileAccess.ReadWrite);
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

}