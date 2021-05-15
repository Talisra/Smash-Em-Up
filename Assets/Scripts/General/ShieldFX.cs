using UnityEngine;

public class ShieldFX : MonoBehaviour
{
    public GameObject core;
    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;
    private int fresnelID;
    private float counter = 0;
    private float duration;
    private const float fullFresnelPower = 1.5f;
    private float currentFresnel;

    private void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        _renderer = core.GetComponent<Renderer>();
        fresnelID = Shader.PropertyToID("_FresnelPower");
    }

    public void SetShield(GameObject parent, float duration)
    {
        AudioManager.Instance.Play("Shield");
        transform.SetParent(parent.transform);
        this.duration = duration;
        currentFresnel = fullFresnelPower;
        counter = 0;
        transform.localScale =
           transform.localScale + parent.transform.localScale * 2.5f;
    }

    private void Update()
    {
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat(fresnelID, currentFresnel);
        _renderer.SetPropertyBlock(_propBlock);
        counter += Time.deltaTime;
        //currentFresnel -= Time.deltaTime;
        if (duration - counter <= 1)
        {
            currentFresnel -= Time.deltaTime*2;
        }
        if (counter >= duration)
        {
            transform.SetParent(null);
            PrefabPooler.Instance.ReturnToPool(gameObject);
        }
    }

    private void OnDisable()
    {
        transform.localScale = new Vector3(1,1,1);
    }

}
