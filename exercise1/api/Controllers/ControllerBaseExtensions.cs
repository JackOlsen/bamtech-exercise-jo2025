using Microsoft.AspNetCore.Mvc;

namespace StargateAPI.Controllers;

public static class ControllerBaseExtensions
{
    public static IActionResult GetResponse(this ControllerBase controllerBase, BaseResponse response) => 
        new ObjectResult(
            value: response)
        {
            StatusCode = response.ResponseCode
        };
}