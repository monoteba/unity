using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class iOSAppSettings : MonoBehaviour {
	#if UNITY_IPHONE
		[DllImport("__Internal")]
		private static extern void iOSAppSettings_OpenSettingsApp();
	#endif

	public static void OpenSettingsApp()
	{
		#if UNITY_IPHONE

		if (Application.platform == RuntimePlatform.IPhonePlayer)
			iOSAppSettings_OpenSettingsApp();

		#endif
	}

}