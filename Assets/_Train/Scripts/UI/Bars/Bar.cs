using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    private readonly float _maxBarValue = 1f;

    [SerializeField] private Image fillImage;

    private float _maxValue;

    protected Image FillImage => fillImage;

    public void Initialize(int maxValue)
    {
        _maxValue = maxValue;
        fillImage.fillAmount = _maxBarValue;
    }

    public void Initialize(float maxValue)
    {
        _maxValue = maxValue;
    }

    public void SetValue(float value) => fillImage.fillAmount = value;

    protected float HandleValue(int value) => Mathf.Clamp(value / _maxValue, 0, _maxBarValue);

    protected float HandleValue(float value) => Mathf.Clamp(value / _maxValue, 0, _maxBarValue);
}
