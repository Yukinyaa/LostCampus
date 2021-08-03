public class UIEvent
{
    public System.Action onEvent;

    public void Invoke()
    {
        onEvent?.Invoke();
    }
}
