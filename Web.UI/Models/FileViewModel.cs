using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using Web.UI.Services;

namespace Web.UI.Models
{
    public class FileViewModel
    {
        private enum ProcessStage
        {
            Start = 0,
            Init = 1,
            Validation = 2,
            SaveFile = 3,
            SaveReport = 4
        };

        const int NumberOfDays = 30;

        #region 01. Private properties
        private ProcessStage _currentStage = ProcessStage.Start;
        private IFormFile _formFile;
        #endregion

        #region 02. Public properties
        public string originalFileName { get; set; }
        public string tempFileName { get; set; }
        public ValidationResult validationResult { get; set; }
        public DailyStockPrice buyDayAndPrice { get; set; }
        public DailyStockPrice sellDayAndPrice { get; set; }
        #endregion

        #region 03. Constructors
        public FileViewModel()
        {
            this.originalFileName = "";
            this.tempFileName = "";
            this.validationResult = new ValidationResult();
            this.buyDayAndPrice = new DailyStockPrice();
            this.sellDayAndPrice = new DailyStockPrice();

            this._currentStage = ProcessStage.Start;
        }

        public FileViewModel(IFormFile fl)
        {
            this._formFile = fl;

            this.originalFileName = this._formFile.FileName;
            this.tempFileName = FileManager.GetTempFileName(this._formFile.FileName);
            this.validationResult = new ValidationResult();
            this.buyDayAndPrice = new DailyStockPrice();
            this.sellDayAndPrice = new DailyStockPrice();

            this._currentStage = ProcessStage.Init;
        }
        #endregion

        #region 04. Public methods
        public void ValidateData()
        {
            switch (this._currentStage)
            {
                case ProcessStage.Start:
                    throw new System.InvalidOperationException("Can't validate without initializing");
                case ProcessStage.Init:
                    string fileContents = FileManager.ReadFile(this._formFile);

                    DailyStockPrice[] parsedData;
                    this.validationResult = ParseData(fileContents, out parsedData);
                    if (this.validationResult.isValid)
                    {
                        DailyStockPrice[] orderedData = parsedData.OrderBy(d => d.price).ToArray();
                        this.buyDayAndPrice = orderedData.First();
                        this.sellDayAndPrice = orderedData.Last();
                    }

                    this._currentStage = ProcessStage.Validation;
                    break;
                case ProcessStage.Validation:
                case ProcessStage.SaveFile:
                case ProcessStage.SaveReport:
                default:
                    throw new System.InvalidOperationException("Validation done already");
            }            
        }

        public void SaveFile()
        {
            switch (this._currentStage)
            {
                case ProcessStage.Start:
                case ProcessStage.Init:
                    throw new System.InvalidOperationException("Can't save file without validating");
                case ProcessStage.Validation:
                    FileManager.SaveFile(this._formFile, this.tempFileName);

                    this._currentStage = ProcessStage.SaveFile;

                    break;
                case ProcessStage.SaveFile:
                case ProcessStage.SaveReport:
                default:
                    throw new System.InvalidOperationException("File saved already");
            }            
        }

        public void SaveReport()
        {
            switch (this._currentStage)
            {
                case ProcessStage.Start:
                case ProcessStage.Init:
                case ProcessStage.Validation:
                    throw new System.InvalidOperationException("Can't save report without saving the file");
                case ProcessStage.SaveFile:
                    string ReportData = JsonConvert.SerializeObject(this);

                    FileManager.SaveReport(ReportData, this.tempFileName);

                    this._currentStage = ProcessStage.SaveReport;

                    break;
                case ProcessStage.SaveReport:
                default:
                    throw new System.InvalidOperationException("Report saved already");
            }            
        }
        #endregion

        #region 05. Private methods
        private ValidationResult ParseData(string fileContents, out DailyStockPrice[] parsedData)
        {
            ValidationResult retVal = new ValidationResult();
            parsedData = new DailyStockPrice[NumberOfDays];

            string[] tokens = fileContents.Split(",");
            if (tokens.Length == NumberOfDays)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < tokens.Length; i++)
                {
                    decimal parsedDataCell = 0;
                    bool isParsable = decimal.TryParse(tokens[i], out parsedDataCell);
                    if (isParsable)
                    {
                        DailyStockPrice dailyStockPrice = new DailyStockPrice()
                        {
                            day = i + 1,
                            price = parsedDataCell
                        };
                        parsedData[i] = dailyStockPrice;
                    }
                    else
                    {
                        sb.AppendFormat("Column #{0}({1}),", i + 1, tokens[i]);
                    }                        
                }

                if (sb.Length > 0)
                {
                    string errorMsg = sb.ToString();

                    retVal.isValid = false;
                    retVal.message = string.Format(
                        "{0} {1} not parsable", 
                        errorMsg.Substring(0, errorMsg.Length-1),
                        sb.Length == 1 ? "is" : "are");
                } 
                else
                {
                    retVal.isValid = true;
                    retVal.message = "Data is valid";
                }
            } 
            else
            {
                retVal.isValid = false;
                retVal.message = string.Format("Please load {0} days' data", NumberOfDays);
            }

            return retVal;
        }
        #endregion
    }
}