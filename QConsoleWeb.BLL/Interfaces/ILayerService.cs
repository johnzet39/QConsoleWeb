using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QConsoleWeb.BLL.DTO;

namespace QConsoleWeb.BLL.Interfaces
{
    public interface ILayerService
    {
        //layers
        List<LayerDTO> GetLayers();
        //dictionaries
        List<LayerDTO> GetDicts();
        //Change layer
        void ChangeLayer(string tableschema, string tablename, string descript, bool? isupdater, bool? islogger);
        //Get count of changed rows in last days
        int GetCountOfPeriod(string tableshcema, string tablename, int days);
        //GetListOfDictionaries
        List<DictionaryDTO> GetListOfDictionaries();
        //GetListOfAllTAbles
        List<InformationSchemaTableDTO> GetListOfAllTables();
        //Add table name in list of dictionaries (schema_spr.dictionaries)
        void AddTableToDictionaries(string schemaname, string tablename);
        //Remove table from list of dictionaries (schema_spr.dictionaries)
        void RemoveTableFromDictionaries(int id);
    }
}
