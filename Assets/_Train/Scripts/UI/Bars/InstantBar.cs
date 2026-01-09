public class InstantBar : Bar
{
    public void OnValueChanged(int value) => SetValue(HandleValue(value));
}
