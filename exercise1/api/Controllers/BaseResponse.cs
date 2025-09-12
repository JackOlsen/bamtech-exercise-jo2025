using System.Net;

namespace StargateAPI.Controllers;

public class BaseResponse
{
    public readonly bool Success = true;
    public readonly string Message = "Successful";
    public readonly int ResponseCode = (int)HttpStatusCode.OK;
}