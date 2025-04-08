using UnityEngine;

namespace EpicToonFX
{

	public class ETFXPitchRandomizer : MonoBehaviour
	{
	
		public float randomPercent = 10;
	
		void Start ()
		{
			var audio = transform.GetComponent<AudioSource>();
			audio.pitch *= 1 + Random.Range(-randomPercent / 100, randomPercent / 100);
		}
	}
}