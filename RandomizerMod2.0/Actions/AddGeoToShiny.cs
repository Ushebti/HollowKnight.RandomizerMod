﻿using System;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using RandomizerMod.Extensions;
using RandomizerMod.FsmStateActions;

namespace RandomizerMod.Actions
{
    [Serializable]
    public class AddGeoToShiny : RandomizerAction
    {
        [SerializeField] private string sceneName;
        [SerializeField] private string objectName;
        [SerializeField] private string fsmName;
        [SerializeField] private string boolName;
        [SerializeField] private int geoAmount;

        public AddGeoToShiny(string sceneName, string objectName, string fsmName, string boolName, int geoAmount)
        {
            this.sceneName = sceneName;
            this.objectName = objectName;
            this.fsmName = fsmName;
            this.boolName = boolName;
            this.geoAmount = geoAmount;
        }

        public override void Process()
        {
            if (GameManager.instance.GetSceneNameString() == sceneName)
            {
                foreach (PlayMakerFSM fsm in fsmList)
                {
                    if (fsm.FsmName == fsmName && fsm.gameObject.name == objectName)
                    {
                        FsmState pdBool = fsm.GetState("PD Bool?");
                        FsmState charm = fsm.GetState("Charm?");

                        //Remove actions that stop shiny from spawning
                        pdBool.RemoveActionsOfType<PlayerDataBoolTest>();
                        pdBool.RemoveActionsOfType<StringCompare>();

                        //Add our own check to stop the shiny from being grabbed twice
                        pdBool.AddAction(new RandomizerBoolTest(boolName, null, "COLLECTED"));

                        //The "Charm?" state is a good entry point for our geo spawning
                        charm.AddAction(new RandomizerSetBool(boolName, true));
                        charm.AddAction(new RandomizerAddGeo(fsm.gameObject, geoAmount));

                        //Skip all the other type checks
                        charm.ClearTransitions();
                        charm.AddTransition("FINISHED", "Flash");

                        //Changes have been made, stop looping
                        break;
                    }
                }
            }
        }
    }
}
