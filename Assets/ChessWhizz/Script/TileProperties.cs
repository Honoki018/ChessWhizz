using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace UnityStandardAssets.Network{
	public class TileProperties : NetworkBehaviour {
		[SyncVar]
		public bool canClick;
		[SyncVar]
		public GameObject useBy;

		void OnCollisionStay(Collision other){
			if(!isServer)
				return;

			onStay(other.gameObject);
		}

		void OnCollisionExit(Collision other){
			if(!isServer)
				return;

			onExit(other.gameObject);
		}

		[ServerCallback]
		public void onStay(GameObject other){
			if(!useBy){
				useBy = other;
			}
		}

		[ServerCallback]
		public void onExit(GameObject other){
			if(other == useBy){
				useBy = null;
			}
		}
	}
}
