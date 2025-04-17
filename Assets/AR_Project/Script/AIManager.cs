using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class AIManager : MonoBehaviour
{
    private bool gameRunning = true;
    [SerializeField] private float min_Dis = 25, max_Dis = 50;
    private float ai_timer, util_timer;
    [SerializeField] private float ai_coolDownTime = 7.5f, util_coolDownTimer = 10f;
    public GameObject RabbitAI;
    public List<GameObject> utils = new List<GameObject>();
    private int currentAgent;
    private GameObject target;
    private List<NavMeshAgent> agents = new List<NavMeshAgent>();

    // Update is called once per frame
    private void Start()
    {
        ai_timer = ai_coolDownTime;
        util_timer = util_coolDownTimer;
    }
    void Update()
    {
        ai_timer -= Time.deltaTime;
        util_timer -= Time.deltaTime;
        if (agents.Count != 0)
        {
            if (currentAgent >= agents.Count)
            {
                currentAgent = 0;
            }
            if (agents[currentAgent] != null && agents[currentAgent].isOnNavMesh)
                agents[currentAgent].SetDestination(target.transform.position);
            else
                RemoveList(agents[currentAgent].gameObject);
            currentAgent++;
        }
        if (ai_timer <= 0 && agents.Count <= 10)
        {
            ai_timer = ai_coolDownTime;
            SpawnObject(RabbitAI);
        }
        if (util_timer <= 0)
        {
            util_timer = util_coolDownTimer;
            SpawnObject(utils[Random.Range(0, utils.Count)]);
        }
    }
    public void RemoveList(GameObject go)
    {
        //AudioManager.Instance.CheckAudio();
        if (go != null)
        {
            agents.Remove(go.GetComponent<NavMeshAgent>());
            Destroy(go);
        }
        else
        {
            foreach (NavMeshAgent agent in agents)
            {
                if (agent == null)
                {
                    print("agent removed value null");
                    agents.Remove(agent);
                }
            }
        }
        foreach (NavMeshAgent agent in agents)
        {
            agent.GetComponent<RabbitScript>().GetAIScript().GetComponent<AudioSource>().enabled = false;
        }
        for (int i = 0; i < agents.Count && i < 2; i++)
        {
            agents[i].GetComponent<RabbitScript>().GetAIScript().GetComponent<AudioSource>().enabled = true;
            if (agents[i].GetComponent<RabbitScript>().GetAIScript().TryGetComponent<AudioSource>(out AudioSource _source))
            {
                print(agents[i].name + "Audio enabled");
            }
        }
    }
    public void GetTarget(GameObject _target)
    {
        target = _target;
    }
    private void SpawnObject(GameObject _go)
    {
        int angle = Random.Range(-180, 180);
        float angleRadians = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(angleRadians), 0, Mathf.Sin(angleRadians)) * Random.Range(min_Dis, max_Dis);
        if (CheckPosition(target.transform.position + offset))
        {
            GameObject go = Instantiate(_go, target.transform.position + offset, Quaternion.identity);
            if (_go.tag == "Cop")
            {
                agents.Add(go.GetComponent<NavMeshAgent>());
                go.GetComponent<RabbitScript>().GetAIScript().SetAiManager(this);
            }
            foreach (NavMeshAgent agent in agents)
            {
                agent.GetComponent<RabbitScript>().GetAIScript().GetComponent<AudioSource>().enabled = false;
            }
            for (int i = 0; i < agents.Count && i < 2; i++)
            {
                print(agents[i].name + "Audio enabled");
                agents[i].GetComponent<RabbitScript>().GetAIScript().GetComponent<AudioSource>().enabled = true;
            }
            //AudioManager.Instance.CheckAudio();
        }
        else
        {
            SpawnObject(_go);
        }
    }
    private bool CheckPosition(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos + new Vector3(0, 100, 0), Vector3.down, out hit))
        {
            if (hit.collider.tag == "Ground")
            {

                Collider[] hitResults = Physics.OverlapBox(hit.point + new Vector3(0, 1, 0), new Vector3(2, 1, 2));
                foreach (Collider collider in hitResults)
                {
                    if (collider.tag == "Building")
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        return false;
    }
}
