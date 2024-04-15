
using System.Collections.Generic;
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
            go.name = "Doodad-" + idx;
            return go;
        }

        public GameObject CreateAutomaton(string name, Vector3 startPos, Quaternion startRot,
            float wingPower, float torsoPower, float firePower)
        {
            if (automatonProto == null)
            {
                return null;
            }
            GameObject go = Instantiate(automatonProto, startPos, startRot);
            go.name = name;
            var zc = go.GetComponent<ZumCombatant>();
            if (zc != null)
            {
                zc.SetCombatStats(wingPower, torsoPower, firePower);
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