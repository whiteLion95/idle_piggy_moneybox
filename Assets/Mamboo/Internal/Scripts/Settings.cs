using System;
using UnityEngine;

namespace Mamboo.Internal.Scripts
{
    public static class Settings
    {
        private const string VIBRO_KEY = "vibro";
        private const string SOUND_KEY = "sound";
        private const string START_GAME_STAMP = "start_game_stamp";

        public static bool VibrationIsActive { get; private set; } = true;
        public static bool SoundIsActive { get; private set; } = true;
        public static bool IsFirstRun { get; set; } = false;

        public static bool isLoad;

        public static void Load()
        {
            isLoad = true;

            LoadPlayerSettings();
        }
    
        private static void LoadPlayerSettings()
        {
            bool settingsExist = PlayerPrefs.HasKey(VIBRO_KEY)
                                 && PlayerPrefs.HasKey(SOUND_KEY);

            if (settingsExist == false)
                Create();
            else
                Load();

            void Create()
            {
                VibrationIsActive = true;
                SoundIsActive = true;
                IsFirstRun = true;

                PlayerPrefs.SetInt(VIBRO_KEY, 1);
                PlayerPrefs.SetInt(SOUND_KEY, 1);
                PlayerPrefs.SetString(START_GAME_STAMP, DateTime.Now.ToShortDateString());
            }

            void Load()
            {
                VibrationIsActive = PlayerPrefs.GetInt(VIBRO_KEY) != 0;
                SoundIsActive = PlayerPrefs.GetInt(SOUND_KEY) != 0;
            }
        }
    
        public static DateTime GetStartGameDate()
        {
            string value = PlayerPrefs.GetString(START_GAME_STAMP);

            DateTime date;
            if (value != null && DateTime.TryParse(value, out date))
            {
                return date;
            }
            return DateTime.MinValue;
        }
    }
}