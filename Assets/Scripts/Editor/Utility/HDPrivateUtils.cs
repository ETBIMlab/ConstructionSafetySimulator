using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Defines funcitons for some of the private High Definition utilities.
/// This is only used to take advantage of some of the HD specific settings.
/// </summary>
internal class HDPrivateUtils
{
	/// <summary>
	/// Converts a GUID of a file into the Vector4 needed for assigning diffusion profile.
	/// </summary>
	/// <param name="guid">GUID of asset file</param>
	/// <returns>Vector4 that represents the GUID of the given asset file</returns>
	internal static Vector4 HDUtils_ConvertGUIDToVector4(string guid)
	{
		Vector4 vector;
		byte[] bytes = new byte[16];

		for (int i = 0; i < 16; i++)
			bytes[i] = byte.Parse(guid.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);

		// <<< REPLACE unsafe >>>
		//unsafe
		//{
		//	fixed (byte* b = bytes)
		//		vector = *(Vector4*)b;
		//}

		// <<< With >>>
		vector = new Vector4(
			BitConverter.ToSingle(bytes, 0 * sizeof(float)),
			BitConverter.ToSingle(bytes, 1 * sizeof(float)),
			BitConverter.ToSingle(bytes, 2 * sizeof(float)),
			BitConverter.ToSingle(bytes, 3 * sizeof(float))
		);

		return vector;
	}

	/// <summary>
	/// Directly translates the bytes of a uint to the bytes of a float.
	/// </summary>
	/// <param name="val">unit input</param>
	/// <returns>float translation of input bytes</returns>
	internal static float HDShadowUtils_Asfloat(uint val)
	{
		// <<< REPLACE unsafe >>>
		// unsafe { return *((float*)&val);

		// <<< With >>>
		return BitConverter.ToSingle(BitConverter.GetBytes(val), 0);
	}

	/// <summary>
	/// Gets a hash of a diffusion profile, where the hash will nicely convert to a float for identification (in shaders/materials)
	/// </summary>
	/// <param name="asset">Diffusion Profile asset</param>
	/// <returns>uint hash which avoids NaN, inf, and precision issues.</returns>
	internal static uint DiffusionProfileHashTable_GetDiffusionProfileHash(UnityEngine.Object asset)
	{
		string assetPath = AssetDatabase.GetAssetPath(asset);

		// In case the diffusion profile is not yet saved on the disk, we don't generate the hash
		if (String.IsNullOrEmpty(assetPath))
			return 0;

		uint hash32 = (uint)AssetDatabase.AssetPathToGUID(assetPath).GetHashCode();
		uint mantissa = hash32 & 0x7FFFFF;
		uint exponent = 0b10000000; // 0 as exponent

		// only store the first 23 bits so when the hash is converted to float, it doesn't write into
		// the exponent part of the float (which avoids having NaNs, inf or precisions issues)
		return (exponent << 23) | mantissa;
	}
}
