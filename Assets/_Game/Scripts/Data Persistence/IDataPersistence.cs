namespace DLS.Core.Data_Persistence
{
    public interface IDataPersistence : IID
    {
        void LoadData(GameData data);
        
        void SaveData(GameData data);
    }
}
