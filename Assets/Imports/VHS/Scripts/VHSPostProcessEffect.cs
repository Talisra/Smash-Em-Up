using UnityEngine;
using UnityEngine.Video;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/GlitchEffect")]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(VideoPlayer))]
public class VHSPostProcessEffect : MonoBehaviour
{
	public Shader shader;
	public VideoClip VHSClip;

	public bool ScanLinesHardEnable; // only works barely from unity's editor

	private float _yScanline;
	private float _xScanline;
	public bool _xScan = false;
	public VideoPlayer _player;
	private Material _material = null;


	void Start()
	{
		_material = new Material(shader);
		_player = GetComponent<VideoPlayer>();
		_player.isLooping = true;
		_player.renderMode = VideoRenderMode.APIOnly;
		_player.audioOutputMode = VideoAudioOutputMode.None;
		_player.clip = VHSClip;
		_player.Play();

	}

	public void SetScan(bool value)
    {
		_xScan = value;
    }

	[ImageEffectOpaque]
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		_material.SetTexture("_VHSTex", _player.texture);
		_yScanline += Time.deltaTime * 0.01f;
		_xScanline -= Time.deltaTime * 0.01f;

		if (_yScanline >= 1)
		{
			_yScanline = Random.value;
		}
		if (_xScanline <= -50000 || Random.value < 0.05)
		{
			_xScanline = Random.value;
		}
		_material.SetFloat("_yScanline", _yScanline);
		if (_xScan)
			_material.SetFloat("_xScanline", _xScanline);
		else if (ScanLinesHardEnable)
			_material.SetFloat("_xScanline", _xScanline);
		Graphics.Blit(source, destination, _material);
	}

	protected void OnEnable()
    {
		_material = new Material(shader);
	}

	protected void OnDisable()
	{
		if (_material)
		{
			DestroyImmediate(_material);
		}
	}
}
