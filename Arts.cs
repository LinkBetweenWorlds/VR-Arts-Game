using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arts : MonoBehaviour
{
    //Which Arts are unlocked
    public bool hasSpeedArt = false;
    public bool hasJumpArt = false;

    //Which Art is active
    public bool speedArtActive = false;
    public bool jumpArtActive = false;

    //Arts power stats
    public float speedMultiplier = 2;
    public float jumpMultiplier = 2;

    private bool startedTimer = false;
    public bool artActive = false;
    public bool artCooldown = false;

    // Update is called once per frame
    void Update()
    {
        if(artCooldown == false)
        {
            if (startedTimer == false)
            {
                if (speedArtActive)
                {
                    startedTimer = true;
                    artActive = true;
                    StartCoroutine(SpeedArtTimer());
                }
                else if (jumpArtActive)
                {
                    startedTimer = true;
                    artActive = true;
                    StartCoroutine(JumpArtTimer());
                }
            }
        }
    }

    private void ArtOff()
    {
        artActive = false;
        startedTimer = false;
        artCooldown = true;
        StartCoroutine(ArtCooldownTimer());
    }

    IEnumerator SpeedArtTimer()
    {
        yield return new WaitForSecondsRealtime(10);
        speedArtActive = false;
        ArtOff();
    }

    IEnumerator JumpArtTimer()
    {
        yield return new WaitForSecondsRealtime(10);
        jumpArtActive = false;
        ArtOff();
    }

    IEnumerator ArtCooldownTimer()
    {
        yield return new WaitForSecondsRealtime(30);
        artCooldown = false;
    }
}
