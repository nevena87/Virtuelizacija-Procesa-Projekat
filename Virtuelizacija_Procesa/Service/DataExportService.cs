using Common;
using Common.Interface;
using Common.Model;
using Common.Model.Enum;
using DataBase.InMemory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Service
{
    public class DataExportService : IDataExport
    {
        public delegate List<FileData> GenerateCSVDelegate(List<Load> loads);

        public void ProcessFile(FileData fileData)
        {
            try
            {
                List<Load> loads = ParseXmlFile(fileData);

                foreach (Load load in loads)
                {
                    bool isValid = ValidateLoadData(load);
                    if (isValid)
                    {
                        InMemoryLoad.Instance.UpdateLoad(load.Id, load);
                    }
                    else
                    {
                        CreateAuditForLoadError(load);
                    }
                }
                CreateImportedFile(fileData.FileName);
            }
            catch (Exception ex)
            {
                CreateAuditForFileError(fileData.FileName, ex.Message);
            }
        }

        public List<FileData> GetFiles()
        {
            GenerateCSVDelegate generateCSV;

            if (bool.Parse(ConfigurationManager.AppSettings["CreateMultipleCSVFiles"]))
            {
                generateCSV = GenerateMultipleCSVFiles;
            }
            else
            {
                generateCSV = GenerateSingleCSVFile;
            }

            List<FileData> files = generateCSV.Invoke(InMemoryLoad.Instance.GetLoadData().Values.ToList());

            
            return files;
        }
        private List<Load> ParseXmlFile(FileData fileData)
        {
            Load existing = null;
            List<Load> loads = new List<Load>();
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                fileData.FileStream.Position = 0;
                xmlDocument.Load(fileData.FileStream);

                XmlNodeList rowNodes = xmlDocument.SelectNodes("//row");

                foreach (XmlNode rowNode in rowNodes)
                {
                    string timestampString = rowNode.SelectSingleNode("TIME_STAMP")?.InnerText;
                    string forecastValueString = rowNode.SelectSingleNode("FORECAST_VALUE")?.InnerText;
                    string measuredValueString = rowNode.SelectSingleNode("MEASURED_VALUE")?.InnerText;

                    if (string.IsNullOrEmpty(timestampString) || string.IsNullOrEmpty(forecastValueString) || string.IsNullOrEmpty(measuredValueString))
                    {
                        CreateAuditForFileError(fileData.FileName, "Missing data in XML row.");
                        continue;
                    }

                    if (!DateTime.TryParse(timestampString, out DateTime timestamp) || !float.TryParse(forecastValueString, out float forecastValue) || !int.TryParse(measuredValueString, out int measuredValue))
                    {
                        CreateAuditForFileError(fileData.FileName, "Invalid data in XML row.");
                        continue;
                    }

                    existing = InMemoryLoad.Instance.GetLoadByTimeStamp(timestamp);
                    if(existing == null)
                    {
                        Load load = new Load(timestamp, forecastValue, measuredValue);
                        loads.Add(load);
                    }
                    else
                    {
                        existing.ForecastValue = forecastValue;
                        existing.MeasuredValue = measuredValue;
                        InMemoryLoad.Instance.UpdateLoad(existing.Id, existing);
                    }
                }
            }
            catch (Exception ex)
            {
                CreateAuditForFileError(fileData.FileName, $"Error parsing XML file: {ex.Message}");
            }

            return loads;
        }


        private bool ValidateLoadData(Load load)
        {
            if (load.ForecastValue <= 0 || load.MeasuredValue <= 0)
            {
                return false;
            }

            return true;
        }

        private void CreateAuditForLoadError(Load load)
        {
            string message = $"Invalid data for load with Id: {load.Id}";
            Audit audit = new Audit(DateTime.Now, MessageType.Error, message);

            InMemoryAudit.Instance.InsertAudit(audit.Id, audit);
        }

        public List<FileData> GenerateMultipleCSVFiles(List<Load> loads)
        {
            List<FileData> fileDataList = new List<FileData>();

            foreach (var date in loads.Select(l => l.TimeStamp.Date).Distinct())
            {
                List<Load> loadsForDate = loads.Where(l => l.TimeStamp.Date == date).ToList();
                string csvFileName = $"{date.ToString("yyyy-MM-dd")}.csv";

                var memoryStream = new MemoryStream();
                using (var writer = new StreamWriter(memoryStream))
                {
                    WriteCSVData(writer, loadsForDate);
                    writer.Flush();
                }

                var memoryStreamCopy = new MemoryStream(memoryStream.ToArray());
                fileDataList.Add(new FileData(memoryStreamCopy, csvFileName));

            }

            return fileDataList;
        }
        public List<FileData> GenerateSingleCSVFile(List<Load> loads)
        {
            var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream))
            {
                WriteCSVData(writer, loads);
                writer.Flush();
            }

            var memoryStreamCopy = new MemoryStream(memoryStream.ToArray());
            return new List<FileData> { new FileData(memoryStreamCopy, "all_dates.csv") };

        }

        private void WriteCSVData(StreamWriter writer, List<Load> loads)
        {
            writer.WriteLine("DATE,TIME,FORECAST_VALUE,MEASURED_VALUE");

            foreach (Load load in loads)
            {
                string date = load.TimeStamp.ToString("yyyy-MM-dd");
                string time = load.TimeStamp.ToString("HH:mm");
                string forecastValue = load.ForecastValue.ToString();
                string measuredValue = load.MeasuredValue.ToString();

                string csvLine = $"{date},{time},{forecastValue},{measuredValue}";
                writer.WriteLine(csvLine);
            }
        }

        private void CreateImportedFile(string fileName)
        {
            ImportedFile importedFile = new ImportedFile(fileName);

            InMemoryImpFile.Instance.InsertImportedFile(importedFile.Id, importedFile);
        }


        private void CreateAuditForFileError(string fileName, string errorMessage)
        {
            string message = $"Error processing file: {fileName}. {errorMessage}";
            Audit audit = new Audit(DateTime.Now, MessageType.Error, message);

            InMemoryAudit.Instance.InsertAudit(audit.Id, audit);
        }

        public List<Audit> GetAudits()
        {
            return InMemoryAudit.Instance.GetAuditData().Values.ToList();
        }

        public List<ImportedFile> GetImportedFiles()
        {
            return InMemoryImpFile.Instance.GetImportedFileData().Values.ToList();
        }
    }
}
