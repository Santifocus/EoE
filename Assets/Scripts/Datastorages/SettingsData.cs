using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace EoE
{
	public static class SettingsData
	{
		private const string SETTINGS_FILE_NAME = "Settings";
		private const int EXPECTED_FILE_LENGHT = 15; //3 floats & 3 bools
		private const string SETTINGS_FILE_EXTENSION = "data";
		private const string DOT = ".";
		private const string SAVE_FOLDER_PATH = "Data";

		//Default Settings
		private const float MUSIC_VOLUME_SCALE_DEFAULT = 1;
		private const float SOUND_VOLUME_SCALE_DEFAULT = 1;
		private const float GAMMA_DEFAULT = 0;
		private const bool TARGET_AS_TOGGLE = false;
		private const bool INVERT_CAMERA_X_DEFAULT = false;
		private const bool INVERT_CAMERA_Y_DEFAULT = true;

		//Current Non Saved Settings
		public static float ActiveMusicVolumeScale = MUSIC_VOLUME_SCALE_DEFAULT;
		public static float ActiveSoundVolumeScale = SOUND_VOLUME_SCALE_DEFAULT;
		public static float ActiveGamma = GAMMA_DEFAULT;
		public static bool ActiveTargetAsToggle = TARGET_AS_TOGGLE;
		public static bool ActiveInvertCameraX = INVERT_CAMERA_X_DEFAULT;
		public static bool ActiveInvertCameraY = INVERT_CAMERA_Y_DEFAULT;

		//Current Actuall Settings
		public static float MusicVolumeScale = MUSIC_VOLUME_SCALE_DEFAULT;
		public static float SoundVolumeScale = SOUND_VOLUME_SCALE_DEFAULT;
		public static float Gamma = GAMMA_DEFAULT;
		public static bool TargetAsToggle = TARGET_AS_TOGGLE;
		public static bool InvertCameraX = INVERT_CAMERA_X_DEFAULT;
		public static bool InvertCameraY = INVERT_CAMERA_Y_DEFAULT;

		public static void ReadOrCreate()
		{
			if(!Read())
			{
				SaveOrCreateSettings();
			}
		}
		public static void ResetSettings()
		{
			ActiveMusicVolumeScale = MusicVolumeScale = MUSIC_VOLUME_SCALE_DEFAULT;
			ActiveSoundVolumeScale = SoundVolumeScale = SOUND_VOLUME_SCALE_DEFAULT;
			ActiveGamma = Gamma = GAMMA_DEFAULT;
			ActiveTargetAsToggle = TargetAsToggle = TARGET_AS_TOGGLE;
			ActiveInvertCameraX = InvertCameraX = INVERT_CAMERA_X_DEFAULT;
			ActiveInvertCameraY = InvertCameraY = INVERT_CAMERA_Y_DEFAULT;

			SaveOrCreateSettings();
		}
		public static void SaveOrCreateSettings()
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

				writer.Write(MusicVolumeScale = ActiveMusicVolumeScale);
				writer.Write(SoundVolumeScale = ActiveSoundVolumeScale);
				writer.Write(Gamma = ActiveGamma);
				writer.Write(TargetAsToggle = ActiveTargetAsToggle);
				writer.Write(InvertCameraX = ActiveInvertCameraX);
				writer.Write(InvertCameraY = ActiveInvertCameraY);
			}
		}
		private static bool Read()
		{
			string savePath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER_PATH, SETTINGS_FILE_NAME + DOT + SETTINGS_FILE_EXTENSION);

			if (!File.Exists(savePath))
				return false;

			Debug.Log("Reading at: " + savePath);
			using (FileStream fileStream = File.Open(savePath, FileMode.Open))
			{
				if (fileStream.Length != EXPECTED_FILE_LENGHT)
				{
					fileStream.Close();
					return false;
				}

				BinaryReader reader = new BinaryReader(fileStream);

				ActiveMusicVolumeScale = MusicVolumeScale = reader.ReadSingle();
				ActiveSoundVolumeScale = SoundVolumeScale = reader.ReadSingle();
				ActiveGamma = Gamma = reader.ReadSingle();
				ActiveTargetAsToggle = TargetAsToggle = reader.ReadBoolean();
				ActiveInvertCameraX = InvertCameraX = reader.ReadBoolean();
				ActiveInvertCameraY = InvertCameraY = reader.ReadBoolean();
			}
			return true;
		}
	}
}