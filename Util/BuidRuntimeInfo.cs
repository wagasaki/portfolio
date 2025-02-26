using UnityEngine;

namespace BuildInfoUtility
{
	[CreateAssetMenu(fileName = "BuildRuntimeInfoSettings", menuName = "Build runtime info settings")]
	public class BuidRuntimeInfo : ScriptableObject
	{
		public static BuidRuntimeInfo Instance => _instance ??= LoadInfo();
		private static BuidRuntimeInfo _instance;

		public string Version;
		public string BuildId;

		private static BuidRuntimeInfo LoadInfo()
		{
			BuidRuntimeInfo result = Resources.Load<BuidRuntimeInfo>("BuildRuntimeInfoSettings");
			if (result == null)
			{
				Debug.LogError("You need create asset file before build from context menu \"Assets/Create/Build runtime info settings\"");
			}

			return result;
		}
	}
}