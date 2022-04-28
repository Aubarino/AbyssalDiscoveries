using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    AudioSource source, source2;
    AudioSource currentPlayingSource;
    bool source1Playing;
    public float desiredVolume = 1f;

    public List<AudioClip> ambientClips = new List<AudioClip>();
    public List<AudioClip> floodedClips = new List<AudioClip>();
    public List<AudioClip> Int1 = new List<AudioClip>();
    public List<AudioClip> Int2 = new List<AudioClip>();
    public List<AudioClip> Int3 = new List<AudioClip>();
    public List<AudioClip> Int4 = new List<AudioClip>();
    List<AirAreaManager> airAreas = new List<AirAreaManager>();
    List<float> prevAirWaterAmount;

    public float intensity; //from 0 to 4. determines the music
    public float updateTime;
    private void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source2 = gameObject.AddComponent<AudioSource>();
        source.volume = desiredVolume;
        source2.volume = 0f;
        source.dopplerLevel = 0f;
        source2.dopplerLevel = 0f;
        source.spatialBlend = 0f;
        source2.spatialBlend = 0f;
        currentPlayingSource = source;
        currentPlayingSource.clip = ambientClips[0];
        source2.clip = ambientClips[0];
        source.Play();
        source2.Play();
        airAreas.AddRange(FindObjectsOfType<AirAreaManager>());
        prevAirWaterAmount = new List<float>(new float[airAreas.Count]);
    }
    private void Update()
    {
        if(source1Playing)
        {
            source.volume = Mathf.Lerp(source.volume, desiredVolume, Time.deltaTime);
            source2.volume = Mathf.Lerp(source2.volume, 0f, Time.deltaTime);
        }
        else
        {
            source2.volume = Mathf.Lerp(source2.volume, desiredVolume, Time.deltaTime);
            source.volume = Mathf.Lerp(source.volume, 0f, Time.deltaTime);
        }
        updateTime -= Time.deltaTime;
        if(updateTime < 0f) //every 4 seconds we check what music should be playing.
        {
            intensity -= 0.02f;
            intensity = Mathf.Clamp(intensity, 0f, 4f);
            updateTime = 4f;
            float overallFillAmount = 0f; //big variable from 0 to 1, determines how much % of every single air node is filled.
            bool isFlooding = false;
            int i = 0;
            foreach(AirAreaManager area in airAreas)
            {
                overallFillAmount += area.WatLevelCore;
                if (area.WaterVolume - prevAirWaterAmount[i] > 20f) isFlooding = true;
                //print(area.WaterVolume - prevAirWaterAmount[i]);
                prevAirWaterAmount[i] = area.WaterVolume;
                i++;
            }
            overallFillAmount /= airAreas.Count;
            if(intensity < overallFillAmount * 6f)
            {
                intensity = overallFillAmount * 6f;
            }
            //do the playing part
            if(isFlooding && intensity < 3f)
            {
                SwitchMusic(floodedClips[Random.Range(0, floodedClips.Count)]);
            }
            else if(intensity <= 0f)
            {
                SwitchMusic(ambientClips[Random.Range(0,ambientClips.Count)]);
            }
            else if(intensity <= 1f)
            {
                SwitchMusic(Int1[Random.Range(0,Int1.Count)]);
            }
            else if(intensity <= 2f)
            {
                SwitchMusic(Int2[Random.Range(0,Int2.Count)]);
            }
            else if(intensity <= 3f)
            {
                SwitchMusic(Int3[Random.Range(0,Int3.Count)]);
            }
            else
            {
                SwitchMusic(Int4[Random.Range(0, Int4.Count)]);
            }
        }
    }
    public void SwitchMusic(AudioClip clip)
    {
        if(currentPlayingSource.clip != clip)
        {
            source1Playing = !source1Playing;
            if (!source1Playing)
            {
                currentPlayingSource = source2;
            }
            else
            {
                currentPlayingSource = source;
            }
            currentPlayingSource.clip = clip;
            currentPlayingSource.Play();
        }
    }
}
