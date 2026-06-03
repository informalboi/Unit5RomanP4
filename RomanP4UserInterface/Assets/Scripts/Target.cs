using UnityEngine;

public class Target : MonoBehaviour
{
    private Rigidbody targetRb;
    private GameManager gameManager;
    private float minSpeed = 12;
    private float maxSpeed = 16;
    private float maxTorque = 10;
    private float xRange = 4;
    private float ySpawnPos = -6;
    public int pointValue;
    public ParticleSystem explosionParticle;

    // Tag of the zone that should kill targets / end the game (assign in Inspector)
    [SerializeField] private string killZoneTag = "KillZone";


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetRb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        targetRb.AddForce(RandomForce(), ForceMode.Impulse);
        targetRb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);

        transform.position = RandomSpawnPos();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        if(gameManager.isGameActive)
        {
            Destroy(gameObject);
            Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
            gameManager.UpdateScore(pointValue);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Diagnostic - helps verify what actually collides with the target
        Debug.Log($"[Target] OnTriggerEnter: other.name='{other.gameObject.name}', other.tag='{other.gameObject.tag}', " +
            $"isGameActive={(gameManager != null ? gameManager.isGameActive.ToString() : "null")}");

        // Ensure we have a reference to the GameManager
        if (gameManager == null)
        {
            gameManager = GameObject.Find("Game Manager")?.GetComponent<GameManager>();
            Debug.Log("[Target] gameManager was null; attempted to find it at runtime.");
        }

        // Determine if the collider is the kill zone.
        // Primary check: tag match. Fallback: common naming (e.g. "Kill", "Sensor") to help when the tag isn't set.
        bool isKillZone = other.CompareTag(killZoneTag)
                          || other.gameObject.name.ToLower().Contains("kill")
                          || other.gameObject.name.ToLower().Contains("sensor");

        if (!isKillZone)
        {
            // Not the bottom sensor — ignore (prevents inter-target collisions from ending the game)
            return;
        }

        // Only act if the game is currently active
        if (gameManager == null || !gameManager.isGameActive)
        {
            Debug.Log("[Target] Hit kill zone but game is not active; ignoring.");
            return;
        }

        // Destroy the target and end the game if it's not a "Bad" target
        Destroy(gameObject);
        if(!gameObject.CompareTag("Bad"))
        {
            gameManager.GameOver();
        }
       
        //End the game if the target is not a bad target and it hits the sensor

    }
    
    Vector3 RandomForce()
    {
        return Vector3.up * Random.Range(minSpeed, maxSpeed);
    }
    float RandomTorque()
    {
        return Random.Range(-maxTorque, maxTorque);
    }
    Vector3 RandomSpawnPos()
    {
        return new Vector3(Random.Range(-xRange, xRange), ySpawnPos);
    }
}
