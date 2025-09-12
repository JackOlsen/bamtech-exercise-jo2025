using System.Net;

namespace StargateAPI.Controllers;

public class BaseResponse
{
    public readonly bool Success = true;
    public readonly string Message = "Successful";
    // TODO: Do we need ResponseCode property in addition to status code on the response?
    public readonly int ResponseCode = (int)HttpStatusCode.OK;
}