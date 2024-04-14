
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
        public GameObject minionProto;

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

        public GameObject CreateMinion(ZumPawn pawn, float legPower, float torsoPower, float armPower)
        {
            if (minionProto == null)
            {
                return null;
            }
            GameObject go = Instantiate(minionProto);
            go.name = "Minion-" + pawn.name;
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

    }
}