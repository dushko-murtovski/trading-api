using System;

namespace StockWolfTradingAPI.Common
{
    public static class BaseImages
    {
        public static string GetImageBase64(string imageType, byte[] imageData) =>
            !string.IsNullOrEmpty(imageType) && imageData is { Length: > 0 }
            ? $"data:{imageType};base64,{Convert.ToBase64String(imageData, 0, imageData.Length)}"
            : null;
    }
}
