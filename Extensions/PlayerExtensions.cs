/*
 * Seralyth Menu  Extensions/PlayerExtensions.cs
 * A community driven mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2026  Seralyth Software
 * https://github.com/Seralyth/Seralyth-Menu
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using ExitGames.Client.Photon;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Extensions
{
    public static class PlayerExtensions
    {
        #region NetPlayer

        public static Player GetPlayer(this NetPlayer player) =>
            player != null ? RigUtilities.NetPlayerToPlayer(player) : null;

        public static VRRig VRRig(this NetPlayer player) =>
            player != null ? RigUtilities.GetVRRigFromPlayer(player) : null;

        public static bool InRoom(this NetPlayer player) =>
            player != null &&
            NetworkSystem.Instance != null &&
            NetworkSystem.Instance.AllNetPlayers != null &&
            NetworkSystem.Instance.AllNetPlayers.Contains(player);

        public static Hashtable GetCustomProperties(this NetPlayer player)
        {
            var photonPlayer = player?.GetPlayer();
            return photonPlayer?.CustomProperties ?? new Hashtable();
        }

        #endregion

        #region Player

        public static VRRig VRRig(this Player player) =>
            player != null ? RigUtilities.GetVRRigFromPlayer(player) : null;

        public static bool InRoom(this Player player) =>
            player != null &&
            PhotonNetwork.InRoom &&
            PhotonNetwork.PlayerList != null &&
            PhotonNetwork.PlayerList.Contains(player);

        #endregion

        #region GorillaTagger

        public static bool IsGrounded(this GorillaTagger tagger, float maxDistance = 0.15f)
        {
            if (tagger == null || tagger.bodyCollider == null || GTPlayer.Instance == null)
                return false;

            Vector3 origin = tagger.bodyCollider.transform.position - new Vector3(0f, 0.2f, 0f);

            return Physics.Raycast(
                origin,
                Vector3.down,
                maxDistance,
                GTPlayer.Instance.locomotionEnabledLayers
            );
        }

        #endregion
    }
}
