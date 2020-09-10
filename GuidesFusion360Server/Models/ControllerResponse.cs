namespace GuidesFusion360Server.Models
{
    /// <summary>Model for universal controller response.</summary>
    /// <typeparam name="T">Data that will be send in the response.</typeparam>
    public class ControllerResponse<T>
    {
        public T Data { get; set; }

        public string Message { get; set; }
    }
}