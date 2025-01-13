using System.Threading.Tasks;
using DG.Tweening;
using Pathfinding;
using UnityEngine;

namespace Objects
{
    public class CustomerAI : MonoBehaviour
    {
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        #region Variables
        public ColorType colorType;
        [SerializeField] private AIPath aiPath;
        [SerializeField] private Seeker seeker;
        [SerializeField] private Renderer indicateRenderer;
        [SerializeField] private GameObject obstacle;

        private bool canReachDestination;
        private bool onStand;
        
        public bool AllAnimationsCompleted;
        #endregion

        #region Unity Callbacks

        private void Start()
        {
            aiPath.canMove = false;
            aiPath.destination = GameManager.Instance.busManager.defaultTarget.position;
            UpdateReachability();
        }

        private void OnMouseDown()
        {
            if (!canReachDestination) return;

            canReachDestination = false;
            CheckColor();
        }

        #endregion

        #region Reachability and Pathfinding

        public Task UpdateReachability()
        {
            if (onStand) return Task.CompletedTask;
            if (aiPath.destination == Vector3.zero) return Task.CompletedTask;

            obstacle.SetActive(false);
            AstarPath.active.Scan();
        
            // Snap to the nearest valid node
            var snappedStart = AstarPath.active.GetNearest(transform.position).position;
            var snappedEnd = AstarPath.active.GetNearest(aiPath.destination).position;

            // Debug positions
            Debug.Log($"Snapped Start Position: {snappedStart}, Snapped End Position: {snappedEnd}");

            // Get the corresponding nodes
            var startNode = AstarPath.active.GetNearest(transform.position).node;
            var endNode = AstarPath.active.GetNearest(aiPath.destination).node;

            if (startNode == null)
            {
                Debug.LogError("Start node is null!",this);
                return Task.CompletedTask;
            }

            if (endNode == null)
            {
                Debug.LogError($"End node is null! {aiPath.destination}", this);
                return Task.CompletedTask;
            }

            Debug.Log($"Start Node Walkable: {startNode.Walkable}, End Node Walkable: {endNode.Walkable}");

            // Check path connectivity
            canReachDestination = PathUtilities.IsPathPossible(startNode, endNode);

            obstacle.SetActive(true);
            AstarPath.active.Scan();
        
            Debug.Log(canReachDestination
                ? "Path is possible!"
                : "Path is not possible.");
        
            if (canReachDestination)
                MakeAvailable();
            else
                MakeUnavailable();
            return Task.CompletedTask;
        }

        #region Written By GPT

        // private async void UpdateReachability()
        // {
        //     // Logic to check if a path is available
        //     bool reachable = await CheckPathAvailability(GameManager.Instance.BusManager.defaultTarget.position);
        //     canReachDestination = reachable;
        //
        //     if (canReachDestination)
        //         MakeAvailable();
        //     else
        //         MakeUnavailable();
        // }
        // private Task<bool> CheckPathAvailability(Vector3 targetPosition)
        // {
        //     var path = seeker.StartPath(transform.position, targetPosition);
        //     return Task.Run(() =>
        //     {
        //         AstarPath.WaitForPath(path);
        //         return !path.error;
        //     });
        // }

        #endregion


        public async Task SetDestination(Vector3 targetPosition)
        {
            var path = seeker.StartPath(transform.position, targetPosition);
            //TODO: here it was AstarPath.WaitForPath but it said it is obsolete, so I changed it to suggested method BlockUnlitCalculated.
            //basically there are chances of errors. Also, this part was by AI in my original script I was just giving target position and setting canMove to true 
            AstarPath.BlockUntilCalculated(path);
            aiPath.destination = targetPosition;
            aiPath.canMove = true;

            // Follow the path
            while (aiPath.remainingDistance > aiPath.endReachedDistance)
            {
                await Task.Yield();
            }

            aiPath.canMove = false;
        }

        #endregion

        #region Availability Indicators

        private void MakeAvailable()
        {
            indicateRenderer.material.SetColor(EmissionColor, Color.green * 10f);
        }

        private void MakeUnavailable()
        {
            indicateRenderer.material.SetColor(EmissionColor, Color.red * 10f);
        }

        #endregion

        #region Color Checking

        private void CheckColor()
        {
            var currentBus = GameManager.Instance.busManager.GetCurrentBus();

            if (currentBus.colorType == colorType)
            {
                currentBus.CustomerAssign(this);
            }
            else if (GameManager.Instance.standManager.CheckForAvailableStand(this, out var stand))
            {
                _ = SetDestination(stand.position);
                onStand = true;
                GameManager.Instance.busManager.NewBusArrived += CheckIfNewBusIsSameColor;
            }

            MakeUnavailable();
        }

        private void CheckIfNewBusIsSameColor()
        {
            var currentBus = GameManager.Instance.busManager.GetCurrentBus();

            GameManager.Instance.standManager.standingPeople.Remove(this);
            
            if (currentBus.colorType != colorType) return;
            currentBus.CustomerAssign(this);
            GameManager.Instance.busManager.NewBusArrived -= CheckIfNewBusIsSameColor;
        }

        #endregion

        #region Animations

        public async Task SpawnAnimation(Vector3 gridPosition)
        {
            transform.localScale = Vector3.zero;
            transform.position = gridPosition;

            await transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).AsyncWaitForCompletion();
        }

        #endregion

        public void ActivateObstacle(bool isActive)
        {
            obstacle.SetActive(isActive);
        }
    }
}
