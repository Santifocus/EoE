using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace EoE.Information
{
	public class PlayerData
	{
		private static readonly string BASE_PATH = Application.persistentDataPath;
		private const string DATA_FOLDER = "Saves";
		private const string DATA_EXTENSION = ".plydata";

		public static PlayerData LoadedData;

		public static PlayerData[] GetAllPlayerData()
		{
			return null;
		}

		public static bool SaveExist(int saveID)
		{
			return File.Exists(SaveIDToPath(saveID));
		}

		private static string SaveIDToPath(int saveID)
		{
			return "";
		}

		public static void SavePlayerData(int saveID)
		{
			string savePath = SaveIDToPath(saveID);

			if (SaveExist(saveID))
			{
				File.Delete(savePath);
			}

			using (FileStream fs = File.Create(BASE_PATH))
			{
				BinaryWriter writer = new BinaryWriter(fs);
			}
		}
	}
}
