using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QConsoleWeb.DAL.AccessLayer.Entities;

namespace QConsoleWeb.DAL.AccessLayer.Interfaces
{
    public interface ILayerDAO
    {
        //layers
        List<Layer> GetLayers();
        //dictionaries
        List<Layer> GetDicts();
        //Change layer
        void ChangeLayer(string tableschema, string tablename, string descript, bool? isupdater, bool? islogger, string nameDocFilesTable);
        //Get count of changed rows in last days
        int GetCountOfPeriod(string tableshcema, string tablename, int days);
        //Get all non-geometry tables from information_schema
        List<InformationSchemaTable> GetAllTables();
        List<LayerGrants> GetGrantsToLayer(string schemaname, string tablename);
    }
}
