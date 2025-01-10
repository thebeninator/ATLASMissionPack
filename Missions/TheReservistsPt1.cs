/*
using System;
using CustomMissionUtility;
using UnityEngine;
using ATLASMissionPack;
using GHPC.State;
using GHPC.Vehicle;
using GHPC;
using GHPC.Mission;
using Thermals;
using GHPC.AI.Platoons;
using System.Collections;
using GHPC.Weapons;
using System.Collections.Generic;
using GHPC.UI;
using GHPC.Player;

public class TheReservistsPt1 : CustomMission
{
    public override CustomMissionData MissionData
    {
        get
        {
            return new CustomMissionData()
            {
                Name = "Meet the Reservists",
                Id = "atlas_mtr",

                CloudBias = 0f,

                TimeOptions = new RandomEnvironment.EnvSettingOption[] {
                    new RandomEnvironment.EnvSettingOption()
                    {
                        Time = 182f,
                        RandomWeight = 1
                    },
                    new RandomEnvironment.EnvSettingOption()
                    {
                        Time = 90f,
                        RandomWeight = 1
                    },
                },

                Theater = References.Theater.PointAlpha,

                RedFor = true,
                BluFor = false,

                DescriptionBluFor = String.Join("\n\n",
                    "Situation: You and the lads were having a nice coffee break up until your CO suddenly kicked the door off its hinges (knocking down a support beam causing the roof to partially collapse on your driver) and gave orders for your company to attack an enemy-held town.",
                    "Mission: Assault and completely clear out the enemy-held town",
                    "Enemy: 1x company(-) of M60A1s, several ATGM teams",
                    "Friendly: 1x T-54A platoon with attached PT-76B section; BM-21s are available for fire support missions",
                    "Other: N/A"
                ),
            };
        }
    }

    public override IEnumerator MapMarkers(GameState _)
    {
        MapController map = PlayerInput.Instance.MapController;
        map.AddManuallyUpdatedIcon(
            "Supply Convoy",
            GHPC.UI.Map.MapIconType.PointOfInterest,
            new Color(0f, 1f, 0f, 1f),
            Tools.StrToVec3("-437.5235 124.2177 1684.164")
        );
        yield break;
    }


    public override void OnMissionStartedLoading()
    {
    }

    public override void OnMissionFinishedLoading()
    {
    }
}*/