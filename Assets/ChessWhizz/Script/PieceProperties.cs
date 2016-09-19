using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace UnityStandardAssets.Network{
	public class PieceProperties : NetworkBehaviour {
		[SyncVar]
		public GameObject location;
		[SyncVar]
		public bool canEat;
		[SyncVar]
		public int pieceLevel;

		void Start(){
			if(!isServer)
				return;

			getPieceLevel();
		}

		void OnCollisionStay(Collision other){
			if(!isServer)
				return;

			if(location != other.gameObject){
				getLocation(other.gameObject);
			}
		}

		[ServerCallback]
		public void getPieceLevel(){
			if(this.gameObject.name == "Pawn")
				pieceLevel = 0;
			else if(this.gameObject.name == "Rook")
				pieceLevel = 1;
			else if(this.gameObject.name == "Knight")
				pieceLevel = 2;
			else if(this.gameObject.name == "Bishop")
				pieceLevel = 3;
			else if (this.gameObject.name == "Queen")
				pieceLevel = 4;
			else
				pieceLevel = 5;
		}

		[ServerCallback]
		public void getLocation(GameObject other){
				location = other;
		}
	}
}
