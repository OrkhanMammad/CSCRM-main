namespace CSCRM.Models.ResponseTypes
{
    public class BaseResponse
    {
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public dynamic? Data { get; set; }
        public bool Success { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
    }
}
