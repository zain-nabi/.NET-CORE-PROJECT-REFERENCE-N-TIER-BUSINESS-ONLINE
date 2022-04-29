using System.Threading.Tasks;
using Triton.Model.CRM.Tables;

namespace Triton.Interface.CRM
{
    public interface ICollection
    {
        #region ===================Collections==============================
        Task<Collections> GetCollection(int collectionId, string dbName="CRM");
        Task<Collections> GetCollectionByNo(string CollectionNo, string dbName="CRM");
        Task<bool> Put(Collections collections, string dbName="CRM");
        Task<long> Post(Collections collections, string dbName="CRM");
        #endregion
    }
}