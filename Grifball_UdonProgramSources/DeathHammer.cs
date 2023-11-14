﻿using Cyan.PlayerObjectPool;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Cekay.Grifball
{
    public class DeathHammer : UdonSharpBehaviour
    {
        public SettingsPage Settings;
        public Combat CombatScript;
        public CyanPlayerObjectAssigner ObjAssign;
        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (player != Settings.LocalPlayer)
            {
                UdonBehaviour targetScript = (UdonBehaviour)ObjAssign._GetPlayerPooledUdon(player);

                string playerName = (string)targetScript.GetProgramVariable("LocalPlayerName");

                if ((Settings.BlueTeam.Contains(playerName) && (CombatScript.CurrentTeam == "Red")) ||
                    (Settings.RedTeam.Contains(playerName) && (CombatScript.CurrentTeam == "Blue")))
                {
                    targetScript.SendCustomEvent("Die");
                    Settings.AnnouncerAudio.PlayOneShot(CombatScript.Hit);
                }
                else
                {
                    if (Settings.FriendlyFire)
                    {
                        if ((Settings.BlueTeam.Contains(playerName) && (CombatScript.CurrentTeam == "Blue")) ||
                            (Settings.RedTeam.Contains(playerName) && (CombatScript.CurrentTeam == "Red")))
                        {
                            targetScript.SendCustomEvent("Die");
                            targetScript.SendCustomEvent("KillBetrayed");
                            Settings.AnnouncerAudio.PlayOneShot(CombatScript.Betrayal);
                        }
                    }
                }
            }
        }

        // For single player debug in-editor
        public void OnTriggerEnter(Collider other)
        {
            if ((other.gameObject.layer == 29) && (CombatScript.CurrentTeam == "Red") ||
                (other.gameObject.layer == 30) && (CombatScript.CurrentTeam == "Blue"))
            {
                Settings.AnnouncerAudio.PlayOneShot(CombatScript.Hit);
            }
            else
            {
                if (Settings.FriendlyFire)
                {
                    if ((other.gameObject.layer == 30) && (CombatScript.CurrentTeam == "Red") ||
                        (other.gameObject.layer == 29) && (CombatScript.CurrentTeam == "Blue"))
                    {
                        Settings.AnnouncerAudio.PlayOneShot(CombatScript.Betrayal);
                    }
                }
            }
        }
    }
}