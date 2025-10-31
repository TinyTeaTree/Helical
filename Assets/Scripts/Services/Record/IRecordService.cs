using System.Collections.Generic;
using Core;

namespace Services
{
    public interface IRecordService : IService
    {
        void SetUp(IEnumerable<BaseRecord> records);
        
        void PopulateRecord(string recordId, string recordJson);

        IEnumerable<BaseRecord> GetAllRecords();
    }
}