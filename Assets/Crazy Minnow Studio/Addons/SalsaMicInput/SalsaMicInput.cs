using UnityEngine;
using System.Collections;

namespace CrazyMinnow.SALSA
{
	[AddComponentMenu("Crazy Minnow Studio/SALSA LipSync/Add-ons/SalsaMicInput")]
	public class SalsaMicInput : MonoBehaviour
	{
		// RELEASE NOTES & TODO ITEMS:
		//		2.3.0-beta:
		//			~ requires SALSA LipSync Suite v2.3.0+
		//			+ SalsaMicPointerSync added to sync the mic record head position with SALSA.
		//			NOTE: SALSA no longer grabs the record head position on its own,
		//				SalsaMicPointerSync (or similar) is required to ensure the record head
		//				position is updated for SALSA data compute functions.
		//		2.0.0-beta:
		//			+ initial release for SALSA LipSync Suite.
		// ========================================================================================
		// PURPOSE: This script provides simple real-time Microphone input to the
		//		Salsa component. It links up the *default* microphone as an
		//		AudioSource for SALSA. You *must* have at least one microphone
		//		attached, enabled, and working. For the latest information
		//		visit crazyminnowstudio.com.
		// ========================================================================================
		// LOCATION OF FILES:
		//		Assets\Crazy Minnow Studio\Addons\SalsaMicInput
		//		Assets\Crazy Minnow Studio\Examples\Scenes      (if applicable)
		//		Assets\Crazy Minnow Studio\Examples\Scripts     (if applicable)
		// ========================================================================================
		// INSTRUCTIONS:
		//		(visit https://crazyminnowstudio.com/docs/salsa-lip-sync/ for the latest info)
		//		To extend/modify these files, copy their contents to a new set of files and
		//		use a different namespace to ensure there are no scoping conflicts if/when this
		//		add-on is updated.
		// ========================================================================================
		// SUPPORT: Contact assetsupport@crazyminnow.com. Provide:
		//		1) your purchase email and invoice number
		//		2) version numbers (OS, Unity, SALSA, etc.)
		//		3) full details surrounding the problem you are experiencing.
		//		4) relevant information for what you have tried/implemented.
		//		NOTE: Support is only provided for Crazy Minnow Studio products with valid
		//			proof of purchase.
		// ========================================================================================
		// 	KNOWN ISSUES: none.
		// ==========================================================================
		// DISCLAIMER: While every attempt has been made to ensure the safe content
		//		and operation of these files, they are provided as-is, without
		//		warranty or guarantee of any kind. By downloading and using these
		//		files you are accepting any and all risks associated and release
		//		Crazy Minnow Studio, LLC of any and all liability.
		// ==========================================================================

		public int DEFAULT_RATE = 11025;
		public int sampleRate = 0;
		public bool overrideSampleRate = false;
		public bool isAutoStart = true;
		public bool isDebug = false;
		public bool isMuted = false;
		public string selectedMic = default(string);
		public bool linkWithSalsa = false;
		public AudioSource audioSrc;

		private bool isWiredUp = false;
		private bool isMicAvailable = false;
		private float coroutineLoggingIfDebug = 5f; // every 5 seconds

		void Start()
		{
			// Wiring up is now done in a coroutine to wait for the availability of an AudioSource. This
			//		modification is beneficial when integrating with runtime-created AudioSources such as
			//		the case with the SALSA UMA2 workflow.
			StartCoroutine(Wireup());

		} // end Start()



		// Check Microphone.devices list. If at least one device exists,
		//		there will be a default device and we'll use it.
		bool CheckForAvailableMicrophone(string mic)
		{
			// if mic is not specified, check for availability of any mic (Microphone.devices.Length > 0)
			if ( string.IsNullOrEmpty(mic) )
			{
				if ( isDebug )
					Debug.Log("[CheckForAvailableMicrophone()]: INFO: Microphone.device not specified -- using default.");

				if ( Microphone.devices.Length > 0 )
				{
					if ( isDebug )
						Debug.Log("[CheckForAvailableMicrophone()]: INFO: A default Microphone.device is available.");

					return true;

				}

				if ( isDebug )
					Debug.LogWarning("[CheckForAvailableMicrophone()] WARNING: no microphone devices listed.");

				return false;
			}

			for ( int i = 0; i < Microphone.devices.Length; i++ )
			{
				if ( Microphone.devices[i] == mic )
				{
					if ( isDebug )
						Debug.Log("[CheckForAvailableMicrophone()] INFO: Microphone: " + mic + " IS available.");

					return true;
				}
			}

			if ( isDebug )
				Debug.LogWarning("[CheckForAvailableMicrophone()] WARNING: Microphone: " + mic + " is NOT available.");

			return false;
		} // end CheckForAvailableMicrophone()



		// Check the available recording frequencies reported by the Microphone.
		//	- NOTE: a return of 0 indicates no limit.
		//	- If no limit, we'll use 44100, otherwise, we'll use the highest
		//		available frequency.
		bool CheckFreqCapability(string mic, int queryFreq)
		{
			int minFreq = -1;
			int maxFreq = -1;
			// Check for usable recording frequencies (0 = no limit).
			Microphone.GetDeviceCaps(mic, out minFreq, out maxFreq);

			if (minFreq == 0 && maxFreq == 0)
			{
				if ( isDebug )
					Debug.Log("[CheckFreqCapability()] INFO: " + ( string.IsNullOrEmpty(mic) ? "Microphone (default)" : mic ) + " supports all frequencies.");
				return true;
			}

			if ( isDebug )
				Debug.Log("[CheckFreqCapability()] INFO: " + ( string.IsNullOrEmpty(mic) ? "Microphone (default)" : mic ) + " supports min: " + minFreq + " max: " + maxFreq + ".");

			if (queryFreq >= minFreq && queryFreq <= maxFreq)
			{
				if ( isDebug )
					Debug.Log("[CheckFreqCapability()] INFO: " + ( string.IsNullOrEmpty(mic) ? "Microphone (default)" : mic ) + " sampleRate set to: " + sampleRate);

				return true;
			}

			if (overrideSampleRate)
			{
				if ( isDebug )
					Debug.LogWarning("[CheckFreqCapability()] WARNING: " + ( string.IsNullOrEmpty(mic) ? "Microphone (default)" : mic ) + " sampleRate (" + sampleRate + ") is not supported but is overridden.");

				return true;
			}

			if ( isDebug )
				Debug.LogWarning("[CheckFreqCapability()] ERROR: " + ( string.IsNullOrEmpty(mic) ? "Microphone (default)" : mic ) + " sampleRate (" + sampleRate + ") is not supported.");

			return false;
		} // end CheckFreqCapability()



		public void StartMicrophone(string mic)
		{
			// failsafe exit if no mic found.
			if ( !CheckForAvailableMicrophone(mic) )
			{
				if ( isDebug )
				{
					Debug.LogError("[StartMicrophone()] WARNING: no microphone detected.");
				}
				return;
			}

			if ( audioSrc && !isWiredUp )
			{
				if (sampleRate == 0)
					sampleRate = DEFAULT_RATE;

				// confirm sampleRate is valid prior to Microphone.Start()
				if ( !CheckFreqCapability(mic, sampleRate) )
				{
					Debug.LogError("[StartMicrophone()] WARNING: cannot start, invalid sampleRate: " + sampleRate.ToString() + " The microphone may not be reporting a correct recording sample rate. Enable debug mode in micInput for details. It may be necessary to enable Override Rate in micInput if the compatible rates are not correctly reported by the microphone or Unity.");
					return;
				}

				// Set the AudioSource to loop for continuous playback.
				audioSrc.loop = true;

				// If you want to hear the mic input, set this to false.
				audioSrc.mute = isMuted;

				// Start and wait for the microphone to start recording data
				StartCoroutine(WaitForMic(mic));
			}
			else
			{
				if ( isDebug )
					Debug.LogWarning("[StartMicrophone()] WARNING: backing out, AudioSource is NOT available.");
			}

		} // end StartMicrophone()



		public void StopMicrophone(string mic = null)
		{
			if ( audioSrc && isWiredUp )
			{
				// Stop the AudioSource.
				audioSrc.Stop();
				audioSrc.clip = null;

				// Stop the Microphone playback.
				Microphone.End(mic);

				// The AudioSource will be disconnected, flag it.
				isWiredUp = false;

				if ( isDebug )
					Debug.Log("[StopMicrophone()] INFO: " + ( string.IsNullOrEmpty(mic) ? "Microphone (default)" : mic ) + " is stopped.");
			}
			else
			{
				if ( isDebug )
					Debug.LogWarning("[StopMicrophone()] WARNING: backing out, AudioSource is NOT available.");
			}


		} // end StopMicrophone()



		void OnApplicationFocus(bool isFocused)
		{
			if ( isDebug )
			{
				Debug.Log("[OnApplicationFocus()] INFO: isFocused:" + isFocused + " isWired:" + isWiredUp + " runInBack:" + Application.runInBackground);
			}

			// If the application is focused and micInput is not wired up.
			if ( isFocused && !isWiredUp && !Application.runInBackground )
			{

				if ( isMicAvailable && audioSrc )
				{
					if ( isDebug )
						Debug.Log("[OnApplicationFocus()] INFO: StartMicrophone()");

					StartMicrophone(selectedMic);
				}
				else
				{
					if ( isDebug )
						Debug.LogWarning("[OnApplicationFocus()] WARNING: cannot StartMicrophone(); isMicAvailable: " + isMicAvailable + "- AudioSource " + ( audioSrc ? "IS available." : "is NOT available." ));
				}
			}

			// If the application loses focus, stop the Microphone and AudioSource.
			//	- This allows the AudioSource and micInput to be re-wired when the
			//		application is back in focus. Helps prevent it from getting
			//		out of sync. If Application is set to runInBackground, the
			//		application will not respond to focus changes and will continue
			//		running.
			if ( !isFocused && isWiredUp && !Application.runInBackground )
			{
				if ( isMicAvailable && audioSrc )
				{
					if ( isDebug )
						Debug.Log("[OnApplicationFocus()] StopMicrophone()");

					StopMicrophone(selectedMic);
				}
				else
				{
					if ( isDebug )
						Debug.LogWarning("[OnApplicationFocus()] WARNING: cannot StopMicrophone(); isMicAvailable: " + isMicAvailable + "- AudioSource " + ( audioSrc ? "IS available." : "is NOT available." ));
				}
			}

		} // end OnApplicationFocus()



		void OnApplicationPause(bool isPaused)
		{
			if ( isDebug )
			{
				Debug.Log("[OnApplicationPause()] INFO: isPaused:" + isPaused + " isWired:" + isWiredUp + " runInBack:" + Application.runInBackground);
			}

			// If the application is focused and micInput is not wired up.
			if ( !isPaused && !isWiredUp && !Application.runInBackground )
			{
				if ( isMicAvailable && audioSrc )
				{
					if ( isDebug )
						Debug.Log("[OnApplicationPause()] INFO: StartMicrophone()");

					StartMicrophone(selectedMic);
				}
				else
				{
					if ( isDebug )
						Debug.LogWarning("[OnApplicationPause()] WARNING: cannot StartMicrophone(); isMicAvailable: " + isMicAvailable + "- AudioSource " + ( audioSrc ? "IS available." : "is NOT available." ));
				}
			}
			// If the application is paused, stop the Microphone and AudioSource.
			//	- This allows the AudioSource and micInput to be re-wired when the
			//		application is unpaused. Helps prevent it all from getting
			//		out of sync. If Application is set to runInBackground, the
			//		application will not respond to pause changes and will continue
			//		running.
			if ( isPaused && isWiredUp && !Application.runInBackground )
			{
				if ( isMicAvailable && audioSrc )
				{
					if ( isDebug )
						Debug.Log("[OnApplicationPause()] StopMicrophone()");

					StopMicrophone(selectedMic);
				}
				else
				{
					if ( isDebug )
						Debug.LogWarning("[OnApplicationPause()] WARNING: cannot StopMicrophone(); isMicAvailable: " + isMicAvailable + "- AudioSource " + ( audioSrc ? "IS available." : "is NOT available." ));
				}
			}

		} // end OnApplicationPause()



		IEnumerator WaitForMic(string mic)
		{
			float timeCheck = Time.time;

			var loopLength = 0;
			if (sampleRate < 8000)
				loopLength = 15;
			else if (sampleRate < 15000)
				loopLength = 10;
			else if (sampleRate < 30000)
				loopLength = 5;
			else
				loopLength = 3;

			audioSrc.clip = Microphone.Start(mic, true, loopLength, sampleRate);

			// Let the Microphone start filling the buffer prior to activating the AudioSource.
			while ( !( Microphone.GetPosition(mic) > 0 ) )
			{
				if ( isDebug && Time.time - timeCheck > coroutineLoggingIfDebug )
				{
					Debug.Log("[WaitForMic()] - is waiting for the mic to record.");
					timeCheck = Time.time;
				}

				// Wait for Microphone to start gathering data.
				yield return null;
			}

			// If the AudioSource was successfully assigned, play(activate) the AudioSource.
			if ( audioSrc.clip )
			{
				audioSrc.Play();
				isWiredUp = true;


				if (linkWithSalsa)
				{
					var salsa = GetComponent<Salsa>();
					if (salsa)
						salsa.microphone = mic;
				}

				if ( isDebug )
					Debug.Log("[WaitForMic()] INFO: " + ( string.IsNullOrEmpty(mic) ? "Microphone (default)" : mic ) + " is started.");
			}
			else
			{
				Debug.LogError("[WaitForMic()] ERROR: AudioSource has no clip assigned.");
			}
		} // end WaitForMic()



		IEnumerator Wireup()
		{
			float timeCheck = Time.time;

			// confirm an AudioSource is available && give Unity Microphone systems
			// time to instantiate.
			while ( audioSrc == null || Time.time - timeCheck < .1f )
			{
				if ( isDebug && Time.time - timeCheck > coroutineLoggingIfDebug )
				{
					Debug.Log("[Wireup()] - is waiting for an AudioSource.");
					timeCheck = Time.time;
				}

				// no AudioSource found, look for an AudioSource attached to this GameObject
				audioSrc = GetComponent<AudioSource>();

				yield return null;
			}

			if ( isAutoStart )
				StartMicrophone(selectedMic);

		} // end Wireup()

	} // end CM_MicInput Class
}