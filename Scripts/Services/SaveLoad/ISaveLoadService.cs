using System.Collections.Generic;
using System.IO;
using Constructor;
using Services.SaveLoad.Data;
using UnityEngine;

namespace Services.SaveLoad
{
    public interface ISaveLoadService
    {
        void SaveLayers(LayersData data, string filePath);
        void SaveCharacters(CharactersData data, string filePath);
        void SaveCharacter(CharacterData data, string filePath);
        LayersData LoadLayers(string filePath);
    }

    public class SaveLoadService : ISaveLoadService
    {
        public void SaveLayers(LayersData data, string filePath)
        {
            Save(data, filePath);
        }

        public void SaveCharacters(CharactersData data, string filePath)
        {
            Save(data, filePath);
        }

        public void SaveCharacter(CharacterData data, string filePath)
        {
            Save(data, filePath);
        }

        public LayersData LoadLayers(string filePath)
        {
            using var reader = new StreamReader(filePath);
            var json = reader.ReadToEnd();
            var data = JsonUtility.FromJson<LayersData>(json);
            return data;
        }

        private static void Save(object data, string filePath)
        {
            var json = JsonUtility.ToJson(data);
            using var writer = new StreamWriter(File.Create(filePath));
            writer.Write(json);
        }
    }
}