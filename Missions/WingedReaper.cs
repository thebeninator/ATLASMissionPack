using System;
using CustomMissionUtility;
using UnityEngine;
using ATLASMissionPack;
using GHPC.State;
using GHPC.Camera;
using MelonLoader.Utils;
using System.IO;
using FMOD;

public class WingedReaper : CustomMission
{
    public override CustomMissionData MissionData
    {
        get
        {
            return new CustomMissionData()
            {
                Name = "Winged Reaper",
                Id = " ac130_mission_winged_reaper",
                DefaultTime = 125f,

                Theater = References.Theater.EasternHills,

                RedFor = false,
                BluFor = true,

                DescriptionBluFor = String.Join("\n\n",
                    "Situation: Reconnaissance aircraft have reported that a large enemy force is assembling in this region. You have been sent in to engage and destroy all enemy assets.",
                    "Mission: Destroy all enemy vehicles",
                    "Enemy: Unknown quantities of tanks, IFVs, APCs, and trucks",
                    "Friendly: 1x AC-130E",
                    "Gunship Loadout: 105mm howitzer M102, 40mm cannon L/60, 20mm rotary cannon M61"
                ),
            };
        }
    }

    public override void OnMissionStartedLoading() {
        if (!M61SoundManager.sounds_created)
        {
            var corSystem = FMODUnity.RuntimeManager.CoreSystem;

            corSystem.createSound(Path.Combine(MelonEnvironment.ModsDirectory + "/missioncreator", "m61burst.ogg"), MODE._3D_INVERSEROLLOFF | MODE.LOOP_NORMAL, out M61SoundManager.sound);
            M61SoundManager.sound.set3DMinMaxDistance(30f, 1300f);

            corSystem.createSound(Path.Combine(MelonEnvironment.ModsDirectory + "/missioncreator", "m61closing.ogg"), MODE._3D_INVERSEROLLOFF, out M61SoundManager.sound_closing);
            M61SoundManager.sound_closing.set3DMinMaxDistance(30f, 1300f);

            M61SoundManager.sounds_created = true;
        }

        AC130.Init();
        StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(AC130.SpawnAC130), GameStatePriority.Medium);
    }

    public override void OnMissionFinishedLoading()
    {
        Tools.SpawnVehicle(References.Vehicles.BMP1_NVA, new Vector3(1535.657f, 24.1292f, 1268.022f), new Vector3(1.4799f, 69.464f, 358.3769f));
        Tools.SpawnVehicle(References.Vehicles.BMP1_NVA, new Vector3(1635.186f, 24.5157f, 1289.566f), new Vector3(0.3177f, 259.1504f, 359.5566f));
        Tools.SpawnVehicle(References.Vehicles.BMP1_NVA, new Vector3(1625.462f, 25.9402f, 1271.911f), new Vector3(5.356f, 247.3495f, 1.0677f));

        Tools.SpawnVehicle(References.Vehicles.T72M1, new Vector3(2030.841f, 59.9901f, 968.3901f), new Vector3(0f, 286.6731f, 0f));
        Tools.SpawnVehicle(References.Vehicles.T72M1, new Vector3(2005.167f, 58.2442f, 973.3553f), new Vector3(0f, 240.6731f, 0f));

        AC130.ac130 = Tools.SpawnVehicle(References.Vehicles.M1IP, new Vector3(2005.167f, 58.2442f, 973.3553f), new Vector3(0f, 240.6731f, 0f)).gameObject;

        if (CameraManager._mainCamera.gameObject.GetComponent<CameraShake>() == null)
            CameraManager._mainCamera.gameObject.AddComponent<CameraShake>();
        CameraManager._mainCamera.transform.Find("Scope/Scope Sprite").localScale = Vector3.zero;
    }
}