using Unity.Netcode;
using Newtonsoft.Json;

namespace HarnessTestBed.Data
{
    public struct PlayerState : INetworkSerializable
    {
        public string JsonPayload;

        public PlayerState(string json)
        {
            JsonPayload = json ?? "{}";
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref JsonPayload);
        }
    }
}
