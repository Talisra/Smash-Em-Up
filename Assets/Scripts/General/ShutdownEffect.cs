using UnityEngine;
using UnityEngine.Video;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/ShutdownEffect")]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(VideoPlayer))]
public class ShutdownEffect : MonoBehaviour
{
	public Shader shader;
	public VideoClip Animation;
	public VideoPlayer _player;
	public GameObject screen;
	private Material _material = null;

	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
		_material.SetTexture("_VHSTex", _player.texture);
		Graphics.Blit(source, destination, _material);
	}

	public void StartEffect(float videoStartPos)
    {
		screen.gameObject.SetActive(true);
		_player.isLooping = false;
		_player.renderMode = VideoRenderMode.APIOnly;
		_player.audioOutputMode = VideoAudioOutputMode.None;
		_player.clip = Animation;
		_player.Stop();
		_player.time = videoStartPos;
		_player.Play();

		_material = new Material(shader);
		Invoke("PlaySound", 1.5f - videoStartPos);
		Invoke("Reset", 2f);
	}

	private void PlaySound()
	{
		AudioManager.Instance.Play("ScreenShut");
		AudioManager.Instance.muteSound = true;
		AudioManager.Instance.Reset();
	}
	private void Reset()
    {
		enabled = false;
    }

    protected void OnDisable()
	{
		if (_material)
		{
			DestroyImmediate(_material);
			//screen.gameObject.SetActive(false);
		}
	}
}
