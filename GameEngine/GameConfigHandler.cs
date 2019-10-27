using System;
using Newtonsoft.Json;
namespace GameEngine
{
    public class GameConfigHandler
    {
        private const string FileName = "Continue.json";
        
        public static void SaveConfig(GameSettings settings, string fileName = FileName)
        {
            using (var writer = System.IO.File.CreateText(fileName))
            {
                settings.FileName = fileName;
                var jsonString = JsonConvert.SerializeObject(settings);
                writer.Write(jsonString);
            }
        }

        public static GameSettings LoadConfig(string fileName = FileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                var jsonString = System.IO.File.ReadAllText(fileName);
                var res = JsonConvert.DeserializeObject<GameSettings>(jsonString);
                    return res;
            }
            
            return new GameSettings();
        }

    }
}