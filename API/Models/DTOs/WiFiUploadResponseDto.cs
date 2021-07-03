using System.Collections.Generic;

namespace HerstAPI.Models.DTOs
{

    public class WiFiUploadResponseDto
    {
        public int TotalLineCount { get; set; }
        public int ValidEntries {get;set;}
        public int NewEntries {get;set;}
        public int Beacons { get; set; }
        public int Probes { get; set; }
        public List<string> InvalidEntries { get; set; }
    }
}