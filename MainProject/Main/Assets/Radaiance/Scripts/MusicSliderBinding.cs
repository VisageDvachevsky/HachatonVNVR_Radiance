using UnityEngine;
using UnityEngine.UI;

public class MusicSliderBinding : MonoBehaviour
{
    private void Start()
    {
        GameObject musicObject = GameObject.FindGameObjectWithTag("Music");

        if (musicObject == null)
        {
            Debug.LogError("Не удалось найти объект с тегом 'Music'. Убедитесь, что аудио сурс правильно помечен.");
            return;
        }

        AudioSource audioSource = musicObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("Объект с тегом 'Music' не содержит компонента AudioSource.");
            return;
        }

        Slider musicSlider = GetComponentInChildren<Slider>();
        if (musicSlider == null)
        {
            Debug.LogError("Слайдер не найден внутри префаба. Убедитесь, что внутри есть слайдер с компонентом Slider.");
            return;
        }

        musicSlider.value = audioSource.volume;

        musicSlider.onValueChanged.AddListener(value => audioSource.volume = value);
    }
}
