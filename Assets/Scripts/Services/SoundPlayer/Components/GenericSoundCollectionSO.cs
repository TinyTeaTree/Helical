using Core;
using UnityEngine;

namespace Services
{
	public class GenericSoundCollectionSO : BaseSO
	{
		[SerializeField] private System.Collections.Generic.List<BaseSoundDesign> _sounds = new System.Collections.Generic.List<BaseSoundDesign>();

		private System.Collections.Generic.Dictionary<string, BaseSoundDesign> _nameToDesign;

		public System.Collections.Generic.IReadOnlyList<BaseSoundDesign> Sounds => _sounds;

		public BaseSoundDesign Get(string designName)
		{
			if (string.IsNullOrEmpty(designName))
				return null;

			if (_nameToDesign == null)
			{
				_nameToDesign = new System.Collections.Generic.Dictionary<string, BaseSoundDesign>();
				for (int i = 0; i < _sounds.Count; i++)
				{
					var d = _sounds[i];
					if (d == null || string.IsNullOrEmpty(d.name))
						continue;
					_nameToDesign[d.name] = d;
				}
			}

			return _nameToDesign.TryGetValue(designName, out var result) ? result : null;
		}
	}
}

