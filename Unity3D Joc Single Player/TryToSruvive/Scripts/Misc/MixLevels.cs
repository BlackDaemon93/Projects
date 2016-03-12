using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

// Clasa necesarã pentru a putea regla volumul aplicaþiei
public class MixLevels : MonoBehaviour {

    public AudioMixer masterMixer;

    void Start() {
        masterMixer.SetFloat("sfxVol", 0);
        masterMixer.SetFloat("musicVol", -15);
    }

    public void SetSfxLvl(float sfxLvl) {
        masterMixer.SetFloat("sfxVol", sfxLvl);
    }

    public void SetMusicLvl(float musicLvl) {
        masterMixer.SetFloat("musicVol", musicLvl);
    }
}
