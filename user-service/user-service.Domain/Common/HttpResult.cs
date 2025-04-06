using static user_service.Domain.Enums.Enums;

namespace user_service.Domain.Common
{
    public class HttpResult
    {
        protected StatusCode ResponseStatusCodeType { get; private set; }
        protected StatusMessage ResponseStatusMessageType { get; private set; }
        public HttpResult() { }
        public HttpResult(StatusCode responseStatusCodeType, StatusMessage responseStatusMessageType)
        {
            ResponseStatusCodeType = responseStatusCodeType;
            ResponseStatusMessageType = responseStatusMessageType;
        }
        public string GetResponseStatusMessage()
        {
            return ResponseStatusMessageType.ToString();
        }
        public int GetResponseStatusCode()
        {
            return (int)ResponseStatusCodeType;
        }
    }
}
