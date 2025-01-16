using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class BtnShakeEffect : MonoBehaviour
{
    // Start is called before the first frame update
    private Button _button;
    
    void Start()
    {
        // Lấy component Button
        _button = GetComponent<Button>();

        // Kiểm tra xem Button có tồn tại không
        if (_button != null)
        {
            _button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("Button component not found!");
        }
    }

    private void OnButtonClick()
    {
        // Rung nhẹ button
        transform.DOShakePosition(0.3f, strength: new Vector3(10f, 10f, 0f), vibrato: 10, randomness: 90, snapping: false, fadeOut: true)
                 .SetEase(Ease.OutQuad);
    }

    private void OnDestroy()
    {
        // Gỡ bỏ sự kiện khi GameObject bị hủy
        if (_button != null)
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }
        DOTween.Kill(gameObject);
    }
}
