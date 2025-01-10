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
using MelonLoader;
using GHPC.Event;
using CustomMissionUtility.Events;
using GHPC.Weapons.Artillery;

public class SupplyRun : CustomMission
{
    private float[] camo_wears = new float[] {
        0.66f,
        0.585f,
        0.69f
    };

    private float[] scorch_amounts = new float[] {
        0.53f,
        0f,
        0f
    };

    public override CustomMissionData MissionData
    {
        get
        {
            return new CustomMissionData()
            {
                Name = "Supply Run",
                Id = "atlas_supplyrun",

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

                PlayableFactions = new Faction[] { Faction.Blue },

                DescriptionBluFor = String.Join("\n\n",
                    "Situation: Your tank company has seen some heavy fighting and ammunition reserves are dangerously low. A supply convoy carrying ammunition intended for your company has gone radio silent. Your platoon has been dispatched to investigate their last known location",
                    "Mission: Determine the status of the supply convoy and secure the area",
                    "Enemy: Unknown",
                    "Friendly: 4x M60A1 RISE (P) in your platoon",
                    "Support: 1x M270 fire mission, 3x M109 smoke fire missions, 5x ",
                    "Other: Your platoon has already expended most of their APFSDS rounds"
                ),
            };
        }
    }

    static PlatoonData bmp2_platoon_1;
    static PlatoonData kilo;

    public override void UnitSpawnOrders()
    {
        CreateSpawnOrder(References.Vehicles.M60A1_RISE_Late, Faction.Blue);
    }

    public override IEnumerator MapMarkers(GameState _) {       
        MapController map = PlayerInput.Instance.MapController;
        map.AddManuallyUpdatedIcon(
            "Supply Convoy",
            GHPC.UI.Map.MapIconType.PointOfInterest,
            new Color(0f, 1f, 0f, 1f),
            Tools.StrToVec3("256.2155 138.4403 1133.697")
        );

        FireMissionManager man = CMU.FireMissionManager;

        ArtilleryBattery battery = new ArtilleryBattery();
        battery._displayName = "M270";
        battery._munitions = new GHPC.Weaponry.Artillery.BatteryMunitionsChoice[] { FireSupport.HE_155mm_battery_munition };
        battery._munitionTypes = new IndirectFireMunitionType[] { IndirectFireMunitionType.AntiPersonnel };
        battery._shots = 12;
        battery._onCallImpactDelay = 15f;
        battery._interShotDelaySeconds = 0.55f;
        battery._randomDispersionRadiusMeters = 70f;

        man.BlueArtilleryBatteries = new ArtilleryBattery[] { battery };

        man.Awake();

        /*
        GameObject event_holder = new GameObject();
        GhpcEvent e = event_holder.AddComponent<GhpcEvent>();
        MissionTimeTrigger mtt = event_holder.AddComponent<MissionTimeTrigger>();
        SystemMessageAction sma = event_holder.AddComponent<SystemMessageAction>();

        e._mode = GhpcEvent.EventTriggerOperatorMode.OR;
        e._label = "";
        mtt.DelaySeconds = 5f;
        sma._duration = 15f;
        sma._message = "HELLO WORLD";
        e.Awake();
        */

        /*
        MissionTimeTrigger mtt = e.Add<MissionTimeTrigger>();
        mtt.DelaySeconds = 5f; 
        */


        MissionEvent e = new MissionEvent(GhpcEvent.EventTriggerOperatorMode.OR);

        UnitTargetedTrigger utt = e.Add<UnitTargetedTrigger>();
        utt.SpawnPoints = new List<DynamicSpawnPoint>() { CMU.DynamicSpawnpointsSolo[2] };
        utt.Condition = UnitTargetedTrigger.TargetedType.Hit;
        utt.eventChecks = new List<GhpcEvent>();

        SystemMessageAction sma = e.Add<SystemMessageAction>();
        sma._duration = 15f;
        sma._message = "HELLO WORLD 2";

        e.m_event.Awake();


        foreach (Vehicle m60a1 in kilo.Units)
        {
            if (m60a1.FriendlyName == "M60A1 RISE (Passive)") continue;
            /*
            float camo_wear = camo_wears[UnityEngine.Random.Range(0, 2)];
            float scorch_amount = scorch_amounts[UnityEngine.Random.Range(0, 2)];

            MeshRenderer hull_mesh = m60a1.transform.Find("M60_mesh/M60_hull").GetComponent<MeshRenderer>();
            hull_mesh.materials[0].SetFloat("_CamoAmount", camo_wear);

            SkinnedMeshRenderer turret_mesh = m60a1.transform.Find("M60_mesh/M60 1").GetComponent<SkinnedMeshRenderer>();
            turret_mesh.materials[0].SetFloat("_CamoAmount", camo_wear);

            turret_mesh.materials[0].SetFloat("_ScorchAmount", scorch_amount);
            hull_mesh.materials[0].SetFloat("_ScorchAmount", scorch_amount);

            m60a1.transform.Find("M60_mesh").gameObject.GetComponent<HeatSource>().FetchSwapableMats();
            */

            LoadoutManager loadout_manager = m60a1.LoadoutManager;

            int apfsds_count = UnityEngine.Random.Range(2, 4);
            loadout_manager.TotalAmmoCounts[0] = apfsds_count;

            for (int i = 0; i < loadout_manager.RackLoadouts.Length; i++)
            {
                GHPC.Weapons.AmmoRack rack = loadout_manager.RackLoadouts[i].Rack;

                if (i == 0)
                {
                    var fixed_choices = new List<LoadoutManager.RackLoadoutFixedChoice>() { };

                    for (int j = 0; j < apfsds_count; j++)
                    {
                        fixed_choices.Add(new LoadoutManager.RackLoadoutFixedChoice()
                        {
                            AmmoClipIndex = 0,
                            RackSlotIndex = j
                        });
                    }

                    loadout_manager.RackLoadouts[i].FixedChoices = fixed_choices.ToArray();
                }

                Util.EmptyRack(rack);
            }

            loadout_manager.SpawnCurrentLoadout(true);
        }

        yield break;
    }


    public override void OnMissionStartedLoading()
    {
        //Tools.SetStartingUnit(kilo.Units[0], Faction.Blue);
        
        /*
        bmp2_platoon_1 = Tools.CreatePlatoon("Red BMP-2 Platoon 1",
            Tools.SpawnVehicle(References.Vehicles.Mi_8, Tools.StrToVec3("-546.2668 116.7953 1619.693"), Tools.StrToVec3("0 85.8523 0")),
            Tools.SpawnVehicle(References.Vehicles.Mi_8, Tools.StrToVec3("-571.6708 115.6059 1617.499"), Tools.StrToVec3("0 85.89 0")),
            Tools.SpawnVehicle(References.Vehicles.Mi_8, Tools.StrToVec3("-598.8732 114.2752 1616.985"), Tools.StrToVec3("0 92.0309 0")),
            Tools.SpawnVehicle(References.Vehicles.Mi_8, Tools.StrToVec3("-636.1891 113.6643 1617.958"), Tools.StrToVec3("0 91.8049 0"))
        );
        */
        /*
        Tools.SpawnVehicle(References.Vehicles.Mi24V_Soviet, Tools.StrToVec3("512.4258 110.3971 1237.638"), Tools.StrToVec3("0 110.9017 0"));
        Tools.SpawnVehicle(References.Vehicles.Mi24V_Soviet, Tools.StrToVec3("485.6556 112.7275 1207.139"), Tools.StrToVec3("0 105.54 0"));
        Tools.SpawnVehicle(References.Vehicles.Mi24V_Soviet, Tools.StrToVec3("467.4625 115.7883 1167.233"), Tools.StrToVec3("0 94.7434 0"));
        Tools.SpawnVehicle(References.Vehicles.Mi24V_Soviet, Tools.StrToVec3("460.5601 117.94 1136.411"), Tools.StrToVec3("0 87.4151 0"));
        Tools.SpawnVehicle(References.Vehicles.Mi24V_Soviet, Tools.StrToVec3("457.4448 121.4663 1087.274"), Tools.StrToVec3("0 82.3135 0"));
        */
    }

    public override IEnumerator OnMissionFinishedLoading(GameState _)
    {
        Tools.SpawnVehicle(References.Vehicles.T80B, Tools.StrToVec3("-40.7117 129.28 1335.091"), Tools.StrToVec3("0 106.4646 0"));

        kilo = Tools.CreatePlatoon("3rd Platoon, Kilo Company",
            Tools.SpawnVehicle(References.Vehicles.M1IP, Tools.StrToVec3("949.7223 96.5759 1050.867"), Tools.StrToVec3("352.0813 267.2843 0.3531")),
            Tools.SpawnVehicle(References.Vehicles.M1IP, Tools.StrToVec3("946.6692 96.8055 1076.604"), Tools.StrToVec3("352.5544 270.3184 359.7101")),
            Tools.SpawnVehicle(References.Vehicles.M1IP, Tools.StrToVec3("950.5035 96.1967 1029.84"), Tools.StrToVec3("352.3546 270.6481 1.0246")),
            Tools.SpawnVehicle(References.Vehicles.M1IP, Tools.StrToVec3("947.6934 96.501 1100.451"), Tools.StrToVec3("351.3318 270.2562 358.3939"))
        );

        yield break;
    }
}