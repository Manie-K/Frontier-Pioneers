using System;
using System.Collections.Generic;
using MG_Utilities;

namespace FrontierPioneers.Gameplay.NPC
{
    /// <summary>
    /// Handles the management of all NPCs in the game.
    /// Every <see cref="NPCBrain"/> NPC should register and deregister itself with this manager
    /// </summary>
    public class NPCManager : Singleton<NPCManager>
    {
        readonly List<NPCBrain> _managedEntities = new();

        /// <param name="npcBrain"> NPC to be registered </param>
        public void RegisterNPC(NPCBrain npcBrain)
        {
            if(npcBrain != null && !_managedEntities.Contains(npcBrain))
            {
                _managedEntities.Add(npcBrain);
            }
        }
        
        /// <param name="npcBrain"> NPC to be unregistered </param>
        public void UnregisterNPC(NPCBrain npcBrain)
        {
            if(npcBrain != null && _managedEntities.Contains(npcBrain))
            {
                _managedEntities.Remove(npcBrain);
            }
        }
        
        void Update()
        {
            for(int i = 0; i < _managedEntities.Count; i++)
            {
                _managedEntities[i].OnUpdate();
            }
        }

        void FixedUpdate()
        {
            for(int i = 0; i < _managedEntities.Count; i++)
            {
                _managedEntities[i].OnFixedUpdate();
            }
        }
    }
}