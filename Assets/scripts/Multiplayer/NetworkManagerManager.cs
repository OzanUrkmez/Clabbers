using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerManager : NetworkManager{

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        if (Server.Players[conn.connectionId].ReadiedUp)
            Player.UnreadyPlayer();
        Server.UnRegisterPlayer(conn.connectionId);

        
    }

}
