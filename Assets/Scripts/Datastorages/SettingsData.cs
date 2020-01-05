using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace EoE
{
	public static class SettingsData
	{
		private const string SETTINGS_FILE_NAME = "Settings";
		private const string SETTINGS_FILE_EXTENSION = "data";
		private const string DOT = ".";
		private const string SAVE_FOLDER_PATH = "Data";

		//Default Settings
		private const float MUSIC_VOLUME_SCALE_DEFAULT = 1;
		private const float SOUND_VOLUME_SCALE_DEFAULT = 1;
		private const float GAMMA_DEFAULT = 0;
		private const bool INVERT_CAMERA_X_DEFAULT = false;
		private const bool INVERT_CAMERA_Y_DEFAULT = true;

		//Current Non Saved Settings
		public static float ActiveMusicVolumeScale = MUSIC_VOLUME_SCALE_DEFAULT;
		public static float ActiveSoundVolumeScale = SOUND_VOLUME_SCALE_DEFAULT;
		public static float ActiveGamma = GAMMA_DEFAULT;
		public static bool ActiveInvertCameraX = INVERT_CAMERA_X_DEFAULT;
		public static bool ActiveInvertCameraY = INVERT_CAMERA_Y_DEFAULT;

		//Current Actuall Settings
		public static float MusicVolumeScale = MUSIC_VOLUME_SCALE_DEFAULT;
		public static float SoundVolumeScale = SOUND_VOLUME_SCALE_DEFAULT;
		public static float Gamma = GAMMA_DEFAULT;
		public static bool InvertCameraX = INVERT_CAMERA_X_DEFAULT;
		public static bool InvertCameraY = INVERT_CAMERA_Y_DEFAULT;

		public static void ReadOrCreate()
		{
			string savePath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER_PATH, SETTINGS_FILE_NAME + DOT + SETTINGS_FILE_EXTENSION);
			if (File.Exists(savePath))
			{
				Read();
			}
			else
			{
				SaveSettings();
			}
		}
		public static void ResetSettings()
		{
			ActiveMusicVolumeScale = MusicVolumeScale = MUSIC_VOLUME_SCALE_DEFAULT;
			ActiveSoundVolumeScale = SoundVolumeScale = SOUND_VOLUME_SCALE_DEFAULT;
			ActiveGamma = Gamma = GAMMA_DEFAULT;
			ActiveInvertCameraX = InvertCameraX = INVERT_CAMERA_X_DEFAULT;
			ActiveInvertCameraY = InvertCameraY = INVERT_CAMERA_Y_DEFAULT;

			SaveSettings();
		}
		public static void SaveSettings()
		{
			string folderPath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER_PATH);
			string savePath = Path.Combine(folderPath, SETTINGS_FILE_NAME + DOT + SETTINGS_FILE_EXTENSION);

			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			Debug.Log("Saving at: " + savePath);
			using (FileStream fileStream = File.Open(savePath, FileMode.Create))
			{
				BinaryWriter writer = new BinaryWriter(fileStream);

				writer.Write(ActiveMusicVolumeScale);
				writer.Write(ActiveSoundVolumeScale);
				writer.Write(ActiveGamma);
				writer.Write(ActiveInvertCameraX);
				writer.Write(ActiveInvertCameraY);
			}

			//Update Settings
			MusicVolumeScale = ActiveMusicVolumeScale;
			SoundVolumeScale = ActiveSoundVolumeScale;
			Gamma = ActiveGamma;
			InvertCameraX = ActiveInvertCameraX;
			InvertCameraY = ActiveInvertCameraY;
		}
		private static void Read()
		{
			string savePath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER_PATH, SETTINGS_FILE_NAME + DOT + SETTINGS_FILE_EXTENSION);

			if (!File.Exists(savePath))
				return;

			Debug.Log("Reading at: " + savePath);
			using (FileStream fileStream = File.Open(savePath, FileMode.Open))
			{
				BinaryReader reader = new BinaryReader(fileStream);

				ActiveMusicVolumeScale = MusicVolumeScale = reader.ReadSingle();
				ActiveSoundVolumeScale = SoundVolumeScale = reader.ReadSingle();
				ActiveGamma = Gamma = reader.ReadSingle();
				ActiveInvertCameraX = InvertCameraX = reader.ReadBoolean();
				ActiveInvertCameraY = InvertCameraY = reader.ReadBoolean();
			}
		}
	}
}