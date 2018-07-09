using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour {
	private WaveManager wManager;

	[Header("Enemy Info")]
	public int hitPoints;
    bool dead = false;
	public float moveSpeed;
    public EnumManager.GemType rewardGemType;
    public int rewardGemAmount;
	bool attacking = false;
    bool alternateRoute;
    [HideInInspector]
    Queue<int> hitQueue = new Queue<int>();
    public NavMeshAgent navMeshAgent;
    [HideInInspector]
    public bool hasBombDeath;
	
	[Header("Audio")]
	public AudioClip[] deathSounds;
	public AudioClip[] attackSounds;
	public AudioClip footstepSound;
	private AudioSource[] audioSource;

	[Header("Animation")]
	private Animator animator; 
	public float attackDistance;
	private bool isMoving;
	public bool AtTarget {get; set;}
	
	[Header("Environment")]
	private DoorHealth dH;
	public Transform currentTarget;
    [HideInInspector]
    public Transform fenceTarget;
    [HideInInspector]
    public bool isTargetDummy;


	private int currentAudioSource;
	
    void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        
        //navMeshAgent.stoppingDistance = attackDistance;
        //navMeshAgent.
    }

	// Use this for initialization
	void Start () {
		
        //navMeshAgent;
		isMoving = false;
		wManager = GameObject.FindObjectOfType<WaveManager> ();

		//get 3 different audio sources so they don't overlap all the time
		audioSource = new AudioSource[] {GameObject.Find ("EnemyAudio").GetComponent<AudioSource>(),
					GameObject.Find ("EnemyAudio2").GetComponent<AudioSource>(),
					GameObject.Find ("EnemyAudio3").GetComponent<AudioSource>()};

		//Out of the available targets choose a one randomly as our target
		//GameObject[] Targets = GameObject.FindGameObjectsWithTag ("Target");
		//int RanNum = Random.Range (0, Targets.Length);
		//Target = Targets [RanNum];
		StartCoroutine(WaitToPlay(2f));

        GameStateManager.instance.levelManager.activeEnemies.Add(this);
	}
	
	IEnumerator WaitToPlay(float time){
		yield return new WaitForSeconds (time);
		currentAudioSource = Random.Range(0, audioSource.Length);
		audioSource[currentAudioSource].loop = true;
		audioSource[currentAudioSource].clip = footstepSound;
		audioSource[currentAudioSource].Play ();
	}
	void Update()
	{
		//if he's alive have him walk to target
		if (hitPoints > 0) {
			//rotate the character correctly in the direction of the heading
			//Quaternion newRot = Quaternion.LookRotation (target.transform.position - this.transform.position);
			//transform.rotation = Quaternion.Euler (new Vector3 (0, newRot.eulerAngles.y, 0));

			//if (isMoving == false) StartCoroutine (WalkToTarget());

            if (hitQueue.Count > 0)
            {
                ReceiveDamage(hitQueue.Dequeue());
            }
        }
		/*
		if (dH.GetComponent<Renderer> ().enabled == false) {

		}*/
	}
		
	public void SetTarget(Transform initialTarget){
		fenceTarget = currentTarget = initialTarget;
        navMeshAgent.SetDestination(currentTarget.position + new Vector3(0,3,0));
        dH = initialTarget.gameObject.GetComponent<DoorHealth>();
        StartCoroutine(WalkToTarget());
        
    }
	
	public void UpdateTarget(Transform newTarget, bool isDummy = false)
    {
		currentTarget = newTarget;
        navMeshAgent.SetDestination(currentTarget.position + new Vector3(0,3,0));
        animator.SetBool ("isAttacking", false);
		animator.Play ("move");
		isMoving = true;
		attacking = false;
        isTargetDummy = isDummy;
        StartCoroutine (WalkToTarget());
	}
	
	public void TakeDamage(int DMG)
	{
        hitQueue.Enqueue(DMG);
        
	}

    public void StunsEnemy(float timeStunned)
    {

        //starts coroutine which sets speeds to 0 for the TimeStunned Seconds
        StartCoroutine(StunTimer(timeStunned));
    }

    IEnumerator StunTimer(float stunTime)
    {

        //temp speeds
        float normalNavMeshSpeed = navMeshAgent.speed;
        float normalAnimSpeed = animator.speed;

        //Setting speeds to 0
        navMeshAgent.speed = 0f;
        animator.speed = 0;
        yield return new WaitForSeconds(stunTime);

        //returning the speeds to normal
        navMeshAgent.speed = normalNavMeshSpeed;
        animator.speed = normalAnimSpeed;
    }

    public void  SlowsEnemy(float slowAmount, float slowTimeSpan)
    {
        //starts Coroutine which slows the enemies NavMeshAgentSpeed
        //for the slowTimeSpan in seconds

        StartCoroutine(SlowTimer(slowAmount, slowTimeSpan));

    }

    IEnumerator SlowTimer(float slowAmount, float slowTimeSpan)
    {
        //Temp Speeds
        float normalNavMeshAgentSpeed = moveSpeed;
        float normalAnimSpeed = animator.speed;

        //Adjusting Speeds
        navMeshAgent.speed = moveSpeed - slowAmount;
        animator.speed = animator.speed - slowAmount;
        yield return new WaitForSeconds(slowTimeSpan);

        //Return speeds to normal
        navMeshAgent.speed = normalNavMeshAgentSpeed;
        animator.speed = normalAnimSpeed;
    }
    private void ReceiveDamage(int dmg)
    {
        hitPoints -= dmg;
        if (hitPoints <= 0)
        {
            if (!dead)
            {
                Killed();
                dead = true;
            }
        }
        else animator.Play("wound1");

        //		Anim.SetBool ("isHit", true);
        // if (HitPoints > 0) 

        //Anim.SetBool ("isMoving", false);
        //Anim.SetBool ("isHit", true);

        
    }

	IEnumerator WalkToTarget()
	{
        //yield return new WaitForSeconds(.5f);
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = moveSpeed;
        isMoving = true;
        // Old physics based movement
        /*
		Vector3 StartPos = transform.position;
		Vector3 heading = (target.position - StartPos);
		float distance = heading.magnitude;
        */
        float distance = Vector3.Magnitude(currentTarget.position-transform.position);
        animator.SetBool("isAttacking", false);
        animator.SetBool ("isMoving", true);
		
        //move tbe player at a constant velocity to the target until they are a certain disstance away
		while (distance > attackDistance && hitPoints>0) {
            //Debug.Log("Remaining distance: " + distance);
            //Debug.DrawLine(transform.position, navMeshAgent.destination);
            distance = Vector3.Magnitude(navMeshAgent.destination - transform.position);
            Vector3 lastV3 = transform.position;
            /*
            foreach (Vector3 v3 in navMeshAgent.path.corners)
            {
                Debug.DrawLine(lastV3, v3, Color.blue);
                lastV3 = v3;
            }
            */
            // Old movement
            /*
			//Calculate the current heading, normalized
			heading = (target.transform.position  - this.transform.position).normalized;

			//what is the current distance between the characters
			distance = Mathf.Abs(Vector3.Distance (this.transform.position, target.transform.position));

			//set velocity
			this.GetComponent<Rigidbody> ().velocity = (heading * moveSpeed);
			transform.position = new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(transform.position)-9.3f,transform.position.z);
			*/

            yield return new WaitForFixedUpdate ();
            
            
		}

        //make sure this only happens when the soldier is alive
        if (hitPoints > 0 && !attacking && !GameStateManager.instance.IsLost()) {
			//print("attacking = true");
			attacking = true;
			currentAudioSource = Random.Range(0, audioSource.Length);

			audioSource[currentAudioSource].loop = false;
			audioSource[currentAudioSource].Stop();

			isMoving = false;
            navMeshAgent.isStopped = true;
            navMeshAgent.speed = 0;
            navMeshAgent.velocity = Vector3.zero;
            
			this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			animator.SetBool ("isMoving", false);
			/*
			 if (Time.time > nextActionTime ) {
				nextActionTime = Time.time + period; 
				dH.Health -= 8;
			 }
			print(dH.Health);
			*/

			animator.SetBool ("isAttacking", true);
			animator.Play("attack");
		}


	}

	//do this when the player gets killed
	public void Killed()
	{
        GameStateManager.instance.levelManager.activeEnemies.Remove(this);
        navMeshAgent.speed = 0;
        navMeshAgent.enabled = false;

		currentAudioSource = Random.Range(0, audioSource.Length);
		audioSource[currentAudioSource].loop = false;
		audioSource[currentAudioSource].clip = deathSounds[Random.Range(0, deathSounds.Length)];
		audioSource[currentAudioSource].Play ();

        GameObject GemSpawner = Instantiate(Resources.Load("Misc/GemSpawner"),transform.position, Quaternion.identity) as GameObject;
        GemSpawner.GetComponent<GemSpawner>().SetGemAndStartSpawn(rewardGemType, transform.parent, rewardGemAmount);
        GameStateManager.instance.levelManager.RecieveGems(rewardGemAmount, rewardGemType);

		Collider enemyHitbox = this.GetComponent<Collider>();
		//Destroy(enemyHitbox);
		
        if(!hasBombDeath)
		this.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;		
		
		animator.SetBool ("isAttacking", false);
		animator.SetBool ("isMoving", false);

		animator.SetBool ("isDead", true);
		//Anim.Play("death");

		wManager.EnemyKilled ();
		StartCoroutine (WaitToDestroy());
	}

	//wait to 
	IEnumerator WaitToDestroy()
	{
		yield return new WaitForSeconds (2);
		Destroy (this.gameObject);
	}

	public void DamageGate(int damage) {
        if (!isTargetDummy)
        {
            currentAudioSource = Random.Range(0, audioSource.Length);
            audioSource[currentAudioSource].clip = attackSounds[Random.Range(0, attackSounds.Length)];
            audioSource[currentAudioSource].Play();
            dH.TakeDamageGate(damage);
        }
	}

    public bool GetIsDead()
    {
        return dead;
    }

}
