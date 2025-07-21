using SimpleJSON;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
public struct LocationData
{
    public string cityName;
    public string latitude;
    public string longitude;
}

public class WeatherAPIHandler : MonoBehaviour
{
    [Header("Weather Settings")]
    [SerializeField] WeatherData weatherData;
    [SerializeField] private LocationData[] locations; 
    [SerializeField] private string latitude = "40.7128";
    [SerializeField] private string longitude = "-74.0060";
    [SerializeField] private string url;
    private string urlTemplate = "https://api.openweathermap.org/data/3.0/onecall?lat={0}&lon={1}&appid=7fe45acb4f5a69f83c45312aad97613a&units=metric";

    [Header("Light Settings")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private float lightColorTransitionSpeed = 0.5f;

    [Header("Post Processing URP")]
    [SerializeField] private Volume postProcessVolume;
    private ColorAdjustments colorAdjustments;
    private Bloom bloom;
    private Vignette vignette;
    private FilmGrain filmGrain;

    private string jsonRaw;
    private Color targetLightColor;
    private Coroutine colorTransitionCoroutine;
    private Coroutine postProcessCoroutine;
    private Coroutine locationUpdateCoroutine;

    private bool visualEffectsAppliedForCurrentLocation = false;
    private string currentLocationCity = "";

    private void OnValidate()
    {
        url = $"https://api.openweathermap.org/data/3.0/onecall?lat={latitude}&lon={longitude}&appid=7fe45acb4f5a69f83c45312aad97613a&units=metric";
    }

    private void Start()
    {
        // Validaciones mejoradas
        if (directionalLight == null)
        {
            return;
        }

        if (postProcessVolume == null)
        {
            return;
        }

        if (locations == null || locations.Length == 0)
        {
            return;
        }
        if (postProcessVolume.profile.TryGet(out colorAdjustments) &&
            postProcessVolume.profile.TryGet(out bloom) &&
            postProcessVolume.profile.TryGet(out vignette) &&
            postProcessVolume.profile.TryGet(out filmGrain))
        {
            // Inicializar con la primera ubicación
            if (locations.Length > 0)
            {
                UpdateLocation(locations[0]);
            }

            locationUpdateCoroutine = StartCoroutine(UpdateLocationPeriodically(10f));
        }
    }

    IEnumerator UpdateLocationPeriodically(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            if (locations.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, locations.Length);
                Debug.Log($"Cambiando a ubicación {randomIndex}: {locations[randomIndex].cityName}");
                UpdateLocation(locations[randomIndex]);
            }
        }
    }

    private void UpdateLocation(LocationData newLocation)
    {
        Debug.Log($"Actualizando ubicación a: {newLocation.cityName} ({newLocation.latitude}, {newLocation.longitude})");

        visualEffectsAppliedForCurrentLocation = false;
        currentLocationCity = newLocation.cityName;

        if (colorTransitionCoroutine != null)
        {
            StopCoroutine(colorTransitionCoroutine);
        }
        if (postProcessCoroutine != null)
        {
            StopCoroutine(postProcessCoroutine);
        }

        url = string.Format(urlTemplate, newLocation.latitude, newLocation.longitude);
        Debug.Log($"Nueva URL: {url}");

        weatherData.city = newLocation.cityName;
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
            Debug.LogError($"Response Code: {request.responseCode}");
            Debug.LogError($"URL utilizada: {url}");
        }
        else
        {
            jsonRaw = request.downloadHandler.text;
            DecodeJson();
        }
    }

    private void DecodeJson()
    {
        try
        {
            var json = JSON.Parse(jsonRaw);

            if (json["cod"] != null && json["cod"] != 200)
            {
                Debug.LogError($"Error de API: {json["message"]}");
                return;
            }

            weatherData.continent = json["timezone"].Value.Split('/')[0];
            weatherData.city = json["timezone"].Value.Split('/')[1];
            weatherData.actualTemp = json["current"]["temp"];
            weatherData.description = json["current"]["weather"][0]["description"];
            weatherData.windSpeed = json["current"]["wind_speed"];

            Debug.Log($"Clima en {weatherData.city}: {weatherData.actualTemp}°C, {weatherData.description}");

            if (!visualEffectsAppliedForCurrentLocation)
            {
                Debug.Log($"Aplicando efectos visuales para {currentLocationCity} - Temperatura: {weatherData.actualTemp}°C");
                UpdateVisualEffects();
                visualEffectsAppliedForCurrentLocation = true;
            }
            else
            {
                Debug.Log($"Efectos visuales ya aplicados para {currentLocationCity} - Manteniendo efectos actuales");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error al parsear JSON: {e.Message}");
            Debug.LogError($"JSON recibido: {jsonRaw}");
        }
    }

    private void UpdateVisualEffects()
    {
        float bloomIntensity = 0.5f;
        float vignetteIntensity = 0.1f;
        float filmGrainIntensity = 0.1f;
        float colorAdjustmentSaturation = 0f;

        switch (weatherData.actualTemp)
        {
            case float temp when temp < 0f:
                targetLightColor = new Color(0.6839622f, 0.9933318f, 1f);
                bloomIntensity = 0.2f;
                vignetteIntensity = 0.2f;
                filmGrainIntensity = 0.05f;
                colorAdjustmentSaturation = -20f;
                break;

            case float temp when temp <= 10f: 
                targetLightColor = new Color(0.8941f, 0.9294f, 0.9804f);
                bloomIntensity = 0.5f;
                vignetteIntensity = 0.1f;
                filmGrainIntensity = 0.1f;
                colorAdjustmentSaturation = -10f;
                break;

            case float temp when temp <= 25f: 
                targetLightColor = new Color(0.5467002f, 0.8207547f, 0.2981043f);
                bloomIntensity = 1f;
                vignetteIntensity = 0.12f;
                filmGrainIntensity = 0.2f;
                colorAdjustmentSaturation = 10f;
                break;

            case float temp when temp <= 40f: 
                targetLightColor = Color.red;
                bloomIntensity = 2f;
                vignetteIntensity = 0.3f;
                filmGrainIntensity = 0.3f;
                colorAdjustmentSaturation = 20f;
                break;

            default: 
                targetLightColor = new Color(1f, 0.3f, 0f);
                bloomIntensity = 3f;
                vignetteIntensity = 0.4f;
                filmGrainIntensity = 0.4f;
                colorAdjustmentSaturation = 30f;
                break;
        }

        if (postProcessCoroutine != null)
        {
            StopCoroutine(postProcessCoroutine);
        }
        postProcessCoroutine = StartCoroutine(TransitionPostProcessingEffects(
            bloomIntensity,
            vignetteIntensity,
            filmGrainIntensity,
            colorAdjustmentSaturation
        ));

        if (colorTransitionCoroutine != null)
        {
            StopCoroutine(colorTransitionCoroutine);
        }
        colorTransitionCoroutine = StartCoroutine(TransitionLightColor());
    }

    IEnumerator TransitionPostProcessingEffects(float targetBloom, float targetVignette,
                                               float targetFilmGrain, float targetSaturation)
    {
        float duration = 2f;
        float elapsed = 0f;

        float startBloom = bloom.intensity.value;
        float startVignette = vignette.intensity.value;
        float startFilmGrain = filmGrain.intensity.value;
        float startSaturation = colorAdjustments.saturation.value;
        Color startVignetteColor = vignette.color.value;

        Color targetVignetteColor = Color.Lerp(
            new Color(0f, 0.5f, 1f),     
            new Color(1f, 0.5f, 0f),      
            Mathf.InverseLerp(-20f, 40f, weatherData.actualTemp)
        );

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            bloom.intensity.value = Mathf.Lerp(startBloom, targetBloom, t);
            vignette.intensity.value = Mathf.Lerp(startVignette, targetVignette, t);
            vignette.color.value = Color.Lerp(startVignetteColor, targetVignetteColor, t);
            filmGrain.intensity.value = Mathf.Lerp(startFilmGrain, targetFilmGrain, t);
            colorAdjustments.saturation.value = Mathf.Lerp(startSaturation, targetSaturation, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        bloom.intensity.value = targetBloom;
        vignette.intensity.value = targetVignette;
        vignette.color.value = targetVignetteColor;
        filmGrain.intensity.value = targetFilmGrain;
        colorAdjustments.saturation.value = targetSaturation;
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
    }

    private void OnDestroy()
    {
        if (locationUpdateCoroutine != null)
        {
            StopCoroutine(locationUpdateCoroutine);
        }
        if (colorTransitionCoroutine != null)
        {
            StopCoroutine(colorTransitionCoroutine);
        }
        if (postProcessCoroutine != null)
        {
            StopCoroutine(postProcessCoroutine);
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