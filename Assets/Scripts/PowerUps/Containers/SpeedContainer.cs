using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedContainer : ReconstructableObject
{
    public GameObject[] cogs;
    public int[] cogsRotSpeed;
    public GameObject button;
    public GameObject inner;
    public GameObject outer;
    public GameObject reward;

    public GameObject summonPrefab;
    public GameObject shatterPrefab;

    private BoxCollider boxCollider;
    private Rigidbody rb;

    private Renderer bodyRenderer;
    private Renderer innerRenderer;
    private Renderer outerRenderer;
    private MaterialPropertyBlock _bodyPropBlock;
    private MaterialPropertyBlock _innerPropBlock;
    private MaterialPropertyBlock _outerPropBlock;
    private int mainTexId;
    private int mainTexSTId;
    private int cutoffId;
    private int fresnelId;


    private int state = 5; // 0: normal, 1: triggerred, 2: open, 3: break, 4: lock 5: respawn 6: disable.
    private bool isBroken = false;

    private float bodyCutoff = 1;

    private float inner_offset_x = 0;
    private float inner_offset_y = 0;
    private float maxCutoff = 1.0f;
    private float cuttoffSign = 1;
    private float innerCutoff = 0;
    private float outer_offset_x = 0;

    private float fresnel_power;

    private float clickAcc = 0;
    private float velocityBeforeCollision = 0;

    public float maxVelocity;

    protected override void Awake()
    {
        base.Awake();
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        bodyRenderer = GetComponent<Renderer>();
        innerRenderer = inner.GetComponent<Renderer>();
        outerRenderer = outer.GetComponent<Renderer>();
        _bodyPropBlock = new MaterialPropertyBlock();
        _innerPropBlock = new MaterialPropertyBlock();
        _outerPropBlock = new MaterialPropertyBlock();
        mainTexId = Shader.PropertyToID("_MainTex");
        mainTexSTId = Shader.PropertyToID("_MainTex_ST");
        cutoffId = Shader.PropertyToID("_Cutoff");
        fresnelId = Shader.PropertyToID("_FresnelPower");
        fresnel_power = 5;
        GetPropertyBlocks();
        _bodyPropBlock.SetFloat(cutoffId, bodyCutoff);
        _outerPropBlock.SetFloat(fresnelId, fresnel_power);
        SetPropertyBlocks();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // hit by player: activation
        if (collision.contacts[0].thisCollider.gameObject.name == "Button")
        {
            if (state == 0 && collision.gameObject.tag == "Player")
            {
                button.transform.position += 
                    new Vector3(transform.rotation.y == 1? 0.4f : -0.4f, 0, 0);
                Activate(Mathf.Abs(collision.gameObject.GetComponent<Player>().GetDeltaX()*8));
            }

        }
        // Hit the wall: reward check
        if (collision.gameObject.tag == "Unpassable")
        {
            if (state == 1)
            {
                if (Mathf.Abs(velocityBeforeCollision) > maxVelocity)
                {
                    Shatter();
                }
                else
                    Open();
            }

        }
        else
            Physics.IgnoreCollision(collision.collider, boxCollider, true);
    }
    public void Activate(float clickPower)
    {
        AudioManager.Instance.Play("Click");
        rb.isKinematic = false;
        clickAcc = transform.rotation.y == 1 ? clickPower : -clickPower;
        state = 1;
        Invoke("Lock", 2.5f);
    }

    private void Open()
    {
        state = 2;
        AudioManager.Instance.Play("ContainerOpen");
        button.SetActive(false);
        Invoke("SpawnReward", 0.2f);
    }

    private void Lock()
    {
        if (state == 1)
        {
            state = 3;
            AudioManager.Instance.Play("ContainerOpen");
            GameObject effect = Instantiate(
            summonPrefab,
            inner.transform.position,
            inner.transform.rotation) as GameObject;
            Destroy(effect, 2.5f);
            Invoke("RemoveReward", 0.5f);
            Invoke("Reset", 5f);
        }
    }
    
    private void Shatter()
    {
        state = 4;
        AudioManager.Instance.Play("ContainerBreak");
        GameObject effect = Instantiate(
        shatterPrefab,
        inner.transform.position,
        inner.transform.rotation) as GameObject;
        Destroy(effect, 0.5f);
        RemoveReward();
    }

    private void RemoveReward()
    {
        inner.SetActive(false);
        outer.SetActive(false);
        button.SetActive(false);
        reward.SetActive(false);
    }

    protected override void Break()
    {
        base.Break();
        boxCollider.enabled = false;
        button.SetActive(false);
        isBroken = true;
        Invoke("Reset", 5f);
    }

    protected override void Reset()
    {
        base.Reset();
        rb.isKinematic = true;
        isBroken = false;
        boxCollider.enabled = true;
        bodyCutoff = 1;
        innerCutoff = 0;
        fresnel_power = 5;
        GetPropertyBlocks();
        _bodyPropBlock.SetFloat(cutoffId, bodyCutoff);
        _innerPropBlock.SetFloat(cutoffId, innerCutoff);
        _outerPropBlock.SetFloat(fresnelId, fresnel_power);
        SetPropertyBlocks();
        inner.SetActive(true);
        outer.SetActive(true);
        reward.SetActive(true);
        button.SetActive(true);
        state = 6;
    }



    public void SpawnSelf()
    {
        AudioManager.Instance.Play("ContainerSummon");
        state = 5;
        FadeIn();
        _bodyPropBlock.SetFloat(cutoffId, bodyCutoff);
        GameObject effect = Instantiate(
        summonPrefab,
        inner.transform.position,
        inner.transform.rotation) as GameObject;
        Destroy(effect, 2.5f);
    }

    private void SpawnReward()
    {
        reward.SetActive(false);
        HealthUpPool.Instance.Get(reward.transform.position, reward.transform.rotation);
    }



    private void MoveCogs()
    {
        for (int i = 0; i < cogs.Length; i++)
        {
            cogs[i].transform.Rotate(0, cogsRotSpeed[i] * Time.deltaTime, 0, Space.Self);
        }
    }

    private void UpdateShaders()
    {
        inner_offset_x += Time.deltaTime/Random.Range(5,20);
        inner_offset_y += Time.deltaTime/Random.Range(10, 50);
        outer_offset_x += Time.deltaTime / Random.Range(50, 70);
        if (innerCutoff > maxCutoff || innerCutoff < 0)
        {
            cuttoffSign *= -1;
        }
        innerCutoff += Time.deltaTime / 3 * cuttoffSign;
        if (inner_offset_x > 1)
            inner_offset_x = 0;
        if (inner_offset_y > 1)
            inner_offset_y = 0;
        if (outer_offset_x > 1)
            outer_offset_x = 0;
        _innerPropBlock.SetVector(mainTexSTId, new Vector4(1, 1, inner_offset_x, inner_offset_y));
        _innerPropBlock.SetFloat(cutoffId, innerCutoff);
        _outerPropBlock.SetVector(mainTexSTId, new Vector4(1, 1, outer_offset_x, 0));
    }

    private void OpenShaders()
    {
        _bodyPropBlock.SetFloat(cutoffId, bodyCutoff);
        bodyCutoff += Time.deltaTime / 5;
        innerCutoff += Time.deltaTime;
        fresnel_power -= Time.deltaTime * 5;
        if (bodyCutoff > 0.15f)
        {
            if (!isBroken)
            {
                Break();
            }
        }
        _outerPropBlock.SetFloat(fresnelId, fresnel_power);
        _innerPropBlock.SetFloat(cutoffId, innerCutoff);
        if (fresnel_power < 0)
        {
            fresnel_power = 0;
            outer.SetActive(false);
        }
        if (innerCutoff > 1)
        {
            innerCutoff = 0;
            inner.SetActive(false);
        }
    }

    private void GetPropertyBlocks()
    {
        // Get the current value of the material properties in the renderer.
        bodyRenderer.GetPropertyBlock(_bodyPropBlock);
        innerRenderer.GetPropertyBlock(_innerPropBlock);
        outerRenderer.GetPropertyBlock(_outerPropBlock);
    }

    private void SetPropertyBlocks()
    {
        bodyRenderer.SetPropertyBlock(_bodyPropBlock);
        innerRenderer.SetPropertyBlock(_innerPropBlock);
        outerRenderer.SetPropertyBlock(_outerPropBlock);
    }

    // Update is called once per frame
    void Update()
    {
        GetPropertyBlocks();
        switch (state)
        {
            case 0: //normal
                {
                    UpdateShaders();
                    break;
                }
            case 1: // triggerred
                {
                    MoveCogs();
                    break;
                }
            case 2: // open
                {
                    OpenShaders();
                    MoveCogs();
                    break;
                }
            case 3:
                {
                    OpenShaders();
                    break;
                }
            case 4:
                {
                    OpenShaders();
                    break;
                }
            case 5: // respawn
                {
                    bodyCutoff -= Time.deltaTime;
                    _bodyPropBlock.SetFloat(cutoffId, bodyCutoff);
                    if (bodyCutoff < 0)
                        state = 0;
                    break;
                }
        }
        SetPropertyBlocks();
    }

    private void FixedUpdate()
    {
        if (state == 1)
        {
            rb.AddForce(new Vector3(clickAcc, 0, 0), ForceMode.Acceleration);
            velocityBeforeCollision = rb.velocity.x;
        }
    }

    public override void BackToPool()
    {
        SpeedContainerPool.Instance.ReturnToPool(this);
    }
}
