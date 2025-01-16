using System.Collections.Generic;

public static class Subject 
{
    // Danh sách các Observer đã đăng ký
    private static List<IObserver> observers = new List<IObserver>();
    // Thêm một Observer vào danh sách
    public static void RegisterObserver(IObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
            
        }

    }

    // Gỡ bỏ một Observer khỏi danh sách
    public static void UnregisterObserver(IObserver observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }

    // Thông báo sự kiện đến tất cả Observers
    public static void NotifyObservers(string eventName, object eventData = null)
    {
        // Tạo một bản sao danh sách trước khi duyệt
        var observersCopy = new List<IObserver>(observers);
        foreach (var observer in observersCopy)
        {
            observer.OnNotify(eventName, eventData);
        }
    }
}