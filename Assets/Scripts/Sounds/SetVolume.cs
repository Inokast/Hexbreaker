using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class SetVolume : MonoBehaviour
{
    private Slider volSlider;

    private BGMController bgmController;
    private SoundFXController sfxController;

    [SerializeField] private bool isBGMSlider;


    private void Start()
    {
        volSlider = GetComponent<Slider>();

        if (isBGMSlider == true) 
        {
            bgmController = FindObjectOfType<BGMController>();
            volSlider.value = bgmController.bgmVolumeLevel;

        }

        else if (isBGMSlider == false)
        {
            sfxController = FindObjectOfType<SoundFXController>();
            volSlider.value = sfxController.sfxVolumeLevel;

        }
    }
    public void OnSliderValueChanged(float sliderValue)
    {
        
        if (isBGMSlider == true)
        {
            bgmController.SetMusicLevel(sliderValue);

        }

        else if (isBGMSlider == false)
        {
            sfxController.SetSoundLevel(sliderValue);
        }
    }

}