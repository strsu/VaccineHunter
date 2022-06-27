
using UnityEngine;
using System.Collections;

public enum ChannelType { BGM, EFM }
public class SoundPlayerEX : MonoBehaviour {

    [Header("Generals")]
    public SoundTrackEX track       = SoundTrackEX.enum_max;
    public bool         automatic   = true;
    public bool         destroyable = true;
    public bool         onStart     = true;
    public float        startDelay  = 0f;

    private bool isPlaying    = false;
    private bool watingToPlay = false;
    public  bool IsPlaying() { return isPlaying == true || watingToPlay == true; }
    public  void Play()      { StartCoroutine(Playing()); }
    public  void Stop()      { StopAllCoroutines(); channel.Stop(); }

    [Header("Channel Options")]
    [Range(0, 1)] public float blend = 1f;
                  public bool  loop  = false;

    private void Start() {
        // channel = FindObjectOfType<SoundManagerEX>().GetSoundChannel(transform, channelType.ToString());
        channel              = SingleTon.st.exSM.GetSoundChannel(transform, channelType.ToString());
        channel.spatialBlend = blend;
        channel.loop         = loop;

        if (automatic == true) {
            if (onStart == true) {
                channel.Play();
            } else {
                StartCoroutine(Playing());
            }
        }
    }
    private void Update() {
        isPlaying = channel.isPlaying;

        if (startDelay          <= 0f    &&
            channel             != null  &&
            channel.isPlaying   == false &&
            destroyable         == true) {
            ReturnChannelToManager();
        }
        if(SingleTon.st.isMontaDead) {
            track = SoundTrackEX.Inspiration;
        }
    }
    IEnumerator Playing() {
        // channel.clip = FindObjectOfType<SoundManagerEX>().GetSoundClip(track);
            channel.clip = SingleTon.st.exSM.GetSoundClip(track);
        var delay        = startDelay;
            watingToPlay = true;
        while (true) {
            if ((delay -= Time.deltaTime) <= 0f) {
                channel.Play();
                watingToPlay = false;
                break;
            }
            yield return null;
        }
    }
    [Header("Channel Type")]
                      public ChannelType    channelType  = ChannelType.BGM;
    [HideInInspector] public AudioSource    channel      = null;

    private void OnDestroy() {
        ReturnChannelToManager();
    }
    private void ReturnChannelToManager() {
        if (channel == null) return;
        channel.Stop();

        var exSM = SingleTon.st.exSM;
        // var exSM = FindObjectOfType<SoundManagerEX>();
        if (exSM == null) {
            Debug.LogError("Can't Find Sound Manager EX");
            return;
        }

        exSM.ReceiveAudioSource(channel);
        Destroy(gameObject);
    }
    private void OnValidate() {
        if (onStart == true) { startDelay = 0f; automatic = true; }
        if (startDelay < 0f) { startDelay = 0f; }

            if (onStart == true ||
            startDelay   > 0f) {
            automatic = true;
        }

        if (onStart     == false &&
            startDelay  == 0f) {
            automatic = false;
        }
    }
}