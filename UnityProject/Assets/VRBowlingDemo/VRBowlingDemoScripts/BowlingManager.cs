using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace VRBowlingDemo
{
    public class BowlingManager : MonoBehaviour
    {
        //List of all bowling pins
        [SerializeField] private List<BowlingPin> _bowlingPins;
        //how long to wait with the evaluation after the ball enters the back of the bowling alley
        //We need this as bowling pins can take some time to fall or might hit other pins while falling
        [SerializeField] private float secondsToWaitAfterBallBack;
        
        //when the throw was evaluated, how long should the points
        [SerializeField] private float secondsToWaitAfterPointsShown;

        [SerializeField] private GameObject pointsUI;
        [SerializeField] private TextMeshProUGUI pointsText;
        [SerializeField] private BowlingBall ball;


        [ContextMenu("ResetPositions")]
        public void DoReset()
        {
            //reset the positions of all bowling pins
            foreach (var pin in _bowlingPins)
            {
                pin.ResetPosition();
            }

            //also reset the ball
            ball.ResetPosition();
            
            //deactivate the UI showing the points
            pointsUI.SetActive(false);
        }

        
        private int CalculatePoints()
        {
            //count, how many points are not standing anymore
            var points = 0;
            foreach (var pin in _bowlingPins)
            {
                if (!pin.CheckIsStanding())
                {
                    points++;
                }
            }

            return points;
        }

        //Called by BowlingBall.cs when the ball entered the back area of the bowling alley (i.e. the throw is finished)
        public void OnBallEnteredBack()
        {
            //Start the BallEnteredDelay coroutine, i.e. wait some time before evaluating the points.
            //Needed so all pins are finished falling before it is evaluated
            StartCoroutine(BallEnteredDelay());
        }

        //Evaluate the throw
        public void Evaluate()
        {
            //Count the points and write it on the UI
            pointsText.text = CalculatePoints().ToString();
            //show the ui
            pointsUI.SetActive(true);
            //Show the Points for some time before the ball and pins are reset for the next throw
            StartCoroutine(PointsShownDelay());
        }


        private IEnumerator BallEnteredDelay()
        {
            //wait secondsToWaitAfterBallBack seconds until Evaluate() is executed
            yield return new WaitForSeconds(secondsToWaitAfterBallBack);
            Evaluate();
        }

        private IEnumerator PointsShownDelay()
        {
            //wait secondsToWaitAfterPointsShown seconds until DoReset() is executed
            yield return new WaitForSeconds(secondsToWaitAfterPointsShown);
            DoReset();
        }
    }
}