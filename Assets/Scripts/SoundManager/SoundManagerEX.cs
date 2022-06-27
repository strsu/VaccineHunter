
using UnityEngine;

public enum SoundTrackEX {
    Tears_of_Ruins,
    Walk,
    Dead,
    Lost_Memory,
    Hybridization,
    Goled,
    Inspiration,
    enum_max
}

public class SoundManagerEX : MonoBehaviour {

    public int          BGMChannelMax   = 5;
    public int          EFMChannelMax   = 10;
    public AudioClip[]  clips           = null;

    [HideInInspector] public float bgmVolume = 1f;
    [HideInInspector] public float efmVolume = 1f;

    private GameObject[] channelsBGM = null;
    private GameObject[] channelsEFM = null;
    private void Awake() {
        CreateSoundChannel(BGMChannelMax, out channelsBGM);
        CreateSoundChannel(EFMChannelMax, out channelsEFM);
    }
    private void CreateSoundChannel(int maxCount, out GameObject[] GOs) {
        GOs = new GameObject[maxCount];
        for (int i = 0; i < maxCount; i++) {
            var go      = new GameObject();
                go.transform.SetParent(transform);
            var channel = go.AddComponent<AudioSource>();
                go.name = "channel";

                GOs[i]  = go;
        }
    }
    public AudioSource GetSoundChannel(Transform parent, string type) {
        int          count;
        GameObject[] temp;
        
        switch (type) {
            case "BGM":
                count = BGMChannelMax;
                temp  = channelsBGM;
                break;

            case "EFM":
                count = EFMChannelMax;
                temp  = channelsEFM;
                break;

            default:
                Debug.LogError(name + " -- SoundPlayerEX.cs : Please define channel identity");
                return null;
        }
        
        for (int i = 0; i < count; i++) {
            var channel = temp[i].GetComponent<AudioSource>();
            if (channel.isPlaying == true) continue;
            channel.transform.SetParent(parent);
            return channel;
        }

        Debug.LogError(name + " -- SoundPlayerEX.cs : Don't have any empty channels");
        return null;
    }
    public AudioClip GetSoundClip(SoundTrackEX track) {
        return clips[(int)track];
    }
    public void Volume(string strType, float scale) {
        int          count;
        GameObject[] temp;
        switch (strType) {
            case "BGM":
                count       = BGMChannelMax;
                temp        = channelsBGM;
                bgmVolume   = scale;
                break;

            case "EFM":
                count       = EFMChannelMax;
                temp        = channelsEFM;
                efmVolume   = scale;
                break;

            default:
                Debug.LogError("sound type - 'BGM', 'EFM'");
                return;
        }

        for (int i = 0; i < count; i++) {
            var channel         = temp[i].GetComponent<AudioSource>();
                channel.volume  = scale;
        }
    }
    public void Switch(string strType, bool _switch) {
        int          count;
        GameObject[] temp;
        float        scale;
        switch (strType) {
            case "BGM":
                count = BGMChannelMax;
                temp  = channelsBGM;
                scale = bgmVolume;
                break;

            case "EFM":
                count = EFMChannelMax;
                temp  = channelsEFM;
                scale = efmVolume;
                break;

            default:
                Debug.LogError("sound type - 'BGM', 'EFM'");
                return;
        }

        var value = (_switch == true) ? scale : 0f;
        for (int i = 0; i < count; i++) {
            var AS          = temp[i].GetComponent<AudioSource>();
                AS.volume   = value;
        }
    }
    public void ReceiveAudioSource(AudioSource source) {
        source.clip    = null;
        source.transform.SetParent(transform);
    }
}