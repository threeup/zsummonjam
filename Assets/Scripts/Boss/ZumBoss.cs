
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;

namespace zum
{
    public enum BossStateType
    {
        NACENT,
        SETUP_WORLD,
        SETUP_ACTORS,
        ACTIVE,
        GAMEOVER,
    }

    public class ZumBoss : ZapoLazySingleton<ZumBoss>
    {
        public List<ZumController> Controllers = new List<ZumController>();
        public int PendingPlayers = 0;
        public List<GameObject> Minerals = new List<GameObject>();
        public List<ZumDoodad> Doodads = new List<ZumDoodad>();
        public ZapoStateMach<BossStateType> BossMachine = new ZapoStateMach<BossStateType>();

        public float TimeInState() { return BossMachine.timeInState; }
        public bool IsActive() { return BossMachine.IsState(BossStateType.ACTIVE); }


        public int TargetMinerals = 80;
        public int TargetLocalPlayers = 1;
        public int TargetTotalPlayers = 2;
        public bool HasEnoughMinerals() { return Minerals.Count >= TargetMinerals; }
        public bool HasEnoughPawns() { return Controllers.FindAll(x => x.PossessedPawn == null).Count == 0; }

        private CinemachineVirtualCamera _cam;
        public bool IsMineralMaking;

        private bool _hasFactoryOrigin = false;
        private Vector3 _factoryOriginPos;
        private Vector3 _factoryOriginDir;

        public void Setup(CinemachineVirtualCamera cam)
        {
            _cam = cam;
        }
        public void RegisterController(ZumController ctrlr)
        {
            PendingPlayers--;
            Controllers.Add(ctrlr);
            if (ctrlr is ZumPlayerController zpc)
            {
                zpc.cam = _cam;
            }
        }

        public void DeregisterController(ZumController ctrlr)
        {
            Controllers.Remove(ctrlr);
        }

        public void Awake()
        {
            BossMachine.AdvanceMap = new Dictionary<BossStateType, BossStateType>{
                {BossStateType.NACENT, BossStateType.SETUP_WORLD},
                {BossStateType.SETUP_WORLD, BossStateType.SETUP_ACTORS},
                {BossStateType.SETUP_ACTORS, BossStateType.ACTIVE},
                {BossStateType.GAMEOVER, BossStateType.SETUP_WORLD}
            };
            BossMachine.Initialize(this);
            //DontDestroyOnLoad(gameObject);
            ZumBossNacentState.Bind(BossMachine.GetStateByType(BossStateType.NACENT));
            ZumBossSetupWorldState.Bind(BossMachine.GetStateByType(BossStateType.SETUP_WORLD));
            ZumBossSetupActorsState.Bind(BossMachine.GetStateByType(BossStateType.SETUP_ACTORS));
            ZumBossActiveState.Bind(BossMachine.GetStateByType(BossStateType.ACTIVE));
            ZumBossGameOverState.Bind(BossMachine.GetStateByType(BossStateType.GAMEOVER));
        }

        public void StoreFactoryOrigin(Transform t)
        {
            if (!_hasFactoryOrigin)
            {
                _hasFactoryOrigin = true;
                _factoryOriginPos = t.position;
                _factoryOriginDir = t.forward;
            }
        }

        public void ResetFactoryPlace(float yPos)
        {
            Vector3 nextPos = new Vector3(_factoryOriginPos.x, yPos, _factoryOriginPos.z);
            ZumFactory.Instance.SetPlace(nextPos, _factoryOriginDir);
        }

        public bool MakeMinerals(int max)
        {
            if (IsMineralMaking)
            {
                return false;
            }
            StartCoroutine(MineralMaker(max));
            return true;
        }

        IEnumerator MineralMaker(int max)
        {

            IsMineralMaking = true;
            float forwardAmount = Random.Range(-1.0f, 1.0f);
            float rightAmount = Random.Range(-1.0f, 1.0f);
            Vector3 stepDir = Vector3.forward * forwardAmount + Vector3.right * rightAmount;
            float stepDistance = 20.0f / max;
            for (int i = 0; i < max; ++i)
            {
                int attempts = 0;
                RaycastHit? placeHit = null;
                ZumFactory.Instance.StepPlaceByDir(stepDir, stepDistance);
                while (placeHit == null && attempts++ < 20)
                {
                    ZumFactory.Instance.RandomizePlaceFacing();

                    placeHit = ZumFactory.Instance.GetPlacePointingHit(4.0f);
                }
                if (placeHit is RaycastHit hit)
                {
                    Vector3 mineralPos = hit.point;
                    Vector3 mineralDir = hit.normal;
                    GameObject go = ZumFactory.Instance.CreateMineral(mineralPos, mineralDir);
                    Minerals.Add(go);
                }
                else
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        ResetFactoryPlace(2.0f);
                    }
                    else
                    {
                        ResetFactoryPlace(5.5f);
                    }

                }
                yield return new WaitForSeconds(.02f);
            }

            IsMineralMaking = false;
            yield return null;
        }

        public void DestroyMinerals()
        {
            for (int i = Minerals.Count - 1; i >= 0; ++i)
            {
                Destroy(Minerals[i].gameObject);
            }
        }


        public void MakeHumanPlayers()
        {
            int playerCount = Controllers.FindAll(x => !x.IsNPC).Count + PendingPlayers;
            for (int i = playerCount; i < TargetLocalPlayers; ++i)
            {
                GameObject go = ZumFactory.Instance.CreateHuman();
                PendingPlayers++;
            }
        }
        public void MakeNPCPlayers()
        {
            int controllerCount = Controllers.Count + PendingPlayers;
            for (int i = controllerCount; i < TargetTotalPlayers; ++i)
            {
                ZumFactory.Instance.CreateNPC();
                PendingPlayers++;
            }
        }

        public void MakeDoodads()
        {
            float startH = 5.15f;
            Vector3[] startPos = {
                new(-18, startH, -10),
                new(0, startH, 17),
                new(18, startH, -10)
            };
            for (int i = 0; i < 3; ++i)
            {
                GameObject go = ZumFactory.Instance.CreateDoodad(startPos[i], i);
                Doodads.Add(go.GetComponent<ZumDoodad>());
            }
            ZapoHelpers.Shuffle(ref Doodads);
            Doodads[0].AssignTeam(ZumTeam.RED);
            Doodads[1].AssignTeam(ZumTeam.GREEN);
            Doodads[2].AssignTeam(ZumTeam.BLUE);
        }

        public void MakePawns()
        {
            List<ZumController> noPawns = Controllers.FindAll(zc => zc.PossessedPawn == null);
            noPawns.ForEach(zc => ZumFactory.Instance.CreatePawn(zc));
        }

        public void AssociatePawnsToTeams()
        {
            ZapoHelpers.Shuffle(ref Doodads);
            for (int i = 0; i < Controllers.Count; ++i)
            {
                ZumPawn zp = Controllers[i].PossessedPawn as ZumPawn;
                if (zp != null)
                {
                    ZumDoodad home = Doodads[i % 3];
                    zp.ResetTeamAssociation();
                    zp.SetTeamAssociation(home.Team, 0.5f);
                    zp.TeleportHome(home);
                }
            }
        }

        public ZumPawn GetRandomPawn()
        {
            List<ZumController> hasPawns = Controllers.FindAll(zc => zc.PossessedPawn != null);
            if (hasPawns.Count == 0)
            {
                return null;
            }
            int k = Random.Range(0, hasPawns.Count);
            return hasPawns[k].PossessedPawn as ZumPawn;
        }

        public void DestroyPawns()
        {
            foreach (var zc in Controllers)
            {
                if (zc.PossessedPawn != null)
                {
                    var dead = zc.PossessedPawn;
                    zc.Dispossess();
                    Destroy(dead.gameObject);
                }
            }
        }
        public void DestroyNPCs()
        {
            List<ZumController> purged = Controllers.FindAll(zc => zc.IsNPC);
            purged.ForEach(zc => Destroy(zc.gameObject));
        }

        public void Update()
        {
            BossMachine.MachineUpdate(Time.deltaTime);
        }

    }
}