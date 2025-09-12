using System.Net;

namespace StargateAPI.Controllers;

public class BaseResponse
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "Successful";
    // TODO: Do we need ResponseCode property in addition to status code on the response?
    public int ResponseCode { get; set; } = (int)HttpStatusCode.OK;
}