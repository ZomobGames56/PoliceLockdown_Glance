using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.AI;
public class AiScript : MonoBehaviour
{
    private bool running;
    private float Health;
    private bool emitting;
    private GameObject agent;
    private AIManager aiManager;
    [SerializeField] private int damage;
    [SerializeField] private int targetAngle;
    [SerializeField] private Transform target; // The target to follow (e.g., player)
    [SerializeField] private CarObject car_SO;
    [SerializeField] private Collider collider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private int maxRotationAngle;
    [SerializeField] private float moveSpeed = 5f; // Speed of forward movement
    [SerializeField] private Renderer objectRenderer;
    [SerializeField] private float driftFactor = 0.5f; // Factor for drift effect
    [SerializeField] private GameObject visual, effect;
    [SerializeField] private float rotationSpeed = 180f; // Degrees per second

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        running = true;
        visual.SetActive(true);
        effect.SetActive(false);
        Health = car_SO.Health;
        collider.enabled = false;
        SetMaterialTransparent(.25f);
        StartCoroutine(Cooldown(1.5f, ActivateCollider));

    }
    void FixedUpdate()
    {
        if (running)
        {
            Vector3 direction = Vector3.zero;
            if (target != null)
            {
                // Calculate direction to target
                direction = target.position - transform.position;
            }
            else
            {
                DestroyCar();
            }
            direction.y = 0; // Assuming we're on a plane, ignore y difference
            if (direction == Vector3.zero) return; // Avoid invalid operations

            // Calculate desired rotation
            Quaternion desiredRotation = Quaternion.LookRotation(direction);

            // Rotate towards target
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, rotationSpeed * Time.fixedDeltaTime);

            // Move forward
            transform.Translate(transform.forward * moveSpeed * Time.fixedDeltaTime, Space.World);

            // Apply drift rotation to car visual
            if (visual != null)
            {
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                float dotProduct = Vector3.Dot(this.transform.right, directionToTarget);
                RotateSmoothly(dotProduct);
            }
        }
    }
    void RotateSmoothly(float input)
    {
        if (input > .1)
            targetAngle = maxRotationAngle;  // Rotate to +15 degrees
        else if (input < -.1)
            targetAngle = -maxRotationAngle; // Rotate to -15 degrees
        else
            targetAngle = 0; // Return to center smoothly
        // Smoothly interpolate local Y-axis rotation
        float newAngle = Mathf.LerpAngle(visual.transform.localEulerAngles.y, targetAngle, Time.deltaTime * rotationSpeed);

        // Convert to local Quaternion
        Quaternion targetRotation = Quaternion.Euler(0, newAngle, 0);

        // Apply rotation locally
        visual.transform.localRotation = Quaternion.Slerp(visual.transform.localRotation, targetRotation, Time.deltaTime * 5f);
    }

    public float GetDamage()
    {
        return damage;
    }
    public CarObject GetCarDetails()
    {
        return car_SO;
    }
    private void DestroyCar()
    {
        running = false;
        visual.SetActive(false);
        effect.SetActive(true);
        //AudioManager.Instance.CheckAudio();
        aiManager.RemoveList(agent);
        StartCoroutine(Cooldown(2f, DestroyAI));
    }
    private void DestroyAI()
    {
        Destroy(this.gameObject);
    }
    public void SetAiManager(AIManager _aiManager)
    {
        aiManager = _aiManager;
    }
    private void UpdateUI()
    {
        healthSlider.value = Health / car_SO.Health;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Health -= damage;
            collider.enabled = false;
            SetMaterialTransparent(.25f);
            StartCoroutine(Cooldown(1.5f, ActivateCollider));
        }
        else if (other.tag == "Cop")
        {
            Health -= other.GetComponent<AiScript>().GetDamage();
            collider.enabled = false;
            SetMaterialTransparent(.25f);
            StartCoroutine(Cooldown(1.5f, ActivateCollider));
        }
        else if (other.tag == "Building")
        {
            Health -= car_SO.Health / 2;
            collider.enabled = false;
            SetMaterialTransparent(.25f);
            StartCoroutine(Cooldown(1.5f, ActivateCollider));
        }
        UpdateUI();
        if (Health <= 0)
        {
            DestroyCar();
        }
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0)
        {
            DestroyCar();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            Health -= damage;
            collider.enabled = false;
            SetMaterialTransparent(.25f);
            StartCoroutine(Cooldown(1.5f, ActivateCollider));
        }
        else if (other.tag == "Cop")
        {
            Health -= other.GetComponent<AiScript>().GetDamage();
            collider.enabled = false;
            SetMaterialTransparent(.25f);
            StartCoroutine(Cooldown(1.5f, ActivateCollider));
        }
        else if (other.tag == "Building")
        {
            Health -= car_SO.Health / 2;
            collider.enabled = false;
            SetMaterialTransparent(.25f);
            StartCoroutine(Cooldown(1.5f, ActivateCollider));
        }

        UpdateUI();
        if (Health <= 0)
        {
            collider.enabled = false;
            DestroyCar();
        }
    }
    private void ActivateCollider()
    {
        collider.enabled = true;
        SetMaterialOpaque();
    }
    IEnumerator Cooldown(float coolDownTimer, Action function)
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
    public void SetAgent(GameObject _agent)
    {
        agent = _agent;
        target = agent.transform;
    }
}
