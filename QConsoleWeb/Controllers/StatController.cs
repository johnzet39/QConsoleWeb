using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QConsoleWeb.BLL.DTO;
using QConsoleWeb.BLL.Interfaces;
using QConsoleWeb.BLL.Services;
using QConsoleWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Controllers
{
    [Authorize]
    public class StatController : Controller
    {
        private ILayerService _layerService;
        private ILoggerService _loggerService;
        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };

        static string[] ColorValues = new string[] {
            "FF0000", "00FF00", "0000FF", "FFFF00", "FF00FF", "00FFFF", "000000",
            "800000", "008000", "000080", "808000", "800080", "008080", "808080",
            "C00000", "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0",
            "400000", "004000", "000040", "404000", "400040", "004040", "404040",
            "200000", "002000", "000020", "202000", "200020", "002020", "202020",
            "600000", "006000", "000060", "606000", "600060", "006060", "606060",
            "A00000", "00A000", "0000A0", "A0A000", "A000A0", "00A0A0", "A0A0A0",
            "E00000", "00E000", "0000E0", "E0E000", "E000E0", "00E0E0", "E0E0E0",
        };

        public StatController(ILoggerService loggerServ, ILayerService layerServ)
        {
            _loggerService = loggerServ;
            _layerService = layerServ;
        }

        private List<Layer> GetLayers()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<LayerDTO, Layer>()).CreateMapper();
            return mapper.Map<IEnumerable<LayerDTO>, List<Layer>>(_layerService.GetLayers());
        }

        public IActionResult Index()
        {
            ViewBag.Title = "Статистика";
            return View();
        }

        public IActionResult GetOperationsCount(string datefrom, string dateto)
        {
            var layerNameList = GetLayers().Select(o => $"{o.Table_schema}.{o.Table_name}");


            DateTime DateFrom = DateTime.ParseExact(datefrom, "yyyy-MM", System.Globalization.CultureInfo.InvariantCulture);
            DateTime DateTo = DateTime.ParseExact(dateto, "yyyy-MM", System.Globalization.CultureInfo.InvariantCulture).AddMonths(1);

            var logList = _loggerService.GetAllLogByPeriod(DateFrom, DateTo);
            var inserts = logList.Where(o => o.Action == "INSERT" && layerNameList.Contains($"{o.Tableschema}.{o.Tablename}") ).Count();
            var updates = logList.Where(o => o.Action == "UPDATE" && layerNameList.Contains($"{o.Tableschema}.{o.Tablename}")).Count();
            var deletes = logList.Where(o => o.Action == "DELETE" && layerNameList.Contains($"{o.Tableschema}.{o.Tablename}")).Count();

            string[] labels = new string[3] { $"INSERTS({inserts})", $"UPDATES({updates})", $"DELETES({deletes})" };
            int[] dataset = new int[3] { inserts, updates, deletes };
            string[] colors = new string[3] { "rgb(227, 26, 28, 0.5)", "rgb(31, 120, 180, 0.5)", "rgb(255, 177, 0, 0.5)" };

            ViewBag.StatOperations_labels = JsonConvert.SerializeObject(labels, _jsonSetting);
            ViewBag.StatOperations_dataset = JsonConvert.SerializeObject(dataset, _jsonSetting);
            ViewBag.StatOperations_colors = JsonConvert.SerializeObject(colors, _jsonSetting);

            return PartialView();
        }

        public IActionResult GetInsertsCount(string datefrom, string dateto)
        {
            DateTime DateFrom = DateTime.ParseExact(datefrom, "yyyy-MM", System.Globalization.CultureInfo.InvariantCulture);
            DateTime DateTo = DateTime.ParseExact(dateto, "yyyy-MM", System.Globalization.CultureInfo.InvariantCulture).AddMonths(1);

            var layerList = GetLayers();

            List<string> labels = new List<string>();
            List<int> dataset = new List<int>();
            List<string> colors = new List<string>();

            var logList = _loggerService.GetAllLogByPeriod(DateFrom, DateTo)
                .Where(o => o.Action.ToUpper() == "INSERT");

            int idx = 0;
            foreach (Layer layer in layerList)
            {
                //int count = _loggerService.GetCountInserts(layer.Table_schema, layer.Table_name, DateFrom, DateTo);
                int count = logList
                        .Where(o => o.Tablename == layer.Table_name)
                        .Where(o => o.Tableschema == layer.Table_schema)
                        .Count();
                if (count > 0)
                {
                    labels.Add($"{layer.Table_schema}.{layer.Table_name} ({count})");
                    dataset.Add(count);

                    if (idx > ColorValues.Length - 1)
                        idx = 0;
                    colors.Add("#"+ColorValues[idx]+"77");
                    ++idx;
                }
            }

            ViewBag.StatInserts_labels = JsonConvert.SerializeObject(labels, _jsonSetting);
            ViewBag.StatInserts_dataset = JsonConvert.SerializeObject(dataset, _jsonSetting);
            ViewBag.StatInserts_colors = JsonConvert.SerializeObject(colors, _jsonSetting);

            return PartialView();
        }

        public IActionResult GetYearStatDataCJ(string datefrom, string dateto)
        {
            DateTime DateFrom = DateTime.ParseExact(datefrom, "yyyy-MM", System.Globalization.CultureInfo.InvariantCulture);
            DateTime DateTo = DateTime.ParseExact(dateto, "yyyy-MM", System.Globalization.CultureInfo.InvariantCulture);
            int period_months = MonthDifference(DateFrom, DateTo);

            List<Layer> layerList = GetLayers();

            List<Tuple<int, int>>  dateList = new List<Tuple<int, int>>();
            List<string> LabelsYears = new List<string>();
            for (int i = period_months; i >= 0; i--)
            {
                DateTime date = DateTo.AddMonths(-i);
                int month = date.Month;
                int year = date.Year;
                dateList.Add(new Tuple<int, int>(month, year));
            }
            for (int j = 0; j < dateList.Count; j++)
            {
                LabelsYears.Add(dateList[j].Item1 + "." + dateList[j].Item2);
            }

            var logList = _loggerService.GetAllLogByPeriod(DateFrom, DateTo.AddMonths(1))
                .Where(o => o.Action.ToUpper() == "INSERT");

            int idx = 0;
            List<SerieChartJs> datasets = new List<SerieChartJs>();

            foreach (Layer layer in layerList)
            {
                List<int> values = new List<int>();
                bool isHasData = false;
                foreach (var date in dateList)
                {
                    //int count = _loggerService.GetCountInsertsMonth(layer.Table_schema, layer.Table_name, date.Item1, date.Item2);
                    int count = logList
                        .Where(o => o.Tablename == layer.Table_name)
                        .Where(o => o.Tableschema == layer.Table_schema)
                        .Where(o => o.Timechange.Year == date.Item2 && o.Timechange.Month == date.Item1)
                        .Count();

                    values.Add(count);
                    if (count > 0)
                        isHasData = true;
                }

                if (isHasData)
                {
                    if (idx > ColorValues.Length - 1)
                        idx = 0;
                    var color = ColorValues[idx];

                    SerieChartJs serieChartJs = new SerieChartJs()
                    {
                        data = values,
                        label = $"{layer.Table_schema}.{layer.Table_name}",
                        borderColor =  "#" + color + "CC",
                        backgroundColor = "#" + color + "33",
                        borderWidth = 1.5,
                        pointRadius = 3,
                        pointStyle = "rectRot"
                    };
                    datasets.Add(serieChartJs);
                    ++idx;
                }
            }
            DataChartJs datachartjs = new DataChartJs()
            {
                labels = LabelsYears,
                datasets = datasets
            };
            ViewBag.YearStatDataCJ = JsonConvert.SerializeObject(datachartjs, _jsonSetting);
            return PartialView();
        }

        private int MonthDifference(DateTime lValue, DateTime rValue)
        {
            return Math.Abs((lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year));
        }
    }
}
