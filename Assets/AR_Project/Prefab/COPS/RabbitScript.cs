using UnityEngine;
using UnityEngine.AI;
public class RabbitScript : MonoBehaviour
{
    [SerializeField] private GameObject copCar;
    [SerializeField] private AiScript aiScript;
    public void Awake()
    {
        GameObject go = Instantiate(copCar, this.transform.position + new Vector3(0,1,0), this.transform.rotation);
        aiScript = go.GetComponent<AiScript>();
        aiScript.SetAgent(this.gameObject);
    }
    public AiScript GetAIScript()
    {
        return aiScript;
    }
}
