using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using System.Text;
using HioCom.Options;
using HioCom.Extentions;
using UnityEngine;

namespace HioCom
{
    public class Hio
    {
        private readonly BaseOptions _baseOptions;

        public List<IInterceptor> Interceptors = new();

        public Hio() { }

        public Hio(BaseOptions baseOptions)
        {
            _baseOptions = baseOptions;
        }

        public async Task<Response<Dictionary<string, object>>> Get(
            string path,
            [Optional] Dictionary<string, object> queryParameters,
            [Optional] RequestOptions options
        )
        {
            return await Get<Dictionary<string, object>>(path, queryParameters, options);
        }

        public async Task<Response<T>> Get<T>(
            string path,
            [Optional] Dictionary<string, object> queryParameters,
            [Optional] RequestOptions options
        )
        {
            options ??= new RequestOptions();
            var url = path.ToQueryUrl(_baseOptions.BaseUrl ?? options.BaseUrl, queryParameters);

            UnityWebRequest request = UnityWebRequest.Get(url);
            request = ApplyInterceptorsRequest(request, options);
            if (options?.ConnectionTimeout != null)
                request.timeout = options.ConnectionTimeout;

            await WaitFor(request.SendWebRequest());

            if (request != null)
            {
                ApplyInterceptorsError(request);

                string body = request.downloadHandler.text;

                var response = new Response<T>
                {
                    Data = JsonConvert.DeserializeObject<T>(body),
                    StatusCode = request.responseCode,
                    Headers = request.GetResponseHeaders()
                };

                return response;
            }

            return default;
        }

        public async Task<Response<Dictionary<string, object>>> Post(
            string path,
            [Optional] Dictionary<string, object> data,
            [Optional] RequestOptions options
        )
        {
            return await Post<Dictionary<string, object>>(path, data, options);
        }

        public async Task<Response<T>> Post<T>(
            string path,
            [Optional] Dictionary<string, object> data,
            [Optional] RequestOptions options
        )
        {
            data ??= new Dictionary<string, object>();
            options ??= new RequestOptions();

            string content = JsonConvert.SerializeObject(data);
            Debug.Log(path.ToQueryUrl(_baseOptions.BaseUrl ?? options.BaseUrl));
            var request = new UnityWebRequest(
                path.ToQueryUrl(_baseOptions.BaseUrl ?? options.BaseUrl),
                "POST"
            );
            byte[] bodyRaw = Encoding.UTF8.GetBytes(content);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            request = ApplyInterceptorsRequest(request, options);

            await WaitFor(request.SendWebRequest());

            if (request != null)
            {
                ApplyInterceptorsError(request);

                string body = request.downloadHandler.text;

                var response = new Response<T>
                {
                    Data = JsonConvert.DeserializeObject<T>(body),
                    StatusCode = request.responseCode,
                    Headers = request.GetResponseHeaders()
                };

                return response;
            }

            return default;
        }

        public async Task<Response<Dictionary<string, object>>> Patch(
            string path,
            [Optional] Dictionary<string, object> data,
            [Optional] RequestOptions options
        )
        {
            return await Patch<Dictionary<string, object>>(path, data, options);
        }

        public async Task<Response<T>> Patch<T>(
            string path,
            [Optional] Dictionary<string, object> data,
            [Optional] RequestOptions options
        )
        {
            data ??= new Dictionary<string, object>();
            options ??= new RequestOptions();

            string content = JsonConvert.SerializeObject(data);

            var request = new UnityWebRequest(
                path.ToQueryUrl(_baseOptions.BaseUrl ?? options.BaseUrl),
                "PATH"
            );
            byte[] bodyRaw = Encoding.UTF8.GetBytes(content);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            request = ApplyInterceptorsRequest(request, options);

            await WaitFor(request.SendWebRequest());

            if (request != null)
            {
                ApplyInterceptorsError(request);

                string body = request.downloadHandler.text;

                var response = new Response<T>
                {
                    Data = JsonConvert.DeserializeObject<T>(body),
                    StatusCode = request.responseCode,
                    Headers = request.GetResponseHeaders()
                };

                return response;
            }

            return default;
        }

        public async Task<Response<Dictionary<string, object>>> Delete(
            string path,
            [Optional] RequestOptions options
        )
        {
            return await Delete<Dictionary<string, object>>(path, options);
        }

        public async Task<Response<T>> Delete<T>(string path, [Optional] RequestOptions options)
        {
            options ??= new RequestOptions();

            UnityWebRequest request = UnityWebRequest.Delete(path);

            request = ApplyInterceptorsRequest(request, options);

            await WaitFor(request.SendWebRequest());

            if (request != null)
            {
                ApplyInterceptorsError(request);

                string body = request.downloadHandler.text;

                var response = new Response<T>
                {
                    Data = JsonConvert.DeserializeObject<T>(body),
                    StatusCode = request.responseCode,
                    Headers = request.GetResponseHeaders(),
                    ErrorMessage = request.error
                };

                return response;
            }

            return default;
        }

        private UnityWebRequest ApplyInterceptorsRequest(
            UnityWebRequest request,
            RequestOptions options
        )
        {
            foreach (var intercepter in Interceptors)
            {
                request = intercepter.OnRequest(
                    options,
                    (opt) =>
                    {
                        foreach (var header in opt.Headers)
                        {
                            request.SetRequestHeader(header.Key, header.Value);
                        }

                        return request;
                    }
                );
            }
            return request;
        }

        private void ApplyInterceptorsError(UnityWebRequest request)
        {
            Exception error = null;

            if (request.error != null)
            {
                error = new Exception(request.error);
            }

            if (error != null)
            {
                if (Interceptors.Count > 0)
                {
                    foreach (var intercepter in Interceptors)
                    {
                        intercepter.OnError(error);
                    }
                }
            }
        }

        private async Task WaitFor(UnityWebRequestAsyncOperation operation)
        {
            while (!operation.isDone)
            {
                await Task.Yield();
            }
        }
    }
}
