using UnityEngine;  
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.Serialization;

namespace Objects
{

public class Bus : MonoBehaviour
{
    #region Variables

    [FormerlySerializedAs("color")] public ColorType colorType;
    [SerializeField] private List<Transform> availableSeats = new();
    private readonly Dictionary<Transform, CustomerAI> seatAssignments = new();
    private readonly List<Task> customerAnimationTasks = new();
    [SerializeField] private Transform door;
    private BusManager busManager;
    [SerializeField] private float doorUpY = 2.4f;
    [SerializeField] private float doorDownY = 0.8f;

    #endregion

    #region Functions

    private void Awake()
    {
        busManager = GameManager.Instance.busManager;
    }

    /// <summary>
    /// Assigns a customer to an available seat.
    /// </summary>
    /// <param name="customer">The customer to assign.</param>
    public void CustomerAssign(CustomerAI customer)
    {
        if (availableSeats.Count > 0)
        {
            var seat = availableSeats[0];
            availableSeats.RemoveAt(0);
            seatAssignments[seat] = customer;

            PlayCustomerAssignAnimation(customer, seat);

            if (availableSeats.Count <= 0)
            {
                RemoveThisBus();
            }
        }
        else
        {
            Debug.LogWarning("No available seats to assign.");
        }
    }

    /// <summary>
    /// Plays the animation for assigning a customer to a seat.
    /// </summary>
    /// <param name="customer">The customer being assigned.</param>
    /// <param name="seat">The seat assigned to the customer.</param>
    private void PlayCustomerAssignAnimation(CustomerAI customer, Transform seat)
    {
        customerAnimationTasks.Add(customer.SetDestination(seat.position));
    }

    private async void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out CustomerAI customer)) return;

        OpenDoorAnimation();
        
        // Find the seat assigned to the customer
        foreach (var seat in from entry in seatAssignments where entry.Value == customer select entry.Key)
        {
            // Play spawn animation at the assigned seat
            await customer.SpawnAnimation(seat.position);
            CloseDoor();
            // Remove the customer from CustomerManager
            GameManager.Instance.customerManager.RemoveCustomer(customer);

            return;
        }
    }
    private void OpenDoorAnimation()
    {
        door.DOKill();
        door.DOLocalMoveY(doorUpY, 0.2f);
    }

    private void CloseDoor()
    {
        door.DOLocalMoveY(doorDownY, 0.2f);
    }


    /// <summary>
    /// Removes this bus and updates bus positions.
    /// </summary>
    private void RemoveThisBus()
    {
        busManager.RemoveBus(this);
        UpdateBusPositions();
    }

    /// <summary>
    /// Waits for all customer animations to complete before updating bus positions.
    /// </summary>
    private async void UpdateBusPositions()
    {
        await Task.WhenAll(customerAnimationTasks);
        customerAnimationTasks.Clear();

        busManager.UpdatePositions(this);
    }

    /// <summary>
    /// Moves the bus to the specified position using DOTween.
    /// </summary>
    /// <param name="position">The target position.</param>
    public void BusMovingForward(Vector3 position)
    {
        transform.DOMove(position, 1f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            Debug.Log("Bus reached the next position.");
        });
    }

    /// <summary>
    /// Handles the bus going off the screen and plays destruction animation.
    /// </summary>
    public void BusGoingOutOfScreenAndDestroyAnimation()
    {
        var offScreenPosition = transform.position + Vector3.right * 20f; // Example off-screen direction
        transform.DOMove(offScreenPosition, 2f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            Destroy(gameObject);
            Debug.Log("Bus destroyed after moving off screen.");
        });
    }

    #endregion
}

}
