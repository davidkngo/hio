using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HioCom;
using HioCom.Options;
using System;
using UnityEngine.Networking;

class AuthInterceptor : IInterceptor
{
    public void OnError(Exception error) { }

    public UnityWebRequest OnRequest(
        RequestOptions options,
        Func<RequestOptions, UnityWebRequest> handler
    )
    {
        options.Headers["Authorization"] = "Bearer " + PlayerPrefs.GetString("accessToken");
        return handler.Invoke(options);
    }

    public void OnResponse() { }
}

public class SceneManager : MonoBehaviour
{
    // Start is called before the first frame update

    private Hio hio;

    async void Start()
    {
        hio = new Hio(
            baseOptions: new BaseOptions() { BaseUrl = "https://api.escuelajs.co/api/v1" }
        );
        var data = new Dictionary<string, object>()
        {
            ["email"] = "john@mail.com",
            ["password"] = "changeme"
        };
        var response = await hio.Post("/auth/login", data: data);
        PlayerPrefs.SetString("accessToken", response.Data["access_token"].ToString());

        hio.Interceptors.Add(new AuthInterceptor());

        var getResponse = await hio.Get<List<Dictionary<string, object>>>("/categories");
        Debug.Log(getResponse.StatusCode);
    }

    // Update is called once per frame
    void Update() { }
}
