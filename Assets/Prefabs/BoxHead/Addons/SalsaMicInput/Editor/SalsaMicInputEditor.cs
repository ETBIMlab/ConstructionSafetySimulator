using UnityEngine;
using UnityEditor;

namespace CrazyMinnow.SALSA
{
	/// <summary>
	/// Custom Inspector for MicInput Lite
	/// </summary>
	[CustomEditor(typeof(SalsaMicInput))]
	public class SalsaMicInputEditor : Editor {

		string[] sampleRates = { "4800", "9600", "11025", "22050", "44100" };
		string[] micDevices;
		int rateIndex = 2;
		const int DEFAULT_RATE = 2;
		int deviceIndex = DEFAULT_RATE;

		SalsaMicInput salsaMicInput;

		void OnEnable()
		{
			salsaMicInput = (SalsaMicInput)target;
			GetMicList();
			deviceIndex = GetMicListIndex(salsaMicInput.selectedMic);  // obtain the microphone device setting from micInput instance
			rateIndex = GetRateIndex(salsaMicInput.sampleRate);
		}

		public override void OnInspectorGUI()
		{
			GUILayout.Space(10f);

			GUILayout.BeginVertical(EditorStyles.helpBox);
			salsaMicInput.audioSrc = (AudioSource) EditorGUILayout.ObjectField(new GUIContent("Audio Source", "Manually link this field or micInput will wait for an AudioSource on this GameObject."), salsaMicInput.audioSrc, typeof(AudioSource), true);
			EditorGUI.indentLevel++;
			salsaMicInput.isDebug = EditorGUILayout.Toggle("Debug Mode", salsaMicInput.isDebug);
			salsaMicInput.isAutoStart = EditorGUILayout.Toggle("AutoStart Mode", salsaMicInput.isAutoStart);

			EditorGUI.BeginChangeCheck();
			salsaMicInput.isMuted = EditorGUILayout.Toggle("Mute Microphone", salsaMicInput.isMuted);
			if (EditorGUI.EndChangeCheck())
			{
				if (salsaMicInput.audioSrc == null)
				{
					var src = salsaMicInput.GetComponent<AudioSource>();
					if (src)
						src.mute = salsaMicInput.isMuted;
				}
				else
					salsaMicInput.audioSrc.mute = salsaMicInput.isMuted;
			}
			EditorGUI.indentLevel--;
			GUILayout.EndVertical();

			GUILayout.Space(5f);

			GUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUI.BeginChangeCheck();
			int conversionCatch;
			rateIndex = EditorGUILayout.Popup("Sample Rate", rateIndex, sampleRates);
			if ( int.TryParse(sampleRates[rateIndex], out conversionCatch) )
				salsaMicInput.sampleRate = conversionCatch;
			else
				salsaMicInput.sampleRate = salsaMicInput.DEFAULT_RATE;
			if ( EditorGUI.EndChangeCheck() )
			{
				salsaMicInput.StopMicrophone(salsaMicInput.selectedMic);
				salsaMicInput.StartMicrophone(salsaMicInput.selectedMic);
			}

			EditorGUI.indentLevel++;
			salsaMicInput.overrideSampleRate = EditorGUILayout.Toggle(new GUIContent("Override Rate", "Override the system-reported rate if it is not reported correctly."), salsaMicInput.overrideSampleRate);
			EditorGUI.BeginChangeCheck();
			salsaMicInput.linkWithSalsa = EditorGUILayout.Toggle(new GUIContent( "Link with SALSA", "At runtime, looks for SALSA on this GameObject and updates key SALSA fields."), salsaMicInput.linkWithSalsa);
			if (EditorGUI.EndChangeCheck())
			{
				var salsa = salsaMicInput.GetComponent<Salsa>();
				if (salsaMicInput.linkWithSalsa)
				{
					if (salsa)
					{
						salsa.autoAdjustAnalysis = true;
						salsa.autoAdjustMicrophone = salsaMicInput.linkWithSalsa;
						if (salsaMicInput.GetComponent<SalsaMicPointerSync>() == null)
							salsaMicInput.gameObject.AddComponent<SalsaMicPointerSync>();
					}
				}
				else
					salsa.autoAdjustMicrophone = false;
			}
			EditorGUI.indentLevel--;
			GUILayout.EndVertical();

			GUILayout.Space(5f);

			GUILayout.BeginVertical(EditorStyles.helpBox);
			// get selection index, then parse micDevices[] for the appropriate device name
			EditorGUI.BeginChangeCheck();
			string prevMic = salsaMicInput.selectedMic;

			deviceIndex = EditorGUILayout.Popup("Select Microphone", deviceIndex, micDevices);
			if ( deviceIndex > 0 )
				salsaMicInput.selectedMic = micDevices[deviceIndex];
			else
				salsaMicInput.selectedMic = default(string);
			if ( EditorGUI.EndChangeCheck() )
			{
				salsaMicInput.StopMicrophone(prevMic);
				salsaMicInput.StartMicrophone(salsaMicInput.selectedMic);
			}

			// refresh microphone button -- not usually necessary...
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if ( GUILayout.Button(new GUIContent("Refresh Mic List","If hardware is added or removed and does not update automatically, use this option to force a refresh."), GUILayout.ExpandWidth(false)) )
			{
				GetMicList();
				deviceIndex = GetMicListIndex(salsaMicInput.selectedMic); // obtain the microphone device setting from micInput instance (IF POSSIBLE)
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}

		void GetMicList()
		{
			if ( Microphone.devices.Length > 0 )
			{
				micDevices = new string[Microphone.devices.Length + 1];

				// create an entry for 'Default' mic
				micDevices[0] = "Default";

				for ( int i = 0; i < Microphone.devices.Length; i++ )
				{
					micDevices[i + 1] = Microphone.devices[i];
				}
			}
			else
			{
				micDevices = new string[1] { "ERROR - no microphones available" };
			}
		}

		int GetMicListIndex(string selected)
		{
			for ( int i = 0; i < micDevices.Length; i++ )
			{
				if ( micDevices[i] == selected )
				{
					return i;
				}
			}

			return 0;	// return default
		}

		int GetRateIndex(int rate)
		{
			for ( int i = 0; i < sampleRates.Length; i++ )
			{
				if ( rate.ToString() == sampleRates[i] )
				{
					return i;
				}
			}

			return DEFAULT_RATE;	// return default
		}
	}
}
