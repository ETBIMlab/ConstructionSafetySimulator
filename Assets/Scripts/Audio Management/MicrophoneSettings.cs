using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class MicrophoneSettings : MonoBehaviour
{
    [NonSerialized]
    private static string mic = null;
    [NonSerialized]
    public static Action<string> OnMicChanged;
    public static string Mic
    {
        get
        {
            if (mic == null || !Microphone.devices.Contains(mic))
                return null;
            return mic;
        }
        set
        {
            if (value != null && Microphone.devices.Contains(value))
            {
                if (OnMicChanged != null) OnMicChanged(value);
                mic = value;
            }
        }
    }

    public Dropdown dropdown;

    private int micIndex;
    public int MicIndex
    {
        get
        {
            return micIndex;
        }
        set
        {
            micIndex = value;
            if (Microphone.devices.Length > value)
            {
                Mic = Microphone.devices[value];
            }
        }
    }

    private void OnEnable()
    {
        dropdown.options = new List<Dropdown.OptionData>();

        foreach (string device in Microphone.devices)
        {
            dropdown.options.Add(new Dropdown.OptionData(device));
        }

        int current = Array.IndexOf(Microphone.devices, Mic);
        if (current < 0)
            current = 0;

        dropdown.value = current;
        micIndex = current;
    }
}
