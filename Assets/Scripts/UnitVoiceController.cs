using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitVoiceController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public AudioSource PrivateChannel;//commander will always hear the unit
    public AudioSource PublicChannel;//players will only hear the unit if they are looking nearby
    //responses are played in response to commands.  always on private channel
    //only one response is given to any one command, by the "lead" unit
    public AudioClip[] SelectionResponses;
    public AudioClip[] MoveResponses;
    public AudioClip[] AttackResponses;
    public AudioClip[] SupportResponses;
    public AudioClip[] AbilityResponses;
    //reports are played automatically. always on private channel
    //only one report may be playing at the same time
    public AudioClip[] UnderAttackReport;//played when a unit comes under significant dps
    public AudioClip[] LowHealthReport;//played when a unit is at or below 25% health
    public AudioClip[] LowAmmoReport;//played when a unit is low enough on ammo that it can't use its ability
    public AudioClip[] AmmoEmptyReport;//played when a unit is out of ammo
    //quips are played automatically in rtesponse to a variety of situations
    //quips are only played publicly
    #endregion
    #region private fields

    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        if(PrivateChannel == null)
        {
            PrivateChannel = Camera.main.GetComponent<AudioSource>();
        }
    }
    #endregion
    #region public methods
    public void PlaySelectionResponse()
    {
        PlayClip(SelectionResponses, PrivateChannel);
    }
    public void PlayMoveResponse()
    {
        PlayClip(MoveResponses, PrivateChannel);
    }
    public void PlayAttackResponse()
    {
        PlayClip(AttackResponses, PrivateChannel);
    }
    public void PlaySupportResponse()
    {
        PlayClip(SupportResponses, MoveResponses, PrivateChannel);
    }
    public void PlayAbilityResponse()
    {
        PlayClip(AbilityResponses, PrivateChannel);
    }
    public void PlayUnderAttackReport()
    {
        PlayClip(UnderAttackReport, PrivateChannel);
    }
    public void PlayLowHealthReport()
    {
        PlayClip(LowHealthReport, PrivateChannel);
    }
    public void PlayAmmoLowReport()
    {
        PlayClip(LowAmmoReport, PrivateChannel);
    }
    public void PlayAmmoEmptyReport()
    {
        PlayClip(AmmoEmptyReport, PrivateChannel);
    }
    #endregion
    #region private methods
    private void PlayClip(AudioClip[] clips, AudioSource channel)
    {
        if(clips.Length > 0)
        {
            var randClip = clips[Random.Range(0, clips.Length)];
            channel.clip = randClip;
            channel.PlayOneShot(randClip);
        }
        else
        {
            Debug.LogError("Could not play clip, list is empty.");
            
        }
    }
    private void PlayClip(AudioClip[] clips, AudioClip[] altClips, AudioSource channel)
    {
        if(clips.Length > 0)
        {
            PlayClip(clips, channel);
        }
        else
        {
            PlayClip(altClips, channel);
        }
    }
    #endregion
}
