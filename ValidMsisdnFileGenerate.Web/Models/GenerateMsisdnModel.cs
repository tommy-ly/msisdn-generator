namespace ValidMsisdnFileGenerate.Web.Models
{
    public class GenerateMsisdnModel
    {
        public int MsisdnCount { get; set; }
        public string Filename { get; set; }
        public string AdditionalMsisdns { get; set; }
        public bool RandomData { get; set; }
        public bool PrefixPlus { get; set; }
        public bool IncludeBadMsisdn { get; set; }
    }
}
