using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagerCustom : MonoBehaviour
{
    [System.Serializable]
    public class SceneInfo
    {
        public string sceneName;
        public int sceneId;

        public SceneInfo(string name, int id)
        {
            sceneName = name;
            sceneId = id;
        }
    }

    public List<SceneInfo> scenes = new List<SceneInfo>();

    public Dropdown sceneDropdown;

    private void Awake()
    {
        if (sceneDropdown != null)
        {
            PopulateDropdown();
            sceneDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }
    }

    public void AddScene(string sceneName, int sceneId)
    {
        foreach (var scene in scenes)
        {
            if (scene.sceneId == sceneId)
            {
                Debug.LogError($"����� � ID {sceneId} ��� ����������!");
                return;
            }
        }

        scenes.Add(new SceneInfo(sceneName, sceneId));
    }

    public void LoadSceneById(int sceneId)
    {
        foreach (var scene in scenes)
        {
            if (scene.sceneId == sceneId)
            {
                SceneManager.LoadScene(scene.sceneName);
                return;
            }
        }

        Debug.LogError($"����� � ID {sceneId} �� �������!");
    }

    private void PopulateDropdown()
    {
        sceneDropdown.ClearOptions();

        List<string> options = new List<string> { "�������� �����" };
        foreach (var scene in scenes)
        {
            options.Add(scene.sceneName);
        }

        sceneDropdown.AddOptions(options);

        sceneDropdown.value = 0;
    }

    private void OnDropdownValueChanged(int index)
    {
        if (index == 0)
        {
            Debug.Log("������� ������� ������: ������ �� ����������.");
            return;
        }

        int actualIndex = index - 1; 
        if (actualIndex >= 0 && actualIndex < scenes.Count)
        {
            LoadSceneById(scenes[actualIndex].sceneId);
        }
        else
        {
            Debug.LogError("��������� �������� Dropdown ��� ���������!");
        }
    }
}
