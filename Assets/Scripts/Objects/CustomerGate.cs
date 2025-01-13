using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinding;
using UnityEngine;

namespace Objects
{
    public class CustomerGate : MonoBehaviour
    {
        // Variables
        [SerializeField] private List<CustomerAI> assignedCustomerList; // List of customers assigned to this gate
        [SerializeField] private MyGrid spawnGrid;                      // Reference to the grid where this gate will spawn customers


        // Check if the referenced spawnGrid is available or not
        public async void CheckSpace()
        {
            try
            {
                var pathPossible = await IsPathPossible();

                if (pathPossible)
                {
                    AddCustomer();
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error: {e} inside Customer Gate", this);
            }
        }

        private Task<bool> IsPathPossible()
        {
            AstarPath.active.Scan();
            
            // Get the corresponding nodes
            var startNode = AstarPath.active.GetNearest(transform.position).node;
            var endNode = AstarPath.active.GetNearest(spawnGrid.transform.position).node;

            if (startNode == null || endNode == null)
            {
                Debug.LogError("Start or End node is null!");
                return Task.FromResult(false);
            }

            Debug.Log($"Start Node Walkable: {startNode.Walkable}, End Node Walkable: {endNode.Walkable}");

            // Check path connectivity
            return Task.FromResult(PathUtilities.IsPathPossible(startNode, endNode));
        }

        // Add first customer from the list, remove from current list and assign to Customer Manager's spawned list
        private void AddCustomer()
        {
            if (assignedCustomerList is not { Count: > 0 } || !GameManager.Instance.customerManager) return;
            
            var customer = assignedCustomerList[0];
            assignedCustomerList.RemoveAt(0); // Remove the first customer from the list

            // Add customer to the Customer Manager's spawned list
            GameManager.Instance.customerManager.AddSpawnedCustomer(customer);

            // Call the gate spawn animation for the customer
            _ = customer.SpawnAnimation(spawnGrid.transform.position);
        }

        public void AssignSpawnGrid(MyGrid grid)
        {
            spawnGrid = grid;
            transform.LookAt(spawnGrid.transform.position);
        }
        public bool HasNoCustomers()
        {
            return assignedCustomerList.Count <= 0;
        }
    }
}