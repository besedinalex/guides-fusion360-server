namespace GuidesFusion360Server.Models
{
    /// <summary>Model for universal service response.</summary>
    /// <typeparam name="T">Data that will be send in the response.</typeparam>
    public class ServiceResponseModel<T>
    {
        public T Data { get; set; }

        public bool Success { get; set; } = true;

        public string Message { get; set; }
    }
}