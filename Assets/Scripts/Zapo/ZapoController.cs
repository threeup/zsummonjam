using UnityEngine;


namespace zapo
{
    public class ZapoController : MonoBehaviour
    {
        [Header("General")]
        public Vector2 MoveVec;
        public Vector2 LookVec;
        public bool WantsSprint;
        public bool WantsJump;
        public ZapoPawn PossessedPawn;

        public virtual void Possess(ZapoPawn p)
        {
            PossessedPawn = p;
            if (p != null)
            {
                p.PossessedBy(this);
            }
        }

        public virtual void Dispossess()
        {
            if (PossessedPawn != null)
            {
                PossessedPawn.PossessedBy(null);
                PossessedPawn = null;
            }
        }

        public virtual bool IsNPC
        {
            get
            {
                return true;
            }
        }

        public virtual bool IsCurrentDeviceMouse
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsAnalogMovement
        {
            get
            {
                return false;
            }
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            if (PossessedPawn == null)
            {
                var foundPawn = GetComponentInParent<ZapoPawn>();
                if (foundPawn != null)
                {
                    Possess(foundPawn);
                }
            }
            ReadInputs();
        }

        protected virtual void ReadInputs()
        {
            //todo
        }

    }
}