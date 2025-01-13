using UnityEngine;

namespace Objects
{
    public class MyGrid : MonoBehaviour
    {
        public GameObject walkable;
        public GameObject nonWalkable;
        public GameObject gate;
    
        private void Start()
        {
            ActivateWalkable();
        }

        public void ActivateWalkable()
        {
            if (walkable)
            {
                walkable.SetActive(true);
                if (nonWalkable) nonWalkable.SetActive(false);
                if (gate) gate.SetActive(false);

                Debug.Log("Walkable activated, others deactivated.");
            }
            else
            {
                Debug.LogWarning("Walkable GameObject is not assigned.");
            }
        }

        public void ActivateNonWalkable()
        {
            if (nonWalkable)
            {
                nonWalkable.SetActive(true);
                if (walkable) walkable.SetActive(false);
                if (gate) gate.SetActive(false);

                Debug.Log("Non-Walkable activated, others deactivated.");
            }
            else
            {
                Debug.LogWarning("Non-Walkable GameObject is not assigned.");
            }
        }

        private void ActivateGate()
        {
            if (gate)
            {
                gate.SetActive(true);
                if (walkable) walkable.SetActive(false);
                if (nonWalkable) nonWalkable.SetActive(false);

                Debug.Log("Gate activated, others deactivated.");
            }
            else
            {
                Debug.LogWarning("Gate GameObject is not assigned.");
            }
        }

        public CustomerGate GetCustomerGate()
        {
            ActivateGate();
            return gate.GetComponent<CustomerGate>();
        }
    }
}