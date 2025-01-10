using System.Collections;
using System.Collections.Generic;
using GHPC.Equipment.Optics;
using GHPC.State;
using GHPC.Vehicle;
using GHPC.Weapons;
using NWH.VehiclePhysics;
using Reticle;
using UnityEngine.UI;
using UnityEngine;
using HarmonyLib;

namespace ATLASMissionPack
{
    public class AC130
    {
        public static GameObject ac130;

        static WeaponSystemCodexScriptable gun_m102;

        static AmmoClipCodexScriptable clip_codex_m1;
        static AmmoType.AmmoClip clip_m1;
        static AmmoCodexScriptable ammo_codex_m1;
        static AmmoType ammo_m1;

        static AmmoClipCodexScriptable clip_codex_m81a1;
        static AmmoType.AmmoClip clip_m81a1;
        static AmmoCodexScriptable ammo_codex_m81a1;
        static AmmoType ammo_m81a1;

        static AmmoType ammo_m456;
        static AmmoType ammo_m791;
        static AmmoType ammo_m792;

        public class AC130Pivot : MonoBehaviour
        {
            void Update()
            {
                transform.Rotate(0f, 2f * Time.deltaTime, 0f);
            }
        }

        static List<List<Vector3>> borders = new List<List<Vector3>>() {
            new List<Vector3> {new Vector3(0f, -420f, 0f), new Vector3(0f, 0f, 180f)},
            new List<Vector3> {new Vector3(0f, 420f, 0f), new Vector3(0f, 0f, 0f)},
            new List<Vector3> {new Vector3(520f, 0f, 0f), new Vector3(0f, 0f, 270f)},
            new List<Vector3> {new Vector3(-520f, 0f, 0f), new Vector3(0f, 0f, 90f)}
        };

        static GameObject box_canvas;
        static ReticleSO reticleSO;
        static ReticleMesh.CachedReticle reticle_cached;

        static ReticleSO reticleSO_night = new ReticleSO();
        static ReticleMesh.CachedReticle reticle_cached_night;

        public class AlwaysLase : MonoBehaviour
        {
            FireControlSystem fcs;
            void Awake()
            {
                fcs = GetComponent<FireControlSystem>();
            }

            void Update()
            {
                fcs.Lase();
            }
        }

        [HarmonyPatch(typeof(GHPC.Weapons.FireControlSystem), "DoLase")]
        public static class StabCorrection
        {
            private static void Prefix(GHPC.Weapons.FireControlSystem __instance)
            {
                if (__instance.GetComponent<AlwaysLase>() != null)
                    __instance.CurrentStabMode = StabilizationMode.Vector;
            }

            private static void Postfix(GHPC.Weapons.FireControlSystem __instance)
            {
                if (__instance.GetComponent<AlwaysLase>() != null)
                    __instance.CurrentStabMode = StabilizationMode.WorldPoint;
            }
        }

        public static IEnumerator SpawnAC130(GameState _)
        {
            GameObject pivot = new GameObject("AC130 PIVOT");
            pivot.AddComponent<AC130Pivot>();
            pivot.transform.position = new Vector3(1601.238f, 22.983f, 1221.3f);
            pivot.transform.localEulerAngles = Vector3.zero;

            ac130.transform.parent = pivot.transform;
            ac130.transform.localPosition = new Vector3(-429.9814f, 900.1295f, 1400.8975f);
            ac130.transform.localEulerAngles = Vector3.zero;
            ac130.GetComponent<Rigidbody>().useGravity = false;
            ac130.GetComponent<VehicleController>().enabled = false;

            Vehicle vic = ac130.GetComponent<Vehicle>();
            vic._friendlyName = "AC-130E";
            vic.transform.Find("US Tank Voice").gameObject.SetActive(false);

            WeaponSystem main_gun = vic.WeaponsManager.Weapons[0].Weapon;
            main_gun.WeaponSound.SingleShotEventPaths[0] = "event:/Weapons/canon_125mm-2A46";
            main_gun.Impulse = 0f;
            main_gun.CodexEntry = gun_m102;
            main_gun.name = "105mm howitzer M102";
            main_gun.FCS.SuperleadWeapon = false;
            main_gun.FCS.CurrentStabMode = StabilizationMode.WorldPoint;
            main_gun.FCS.ImperfectFixedRangePointStab = false;
            main_gun.FCS.PointStabAffectsLead = true;
            main_gun.FCS._fixParallaxForVectorMode = false;
            main_gun.RecoilBlurMultiplier = 1.66f;
            main_gun.BaseDeviationAngle = 0.03f;

            main_gun.FCS.gameObject.AddComponent<AlwaysLase>();

            vic.AimablePlatforms[0].LocalEulerLimits = new Vector2(-50f, 30f);
            vic.AimablePlatforms[1].SpeedPowered = 60f;

            vic.AimablePlatforms[0]._stabMode = StabilizationMode.WorldPoint;
            vic.AimablePlatforms[1]._stabMode = StabilizationMode.WorldPoint;

            UsableOptic optic = vic.transform.Find("IPM1_rig/HULL/TURRET/Turret Scripts/GPS/Optic").GetComponent<UsableOptic>();
            vic.transform.Find("IPM1_rig/HULL/TURRET/Turret Scripts/GPS/Optic/Abrams GPS canvas").gameObject.SetActive(false);
            optic.CantCorrectMaxSpeed = 9999f;
            optic.LocalElevationLimits = new Vector2(-50f, 30f);
            optic.RotateAzimuth = true;
            optic.Alignment = OpticAlignment.FcsRange;
            optic.ForceHorizontalReticleAlign = true;
            optic.slot.OtherFovs = new float[] { };

            UsableOptic night_optic = optic.slot.LinkedNightSight.PairedOptic;
            vic.transform.Find("IPM1_rig/HULL/TURRET/Turret Scripts/GPS/FLIR/Abrams GPS canvas (1)").gameObject.SetActive(false);
            night_optic.CantCorrectMaxSpeed = 9999f;
            night_optic.LocalElevationLimits = new Vector2(-50f, 30f);
            night_optic.RotateAzimuth = true;
            night_optic.Alignment = OpticAlignment.FcsRange;
            night_optic.ForceHorizontalReticleAlign = true;
            night_optic.slot.BaseBlur = 0.15f;
            night_optic.slot.OtherFovs = new float[] { };
            night_optic.FovLimitedItems = new UsableOptic.FovLimitedItem[] { };
            night_optic.reticleMesh = night_optic.slot.transform.Find("Reticle Mesh").GetComponent<ReticleMesh>();
            night_optic.slot.transform.Find("Reticle Mesh WFOV").gameObject.SetActive(false);

            vic.transform.Find("IPM1_rig/HULL/TURRET/Turret Scripts/GPS/FLIR/Canvas Scanlines").gameObject.SetActive(false);
            vic.transform.Find("IPM1_rig/HULL/TURRET/Turret Scripts/GPS/FLIR/Scanline FOV change").gameObject.SetActive(false);

            //ColorGrading color_grading;
            //night_optic.post.profile.TryGetSettings(out color_grading);
            //color_grading.colorFilter.value = new Color(1f, 1f, 1f);

            if (!reticleSO)
            {
                reticleSO = ScriptableObject.Instantiate(ReticleMesh.cachedReticles["M1A1_generic"].tree);
                reticleSO.name = "ac130 day";

                Util.ShallowCopy(reticle_cached, ReticleMesh.cachedReticles["M1A1_generic"]);
                reticle_cached.tree = reticleSO;

                reticle_cached.tree.lights = new List<ReticleTree.Light>()
                {
                    new ReticleTree.Light()
                };

                reticle_cached.tree.lights[0].color = new RGB(1, 1, 1, false);
                reticle_cached.tree.lights[0].type = ReticleTree.Light.Type.Powered;

                ReticleTree.Angular angular = (reticle_cached.tree.planes[0].elements[0] as ReticleTree.Angular);
                angular.align = ReticleTree.GroupBase.Alignment.Impact;

                ReticleTree.Angular crosshair_angular = angular.elements[0] as ReticleTree.Angular;
                crosshair_angular.elements.Clear();

                for (int i = -1; i <= 1; i += 2)
                {
                    ReticleTree.Line line_vert = new ReticleTree.Line(
                        position: new Vector2(0f, 18f * i), degrees: 90f, thickness: 0.25f, roundness: 0f, length: 22f);
                    line_vert.visualType = ReticleTree.VisualElement.Type.Painted;
                    line_vert.illumination = ReticleTree.Light.Type.Powered;
                    crosshair_angular.elements.Add(line_vert);

                    ReticleTree.Line dot = new ReticleTree.Line(
                        position: new Vector2(0f, 3f * i), degrees: 0f, thickness: 0.4f, roundness: 0f, length: 0.4f);
                    dot.visualType = ReticleTree.VisualElement.Type.Painted;
                    dot.illumination = ReticleTree.Light.Type.Powered;
                    crosshair_angular.elements.Add(dot);

                    float[] blip_pos = new float[2] { 29f, 16f };
                    foreach (float f in blip_pos)
                    {
                        ReticleTree.Line blip = new ReticleTree.Line(
                             position: new Vector2(0f, f * i), degrees: 0f, thickness: 0.25f, roundness: 0f, length: 5f);
                        blip.visualType = ReticleTree.VisualElement.Type.Painted;
                        blip.illumination = ReticleTree.Light.Type.Powered;
                        crosshair_angular.elements.Add(blip);
                    }
                }

                for (int i = -1; i <= 1; i += 2)
                {
                    ReticleTree.Line line_horz = new ReticleTree.Line(
                        position: new Vector2(24f * i, 0f), degrees: 0f, thickness: 0.25f, roundness: 0f, length: 35f);
                    line_horz.visualType = ReticleTree.VisualElement.Type.Painted;
                    line_horz.illumination = ReticleTree.Light.Type.Powered;
                    crosshair_angular.elements.Add(line_horz);

                    ReticleTree.Line dot = new ReticleTree.Line(
                        position: new Vector2(3f * i, 0f), degrees: 0f, thickness: 0.4f, roundness: 0f, length: 0.4f);
                    dot.visualType = ReticleTree.VisualElement.Type.Painted;
                    dot.illumination = ReticleTree.Light.Type.Powered;
                    crosshair_angular.elements.Add(dot);

                    float[] blip_pos = new float[3] { 41.5f, 28f, 15f };
                    foreach (float f in blip_pos)
                    {
                        ReticleTree.Line blip = new ReticleTree.Line(
                        position: new Vector2(f * i, 0f), degrees: 90f, thickness: (f == 41.5f ? 0.40f : 0.25f), roundness: 0f, length: 5f);
                        blip.visualType = ReticleTree.VisualElement.Type.Painted;
                        blip.illumination = ReticleTree.Light.Type.Powered;
                        crosshair_angular.elements.Add(blip);
                    }
                }

                reticleSO_night = ScriptableObject.Instantiate(ReticleMesh.cachedReticles["M1-TIS"].tree);
                reticleSO_night.name = "ac130 night";

                Util.ShallowCopy(reticle_cached_night, ReticleMesh.cachedReticles["M1-TIS"]);
                reticle_cached_night.tree = reticleSO_night;

                reticle_cached_night.tree.lights = new List<ReticleTree.Light>()
                {
                    new ReticleTree.Light()
                };

                reticle_cached_night.tree.lights[0].color = new RGB(1, 1, 1, false);
                reticle_cached_night.tree.lights[0].type = ReticleTree.Light.Type.Powered;

                ReticleTree.Angular angular_night = (reticle_cached_night.tree.planes[0].elements[0] as ReticleTree.Angular);
                angular_night.align = ReticleTree.GroupBase.Alignment.Impact;

                angular_night.elements = angular.elements;
            }

            optic.reticleMesh.reticleSO = reticleSO;
            optic.reticleMesh.reticle = reticle_cached;
            optic.reticleMesh.SMR = null;
            optic.reticleMesh.Load();

            night_optic.reticleMesh.reticleSO = reticleSO_night;
            night_optic.reticleMesh.reticle = reticle_cached_night;
            night_optic.reticleMesh.SMR = null;
            night_optic.reticleMesh.Load();

            SharedZoomHandler digital_enhance = main_gun.FCS.gameObject.AddComponent<SharedZoomHandler>();
            digital_enhance.day = optic.slot;
            digital_enhance.night = night_optic.slot;
            digital_enhance.day_reticle_plane = optic.slot.transform.Find("Reticle Mesh/FFP");
            digital_enhance.night_reticle_plane = night_optic.slot.transform.Find("Reticle Mesh/FFP");

            for (float i = optic.slot.DefaultFov - 0.5f; i > 3f; i -= 0.5f)
            {
                digital_enhance.Add(i, 0f, i / optic.slot.DefaultFov);
            }
            digital_enhance.Add(3f, 0f, 3f / optic.slot.DefaultFov);

            LoadoutManager loadout_manager = vic.GetComponent<LoadoutManager>();
            loadout_manager.LoadedAmmoTypes = new AmmoClipCodexScriptable[] { clip_codex_m1 };
            loadout_manager.TotalAmmoCounts = new int[] { 40 };
            loadout_manager._totalAmmoTypes = 1;
            loadout_manager._totalAmmoCount = 40;

            for (int i = 0; i <= 2; i++)
            {
                GHPC.Weapons.AmmoRack rack = loadout_manager.RackLoadouts[i].Rack;
                rack.ClipTypes = new AmmoType.AmmoClip[] { clip_codex_m1.ClipType };
                Util.EmptyRack(rack);
            }

            WeaponSystem coax = vic.GetComponent<WeaponsManager>().Weapons[1].Weapon;
            vic.GetComponent<WeaponsManager>().Weapons[1].Name = "40mm cannon L/60";
            coax.Impulse = 0f;
            coax.RecoilBlurMultiplier = 0f;
            //coax.BaseDeviationAngle = 0.045f;
            coax.BaseDeviationAngle = 0.2f;
            coax.SetCycleTime(0.0195f);
            coax.Feed._totalCycleTime = 0.007f;
            coax.WeaponSound.SingleShotEventPaths = new string[] { "m61" };
            coax.WeaponSound.SingleShotMode = true;
            coax.Feed.AmmoTypeInBreech = null;
            coax.Feed.ReadyRack.ClipTypes[0] = clip_codex_m81a1.ClipType;
            coax.Feed.ReadyRack.Awake();
            coax.Feed.Start();
            M61SoundManager sound_manager = coax.gameObject.AddComponent<M61SoundManager>();
            sound_manager.audio_origin = coax.WeaponSound.transform;

            loadout_manager.SpawnCurrentLoadout();
            main_gun.Feed.AmmoTypeInBreech = null;
            main_gun.Feed.Start();
            loadout_manager.RegisterAllBallistics();

            foreach (MeshRenderer rend in ac130.GetComponentsInChildren<MeshRenderer>())
            {
                rend.gameObject.SetActive(false);
            }

            foreach (SkinnedMeshRenderer rend in ac130.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (rend.name == "Reticle Mesh") continue;
                rend.gameObject.SetActive(false);
            }

            for (int i = 0; i <= 3; i++)
            {
                GameObject t = GameObject.Instantiate(box_canvas, optic.transform);
                t.transform.GetChild(0).localPosition = borders[i][0];
                t.transform.GetChild(0).localEulerAngles = borders[i][1];
                if (i == 2 || i == 3)
                    t.GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
                t.SetActive(true);

                t = GameObject.Instantiate(box_canvas, night_optic.transform);
                t.transform.GetChild(0).localPosition = borders[i][0];
                t.transform.GetChild(0).localEulerAngles = borders[i][1];
                if (i == 2 || i == 3)
                    t.GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
                t.SetActive(true);
            }

            yield break;
        }

        public static void Init()
        {
            foreach (Vehicle obj in Resources.FindObjectsOfTypeAll(typeof(Vehicle)))
            {
                if (obj.name == "M2 Bradley")
                {
                    box_canvas = GameObject.Instantiate(obj.transform.Find("FCS and sights/GPS Optic/M2 Bradley GPS canvas").gameObject);
                    GameObject.Destroy(box_canvas.transform.GetChild(2).gameObject);
                    box_canvas.SetActive(false);
                    box_canvas.hideFlags = HideFlags.DontUnloadUnusedAsset;
                    box_canvas.name = "boxy";
                    break;
                }
            }

            if (gun_m102 == null)
            {
                foreach (AmmoCodexScriptable s in Resources.FindObjectsOfTypeAll(typeof(AmmoCodexScriptable)))
                {
                    if (s.AmmoType.Name == "M456 HEAT-FS-T") ammo_m456 = s.AmmoType;
                    if (s.AmmoType.Name == "25mm APDS-T M791") ammo_m791 = s.AmmoType;
                    if (s.AmmoType.Name == "25mm HEI-T M792") ammo_m792 = s.AmmoType;

                    if (ammo_m456 != null && ammo_m791 != null && ammo_m792 != null) break;
                }

                gun_m102 = ScriptableObject.CreateInstance<WeaponSystemCodexScriptable>();
                gun_m102.name = "gun_m102";
                gun_m102.CaliberMm = 105;
                gun_m102.FriendlyName = "105mm howitzer M102";
                gun_m102.Type = WeaponSystemCodexScriptable.WeaponType.LargeCannon;

                ammo_m1 = new AmmoType();
                Util.ShallowCopy(ammo_m1, ammo_m456);
                ammo_m1.Name = "M1/M557 HE-PD";
                ammo_m1.AlwaysProduceBlast = true;
                ammo_m1.ShatterOnRicochet = false;
                ammo_m1.Category = AmmoType.AmmoCategory.Explosive;
                ammo_m1.Caliber = 105;
                ammo_m1.RhaPenetration = 40f;
                ammo_m1.MuzzleVelocity = 600f; // actual mv is ~500 m/s but makes the howitzer a lot more finicky to use 
                ammo_m1.Mass = 0.855f;
                ammo_m1.SpallMultiplier = 1.35f;
                ammo_m1.TntEquivalentKg = 15.66f; // overkill b/c fun
                ammo_m1.DetonateSpallCount = 120;
                ammo_m1.MaxSpallRha = 55f;
                ammo_m1.MinSpallRha = 20f;
                ammo_m1.UseTracer = false;
                ammo_m1.Coeff = 0.0f;
                //ammo_m1.ImpactFuseTime = 0.05f;
                ammo_m1.ImpactEffectDescriptor = new GHPC.Effects.ParticleEffectsManager.ImpactEffectDescriptor()
                {
                    HasImpactEffect = true,
                    ImpactCategory = GHPC.Effects.ParticleEffectsManager.Category.HighExplosive,
                    EffectSize = GHPC.Effects.ParticleEffectsManager.EffectSize.MainGun,
                    Flags = GHPC.Effects.ParticleEffectsManager.ImpactModifierFlags.Large,
                    RicochetType = GHPC.Effects.ParticleEffectsManager.RicochetType.None,
                    MinFilterStrictness = GHPC.Effects.ParticleEffectsManager.FilterStrictness.Low
                };

                ammo_codex_m1 = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
                ammo_codex_m1.AmmoType = ammo_m1;
                ammo_codex_m1.name = "ammo_m1";

                clip_m1 = new AmmoType.AmmoClip();
                clip_m1.Capacity = 1;
                clip_m1.Name = "M1/M557 HE-PD";
                clip_m1.MinimalPattern = new AmmoCodexScriptable[1];
                clip_m1.MinimalPattern[0] = ammo_codex_m1;

                clip_codex_m1 = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
                clip_codex_m1.name = "clip_m1";
                clip_codex_m1.CompatibleWeaponSystems = new WeaponSystemCodexScriptable[1];
                clip_codex_m1.CompatibleWeaponSystems[0] = gun_m102;
                clip_codex_m1.ClipType = clip_m1;

                ammo_m81a1 = new AmmoType();
                Util.ShallowCopy(ammo_m81a1, ammo_m791);
                ammo_m81a1.Name = "M81A1 AP-T";
                ammo_m81a1.Caliber = 40;
                ammo_m81a1.RhaPenetration = 55f;
                ammo_m81a1.MuzzleVelocity = 850f;
                ammo_m81a1.Mass = 0.850f;
                ammo_m81a1.Coeff = 0.08f;
                ammo_m81a1.SectionalArea *= 3.5f;
                ammo_m81a1.UseTracer = true;

                ammo_codex_m81a1 = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
                ammo_codex_m81a1.AmmoType = ammo_m81a1;
                ammo_codex_m81a1.name = "ammo_m81a1";

                clip_m81a1 = new AmmoType.AmmoClip();
                clip_m81a1.Capacity = 2500;
                clip_m81a1.Name = "M81A1 AP-T";
                clip_m81a1.MinimalPattern = new AmmoCodexScriptable[1];
                clip_m81a1.MinimalPattern[0] = ammo_codex_m81a1;

                clip_codex_m81a1 = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
                clip_codex_m81a1.name = "clip_m81a1";
                clip_codex_m81a1.ClipType = clip_m81a1;
            }
        }
    }
}
