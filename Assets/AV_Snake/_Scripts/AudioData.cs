using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioData : MonoBehaviour
{

    
    
    private const int NUM_BANDS = 8;
    private const int SAMPLES_COUNT = 512;

    [SerializeField] private bool useMicro;
    
    [SerializeField] private float[] freqBands;
    [SerializeField] private float[] samples;
    [SerializeField] private Transform bar;
    [SerializeField] private float amplifier = 10;

    [SerializeField] private AudioMixerGroup micMixerGroup;

    private List<Transform> bars = new List<Transform>();
    private int _currentSample;

    private AudioSource _audioSource;



    private void Awake()
    {
        samples = new float [SAMPLES_COUNT];
        freqBands = new float[NUM_BANDS];
        _audioSource = GetComponent<AudioSource>();

        if (useMicro)
        {
            _audioSource.playOnAwake = false;
            _audioSource.Stop();

            for (int i = 0; i < Microphone.devices.Length; i++)
            {
                Debug.Log(Microphone.devices[i]);
            }

            _audioSource.outputAudioMixerGroup = micMixerGroup;
            _audioSource.clip = Microphone.Start(Microphone.devices[0], true, 1, AudioSettings.outputSampleRate);
            
            while((Microphone.GetPosition(Microphone.devices[0]) > 0f) == false) { }
            _audioSource.Play();


        }
        
    }

    private void Update()
    {
        //AudioListener.GetOutputData(samples, 0);

        _audioSource.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);
        SetFrequencyBands();
    }

    public float GetSignal()
    {
        _currentSample++;
        if (_currentSample >= samples.Length)
            _currentSample = 0;

        return samples[_currentSample] * amplifier;
    }

    public float GetAverage()
    {
        float average = 0;
        foreach (var sample in samples)
        {
            average += sample;
        }

        average /= samples.Length;
        return average * amplifier;
    }

   
    
    
    private void SetFrequencyBands()
    {
        int count = 0;
        for (int i = 0; i < NUM_BANDS; i++)
        {
            float average = 0f;
            int samplesPerBand = (int)Mathf.Pow(2, i) * 2;
           
          //  Debug.Log(samplesPerBand);

            for (int j = 0; j < samplesPerBand; j++)
            {
                average += samples[count];
                count++;
            }

            average /= count;
            freqBands[i] = average * amplifier;

        }
    }

    public float GetBandFrequency(int band)
    {
        band = Mathf.Clamp(band, 0, NUM_BANDS-1);

        float amp = amplifier * Mathf.Pow(2, band) * 2;
        return freqBands[band] * amp;
    }



}
