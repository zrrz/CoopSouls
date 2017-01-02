using UnityEngine;
using System.Collections;

public class Dust : MonoBehaviour {


	//TODO Make pretty.
	//Make spawn over time.
	//Clean up particle pull
	//Add dust to inverntory

//	float amount;

	Transform target = null;
	ParticleSystem particleSystem;
	ParticleSystem.Particle[] particles;

	const float particleSpeed = 0.2f;
	const float delay = 0.5f;
	float delayTimer = 0f;

	void Awake () {
		particleSystem = GetComponent<ParticleSystem>();
	}
	
	void Update () {
		if(delayTimer < delay)
			delayTimer += Time.deltaTime;
		if(target != null) {
			if(particles == null || particles.Length < particleSystem.maxParticles)
				particles = new ParticleSystem.Particle[particleSystem.maxParticles];
			int numParticlesAlive = particleSystem.GetParticles(particles);
			for(int i = 0; i < numParticlesAlive; i++) {
				Vector3 dist = target.transform.position - particles[i].position;
				if(dist.magnitude < 0.5f) {
					particles[i].lifetime = 0f;
				} else {
					float r = dist.sqrMagnitude;
					dist.Normalize();
					dist /= r;

	//				float force = 1 / r*r;

	//				particles[i].velocity = dist;//* force;
					Vector3 prevPos = particles[i].position;
					particles[i].position += dist * particleSpeed;
//					Debug.DrawLine(prevPos, particles[i].position, Color.red, 2f);
				}
			}
			particleSystem.SetParticles(particles, numParticlesAlive);
		}
	}

	public void SetAmount(int amount) {
//		ParticleSystem.EmissionModule emission = particleSystem.emission;
//		emission.rate = amount/20f; //Half and a tenth
		particleSystem.maxParticles = amount;
	}

	void OnTriggerStay2D(Collider2D col) {
		if(delayTimer < delay)
			return;
		if(target == null) {
			if(col.tag == "Player") {
				target = col.transform;
				ParticleSystem.EmissionModule emission = particleSystem.emission;
				emission.rate = 0f;
			}
		}
	}
}
