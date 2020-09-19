namespace GuidesFusion360Server.Models
{
    /// <summary>
    /// Model for service response with http code so that controller could choose status code for response.
    /// </summary>
    /// <typeparam name="T">Data that will be send in the response.</typeparam>
    public class ServiceResponse<T> : ControllerResponse<T>
    {
        public int StatusCode { get; set; } = 200;

        public ControllerResponse<T> ToControllerResponse() =>
            new ControllerResponse<T> {Data = Data, Message = Message, MessageRu = MessageRu};
    }
}