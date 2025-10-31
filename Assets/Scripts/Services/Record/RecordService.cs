using System;
using System.Collections.Generic;
using System.Linq;
using Core;

namespace Services
{
    public class RecordService : BaseService, IRecordService
    {
        private Dictionary<string, BaseRecord> _records;
        
        public void SetUp(IEnumerable<BaseRecord> records)
        {
            _records = records.ToDictionary(record => record.Id, record => record);
        }

        public void PopulateRecord(string recordId, string recordJson)
        {
            _records[recordId].Populate(recordJson);
        }

        public IEnumerable<BaseRecord> GetAllRecords()
        {
            return _records.Values;
        }
    }
}