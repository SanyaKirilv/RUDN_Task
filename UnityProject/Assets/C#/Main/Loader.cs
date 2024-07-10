using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    [Header("Initial data json")]
    public TextAsset InitialData;
    [Header("Objects")]
    [SerializeField] private List<MovableObject> objects;
    [Header("Data")]
    [SerializeField] private SaveData saveData;

    private string FilePath => Path.Combine(Application.persistentDataPath, FileName);
    private string FileName => $"_data.json";
    private bool CheckForExists => File.Exists(FilePath);

    private void Start()
    {
        if (!CheckForExists)
            SaveData();
        LoadData();
    }

    public void SaveData()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            var obj = objects[i].gameObject;
            var data = saveData.objects.Find(x => x.objectName == obj.name);

            if (data != null)
            {
                data.position.SetData(obj.transform.position);
                data.rotation.SetData(obj.transform.rotation.eulerAngles);
            }
            else
            {
                Debug.LogWarning($"Object {obj.name} not found.");
            }
        }
        print(FilePath);
        File.WriteAllText(FilePath, JsonUtility.ToJson(saveData));
    }

    public void LoadData()
    {
        saveData = CheckForExists ?
            JsonUtility.FromJson<SaveData>(File.ReadAllText(FilePath)) :
            saveData;
        UpdateData();
    }

    private void UpdateData()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            var obj = objects[i];
            var data = saveData.objects.Find(x => x.objectName == obj.name);

            if (data != null)
            {
                obj.defaultPosition = data.position.GetData;
                obj.defaultRotation = data.rotation.GetData;
            }
            else
            {
                Debug.LogWarning($"Object {obj.name} not found in save data.");
            }
        }
    }

    [ContextMenu("Force Delete")]
    private void ForceDeleteFile()
    {
        File.Delete(FilePath);
    }
}