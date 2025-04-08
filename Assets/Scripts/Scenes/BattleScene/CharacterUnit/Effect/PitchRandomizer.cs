using UnityEngine;

public class PitchRandomizer : MonoBehaviour
{
	public float randomPercent = 10;

	void Start()
	{
		var audio = transform.GetComponent<AudioSource>();
		audio.volume = Config.seVolume;
		audio.pitch *= 1 + Random.Range(-randomPercent / 100, randomPercent / 100);
	}
}
