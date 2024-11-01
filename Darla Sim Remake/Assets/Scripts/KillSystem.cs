using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KillSystem : MonoBehaviour
{
	[Header("Settings")]
	public bool Dead;
	private bool LerpStudent;
	[Header("Kill Pos")]
	public Transform KillPosFront;
	[Header("Misc")]
    public Yandere Player;
	public Animation PlayerAnim;
	[Space]
    public StudentAI NPC;
	private Animation NPCAnim;
	[Space]
	public AudioSource KnifeSound;
	
    void Start()
    {
        NPCAnim = GetComponent<Animation>();
        PlayerAnim.GetComponent<Animation>();
        SetRagdollState(false);
    }


    void Update()
    {

		if (LerpStudent)
		{
			NPC.transform.position = Vector3.Lerp(NPC.transform.position, KillPosFront.position, Time.deltaTime * 10f);
			NPC.transform.rotation = Quaternion.Slerp(NPC.transform.rotation, KillPosFront.rotation, Time.deltaTime * 10f);
		}
        if(Vector3.Distance(Player.transform.position, NPC.transform.position) < 1f)
        {
            if (Input.GetKeyDown("f") && !Dead)
            {
                StartCoroutine(Kill());
            }
        }
    }

    IEnumerator Kill()
    {
		Dead = true;
		Player.enabled = false;
		NPC.enabled = false;
		NPC.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        yield return new WaitForSeconds(0.5f);
		LerpStudent = true;
        KnifeSound.Play();
        yield return new WaitForSeconds(0.3f);
        //In the "" put the name of the animation
        PlayerAnim.CrossFade("f02_knifeHighSanityA_00");
        NPCAnim.CrossFade("f02_knifeHighSanityB_00");
        yield return new WaitForSeconds(1.5f);
		LerpStudent = false;
        SetRagdollState(true);
        KnifeSound.Stop();
        gameObject.name = "Body";
		Player.enabled = true;
    }

        private void SetRagdollState(bool state)
    {
        NPCAnim.enabled = !state;
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = !state;
        }

        foreach (var col in colliders)
        {
            col.enabled = state;
        }
    }
}