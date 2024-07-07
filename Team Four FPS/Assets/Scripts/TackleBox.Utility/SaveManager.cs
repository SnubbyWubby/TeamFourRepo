using System;
using System.IO;
using UnityEngine;

namespace MyGame.SaveSystem
{
    [Serializable]
    public class SaveData
    {
        public int totalScore = 0;
        public float totalTime = 0f;
    }
    public class SaveManager : MonoBehaviour
    {
        private static SaveManager _instance;
        private static SaveData _currentData;
        public static SaveManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Search for an existing instance in the scene
                    _instance = FindObjectOfType<SaveManager>();

                    // If none exists, create a new GameObject and attach SaveManager to it
                    if (_instance == null)
                    {
                        GameObject saveManager = new GameObject("SaveManager");
                        _instance = saveManager.AddComponent<SaveManager>();
                    }

                    // Mark the instance to not be destroyed on scene load
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        public static SaveData CurrentData
        {
            get 
            { 
                if ( _currentData == null)
                    _currentData = new SaveData();

                return _currentData;
            }
            set
            {
                if (value != null && value is SaveData)
                    _currentData = value;
            }

        }

        // This method will be called before any scene is loaded
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            // Ensure the instance is created
            SaveManager instance = Instance;
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(_instance.gameObject);
            }
            else if (_instance != this)
            {
                Destroy(_instance.gameObject);
            }
        }

        public void Save(string filename)
        {
            string path = GetPath(filename);
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    string json = JsonUtility.ToJson(CurrentData, true);
                    writer.Write(json);
                }
                Debug.Log($"Data successfully saved to {path}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save data to {path}: {e.Message}");
            }
        }

        public SaveData Load(string filename)
        {
            string path = GetPath(filename);
            if (File.Exists(path))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        string json = reader.ReadToEnd();
                        SaveData data = JsonUtility.FromJson<SaveData>(json);
                        CurrentData = data;
                        return data;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load data from {path}: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"Save file not found at {path}");
            }
            return default;
        }

        private string GetPath(string filename)
        {
            return Path.Combine(Application.persistentDataPath, filename);
        }
    }
}
