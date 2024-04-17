
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using zapo;

namespace zum
{

    public class ZumFactory : ZapoSingleton<ZumFactory>
    {
        public GameObject npcCtrlrProto;
        public GameObject humanCtrlrProto;
        public GameObject pawnProto;
        public GameObject automatonProto;
        public GameObject doodadProto;
        public GameObject cursorProto;

        public List<GameObject> mineralsProto;

        public Cinemachine.CinemachineVirtualCamera cam;

        RaycastHit[] _rayResults = new RaycastHit[5];

        Collider[] _sphereColliders = new Collider[50];

        private List<GameObject> _sphereOverlapResult = new();

        public void Awake()
        {
            ZumBoss.Instance.Setup(cam);
        }

        public GameObject CreateHuman()
        {
            if (humanCtrlrProto == null)
            {
                return null;
            }
            GameObject go = Instantiate(humanCtrlrProto);
            go.name = "Human" + Time.timeSinceLevelLoad;
            if (go.TryGetComponent<ZumPlayerController>(out var zpc))
            {
                GameObject cursor = Instantiate(cursorProto);
                if (cursor.TryGetComponent<ZumThrowCursor>(out var tc))
                {
                    zpc.ThrowCursor = tc;
                    tc.SetStrengthAndForward(-1, Vector3.right);
                }
            }


            return go;
        }
        public GameObject CreateNPC()
        {
            if (npcCtrlrProto == null)
            {
                return null;
            }
            GameObject go = Instantiate(npcCtrlrProto);
            go.name = "NPC-" + ZapoDataHelper.GetRandomName();
            return go;
        }

        public GameObject CreatePawn(ZumController zc)
        {
            if (pawnProto == null)
            {
                return null;
            }
            GameObject go = Instantiate(pawnProto);
            go.name = "Pawn-" + zc.name;
            zc.transform.SetParent(go.transform);
            return go;
        }
        public GameObject CreateMineral(Vector3 pos, Vector3 dir)
        {
            if (mineralsProto.Count == 0)
            {
                return null;
            }
            int k = Random.Range(0, mineralsProto.Count);
            GameObject go = Instantiate(mineralsProto[k], pos, Quaternion.LookRotation(dir));
            go.name = "Mineral-" + pos.ToString();
            return go;
        }

        public GameObject CreateDoodad(Vector3 pos, int idx)
        {
            if (doodadProto == null)
            {
                return null;
            }
            Vector3 dir = -pos.normalized;
            GameObject go = Instantiate(doodadProto, pos, Quaternion.LookRotation(dir));
            //idk this up thing
            go.transform.rotation = Quaternion.LookRotation(Vector3.up, go.transform.up);
            go.name = "Doodad-" + idx;
            return go;
        }

        public GameObject CreateAutomaton(string name, Vector3 startPos, Quaternion startRot,
            float speed, float atkVsRed, float atkVsGreen, float atkVsBlue)
        {
            if (automatonProto == null)
            {
                return null;
            }
            GameObject go = Instantiate(automatonProto, startPos, startRot);
            go.name = name;
            if (go.TryGetComponent<ZumAutomaton>(out var za))
            {
                if (speed > 2.0f)
                {
                    // 5 becomes 0
                    // 2 becomes -1
                    float bonusLost = Mathf.Clamp((5.0f - speed) / 3.0f, 0.0f, 1.0f);
                    za.SetSpeed(3.0f - bonusLost);
                }
                else
                {
                    za.SetSpeed(Mathf.Max(speed, 0.5f));
                }
            }

            // square to decrease power, 0.9 => 0.81, 0.5 => 0.25
            float r = atkVsRed * atkVsRed;
            float g = atkVsGreen * atkVsGreen;
            float b = atkVsBlue * atkVsBlue;
            if (go.TryGetComponent<ZumCombatant>(out var zc))
            {
                zc.SetCombatStats(r, g, b);
            }
            ZumMaterial zm = go.GetComponentInChildren<ZumMaterial>();
            if (zm != null)
            {
                zm.SetTargetColor(r, g, b);
            }
            return go;
        }

        public RaycastHit? GetPlacePointingHit(float dist)
        {
            // Set the layer mask to default layers
            var layerMask = LayerMask.GetMask("Default");

            int hits = Physics.RaycastNonAlloc(transform.position, transform.forward, _rayResults, dist, layerMask);
            if (hits == 0)
            {
                return null;
            }
            return _rayResults[0];
        }

        public void StepPlaceByDir(Vector3 dir, float amount)
        {
            this.transform.position = this.transform.position + dir * amount;
        }

        public void RandomizePlaceFacing()
        {
            this.transform.forward = Random.onUnitSphere;
        }

        public void SetPlace(Vector3 pos, Vector3 dir)
        {
            this.transform.position = pos;
            this.transform.forward = dir;
        }

        public GameObject[] GetSphereOverlapsItem(Vector3 pos, float radius)
        {

            var layerMask = LayerMask.GetMask("Item");

            _sphereOverlapResult.Clear();
            int numColliders = Physics.OverlapSphereNonAlloc(pos, radius, _sphereColliders, layerMask);
            for (int i = 0; i < numColliders; i++)
            {
                _sphereOverlapResult.Add(_sphereColliders[i].gameObject);
            }
            return _sphereOverlapResult.ToArray();
        }

        public GameObject[] GetSphereOverlapsAutomaton(Vector3 pos, float radius)
        {

            var layerMask = LayerMask.GetMask("Automaton");

            _sphereOverlapResult.Clear();
            int numColliders = Physics.OverlapSphereNonAlloc(pos, radius, _sphereColliders, layerMask);
            for (int i = 0; i < numColliders; i++)
            {
                _sphereOverlapResult.Add(_sphereColliders[i].gameObject);
            }
            return _sphereOverlapResult.ToArray();
        }


    }
}