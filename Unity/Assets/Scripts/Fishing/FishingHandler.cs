using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

using UnityEngine.UI;

public class FishingHandler : MonoBehaviour
{
    //* This handles the main part of fishing.
    
    // PUBLICS
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

    // PRIVATES
    private bool startedHolding = false;
    private bool increasing = true;
    private float currentPower = 0;
    private AudioSource source;

    void Start()
    {
        // Get the source.
        source = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        
        // Listen for starting to hold.
        if (!startedHolding && Input.GetMouseButtonDown(0) && ToolHandler.instance.tool != null && ToolHandler.instance.tool.toolType == ToolTypes.Rod)
        {
            sliderAnimator.SetBool("Open", true);
            startedHolding = true;
            increasing = true;
            currentPower = 0;
            source.Play();
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
                GlobalClientSounds.PlaySound.Invoke(1f, ToolHandler.instance.tool.toolGameObject.transform.position, 0);
            }
            GlobalClientSounds.PlaySound.Invoke(castSoundVolume, ToolHandler.instance.tool.toolGameObject.transform.position, 1);
            ToolHandler.instance.tool.UseRod(currentPower * distanceMultiplier);
        }

        // Listen while holding.
        if (startedHolding && Input.GetMouseButton(0))
        {
            // Update.
            UpdateSlider();
        }
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
