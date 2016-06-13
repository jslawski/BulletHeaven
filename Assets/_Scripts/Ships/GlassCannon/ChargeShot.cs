using UnityEngine;
using System.Collections;

public class ChargeShot : MonoBehaviour {
	ParticleSystem chargeParticle;

	float chargeTime = 3f;
	float maxChargeAngle = 25f;
	float minChargeAngle = 0f;
	float minChargeRotationSpeed = 1f;
	float maxChargeRotationSpeed = 10f;
	float maxStartSize = 1f;
	float minStartSize = 0.2f;
	Color startColor = new Color(1,1,1, 82f/255f);
	Color endColor = new Color(1, 86f/255f, 86f/255f, 82f/255f);

	// Use this for initialization
	void Start () {
		chargeParticle = transform.FindChild("ChargeParticles").GetComponent<ParticleSystem>();
		StartCoroutine(Charge());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator Charge() {
		float timeElapsed = 0f;
		ParticleSystem.ShapeModule shape = chargeParticle.shape;
		while (timeElapsed < chargeTime) {
			timeElapsed += Time.deltaTime;
			float percent = timeElapsed/chargeTime;

			shape.radius = Mathf.Lerp(1, 0, percent);
			shape.angle = Mathf.Lerp(maxChargeAngle, minChargeAngle, percent*percent);
			Vector3 curRot = chargeParticle.transform.localRotation.eulerAngles;
			curRot.z += Mathf.Lerp(minChargeRotationSpeed, maxChargeRotationSpeed, percent*percent);
			chargeParticle.transform.localRotation = Quaternion.Euler(curRot);

			chargeParticle.startSize = Mathf.Lerp(maxStartSize, minStartSize, percent * percent);
			chargeParticle.startColor = Color.Lerp(startColor, endColor, percent * percent);

			yield return null;
		}
	}
}
