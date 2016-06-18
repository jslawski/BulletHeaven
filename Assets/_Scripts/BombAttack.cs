using UnityEngine;
using System.Collections;

public interface BombAttack {
	Player owningPlayer { get; set; }

	void FireBurst();
}
