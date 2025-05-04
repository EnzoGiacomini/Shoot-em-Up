using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{

    public GameObject options_;

    public List<AudioSource> audio_sorces;
    public Slider volume_;
    public Dropdown resolution_;

    // Lista para armazenar os valores iniciais de volume
    private List<float> initialVolumes = new List<float>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Armazenar os volumes iniciais
        foreach (AudioSource audio in audio_sorces)
        {
            initialVolumes.Add(audio.volume);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Abre o menu e pausa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            options_.SetActive(true);
            Time.timeScale = 0;
        }
    }
    //Aplica as confing
    public void ApplyOptions()
    {
        options_.SetActive(false);
        Time.timeScale = 1;

        // Aplicar o volume com base no valor inicial armazenado
        for (int i = 0; i < audio_sorces.Count; i++)
        {
            // Multiplicar o valor inicial do volume pelo valor do slider
            audio_sorces[i].volume = initialVolumes[i] * volume_.value;
        }
    }

    //Altera a resolução
    public void ChangeResolution()
    {
        if (resolution_.value == 0)
        {
            Screen.SetResolution(1920, 1080, true);
        }
        else if (resolution_.value == 1)
        {
            Screen.SetResolution(1280, 720, true);
        }
        else if (resolution_.value == 2)
        {
            Screen.SetResolution(900, 600, true);
        }
    }
}
