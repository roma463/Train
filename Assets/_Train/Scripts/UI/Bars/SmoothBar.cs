using System.Collections;
using UnityEngine;

public class SmoothBar : Bar
{
    private readonly float _renderSpeed = 0.2f;

    private Coroutine _coroutine;

    public void OnValueChanged(int value)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(Render(HandleValue(value)));
    }

    private IEnumerator Render(float value)
    {
        while (!Mathf.Approximately(FillImage.fillAmount, value))
        {
            FillImage.fillAmount = Mathf.MoveTowards(FillImage.fillAmount, value, Time.deltaTime * _renderSpeed);

            yield return null;
        }
    }
}