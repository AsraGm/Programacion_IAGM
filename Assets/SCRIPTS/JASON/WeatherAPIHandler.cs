using SimpleJSON;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WeatherAPIHandler : MonoBehaviour
{
    [Header("Weather Settings")]
    [SerializeField] WeatherData weatherData;
    [SerializeField] private string latitude = "40.7128";
    [SerializeField] private string longitude = "-74.0060";
    [SerializeField] private string url;

    [Header("Light Settings")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private float lightColorTransitionSpeed = 0.5f;

    [Header("Post Processing URP")]
    [SerializeField] private Volume postProcessVolume; // Cambiado de PostProcessVolume a Volume
    private ColorAdjustments colorAdjustments;
    private Bloom bloom;
    private Vignette vignette;

    private string jsonRaw;
    private Color targetLightColor;
    private Coroutine colorTransitionCoroutine;
    private Coroutine postProcessCoroutine;

    private void OnValidate()
    {
        url = $"https://api.openweathermap.org/data/3.0/onecall?lat={latitude}&lon={longitude}&appid=7fe45acb4f5a69f83c45312aad97613a&units=metric";
    }

    private void Start()
    {
        if (postProcessVolume != null && postProcessVolume.profile.TryGet(out colorAdjustments))
            StartCoroutine(WeatherUpdate());
    }

    IEnumerator WeatherUpdate()
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error fetching weather data: {request.error}");
        }
        else
        {
            jsonRaw = request.downloadHandler.text;
            DecodeJson();
        }
    }

    private void DecodeJson()
    {
        var json = JSON.Parse(jsonRaw);

        weatherData.continent = json["timezone"].Value.Split('/')[0];
        weatherData.city = json["timezone"].Value.Split('/')[1];
        weatherData.actualTemp = json["current"]["temp"];
        weatherData.description = json["current"]["weather"][0]["description"];
        weatherData.windSpeed = json["current"]["wind_speed"];

        Debug.Log($"Weather in {weatherData.city}: {weatherData.actualTemp}°C, {weatherData.description}");

        UpdateDirectionalLight();
    }

    private void UpdateDirectionalLight()
    {
        float bloomIntensity = 5;
        float vignetteIntensity = .1f;

        // Asignación de color basado en temperatura usando switch
        switch (weatherData.actualTemp)
        {
            case float temp when temp < 0f:
                targetLightColor = new Color(0.7137f, 0.9725f, 1f); // Azul frío
                bloomIntensity = 5f;
                vignetteIntensity = 0.2f;
                break;

            case float temp when temp <= 10f:
                targetLightColor = new Color(0.8941f, 0.9294f, 0.9804f); // Azul claro
                bloomIntensity = 7f;
                vignetteIntensity = 0.1f;
                break;

            case float temp when temp <= 25f:
                targetLightColor = new Color(1f, 0.9216f, 0.6275f); // Amarillo cálido
                bloomIntensity = 7f;      // Bloom alto (calor)
                vignetteIntensity = 0.12f;
                break;

            case float temp when temp <= 40f:
                targetLightColor = Color.red;
                bloomIntensity = 15f;
                vignetteIntensity = 0.3f;
                break;
        }
        if (postProcessVolume.profile.TryGet(out colorAdjustments) &&
          postProcessVolume.profile.TryGet(out bloom) &&
          postProcessVolume.profile.TryGet(out vignette))
        {
            // Detén la corrutina anterior si está en progreso
            if (postProcessCoroutine != null)
            {
                StopCoroutine(postProcessCoroutine);
            }

            // Inicia la transición de post-processing
            postProcessCoroutine = StartCoroutine(TransitionPostProcessingEffects(
                bloomIntensity,
                vignetteIntensity
            ));
        }

        // Transición de luz (tu código existente)
        if (colorTransitionCoroutine != null)
        {
            StopCoroutine(colorTransitionCoroutine);
        }
        colorTransitionCoroutine = StartCoroutine(TransitionLightColor());
    }

    IEnumerator TransitionPostProcessingEffects(float targetTemp, float targetBloom, float targetVignette)
    {
        float duration = 2f;
        float elapsed = 0f;

        // Valores iniciales
        float startTemp = colorAdjustments.postExposure.value;
        float startBloom = bloom.intensity.value;
        float startVignette = vignette.intensity.value;
        Color startVignetteColor = vignette.color.value;

        Color targetVignetteColor = Color.Lerp(
            new Color(0f, 0.5f, 1f), // Azul
            new Color(1f, 0.5f, 0f), // Naranja
            Mathf.InverseLerp(-20f, 40f, targetTemp) // Normalizado
        );

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Interpola todos los parámetros
            colorAdjustments.postExposure.value = Mathf.Lerp(startTemp, targetTemp, t);
            bloom.intensity.value = Mathf.Lerp(startBloom, targetBloom, t);
            vignette.intensity.value = Mathf.Lerp(startVignette, targetVignette, t);
            vignette.color.value = Color.Lerp(startVignetteColor, targetVignetteColor, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Asegura valores finales exactos
        colorAdjustments.postExposure.value = targetTemp;
        bloom.intensity.value = targetBloom;
        vignette.intensity.value = targetVignette;
        vignette.color.value = targetVignetteColor;

        if (colorTransitionCoroutine != null)
        {
            StopCoroutine(colorTransitionCoroutine);
        }
        colorTransitionCoroutine = StartCoroutine(TransitionLightColor());
    }

    IEnumerator TransitionLightColor()
    {
        Color startColor = directionalLight.color;
        float transitionProgress = 0f;

        while (transitionProgress < 1f)
        {
            transitionProgress += Time.deltaTime * lightColorTransitionSpeed;
            directionalLight.color = Color.Lerp(startColor, targetLightColor, transitionProgress);
            yield return null;
        }

        directionalLight.color = targetLightColor;
        Debug.Log($"Light color transition completed to {targetLightColor}");
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