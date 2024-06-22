using Assets.Scripts.Consts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private TextMeshProUGUI instructionsText;
    private List<string> sceneNames = new List<string>();
    private List<KeyCode> allowedSceneLoadButtons = new List<KeyCode>
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
        KeyCode.Alpha0,
    };
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instructionsPanel.SetActive(Debug.isDebugBuild);
        if (Debug.isDebugBuild)
        {
            FetchScenes();
            string sceneName = sceneNames.FirstOrDefault(s => s != SceneManager.GetActiveScene().name);
            if (!string.IsNullOrWhiteSpace(sceneName))
            {
                LoadScene(sceneName);
            }
        }
    }

    private void FetchScenes()
    {
        StringBuilder scenesStringBuilder = new StringBuilder();
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        scenesStringBuilder.AppendLine("Press ESC to exit");
        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            sceneNames.Add(sceneName);
            scenesStringBuilder.AppendLine($"Press \"{i + 1}\" to load {sceneName} scene");
        }
        instructionsText.text = scenesStringBuilder.ToString();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        foreach (var sceneLoadButton in allowedSceneLoadButtons)
        {
            if (Input.GetKeyDown(sceneLoadButton))
            {
                int index = allowedSceneLoadButtons.IndexOf(sceneLoadButton);
                string sceneName = sceneNames[index];
                LoadScene(sceneName);
            }
        }
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
