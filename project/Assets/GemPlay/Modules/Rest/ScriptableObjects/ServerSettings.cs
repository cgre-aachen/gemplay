using UnityEngine;


namespace GemPlay.Modules.Rest.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ServerSettings", menuName = "ScriptableObjects/GemPlay/Rest/ServerSettings", order = 1)]
    public class ServerSettings : ScriptableObject
    {
        public string host;

        public int port;
    }
}
