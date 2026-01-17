using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LoopCut.Domain.Enums.EnumConfig
{ 

    public enum StatusCodeHelper
    {
        [CustomName("Success")] OK = 200,
        [CustomName("Created")] Created = 201,
        [CustomName("No Content")] NoContent = 204,
        [CustomName("Bad Request")] BadRequest = 400,
        [CustomName("Unauthorized")] Unauthorized = 401,
        [CustomName("Forbidden")] Forbidden = 403,
        [CustomName("Not Found")] NotFound = 404,
        [CustomName("Method Not Allowed")] MethodNotAllowed = 405,
        [CustomName("Not Acceptable")] NotAcceptable = 406,
        [CustomName("Request Timeout")] RequestTimeout = 408,
        [CustomName("Conflict")] Conflict = 409,
        [CustomName("Precondition Failed")] PreconditionFailed = 412,
        [CustomName("Unprocessable Entity")] UnprocessableEntity = 422,
        [CustomName("Precondition Required")] PreconditionRequired = 428,
        [CustomName("Too Many Requests")] TooManyRequests = 429,
        [CustomName("Request Header Fields Too Large")] RequestHeaderFieldsTooLarge = 431,
        [CustomName("Unavailable For Legal Reasons")] UnavailableForLegalReasons = 451,
        [CustomName("Client Closed Request")] ClientClosedRequest = 499,
        [CustomName("Internal Server Error")] ServerError = 500,
        [CustomName("Not Implemented")] NotImplemented = 501,
        [CustomName("Bad Gateway")] BadGateway = 502,
        [CustomName("Service Unavailable")] ServiceUnavailable = 503,
        [CustomName("Gateway Timeout")] GatewayTimeout = 504,
        [CustomName("HTTP Version Not Supported")] HttpVersionNotSupported = 505,
        [CustomName("Insufficient Storage")] InsufficientStorage = 507,
        [CustomName("Loop Detected")] LoopDetected = 508,
        [CustomName("Not Extended")] NotExtended = 510,
        [CustomName("Network Authentication Required")] NetworkAuthenticationRequired = 511,
        [CustomName("Network Connect Timeout Error")] NetworkConnectTimeoutError = 599,
    }
}
