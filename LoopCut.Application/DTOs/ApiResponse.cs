using LoopCut.Domain.Enums.EnumConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LoopCut.Application.DTOs
{
    public class ApiResponse<T>
    {
        [JsonPropertyOrder(1)]
        public StatusCodeHelper Code { get; set; }
        [JsonPropertyOrder(2)]
        public string StatusCode { get; set; } = string.Empty;
        [JsonPropertyOrder(3)]
        public string Message { get; set; } = string.Empty;
        [JsonPropertyOrder(4)]
        public T? Data { get; set; }

        public ApiResponse(StatusCodeHelper code, string statusCode, T? data, string? message)
        {
            StatusCode = statusCode;
            Code = code;
            Message = message;
            Data = data;
        }

        public ApiResponse(StatusCodeHelper code, string statusCode, string? message)
        {
            StatusCode = statusCode;
            Code = code;
            Message = message;
        }

        public static ApiResponse<T> OkResponse(T? data, string? mess, string StatusCode)
        {
            return new ApiResponse<T>(StatusCodeHelper.OK, StatusCodeHelper.OK.Names(), data, mess);
        }
        public static ApiResponse<T> OkResponse(string? mess, string StatusCode)
        {
            return new ApiResponse<T>(StatusCodeHelper.OK, StatusCodeHelper.OK.Names(), mess);
        }

        public static ApiResponse<T> CreateResponse(string? mess)
        {
            return new ApiResponse<T>(StatusCodeHelper.Created, StatusCodeHelper.Created.Names(), mess);
        }

        public static ApiResponse<T> UnauthorizedResponse(string mess)
        {
            return new ApiResponse<T>(StatusCodeHelper.Unauthorized, StatusCodeHelper.Unauthorized.Names(), mess);
        }

        public static ApiResponse<T> NotFoundResponse(string mess)
        {
            return new ApiResponse<T>(StatusCodeHelper.NotFound, StatusCodeHelper.NotFound.Names(), mess);
        }

        public static ApiResponse<T> BadRequestResponse(string mess)
        {
            return new ApiResponse<T>(StatusCodeHelper.BadRequest, StatusCodeHelper.BadRequest.Names(), mess);
        }

        public static ApiResponse<T> InternalErrorResponse(string mess)
        {
            return new ApiResponse<T>(StatusCodeHelper.ServerError, StatusCodeHelper.ServerError.Names(), mess);
        }


    }
}
