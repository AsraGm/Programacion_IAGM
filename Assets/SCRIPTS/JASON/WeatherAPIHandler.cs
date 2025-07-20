using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using SimpleJSON; // Nos sirve para lleer el formato JSON
using static System.Net.WebRequestMethods;
using System;
using UnityEngine.Networking;

public class WeatherAPIHandler : MonoBehaviour
{
    [SerializeField] WeatherData weatherData;
    [SerializeField] private string latitude = "40.7128"; // Replace with your latitude
    [SerializeField] private string longitude = "-74.0060"; // Replace with your longitude
    [SerializeField] private string url;

    private string jsonRaw;

    private void OnValidate()
    {
        url = $"https://api.openweathermap.org/data/3.0/onecall?lat={latitude}&lon={longitude}&appid=7fe45acb4f5a69f83c45312aad97613a&units=metric";
    }
    private void Start()
    { 
        StartCoroutine(WeatherUpdate());
    }

    IEnumerator WeatherUpdate()
    { 
        UnityWebRequest request = new UnityWebRequest(url);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        { 
            Debug.Log("Weather data received successfully.");
            jsonRaw = request.downloadHandler.text;
            Debug.Log(jsonRaw);
        }
    }
}

[Serializable]
public struct WeatherData
{
    public string continent;
    public string city;
    public float actualTemp;
    public string description;
    public float windSpeed;
}