using System.Collections.Generic;
using UnityEngine;
using GHPC.Camera;
using GHPC.Player;

namespace ATLASMissionPack
{
	public class SharedZoomHandler : MonoBehaviour
	{
		private Dictionary<float, float[]> zoom_levels = new Dictionary<float, float[]>();
		public CameraSlot day;
		public CameraSlot night;
		public Transform day_reticle_plane;
		public Transform night_reticle_plane;

		public void Add(float zoom, float blur, float reticle_scale)
		{
			day.OtherFovs = Util.AppendToArray(day.OtherFovs, zoom);
			night.OtherFovs = Util.AppendToArray(night.OtherFovs, zoom);
			zoom_levels.Add(zoom, new float[] { blur, reticle_scale });
		}

		void Update()
		{
			if (PlayerInput.Instance.CurrentPlayerWeapon.FCS.GetInstanceID() != day.PairedOptic.FCS.GetInstanceID()) return;

			CameraSlot current = day.PairedOptic.FCS.MainOptic.slot; 

			if (!zoom_levels.ContainsKey(current.CurrentFov))
			{
				day_reticle_plane.localScale = Vector3.one;
				night_reticle_plane.localScale = Vector3.one;
				return;
			};

			float scale = zoom_levels[current.CurrentFov][1];
			day_reticle_plane.localScale = new Vector3(scale, scale, scale);
			night_reticle_plane.localScale = new Vector3(scale, scale, scale);
			day.FovIndex = current.FovIndex;
			night.FovIndex = current.FovIndex;
		}
	}
}
