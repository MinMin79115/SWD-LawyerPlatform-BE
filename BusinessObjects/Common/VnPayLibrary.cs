using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace BusinessObjects.Common
{
    public class VnPayLibrary
    {
        private readonly SortedDictionary<string, string> _requestData = new();
        private readonly SortedDictionary<string, string> _responseData = new();

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData[key] = value;
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData[key] = value;
            }
        }

        public string GetResponseData(string key)
        {
            return _responseData.TryGetValue(key, out var retValue) ? retValue : string.Empty;
        }

        public string CreateRequestUrl(string baseUrl, string vnpHashSecret)
        {
            var data = new StringBuilder();
            
            var sortedParams = _requestData
                .Where(kv => !string.IsNullOrEmpty(kv.Value))
                .OrderBy(kv => kv.Key, StringComparer.Ordinal);
            
            foreach (var (key, value) in sortedParams)
            {
                data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }

            var queryString = data.ToString();
            if (queryString.Length > 0)
            {
                queryString = queryString.Remove(queryString.Length - 1, 1);
            }

            var vnpSecureHash = HmacSha512(vnpHashSecret, queryString);
            baseUrl += "?" + queryString + "&vnp_SecureHash=" + vnpSecureHash;

            return baseUrl;
        }

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            var rspRaw = GetResponseData();
            var myChecksum = HmacSha512(secretKey, rspRaw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetResponseData()
        {
            var data = new StringBuilder();
            
            if (_responseData.ContainsKey("vnp_SecureHashType"))
            {
                _responseData.Remove("vnp_SecureHashType");
            }

            if (_responseData.ContainsKey("vnp_SecureHash"))
            {
                _responseData.Remove("vnp_SecureHash");
            }

            var sortedParams = _responseData
                .Where(kv => !string.IsNullOrEmpty(kv.Value))
                .OrderBy(kv => kv.Key, StringComparer.Ordinal);

            foreach (var (key, value) in sortedParams)
            {
                data.Append(key + "=" + value + "&");
            }

            if (data.Length > 0)
            {
                data.Remove(data.Length - 1, 1);
            }

            return data.ToString();
        }

        public static string HmacSha512(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        public static string HmacSha256(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        public static string GetIpAddress(HttpContext httpContext)
        {
            var ipAddress = string.Empty;
            try
            {
                var forwardedHeader = httpContext.Request.Headers["X-Forwarded-For"];
                if (!string.IsNullOrEmpty(forwardedHeader))
                {
                    var addresses = forwardedHeader.ToString().Split(',');
                    if (addresses.Length != 0)
                    {
                        ipAddress = addresses[0].Trim();
                    }
                }

                if (string.IsNullOrEmpty(ipAddress) && httpContext.Connection.RemoteIpAddress != null)
                {
                    var remoteIp = httpContext.Connection.RemoteIpAddress.ToString();
                    if (remoteIp == "::1")
                    {
                        ipAddress = "127.0.0.1";
                    }
                    else
                    {
                        ipAddress = remoteIp;
                    }
                }

                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = "127.0.0.1";
                }
            }
            catch (Exception ex)
            {
                ipAddress = "127.0.0.1";
            }

            return ipAddress;
        }

        public static DateTime ParseVnPayDate(string dateStr)
        {
            return DateTime.ParseExact(dateStr, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }

        public static string FormatVnPayDate(DateTime date)
        {
            return date.ToString("yyyyMMddHHmmss");
        }
    }
}
