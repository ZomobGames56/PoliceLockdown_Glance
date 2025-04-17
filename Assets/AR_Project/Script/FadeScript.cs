using UnityEngine;

public class FadeScript : MonoBehaviour
{
    private Renderer renderer;
    private Material originalMat;
    [SerializeField] private Material fadeMat;
    private void Awake()
    {
        renderer = gameObject.GetComponent<Renderer>();
        originalMat = renderer.material;
    }
    public void FadeMaterial()
    {
        renderer.material = fadeMat;
    }
    public void BackToNormal()
    {
        renderer.material = originalMat;
    }
}
