public interface IObserver 
{
    void OnNotify(string eventName, object eventData);
}