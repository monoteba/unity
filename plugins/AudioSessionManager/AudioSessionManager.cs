using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class AudioSessionManager : MonoBehaviour {

	#if UNITY_IPHONE
		[DllImport("__Internal")]
		private static extern void AudioSessionManager_SetupAVAudioSession();

		[DllImport("__Internal")]
		private static extern bool AudioSessionManager_RequestRecordPermission();

		[DllImport("__Internal")]
		private static extern bool AudioSessionManager_RecordPermissionStatus();
	#endif

	public static void SetupAVAudioSession()
	{
		#if UNITY_IPHONE

		if (Application.platform == RuntimePlatform.IPhonePlayer)
			AudioSessionManager_SetupAVAudioSession();

		#endif
	}

	public static bool RequestRecordPermission()
	{
	    #if UNITY_IPHONE

	    if (Application.platform == RuntimePlatform.IPhonePlayer)
	        return AudioSessionManager_RequestRecordPermission();

        #endif

    	return false;
	}

	public static bool RecordPermissionStatus()
	{
		#if UNITY_IPHONE

		if (Application.platform == RuntimePlatform.IPhonePlayer)
			return AudioSessionManager_RecordPermissionStatus();

		#endif

		return false;
	}
}
