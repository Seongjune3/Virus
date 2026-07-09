using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    public int currentWorld = 1;
    public int currentStage = 1;

    public string titleSceneName = "Title";

    public List<GameObject> stagePrefabs;

    GameObject currentMapInstance;

    [Header("UI Settings")]
    public GameObject clearUI;
    public GameObject nextStageButton;
    public GameObject endingButton;
    public GameObject settingsUI;
    public GameObject titleUI;

    public TextMeshProUGUI clearTitleText;
    public TextMeshProUGUI clearStageText;

    public List<Image> memoryImages;
    private int collectedMemories = 0;

    public bool IsCleared = false;

    public CanvasGroup fadeImage;
    public float fadeDuration = 1.0f;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static GameManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != titleSceneName)
        {
            if (titleUI != null) titleUI.SetActive(false);

            currentWorld = PlayerPrefs.GetInt("SavedWorld", 1);
            currentStage = PlayerPrefs.GetInt("SavedStage", 1);

            if (stagePrefabs != null && stagePrefabs.Count > 0)
            {
                SpawnStage(currentStage - 1);
            }
        }
        else
        {
            if (titleUI != null) titleUI.SetActive(true);
            clearUI.SetActive(false);
            settingsUI.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettings();
        }
    }

    public void AddMemory()
    {
        if (collectedMemories < 3)
        {
            collectedMemories++;
        }
    }

    public void SpawnStage(int stageIndex)
    {
        if (currentMapInstance != null)
        {
            currentMapInstance.SetActive(false);
            Destroy(currentMapInstance);
        }

        currentMapInstance = Instantiate(stagePrefabs[stageIndex], Vector3.zero, Quaternion.identity);
        currentStage = stageIndex + 1;

        IsCleared = false;
        collectedMemories = 0;

        clearUI.SetActive(false);
        settingsUI.SetActive(false);
    }

    public void StageClear()
    {
        if (IsCleared) return;
        IsCleared = true;

        for (int i = 0; i < memoryImages.Count; i++)
        {
            if (i < collectedMemories)
            {
                memoryImages[i].color = Color.white;
            }
            else
            {
                memoryImages[i].color = new Color32(55, 55, 55, 255);
            }
        }

        clearStageText.text = currentWorld + "-" + currentStage;
        if (currentStage >= stagePrefabs.Count)
        {
            clearTitleText.text = "올 클리어!!";
            nextStageButton.SetActive(false);
            endingButton.SetActive(true);
        }
        else
        {
            clearTitleText.text = "클리어!!";
            nextStageButton.SetActive(true);
            endingButton.SetActive(false);
        }

        clearUI.SetActive(true);
    }

    public void NextStage()
    {
        int nextIndex = currentStage;

        if (nextIndex < stagePrefabs.Count)
        {
            SpawnStage(nextIndex);
        }
    }

    public void Retry()
    {
        SpawnStage(currentStage - 1);
    }

    public void SaveGame()
    {
        int stageToSave = currentStage;

        if (IsCleared)
        {
            if (currentStage < stagePrefabs.Count)
            {
                stageToSave = currentStage + 1;
            }
            else
            {
                stageToSave = 1;
            }
        }

        PlayerPrefs.SetInt("SavedWorld", currentWorld);
        PlayerPrefs.SetInt("SavedStage", stageToSave);
        PlayerPrefs.Save();
    }

    public void LoadScene(string sceneName)
    {
        if (SceneManager.GetActiveScene().name == titleSceneName && sceneName == titleSceneName)
        {
            ToggleSettings();
            return;
        }

        SaveGame();
        
        StartCoroutine(FadeAndLoad(sceneName));
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.blocksRaycasts = true;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeImage.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        fadeImage.alpha = 1f;

        SceneManager.LoadScene(sceneName);

        yield return null; 

        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeImage.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        
        fadeImage.alpha = 0f;
        fadeImage.blocksRaycasts = false;
        fadeImage.gameObject.SetActive(false);
    }

    public void ToggleSettings()
    {
        bool isActive = !settingsUI.activeSelf;
        settingsUI.SetActive(isActive);

        if (isActive)
        {
            if (titleUI != null) titleUI.SetActive(false);
            clearUI.SetActive(false);
        }
        else
        {
            if (SceneManager.GetActiveScene().name == titleSceneName)
            {
                if (titleUI != null) titleUI.SetActive(true);
            }
            else if (IsCleared)
            {
                clearUI.SetActive(true);
            }
        }
    }

    public void QuitGame()
    {
        SaveGame();
        Application.Quit();
    }
}