using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Kruso.Umbraco.Delivery.Extensions
{
    public static class StringExtensions
    {
        public static JToken ToJson(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) 
            { 
                return null; 
            }
            text = text.Trim();
            if ((text.StartsWith("{") && text.EndsWith("}")) || //For object
                (text.StartsWith("[") && text.EndsWith("]"))) //For array
            {
                try
                {
                    return JToken.Parse(text);
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return null;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static bool IsJson(this string json)
        {
            if (string.IsNullOrWhiteSpace(json)) { return false; }
            json = json.Trim();
            if ((json.StartsWith("{") && json.EndsWith("}")) || //For object
                (json.StartsWith("[") && json.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(json);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static string Capitalize(this string text)
        {
            return string.IsNullOrEmpty(text)
                ? text
                : $"{text.Substring(0, 1).ToUpper()}{(text.Length > 1 ? text.Substring(1) : string.Empty)}";
        }

        public static string CamelCase(this string text)
        {
            return string.IsNullOrEmpty(text)
                ? text
                : $"{text.Substring(0, 1).ToLower()}{(text.Length > 1 ? text.Substring(1) : string.Empty)}";
        }

        public static NameValueCollection ToNvc(this string queryString)
        {
            var nvc = new NameValueCollection();
            if (!string.IsNullOrEmpty(queryString))
            {
                foreach (var parm in queryString.TrimStart('?').Split('&'))
                {
                    if (!string.IsNullOrEmpty(parm))
                    {
                        var parts = parm.Split('=');
                        if (parts.Length == 2)
                        {
                            nvc.Add(parts[0], parts[1]);
                        }
                    }

                }
            }

            return nvc;
        }

        public static byte[] ToHash(this string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return null;

            using (HashAlgorithm algorithm = MD5.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string ToHashString(this string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return null;

            StringBuilder sb = new StringBuilder();
            foreach (byte b in ToHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}