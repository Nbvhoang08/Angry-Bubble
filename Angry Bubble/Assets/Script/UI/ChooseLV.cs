using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ChooseLV : UICanvas
{
    [SerializeField] private List<LevelData> levels = new List<LevelData>();
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button levelButtonPrefab;

    // [Header("Button Sprites")]
    // [SerializeField] private Sprite unlockedSprite;
    // [SerializeField] private Sprite lockedSprite;

    [Header("Level Number")]
    [SerializeField] private Color unlockedTextColor = Color.white;
    [SerializeField] private Color lockedTextColor = Color.gray;

    [Header("Scene Loading")]
    [SerializeField] private float loadDelay = 0.1f; // Delay trước khi load scene mới
    [SerializeField] private GameObject loadingScreen; // Optional: màn hình loading

    [Header("Layout Settings")]

    [SerializeField] private Vector2 buttonSize; // Kích thước của button
    private Coroutine loadLevelCoroutine;

    [Header("Text Settings")]
    [SerializeField] public int fontSize = 14; // Cỡ chữ
    [SerializeField] public Font font; // Phông chữ
   

    private void OnEnable()
    {
        InitializeLevelButtons();

        // Ẩn loading screen nếu có
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }
    private void OnDisable()
    {
        if (loadLevelCoroutine != null)
        {
            StopCoroutine(loadLevelCoroutine);
            loadLevelCoroutine = null;
        }
    }


    private void InitializeLevelButtons()
    {
         // Xoá các button hiện có
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < levels.Count; i++)
        {
            Button btn = Instantiate(levelButtonPrefab, buttonContainer);
            levels[i].levelButton = btn;

            RectTransform rectTransform = btn.GetComponent<RectTransform>();
            rectTransform.sizeDelta = buttonSize;

            Text buttonText = btn.GetComponentInChildren<Text>();
            UpdateBtn(levels[i]);

            if (buttonText != null)
            {
                buttonText.text = levels[i].levelName;
                buttonText.color = levels[i].isUnlocked ? unlockedTextColor : lockedTextColor;
                buttonText.fontSize = fontSize; // Thiết lập cỡ chữ
                buttonText.font = font; // Thiết lập phông chữ
            }
            btn.interactable = levels[i].isUnlocked;

            // Tìm object con tên là "Image" và điều chỉnh trạng thái theo isUnlocked
            Transform childImageTransform = btn.transform.Find("Image");
            if (childImageTransform != null)
            {
                childImageTransform.gameObject.SetActive(!levels[i].isUnlocked); // Bật/tắt dựa trên trạng thái
            }

            int levelIndex = i;
            btn.onClick.AddListener(() => LoadLevel(levelIndex));

            PlaySpawnAnimation(btn.gameObject, i * 0.1f);
        }
    }

    

    private void UpdateBtn(LevelData levelData)
    {
        if (!levelData.isUnlocked)
        {
            string savedScenes = PlayerPrefs.GetString("SavedScenes", string.Empty);
            if (!string.IsNullOrEmpty(savedScenes))
            {
                List<string> savedSceneList = new List<string>(savedScenes.Split(','));
                if (savedSceneList.Contains(levelData.sceneName))
                {
                    levelData.isUnlocked = true;

                    // Update button appearance
                    Text buttonText = levelData.levelButton.GetComponentInChildren<Text>();
                    if (buttonText != null)
                    {
                        buttonText.color = unlockedTextColor;
                        buttonText.fontSize = fontSize; // Thiết lập cỡ chữ
                        buttonText.font = font; // Thiết lập phông chữ
                    }

                    levelData.levelButton.interactable = true;

                    // Tìm object con tên là "Image" và điều chỉnh trạng thái
                    Transform childImageTransform = levelData.levelButton.transform.Find("Image");
                    if (childImageTransform != null)
                    {
                        childImageTransform.gameObject.SetActive(false); // Tắt object "Image" khi button được mở khóa
                    }
                }
            }
        }
    }




    private void PlaySpawnAnimation(GameObject button, float delay)
    {
        // Set initial scale to zero
        button.transform.localScale = Vector3.zero;

        // Start spawn animation
        StartCoroutine(SpawnAnimationRoutine(button, delay));
    }


    private IEnumerator SpawnAnimationRoutine(GameObject button, float delay)
    {
        yield return new WaitForSeconds(delay);
        float duration = 0.3f;
        float elapsed = 0f;
        SoundManager.Instance.PlayVFXSound(1);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Elastic ease out effect
            float scale = Mathf.Sin(-13f * (progress + 1) * Mathf.PI * 0.5f) * Mathf.Pow(2f, -10f * progress) + 1f;
            button.transform.localScale = Vector3.one * scale;

            yield return null;
        }
        button.transform.localScale = Vector3.one;
    }

    private void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levels.Count && levels[levelIndex].isUnlocked)
        {
            SoundManager.Instance.PlayClickSound();
            StartCoroutine(LoadLevelSequence(levelIndex));
        }
    }

    private IEnumerator LoadLevelSequence(int levelIndex)
    {
        // 1. Show loading screen if available
        yield return new WaitForSeconds(loadDelay);
        // 4. Load new scene asynchronously
        SceneManager.LoadScene(levels[levelIndex].sceneName);
        yield return null;
        yield return null; 
        // Đóng tất cả UI hiện tại
        UIManager.Instance.CloseUIDirectly<ChooseLV>();
        // Thiết lập UI cho gameplay
        SetupGameplayUI();
    }

    private void SetupGameplayUI()
    {
        // Ensure we're on the main thread
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }

        // Open Gameplay Canvas
        UIManager.Instance.OpenUI<GamePlay>();
    }

    private void LoadLevelProgress()
    {
        if (levels.Count > 0)
        {
            levels[0].isUnlocked = true;
        }
        for (int i = 0; i < levels.Count; i++)
        {
            string key = $"Level_{i}_Unlocked";
            levels[i].isUnlocked = PlayerPrefs.GetInt(key, i == 0 ? 1 : 0) == 1;
        }
    }
    public void UnlockNextLevel(int currentLevelIndex)
    {
        if (currentLevelIndex + 1 < levels.Count)
        {
            int nextLevelIndex = currentLevelIndex + 1;
            levels[nextLevelIndex].isUnlocked = true;

            Button nextButton = levels[nextLevelIndex].levelButton;
            if (nextButton != null)
            {
                // Cập nhật màu chữ và trạng thái tương tác
                Text buttonText = nextButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.color = unlockedTextColor;
                }
                nextButton.interactable = true;

                // Tìm object con tên là "Image" và tắt nó nếu button được mở khóa
                Transform childImageTransform = nextButton.transform.Find("Image");
                if (childImageTransform != null)
                {
                    childImageTransform.gameObject.SetActive(false); // Tắt object "Image" khi button được mở khóa
                }
            }

            // Lưu trạng thái mở khóa của level vào PlayerPrefs
            PlayerPrefs.SetInt($"Level_{nextLevelIndex}_Unlocked", 1);
            PlayerPrefs.Save();
        }
    }

}


[System.Serializable]
public class LevelData
{
    public string levelName;
    public string sceneName;
    public Button levelButton;
    public bool isUnlocked = false;
}