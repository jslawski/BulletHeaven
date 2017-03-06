using UnityEngine;
using System.Collections;

public interface BombAttack {
	PlayerEnum owningPlayer { get; set; }

	void FireBurst();
}
