using UnityEngine;
using System;
using System.Collections;
public class CarScript : MonoBehaviour
{
    private bool emitting;
    [SerializeField] private GameObject effect, visual;
    private bool sheildActive = false;
    private  float balanceSpeed = 35;
    private float Health, Fuel;
    private GameManager gameManager;
    public GameObject particles;
    [SerializeField] private MeshRenderer objectRenderer;
    [SerializeField] private bool isInteractable;
    [SerializeField] private CarObject carObject_SO;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SkidMarksScript skidMarksScript;
    [SerializeField] private int targetAngle, maxRotationAngle, rotationSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        gameObject.GetComponent<Collider>().enabled = true;
        visual.SetActive(true);
        effect.SetActive(false);
        Health = carObject_SO.Health;
        Fuel = carObject_SO.FuelCapacity;
    }

    // Update is called once per frame
    void Update()
    {
        Fuel -= Time.deltaTime;
        if(isInteractable)
        {
            Move();
            Rotate();
            gameManager.SetSliders(Fuel / carObject_SO.FuelCapacity, Health / carObject_SO.Health);
        }
        if (Fuel/carObject_SO.FuelCapacity <= 0.01 && isInteractable == true)
        {
            HideCar();
        }
    }
    private void Move()
    {
        float moveSpeed = carObject_SO.Speed/10;
        transform.Translate(Vector3.forward * Time.deltaTime* (balanceSpeed+moveSpeed));
    }
    private void Rotate()
    {
        float rotationInput = 0;
        if (Input.touchCount > 0)
        {
            // Get the first touch
            Touch firstTouch = Input.GetTouch(0);

            // Check if the touch just began
            if (firstTouch.phase == TouchPhase.Stationary || firstTouch.phase == TouchPhase.Moved)
            {
                // Get the position of the touch
                Vector2 touchPosition = firstTouch.position;

                // Check if the touch is on the right or left side of the screen
                if (touchPosition.x > Screen.width / 2)
                {
                    rotationInput = 1;
                }
                else
                {
                    rotationInput = -1;
                }
            }
        }
        else if (Input.GetMouseButton(0)) // Detect left mouse button click
        {
            if (Input.mousePosition.x < Screen.width / 2)
                rotationInput = -1; // Clicked on the left side of the screen
            else
                rotationInput = 1;  // Clicked on the right side of the screen
        }
        else
        {
            rotationInput = Input.GetAxis("Horizontal");
        }
        float rotationSpeed = carObject_SO.Handling;
        transform.Rotate(Vector3.up, rotationInput * rotationSpeed * Time.deltaTime * 15);
        RotateSmoothly(rotationInput);
    }
    void RotateSmoothly(float input)
    {
        if (input > .1)
            targetAngle = maxRotationAngle;  // Rotate to +15 degrees
        else if (input < -.1)
            targetAngle = -maxRotationAngle; // Rotate to -15 degrees
        else
            targetAngle = 0; // Return to center smoothly
        if (targetAngle != 0 && emitting == false)
        {
            emitting = true;
            audioSource.Play();
            skidMarksScript.StartTrial();
        }
        else if (emitting == true && targetAngle == 0)
        {
            emitting = false;
            audioSource.Stop();
            skidMarksScript.StopTrial();
        }
        else if(targetAngle == 0 )
        {
            audioSource.Stop();
        }
        // Smoothly interpolate local Y-axis rotation
        float newAngle = Mathf.LerpAngle(visual.transform.localEulerAngles.y, targetAngle, Time.deltaTime * rotationSpeed);

        // Convert to local Quaternion
        Quaternion targetRotation = Quaternion.Euler(0, newAngle, 0);

        // Apply rotation locally
        visual.transform.localRotation = Quaternion.Slerp(visual.transform.localRotation, targetRotation, Time.deltaTime * 25f);
    }
    public void SetInteraction(bool _bool)
    {
        isInteractable = _bool;
    }
    public CarObject GetCarSO()
    {
        return carObject_SO;
    }
    public void SetCarAvailabilty(bool _bool)
    {
        carObject_SO.isAvailable = _bool;
    }
    private void HideCar()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        visual.SetActive(false);
        effect.SetActive(true);
        Instantiate(particles, effect.transform);
        //AudioManager.Instance.CheckAudio();
        isInteractable = false;

        StartCoroutine(Cooldown(2f, gameManager.GameOver));
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Health")
        {
            Health = carObject_SO.Health;
            Destroy(other.gameObject);
        }
        else if(other.tag == "Fuel")
        {
            Fuel += 10;
            Destroy(other.gameObject);
        }else if(other.tag == "Cash")
        {
            int amount = other.gameObject.GetComponent<CashScript>().GetAmount();
            gameManager.CoinPicked(amount);
            Destroy(other.gameObject);
        }
        else if(other.tag == "Building")
        {
            HideCar();
        }
        else if(other.tag == "Cop" && !sheildActive)
        {
            Health -=other.gameObject.GetComponent<AiScript>().GetDamage();
            other.gameObject.GetComponent<AiScript>().TakeDamage((int)carObject_SO.Health/3);
            if (Health/ carObject_SO.Health < 0.1)
            {
                HideCar();
            }
            sheildActive = true;
            SetMaterialTransparent(.25f);
            StartCoroutine(Cooldown(2f , DeactivateShield));
        }
    }
    public void SetGameManager(GameManager _gameManager)
    {
        gameManager = _gameManager;
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Building")
        {
            HideCar();
        }
        else if (other.tag == "Cop" && !sheildActive)
        {
            Health -= other.gameObject.GetComponent<AiScript>().GetDamage();
            if (Health / carObject_SO.Health < 0.1)
            {
                HideCar();
            }
            sheildActive = true;
            SetMaterialTransparent(.25f);
            StartCoroutine(Cooldown(2f , DeactivateShield));
        }
    }
    private void DeactivateShield()
    {
        sheildActive = false;
        SetMaterialOpaque();
    }
    IEnumerator Cooldown(float coolDownTimer , Action function)
    {
        yield return new WaitForSeconds(coolDownTimer);
        function();
    }
    private void SetMaterialTransparent(float transparency)
    {
        if (objectRenderer != null && objectRenderer.material != null)
        {
            // Change the material's surface type to Transparent
            objectRenderer.material.SetFloat("_Surface", 1);  // 1 = Transparent in URP

            // Enable alpha blending
            objectRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            objectRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            objectRenderer.material.SetInt("_ZWrite", 0);  // Disable depth writing for transparency
            objectRenderer.material.renderQueue = 3000;

            // Set the color with the specified transparency
            Color currentColor = objectRenderer.material.color;
            currentColor.a = transparency;  // Adjust alpha
            objectRenderer.material.color = currentColor;
        }
    }
    private void SetMaterialOpaque()
    {
        if (objectRenderer != null && objectRenderer.material != null)
        {
            // Change the material's surface type to Opaque
            objectRenderer.material.SetFloat("_Surface", 0);  // 0 = Opaque in URP

            // Disable alpha blending
            objectRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            objectRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            objectRenderer.material.SetInt("_ZWrite", 1);  // Enable depth writing for opaque
            objectRenderer.material.renderQueue = 2000;

            // Set the color to fully opaque
            Color currentColor = objectRenderer.material.color;
            currentColor.a = 1f;  // Fully opaque
            objectRenderer.material.color = currentColor;
        }
    }
}
