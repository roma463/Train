using System;
using System.Collections.Generic;
using System.Linq;
using _Train.Scripts.Character;
using _Train.Scripts.Level.Items.Data;
using _Train.Scripts.Root;
using Mirror;
using UnityEngine;

namespace _Train.Scripts
{
    public class PickupObject : NetworkBehaviour, IGrabble
    {
        [SerializeField] protected NetworkTransformUnreliable networkTransformUnreliable;
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private Collider collider;
        [SerializeField] private LevelItem levelItem;
        
        [SerializeField] private List<GameObject> layerChangeableObjects = new List<GameObject>();

        public Rigidbody Rigidbody => rigidBody;
        public virtual string GrabAnimName => "Take";
        public virtual string DropAnimName => "Drop";
        public virtual string ThrowAnimName => "Throw";
        public virtual string InteractDoName => "Take";
        public LevelItem LevelItem => levelItem;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (layerChangeableObjects.Count == 0)
            {
                var meshRenderers = GetComponentsInChildren<MeshRenderer>();
                layerChangeableObjects = meshRenderers.Select(p=>p.gameObject).ToList();
                // UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            }
        }
#endif
        
        public override void OnStartClient()
        {
            rigidBody.isKinematic = !NetworkServer.active;
        }

        public virtual bool CanThrow()
        {
            return true;
        }

        public virtual bool CanAttach()
        {
            return true;
        }

        [Server]
        private void CmdGrab()
        {
            rigidBody.isKinematic = true;
            RemoveVelocity();
            RpcGrab();
            
            CmdGrabBefore();
        }

        [Server]
        public void RemoveVelocity()
        {
            rigidBody.linearVelocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }

        protected virtual void CmdGrabBefore()
        {
            
        }

        [ClientRpc]
        private void RpcGrab()
        {
            networkTransformUnreliable.enabled = false;
            collider.enabled = false;
            RpcAfterGrab();
        }

        protected virtual void RpcAfterGrab()
        {
            
        }

        public virtual bool CanGrab()
        {
            return true;
        }

        public void Grab(Character.Character character)
        {
            if (isServer)
            {
                CmdAddAuthority(character.connectionToClient);
                CmdGrab();
            }
        }

        public void Drop()
        {
            if (isServer)
                CmdDrop();
        }

        public void Throw(Vector3 direction, float force)
        {
            CmdDrop();
            CmdThrow(direction, force);
        }

        public void ChangeLayer(string newLayer)
        {
            foreach (var obj in layerChangeableObjects)
            {
                obj.layer = LayerMask.NameToLayer(newLayer);
            }
        }

        [Server]
        private void CmdThrow(Vector3 direction, float force)
        {
            rigidBody.AddForce(direction * force, ForceMode.Impulse);
        }

        [Server]
        private void CmdAddAuthority(NetworkConnectionToClient conn)
        {
            netIdentity.AssignClientAuthority(conn);
        }

        [Server]
        private void CmdDrop()
        {
            netIdentity.RemoveClientAuthority();
            rigidBody.isKinematic = false;
            RpcDrop();
            
            CmdAfterDrop();
        }

        protected virtual void CmdAfterDrop()
        {
            
        }

        [ClientRpc]
        private void RpcDrop()
        {
            networkTransformUnreliable.enabled = true;
            collider.enabled = true;
            RpcAfterDrop();
        }
        
        protected virtual void RpcAfterDrop()
        {
            
        }
    }
}
