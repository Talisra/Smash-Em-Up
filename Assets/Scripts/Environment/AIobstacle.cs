using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIobstacle : Unpassable
{
    private Rigidbody rb;

    // Movement
    private AttachPoint ap;
    private bool move = false;
    private bool isHorizontal = true;
    public Vector3 targetLocation;
    private float moveSpeedY = 15;
    private float moveSpeedX = 30f;
    private bool adjustingY = true;


    // Chain Behaviour
    public AIobstacle parent = null;
    private bool waitForParent = false;
    private float behaviourDelay = 0.3f;
    private float behaviourCounter = 0;

    // Graphics
    public float animationDelaySpeed = 3;
    private Renderer aioRenderer;
    public Material material_orig;
    public Material material_spawn;
    private MaterialPropertyBlock propertyBlock;
    private int cutoffId;
    private float cutoff = 0.3f;
    private bool spawnAnimationOn = false;
    private int cutoffSign;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        aioRenderer = GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();
        cutoffId = Shader.PropertyToID("_Cutoff");
    }

    private void OnEnable()
    {

    }

    public void SpawnIn()
    {
        rb.velocity = Vector3.zero;
        AudioManager.Instance.Play("AIOspawn");
        aioRenderer.material = material_spawn;
        cutoff = 0.3f;
        propertyBlock.SetFloat(cutoffId, cutoff);
        cutoffSign = -1;
        spawnAnimationOn = true;
        adjustingY = true;
    }

    public void SpawnOut()
    {
        AudioManager.Instance.Play("AIOspawn");
        aioRenderer.material = material_spawn;
        cutoff = 0;
        propertyBlock.SetFloat(cutoffId, cutoff);
        cutoffSign = 1;
        spawnAnimationOn = true;
    }

    public void SetAttachPoint(AttachPoint ap, Vector3 position)
    {
        this.ap = ap;
        isHorizontal = ap.isHorizontal ? true : false;
        this.targetLocation = position;
        move = true;
    }

    public void Deploy()
    {
        rb.isKinematic = true;
    }

    public void Undeploy()
    {
        rb.isKinematic = false;
    }

    public void SetParentAIO(AIobstacle aioParent)
    {
        parent = aioParent;
        waitForParent = true;
    }

    private void Update()
    {
        if (spawnAnimationOn)
        {
            aioRenderer.GetPropertyBlock(propertyBlock);
            cutoff += (Time.deltaTime * cutoffSign) / animationDelaySpeed;
            propertyBlock.SetFloat(cutoffId, cutoff);
            aioRenderer.SetPropertyBlock(propertyBlock);
            if (cutoff < 0)
            {
                cutoff = 0;
                spawnAnimationOn = false;
                aioRenderer.material = material_orig;
            }
            else if (cutoff > 0.3f)
            {
                cutoff = 0.3f;
                spawnAnimationOn = false;
                aioRenderer.material = material_orig;
                Undeploy();
                PrefabPooler.Instance.ReturnToPool(gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        if (move && !waitForParent)
        {
            if (Mathf.Abs(transform.position.y - targetLocation.y) > 0.3) // first check the y
            {
                rb.AddForce(new Vector3(0, -Mathf.Sign(transform.position.y - targetLocation.y)) * moveSpeedY * Time.deltaTime, ForceMode.VelocityChange);
            }
            else
            {
                if (adjustingY)
                {
                    transform.position = new Vector3(transform.position.x, targetLocation.y, 0); // adjust y
                    rb.velocity = Vector3.zero;
                    adjustingY = false;
                }
                rb.AddForce(new Vector3(-Mathf.Sign(transform.position.x - targetLocation.x) * moveSpeedX * Time.deltaTime, 0, 0), ForceMode.VelocityChange);
            }
            if (Mathf.Abs(transform.position.y - targetLocation.y) < 0.5 && Mathf.Abs(transform.position.x - targetLocation.x) < 0.5) // at attach point
            {
                transform.position = targetLocation; //adjust location
                Deploy();
                ap.AIOmessage(); // set message that its ready when finished adjusting the y
                move = false;
            }
        }else if (waitForParent && !parent.waitForParent)
        {
            behaviourCounter += Time.deltaTime;
            if (behaviourCounter >= behaviourDelay)
            {
                behaviourCounter = 0;
                waitForParent = false;
            }
        }
    }
}
