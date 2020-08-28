namespace GuidesFusion360Server.Models
{
    /// <summary>Model for universal server response.</summary>
    /// <typeparam name="T">Type of data that will be send in the response.</typeparam>
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        
        public bool Success { get; set; } = true;
        
        public string Message { get; set; }
    }
}
