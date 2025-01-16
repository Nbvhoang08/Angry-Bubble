using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Timeline;
public class GamePlay : UICanvas 
{
     [Header("Sound Setting")]
    public Sprite OnVolume;
    public Sprite OffVolume;
    [SerializeField] private Image buttonImage;
    

    [Header("Game Manager")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Text LevelName;
    [SerializeField] private Text Step;
    public int StepNum;
    void OnEnable()
    {
        if(_gameManager == null)
        {
            _gameManager = FindObjectOfType<GameManager>();
        }
    }
    private void Update()
    {
        if(_gameManager == null)
        {
            _gameManager = FindObjectOfType<GameManager>();
        }else
        {
            StepNum = _gameManager.Step;
        }
        UpdateLevelText();
        UpdateButtonImage();
        UpdateStepText();
    }
   
    public void HomeBtn()
    {
        
        Time.timeScale = 1;
        StartCoroutine(LoadHome());
        SoundManager.Instance.PlayClickSound();
    }
    IEnumerator LoadHome()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Home");
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<ChooseLV>();
    }
    public void RetryBtn()
    {
        SoundManager.Instance.PlayClickSound();
        Time.timeScale = 1;
        StartCoroutine(ReLoad());
    }
    IEnumerator ReLoad()
    {
        yield return new WaitForSeconds(0.3f);
        ReloadCurrentScene();
    }
    public void ReloadCurrentScene()
    {
        // Lấy tên của scene hiện tại 
        string currentSceneName = SceneManager.GetActiveScene().name;
        //Tải lại scene hiện tại
        SceneManager.LoadScene(currentSceneName);
      
    }
    private void UpdateLevelText()
    {
        if (LevelName != null)
        {   
            int levelNumber = SceneManager.GetActiveScene().buildIndex;
            LevelName.text = $"Level: {levelNumber-1:D2}"; // Hiển thị với 2 chữ số, ví dụ: 01, 02
        }   
    }
    private void UpdateStepText()
    {
        if (Step != null)
        {   
            Step.text = $"{StepNum:D2}"; // Hiển thị với 2 chữ số, ví dụ: 01, 02
        }   
    }
    
    public void SoundBtn()
    {
        SoundManager.Instance.TurnOn = !SoundManager.Instance.TurnOn;
        UpdateButtonImage();
        SoundManager.Instance.PlayClickSound();

    }

    private void UpdateButtonImage()
    {
        if (SoundManager.Instance.TurnOn)
        {
            buttonImage.sprite = OnVolume;
        }
        else
        {
            buttonImage.sprite = OffVolume;
        }
    }
}
