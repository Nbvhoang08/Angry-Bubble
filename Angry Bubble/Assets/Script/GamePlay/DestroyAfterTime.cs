using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
      public float lifetime = 5f; // Thời gian tồn tại của viên đạn (mặc định là 5 giây)

    void Start()
    {
        // Hủy object sau khoảng thời gian `lifetime`
        Destroy(gameObject, lifetime);
    }
}
