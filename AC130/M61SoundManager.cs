using FMOD;
using FMODUnity;
using GHPC.Camera;
using GHPC.Weapons;
using UnityEngine;
using HarmonyLib;

namespace ATLASMissionPack
{
    public class M61SoundManager : MonoBehaviour
    {
        public static bool sounds_created = false;
        public static FMOD.Sound sound_exterior;
        public static FMOD.Sound sound;
        public static FMOD.Sound sound_closing;

        public FMOD.ChannelGroup channel_group = new ChannelGroup();
        public FMOD.Channel channel = new FMOD.Channel();
        public FMOD.Channel channel_closing = new FMOD.Channel();

        public Transform audio_origin;

        [HarmonyPatch(typeof(GHPC.Weapons.WeaponSystem), "Fire")]
        public static class M61StartedFiring
        {
            public static bool Prefix(WeaponSystem __instance)
            {
                if (!__instance.AbleToFire) return true;

                if (__instance.Feed.AmmoTypeInBreech.Caliber == 105)
                {
                    CameraManager._mainCamera.GetComponent<CameraShake>().shakeAmount = 3.2f;
                    CameraManager._mainCamera.GetComponent<CameraShake>().shakeDuration = 0.25f;
                }

                M61SoundManager sound_manager = __instance.GetComponent<M61SoundManager>();

                if (sound_manager == null) return true;

                sound_manager.Fire();

                bool closing_playing;
                sound_manager.channel_closing.isPlaying(out closing_playing);
                if (closing_playing)
                {
                    sound_manager.channel_closing.stop();
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(GHPC.Weapons.WeaponSystem), "StopFiring")]
        public static class M61StoppedFiring
        {
            public static bool Prefix(WeaponSystem __instance)
            {
                M61SoundManager sound_manager = __instance.GetComponent<M61SoundManager>();

                if (sound_manager == null) return true;

                bool playing;
                sound_manager.channel.isPlaying(out playing);

                if (__instance.CurrentClipRemainingCount == 0 && !playing) return true;

                if (playing)
                    sound_manager.FireClosing();
                sound_manager.channel.stop();

                return true;
            }
        }

        [HarmonyPatch(typeof(WeaponAudio), "FinalStartLoop")]
        public static class ReplaceSound
        {
            public static bool Prefix(WeaponAudio __instance)
            {
                if (__instance.SingleShotMode && __instance.SingleShotEventPaths[0].Contains("m61"))
                {
                    return false;
                }

                return true;
            }
        }


        void Update()
        {
            Vector3 vec = audio_origin.position;

            VECTOR pos = new VECTOR();
            pos.x = vec.x;
            pos.y = vec.y;
            pos.z = vec.z;

            VECTOR vel = new VECTOR();
            vel.x = audio_origin.forward.x * 0f;
            vel.y = audio_origin.forward.y * 0f;
            vel.z = audio_origin.forward.z * 0f;

            channel.set3DAttributes(ref pos, ref vel);
            channel_closing.set3DAttributes(ref pos, ref vel);
        }

        void Awake()
        {
            var corSystem = RuntimeManager.CoreSystem;
            corSystem.createChannelGroup("master", out channel_group);
            corSystem.createChannelGroup("master", out channel_group);
        }

        public void FireClosing()
        {
            var corSystem = RuntimeManager.CoreSystem;

            Vector3 vec = audio_origin.position;

            VECTOR pos = new VECTOR();
            pos.x = vec.x;
            pos.y = vec.y;
            pos.z = vec.z;

            VECTOR vel = new VECTOR();
            vel.x = audio_origin.forward.x * 0f;
            vel.y = audio_origin.forward.y * 0f;
            vel.z = audio_origin.forward.z * 0f;

            //bool interior = __instance.IsInterior && __instance == PactIncreasedLethalityMod.player_manager.CurrentPlayerWeapon.Weapon.WeaponSound;

            channel_group.setVolumeRamp(true);
            channel_group.setMode(MODE._3D_WORLDRELATIVE);

            //FMOD.Channel channel;
            //FMOD.Sound sound_interior = __instance.SingleShotEventPaths[0].Contains("actually_2a72") ? sound_alt : sound;
            FMOD.Sound s = sound_closing; //? sound_interior : sound_exterior;
            channel_closing.setFrequency(48000f);
            channel_closing.setVolumeRamp(true);
            corSystem.playSound(s, channel_group, true, out channel_closing);
            /*
            channels.Add(channel);

            if (channels.Count > 1)
            {
                channels[1].stop();
                channels.RemoveAt(1);
            }
            */

            //float game_vol = PactIncreasedLethalityMod.audio_settings_manager._previousVolume;
            //float gun_vol = interior ? game_vol + 0.0185f * (game_vol * 10f) : game_vol;

            //channel.setVolume(gun_vol);
            channel_closing.set3DAttributes(ref pos, ref vel);
            channel_group.set3DAttributes(ref pos, ref vel);
            channel_closing.setPaused(false);
        }

        public void Fire()
        {
            channel.isPlaying(out bool playing);
            if (!playing)
            {
                var corSystem = RuntimeManager.CoreSystem;

                Vector3 vec = audio_origin.position;

                VECTOR pos = new VECTOR();
                pos.x = vec.x;
                pos.y = vec.y;
                pos.z = vec.z;

                VECTOR vel = new VECTOR();
                vel.x = audio_origin.forward.x * 0f;
                vel.y = audio_origin.forward.y * 0f;
                vel.z = audio_origin.forward.z * 0f;

                //bool interior = __instance.IsInterior && __instance == PactIncreasedLethalityMod.player_manager.CurrentPlayerWeapon.Weapon.WeaponSound;

                channel_group.setVolumeRamp(true);
                channel_group.setMode(MODE._3D_WORLDRELATIVE);

                //FMOD.Channel channel;
                //FMOD.Sound sound_interior = __instance.SingleShotEventPaths[0].Contains("actually_2a72") ? sound_alt : sound;
                FMOD.Sound s = sound; //? sound_interior : sound_exterior;

                channel.setFrequency(48000f);
                channel.setVolumeRamp(true);
                corSystem.playSound(s, channel_group, true, out channel);

                /*
                channels.Add(channel);

                if (channels.Count > 1)
                {
                    channels[1].stop();
                    channels.RemoveAt(1);
                }
                */

                //float game_vol = PactIncreasedLethalityMod.audio_settings_manager._previousVolume;
                //float gun_vol = interior ? game_vol + 0.0185f * (game_vol * 10f) : game_vol;

                //channel.setVolume(gun_vol);
                channel.set3DAttributes(ref pos, ref vel);
                channel_group.set3DAttributes(ref pos, ref vel);
                channel.setPaused(false);
            }
        }
    }
}
