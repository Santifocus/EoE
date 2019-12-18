using System.IO;
using UnityEngine;

namespace EoE.Information
{
	public class DataManager
	{
		private static readonly string BASE_PATH = Application.persistentDataPath;
		private const string DATA_FOLDER = "Saves";
		private const string DATA_EXTENSION = ".plydata";

		public static PlayerData LoadedData;

		public static void SavePlayerData(int saveID)
		{
			string savePath = IDToPath(saveID);

			EnsureSaveFolder();
			if (SaveExist(saveID))
			{
				File.Delete(savePath);
			}

			using (FileStream fs = File.Create(savePath))
			{
				BinaryWriter writer = new BinaryWriter(fs);
			}
		}
		public static DataManager[] GetAllPlayerData()
		{
			return null;
		}

		public static bool SaveExist(int saveID)
		{
			return File.Exists(IDToPath(saveID));
		}

		public static string IDToPath(int saveID)
		{
			return Path.Combine(BASE_PATH, DATA_FOLDER, saveID + DATA_EXTENSION);
		}

		private static void EnsureSaveFolder()
		{
			string folderPath = Path.Combine(BASE_PATH, DATA_FOLDER);
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
		}
	}

	public class PlayerData
	{
		public Vector3 Position;
		public int Souls;
		public int[] AppliedSkillPoints;
	}
}
