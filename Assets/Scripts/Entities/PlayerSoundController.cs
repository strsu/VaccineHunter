
using UnityEngine;

[RequireComponent(typeof(SoundPlayerEX))]
public class PlayerSoundController : MonoBehaviour {

    private Animator animator = null;
    private SoundPlayerEX exSP = null;
    private void Awake() {
        exSP = GetComponent<SoundPlayerEX>();
    }
    private void Start() {
        animator = transform.GetComponentInParent<Animator>();
    }
    private void Update() {
        if (animator.GetBool("WALKING") == true) {
            if (exSP.track != SoundTrackEX.Walk) {
                exSP.Stop();
            }
            if (exSP.IsPlaying() == false) {
                exSP.track      = SoundTrackEX.Walk;
                exSP.startDelay = 0.5f;
                exSP.Play();
            }
        }

        // Hitting
        /*
        if (animator.GetBool("HITTING") == true) {
            if (exSP.track != SoundTrackEX.Hit) {
                exSP.Stop();
                exSP.track      = SoundTrackEX.Hit;
                exSP.startDelay = 0.5f;
                exSP.Play();
            }
        }
        */

        if (animator.GetBool("DEAD") == true) {
            if (exSP.track != SoundTrackEX.Dead) {
                exSP.Stop();
                exSP.track      = SoundTrackEX.Dead;
                exSP.startDelay = 0.1f;
                exSP.Play();
            }
        }
    }
}
