using UnityEngine;
using UnityEngine.UI;

namespace BuildInfoUtility
{
	public class BuildIdView : MonoBehaviour
	{
		[SerializeField]
		private Text _text;

		private void Reset()
		{
			_text = GetComponent<Text>();
			_text.text = $"Build ID: ***";
		}

		private void Start()
		{
			if (BuidRuntimeInfo.Instance != null)
			{
				_text.text = $"Build ID: {BuidRuntimeInfo.Instance.BuildId}";
			}
		}
	}
}