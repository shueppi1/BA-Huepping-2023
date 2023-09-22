using UnityEngine;
namespace VRBowlingDemo
{
    public class BowlingPin : MonoBehaviour
    {
        [SerializeField] private float standingUpThreshold;
        [SerializeField] private AudioSource audioSource;

        private Rigidbody _rigidbody;

        private Vector3 _spawnPosition;
        private Quaternion _spawnRotation;


        // Start is called before the first frame update
        private void Start()
        {
            //Cache reference on rigidbody and spawn
            _rigidbody = GetComponent<Rigidbody>();
            _spawnPosition = transform.position;
            _spawnRotation = transform.rotation;
        }

        private void OnCollisionEnter(Collision other)
        {
            //play a sound, when hitting the environment or being hit by the ball
            if (!other.gameObject.CompareTag("Environment") || other.gameObject.CompareTag("BowlingBall"))
            {
                //only play the sound when no other sound is playing. else, the sound would always start again without finishing
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
        }

        //Draw the Up direction of the Pin in the SceneView
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.up);
        }

        //Is the Pin standing?
        public bool CheckIsStanding()
        {
            //transform.up is normalized. That means, when the pin is standing straight, its y value is 1.
            //To provide a little bit of tolerance, we use a standingThreshold. When the pin is straight enough, it is considered as straight
            return transform.up.y > standingUpThreshold;
        }

        public void ResetPosition()
        {
            //set the position and rotation to the cached default values
            transform.position = _spawnPosition;
            transform.rotation = _spawnRotation;

            //We need to reset the velocity.
            //If the pin is falling or rolling and we only reset the position and rotation, the velocity/movement will remain
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }
}