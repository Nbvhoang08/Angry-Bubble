using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour , IObserver
{
    public int MaxBubbleNum;
    public int CurrentBubbleNum = 0;
    public int Step;
    public bool letCheckWin => Step == 0 && !isWaitingForCheckWin;
    public bool isGameOver = false;
    private Coroutine checkWinCoroutine = null; // Coroutine để quản lý quá trình kiểm tra win
    private bool isWaitingForCheckWin = false; // Để theo dõi trạng thái chờ kiểm tra

    void Awake()
    {
        Subject.RegisterObserver(this);
    }

    void Start()
    {
        MaxBubbleNum = FindObjectsOfType<Bubble>().Length;
        Subject.NotifyObservers("InitStep", Step);
        isGameOver = false;
    }

    void OnDestroy()
    {
        Subject.UnregisterObserver(this);
    }

    public void OnNotify(string eventName, object eventData)
    {
        if (eventName == "CountBubble")
        {
            CurrentBubbleNum++;
        }
        else if (eventName == "reduceStep")
        {
            Step--;
        }
        else if (eventName == "shoot")
        {
            // Nếu đang chờ kiểm tra win và nhận được thông báo "shoot", reset thời gian chờ
            if (checkWinCoroutine != null)
            {
                StopCoroutine(checkWinCoroutine);
            }
            checkWinCoroutine = StartCoroutine(WaitAndCheckWin());
        }
    }

    void Update()
    {
        if (!letCheckWin || isGameOver) return;

        // Nếu điều kiện win thỏa mãn, bắt đầu quá trình chờ kiểm tra win
        if (checkWinCoroutine == null)
        {
            checkWinCoroutine = StartCoroutine(WaitAndCheckWin());
        }
    }

    /// <summary>
    /// Coroutine chờ 3 giây và kiểm tra điều kiện thắng/thua
    /// </summary>
    IEnumerator WaitAndCheckWin()
    {
        isWaitingForCheckWin = true;

        float waitTime = 3f; // Thời gian chờ
        float elapsed = 0f; // Thời gian đã trôi qua

        while (elapsed < waitTime)
        {
            // Mỗi khung hình, tăng thời gian đã trôi qua
            elapsed += Time.deltaTime;
            yield return null;

            // Nếu trong thời gian chờ mà Step > 0 (vẫn còn hoạt động), dừng coroutine
            if (Step > 0)
            {
                isWaitingForCheckWin = false;
                yield break;
            }
        }

        // Sau khi hết thời gian chờ và Step đã về 0, kiểm tra win
        isWaitingForCheckWin = false;
        CheckWinCondition();
    }

    public void CheckWinCondition()
    {
        isGameOver = true;
        StartCoroutine(CheckWin());
    }

    IEnumerator CheckWin()
    {
        yield return new WaitForSeconds(0.5f); // Thời gian ngắn trước khi mở UI
        if (CurrentBubbleNum == MaxBubbleNum)
        {
            UIManager.Instance.OpenUI<Success>();
            LevelManager.Instance.SaveGame();
        }
        else
        {
            UIManager.Instance.OpenUI<Fail>();
        }
    }

    IEnumerator StopGame()
    {
        yield return new WaitForSeconds(0.3f);
        Time.timeScale = 0;
    }
}
