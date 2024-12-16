using UnityEngine;
using UnityEngine.UI;

public class MusicSliderBinding : MonoBehaviour
{
    private void Start()
    {
        GameObject musicObject = GameObject.FindGameObjectWithTag("Music");

        if (musicObject == null)
        {
            Debug.LogError("�� ������� ����� ������ � ����� 'Music'. ���������, ��� ����� ���� ��������� �������.");
            return;
        }

        AudioSource audioSource = musicObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("������ � ����� 'Music' �� �������� ���������� AudioSource.");
            return;
        }

        Slider musicSlider = GetComponentInChildren<Slider>();
        if (musicSlider == null)
        {
            Debug.LogError("������� �� ������ ������ �������. ���������, ��� ������ ���� ������� � ����������� Slider.");
            return;
        }

        musicSlider.value = audioSource.volume;

        musicSlider.onValueChanged.AddListener(value => audioSource.volume = value);
    }
}
