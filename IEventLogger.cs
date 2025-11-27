namespace he_dieu_hanh
{
    // Interface này định nghĩa các hành động mà lớp ghi log phải có
    public interface IEventLogger
    {
        void StartHooking();
        void StopHooking();
        // Event để thông báo khi có log mới
        event Action<string, string> OnNewLogEntry; // (type, details)
    }
}