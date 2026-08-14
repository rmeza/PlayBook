// BAD: Rompe SRP e ISP
public interface IOrderProcessor 
{
    void ProcessOrder(Order order);
    void SaveToDatabase(Order order);
    void SendEmailNotification(Order order);
}

// GOOD: Respetando SRP, ISP y DIP
public interface IOrderProcessor 
{
    Task ProcessAsync(Order order);
}

public interface IOrderRepository 
{
    Task SaveAsync(Order order);
}

public interface INotificationService 
{
    Task SendConfirmationAsync(Order order);
}