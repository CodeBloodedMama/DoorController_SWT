namespace DoorControl
{
    public interface IDoorControlFactory
    {
        IUserValidation CreateUserValidation();
        IDoor CreateDoor();
        IEntryNotification CreateEntryNotification();
        IAlarm CreateAlarm();
    }


}