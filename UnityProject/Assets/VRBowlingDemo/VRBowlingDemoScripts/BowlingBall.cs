using UnityEngine;
namespace VRBowlingDemo
{
    public class BowlingBall : MonoBehaviour
    {
        [SerializeField] private float standingThreshold;
        [SerializeField] private AudioSource audioSource;

        private Rigidbody _rigidbody;
        private Vector3 _spawnPosition;
        private Quaternion _spawnRotation;

        private BowlingManager _bowlingManager;
        private bool _isInThrow;

        private void Start()
        {
            //cache a reference to the Bowling Manager, rigidbody and spawn positions
            _bowlingManager = FindObjectOfType<BowlingManager>();
            _rigidbody = GetComponent<Rigidbody>();
            _spawnPosition = transform.position;
            _spawnRotation = transform.rotation;
        }

        private void FixedUpdate()
        {
            //trigger a reset if the ball stops rolling. that can e.g. happen when it was not thrown hard enough
            
            //only check if the ball was thrown, not when it rests in the ball spawner
            if (_isInThrow)
            {
                //check if the velocity magnitude is lower than a certain threshold (i.e. when the ball is rolling very slow)
                //use the sqrMagnitude as i requires less calculation and we can use it the same way as the "normal" magnitude
                if (_rigidbody.velocity.sqrMagnitude < standingThreshold)
                {
                    _bowlingManager.Evaluate();
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            //play a sound when the ball collides with the environment
            if (collision.gameObject.CompareTag("Environment"))
            {
                if (audioSource)
                {
                    audioSource.Play();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            //when the ball enters the back of the bowling alley (i.e. the trow is complete)
            if (other.CompareTag("Back"))
            {
                //evaluate the throw and reset
                _bowlingManager.OnBallEnteredBack();
            }
            //reset the balls position when the death plane is hit
            else if (other.CompareTag("DeathPlane"))
            {
                ResetPosition();
            }
        }


        //called by the SelectExit event of the XRGrabInteractable
        //That means that the user does not hold the ball anymore
        //--> we assume the ball was thrown and we want to trigger a reset, when the ball stops rolling
        public void OnBallThrow()
        {
            _isInThrow = true;
        }

        //called by the SelectEntered event of the XRGrabInteractable
        //That means the user starts grabbing the ball --> the ball is not "in a throw"
        public void OnBallGrab()
        {
            _isInThrow = false;
        }

        //reset the position of the ball
        public void ResetPosition()
        {
            //it is not in a throw anymore
            _isInThrow = false;
            
            //reset the position and rotation
            transform.position = _spawnPosition;
            transform.rotation = _spawnRotation;
            //reset the velocity (See BowlingPin.cs for more information)
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }
}