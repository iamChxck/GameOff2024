using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LensStaminaBar : MonoBehaviour
{
    [SerializeField] private Slider staminaSlider; 
    [SerializeField] private PlayerDeviceController playerDeviceController; 
    private void Awake()
    {
        staminaSlider = GetComponentInChildren<Slider>();
        playerDeviceController = FindObjectOfType<PlayerDeviceController>();
    }

    private void Start()
    {
        InitializeStaminaValues();
        
    }

    private void Update()
    {
        HandleStamina();
        
    }

    public void InitializeStaminaValues()
    {
        staminaSlider.maxValue = 100f;
        staminaSlider.value = playerDeviceController.lensStamina;
    }

    public void HandleStamina()
    {
        if(PlayerDeviceController.currentlySelectedItem != null && PlayerDeviceController.currentlySelectedItem.name == "PlayerDevice")
        {
            staminaSlider.gameObject.SetActive(true);
        }
        else
            staminaSlider.gameObject.SetActive(false);


        staminaSlider.value = playerDeviceController.lensStamina;
    }
}
