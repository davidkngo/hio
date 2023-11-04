using System;
using HioCom.Options;
using UnityEngine.Networking;

public interface IInterceptor
{
    void OnError(Exception error);

    UnityWebRequest OnRequest(
        RequestOptions options,
        Func<RequestOptions, UnityWebRequest> handler
    );

    void OnResponse();
}
