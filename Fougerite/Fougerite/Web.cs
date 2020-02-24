
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Fougerite
{
    using System.Net;
    using System.Text;

    /// <summary>
    /// This class helps plugins to use simple webrequests.
    /// </summary>
    public class Web
    {
        /// <summary>
        /// SSL Protocols.
        /// </summary>
        [Flags]
        public enum MySecurityProtocolType
        {
            //
            // Summary:
            //     Specifies the Secure Socket Layer (SSL) 3.0 security protocol.
            Ssl3 = 48,
            //
            // Summary:
            //     Specifies the Transport Layer Security (TLS) 1.0 security protocol.
            Tls = 192,
            //
            // Summary:
            //     Specifies the Transport Layer Security (TLS) 1.1 security protocol.
            Tls11 = 768,
            //
            // Summary:
            //     Specifies the Transport Layer Security (TLS) 1.2 security protocol.
            Tls12 = 3072
        }

        
        /// <summary>
        /// Does a GET request to the specified URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GET(string url)
        {
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                return client.DownloadString(url);
            }
        }

        /// <summary>
        /// Does a post request to the specified URL with the data.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string POST(string url, string data)
        {
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                byte[] bytes = client.UploadData(url, "POST", Encoding.ASCII.GetBytes(data));
                return Encoding.ASCII.GetString(bytes);
            }
        }

        /// <summary>
        /// Does a GET request to the specified URL, and accepts all SSL certificates.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GETWithSSL(string url)
        {
            System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(MySecurityProtocolType.Tls12 | MySecurityProtocolType.Tls11 | MySecurityProtocolType.Tls);
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                return client.DownloadString(url);
            }
        }
        
        /// <summary>
        /// Does a post request to the specified URL with the data, and accepts all SSL certificates.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string POSTWithSSL(string url, string data)
        {
            System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(MySecurityProtocolType.Tls12 | MySecurityProtocolType.Tls11 | MySecurityProtocolType.Tls);
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                byte[] bytes = client.UploadData(url, "POST", Encoding.ASCII.GetBytes(data));
                return Encoding.ASCII.GetString(bytes);
            }
        }
        
        private bool AcceptAllCertifications(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            return true;
        }
        
        /// <summary>
        /// Creates an Async request for the specified URL, and headers.
        /// The result will be passed to the specified callback's parameter.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        /// <param name="method"></param>
        /// <param name="AdditionalHeaders"></param>
        public void CreateAsyncHTTPRequest(string url, Action<string> callback, string method = "GET", Dictionary<string, string> AdditionalHeaders = null)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.SetRawHeader("Content-Type", "application/json");
            request.Method = method;
            if (AdditionalHeaders != null)
            {
                foreach (var x in AdditionalHeaders.Keys)
                {
                    //request.Headers[x] = AdditionalHeaders[x];
                    request.SetRawHeader(x, AdditionalHeaders[x]);
                }
            }
            
            DoWithResponse(request, (response) =>
            {
                Stream stream = response.GetResponseStream();
                if (stream != null)
                {
                    string body = new StreamReader(stream).ReadToEnd();
                    callback(body);
                }
                else
                {
                    callback("Failed");
                }
            });
        }
        
        /// <summary>
        /// This handles the Async webrequests of the CreateAsyncHTTPRequest method.
        /// You can use this if you are creating your own HttpWebRequest instance.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseAction"></param>
        public void DoWithResponse(HttpWebRequest request, Action<HttpWebResponse> responseAction)
        {
            Action wrapperAction = () =>
            {
                request.BeginGetResponse(new AsyncCallback((iar) =>
                {
                    var response = (HttpWebResponse)((HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
                    responseAction(response);
                }), request);
            };
            wrapperAction.BeginInvoke(new AsyncCallback((iar) =>
            {
                var action = (Action)iar.AsyncState;
                action.EndInvoke(iar);
            }), wrapperAction);
        }
    }
    
    // https://stackoverflow.com/questions/239725/cannot-set-some-http-headers-when-using-system-net-webrequest
    public static class HttpWebRequestExtensions
    {
        static readonly string[] RestrictedHeaders = new string[] {
            "Accept",
            "Connection",
            "Content-Length",
            "Content-Type",
            "Date",
            "Expect",
            "Host",
            "If-Modified-Since",
            "Keep-Alive",
            "Proxy-Connection",
            "Range",
            "Referer",
            "Transfer-Encoding",
            "User-Agent"
        };

        static readonly Dictionary<string, PropertyInfo> HeaderProperties = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

        static HttpWebRequestExtensions()
        {
            Type type = typeof(HttpWebRequest);
            foreach (string header in RestrictedHeaders)
            {
                string propertyName = header.Replace("-", "");
                PropertyInfo headerProperty = type.GetProperty(propertyName);
                HeaderProperties[header] = headerProperty;
            }
        }

        public static void SetRawHeader(this HttpWebRequest request, string name, string value)
        {
            if (HeaderProperties.ContainsKey(name))
            {
                PropertyInfo property = HeaderProperties[name];
                if (property.PropertyType == typeof(DateTime))
                    property.SetValue(request, DateTime.Parse(value), null);
                else if (property.PropertyType == typeof(bool))
                    property.SetValue(request, Boolean.Parse(value), null);
                else if (property.PropertyType == typeof(long))
                    property.SetValue(request, Int64.Parse(value), null);
                else
                    property.SetValue(request, value, null);
            }
            else
            {
                request.Headers[name] = value;
            }
        }
    }
}