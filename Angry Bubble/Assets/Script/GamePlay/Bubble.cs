using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
public class Bubble : MonoBehaviour ,IObserver
{
     public BubbleType bubbleType; // Loại của Bubble
    public GameObject bulletPrefab; // Prefab của viên đạn
    public GameObject destroyEffectPrefab; // Prefab của hiệu ứng khi Bubble bị phá hủy
    public Sprite fourDirSprite; // Sprite mới cho loại FourDir
    public float bulletSpeed = 5f; // Tốc độ của viên đạn
    public int Step = 0;
    public bool CanAction => Step <= 0;

    private bool hasSpawnedEffect = false; // Đảm bảo hiệu ứng chỉ được spawn một lần

    void Awake()
    {
        Subject.RegisterObserver(this);
    }

    void OnDestroy()
    {
        Subject.NotifyObservers("CountBubble");
        DOTween.Kill(gameObject);
        Subject.UnregisterObserver(this);
    }

    public void OnNotify(string eventName, object eventData)
    {
        if (eventName == "InitStep")
        {
            Step = (int)eventData;
        }
        else if (eventName == "reduceStep")
        {
            Step--;
        }
    }

    void OnMouseDown()
    {
        if (CanAction) return;
        Subject.NotifyObservers("reduceStep");
        HandleInteraction();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject); // Xóa viên đạn khi va chạm
            HandleInteraction();
        }
    }
    public GameObject RockEffect;
    private void HandleInteraction()
{
    if (bubbleType == BubbleType.ZeroDir)
    {
        // Chuyển sang loại FourDir và thay đổi hình dạng
        bubbleType = BubbleType.FourDir;
        
        if (fourDirSprite != null)
        {
            // Thay đổi sprite nếu đã được gán
            GetComponent<SpriteRenderer>().sprite = fourDirSprite;
        }

        if (RockEffect != null)
        {
            // Tạo hiệu ứng đá
            Instantiate(RockEffect, transform.position, Quaternion.identity);
        }
    }
    else
    {
        // Thực hiện animation phình ra và sau đó xử lý bắn đạn và phá hủy
        AnimateBubble(() =>
        {
            ShootBullets(); // Bắn đạn ra
            SpawnDestroyEffect(); // Hiệu ứng phá hủy

            // Hủy mọi tween liên quan đến object này trước khi phá hủy
            if (DOTween.IsTweening(gameObject))
            {
                DOTween.Kill(gameObject);
            }

            // Phá hủy object sau khi xử lý xong
            Destroy(gameObject);
        });
    }
}

public float animationDuration = 0.5f; // Thời gian cho mỗi bước phình ra/bé lại
private void AnimateBubble(Action onComplete)
{
    // Kiểm tra object còn tồn tại không trước khi thực hiện tween
    if (transform == null)
    {
        return;
    }

    // Thực hiện hiệu ứng phình ra -> bé lại -> phình ra
    transform.DOScale(1.2f, animationDuration) // Phình ra
        .OnComplete(() =>
        {
            if (transform == null)
            {
                return;
            }

            // Gọi callback khi hiệu ứng hoàn tất
            onComplete?.Invoke();
        });
    }


    private void ShootBullets()
    {
        List<Vector2> directions = GetDirections();
        foreach (Vector2 direction in directions)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * bulletSpeed;
            }
        }
        Subject.NotifyObservers("shoot");
    }

    private void SpawnDestroyEffect()
    {
        // Chỉ spawn hiệu ứng một lần
        if (!hasSpawnedEffect && destroyEffectPrefab != null)
        {
            Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
            hasSpawnedEffect = true;
        }
    }

    private List<Vector2> GetDirections()
    {
        // Lấy danh sách các hướng dựa trên loại Bubble
        List<Vector2> directions = new List<Vector2>();

        switch (bubbleType)
        {
            case BubbleType.FourDir:
                directions.Add(Vector2.up);
                directions.Add(Vector2.down);
                directions.Add(Vector2.left);
                directions.Add(Vector2.right);
                break;
            case BubbleType.Up:
                directions.Add(Vector2.up);
                break;
            case BubbleType.Down:
                directions.Add(Vector2.down);
                break;
            case BubbleType.Right:
                directions.Add(Vector2.right);
                break;
            case BubbleType.Left:
                directions.Add(Vector2.left);
                break;
            case BubbleType.ZeroDir:
                // Không bắn đạn
                break;
        }

        return directions;
    }
}

public enum BubbleType
{
    FourDir,
    Up,
    Down,
    Right,
    Left,
    ZeroDir
}