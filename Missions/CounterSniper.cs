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

public class CounterSniper : CustomMission
{
    public override CustomMissionData MissionData
    {
        get
        {
            return new CustomMissionData()
            {
                Name = "Counter Sniper",
                Id = "atlas_counter_sniper",

                CloudBias = 0.8f,

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
                    "Situation: Enemy tanks have taken up long-range positions amongst the trees and are preventing us from advancing.",
                    "Mission: Eliminate all enemy tanks",
                    "Enemy: 6x M60A1 RISE (Passive)",
                    "Friendly: 3x T-62, 2x BMP-1",
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