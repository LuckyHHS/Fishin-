using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using TMPro;

public class FishingHandler : MonoBehaviour
{
    //* This handles the main part of fishing, reeling casting, and finding which fish to catch and their data.
    
    // PUBLICS
    [Header("Fishes")]
    [SerializeField] private Fish[] fishes;

    [Header("Casting")]
    [SerializeField] private Animator sliderAnimator;
    [SerializeField] private RectTransform sliderTransform;
    [SerializeField] private float sliderSpeed = 1.25f;
    [SerializeField] private float sliderWidthMultiplier = 500f;
    [SerializeField] private Gradient colorGradient;
    [SerializeField] private float distanceMultiplier;
    [SerializeField] private float PitchMultiplier;
    [SerializeField] private float PitchAdd;
    public float PerfectSoundRange = 0f;
    [SerializeField] private AudioClip PerfectSound;
    [SerializeField] private AudioClip CastSound;
    [SerializeField] private float castSoundVolume = 0.5f;

    [Header("Reeling")]
    [SerializeField] private Transform fishObject;
    [SerializeField] private RectTransform fishSliderObject;
    [SerializeField] private RectTransform fishSpriteObject;

    [SerializeField] private Animator reelingUIAnimator;
    [SerializeField] private float LowestWaitTime;
    [SerializeField] private float MaxWaitTime;
    [SerializeField] private float MinRandomArea;
    [SerializeField] private float MaxRandomArea;
    [SerializeField] private float MinMoveArea;
    [SerializeField] private float BarGravity = 9.8f;
    [SerializeField] private float BarHoldGravity = 9.8f;
    [SerializeField] private float MinSliderArea;
    [SerializeField] private float MaxSliderArea;
    [SerializeField] private AudioClip FishDing;
    [SerializeField] private AudioClip FishCatch;
    [SerializeField] private AudioClip ClickSound;
    [SerializeField] private AudioSource FishEffects;
    [SerializeField] private AudioSource ReelingSource;
    
    [SerializeField] private TextMeshProUGUI AmountReeledText;
 
    [Header("Settings")]
    
    

    // PRIVATES
    private bool startedHolding = false;
    private bool increasing = true;
    private float currentPower = 0;
    private Fish currentFish; // The current fish we are reeling in.
    public bool reeling = false;
     
    private bool fishMoving = false;
    private AudioSource source;
    public Coroutine waitingForFish;
    public bool playingReelAnimation = false; // The yanking animation that plays. We don't any logic to happen so we have a bool.
    private Vector2 wantedFishPos;
    private bool startFishGame = false;
    private float amountReeled = 0.0f;
    private float barVelocity = 0.0f; // The velocity of the bar.

    void Start()
    {
        // Get the source.
        source = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        // Don't do logic while playing animation.
        if (playingReelAnimation) { return; }

        // Check if we are reeling.
        if (reeling)
        {
            ReelUpdate();
            return;
        }   
     

        // Listen for starting to hold.
        if (!startedHolding && Input.GetMouseButtonDown(0) && ToolHandler.instance.tool != null && ToolHandler.instance.tool.toolType == ToolTypes.Rod)
        {
            sliderAnimator.SetBool("Open", true);
            startedHolding = true;
            increasing = true;
            currentPower = 0;
            source.Play();
            // Reel in fish.
            if (waitingForFish != null)
            {
                StopCoroutine(waitingForFish);
                waitingForFish = null;
            }
        }
       
        // Listen for let go.
        if (startedHolding && Input.GetMouseButtonUp(0))
        {
            sliderAnimator.SetBool("Open", false);
            startedHolding = false;
            source.Stop();

            // Use tool
            if (currentPower >= PerfectSoundRange)
            {
                GlobalClientSounds.instance.PlaySound.Invoke(1f, ToolHandler.instance.tool.toolGameObject.transform.position, 0);
            }
            
         
            ToolHandler.instance.tool.UseRod(currentPower * distanceMultiplier);

            // Reel in fish.
            if (waitingForFish != null)
            {
                StopCoroutine(waitingForFish);
                waitingForFish = null;
            }
            waitingForFish = StartCoroutine(WaitForBait());
        }

        // Listen while holding.
        if (startedHolding && Input.GetMouseButton(0))
        {
            // Update.
            UpdateSlider();
        }
    }

    void ReelUpdate()
    {   
        if (startFishGame == false) return;

        // Update fish position, based on chance.
        if (!fishMoving && Random.Range(0.0f, currentFish.MoveTime) > 0.5f)
        {
            fishMoving = true;
            wantedFishPos = new Vector2(Random.Range(MinRandomArea, MaxRandomArea), 0);

            if (Vector2.Distance(wantedFishPos, fishObject.transform.localPosition) > currentFish.MaxMovePosition)
            {
                wantedFishPos = new Vector2((fishObject.transform.localPosition + new Vector3(Random.Range(0f, 1f) > 0.5 ? currentFish.MaxMovePosition + MinMoveArea : -(currentFish.MaxMovePosition + MinMoveArea), 0, 0)).x, 0);
            }
            
            // Clamp
            if (wantedFishPos.x < MinRandomArea)
            {
                wantedFishPos = new Vector3(MinRandomArea, 0);
            }
            else if (wantedFishPos.x > MaxRandomArea)
            {
                wantedFishPos = new Vector3(MaxRandomArea, 0);
            }
        }

        

        // Convert local positions to world positions
        Vector2 sliderWorldPosition = fishSliderObject.position;
        Vector2 fishWorldPosition = fishSpriteObject.position;

        // Calculate the distance between the centers in world space
        float distance = Vector2.Distance(sliderWorldPosition, fishWorldPosition);

        // Get the half widths
        float sliderHalfWidth = fishSliderObject.rect.height / 2;
   
        // Check if the distance is less 
        if (distance < sliderHalfWidth)
        {
            amountReeled += Time.deltaTime * (ToolHandler.instance.tool as Rod).reelTime * currentFish.IncreaseTime;
        }
        else
        {
            amountReeled -= Time.deltaTime * (ToolHandler.instance.tool as Rod).reelTime * currentFish.DecreaseTime;
        }




        // TODO: Play an animation that shows you pulling back hard on rod and the bobber yanking out. Then spawn in fish if caught, otherwise dont.
        
        // Check amounts.
        if (amountReeled <= 0.0)
        {
            playingReelAnimation = true;
            reeling = false;
            startFishGame = false;
              reelingUIAnimator.SetBool("Open", false);
              ReelingSource.Stop();
                FishEffects.PlayOneShot(FishCatch);
            StartCoroutine(PlayCaughtAnimation());
            FailedReel();
        }
        else if (amountReeled >= 100.0)
        {
            playingReelAnimation = true;
            reeling = false;
            startFishGame = false;
            reelingUIAnimator.SetBool("Open", false);
            ReelingSource.Stop();
            FishEffects.PlayOneShot(FishCatch);
         
            StartCoroutine(PlayCaughtAnimation());
            SucessfullyReeled(currentFish);
        }
        

        // Update text.
        AmountReeledText.text = Mathf.Round((Math.Clamp(amountReeled, 0.0f, 100.0f))) + "%";

        // Bar gravity
        barVelocity += Time.deltaTime * BarGravity;

        // Check if mouse down for bar moving velocity.
        if (Input.GetMouseButton(0))
        {
            barVelocity += Time.deltaTime * BarHoldGravity;
        }

        if (Input.GetMouseButtonDown(0))
        {
            FishEffects.PlayOneShot(ClickSound);
        } else if (Input.GetMouseButtonUp(0))
        {
            FishEffects.PlayOneShot(ClickSound);
        }

        // Calculate
        float newX=fishSliderObject.localPosition.x + barVelocity * Time.deltaTime;

        // Calculates
        if (newX  < MinSliderArea + fishSliderObject.rect.height/2)
        {
            newX = MinSliderArea + fishSliderObject.rect.height/2;
            barVelocity = 0f;
        }
        else if (newX  > MaxSliderArea - fishSliderObject.rect.height/2)
        {
            newX = MaxSliderArea - fishSliderObject.rect.height/2;
            barVelocity = 0f;
        }

       

        // Limits
        fishSliderObject.localPosition = new Vector3(newX, fishSliderObject.localPosition.y);

        

        // Check if fish moving is true
        if (fishMoving)
        {
            // Move fish
            fishObject.localPosition = Vector2.Lerp(fishObject.localPosition, new Vector2(wantedFishPos.x, fishObject.localPosition.y), Time.deltaTime * currentFish.MoveSpeed);

            // Check if arrives
            if (Math.Abs(fishObject.localPosition.x - wantedFishPos.x) < 0.1f)
            {
                // Arrived
                fishMoving = false;
            }
        }
    }

    IEnumerator PlayCaughtAnimation()
    {
        (ToolHandler.instance.tool as Rod).GetComponent<Animator>().SetTrigger("Yank");
        yield return new WaitForSeconds(2);
        playingReelAnimation = false;
    }

    void SucessfullyReeled(Fish caughtFish)
    {
        // Do code to spawn in fish and show things.
        Debug.Log("Caught a " + caughtFish.name);

        
    }

    void FailedReel()
    {
        // Do code to show you failed.
        Debug.Log("Failed to catch!");
        
    }

    Fish GetRandomFish()
    {
        return fishes[Random.Range(0, fishes.Length)];
    }

    IEnumerator WaitForBait()
    {
           Debug.Log("Started wait time");
         yield return new WaitForSeconds(0.5f);
        GlobalClientSounds.instance.PlaySound.Invoke(castSoundVolume, ToolHandler.instance.tool.toolGameObject.transform.position, 1);
     
        yield return new WaitForSeconds(Random.Range(LowestWaitTime, MaxWaitTime));

        // Get random fish
        currentFish = GetRandomFish();
      
        amountReeled = 25f;
        AmountReeledText.text = amountReeled + "%";
        barVelocity = 0;
        fishSliderObject.transform.localPosition = new Vector2(-50.82f, fishSliderObject.transform.localPosition.y);
        reeling = true;
        startFishGame = false;
        FishEffects.PlayOneShot(FishDing);
        // Debug
        Debug.Log("Reeling in fish " + currentFish.Name);

        // Set animator
        reelingUIAnimator.SetBool("Open", true);

        yield return new WaitForSeconds(1.25f);
        startFishGame = true;
        ReelingSource.Play();
        
    }

    void UpdateSlider()
    {
        if (increasing)
        {
            // Increase power.
            currentPower += Time.deltaTime * sliderSpeed;

            // If reach top
            if (currentPower >= 1)
            {
                increasing = false;
            }
        }
        else
        {
            // Decrease power.
            currentPower -= Time.deltaTime * sliderSpeed;

            // If reach top
            if (currentPower <= 0)
            {
                increasing = true;
            }
        }

        // Set slider transform.
        sliderTransform.sizeDelta = new Vector2(sliderTransform.sizeDelta.x, currentPower * sliderWidthMultiplier);
        sliderTransform.gameObject.GetComponent<Image>().color = colorGradient.Evaluate(currentPower);

        // Pitch
        source.pitch = PitchMultiplier * currentPower + PitchAdd;
    }
}
